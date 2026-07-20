using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using GTA;

namespace ModProject
{
    public class Mod : Script
    {
        // ── WebSocket 客户端 ──────────────────────────────────────
        private LiveStudioClient _liveStudioClient;

        // ── Dispatcher ─────────────────────────────────────────────
        private readonly MainThreadDispatcher _dispatcher;

        // ── 玩法状态 ───────────────────────────────────────────────
        private readonly HashSet<string> _processedMsgIds = new HashSet<string>();
        private DateTime _cooldownUntil = DateTime.MinValue;
        private bool _isSurvivalActive;
        private readonly int _customRelationshipGroupHash;

        // ── 常量 ──────────────────────────────────────────────────
        private const int CooldownMs = 60000;
        private const int MaxMsgIdCache = 1000;
        private const int GiftIdRose = 5655;
        private const float NearbyPedsRadius = 50f;

        // ── 构造 ──────────────────────────────────────────────────
        public Mod()
        {
            _dispatcher = new MainThreadDispatcher(capacity: 500);
            _customRelationshipGroupHash = "ROSE_SURVIVAL".GetHashCode();

            this.Tick += OnTick;
            this.Aborted += OnAborted;

            _liveStudioClient = new LiveStudioClient(_dispatcher, HandleEvent);
            _liveStudioClient.Start();
        }

