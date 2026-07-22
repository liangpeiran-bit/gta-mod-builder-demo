using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.UI;
using ModProject.LiveStudio;

namespace ModProject
{
    public partial class Mod
    {
        // 聊天 backup 冷却：按 UserId
        private Dictionary<string, DateTime> _userCooldowns = new Dictionary<string, DateTime>();
        private static readonly TimeSpan BackupCooldown = TimeSpan.FromSeconds(3);

        // 礼物救护车全局冷却
        private DateTime _lastGiftSpawnTime = DateTime.MinValue;
        private static readonly TimeSpan GiftSpawnCooldown = TimeSpan.FromSeconds(5);

        // 追踪本玩法生成的救护车（FIFO）
        private List<Vehicle> _spawnedAmbulances = new List<Vehicle>();
        private const int MaxAmbulances = 10;

        partial void InitializeGameplay()
        {
        }

        partial void OnGameplayTick()
        {
        }

        partial void OnGameplayAborted()
        {
            // 删除所有本玩法生成的救护车
            foreach (Vehicle v in _spawnedAmbulances)
            {
                if (v != null && v.Exists())
                {
                    v.Delete();
                }
            }
            _spawnedAmbulances.Clear();
            _userCooldowns.Clear();
            _lastGiftSpawnTime = DateTime.MinValue;
        }

        partial void OnChat(ChatEvent chat)
        {
            if (chat.Content == null)
                return;

            // 不区分大小写匹配 "backup"
            if (chat.Content.IndexOf("backup", StringComparison.OrdinalIgnoreCase) < 0)
                return;

            // 先清理过期冷却条目，再检查冷却
            CleanExpiredCooldowns();

            if (_userCooldowns.TryGetValue(chat.UserId, out DateTime lastTime))
            {
                if (DateTime.UtcNow - lastTime < BackupCooldown)
                {
                    return;
                }
            }

            // 显示通知：sender 字段显示观众昵称
            try
            {
                GTA.UI.Notification.Show(
                    NotificationIcon.Default,
                    chat.Nickname,
                    "Backup 请求",
                    "",
                    false,
                    false
                );
            }
            catch
            {
                // 降级：简单文本通知
                GTA.UI.Notification.Show("backup: " + chat.Nickname);
            }

            _userCooldowns[chat.UserId] = DateTime.UtcNow;
            LogGameplay("backup 触发: " + chat.Nickname);
        }

        partial void OnGift(GiftEvent gift)
        {
            TriggerGiftOnce(gift, "*", matchedGift =>
            {
                // 全局冷却检查
                if (DateTime.UtcNow - _lastGiftSpawnTime < GiftSpawnCooldown)
                {
                    LogGameplay("礼物救护车冷却中，忽略: " + matchedGift.Nickname + " 赠送 " + matchedGift.GiftName);
                    return;
                }

                SpawnAmbulance(matchedGift);
                _lastGiftSpawnTime = DateTime.UtcNow;
            });
        }

        private void SpawnAmbulance(GiftEvent gift)
        {
            Ped playerPed = Game.Player.Character;
            if (playerPed == null)
                return;

            Vector3 spawnPos = playerPed.Position + playerPed.ForwardVector * 5.0f;
            float heading = playerPed.Heading;

            Vehicle amb = World.CreateVehicle(
                new Model(VehicleHash.Ambulance),
                spawnPos,
                heading
            );

            if (amb != null && amb.Exists())
            {
                _spawnedAmbulances.Add(amb);

                // FIFO 上限：超过 10 辆则删除最早生成的
                while (_spawnedAmbulances.Count > MaxAmbulances)
                {
                    Vehicle oldest = _spawnedAmbulances[0];
                    if (oldest != null && oldest.Exists())
                    {
                        oldest.Delete();
                    }
                    _spawnedAmbulances.RemoveAt(0);
                }

                LogGameplay("救护车已生成: " + gift.Nickname + " 赠送 " + gift.GiftName + "，总数 " + _spawnedAmbulances.Count);
            }
        }

        private void CleanExpiredCooldowns()
        {
            DateTime threshold = DateTime.UtcNow - BackupCooldown;
            var expiredKeys = new List<string>();
            foreach (var kvp in _userCooldowns)
            {
                if (kvp.Value < threshold)
                {
                    expiredKeys.Add(kvp.Key);
                }
            }
            foreach (string key in expiredKeys)
            {
                _userCooldowns.Remove(key);
            }
        }
    }
}
