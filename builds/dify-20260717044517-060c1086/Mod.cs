using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GTA;

public class RepairAndBoostMod : Script
{
    // WebSocket
    private ClientWebSocket _ws;
    private CancellationTokenSource _cts;
    private Task _receiveTask;
    
    // 消息队列
    private ConcurrentQueue<string> _commandQueue = new ConcurrentQueue<string>();
    
    // 冷却
    private DateTime _lastRepairTime = DateTime.MinValue;
    private DateTime _lastBoostTime = DateTime.MinValue;
    private readonly TimeSpan _cooldown = TimeSpan.FromSeconds(8);
    
    // Boost 状态
    private bool _boostActive = false;
    private DateTime _boostEndTime = DateTime.MinValue;
    private Vehicle _boostVehicle = null;
    private float _originalPowerMultiplier = 1.0f;
    
    // 常量
    private const float MaxEngineHealth = 1000.0f;
    private const float RepairRatio = 0.12f;
    private const float BoostMultiplier = 1.8f;
    private const float BoostDurationSec = 4.0f;
    
    public RepairAndBoostMod()
    {
        _cts = new CancellationTokenSource();
        _ws = new ClientWebSocket();
        _receiveTask = Task.Run(() => ReceiveLoop(_cts.Token));
        
        Tick += OnTick;
        Aborted += OnAborted;
    }
    
    private async Task ReceiveLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                // 连接
                await _ws.ConnectAsync(new Uri("ws://127.0.0.1:60080"), token);
                
                // 发送订阅
                string subscribeJson = "{\"type\":\"subscribe\",\"topics\":[\"serviceSignalSub\",\"IM_MESSAGE_TRANSPORT\"]}";
                byte[] subscribeBytes = Encoding.UTF8.GetBytes(subscribeJson);
                await _ws.SendAsync(new ArraySegment<byte>(subscribeBytes), WebSocketMessageType.Text, true, token);
                
                // 接收循环
                var buffer = new byte[4096];
                var messageBuilder = new StringBuilder();
                
                while (_ws.State == WebSocketState.Open && !token.IsCancellationRequested)
                {
                    var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                    
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", token);
                        break;
                    }
                    
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                        
                        if (result.EndOfMessage)
                        {
                            string message = messageBuilder.ToString();
                            messageBuilder.Clear();
                            ProcessMessage(message);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception)
            {
                // 重连延迟
            }
            
            // 清理
            if (_ws.State != WebSocketState.Closed)
            {
                try { _ws.Dispose(); } catch {}
                _ws = new ClientWebSocket();
            }
            
            // 重连延迟
            if (!token.IsCancellationRequested)
            {
                try { await Task.Delay(3000, token); } catch (OperationCanceledException) { break; }
            }
        }
    }
    
    private void ProcessMessage(string message)
    {
        // 检查是否为 WebcastChatMessage
        if (!message.Contains("WebcastChatMessage"))
            return;
        
        // 提取 content 字段
        string content = ExtractJsonStringField(message, "content");
        if (string.IsNullOrWhiteSpace(content))
            return;
        
        content = content.Trim();
        
        if (string.Equals(content, "repair", StringComparison.OrdinalIgnoreCase))
            _commandQueue.Enqueue("repair");
        else if (string.Equals(content, "boost", StringComparison.OrdinalIgnoreCase))
            _commandQueue.Enqueue("boost");
    }
    
    private string ExtractJsonStringField(string json, string fieldName)
    {
        // 简单的 JSON 字符串字段提取
        string key = "\"" + fieldName + "\"";
        int keyIndex = json.IndexOf(key, StringComparison.Ordinal);
        if (keyIndex < 0) return null;
        
        int colonIndex = json.IndexOf(':', keyIndex + key.Length);
        if (colonIndex < 0) return null;
        
        int valueStart = json.IndexOf('"', colonIndex + 1);
        if (valueStart < 0) return null;
        
        int valueEnd = json.IndexOf('"', valueStart + 1);
        if (valueEnd < 0) return null;
        
        return json.Substring(valueStart + 1, valueEnd - valueStart - 1);
    }
    
    private void OnTick(object sender, EventArgs e)
    {
        // 处理命令队列
        while (_commandQueue.TryDequeue(out string command))
        {
            if (command == "repair")
                ExecuteRepair();
            else if (command == "boost")
                ExecuteBoost();
        }
        
        // 检查 boost 到期
        if (_boostActive && DateTime.UtcNow >= _boostEndTime)
        {
            RestoreBoost();
        }
    }
    
    private void ExecuteRepair()
    {
        if ((DateTime.UtcNow - _lastRepairTime) < _cooldown)
            return;
        
        Vehicle vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle == null)
            return;
        
        float currentHealth = vehicle.EngineHealth;
        float newHealth = Math.Min(currentHealth + MaxEngineHealth * RepairRatio, MaxEngineHealth);
        vehicle.EngineHealth = newHealth;
        
        _lastRepairTime = DateTime.UtcNow;
    }
    
    private void ExecuteBoost()
    {
        if ((DateTime.UtcNow - _lastBoostTime) < _cooldown)
            return;
        
        Vehicle vehicle = Game.Player.Character.CurrentVehicle;
        if (vehicle == null)
            return;
        
        // 如果已有活跃 boost，先恢复旧车
        if (_boostActive && _boostVehicle != null && _boostVehicle != vehicle)
        {
            _boostVehicle.EnginePowerMultiplier = _originalPowerMultiplier;
        }
        
        // 应用新 boost
        _originalPowerMultiplier = vehicle.EnginePowerMultiplier;
        vehicle.EnginePowerMultiplier = BoostMultiplier;
        _boostVehicle = vehicle;
        _boostActive = true;
        _boostEndTime = DateTime.UtcNow.AddSeconds(BoostDurationSec);
        _lastBoostTime = DateTime.UtcNow;
    }
    
    private void RestoreBoost()
    {
        if (_boostVehicle != null)
        {
            _boostVehicle.EnginePowerMultiplier = _originalPowerMultiplier;
        }
        _boostActive = false;
        _boostVehicle = null;
    }
    
    private void OnAborted(object sender, EventArgs e)
    {
        // 取消后台任务
        _cts.Cancel();
        
        // 等待任务结束
        try { _receiveTask.Wait(2000); } catch {}
        
        // 关闭 WebSocket
        if (_ws != null && _ws.State == WebSocketState.Open)
        {
            try { _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).Wait(2000); } catch {}
        }
        _ws?.Dispose();
        _cts?.Dispose();
        
        // 恢复 boost
        if (_boostActive)
        {
            RestoreBoost();
        }
        
        // 注销事件
        Tick -= OnTick;
        Aborted -= OnAborted;
    }
}