        // ═══════════════════════════════════════════════════════════
        //  GTA 主线程 Tick
        // ═══════════════════════════════════════════════════════════
        private void OnTick(object sender, EventArgs e)
        {
            _dispatcher.Drain();

            // 检测生存模式中玩家死亡
            if (_isSurvivalActive)
            {
                var playerPed = Game.Player.Character;
                if (playerPed != null && playerPed.IsDead)
                {
                    _isSurvivalActive = false;
                    _cooldownUntil = DateTime.UtcNow.AddMilliseconds(CooldownMs);
                    GTA.UI.Notification.Show("💀 你倒在了玫瑰生存中...");
                    RestoreRelationshipGroup();
                }
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  脚本中止
        // ═══════════════════════════════════════════════════════════
        private void OnAborted(object sender, EventArgs e)
        {
            // 停止 WebSocket 客户端（取消 + 清理，不阻塞主线程）
            _liveStudioClient?.Cancel();

            // 恢复关系组
            RestoreRelationshipGroup();

            // 注销事件
            this.Tick -= OnTick;
            this.Aborted -= OnAborted;
        }

        // ═══════════════════════════════════════════════════════════
        //  主线程事件处理
        // ═══════════════════════════════════════════════════════════
        private void HandleEvent(LiveStudioEvent evt)
        {
            if (evt is GiftEvent giftEvent)
            {
                HandleGiftEvent(giftEvent);
            }
            // ChatEvent 保留分发能力，暂不处理
        }

        private void HandleGiftEvent(GiftEvent gift)
        {
            // 仅处理 DESIGN.md 核验的 gift_id
            if (gift.GiftId != GiftIdRose) return;

            // 默认一次性礼物效果仅在 repeatEnd=true 触发
            if (!gift.RepeatEnd) return;

            // msgId 去重
            if (_processedMsgIds.Contains(gift.MsgId)) return;

            // 冷却检查
            if (DateTime.UtcNow < _cooldownUntil) return;

            // busy-state：已在生存模式中则丢弃
            if (_isSurvivalActive) return;

            // 写入去重集合
            _processedMsgIds.Add(gift.MsgId);
            if (_processedMsgIds.Count > MaxMsgIdCache)
            {
                _processedMsgIds.Clear();
            }

            // 触发生存模式
            ActivateSurvival();
        }

        // ═══════════════════════════════════════════════════════════
        //  生存模式激活
        // ═══════════════════════════════════════════════════════════
        private void ActivateSurvival()
        {
            try
            {
                var playerPed = Game.Player.Character;
                if (playerPed == null) return;

                // 1. 通缉等级 5 星
                Game.Player.WantedLevel = 5;

                // 2. 发放冲锋枪 + 1000 子弹
                playerPed.Weapons.Give(WeaponHash.SMG, 1000, equipNow: true, isAmmoLoaded: true);

                // 3. 创建敌对关系组并设为仇恨
                var roseGroup = new RelationshipGroup(_customRelationshipGroupHash);
                var playerGroup = playerPed.RelationshipGroup;
                roseGroup.SetRelationshipBetweenGroups(playerGroup, Relationship.Hate, bidirectionally: true);

                // 4. 将附近 NPC 切换为敌对并攻击主角
                var nearbyPeds = World.GetNearbyPeds(playerPed, NearbyPedsRadius, null);
                if (nearbyPeds != null)
                {
                    foreach (var ped in nearbyPeds)
                    {
                        if (ped == null || ped == playerPed) continue;
                        if (!ped.IsAlive || ped.IsPlayer) continue;

                        try
                        {
                            ped.RelationshipGroup = roseGroup;
                            ped.Task.FightAgainst(playerPed);
                        }
                        catch
                        {
                            // 单个 Ped 操作失败不阻断整体
                        }
                    }
                }

                // 5. 通知
                GTA.UI.Notification.Show("🌹 玫瑰生存模式启动！通缉5星，杀出重围！");

                _isSurvivalActive = true;
            }
            catch
            {
                // 静默处理，不崩溃
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  关系组恢复
        // ═══════════════════════════════════════════════════════════
        private void RestoreRelationshipGroup()
        {
            try
            {
                var playerPed = Game.Player.Character;
                if (playerPed == null) return;

                var roseGroup = new RelationshipGroup(_customRelationshipGroupHash);
                var playerGroup = playerPed.RelationshipGroup;
                roseGroup.SetRelationshipBetweenGroups(playerGroup, Relationship.Neutral, bidirectionally: true);
            }
            catch
            {
                // 静默
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  专用 LIVE Studio WebSocket 客户端
    // ═══════════════════════════════════════════════════════════════

    public class LiveStudioClient
    {
        private ClientWebSocket _webSocket;
        private CancellationTokenSource _cts;
        private Task _receiveTask;
        private readonly MainThreadDispatcher _dispatcher;
        private readonly Action<LiveStudioEvent> _eventHandler;

        private const string WebSocketUrl = "ws://127.0.0.1:60080";
        private const string SubscribeJson =
            "{\"type\":\"subscribe\",\"id\":\"gta-mod\",\"data\":{\"type\":\"serviceSignalSub\",\"name\":\"IM_MESSAGE_TRANSPORT\"}}";

        public LiveStudioClient(MainThreadDispatcher dispatcher, Action<LiveStudioEvent> eventHandler)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
        }

        // ── 启动连接（后台线程） ─────────────────────────────────
        public void Start()
        {
            if (_cts != null) return; // 已启动

            _cts = new CancellationTokenSource();
            _receiveTask = Task.Run(() => ConnectLoopAsync(_cts.Token));
        }

        // ── 取消（不阻塞主线程） ─────────────────────────────────
        public void Cancel()
        {
            try
            {
                _cts?.Cancel();
            }
            catch
            {
                // 静默
            }

            try
            {
                _webSocket?.Abort();
            }
            catch
            {
                // 静默
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  WebSocket 连接循环（后台线程）
        // ═══════════════════════════════════════════════════════════
        private async Task ConnectLoopAsync(CancellationToken ct)
        {
            int retryCount = 0;
            const int maxRetries = 3;
            const int retryDelayMs = 5000;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using (var ws = new ClientWebSocket())
                    {
                        _webSocket = ws;

                        await ws.ConnectAsync(new Uri(WebSocketUrl), ct);

                        // 发送精确订阅
                        var subscribeBytes = Encoding.UTF8.GetBytes(SubscribeJson);
                        await ws.SendAsync(
                            new ArraySegment<byte>(subscribeBytes),
                            WebSocketMessageType.Text,
                            endOfMessage: true,
                            cancellationToken: ct);

                        retryCount = 0; // 连接成功，重置重试计数

                        // 进入接收循环
                        await ReceiveLoopAsync(ws, ct);
                    }
                }
                catch (OperationCanceledException)
                {
                    break; // 正常取消
                }
                catch (Exception)
                {
                    retryCount++;
                    if (retryCount > maxRetries)
                        break; // 超过最大重试次数

                    try { await Task.Delay(retryDelayMs, ct); }
                    catch (OperationCanceledException) { break; }
                }
                finally
                {
                    _webSocket = null;
                }
            }
        }

        // ═══════════════════════════════════════════════════════════
        //  WebSocket 接收循环（后台线程，支持分片）
        // ═══════════════════════════════════════════════════════════
        private async Task ReceiveLoopAsync(ClientWebSocket ws, CancellationToken ct)
        {
            var buffer = new byte[4096];
            var messageBuilder = new StringBuilder();

            while (ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
            {
                WebSocketReceiveResult result;
                try
                {
                    result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (WebSocketException)
                {
                    break; // 连接断开，触发重连
                }

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    try { await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None); }
                    catch { }
                    break;
                }

                messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                if (result.EndOfMessage)
                {
                    var json = messageBuilder.ToString();
                    messageBuilder.Clear();

                    // 解析并分发到主线程
                    try
                    {
                        var parsed = LiveStudioParser.Parse(json);
                        if (parsed != null)
                        {
                            _dispatcher.Enqueue(() => _eventHandler(parsed));
                        }
                    }
                    catch
                    {
                        // 单条坏消息不得终止接收循环
                    }
                }
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  数据结构
    // ═══════════════════════════════════════════════════════════════

    public abstract class LiveStudioEvent
    {
        public string MsgId { get; set; }
        public long CreateTime { get; set; }
        public string UserId { get; set; }
        public string Nickname { get; set; }
    }

    public class ChatEvent : LiveStudioEvent
    {
        public string Content { get; set; }
    }

    public class GiftEvent : LiveStudioEvent
    {
        public int GiftId { get; set; }
        public string GiftName { get; set; }
        public int DiamondCount { get; set; }
        public int RepeatCount { get; set; }
        public bool RepeatEnd { get; set; }
        public int ComboCount { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════
    //  JSON 解析器（仅使用 JavaScriptSerializer）
    // ═══════════════════════════════════════════════════════════════

    public static class LiveStudioParser
    {
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();

        public static LiveStudioEvent Parse(string json)
        {
            try
            {
                var root = Serializer.Deserialize<Dictionary<string, object>>(json);
                if (root == null || !root.ContainsKey("data")) return null;

                var data = root["data"] as Dictionary<string, object>;
                if (data == null || !data.ContainsKey("common")) return null;

                var common = data["common"] as Dictionary<string, object>;
                if (common == null || !common.ContainsKey("method")) return null;

                var method = common["method"] as string;

                switch (method)
                {
                    case "WebcastChatMessage":
                        return ParseChat(data);
                    case "WebcastGiftMessage":
                        return ParseGift(data);
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private static ChatEvent ParseChat(Dictionary<string, object> data)
        {
            var common = data["common"] as Dictionary<string, object>;
            var user = data["user"] as Dictionary<string, object>;

            return new ChatEvent
            {
                MsgId = SafeGetString(common, "msgId"),
                CreateTime = SafeGetLong(common, "createTime"),
                UserId = SafeGetString(user, "id"),
                Nickname = SafeGetString(user, "nickname"),
                Content = SafeGetString(data, "content")
            };
        }

        private static GiftEvent ParseGift(Dictionary<string, object> data)
        {
            var common = data["common"] as Dictionary<string, object>;
            var user = data["user"] as Dictionary<string, object>;
            var gift = data["gift"] as Dictionary<string, object>;

            return new GiftEvent
            {
                MsgId = SafeGetString(common, "msgId"),
                CreateTime = SafeGetLong(common, "createTime"),
                UserId = SafeGetString(user, "id"),
                Nickname = SafeGetString(user, "nickname"),
                GiftId = SafeGetInt(gift, "id"),
                GiftName = SafeGetString(gift, "name"),
                DiamondCount = SafeGetInt(gift, "diamondCount"),
                RepeatCount = SafeGetInt(data, "repeatCount"),
                RepeatEnd = SafeGetBool(data, "repeatEnd"),
                ComboCount = SafeGetInt(data, "comboCount")
            };
        }

        // ── 安全类型转换 ────────────────────────────────────────

        private static string SafeGetString(Dictionary<string, object> dict, string key)
        {
            if (dict == null) return null;
            if (dict.TryGetValue(key, out object value) && value != null)
                return value.ToString();
            return null;
        }

        private static int SafeGetInt(Dictionary<string, object> dict, string key)
        {
            if (dict == null) return 0;
            if (dict.TryGetValue(key, out object value) && value != null)
            {
                if (value is int i32) return i32;
                if (value is long i64) return (int)i64;
                if (value is double d) return (int)d;
                if (int.TryParse(value.ToString(), out int parsed)) return parsed;
            }
            return 0;
        }

        private static long SafeGetLong(Dictionary<string, object> dict, string key)
        {
            if (dict == null) return 0L;
            if (dict.TryGetValue(key, out object value) && value != null)
            {
                if (value is long i64) return i64;
                if (value is int i32) return (long)i32;
                if (value is double d) return (long)d;
                if (long.TryParse(value.ToString(), out long parsed)) return parsed;
            }
            return 0L;
        }

        private static bool SafeGetBool(Dictionary<string, object> dict, string key)
        {
            if (dict == null) return false;
            if (dict.TryGetValue(key, out object value) && value != null)
            {
                if (value is bool b) return b;
                if (bool.TryParse(value.ToString(), out bool parsed)) return parsed;
            }
            return false;
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  主线程调度器（线程安全 bounded queue）
    // ═══════════════════════════════════════════════════════════════

    public class MainThreadDispatcher
    {
        private readonly Queue<Action> _queue = new Queue<Action>();
        private readonly int _capacity;
        private readonly object _lock = new object();

        public MainThreadDispatcher(int capacity)
        {
            _capacity = capacity;
        }

        public void Enqueue(Action action)
        {
            lock (_lock)
            {
                if (_queue.Count >= _capacity) return;
                _queue.Enqueue(action);
            }
        }

        public void Drain()
        {
            List<Action> batch = null;
            lock (_lock)
            {
                if (_queue.Count > 0)
                {
                    batch = new List<Action>(_queue);
                    _queue.Clear();
                }
            }

            if (batch != null)
            {
                foreach (var action in batch)
                {
                    try
                    {
                        action();
                    }
                    catch
                    {
                        // 单个 Action 失败不影响后续
                    }
                }
            }
        }
    }
}