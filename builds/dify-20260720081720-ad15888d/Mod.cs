using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using GTA;

public class RoseSurvivalMod : Script
{
    // ---------- 嵌套类型 ----------
    private enum ModState { Idle, Survival }

    private sealed class ChatEvent
    {
        public string MsgId;
        public long CreateTime;
        public string UserId;
        public string Nickname;
        public string Content;
    }

    private sealed class GiftEvent
    {
        public string MsgId;
        public long CreateTime;
        public string UserId;
        public string Nickname;
        public int GiftId;
        public string GiftName;
        public int DiamondCount;
        public int RepeatCount;
        public bool RepeatEnd;
        public int ComboCount;
    }

    // ---------- MainThreadDispatcher ----------
    private sealed class MainThreadDispatcher
    {
        private readonly object _lock = new object();
        private readonly Queue<Action> _queue = new Queue<Action>();
        private readonly int _cap;

        public MainThreadDispatcher(int cap = 3) { _cap = Math.Max(1, cap); }

        public void Enqueue(Action action)
        {
            lock (_lock)
            {
                while (_queue.Count >= _cap) _queue.Dequeue();
                _queue.Enqueue(action);
            }
        }

        public void DrainOne()
        {
            Action action = null;
            lock (_lock)
            {
                if (_queue.Count > 0) action = _queue.Dequeue();
            }
            action?.Invoke();
        }
    }

    // ---------- LiveStudioParser ----------
    private sealed class LiveStudioParser
    {
        private readonly HashSet<string> _dedupeSet = new HashSet<string>();
        private readonly Queue<string> _dedupeOrder = new Queue<string>();
        private const int MaxDedupe = 200;

        public ChatEvent TryParseChat(Dictionary<string, object> root)
        {
            if (!TryGetDict(root, "data", out var data)) return null;
            if (!TryGetDict(data, "common", out var common)) return null;
            if (!TryGetString(common, "method", out var method) || method != "WebcastChatMessage") return null;

            var msgId = GetStringSafe(common, "msgId");
            if (string.IsNullOrEmpty(msgId)) return null;
            if (!TryGetDict(data, "user", out var user)) return null;

            return new ChatEvent
            {
                MsgId = msgId,
                CreateTime = GetLongSafe(common, "createTime"),
                UserId = GetStringSafe(user, "id"),
                Nickname = GetStringSafe(user, "nickname"),
                Content = GetStringSafe(data, "content")
            };
        }

        public GiftEvent TryParseGift(Dictionary<string, object> root)
        {
            if (!TryGetDict(root, "data", out var data)) return null;
            if (!TryGetDict(data, "common", out var common)) return null;
            if (!TryGetString(common, "method", out var method) || method != "WebcastGiftMessage") return null;

            var msgId = GetStringSafe(common, "msgId");
            if (string.IsNullOrEmpty(msgId)) return null;
            if (!TryGetDict(data, "gift", out var gift)) return null;
            if (!TryGetDict(data, "user", out var user)) return null;

            int giftId = GetIntSafe(gift, "id");
            bool repeatEnd = GetBoolSafe(data, "repeatEnd");

            return new GiftEvent
            {
                MsgId = msgId,
                CreateTime = GetLongSafe(common, "createTime"),
                UserId = GetStringSafe(user, "id"),
                Nickname = GetStringSafe(user, "nickname"),
                GiftId = giftId,
                GiftName = GetStringSafe(gift, "name"),
                DiamondCount = GetIntSafe(gift, "diamondCount"),
                RepeatCount = GetIntSafe(data, "repeatCount"),
                RepeatEnd = repeatEnd,
                ComboCount = GetIntSafe(data, "comboCount")
            };
        }

        public bool TryDedupe(string msgId)
        {
            if (string.IsNullOrEmpty(msgId)) return false;
            if (_dedupeSet.Contains(msgId)) return false;
            _dedupeSet.Add(msgId);
            _dedupeOrder.Enqueue(msgId);
            while (_dedupeOrder.Count > MaxDedupe)
            {
                var old = _dedupeOrder.Dequeue();
                _dedupeSet.Remove(old);
            }
            return true;
        }

