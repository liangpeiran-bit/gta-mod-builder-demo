using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using ModProject.LiveStudio;

namespace ModProject
{
    public partial class Mod
    {
        // ── Chat "backup" per-user cooldown ──
        private Dictionary<string, DateTime> _backupCooldowns;
        private const double BackupCooldownSec = 3.0;

        // ── Gift ambulance spawn global cooldown ──
        private DateTime _lastGiftSpawnUtc;
        private const double GiftCooldownSec = 5.0;

        // ── Gift msgId dedup ──
        private HashSet<string> _giftMsgIds;
        private const int MaxGiftMsgIds = 500;

        // ── Periodic cleanup tick ──
        private int _cleanupTick;
        private const int CleanupEveryNTicks = 600; // ≈10 s @ 60 fps

        // ── Fallback vehicle when Ambulance fails ──
        private const VehicleHash FallbackVehicleHash = VehicleHash.Burrito;

        // ═══════════════════════════════════════════════
        //  INITIALIZATION
        // ═══════════════════════════════════════════════
        partial void InitializeGameplay()
        {
            _backupCooldowns = new Dictionary<string, DateTime>();
            _giftMsgIds = new HashSet<string>();
            _lastGiftSpawnUtc = DateTime.MinValue;
            _cleanupTick = 0;
        }

        // ═══════════════════════════════════════════════
        //  TICK – periodic housekeeping
        // ═══════════════════════════════════════════════
        partial void OnGameplayTick()
        {
            _cleanupTick++;
            if (_cleanupTick >= CleanupEveryNTicks)
            {
                _cleanupTick = 0;
                CleanupStaleCooldowns();
            }
        }

        // ═══════════════════════════════════════════════
        //  ABORTED – release all gameplay state
        // ═══════════════════════════════════════════════
        partial void OnGameplayAborted()
        {
            _backupCooldowns?.Clear();
            _giftMsgIds?.Clear();
            _lastGiftSpawnUtc = DateTime.MinValue;
            _cleanupTick = 0;
        }

        // ═══════════════════════════════════════════════
        //  CHAT – "backup" command
        // ═══════════════════════════════════════════════
        partial void OnChat(ChatEvent chat)
        {
            if (chat == null || string.IsNullOrEmpty(chat.Content))
                return;

            // Case-insensitive exact match after trim
            if (!string.Equals(chat.Content.Trim(), "backup", StringComparison.OrdinalIgnoreCase))
                return;

            string userId = chat.UserId ?? string.Empty;
            DateTime now = DateTime.UtcNow;

            // Per-user cooldown
            if (_backupCooldowns.TryGetValue(userId, out DateTime last))
            {
                if ((now - last).TotalSeconds < BackupCooldownSec)
                    return;
            }

            _backupCooldowns[userId] = now;

            string nickname = chat.Nickname ?? "观众";
            string notifyText = nickname + " 请求支援";
            GTA.UI.Notification.Show(notifyText);

            LogGameplay("backup triggered – user: " + nickname);
        }

        // ═══════════════════════════════════════════════
        //  GIFT – ambulance spawn on combo end
        // ═══════════════════════════════════════════════
        partial void OnGift(GiftEvent gift)
        {
            if (gift == null)
                return;

            // Only combo-end events
            if (!gift.RepeatEnd)
                return;

            // msgId dedup
            string msgId = gift.MsgId;
            if (!string.IsNullOrEmpty(msgId))
            {
                if (_giftMsgIds.Contains(msgId))
                    return;

                if (_giftMsgIds.Count >= MaxGiftMsgIds)
                {
                    _giftMsgIds.Clear();
                    LogGameplay("Gift dedup set cleared (cap reached).");
                }

                _giftMsgIds.Add(msgId);
            }

            // Global cooldown
            DateTime now = DateTime.UtcNow;
            if ((now - _lastGiftSpawnUtc).TotalSeconds < GiftCooldownSec)
                return;

            _lastGiftSpawnUtc = now;

            // Spawn
            SpawnAmbulance(gift);
        }

        // ═══════════════════════════════════════════════
        //  PRIVATE – ambulance spawn logic
        // ═══════════════════════════════════════════════
        private void SpawnAmbulance(GiftEvent gift)
        {
            try
            {
                Ped playerPed = Game.Player?.Character;
                if (playerPed == null)
                {
                    LogGameplay("Cannot spawn ambulance: player ped unavailable.");
                    return;
                }

                Vector3 playerPos = playerPed.Position;
                Vector3 spawnPos = World.GetNextPositionOnStreet(playerPos, true);

                // Fallback 1: GetNextPositionOnStreet may return zero vector
                if (spawnPos.X == 0f && spawnPos.Y == 0f && spawnPos.Z == 0f)
                {
                    Vector3 probe = playerPos + new Vector3(5f, 0f, 0f);
                    spawnPos = World.GetSafeCoordForPed(probe, true, 0);
                }

                // Fallback 2: GetSafeCoordForPed may also return zero vector
                if (spawnPos.X == 0f && spawnPos.Y == 0f && spawnPos.Z == 0f)
                {
                    spawnPos = playerPos + new Vector3(0f, 5f, 0f);
                }

                // Heading from spawn position toward player
                float heading = (float)(Math.Atan2(
                    playerPos.X - spawnPos.X,
                    playerPos.Y - spawnPos.Y) * (180.0 / Math.PI));

                // Create vehicle – Ambulance first, fallback to Burrito
                Vehicle vehicle = null;
                VehicleHash[] modelsToTry = { VehicleHash.Ambulance, FallbackVehicleHash };

                foreach (VehicleHash vh in modelsToTry)
                {
                    try
                    {
                        vehicle = World.CreateVehicle(vh, spawnPos, heading);
                        if (vehicle != null)
                            break;
                    }
                    catch (Exception ex)
                    {
                        LogGameplay("CreateVehicle failed for " + vh + ": " + ex.Message);
                    }
                }

                if (vehicle == null)
                {
                    LogGameplay("All vehicle models failed – no ambulance spawned.");
                    return;
                }

                vehicle.IsPersistent = false;
                vehicle.PlaceOnGround();

                string gifter = gift.Nickname ?? "观众";
                LogGameplay("Ambulance spawned near player. Gift from: " + gifter);
            }
            catch (Exception ex)
            {
                LogGameplay("SpawnAmbulance error: " + ex.Message);
            }
        }

        // ═══════════════════════════════════════════════
        //  PRIVATE – periodic cooldown-dict cleanup
        // ═══════════════════════════════════════════════
        private void CleanupStaleCooldowns()
        {
            DateTime threshold = DateTime.UtcNow.AddSeconds(-60.0);
            var stale = new List<string>();

            foreach (var kv in _backupCooldowns)
            {
                if (kv.Value < threshold)
                    stale.Add(kv.Key);
            }

            foreach (string key in stale)
            {
                _backupCooldowns.Remove(key);
            }
        }
    }
}
