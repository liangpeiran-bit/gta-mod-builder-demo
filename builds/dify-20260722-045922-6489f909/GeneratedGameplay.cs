using GTA.Math;
using System;
using System.Collections.Generic;
using ModProject.LiveStudio;

namespace ModProject
{
    public partial class Mod
    {
        // ── 聊天冷却：userId → 上次触发时间（UTC） ──
        private Dictionary<string, DateTime> _chatCooldowns;
        // ── 礼物全局冷却：上次警车生成时间 ──
        private DateTime _lastGiftCarTime;

        // ── 冷却常量 ──
        private const double ChatCooldownSeconds = 3.0;
        private const double GiftCooldownSeconds = 5.0;
        private const float CarSpawnDistance = 5.0f;
        // ── 冷却字典清理阈值 ──
        private const int CooldownCleanupThreshold = 100;

        partial void InitializeGameplay()
        {
            _chatCooldowns = new Dictionary<string, DateTime>();
            _lastGiftCarTime = DateTime.MinValue;
            LogGameplay("Gameplay initialized: chat hello notification + gift police car spawn");
        }

        partial void OnGameplayTick()
        {
            // 定期清理过期的聊天冷却条目，防止字典无限增长
            if (_chatCooldowns.Count > CooldownCleanupThreshold)
            {
                DateTime now = DateTime.UtcNow;
                var expiredKeys = new List<string>();
                foreach (var kvp in _chatCooldowns)
                {
                    if ((now - kvp.Value).TotalSeconds > ChatCooldownSeconds)
                    {
                        expiredKeys.Add(kvp.Key);
                    }
                }
                foreach (string key in expiredKeys)
                {
                    _chatCooldowns.Remove(key);
                }
            }
        }

        partial void OnGameplayAborted()
        {
            _chatCooldowns.Clear();
            _lastGiftCarTime = DateTime.MinValue;
            LogGameplay("Gameplay aborted: cooldowns cleared");
        }

        partial void OnChat(ChatEvent chat)
        {
            // 空内容直接跳过
            if (string.IsNullOrEmpty(chat.Content))
                return;

            // 检查是否包含 "hello"（大小写不敏感）
            if (chat.Content.IndexOf("hello", StringComparison.OrdinalIgnoreCase) < 0)
                return;

            // 单用户冷却检查
            DateTime now = DateTime.UtcNow;
            if (_chatCooldowns.TryGetValue(chat.UserId, out DateTime lastTime))
            {
                if ((now - lastTime).TotalSeconds < ChatCooldownSeconds)
                {
                    return; // 冷却中，忽略
                }
            }

            // 更新冷却
            _chatCooldowns[chat.UserId] = now;

            // 显示通知
            string message = string.Format("观众 {0} 向你打招呼！", chat.Nickname ?? "未知");
            GTA.UI.Notification.Show(message);

            LogGameplay(string.Format("Chat hello from {0} ({1})", chat.Nickname, chat.UserId));
        }

        partial void OnGift(GiftEvent gift)
        {
            // 仅在连击结束时触发
            if (!gift.RepeatEnd)
                return;

            // 全局冷却检查
            DateTime now = DateTime.UtcNow;
            if ((now - _lastGiftCarTime).TotalSeconds < GiftCooldownSeconds)
            {
                return; // 冷却中，忽略
            }

            // 更新冷却
            _lastGiftCarTime = now;

            // 生成警车
            try
            {
                var player = GTA.Game.Player;
                var character = player.Character;
                if (character == null)
                {
                    LogGameplay("Player character is null, cannot spawn police car");
                    return;
                }

                GTA.Math.Vector3 playerPos = character.Position;
                GTA.Math.Vector3 forward = character.ForwardVector;
                GTA.Math.Vector3 spawnPos = playerPos + forward * CarSpawnDistance;
                float heading = character.Heading;

                var model = new GTA.Model(GTA.VehicleHash.Police);
                GTA.Vehicle vehicle = GTA.World.CreateVehicle(model, spawnPos, heading);

                if (vehicle != null)
                {
                    string notifyMsg = string.Format("感谢 {0} 的礼物！警车已生成。", gift.Nickname ?? "观众");
                    GTA.UI.Notification.Show(notifyMsg);
                    LogGameplay(string.Format("Police car spawned at {0} for gift from {1} ({2})",
                        spawnPos, gift.Nickname, gift.UserId));
                }
                else
                {
                    LogGameplay("CreateVehicle returned null");
                    GTA.UI.Notification.Show("警车生成失败，请稍后再试");
                }
            }
            catch (Exception ex)
            {
                LogGameplay(string.Format("Error spawning police car: {0}", ex.Message));
                GTA.UI.Notification.Show("警车生成失败，请稍后再试");
            }
        }
    }
}