        // ---- 安全提取 ----
        private static bool TryGetDict(Dictionary<string, object> src, string key, out Dictionary<string, object> val)
        {
            val = null;
            if (src.TryGetValue(key, out object obj) && obj is Dictionary<string, object> d) { val = d; return true; }
            return false;
        }
        private static bool TryGetString(Dictionary<string, object> src, string key, out string val)
        {
            val = null;
            if (src.TryGetValue(key, out object obj)) { val = obj as string; return val != null; }
            return false;
        }
        private static string GetStringSafe(Dictionary<string, object> src, string key)
        {
            src.TryGetValue(key, out object obj);
            return obj as string ?? string.Empty;
        }
        private static int GetIntSafe(Dictionary<string, object> src, string key)
        {
            src.TryGetValue(key, out object obj);
            if (obj is int i) return i;
            if (obj is long l) return (int)l;
            if (obj is double d) return (int)d;
            if (obj is string s && int.TryParse(s, out int p)) return p;
            return 0;
        }
        private static long GetLongSafe(Dictionary<string, object> src, string key)
        {
            src.TryGetValue(key, out object obj);
            if (obj is long l) return l;
            if (obj is int i) return i;
            if (obj is double d) return (long)d;
            if (obj is string s && long.TryParse(s, out long p)) return p;
            return 0;
        }
        private static bool GetBoolSafe(Dictionary<string, object> src, string key)
        {
            src.TryGetValue(key, out object obj);
            if (obj is bool b) return b;
            if (obj is string s && bool.TryParse(s, out bool pb)) return pb;
            return false;
        }
    }

    // ---------- 字段 ----------
    private ClientWebSocket _ws;
    private CancellationTokenSource _cts;
    private readonly LiveStudioParser _parser = new LiveStudioParser();
    private readonly MainThreadDispatcher _dispatcher = new MainThreadDispatcher(3);

    private ModState _state = ModState.Idle;
    private int _originalWantedLevel = -1;
    private DateTime _lastTriggerUtc = DateTime.MinValue;
    private static readonly TimeSpan Cooldown = TimeSpan.FromSeconds(60);
    private const int TargetGiftId = 5655;
    private const int AmmoAmount = 1000;

    // ---------- 构造 ----------
    public RoseSurvivalMod()
    {
        Tick += OnTick;
        Aborted += OnAborted;
        _cts = new CancellationTokenSource();
        Task.Run(() => RunReceiveLoopAsync(_cts.Token));
    }

    // ---------- Tick ----------
    private void OnTick(object sender, EventArgs e)
    {
        _dispatcher.DrainOne();

        // 检测死亡
        if (_state == ModState.Survival)
        {
            try
            {
                if (Game.Player.Character.IsDead)
                {
                    _state = ModState.Idle;
                    _originalWantedLevel = -1;
                    _lastTriggerUtc = DateTime.MinValue;
                }
            }
            catch
            {
                // 安全降级：若 IsDead 不可用则不做死亡检测，避免异常循环
            }
        }
    }

    // ---------- Aborted ----------
    private void OnAborted(object sender, EventArgs e)
    {
        _cts?.Cancel();

        // 恢复通缉等级
        if (_originalWantedLevel >= 0)
        {
            try { Game.Player.WantedLevel = _originalWantedLevel; } catch { }
            _originalWantedLevel = -1;
        }

        // 异步关闭 WebSocket
        var ws = Interlocked.Exchange(ref _ws, null);
        if (ws != null)
        {
            Task.Run(async () =>
            {
                try
                {
                    if (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseReceived)
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                catch { }
                finally { ws.Dispose(); }
            });
        }

        _state = ModState.Idle;
    }

    // ---------- 接收循环 ----------
    private async Task RunReceiveLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await ConnectAndSubscribeAsync(ct);
                await ReceiveMessagesAsync(ct);
            }
            catch (OperationCanceledException) { break; }
            catch { /* 连接/接收失败，等待后重连 */ }

