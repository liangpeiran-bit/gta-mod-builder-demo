using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GTA;

public class LiveCommentMod : Script
{
    // ── WebSocket ──────────────────────────────────────────────
    private const string WebSocketUrl = "ws://127.0.0.1:60080";
    private const int ReconnectDelayMs = 3000;
    private const int ReceiveBufferSize = 4096;

    private readonly object _wsLock = new object();
    private ClientWebSocket _webSocket;
    private CancellationTokenSource _cts;
    private Task _receiveTask;

    // ── Command queue (thread-safe bridge from WS task → Tick) ─
    private readonly ConcurrentQueue<string> _commandQueue = new ConcurrentQueue<string>();

    // ── Cooldowns ──────────────────────────────────────────────
    private const int CooldownMs = 8000;
    private int _lastRepairTick;
    private int _lastBoostTick;

    // ── Boost state ────────────────────────────────────────────
    private const int BoostDurationMs = 4000;
    private const float BoostMultiplier = 1.8f;
    private const float DefaultMultiplier = 1.0f;

    private Vehicle _boostedVehicle;
    private int _boostEndTick;
    private readonly List<Vehicle> _activeBoostVehicles = new List<Vehicle>();

    // ── Repair constants ───────────────────────────────────────
    private const float MaxEngineHealth = 1000.0f;
    private const float RepairAmount = 120.0f;

    // ── Tick throttle ──────────────────────────────────────────
    private const int TickIntervalMs = 100;
    private int _lastTickMs;

    // ── Constructor ────────────────────────────────────────────
    public LiveCommentMod()
    {
        Interval = TickIntervalMs;
        Tick += OnTick;
        Aborted += OnAborted;

        _cts = new CancellationTokenSource();
        _receiveTask = Task.Run(() => ConnectAndReceiveAsync(_cts.Token));
    }

    // ── Tick ───────────────────────────────────────────────────
    private void OnTick(object sender, EventArgs e)
    {
        // Throttle in case Interval is ignored
        int now = Environment.TickCount;
        if (now - _lastTickMs < TickIntervalMs)
            return;
        _lastTickMs = now;

        // Drain command queue (all GTA API on main thread)
        while (_commandQueue.TryDequeue(out string cmd))
        {
            if (cmd == "repair")
                ExecuteRepair(now);
            else if (cmd == "boost")
                ExecuteBoost(now);
        }

        // Check boost expiration
        if (_boostedVehicle != null && now - _boostEndTick >= 0)
        {
            RestoreBoostedVehicle();
        }
    }

    // ── Repair ─────────────────────────────────────────────────
    private void ExecuteRepair(int nowTick)
    {
        // Cooldown check
        if (nowTick - _lastRepairTick < CooldownMs)
            return;

        Vehicle vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle == null || !vehicle.Exists())
            return;

