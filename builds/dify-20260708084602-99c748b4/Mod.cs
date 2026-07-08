using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;
using ModProject.LiveStudio;

namespace ModProject
{
    public class Mod : Script
    {
        private LiveStudioClient _client;
        private DateTime _lastPingTime = DateTime.MinValue;
        private DateTime _lastRoseTime = DateTime.MinValue;
        private Vehicle _lastPoliceVehicle;
        private const int PingCooldownSeconds = 5;
        private const int RoseCooldownSeconds = 15;

        public Mod()
        {
            Tick += OnTick;
            Aborted += OnAborted;

            _client = new LiveStudioClient(
                onEvent: HandleEvent,
                onLog: msg => MainThreadDispatcher.Enqueue(() => GTA.UI.Notification.Show("~y~" + msg))
            );
            _client.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            MainThreadDispatcher.DrainOnTick();
        }

        private void OnAborted(object sender, EventArgs e)
        {
            _client?.Dispose();
            if (_lastPoliceVehicle != null && _lastPoliceVehicle.Exists())
            {
                _lastPoliceVehicle.Delete();
            }
        }

        private void HandleEvent(LiveStudioEvent evt)
        {
            if (evt is ChatEvent chat)
            {
                MainThreadDispatcher.Enqueue(() => HandleChat(chat));
            }
            else if (evt is GiftEvent gift)
            {
                MainThreadDispatcher.Enqueue(() => HandleGift(gift));
            }
        }

        private void HandleChat(ChatEvent chat)
        {
            if (chat.Content.ToLower().Contains("ping"))
            {
                if ((DateTime.UtcNow - _lastPingTime).TotalSeconds < PingCooldownSeconds)
                    return;

                _lastPingTime = DateTime.UtcNow;

                Ped playerPed = Game.Player.Character;
                if (playerPed == null || !playerPed.Exists())
                    return;

                int currentArmor = playerPed.Armor;
                int newArmor = Math.Min(currentArmor + 25, 100);
                playerPed.Armor = newArmor;

                GTA.UI.Notification.Show("Ping received! +25 Armor");
            }
        }

        private void HandleGift(GiftEvent gift)
        {
            if (gift.GiftId == "rose" && !gift.RepeatEnd)
            {
                if ((DateTime.UtcNow - _lastRoseTime).TotalSeconds < RoseCooldownSeconds)
                    return;

                _lastRoseTime = DateTime.UtcNow;

                Ped playerPed = Game.Player.Character;
                if (playerPed == null || !playerPed.Exists() || !playerPed.IsAlive)
                    return;

                if (_lastPoliceVehicle != null && _lastPoliceVehicle.Exists())
                {
                    _lastPoliceVehicle.Delete();
                }

                Model policeModel = new Model("POLICE");
                if (!policeModel.IsValid)
                {
                    GTA.UI.Notification.Show("~r~Failed to load police car model");
                    return;
                }

                Vector3 spawnPos = playerPed.Position + playerPed.ForwardVector * 5f;
                float heading = playerPed.Heading;

                Vehicle policeCar = World.CreateVehicle(policeModel, spawnPos, heading);
                if (policeCar == null || !policeCar.Exists())
                {
                    GTA.UI.Notification.Show("~r~Failed to spawn police car");
                    return;
                }

                policeCar.IsInvincible = true;
                _lastPoliceVehicle = policeCar;
            }
        }
    }
}