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
        private int _chatCooldown;
        private int _giftCooldown;
        private const int CooldownMs = 10000;
        private readonly List<Vehicle> _spawnedVehicles = new List<Vehicle>();

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

        private void HandleEvent(LiveStudioEvent evt)
        {
            if (evt is ChatEvent chat)
            {
                HandleChat(chat);
            }
            else if (evt is GiftEvent gift)
            {
                HandleGift(gift);
            }
        }

        private void HandleChat(ChatEvent chat)
        {
            if (!string.Equals(chat.Content, "ping", StringComparison.OrdinalIgnoreCase))
                return;

            int now = Game.GameTime;
            if (now - _chatCooldown < CooldownMs)
                return;

            _chatCooldown = now;

            MainThreadDispatcher.Enqueue(() =>
            {
                Ped player = Game.Player.Character;
                if (player == null || !player.Exists() || player.IsDead)
                    return;

                Function.Call(Hash.ADD_ARMOUR_TO_PED, player, 25);
                GTA.UI.Notification.Show("PING! +25 Armor");
            });
        }

        private void HandleGift(GiftEvent gift)
        {
            if (gift.GiftId != "rose" || !gift.RepeatEnd)
                return;

            int now = Game.GameTime;
            if (now - _giftCooldown < CooldownMs)
                return;

            _giftCooldown = now;

            MainThreadDispatcher.Enqueue(() =>
            {
                Ped player = Game.Player.Character;
                if (player == null || !player.Exists() || player.IsDead)
                    return;

                Vector3 pos = player.Position;
                Vector3 streetPos;
                float heading;
                streetPos = World.GetNextPositionOnStreet(pos.Around(2.0f));
                heading = Game.Player.Character.Heading;

                Vehicle vehicle = World.CreateVehicle(VehicleHash.Police, streetPos, heading);
                if (vehicle == null || !vehicle.Exists())
                {
                    GTA.UI.Notification.Show("Rose gift: Couldn't spawn the police car.");
                    return;
                }

                vehicle.IsInvincible = true;
                _spawnedVehicles.Add(vehicle);
                GTA.UI.Notification.Show("Rose gift: Invincible Police Car!");
            });
        }

        private void OnTick(object sender, EventArgs e)
        {
            MainThreadDispatcher.DrainOnTick();
        }

        private void OnAborted(object sender, EventArgs e)
        {
            foreach (Vehicle vehicle in _spawnedVehicles)
            {
                if (vehicle != null && vehicle.Exists())
                {
                    vehicle.Delete();
                }
            }
            _spawnedVehicles.Clear();
            _client?.Dispose();
        }
    }
}