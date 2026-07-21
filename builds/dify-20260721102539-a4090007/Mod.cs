using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using GTA;
using GTA.Native;

namespace ModProject
{
    // ──────────────────────────────────────────────
    // 事件数据结构
    // ──────────────────────────────────────────────
    public class ChatEvent
    {
        public string MsgId { get; set; }
        public long CreateTime { get; set; }
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public string Content { get; set; }
    }

    public class GiftEvent
    {
        public string MsgId { get; set; }
        public long CreateTime { get; set; }
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public int GiftId { get; set; }
        public string GiftName { get; set; }
        public int DiamondCount { get; set; }
        public int RepeatCount { get; set; }
        public bool RepeatEnd { get; set; }
        public int ComboCount { get; set; }
    }

    // ──────────────────────────────────────────────
    // JSON 解析器：只使用 JavaScriptSerializer
    // ──────────────────────────────────────────────
    public class LiveStudioParser
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

        public object Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            Dictionary<string, object> root;
            try
            {
                root = _serializer.Deserialize<Dictionary<string, object>>(json);
            }
            catch
            {
                return null;
            }

            if (root == null || !root.ContainsKey("data"))
                return null;

            var data = root["data"] as Dictionary<string, object>;
            if (data == null || !data.ContainsKey("common"))
                return null;

            var common = data["common"] as Dictionary<string, object>;
            if (common == null || !common.ContainsKey("method"))
                return null;

            string method = common["method"] as string;
            if (string.IsNullOrEmpty(method))
                return null;

