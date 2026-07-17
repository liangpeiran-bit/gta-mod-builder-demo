using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GTA;

public class LiveStudioRepairBoost : Script
{
    // ── WebSocket ────────────────────────────────────────────
    private const string WsEndpoint = "ws://127.0.0.1:60080";
    private const string SubscribeJson = "{\"service\":\"serviceSignalSub\",\"method\":\"IM_MESSAGE_TRANSPORT\"}";

    private ClientWebSocket webSocket;
    private CancellationTokenSource cts;
    private Task receiveTask;

    // ── 冷却（毫秒级，各指令独立） ──────────────────────────
    private const int CooldownMs = 8000;
    private int lastRepairTick;
    private int lastBoostTick;

    // ── Boost 临时状态 ─────────────────────────────────────
    private const float BoostMultiplier = 1.8f;
    private const int BoostDurationMs = 6000;

    private bool boostActive;
    private float originalPowerMultiplier = 1.0f;
    private int boostEndTick;
    private Vehicle boostedVehicle;

    // ── Repair 参数 ─────────────────────────────────────────
    private const float MaxEngineHealthFallback = 1000.0f;
    private const float RepairRatio = 0.30f;

    // ── 构造 ────────────────────────────────────────────────
    public LiveStudioRepairBoost()
    {
        Tick += OnTick;
        Aborted += OnAborted;

        cts = new CancellationTokenSource();
        receiveTask = Task.Run(() => ConnectAndReceiveAsync(cts.Token));
    }

    // ── 主循环 ──────────────────────────────────────────────
    private void OnTick(object sender, EventArgs e)
    {
        // 检查 boost 是否到期需要恢复
        if (boostActive && Environment.TickCount >= boostEndTick)
        {
            RestoreBoost();
        }

        Wait(100);
    }

    // ── 脚本卸载清理 ───────────────────────────────────────
    private void OnAborted(object sender, EventArgs e)
    {
        // 1. 取消接收循环
        cts?.Cancel();

        // 2. 恢复 boost 临时状态
        if (boostActive)
        {
            RestoreBoost();
        }

        // 3. 等待接收任务结束
        if (receiveTask != null && !receiveTask.IsCompleted)
        {
            try { receiveTask.Wait(3000); } catch { }
        }

        // 4. 关闭 WebSocket
        if (webSocket != null)
        {
            try
            {
                if (webSocket.State == WebSocketState.Open ||
                    webSocket.State == WebSocketState.CloseReceived)
                {
                    webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Script aborted",
                        CancellationToken.None).Wait(3000);
                }
            }
            catch { }

            webSocket.Dispose();
            webSocket = null;
        }

        // 5. 释放 CancellationTokenSource
        cts?.Dispose();
        cts = null;
    }

    // ── WebSocket 连接与重连 ────────────────────────────────
    private async Task ConnectAndReceiveAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri(WsEndpoint), token);

                // 连接成功后立即发送订阅
                byte[] subscribeBytes = Encoding.UTF8.GetBytes(SubscribeJson);
                await webSocket.SendAsync(
                    new ArraySegment<byte>(subscribeBytes),
                    WebSocketMessageType.Text,
                    true,
                    token);

                // 进入接收循环（阻塞直到断开或取消）
                await ReceiveLoopAsync(token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch
            {
                // 连接失败或异常断开：等待后重连
                if (!token.IsCancellationRequested)
                {
                    try { await Task.Delay(5000, token); }
                    catch (OperationCanceledException) { break; }
                }
            }
            finally
            {
                if (webSocket != null)
                {
                    try { webSocket.Dispose(); } catch { }
                    webSocket = null;
                }
            }
        }
    }

    // ── 消息接收（支持分片） ────────────────────────────────
    private async Task ReceiveLoopAsync(CancellationToken token)
    {
        var buffer = new byte[4096];
        var messageBuilder = new StringBuilder();

        while (webSocket != null &&
               webSocket.State == WebSocketState.Open &&
               !token.IsCancellationRequested)
        {
            WebSocketReceiveResult result;
            try
            {
                result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), token);
            }
            catch (WebSocketException)
            {
                break;
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (result.MessageType == WebSocketMessageType.Close)
            {
                break;
            }

            string chunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
            messageBuilder.Append(chunk);

            if (result.EndOfMessage)
            {
                string fullMessage = messageBuilder.ToString();
                messageBuilder.Clear();
                ProcessMessage(fullMessage);
            }
        }
    }

    // ── 消息分发 ────────────────────────────────────────────
    private void ProcessMessage(string json)
    {
        // 从 WebcastChatMessage JSON 中提取评论文本
        string content = ExtractContentField(json);
        if (string.IsNullOrWhiteSpace(content))
            return;

        content = content.Trim();
        string lower = content.ToLowerInvariant();

        if (lower == "repair")
        {
            ExecuteRepair();
        }
        else if (lower == "boost")
        {
            ExecuteBoost();
        }
        // 其他消息静默忽略
    }

    // ── JSON content 字段提取（fallback 多字段尝试） ────────
    private string ExtractContentField(string json)
    {
        // 按优先级尝试常见字段名
        foreach (string key in new[] { "\"content\":\"", "\"comment\":\"", "\"text\":\"" })
        {
            int idx = json.IndexOf(key, StringComparison.Ordinal);
            if (idx < 0)
                continue;

            idx += key.Length;
            int end = json.IndexOf('"', idx);
            if (end > idx)
            {
                return json.Substring(idx, end - idx);
            }
        }

        return null;
    }

    // ── repair 指令执行 ─────────────────────────────────────
    private void ExecuteRepair()
    {
        int now = Environment.TickCount;
        if (now - lastRepairTick < CooldownMs)
            return;

        Vehicle vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle == null)
            return;

        float current = vehicle.EngineHealth;
        float repairAmount = MaxEngineHealthFallback * RepairRatio; // 300.0
        float target = current + repairAmount;
        if (target > MaxEngineHealthFallback)
            target = MaxEngineHealthFallback;

        vehicle.EngineHealth = target;
        lastRepairTick = now;
    }

    // ── boost 指令执行 ──────────────────────────────────────
    private void ExecuteBoost()
    {
        int now = Environment.TickCount;
        if (now - lastBoostTick < CooldownMs)
            return;

        Vehicle vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle == null)
            return;

        // 如果上一个 boost 尚未恢复，先强制恢复再应用新的
        if (boostActive)
        {
            RestoreBoost();
        }

        originalPowerMultiplier = vehicle.EnginePowerMultiplier;
        vehicle.EnginePowerMultiplier = BoostMultiplier;

        boostActive = true;
        boostedVehicle = vehicle;
        boostEndTick = now + BoostDurationMs;
        lastBoostTick = now;
    }

    // ── boost 恢复 ──────────────────────────────────────────
    private void RestoreBoost()
    {
        if (!boostActive)
            return;

        if (boostedVehicle != null && boostedVehicle.Exists())
        {
            boostedVehicle.EnginePowerMultiplier = originalPowerMultiplier;
        }

        boostActive = false;
        boostedVehicle = null;
        originalPowerMultiplier = 1.0f;
    }
}