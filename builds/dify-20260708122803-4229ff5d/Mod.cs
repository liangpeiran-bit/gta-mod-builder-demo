using System;
using GTA;
using GTA.Math;
using GTA.UI;
using ModProject.LiveStudio;

namespace ModProject
{
    public class Mod : Script
    {
        private LiveStudioClient _client;
        private DateTime _lastSpawnTime;
        private static readonly TimeSpan Cooldown = TimeSpan.FromSeconds(30);
        private static readonly string[] CarModels = { "COMET", "TURISMOR", "CHEETAH", "INFERNUS", "BANSHEE", "COQUETTE" };
        private static readonly Random _rng = new Random();

        public Mod()
        {
            _lastSpawnTime = DateTime.MinValue;
            Tick += OnTick;
            Aborted += OnAborted;
            _client = new LiveStudioClient(HandleEvent, msg => MainThreadDispatcher.Enqueue(() => GTA.UI.Notification.Show("~y~" + msg)));
            _client.Start();
        }

        private void HandleEvent(LiveStudioEvent evt)
        {
            bool shouldSpawn = false;
            if (evt is ChatEvent chat && chat.Content.Trim().ToLower() == "!rosecar")
            {
                shouldSpawn = true;
            }
            else if (evt is GiftEvent gift && gift.GiftId == "rose")
            {
                // Single‑fire gift, ignore RepeatEnd
                shouldSpawn = true;
            }

            if (shouldSpawn && DateTime.Now - _lastSpawnTime >= Cooldown)
            {
                // Player‑state checks omitted due to SHVDN API allowlist; car will spawn anyway.
                MainThreadDispatcher.Enqueue(SpawnRandomSportsCar);
            }
        }

        private void SpawnRandomSportsCar()
        {
            // Double‑check cooldown in case of re‑entrancy
            if (DateTime.Now - _lastSpawnTime < Cooldown) return;

            Model vehicleModel = new Model(CarModels[_rng.Next(CarModels.Length)]);
            if (!vehicleModel.IsValid) return;

            Ped playerPed = Game.Player.Character;
            Vector3 basePos = playerPed.Position + playerPed.ForwardVector * 5f;
            float heading = playerPed.Heading;
            Vector3 right = playerPed.RightVector;

            // Lateral shuffle up to ±3 m
            float[] lateralOffsets = { 0f, 1.5f, -1.5f, 3f, -3f };
            foreach (float lat in lateralOffsets)
            {
                Vector3 testPos = basePos + right * lat;
                testPos.Z += 0.5f;
                Vehicle spawned = World.CreateVehicle(vehicleModel, testPos, heading);
                if (spawned != null)
                {
                    _lastSpawnTime = DateTime.Now;
                    GTA.UI.Notification.Show("A sports car has been delivered!");
                    return;
                }
            }

            GTA.UI.Notification.Show("No space to spawn the car!");
        }

        private void OnTick(object sender, EventArgs e)
        {
            MainThreadDispatcher.DrainOnTick();
        }

        private void OnAborted(object sender, EventArgs e)
        {
            _client?.Dispose();
        }
    }
}