            if (!ct.IsCancellationRequested)
            {
                try { await Task.Delay(10000, ct); } catch (OperationCanceledException) { break; }
            }
        }

        // 接收循环退出时清理 WebSocket
        var ws = Interlocked.Exchange(ref _ws, null);
        if (ws != null)
        {
            try
            {
                if (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseReceived)
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            catch { }
            finally { ws.Dispose(); }
        }
    }

    private async Task ConnectAndSubscribeAsync(CancellationToken ct)
    {
        var ws = new ClientWebSocket();
        await ws.ConnectAsync(new Uri("ws://127.0.0.1:60080"), ct);

        // 原子替换
        var old = Interlocked.Exchange(ref _ws, ws);
        if (old != null)
        {
            try { old.Dispose(); } catch { }
        }

        // 发送订阅（按协议门禁使用 service=serviceSignalSub / method=IM_MESSAGE_TRANSPORT）
        const string subscribeJson = "{\"type\":\"subscribe\",\"id\":\"gta-mod\",\"data\":{\"service\":\"serviceSignalSub\",\"method\":\"IM_MESSAGE_TRANSPORT\"}}";
        var bytes = Encoding.UTF8.GetBytes(subscribeJson);
        await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, ct);
    }

    private async Task ReceiveMessagesAsync(CancellationToken ct)
    {
        var ws = _ws;
        if (ws == null || ws.State != WebSocketState.Open) return;

        var buffer = new byte[4096];
        var messageBuilder = new StringBuilder();
        var serializer = new JavaScriptSerializer();

        while (ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
        {
            WebSocketReceiveResult result;
            try
            {
                result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
            }
            catch (OperationCanceledException) { break; }
            catch (WebSocketException)
            {
                // 连接异常，退出内层循环，外层负责重连
                break;
            }

            if (result.MessageType == WebSocketMessageType.Close)
            {
                try { await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None); } catch { }
                break;
            }

            if (result.MessageType == WebSocketMessageType.Text)
            {
                messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                if (result.EndOfMessage)
                {
                    DispatchMessage(messageBuilder.ToString(), serializer);
                    messageBuilder.Clear();
                }
            }
        }
    }

    private void DispatchMessage(string json, JavaScriptSerializer serializer)
    {
        Dictionary<string, object> root;
        try { root = serializer.Deserialize<Dictionary<string, object>>(json); } catch { return; }
        if (root == null) return;

        // 尝试解析礼物（本玩法只关心礼物）
        var giftEvent = _parser.TryParseGift(root);
        if (giftEvent != null)
        {
            if (giftEvent.GiftId == TargetGiftId && giftEvent.RepeatEnd)
            {
                if (_parser.TryDedupe(giftEvent.MsgId))
                {
                    _dispatcher.Enqueue(() => HandleRoseGift(giftEvent));
                }
            }
            return;
        }

        // 聊天消息（本玩法不处理，按 CONTRACT 可解析但不执行玩法动作）
        // var chatEvent = _parser.TryParseChat(root); // 保留解析能力
    }

    // ---------- 玩法处理（主线程） ----------
    private void HandleRoseGift(GiftEvent e)
    {
        var now = DateTime.UtcNow;

        if (_state == ModState.Idle)
        {
            // 记录原始通缉等级
            try { _originalWantedLevel = Game.Player.WantedLevel; } catch { _originalWantedLevel = 0; }

            // 5 星通缉
            try { Game.Player.WantedLevel = 5; } catch { }

            // 给 SMG + 1000 子弹
            try
            {
                Game.Player.Character.Weapons.Give(GTA.WeaponHash.SMG, AmmoAmount, true, true);
            }
            catch { }

            // 通知
            try { GTA.UI.Screen.ShowSubtitle("玫瑰生存模式开启！5星通缉 + SMG/1000发子弹", 5000); } catch { }

            _state = ModState.Survival;
            _lastTriggerUtc = now;
        }
        else if (_state == ModState.Survival)
        {
            if (now - _lastTriggerUtc < Cooldown)
            {
                // 冷却内忽略
                return;
            }

            // 冷却外且存活：补满弹药
            try
            {
                var ped = Game.Player.Character;
                var smg = ped.Weapons[GTA.WeaponHash.SMG];
                if (smg != null)
                {
                    smg.Ammo = AmmoAmount;
                }
                else
                {
                    ped.Weapons.Give(GTA.WeaponHash.SMG, AmmoAmount, true, true);
                }
                try { GTA.UI.Screen.ShowSubtitle("SMG 弹药已补满至 1000 发", 3000); } catch { }
            }
            catch { }

            _lastTriggerUtc = now;
        }
    }
}