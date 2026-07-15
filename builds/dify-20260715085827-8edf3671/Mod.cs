using GTA;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VehicleCommandMod
{
    public class VehicleCommandMod : Script
    {
        // ── WebSocket ────────────────────────────────────────────
        private ClientWebSocket _ws;
        private CancellationTokenSource _cts;
        private Task _receiveTask;

        // ── 冷却时间戳 ───────────────────────────────────────────
        private DateTime _lastRepairTime = DateTime.MinValue;
        private DateTime _lastBoostTime  = DateTime.MinValue;

        // ── Boost 持续效果状态 ────────────────────────────────────
        private bool     _boostActive;
        private DateTime _boostStartTime;
        private Vehicle  _boostVehicle;
        private float    _originalMultiplier;

        // ── 常量 ─────────────────────────────────────────────────
        private const double CooldownSeconds  = 10.0;
        private const double BoostDurationSec = 3.0;
        private const float  RepairAmount     = 250f;
        private const float  MaxEngineHealth  = 1000f;
        private const float  BoostMultiplier  = 2.0f;

        private const string WsUrl = "ws://127.0.0.1:60080";

        // ── 构造 ─────────────────────────────────────────────────
        public VehicleCommandMod()
        {
            Tick    += OnTick;
            Aborted += OnAborted;

            _cts = new CancellationTokenSource();
            _ = ConnectAndReceiveAsync(_cts.Token);
        }

        // ── WebSocket 连接与接收循环 ──────────────────────────────
        private async Task ConnectAndReceiveAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    using (_ws = new ClientWebSocket())
                    {
                        await _ws.ConnectAsync(new Uri(WsUrl), ct);

                        // 订阅评论消息
                        byte[] sub = Encoding.UTF8.GetBytes(
                            "{\"service\":\"serviceSignalSub\",\"method\":\"IM_MESSAGE_TRANSPORT\"}");
                        await _ws.SendAsync(new ArraySegment<byte>(sub),
                            WebSocketMessageType.Text, true, ct);

                        var buffer = new byte[8192];
                        var sb = new StringBuilder();

                        while (_ws.State == WebSocketState.Open && !ct.IsCancellationRequested)
                        {
                            var result = await _ws.ReceiveAsync(
                                new ArraySegment<byte>(buffer), ct);

                            if (result.MessageType == WebSocketMessageType.Close)
                                break;

                            sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                            if (result.EndOfMessage)
                            {
                                string raw = sb.ToString();
                                sb.Clear();
                                ProcessMessage(raw);
                            }
                        }
                    }
                }
                catch (Exception) when (!ct.IsCancellationRequested)
                {
                    // 连接失败，等待 5 秒后重连
                }

                if (!ct.IsCancellationRequested)
                    await Task.Delay(5000, ct);
            }
        }

        // ── 原始消息分发 ─────────────────────────────────────────
        private void ProcessMessage(string raw)
        {
            // 仅处理 WebcastChatMessage（评论）
            if (!raw.Contains("\"WebcastChatMessage\""))
                return;

            string comment = ExtractField(raw, "content");
            if (string.IsNullOrWhiteSpace(comment))
                return;

            DispatchComment(comment);
        }

        // ── 轻量 JSON 字段提取 ────────────────────────────────────
        private static string ExtractField(string json, string fieldName)
        {
            string key = "\"" + fieldName + "\"";
            int keyIdx = json.IndexOf(key, StringComparison.Ordinal);
            if (keyIdx < 0) return null;

            int colonIdx = json.IndexOf(':', keyIdx + key.Length);
            if (colonIdx < 0) return null;

            int valStart = colonIdx + 1;
            while (valStart < json.Length && json[valStart] == ' ')
                valStart++;

            if (valStart >= json.Length) return null;

            if (json[valStart] == '"')
            {
                int valEnd = json.IndexOf('"', valStart + 1);
                if (valEnd < 0) return null;
                return json.Substring(valStart + 1, valEnd - valStart - 1);
            }
            else
            {
                int valEnd = valStart;
                while (valEnd < json.Length && json[valEnd] != ',' && json[valEnd] != '}' && json[valEnd] != ' ')
                    valEnd++;
                return json.Substring(valStart, valEnd - valStart);
            }
        }

        // ── 评论分发 ─────────────────────────────────────────────
        private void DispatchComment(string comment)
        {
            string normalized = comment.Trim().ToLowerInvariant();

            switch (normalized)
            {
                case "repair":
                    HandleRepair();
                    break;
                case "boost":
                    HandleBoost();
                    break;
            }
        }

        // ── Repair 处理 ──────────────────────────────────────────
        private void HandleRepair()
        {
            if ((DateTime.UtcNow - _lastRepairTime).TotalSeconds < CooldownSeconds)
                return;

            Vehicle vehicle = Game.Player.Character.CurrentVehicle;
            if (vehicle == null || !vehicle.Exists())
                return;

            float current = vehicle.EngineHealth;
            float clamped = Math.Min(current + RepairAmount, MaxEngineHealth);
            vehicle.EngineHealth = clamped;

            _lastRepairTime = DateTime.UtcNow;
        }

        // ── Boost 处理 ───────────────────────────────────────────
        private void HandleBoost()
        {
            if ((DateTime.UtcNow - _lastBoostTime).TotalSeconds < CooldownSeconds)
                return;

            Vehicle vehicle = Game.Player.Character.CurrentVehicle;
            if (vehicle == null || !vehicle.Exists())
                return;

            _originalMultiplier = vehicle.EnginePowerMultiplier;
            vehicle.EnginePowerMultiplier = BoostMultiplier;

            _boostActive    = true;
            _boostStartTime = DateTime.UtcNow;
            _boostVehicle   = vehicle;
            _lastBoostTime  = DateTime.UtcNow;
        }

        // ── 逐帧更新 ─────────────────────────────────────────────
        private void OnTick(object sender, EventArgs e)
        {
            if (!_boostActive) return;

            if ((DateTime.UtcNow - _boostStartTime).TotalSeconds >= BoostDurationSec)
            {
                RestoreBoost();
            }
        }

        // ── 恢复引擎功率倍率 ──────────────────────────────────────
        private void RestoreBoost()
        {
            if (_boostVehicle != null && _boostVehicle.Exists())
            {
                _boostVehicle.EnginePowerMultiplier = _originalMultiplier;
            }

            _boostActive  = false;
            _boostVehicle = null;
        }

        // ── 脚本中止清理 ─────────────────────────────────────────
        private void OnAborted(object sender, EventArgs e)
        {
            // 停止接收循环
            _cts?.Cancel();

            // 关闭 WebSocket
            try
            {
                if (_ws != null && _ws.State == WebSocketState.Open)
                {
                    _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Script aborted",
                        CancellationToken.None).Wait(2000);
                }
            }
            catch { }
            finally
            {
                _ws?.Dispose();
                _cts?.Dispose();
            }

            // 恢复 Boost
            RestoreBoost();

            // 移除事件订阅
            Tick    -= OnTick;
            Aborted -= OnAborted;
        }
    }
}