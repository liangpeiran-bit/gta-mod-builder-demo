using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModProject.LiveStudio
{
    public sealed class LiveStudioClient : IDisposable
    {
        private static readonly Uri Endpoint = new Uri("ws://127.0.0.1:60080");
        private const string SubscribeMessage =
            "{\"type\":\"subscribe\",\"id\":\"gta-mod\",\"data\":{\"type\":\"serviceSignalSub\",\"name\":\"IM_MESSAGE_TRANSPORT\"}}";

        private static readonly Semaphore ProcessGuard =
            new Semaphore(initialCount: 1, maximumCount: 1, name: "Local\\GtaLiveStudioModClient_v1");

        private static readonly TimeSpan MsgIdTtl = TimeSpan.FromSeconds(60);
        private const int MsgIdTableSoftCap = 1024;

        private readonly Action<LiveStudioEvent> _onEvent;
        private readonly Action<string> _onLog;
        private readonly ConcurrentDictionary<string, DateTime> _seenMsgIds =
            new ConcurrentDictionary<string, DateTime>();

        private CancellationTokenSource _cts;
        private Task _loopTask;
        private ClientWebSocket _socket;
        private bool _ownsGuard;

        public LiveStudioClient(Action<LiveStudioEvent> onEvent, Action<string> onLog = null)
        {
            _onEvent = onEvent ?? throw new ArgumentNullException(nameof(onEvent));
            _onLog = onLog ?? (_ => { });
        }

        public bool IsRunning => _loopTask != null && !_loopTask.IsCompleted;

        public void Start()
        {
            if (IsRunning) return;
            _cts = new CancellationTokenSource();
            _loopTask = Task.Run(() => StartAsync(_cts.Token));
        }

        public void Stop()
        {
            try { _cts?.Cancel(); } catch { }
            try { _socket?.Abort(); } catch { }
        }

        public void Dispose()
        {
            Stop();
            try { _socket?.Dispose(); } catch { }
            try { _cts?.Dispose(); } catch { }
            ReleaseGuardIfOwned();
        }

        ~LiveStudioClient()
        {
            ReleaseGuardIfOwned();
        }

        private async Task StartAsync(CancellationToken token)
        {
            bool acquired;
            try
            {
                acquired = await Task.Run(
                    () => ProcessGuard.WaitOne(TimeSpan.FromSeconds(3)),
                    token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                _onLog("LiveStudio: failed to acquire process guard - " + ex.Message);
                return;
            }

            if (!acquired)
            {
                _onLog("LiveStudio: previous client still active; staying idle");
                return;
            }
            _ownsGuard = true;

            await RunLoopAsync(token).ConfigureAwait(false);
        }

        private async Task RunLoopAsync(CancellationToken token)
        {
            var backoff = TimeSpan.FromSeconds(1);
            var maxBackoff = TimeSpan.FromSeconds(30);

            while (!token.IsCancellationRequested)
            {
                try
                {
                    _socket = new ClientWebSocket();
                    await _socket.ConnectAsync(Endpoint, token).ConfigureAwait(false);
                    _onLog("LiveStudio: connected");

                    await SendTextAsync(SubscribeMessage, token).ConfigureAwait(false);
                    backoff = TimeSpan.FromSeconds(1);

                    await ReceiveLoopAsync(token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    _onLog("LiveStudio: connection error - " + ex.Message);
                }
                finally
                {
                    try { _socket?.Dispose(); } catch { }
                    _socket = null;
                }

                if (token.IsCancellationRequested) return;

                try { await Task.Delay(backoff, token).ConfigureAwait(false); }
                catch (OperationCanceledException) { return; }

                var nextTicks = Math.Min(backoff.Ticks * 2, maxBackoff.Ticks);
                backoff = TimeSpan.FromTicks(nextTicks);
            }
        }

        private async Task SendTextAsync(string text, CancellationToken token)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            await _socket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: token).ConfigureAwait(false);
        }

        private async Task ReceiveLoopAsync(CancellationToken token)
        {
            var buffer = new byte[16 * 1024];
            var builder = new StringBuilder();

            while (!token.IsCancellationRequested && _socket.State == WebSocketState.Open)
            {
                builder.Length = 0;

                WebSocketReceiveResult result;
                do
                {
                    result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), token).ConfigureAwait(false);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        try
                        {
                            await _socket.CloseAsync(
                                WebSocketCloseStatus.NormalClosure,
                                "client closing",
                                CancellationToken.None).ConfigureAwait(false);
                        }
                        catch
                        {
                        }
                        return;
                    }
                    builder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                }
                while (!result.EndOfMessage);

                LiveStudioEvent evt;
                try
                {
                    evt = LiveStudioParser.Parse(builder.ToString());
                }
                catch (Exception ex)
                {
                    _onLog("LiveStudio: parse error - " + ex.Message);
                    continue;
                }

                if (evt == null) continue;
                if (!ShouldDeliverEvent(evt)) continue;

                try
                {
                    _onEvent(evt);
                }
                catch (Exception ex)
                {
                    _onLog("LiveStudio: handler error - " + ex.Message);
                }
            }
        }

        private bool ShouldDeliverEvent(LiveStudioEvent evt)
        {
            var msgId = evt.MsgId;
            if (string.IsNullOrEmpty(msgId)) return true;

            var now = DateTime.UtcNow;
            if (!_seenMsgIds.TryAdd(msgId, now))
            {
                return false;
            }

            if (_seenMsgIds.Count > MsgIdTableSoftCap)
            {
                var cutoff = now - MsgIdTtl;
                foreach (var pair in _seenMsgIds)
                {
                    if (pair.Value < cutoff)
                    {
                        DateTime _;
                        _seenMsgIds.TryRemove(pair.Key, out _);
                    }
                }
            }
            return true;
        }

        private void ReleaseGuardIfOwned()
        {
            if (!_ownsGuard) return;
            try { ProcessGuard.Release(); } catch { }
            _ownsGuard = false;
        }
    }
}
