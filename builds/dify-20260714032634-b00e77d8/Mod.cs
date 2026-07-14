using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using GTA;
using GTA.Math;
using GTA.Native;

namespace ModProject
{
    public class Mod : Script
    {
        private const string WsUrl = "ws://127.0.0.1:60080";
        private const int TargetGiftId = 5655;
        private const float CooldownSeconds = 5.0f;
        private const float SpawnDistance = 5.0f;
        private const int ModelLoadTimeoutMs = 3000;

        private static readonly VehicleHash[] CarPool = new VehicleHash[]
        {
            VehicleHash.Adder,
            VehicleHash.Cheetah,
            VehicleHash.EntityXF,
            VehicleHash.Turismor,
            VehicleHash.Zentorno,
            VehicleHash.Turismo2
        };

        private static readonly Random Rng = new Random();

        private readonly ConcurrentQueue<int> _giftQueue = new ConcurrentQueue<int>();
        private ClientWebSocket _ws;
        private Thread _wsThread;
        private float _cooldownRemaining;

        public Mod()
        {
            Tick += OnTick;
            Interval = 100;
            StartWebSocket();
        }

        private void StartWebSocket()
        {
            _wsThread = new Thread(WebSocketLoop)
            {
                IsBackground = true
            };
            _wsThread.Start();
        }

        private void WebSocketLoop()
        {
            try
            {
                _ws = new ClientWebSocket();
                _ws.ConnectAsync(new Uri(WsUrl), CancellationToken.None).Wait();

                string subscribeJson =
                    "{\"type\":\"serviceSignalSub\",\"channel\":\"IM_MESSAGE_TRANSPORT\"}";
                byte[] subscribeBytes = Encoding.UTF8.GetBytes(subscribeJson);
                _ws.SendAsync(
                    new ArraySegment<byte>(subscribeBytes),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None).Wait();

                byte[] buffer = new byte[4096];
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                while (_ws.State == WebSocketState.Open)
                {
                    ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
                    WebSocketReceiveResult result =
                        _ws.ReceiveAsync(segment, CancellationToken.None).Result;

                    if (result.Count == 0)
                    {
                        break;
                    }

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string json = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        try
                        {
                            Dictionary<string, object> dict =
                                serializer.Deserialize<Dictionary<string, object>>(json);

                            if (dict.TryGetValue("type", out object typeObj)
                                && typeObj.ToString() == "WebcastGiftMessage")
                            {
                                if (dict.TryGetValue("gift_id", out object giftIdObj))
                                {
                                    int giftId = Convert.ToInt32(giftIdObj);
                                    _giftQueue.Enqueue(giftId);
                                }
                            }
                        }
                        catch
                        {
                            // 解析失败，跳过
                        }
                    }
                }
            }
            catch
            {
                // WebSocket 连接失败，静默处理
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (_cooldownRemaining > 0f)
            {
                _cooldownRemaining -= Interval / 1000f;
                if (_cooldownRemaining < 0f)
                {
                    _cooldownRemaining = 0f;
                }
            }

            while (_giftQueue.TryDequeue(out int giftId))
            {
                if (giftId == TargetGiftId && _cooldownRemaining <= 0f)
                {
                    SpawnSportsCar();
                }
            }
        }

        private void SpawnSportsCar()
        {
            Ped playerPed = Game.Player.Character;
            if (playerPed == null || !playerPed.IsAlive)
            {
                _cooldownRemaining = CooldownSeconds;
                return;
            }

            Vector3 playerPos = playerPed.Position;
            float heading = playerPed.Heading;

            float rad = heading * (float)Math.PI / 180f;
            Vector3 forward = new Vector3(
                (float)Math.Sin(-rad),
                (float)Math.Cos(-rad),
                0f
            );
            Vector3 spawnPos = playerPos + forward * SpawnDistance;

            VehicleHash carHash = CarPool[Rng.Next(CarPool.Length)];
            Model model = new Model(carHash);

            if (!model.IsValid)
            {
                return;
            }

            model.Request();

            DateTime start = DateTime.Now;
            while (!model.IsLoaded
                   && (DateTime.Now - start).TotalMilliseconds < ModelLoadTimeoutMs)
            {
                Yield();
            }

            if (model.IsLoaded)
            {
                Vehicle vehicle = World.CreateVehicle(model, spawnPos, heading);
                if (vehicle != null)
                {
                    vehicle.MarkAsNoLongerNeeded();
                }

                _cooldownRemaining = CooldownSeconds;
            }

            model.MarkAsNoLongerNeeded();
        }
    }
}