            switch (method)
            {
                case "WebcastChatMessage":
                    return ParseChat(data, common);
                case "WebcastGiftMessage":
                    return ParseGift(data, common);
                default:
                    return null;
            }
        }

        private ChatEvent ParseChat(Dictionary<string, object> data, Dictionary<string, object> common)
        {
            var chat = new ChatEvent();
            chat.MsgId = SafeGetString(common, "msgId");
            chat.CreateTime = SafeGetLong(common, "createTime");

            if (data.ContainsKey("user"))
            {
                var user = data["user"] as Dictionary<string, object>;
                if (user != null)
                {
                    chat.UserId = SafeGetString(user, "id");
                    chat.Nickname = SafeGetString(user, "nickname");
                }
            }

            chat.Content = SafeGetString(data, "content");
            return chat;
        }

        private GiftEvent ParseGift(Dictionary<string, object> data, Dictionary<string, object> common)
        {
            var gift = new GiftEvent();
            gift.MsgId = SafeGetString(common, "msgId");
            gift.CreateTime = SafeGetLong(common, "createTime");

            if (data.ContainsKey("user"))
            {
                var user = data["user"] as Dictionary<string, object>;
                if (user != null)
                {
                    gift.UserId = SafeGetString(user, "id");
                    gift.Nickname = SafeGetString(user, "nickname");
                }
            }

            if (data.ContainsKey("gift"))
            {
                var giftObj = data["gift"] as Dictionary<string, object>;
                if (giftObj != null)
                {
                    gift.GiftId = SafeGetInt(giftObj, "id");
                    gift.GiftName = SafeGetString(giftObj, "name");
                    gift.DiamondCount = SafeGetInt(giftObj, "diamondCount");
                }
            }

            gift.RepeatCount = SafeGetInt(data, "repeatCount");
            gift.RepeatEnd = SafeGetBool(data, "repeatEnd");
            gift.ComboCount = SafeGetInt(data, "comboCount");
            return gift;
        }

        // ── 安全类型转换（单条坏消息绝不终止接收循环）──
        private static string SafeGetString(Dictionary<string, object> dict, string key, string defaultValue = "")
        {
            if (dict.TryGetValue(key, out object value) && value != null)
            {
                try { return value.ToString(); }
                catch { return defaultValue; }
            }
            return defaultValue;
        }

        private static int SafeGetInt(Dictionary<string, object> dict, string key, int defaultValue = 0)
        {
            if (dict.TryGetValue(key, out object value) && value != null)
            {
                try { return Convert.ToInt32(value); }
                catch { return defaultValue; }
            }
            return defaultValue;
        }

        private static long SafeGetLong(Dictionary<string, object> dict, string key, long defaultValue = 0L)
        {
            if (dict.TryGetValue(key, out object value) && value != null)
            {
                try { return Convert.ToInt64(value); }
                catch { return defaultValue; }
            }
            return defaultValue;
        }

        private static bool SafeGetBool(Dictionary<string, object> dict, string key, bool defaultValue = false)
        {
            if (dict.TryGetValue(key, out object value) && value != null)
            {
                try { return Convert.ToBoolean(value); }
                catch { return defaultValue; }
            }
            return defaultValue;
        }
    }

    // ──────────────────────────────────────────────
    // WebSocket 客户端：连接、订阅、分片接收、取消、重连
    // ──────────────────────────────────────────────
    public class LiveStudioClient
    {
        private const string ServerUrl = "ws://127.0.0.1:60080";
        private const int ReconnectDelayMs = 5000;
        private const int ReceiveBufferSize = 4096;

        private ClientWebSocket _ws;
        private CancellationTokenSource _cts;
        private Task _receiveTask;

        public event Action<ChatEvent> OnChat;
        public event Action<GiftEvent> OnGift;

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _receiveTask = Task.Run(() => ReceiveLoopAsync(_cts.Token));
        }

        public void Stop()
        {
            try { _cts?.Cancel(); } catch { }
            // 不等待，不阻塞 GTA 主线程；后台线程自行完成清理
        }

        private async Task ReceiveLoopAsync(CancellationToken ct)
        {
            var parser = new LiveStudioParser();

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using (_ws = new ClientWebSocket())
                    {
                        // 连接
                        await _ws.ConnectAsync(new Uri(ServerUrl), ct);

                        // 精确订阅（LIVE_STUDIO_CONTRACT_V1 第2条）
                        string subscribeJson =
                            "{\"type\":\"subscribe\",\"id\":\"gta-mod\",\"data\":{\"type\":\"serviceSignalSub\",\"name\":\"IM_MESSAGE_TRANSPORT\"}}";
                        byte[] subBytes = Encoding.UTF8.GetBytes(subscribeJson);
                        await _ws.SendAsync(
                            new ArraySegment<byte>(subBytes),
                            WebSocketMessageType.Text,
                            true,
                            ct);

                        // 接收循环（分片拼接）
                        var buffer = new byte[ReceiveBufferSize];
                        var messageBuilder = new StringBuilder();

                        while (_ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
                        {
                            WebSocketReceiveResult result;
                            messageBuilder.Clear();

                            do
                            {
                                result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);

                                if (result.MessageType == WebSocketMessageType.Close)
                                    break;

                                if (result.MessageType == WebSocketMessageType.Text)
                                {
                                    messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                                }
                            }
                            while (!result.EndOfMessage && _ws.State == WebSocketState.Open && !ct.IsCancellationRequested);

                            if (result.MessageType == WebSocketMessageType.Close)
                                break;

                            if (messageBuilder.Length > 0)
                            {
                                ProcessMessage(parser, messageBuilder.ToString());
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    // 异常隔离：任何网络/解析错误都不终止循环
                }

                // 保守重连延迟
                if (!ct.IsCancellationRequested)
                {
                    try { await Task.Delay(ReconnectDelayMs, ct); }
                    catch (OperationCanceledException) { break; }
                }
            }
        }

        private void ProcessMessage(LiveStudioParser parser, string json)
        {
            try
            {
                object result = parser.Parse(json);
                if (result is ChatEvent chat)
                {
                    OnChat?.Invoke(chat);
                }
                else if (result is GiftEvent gift)
                {
                    OnGift?.Invoke(gift);
                }
            }
            catch
            {
                // 单条坏消息绝不终止接收循环
            }
        }
    }

    // ──────────────────────────────────────────────
    // 主线程调度器：线程安全 bounded queue
    // ──────────────────────────────────────────────
    public class MainThreadDispatcher
    {
        private const int MaxQueueSize = 500;
        private const int MaxDrainPerTick = 50;

        private readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();
        private volatile int _count;

        public void Enqueue(Action action)
        {
            if (_count >= MaxQueueSize)
                return;

            _queue.Enqueue(action);
            Interlocked.Increment(ref _count);
        }

        public void Drain()
        {
            int processed = 0;
            while (processed < MaxDrainPerTick && _queue.TryDequeue(out Action action))
            {
                Interlocked.Decrement(ref _count);
                try
                {
                    action();
                }
                catch
                {
                    // 隔离单个 Action 异常
                }
                processed++;
            }
        }
    }

    // ──────────────────────────────────────────────
    // Mod 主类：状态机 + 玫瑰生存玩法
    // ──────────────────────────────────────────────
    public class Mod : Script
    {
        // ── 状态机 ──
        private enum ModState
        {
            Idle,
            SurvivalActive,
            Cooldown
        }

        // ── 常量 ──
        private const int TargetGiftId = 5655;
        private const string TargetGiftName = "Rose";
        private const double SurvivalDurationSeconds = 300.0;
        private const double NpcRefreshIntervalSeconds = 10.0;
        private const double CooldownDurationSeconds = 120.0;
        private const float NpcScanRadius = 200f;
        private const int MaxNpcPerRefresh = 50;
        private const int MsgIdPruneThreshold = 1000;

        // ── 核心组件 ──
        private readonly MainThreadDispatcher _dispatcher;
        private readonly LiveStudioClient _client;

        // ── 状态字段 ──
        private ModState _state = ModState.Idle;
        private readonly HashSet<string> _processedMsgIds = new HashSet<string>();
        private readonly object _msgIdLock = new object();
        private int _originalWantedLevel;
        private readonly HashSet<int> _trackedPedHandles = new HashSet<int>();
        private DateTime _survivalEndTime;
        private DateTime _nextNpcRefreshTime;
        private DateTime _cooldownEndTime;

        // ── 构造 ──
        public Mod()
        {
            _dispatcher = new MainThreadDispatcher();

            _client = new LiveStudioClient();
            _client.OnChat += OnChatReceived;
            _client.OnGift += OnGiftReceived;
            _client.Start();

            Tick += OnTick;
            Aborted += OnAborted;
        }

        // ── Tick ──
        private void OnTick(object sender, EventArgs e)
        {
            _dispatcher.Drain();

            switch (_state)
            {
                case ModState.SurvivalActive:
                    TickSurvival();
                    break;

                case ModState.Cooldown:
                    TickCooldown();
                    break;

                default:
                    break;
            }
        }

        private void TickSurvival()
        {
            // 玩家死亡检测
            if (Game.Player.Character.IsDead)
            {
                EnterSurvivalFailed();
                return;
            }

            // 生存时间到期
            if (DateTime.UtcNow >= _survivalEndTime)
            {
                EnterSurvivalSuccess();
                return;
            }

            // 定时刷新 NPC 仇恨
            if (DateTime.UtcNow >= _nextNpcRefreshTime)
            {
                RefreshNpcAggro();
                _nextNpcRefreshTime = DateTime.UtcNow.AddSeconds(NpcRefreshIntervalSeconds);
            }
        }

        private void TickCooldown()
        {
            if (DateTime.UtcNow >= _cooldownEndTime)
            {
                _state = ModState.Idle;
            }
        }

        // ── 事件回调（后台线程，只做轻量处理）──
        private void OnChatReceived(ChatEvent chat)
        {
            // DESIGN.md 未要求聊天玩法，保留 Hook 但不做任何事
        }

        private void OnGiftReceived(GiftEvent gift)
        {
            // 快速过滤：非目标礼物直接丢弃
            if (gift.GiftId != TargetGiftId || gift.GiftName != TargetGiftName)
                return;

            // 仅一次性触发
            if (!gift.RepeatEnd)
                return;

            // msgId 去重（线程安全）
            lock (_msgIdLock)
            {
                if (_processedMsgIds.Contains(gift.MsgId))
                    return;
                _processedMsgIds.Add(gift.MsgId);
            }

            // 入队主线程处理
            _dispatcher.Enqueue(() => TryTriggerSurvival(gift));
        }

        // ── 触发判定（主线程）──
        private void TryTriggerSurvival(GiftEvent gift)
        {
            // msgId 集合修剪
            if (_processedMsgIds.Count > MsgIdPruneThreshold)
            {
                _processedMsgIds.Clear();
            }

            // 忙时策略：仅在 Idle 状态触发
            if (_state != ModState.Idle)
                return;

            // 二次校验（防御）
            if (gift.GiftId != TargetGiftId || gift.GiftName != TargetGiftName)
                return;
            if (!gift.RepeatEnd)
                return;

            EnterSurvivalStart();
        }

        // ── 状态转换 ──
        private void EnterSurvivalStart()
        {
            try
            {
                // 保存原始通缉等级
                _originalWantedLevel = Game.Player.WantedLevel;

                // 清空追踪
                _trackedPedHandles.Clear();

                // 5 星通缉
                Game.Player.WantedLevel = 5;

                // 给予武器
                GiveWeaponToPlayer();

                // 初始 NPC 仇恨
                RefreshNpcAggro();

                // 设置计时
                _survivalEndTime = DateTime.UtcNow.AddSeconds(SurvivalDurationSeconds);
                _nextNpcRefreshTime = DateTime.UtcNow.AddSeconds(NpcRefreshIntervalSeconds);

                _state = ModState.SurvivalActive;

                GTA.UI.Notification.Show("玫瑰生存：5星通缉！存活5分钟！");
            }
            catch
            {
                // 初始化失败则回到 Idle
                _state = ModState.Idle;
            }
        }

        private void EnterSurvivalSuccess()
        {
            CleanupSurvival();
            GTA.UI.Notification.Show("玫瑰生存：你成功存活了5分钟！");
            _cooldownEndTime = DateTime.UtcNow.AddSeconds(CooldownDurationSeconds);
            _state = ModState.Cooldown;
        }

        private void EnterSurvivalFailed()
        {
            CleanupSurvival();
            GTA.UI.Notification.Show("玫瑰生存：你被击杀了...");
            _cooldownEndTime = DateTime.UtcNow.AddSeconds(CooldownDurationSeconds);
            _state = ModState.Cooldown;
        }

        // ── 核心效果 ──
        private void GiveWeaponToPlayer()
        {
            try
            {
                var player = Game.Player.Character;
                if (player == null || !player.Exists())
                    return;

                // Fallback：使用 native 给予武器（DESIGN.md 确认降级方案）
                Function.Call(Hash.GIVE_WEAPON_TO_PED, player, (int)WeaponHash.SMG, 1000, false, true);
            }
            catch
            {
                // 给予武器失败不影响其他效果
            }
        }

        private void RefreshNpcAggro()
        {
            try
            {
                var player = Game.Player.Character;
                if (player == null || !player.Exists())
                    return;

                // 主方案：附近 200 米 NPC
                Ped[] nearbyPeds = World.GetNearbyPeds(player, NpcScanRadius, null);

                // 降级：如果附近没有 Ped，尝试全局获取
                if (nearbyPeds == null || nearbyPeds.Length == 0)
                {
                    nearbyPeds = World.GetAllPeds(null);
                }

                if (nearbyPeds == null || nearbyPeds.Length == 0)
                    return;

                int count = 0;
                foreach (var ped in nearbyPeds)
                {
                    if (count >= MaxNpcPerRefresh)
                        break;
                    if (ped == null || !ped.Exists())
                        continue;
                    if (ped == player)
                        continue;
                    if (!ped.IsAlive)
                        continue;
                    if (ped.IsPlayer)
                        continue;

                    try
                    {
                        ped.Task.FightAgainst(player);
                        _trackedPedHandles.Add(ped.Handle);
                        count++;
                    }
                    catch
                    {
                        // 单个 Ped 失败不影响其他
                    }
                }
            }
            catch
            {
                // 整轮刷新失败则下一轮重试
            }
        }

        // ── 清理 ──
        private void CleanupSurvival()
        {
            try
            {
                // 恢复通缉等级（不无条件设为 0）
                Game.Player.WantedLevel = _originalWantedLevel;
            }
            catch { }

            // 清理 NPC 攻击任务
            foreach (int handle in _trackedPedHandles)
            {
                try
                {
                    var ped = Entity.FromHandle(handle) as Ped;
                    if (ped != null && ped.Exists())
                    {
                        try
                        {
                            ped.Task.ClearAll();
                        }
                        catch
                        {
                            // Native fallback
                            Function.Call(Hash.CLEAR_PED_TASKS, ped);
                        }
                    }
                }
                catch { }
            }
            _trackedPedHandles.Clear();
        }

        // ── Aborted ──
        private void OnAborted(object sender, EventArgs e)
        {
            // 取消 WebSocket（不阻塞，不调用 CloseAsync().Wait()）
            _client?.Stop();

            // 若处于生存模式则恢复状态
            if (_state == ModState.SurvivalActive)
            {
                try
                {
                    Game.Player.WantedLevel = _originalWantedLevel;
                }
                catch { }

                foreach (int handle in _trackedPedHandles)
                {
                    try
                    {
                        var ped = Entity.FromHandle(handle) as Ped;
                        if (ped != null && ped.Exists())
                        {
                            try
                            {
                                ped.Task.ClearAll();
                            }
                            catch
                            {
                                Function.Call(Hash.CLEAR_PED_TASKS, ped);
                            }
                        }
                    }
                    catch { }
                }
                _trackedPedHandles.Clear();
            }
        }
    }
}