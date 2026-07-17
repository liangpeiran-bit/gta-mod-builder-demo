using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GTA;

public class LiveStudioRepairBoost : Script
{
    private ClientWebSocket webSocket;
    private CancellationTokenSource cts;
    private Task receiveTask;
    
    // 冷却
    private int lastRepairTick = 0;
    private int lastBoostTick = 0;
    private const int CooldownMs = 8000;
    
    // Boost 状态
    private bool boostActive = false;
    private float originalPowerMultiplier = 1.0f;
    private int boostEndTick = 0;
    private Vehicle boostedVehicle = null;
    
    private const float MaxEngineHealth = 1000.0f;
    private const float RepairPercentage = 0.12f;
    private const float BoostMultiplier = 1.8f;
    private const int BoostDurationMs = 4000;
    
    public LiveStudioRepairBoost()
    {
        Tick += OnTick;
        Aborted += OnAborted;
        
        cts = new CancellationTokenSource();
        receiveTask = Task.Run(() => ConnectAndReceiveAsync(cts.Token));
    }
    
    private void OnTick(object sender, EventArgs e)
    {
        // 检查 boost 恢复
        if (boostActive && Environment.TickCount >= boostEndTick)
        {
            RestoreBoost();
        }
        
        // 保持脚本存活
        Wait(100);
    }
    
    private void OnAborted(object sender, EventArgs e)
    {
        // 取消接收任务
        cts?.Cancel();
        
        // 恢复 boost
        if (boostActive)
        {
            RestoreBoost();
        }
        
        // 关闭 WebSocket
        if (webSocket != null)
        {
            try
            {
                webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Script aborted", CancellationToken.None).Wait(3000);
            }
            catch { }
            webSocket.Dispose();
            webSocket = null;
        }
        
        cts?.Dispose();
        cts = null;
    }
    
    private async Task ConnectAndReceiveAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri("ws://127.0.0.1:60080"), token);
                
                // 发送订阅
                string subscribeJson = "{\"service\":\"serviceSignalSub\",\"method\":\"IM_MESSAGE_TRANSPORT\"}";
                byte[] subscribeBytes = Encoding.UTF8.GetBytes(subscribeJson);
                await webSocket.SendAsync(new ArraySegment<byte>(subscribeBytes), WebSocketMessageType.Text, true, token);
                
                // 接收循环
                await ReceiveLoopAsync(token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception)
            {
                // 重连前等待
                if (!token.IsCancellationRequested)
                {
                    try { await Task.Delay(5000, token); } catch { break; }
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
    
    private async Task ReceiveLoopAsync(CancellationToken token)
    {
        var buffer = new byte[4096];
        var messageBuffer = new StringBuilder();
        
        while (webSocket.State == WebSocketState.Open && !token.IsCancellationRequested)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
            
            if (result.MessageType == WebSocketMessageType.Close)
            {
                break;
            }
            
            string chunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
            messageBuffer.Append(chunk);
            
            if (result.EndOfMessage)
            {
                string fullMessage = messageBuffer.ToString();
                messageBuffer.Clear();
                ProcessMessage(fullMessage);
            }
        }
    }
    
    private void ProcessMessage(string json)
    {
        // 解析 WebcastChatMessage - 查找 content 字段
        string content = ExtractContent(json);
        if (string.IsNullOrEmpty(content))
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
    }
    
    private string ExtractContent(string json)
    {
        // 简单 JSON 解析：查找 "content" 字段
        // 也尝试 "comment" 和 "text"
        foreach (var key in new[] { "\"content\":\"", "\"comment\":\"", "\"text\":\"" })
        {
            int idx = json.IndexOf(key);
            if (idx >= 0)
            {
                idx += key.Length;
                int end = json.IndexOf("\"", idx);
                if (end > idx)
                {
                    return json.Substring(idx, end - idx);
                }
            }
        }
        return null;
    }
    
    private void ExecuteRepair()
    {
        int now = Environment.TickCount;
        if (now - lastRepairTick < CooldownMs)
            return;
        
        Vehicle vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle == null)
            return;
        
        float currentHealth = vehicle.EngineHealth;
        float repairAmount = MaxEngineHealth * RepairPercentage; // 120.0f
        float newHealth = Math.Min(currentHealth + repairAmount, MaxEngineHealth);
        vehicle.EngineHealth = newHealth;
        
        lastRepairTick = now;
    }
    
    private void ExecuteBoost()
    {
        int now = Environment.TickCount;
        if (now - lastBoostTick < CooldownMs)
            return;
        
        Vehicle vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle == null)
            return;
        
        // 如果已有 boost 活跃，先恢复
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