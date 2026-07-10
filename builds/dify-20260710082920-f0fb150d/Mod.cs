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
    public class Mod : Script
    {
        // ── LIVE Studio ─────────────────────────────────
        private const string WS_URL = "ws://127.0.0.1:60080";
        private const string SUBSCRIBE_JSON =
            "{\"type\":\"subscribe\",\"signal\":\"serviceSignalSub/IM_MESSAGE_TRANSPORT\"}";

        // ── 礼物触发 ────────────────────────────────────
        private const int ROSE_GIFT_ID = 5655;          // query_live_gifts_tool verified
        private const string ROSE_GIFT_NAME = "Rose";   // query_live_gifts_tool verified
        private const int SMG_AMMO = 500;

        // ── 冷却 ────────────────────────────────────────
        private const float COOLDOWN_SECONDS = 15f;
        private float cooldownRemaining = 0f;

        // ── WebSocket ───────────────────────────────────
        private ClientWebSocket ws;
        private CancellationTokenSource wsCts;

        // ── 主线程调度 ──────────────────────────────────
        private readonly Queue<Action> mainThreadActions = new Queue<Action>();
        private readonly object queueLock = new object();
        private bool isExecuting = false;

        // ── JSON ────────────────────────────────────────
        private readonly JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();

        // ─────────────────────────────────────────────────
        public Mod()
        {
            Tick += OnTick;
            Aborted += OnAborted;
            Interval = 0;

            wsCts = new CancellationTokenSource();
            Task.Run(() => WebSocketLoop(wsCts.Token));
        }

        // ── 脚本终止 ────────────────────────────────────
        private void OnAborted(object sender, EventArgs e)
        {
            try { wsCts?.Cancel(); } catch { }
        }

        // ── 主线程 Tick ─────────────────────────────────
        private void OnTick(object sender, EventArgs e)
        {
            lock (queueLock)
            {
                // 处理排队的 GTA 操作
                while (mainThreadActions.Count > 0)
                {
                    Action action = mainThreadActions.Dequeue();
                    try { action(); } catch { }
                }

                // 冷却递减
                if (cooldownRemaining > 0f)
                {
                    cooldownRemaining -= Game.LastFrameTime;
                    if (cooldownRemaining < 0f)
                        cooldownRemaining = 0f;
                }
            }
        }

        // ── WebSocket 循环（后台线程）────────────────────
        private async Task WebSocketLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using (ws = new ClientWebSocket())
                    {
                        await ws.ConnectAsync(new Uri(WS_URL), ct);

                        // 订阅消息通道
                        byte[] subBytes = Encoding.UTF8.GetBytes(SUBSCRIBE_JSON);
                        await ws.SendAsync(
                            new ArraySegment<byte>(subBytes),
                            WebSocketMessageType.Text,
                            true,
                            ct);

                        byte[] buffer = new byte[8192];
                        StringBuilder sb = new StringBuilder();

                        while (ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
                        {
                            WebSocketReceiveResult result =
                                await ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);

                            if (result.MessageType == WebSocketMessageType.Close)
                                break;

                            sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                            if (result.EndOfMessage)
                            {
                                string json = sb.ToString();
                                sb.Clear();
                                ProcessMessage(json);
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
                    // 连接中断，等待后重连
                }

                if (!ct.IsCancellationRequested)
                {
                    try { await Task.Delay(5000, ct); } catch { break; }
                }
            }
        }

        // ── 消息解析（后台线程）─────────────────────────
        private void ProcessMessage(string json)
        {
            try
            {
                var msg = jsonSerializer.Deserialize<Dictionary<string, object>>(json);
                if (msg == null || !msg.ContainsKey("type"))
                    return;

                string type = msg["type"] as string;
                if (type != "WebcastGiftMessage")
                    return;

                // 提取 gift_id（尝试顶层及嵌套 data 对象）
                int giftId = -1;
                string giftName = null;

                if (msg.ContainsKey("gift_id"))
                {
                    int.TryParse(msg["gift_id"]?.ToString(), out giftId);
                }
                else if (msg.ContainsKey("data"))
                {
                    var data = msg["data"] as Dictionary<string, object>;
                    if (data != null)
                    {
                        if (data.ContainsKey("gift_id"))
                            int.TryParse(data["gift_id"]?.ToString(), out giftId);
                        if (data.ContainsKey("gift_name"))
                            giftName = data["gift_name"] as string;
                    }
                }

                // 精确匹配 Rose 礼物（gift_id 优先，名称降级）
                bool isRose = (giftId == ROSE_GIFT_ID) ||
                              (giftName != null && giftName == ROSE_GIFT_NAME);

                if (!isRose)
                    return;

                // 冷却检查并入队
                lock (queueLock)
                {
                    if (cooldownRemaining <= 0f)
                    {
                        cooldownRemaining = COOLDOWN_SECONDS;
                        mainThreadActions.Enqueue(() => ExecuteRoseEffect());
                    }
                }
            }
            catch
            {
                // 忽略解析错误，不阻塞 WebSocket 循环
            }
        }

        // ── 玫瑰效果（主线程执行）───────────────────────
        private void ExecuteRoseEffect()
        {
            if (isExecuting)
                return;
            isExecuting = true;

            try
            {
                // 1. 天气→下雨
                SetWeatherToRain();

                // 2. 通缉等级→4星
                SetWantedLevelTo4();

                // 3. 发放 SMG 冲锋枪
                GivePlayerSMG();
            }
            finally
            {
                isExecuting = false;
            }
        }

        private void SetWeatherToRain()
        {
            try
            {
                // 受管 API：GTA.World.Weather + GTA.Weather.Raining（均已由 SHVDN API 检索确认）
                World.Weather = Weather.Raining;
            }
            catch
            {
                // 降级：原生 Hash（已确认 Hash.SET_WEATHER_TYPE_NOW_PERSIST）
                try
                {
                    Function.Call(Hash.SET_WEATHER_TYPE_NOW_PERSIST, "Raining");
                }
                catch { }
            }
        }

        private void SetWantedLevelTo4()
        {
            try
            {
                // GTA.Player.WantedLevel（已由 SHVDN API 检索确认：System.Int32 get/set）
                Game.Player.WantedLevel = 4;
            }
            catch { }
        }

        private void GivePlayerSMG()
        {
            try
            {
                Ped playerPed = Game.Player.Character;
                if (playerPed == null)
                    return;

                // 降级方案：原生 GIVE_WEAPON_TO_PED（Hash 已确认）
                // WeaponHash.SMG 枚举已确认
                Function.Call(
                    Hash.GIVE_WEAPON_TO_PED,
                    playerPed,
                    WeaponHash.SMG,
                    SMG_AMMO,
                    false,
                    false);
            }
            catch { }
        }
    }
}
