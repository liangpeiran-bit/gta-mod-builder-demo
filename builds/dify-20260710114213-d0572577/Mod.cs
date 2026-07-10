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
        private enum TriggerState
        {
            Idle,
            Cooling
        }

        private const int RoseGiftId = 5655;
        private const string RoseGiftName = "Rose";
        private const int CooldownSeconds = 10;
        private const int SmgAmmo = 500;
        private const int WantedStars = 4;
        private const string LiveStudioUrl = "ws://127.0.0.1:60080";

        private TriggerState _state;
        private int _cooldownTimer;
        private ClientWebSocket _webSocket;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly object _queueLock;
        private readonly Queue<Action> _mainThreadActions;
        private bool _disposed;

        public Mod()
        {
            _state = TriggerState.Idle;
            _cooldownTimer = 0;
            _queueLock = new object();
            _mainThreadActions = new Queue<Action>();
            _disposed = false;

            Interval = 100;
            Tick += OnTick;
            Aborted += OnAborted;

            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => RunWebSocketLoop(_cancellationTokenSource.Token));
        }

        private void OnTick(object sender, EventArgs e)
        {
            ProcessMainThreadActions();

            if (_state == TriggerState.Cooling)
            {
                _cooldownTimer -= 100;
                if (_cooldownTimer <= 0)
                {
                    _cooldownTimer = 0;
                    _state = TriggerState.Idle;
                }
            }
        }

        private void OnAborted(object sender, EventArgs e)
        {
            Dispose();
        }

        private void ProcessMainThreadActions()
        {
            Queue<Action> snapshot = null;
            lock (_queueLock)
            {
                if (_mainThreadActions.Count > 0)
                {
                    snapshot = new Queue<Action>(_mainThreadActions);
                    _mainThreadActions.Clear();
                }
            }

            if (snapshot != null)
            {
                while (snapshot.Count > 0)
                {
                    try
                    {
                        snapshot.Dequeue()?.Invoke();
                    }
                    catch (Exception)
                    {
                        // Silently ignore per-tick action failures
                    }
                }
            }
        }

        private void EnqueueMainThreadAction(Action action)
        {
            if (_disposed) return;
            lock (_queueLock)
            {
                if (_mainThreadActions.Count < 100)
                {
                    _mainThreadActions.Enqueue(action);
                }
            }
        }

        private async Task RunWebSocketLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    using (_webSocket = new ClientWebSocket())
                    {
                        await _webSocket.ConnectAsync(new Uri(LiveStudioUrl), token);

                        var subMessage = "{\"type\":\"serviceSignalSub\",\"data\":{\"serviceType\":\"IM_MESSAGE_TRANSPORT\"}}";
                        var subBytes = Encoding.UTF8.GetBytes(subMessage);
                        await _webSocket.SendAsync(
                            new ArraySegment<byte>(subBytes),
                            WebSocketMessageType.Text,
                            true,
                            token);

                        var buffer = new byte[8192];
                        var messageBuilder = new StringBuilder();

                        while (_webSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
                        {
                            var result = await _webSocket.ReceiveAsync(
                                new ArraySegment<byte>(buffer),
                                token);

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                break;
                            }

                            messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                            if (result.EndOfMessage)
                            {
                                var fullMessage = messageBuilder.ToString();
                                messageBuilder.Clear();
                                ProcessWebSocketMessage(fullMessage);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Connection failed, wait and retry
                }

                if (!token.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(3000, token);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                }
            }
        }

        private void ProcessWebSocketMessage(string rawMessage)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                var envelope = serializer.Deserialize<WebSocketEnvelope>(rawMessage);

                if (envelope == null || envelope.type != "WebcastGiftMessage")
                {
                    return;
                }

                var giftData = envelope.data;
                if (giftData == null)
                {
                    return;
                }

                var giftId = 0;
                if (giftData.ContainsKey("giftId"))
                {
                    var giftIdObj = giftData["giftId"];
                    if (giftIdObj is int idInt)
                    {
                        giftId = idInt;
                    }
                    else if (giftIdObj is string idStr && int.TryParse(idStr, out var parsedId))
                    {
                        giftId = parsedId;
                    }
                }

                var giftName = string.Empty;
                if (giftData.ContainsKey("giftName"))
                {
                    giftName = giftData["giftName"] as string ?? string.Empty;
                }

                var isRose = giftId == RoseGiftId ||
                    string.Equals(giftName, RoseGiftName, StringComparison.OrdinalIgnoreCase);

                if (isRose)
                {
                    EnqueueMainThreadAction(HandleRoseGift);
                }
            }
            catch (Exception)
            {
                // Malformed message, ignore
            }
        }

        private void HandleRoseGift()
        {
            if (_disposed) return;
            if (_state == TriggerState.Cooling) return;

            _state = TriggerState.Cooling;
            _cooldownTimer = CooldownSeconds * 1000;

            try
            {
                World.Weather = Weather.Raining;
            }
            catch (Exception)
            {
                // Weather change failed, continue
            }

            try
            {
                var player = Game.Player;
                if (player != null)
                {
                    player.WantedLevel = WantedStars;
                }
            }
            catch (Exception)
            {
                // Wanted level change failed, continue
            }

            try
            {
                var playerPed = Game.Player?.Character;
                if (playerPed != null)
                {
                    playerPed.Weapons.Give(WeaponHash.SMG, SmgAmmo, true, true);
                }
            }
            catch (Exception)
            {
                // Weapon give failed, continue
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            try
            {
                _cancellationTokenSource?.Cancel();
            }
            catch (Exception)
            {
            }

            try
            {
                _webSocket?.Abort();
                _webSocket?.Dispose();
            }
            catch (Exception)
            {
            }

            try
            {
                _cancellationTokenSource?.Dispose();
            }
            catch (Exception)
            {
            }

            Tick -= OnTick;
            Aborted -= OnAborted;
        }
    }

    internal sealed class WebSocketEnvelope
    {
        public string type { get; set; }
        public Dictionary<string, object> data { get; set; }
    }
}
