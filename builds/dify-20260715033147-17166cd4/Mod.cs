using System;
using System.Collections.Concurrent;
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
    internal class LiveGiftMessage
    {
        public int GiftId;
        public string Method;
    }

    public class Mod : Script
    {
        private const string WsUrl = "ws://127.0.0.1:60080";
        private const int TargetGiftId = 5655;
        private const int TickIntervalMs = 50;
        private const int SpawnCooldownMs = 500;
        private const int MaxQueueSize = 100;
        private const float SpawnDistance = 3.0f;
        private const int ModelLoadTimeoutMs = 500;
        private const int ReconnectDelayMs = 3000;

        private CancellationTokenSource _cts;
        private ConcurrentQueue<LiveGiftMessage> _giftQueue;
        private int _lastSpawnTick;

        public Mod()
        {
            _giftQueue = new ConcurrentQueue<LiveGiftMessage>();
            _lastSpawnTick = 0;
            _cts = new CancellationTokenSource();

            Interval = TickIntervalMs;
            Tick += OnTick;
            Aborted += OnAborted;

            CancellationToken ct = _cts.Token;
            Task.Run(() => ConnectLoopAsync(ct));
        }

        private void OnTick(object sender, EventArgs e)
        {
            try
            {
                while (_giftQueue.TryDequeue(out LiveGiftMessage msg))
                {
                    if (msg.GiftId != TargetGiftId)
                        continue;

                    int now = Environment.TickCount;
                    if (now - _lastSpawnTick < SpawnCooldownMs)
                        continue;
                    _lastSpawnTick = now;

                    SpawnVehicle();
                }
            }
            catch (Exception)
            {
            }
        }

        private void SpawnVehicle()
        {
            try
            {
                Ped playerPed = Game.Player.Character;
                if (playerPed == null)
                    return;

                Vector3 spawnPos = playerPed.Position + playerPed.ForwardVector * SpawnDistance;
                float heading = playerPed.Heading;

                Model model = new Model(VehicleHash.Infernus);
                bool loaded = model.Request(ModelLoadTimeoutMs);
                if (loaded)
                {
                    Vehicle vehicle = World.CreateVehicle(model, spawnPos, heading);
                    if (vehicle != null)
                    {
                        vehicle.MarkAsNoLongerNeeded();
                    }
                }
                model.MarkAsNoLongerNeeded();
            }
            catch (Exception)
            {
            }
        }

        private void OnAborted(object sender, EventArgs e)
        {
            Cleanup();
        }

        private void Cleanup()
        {
            try
            {
                if (_cts != null)
                {
                    _cts.Cancel();
                }
            }
            catch (Exception)
            {
            }
        }

        private async Task ConnectLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                ClientWebSocket ws = null;
                try
                {
                    ws = new ClientWebSocket();
                    await ws.ConnectAsync(new Uri(WsUrl), ct);
                    await SubscribeAsync(ws, ct);
                    await ReceiveLoopAsync(ws, ct);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                }
                finally
                {
                    try
                    {
                        if (ws != null)
                        {
                            if (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseReceived)
                            {
                                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                            }
                            ws.Dispose();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                if (!ct.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(ReconnectDelayMs, ct);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
        }

        private async Task SubscribeAsync(ClientWebSocket ws, CancellationToken ct)
        {
            var subMessage = new Dictionary<string, object>
            {
                { "method", "serviceSignalSub" },
                { "params", new Dictionary<string, object> { { "channel", "IM_MESSAGE_TRANSPORT" } } }
            };

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(subMessage);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, ct);
        }

        private async Task ReceiveLoopAsync(ClientWebSocket ws, CancellationToken ct)
        {
            byte[] buffer = new byte[4096];
            StringBuilder messageBuilder = new StringBuilder();

            while (ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
            {
                WebSocketReceiveResult result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
                messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                if (result.EndOfMessage)
                {
                    string json = messageBuilder.ToString();
                    messageBuilder.Clear();
                    ProcessMessage(json);
                }
            }
        }

        private void ProcessMessage(string json)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                object deserialized = serializer.DeserializeObject(json);

                IDictionary<string, object> dict = deserialized as IDictionary<string, object>;
                if (dict == null)
                    return;

                if (!dict.TryGetValue("method", out object methodObj) || !(methodObj is string method))
                    return;

                if (method == "WebcastChatMessage")
                    return;

                if (method != "WebcastGiftMessage")
                    return;

                int giftId = 0;
                if (dict.TryGetValue("payload", out object payloadObj))
                {
                    IDictionary<string, object> payload = payloadObj as IDictionary<string, object>;
                    if (payload != null)
                    {
                        if (payload.TryGetValue("giftId", out object gidObj))
                        {
                            giftId = Convert.ToInt32(gidObj);
                        }
                        else if (payload.TryGetValue("gift_id", out object gidObjAlt))
                        {
                            giftId = Convert.ToInt32(gidObjAlt);
                        }
                    }
                }

                if (_giftQueue.Count < MaxQueueSize)
                {
                    _giftQueue.Enqueue(new LiveGiftMessage { GiftId = giftId, Method = method });
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
