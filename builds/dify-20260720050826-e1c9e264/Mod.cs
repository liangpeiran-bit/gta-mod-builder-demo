using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using GTA;
using GTA.Math;

namespace ModProject
{
    public class Mod : Script
    {
        private enum State { IDLE, TRIGGERED, SURVIVAL, END }

        private State currentState = State.IDLE;
        private volatile bool pendingSurvivalTrigger = false;
        private ClientWebSocket ws;
        private CancellationTokenSource cts;
        private Task listenTask;

        private const int GIFT_ID_ROSE = 5655;
        private const WeaponHash SURVIVAL_WEAPON = WeaponHash.AssaultRifle;
        private const int AMMO_COUNT = 1000;
        private const float NPC_SEARCH_RADIUS = 500f;

        private readonly List<Ped> affectedPeds = new List<Ped>();

        public Mod()
        {
            Tick += OnTick;
            Aborted += OnAborted;

            cts = new CancellationTokenSource();
            listenTask = ConnectAndListenAsync(cts.Token);
        }

        private async Task ConnectAndListenAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    using (ws = new ClientWebSocket())
                    {
                        await ws.ConnectAsync(new Uri("ws://127.0.0.1:60080"), token);

                        string subscribeJson = "{\"service\":\"serviceSignalSub\",\"method\":\"IM_MESSAGE_TRANSPORT\"}";
                        byte[] subBytes = Encoding.UTF8.GetBytes(subscribeJson);
                        await ws.SendAsync(new ArraySegment<byte>(subBytes), WebSocketMessageType.Text, true, token);

                        byte[] buffer = new byte[4096];
                        var messageBuilder = new StringBuilder();

                        while (ws.State == WebSocketState.Open && !token.IsCancellationRequested)
                        {
                            WebSocketReceiveResult result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                            messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                            if (result.EndOfMessage)
                            {
                                string fullMessage = messageBuilder.ToString();
                                ProcessMessage(fullMessage);
                                messageBuilder.Clear();
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
                    // 保守重连
                }

                if (!token.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(3000, token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
        }

        private void ProcessMessage(string message)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                object root = serializer.DeserializeObject(message);
                CheckForGiftMessage(root);
            }
            catch
            {
                // 忽略解析错误
            }
        }

        private void CheckForGiftMessage(object node)
        {
            if (node == null || pendingSurvivalTrigger)
                return;

            if (node is Dictionary<string, object> dict)
            {
                // 直接检查 giftId / gift_id 字段
                foreach (string key in dict.Keys)
                {
                    string lowerKey = key.ToLower();
                    if (lowerKey == "giftid" || lowerKey == "gift_id")
                    {
                        int giftId = Convert.ToInt32(dict[key]);
                        if (giftId == GIFT_ID_ROSE)
                        {
                            pendingSurvivalTrigger = true;
                            return;
                        }
                    }
                }

                // 检查 type/method 字段是否包含 GiftMessage，再深入查找 giftId
                foreach (string key in dict.Keys)
                {
                    string lowerKey = key.ToLower();
                    if (lowerKey == "type" || lowerKey == "method")
                    {
                        string typeStr = dict[key] as string;
                        if (typeStr != null && typeStr.Contains("GiftMessage"))
                        {
                            // 在当前层级重新查找 giftId
                            foreach (string innerKey in dict.Keys)
                            {
                                string lowerInnerKey = innerKey.ToLower();
                                if (lowerInnerKey == "giftid" || lowerInnerKey == "gift_id")
                                {
                                    int giftId = Convert.ToInt32(dict[innerKey]);
                                    if (giftId == GIFT_ID_ROSE)
                                    {
                                        pendingSurvivalTrigger = true;
                                        return;
                                    }
                                }
                            }

                            // 检查 data/payload 子对象
                            object dataObj = null;
                            if (dict.ContainsKey("data")) dataObj = dict["data"];
                            else if (dict.ContainsKey("Data")) dataObj = dict["Data"];
                            else if (dict.ContainsKey("payload")) dataObj = dict["payload"];
                            else if (dict.ContainsKey("Payload")) dataObj = dict["Payload"];

                            if (dataObj != null)
                            {
                                CheckForGiftMessage(dataObj);
                                if (pendingSurvivalTrigger) return;
                            }
                        }
                    }
                }

                // 递归检查嵌套对象和数组
                foreach (var kvp in dict)
                {
                    if (kvp.Value is Dictionary<string, object> || kvp.Value is ArrayList)
                    {
                        CheckForGiftMessage(kvp.Value);
                        if (pendingSurvivalTrigger) return;
                    }
                }
            }
            else if (node is ArrayList arr)
            {
                foreach (object item in arr)
                {
                    CheckForGiftMessage(item);
                    if (pendingSurvivalTrigger) return;
                }
            }
        }

        private void TriggerSurvival()
        {
            currentState = State.TRIGGERED;

            Ped playerPed = Game.Player.Character;

            // 1. 设置 5 星通缉
            Game.Player.WantedLevel = 5;

            // 2. 给予 AssaultRifle + 1000 弹药
            playerPed.Weapons.Give(SURVIVAL_WEAPON, AMMO_COUNT, true, true);

            // 3. 附近 NPC 攻击主角
            SetAllNearbyPedsHostile(playerPed);

            currentState = State.SURVIVAL;
        }

        private void SetAllNearbyPedsHostile(Ped playerPed)
        {
            Ped[] nearbyPeds = World.GetNearbyPeds(playerPed.Position, NPC_SEARCH_RADIUS);

            foreach (Ped ped in nearbyPeds)
            {
                if (ped != null && ped != playerPed && ped.IsAlive && !ped.IsPlayer)
                {
                    ped.Task.FightAgainst(playerPed);
                    affectedPeds.Add(ped);
                }
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            // 处理 WebSocket 回调触发的生存模式请求（线程安全）
            if (pendingSurvivalTrigger && currentState == State.IDLE)
            {
                pendingSurvivalTrigger = false;
                TriggerSurvival();
            }

            if (currentState == State.SURVIVAL)
            {
                if (Game.Player.Character.IsDead)
                {
                    CleanupSurvival();
                }
            }
            else if (currentState == State.END)
            {
                currentState = State.IDLE;
            }
        }

        private void CleanupSurvival()
        {
            currentState = State.END;

            // 清除通缉
            Game.Player.WantedLevel = 0;

            // 移除武器
            Ped playerPed = Game.Player.Character;
            if (playerPed != null)
            {
                playerPed.Weapons.Remove(SURVIVAL_WEAPON);
            }

            affectedPeds.Clear();
        }

        private void OnAborted(object sender, EventArgs e)
        {
            // 取消接收任务
            cts?.Cancel();

            // 关闭并释放 WebSocket
            try
            {
                if (ws != null)
                {
                    if (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseReceived)
                    {
                        ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None).Wait(2000);
                    }
                    ws.Dispose();
                }
            }
            catch { }
            finally
            {
                ws = null;
            }

            cts?.Dispose();
            cts = null;

            // 注销事件
            Tick -= OnTick;
            Aborted -= OnAborted;

            // 恢复 GTA 临时状态
            Game.Player.WantedLevel = 0;

            Ped playerPed = Game.Player.Character;
            if (playerPed != null)
            {
                playerPed.Weapons.Remove(SURVIVAL_WEAPON);
            }

            affectedPeds.Clear();
        }
    }
}