        float current = vehicle.EngineHealth;
        float newHealth = Math.Min(current + RepairAmount, MaxEngineHealth);
        if (newHealth > current)
        {
            vehicle.EngineHealth = newHealth;
            _lastRepairTick = nowTick;
        }
    }

    // ── Boost ──────────────────────────────────────────────────
    private void ExecuteBoost(int nowTick)
    {
        // Cooldown check
        if (nowTick - _lastBoostTick < CooldownMs)
            return;

        Vehicle vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle == null || !vehicle.Exists())
            return;

        vehicle.EnginePowerMultiplier = BoostMultiplier;
        _lastBoostTick = nowTick;

        // Track for cleanup
        if (!_activeBoostVehicles.Contains(vehicle))
            _activeBoostVehicles.Add(vehicle);

        // Reset timer (overwrites any previous boost)
        _boostedVehicle = vehicle;
        _boostEndTick = nowTick + BoostDurationMs;
    }

    private void RestoreBoostedVehicle()
    {
        Vehicle v = _boostedVehicle;
        _boostedVehicle = null;

        if (v != null && v.Exists())
        {
            // Only restore if still at our boost value
            if (Math.Abs(v.EnginePowerMultiplier - BoostMultiplier) < 0.01f)
            {
                v.EnginePowerMultiplier = DefaultMultiplier;
            }
        }

        _activeBoostVehicles.Remove(v);
    }

    // ── WebSocket receive loop (runs on background Task) ──────
    private async Task ConnectAndReceiveAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            ClientWebSocket ws = null;
            try
            {
                ws = new ClientWebSocket();
                lock (_wsLock)
                {
                    _webSocket = ws;
                }

                await ws.ConnectAsync(new Uri(WebSocketUrl), token);

                var buffer = new byte[ReceiveBufferSize];
                var messageBuilder = new StringBuilder();

                while (ws.State == WebSocketState.Open && !token.IsCancellationRequested)
                {
                    var segment = new ArraySegment<byte>(buffer);
                    WebSocketReceiveResult result;
                    try
                    {
                        result = await ws.ReceiveAsync(segment, token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        try
                        {
                            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        }
                        catch { }
                        break;
                    }

                    if (result.Count > 0)
                    {
                        messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    }

                    if (result.EndOfMessage)
                    {
                        string json = messageBuilder.ToString();
                        messageBuilder.Clear();
                        ProcessJsonMessage(json);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception)
            {
                // Connect failed or connection lost – retry after delay
            }
            finally
            {
                lock (_wsLock)
                {
                    if (_webSocket == ws)
                        _webSocket = null;
                }

                if (ws != null)
                {
                    try
                    {
                        if (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseReceived)
                        {
                            ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None)
                              .Wait(2000);
                        }
                    }
                    catch { }
                    ws.Dispose();
                }
            }

            if (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(ReconnectDelayMs, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    // ── JSON message processing (background thread → queue) ────
    private void ProcessJsonMessage(string json)
    {
        // Only handle comment-type messages
        string msgType = ExtractJsonStringValue(json, "type");
        if (msgType == null || !string.Equals(msgType, "comment", StringComparison.OrdinalIgnoreCase))
            return;

        string content = ExtractJsonStringValue(json, "content");
        if (string.IsNullOrWhiteSpace(content))
            return;

        content = content.Trim();
        string lower = content.ToLowerInvariant();

        if (lower == "repair" || lower == "boost")
        {
            _commandQueue.Enqueue(lower);
        }
    }

    // ── Minimal JSON string value extractor ────────────────────
    private static string ExtractJsonStringValue(string json, string key)
    {
        string pattern = "\"" + key + "\"";
        int keyIndex = json.IndexOf(pattern, StringComparison.Ordinal);
        if (keyIndex < 0) return null;

        int colonIndex = json.IndexOf(':', keyIndex + pattern.Length);
        if (colonIndex < 0) return null;

        int valueStart = colonIndex + 1;
        while (valueStart < json.Length && json[valueStart] == ' ')
            valueStart++;

        if (valueStart >= json.Length || json[valueStart] != '"')
            return null;

        valueStart++; // skip opening quote

        int valueEnd = valueStart;
        while (valueEnd < json.Length)
        {
            if (json[valueEnd] == '\\')
            {
                valueEnd += 2;
                continue;
            }
            if (json[valueEnd] == '"')
                break;
            valueEnd++;
        }

        if (valueEnd >= json.Length || json[valueEnd] != '"')
            return null;

        return json.Substring(valueStart, valueEnd - valueStart);
    }

    // ── Cleanup ────────────────────────────────────────────────
    private void OnAborted(object sender, EventArgs e)
    {
        // Signal cancellation
        try { _cts?.Cancel(); } catch { }

        // Wait briefly for receive task to observe cancellation
        if (_receiveTask != null)
        {
            try { _receiveTask.Wait(3000); } catch { }
        }

        // Close and dispose websocket
        ClientWebSocket ws;
        lock (_wsLock)
        {
            ws = _webSocket;
            _webSocket = null;
        }

        if (ws != null)
        {
            try
            {
                if (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseReceived)
                {
                    ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None)
                      .Wait(2000);
                }
            }
            catch { }
            try { ws.Dispose(); } catch { }
        }

        // Dispose CTS
        try { _cts?.Dispose(); } catch { }
        _cts = null;

        // Restore all vehicles that still have the boost multiplier
        foreach (Vehicle v in _activeBoostVehicles)
        {
            if (v != null && v.Exists())
            {
                try
                {
                    if (Math.Abs(v.EnginePowerMultiplier - BoostMultiplier) < 0.01f)
                    {
                        v.EnginePowerMultiplier = DefaultMultiplier;
                    }
                }
                catch { }
            }
        }
        _activeBoostVehicles.Clear();
        _boostedVehicle = null;
    }
}