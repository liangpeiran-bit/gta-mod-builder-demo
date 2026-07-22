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
        // ── cooldown & dedup state ──
        private DateTime _lastHelloTime = DateTime.MinValue;
        private DateTime _lastPoliceSpawnTime = DateTime.MinValue;
        private HashSet<string> _processedMsgIds;

        // ── constants ──
        private static readonly TimeSpan HelloCooldown = TimeSpan.FromSeconds(3);
        private static readonly TimeSpan PoliceCooldown = TimeSpan.FromSeconds(5);
        private const int MaxMsgIdCache = 200;

        private static readonly VehicleHash[] PoliceVehicles = new VehicleHash[]
        {
            VehicleHash.Police,
            VehicleHash.Police2,
            VehicleHash.Police3,
            VehicleHash.Police4
        };

        private static readonly Random Rng = new Random();

        // ── lifecycle ──

        partial void InitializeGameplay()
        {
            _processedMsgIds = new HashSet<string>();
            _lastHelloTime = DateTime.MinValue;
            _lastPoliceSpawnTime = DateTime.MinValue;
            LogGameplay("Gameplay initialized: Hello + Police Gift mod ready.");
        }

        partial void OnGameplayTick()
        {
            // No per-frame logic required.  Cooldowns are checked on each event.
        }

        partial void OnGameplayAborted()
        {
            // Only clean up internal state; never remove spawned police cars.
            if (_processedMsgIds != null)
            {
                _processedMsgIds.Clear();
                _processedMsgIds = null;
            }
            LogGameplay("Gameplay aborted: internal state cleared.");
        }

        // ── event handlers ──

        partial void OnChat(ChatEvent chat)
        {
            if (chat == null || string.IsNullOrEmpty(chat.Content))
                return;

            // Exact match "hello" (case-insensitive), whole message only.
            if (!chat.Content.Trim().Equals("hello", StringComparison.OrdinalIgnoreCase))
                return;

            DateTime now = DateTime.UtcNow;
            if (now - _lastHelloTime < HelloCooldown)
                return;

            _lastHelloTime = now;

            string nickname = chat.Nickname ?? "Unknown";
            GTA.UI.Notification.Show($"🎉 {nickname} says Hello!");
            LogGameplay($"Hello from {nickname}");
        }

        partial void OnGift(GiftEvent gift)
        {
            if (gift == null)
                return;

            // Only trigger once per combo — when combo ends.
            if (!gift.RepeatEnd)
                return;

            DateTime now = DateTime.UtcNow;
            if (now - _lastPoliceSpawnTime < PoliceCooldown)
                return;

            // Deduplicate by msgId.
            if (!string.IsNullOrEmpty(gift.MsgId) && _processedMsgIds.Contains(gift.MsgId))
                return;

            _lastPoliceSpawnTime = now;

            if (!string.IsNullOrEmpty(gift.MsgId))
            {
                // Prevent unbounded growth.
                if (_processedMsgIds.Count >= MaxMsgIdCache)
                {
                    _processedMsgIds.Clear();
                }
                _processedMsgIds.Add(gift.MsgId);
            }

            try
            {
                Ped playerPed = Game.Player.Character;
                Vector3 playerPos = playerPed.Position;
                Vector3 spawnPos = World.GetNextPositionOnStreet(playerPos);

                VehicleHash model = PoliceVehicles[Rng.Next(PoliceVehicles.Length)];
                World.CreateVehicle(model, spawnPos, playerPed.Heading);

                string nickname = gift.Nickname ?? "Unknown";
                GTA.UI.Notification.Show($"🚔 {nickname} 的连击礼物召唤了一辆警车！");
                LogGameplay($"Police car ({model}) spawned for {nickname} at {spawnPos}");
            }
            catch (Exception ex)
            {
                LogGameplay($"Failed to spawn police car: {ex.Message}");
            }
        }
    }
}