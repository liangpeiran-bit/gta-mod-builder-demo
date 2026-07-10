using System;
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
    /// <summary>
    /// 线程安全的礼物事件队列项
    /// </summary>
    internal sealed class GiftEventItem
    {
        public int GiftId { get; set; }
        public string GiftName { get; set; }
        public int DiamondCount { get; set; }
    }

    /// <summary>
    /// LIVE Studio WebSocket 消息接收器。
    /// 后台线程连接 ws://127.0.0.1:60080，接收 JSON 消息，
    /// 将 WebcastGiftMessage 事件放入线程安全队列，由主线程 Tick 消费。
    /// </summary>
    public sealed class Mod : Script
    {
        // ── LIVE Studio 连接 ──
        private const string LiveStudioUrl = "ws://127.0.0.1:60080";
        private ClientWebSocket _ws;
        private CancellationTokenSource _cts;

        // ── 消息队列 ──
        private readonly Queue<GiftEventItem> _giftQueue = new Queue<GiftEventItem>();
        private readonly object _queueLock = new object();
        private const int MaxQueueSize = 200;

        // ── 冷却 ──
        private DateTime _lastTriggerTime = DateTime.MinValue;
        private static readonly TimeSpan CooldownDuration = TimeSpan.FromSeconds(10.0);

        // ── 触发礼物 ──
        private const int RoseGiftId = 5655;
        private const string RoseGiftName = "Rose";

        // ── JSON 解析器 ──
        private readonly JavaScriptSerializer _jsonSerializer = new JavaScriptSerializer();

        // ── 重连控制 ──
        private bool _disposed = false;

        public Mod()
        {
            Tick += OnTick;
            Interval = 100; // 100ms
            Aborted += OnAborted;

            _cts = new CancellationTokenSource();
            Task.Run(() => WebSocketLoop(_cts.Token));
        }

        // ═══════════════════════════════════════════════════════════════
        // 主线程 Tick
        // ═══════════════════════════════════════════════════════════════
        private void OnTick(object sender, EventArgs e)
        {
            ProcessGiftQueue();
        }

        private void ProcessGiftQueue()
        {
            List<GiftEventItem> batch = null;

            lock (_queueLock)
            {
                if (_giftQueue.Count == 0)
                    return;

                batch = new List<GiftEventItem>(_giftQueue.Count);
                while (_giftQueue.Count > 0)
                {
                    batch.Add(_giftQueue.Dequeue());
                }
            }

            foreach (var item in batch)
            {
                try
                {
                    if (item.GiftId == RoseGiftId)
                    {
                        TryTriggerRoseEffect(item);
                    }
                }
                catch (Exception ex)
                {
                    // 静默吞下，避免单条异常中断批量处理
                    GTA.UI.Notification.Show("~r~[Mod] Effect error: " + ex.Message);
                }
            }
        }

        private void TryTriggerRoseEffect(GiftEventItem item)
        {
            DateTime now = DateTime.UtcNow;
            if ((now - _lastTriggerTime) < CooldownDuration)
            {
                // 冷却中，忽略
                return;
            }

            _lastTriggerTime = now;

            // 1. 天气 → 下雨（使用已确认的 Weather.Raining 枚举值）
            try
            {
                World.Weather = Weather.Raining;
                // 同时调用 Native 确保立即切换，无渐变
                Function.Call(Hash.SET_WEATHER_TYPE_NOW, "RAIN");
            }
            catch
            {
                // 降级：只使用 World.Weather
            }

            // 2. 通缉等级 → 4 星
            try
            {
                Game.Player.WantedLevel = 4;
            }
            catch
            {
                // 玩家无效时静默忽略
            }

            // 3. 发放冲锋枪 SMG
            try
            {
                Ped playerPed = Game.Player.Character;
                if (playerPed != null && playerPed.IsAlive)
                {
                    // 使用已确认的 Native Hash.GIVE_WEAPON_TO_PED
                    Function.Call(
                        Hash.GIVE_WEAPON_TO_PED,
                        playerPed,
                        (int)WeaponHash.SMG,
                        500,     // ammo
                        false,   // equipNow
                        false    // isAmmoLoaded
                    );
                }
            }
            catch
            {
                // 武器发放失败时静默忽略
            }

            // 通知
            GTA.UI.Notification.Show(
                "~p~[Rose] ~s~Rain + 4★ + SMG | Cooldown " +
                CooldownDuration.TotalSeconds.ToString("F0") + "s"
            );
        }

        // ═══════════════════════════════════════════════════════════════
        // WebSocket 后台线程
        // ═══════════════════════════════════════════════════════════════
        private async Task WebSocketLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && !_disposed)
            {
                try
                {
                    using (_ws = new ClientWebSocket())
                    {
                        await _ws.ConnectAsync(new Uri(LiveStudioUrl), ct);
                        await ReceiveLoop(ct);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    // 连接失败或断开，等待后重连
                }

                if (!ct.IsCancellationRequested && !_disposed)
                {
                    try
                    {
                        await Task.Delay(3000, ct);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
        }

        private async Task ReceiveLoop(CancellationToken ct)
        {
            var buffer = new byte[4096];
            var messageBuffer = new StringBuilder();

            while (_ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
            {
                messageBuffer.Clear();
                WebSocketReceiveResult result;

                // 读取一条完整消息（处理分片）
                do
                {
                    result = await _ws.ReceiveAsync(
                        new ArraySegment<byte>(buffer), ct);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _ws.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            string.Empty,
                            CancellationToken.None);
                        return;
                    }

                    messageBuffer.Append(
                        Encoding.UTF8.GetString(buffer, 0, result.Count));

                } while (!result.EndOfMessage);

                string rawJson = messageBuffer.ToString();
                if (string.IsNullOrWhiteSpace(rawJson))
                    continue;

                // 在主线程外解析 JSON，仅将结果入队
                ParseAndEnqueue(rawJson);
            }
        }

        private void ParseAndEnqueue(string rawJson)
        {
            try
            {
                Dictionary<string, object> dict =
                    _jsonSerializer.Deserialize<Dictionary<string, object>>(rawJson);

                if (dict == null)
                    return;

                // 识别消息类型：检查 "common.method" 或顶层 "type"/"method"
                string method = null;
                if (dict.TryGetValue("common", out object commonObj) &&
                    commonObj is Dictionary<string, object> commonDict)
                {
                    commonDict.TryGetValue("method", out object methodObj);
                    method = methodObj as string;
                }

                if (string.IsNullOrEmpty(method))
                {
                    dict.TryGetValue("method", out object m);
                    method = m as string;
                }
                if (string.IsNullOrEmpty(method))
                {
                    dict.TryGetValue("type", out object t);
                    method = t as string;
                }

                // 只处理 WebcastGiftMessage
                if (string.IsNullOrEmpty(method) ||
                    !method.Equals("WebcastGiftMessage", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                // 提取 gift_id / giftId
                int giftId = 0;
                if (dict.TryGetValue("giftId", out object gidObj))
                {
                    giftId = Convert.ToInt32(gidObj);
                }
                else if (dict.TryGetValue("gift_id", out gidObj))
                {
                    giftId = Convert.ToInt32(gidObj);
                }

                if (giftId == 0)
                    return;

                // 提取礼物名称（可选）
                string giftName = null;
                if (dict.TryGetValue("giftName", out object gnObj))
                {
                    giftName = gnObj as string;
                }
                else if (dict.TryGetValue("gift_name", out gnObj))
                {
                    giftName = gnObj as string;
                }

                // 提取钻石数（可选）
                int diamondCount = 0;
                if (dict.TryGetValue("diamondCount", out object dcObj))
                {
                    diamondCount = Convert.ToInt32(dcObj);
                }
                else if (dict.TryGetValue("diamond_count", out dcObj))
                {
                    diamondCount = Convert.ToInt32(dcObj);
                }

                var item = new GiftEventItem
                {
                    GiftId = giftId,
                    GiftName = giftName ?? string.Empty,
                    DiamondCount = diamondCount
                };

                lock (_queueLock)
                {
                    if (_giftQueue.Count < MaxQueueSize)
                    {
                        _giftQueue.Enqueue(item);
                    }
                }
            }
            catch
            {
                // JSON 解析失败，静默丢弃
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // 清理
        // ═══════════════════════════════════════════════════════════════
        private void OnAborted(object sender, EventArgs e)
        {
            _disposed = true;
            try
            {
                _cts?.Cancel();
            }
            catch
            {
            }

            try
            {
                if (_ws != null && _ws.State == WebSocketState.Open)
                {
                    _ws.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        string.Empty,
                        CancellationToken.None).GetAwaiter().GetResult();
                }
            }
            catch
            {
            }

            try
            {
                _ws?.Dispose();
            }
            catch
            {
            }

            try
            {
                _cts?.Dispose();
            }
            catch
            {
            }
        }
    }
}
