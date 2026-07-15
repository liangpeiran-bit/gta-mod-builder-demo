using GTA;
using System;

namespace VehicleCommandMod
{
    public class VehicleCommandMod : Script
    {
        // ── 冷却时间戳 ───────────────────────────────────────────
        private DateTime _lastRepairTime = DateTime.MinValue;
        private DateTime _lastBoostTime  = DateTime.MinValue;

        // ── Boost 持续效果状态 ────────────────────────────────────
        private bool     _boostActive;
        private DateTime _boostStartTime;
        private Vehicle  _boostVehicle;
        private float    _originalMultiplier;

        // ── 冷却常量 ─────────────────────────────────────────────
        private const double CooldownSeconds  = 10.0;
        private const double BoostDurationSec = 3.0;
        private const float  RepairAmount     = 100f;
        private const float  MaxEngineHealth  = 1000f;
        private const float  BoostMultiplier  = 2.0f;

        // ── 构造 ─────────────────────────────────────────────────
        public VehicleCommandMod()
        {
            // 订阅 LIVE Studio 评论事件（假设 LIVEStudio 静态类可用）
            LIVEStudio.OnCommentReceived += OnCommentReceived;

            // 订阅帧更新与中止事件
            Tick    += OnTick;
            Aborted += OnAborted;
        }

        // ── LIVE Studio 评论入口 ──────────────────────────────────
        private void OnCommentReceived(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment)) return;

            string normalized = comment.Trim().ToLowerInvariant();

            switch (normalized)
            {
                case "repair":
                    HandleRepair();
                    break;
                case "boost":
                    HandleBoost();
                    break;
                // 其他评论静默忽略
            }
        }

        // ── Repair 处理 ──────────────────────────────────────────
        private void HandleRepair()
        {
            // 冷却检查
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
            // 冷却检查
            if ((DateTime.UtcNow - _lastBoostTime).TotalSeconds < CooldownSeconds)
                return;

            Vehicle vehicle = Game.Player.Character.CurrentVehicle;
            if (vehicle == null || !vehicle.Exists())
                return;

            // 保存原始倍率并应用加速
            _originalMultiplier = vehicle.EnginePowerMultiplier;
            vehicle.EnginePowerMultiplier = BoostMultiplier;

            _boostActive    = true;
            _boostStartTime = DateTime.UtcNow;
            _boostVehicle   = vehicle;
            _lastBoostTime  = DateTime.UtcNow;
        }

        // ── 逐帧更新：检查 Boost 是否到期 ─────────────────────────
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
            // 立即恢复正在生效的 Boost
            RestoreBoost();

            // 移除事件订阅
            LIVEStudio.OnCommentReceived -= OnCommentReceived;
            Tick    -= OnTick;
            Aborted -= OnAborted;
        }
    }

    // ── LIVE Studio 静态 API 桩声明 ──────────────────────────────
    // 若实际环境中 LIVEStudio 类位于其他命名空间，请调整 using。
    // 若事件名称不同（如 OnChatMessage / OnComment），请相应修改。
    public static class LIVEStudio
    {
        /// <summary>当观众发送聊天评论时触发。</summary>
        public static event Action<string> OnCommentReceived;
    }
}