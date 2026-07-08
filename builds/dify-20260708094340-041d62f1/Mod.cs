using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.UI;
using ModProject.LiveStudio;

namespace ModProject
{
    public class Mod : Script
    {
        private LiveStudioClient _client;
        private List<Vehicle> _spawnedVehicles = new List<Vehicle>();

        public Mod()
        {
            Tick += OnTick;
            Aborted += OnAborted;

            _client = new LiveStudioClient(HandleEvent, msg =>
            {
                MainThreadDispatcher.Enqueue(() => GTA.UI.Notification.Show("~y~" + msg));
            });
            _client.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            MainThreadDispatcher.DrainOnTick();
        }

        private void OnAborted(object sender, EventArgs e)
        {
            _client?.Dispose();
            foreach (Vehicle vehicle in _spawnedVehicles)
            {
                if (vehicle != null && vehicle.Exists())
                {
                    vehicle.Delete();
                }
            }
            _spawnedVehicles.Clear();
        }

        private void HandleEvent(LiveStudioEvent evt)
        {
            if (evt is ChatEvent chat)
            {
                if (chat.Content.IndexOf("ping", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    MainThreadDispatcher.Enqueue(() =>
                    {
                        Ped player = Game.Player.Character;
                        if (player.IsDead)
                        {
                            return;
                        }

                        GTA.UI.Screen.ShowSubtitle("Ping received! +25 Armor");
                        int armor = player.Armor;
                        armor = Math.Min(armor + 25, 100);
                        player.Armor = armor;
                    });
                }
            }
            else if (evt is GiftEvent gift)
            {
                if (gift.GiftId == "rose" && gift.RepeatEnd)
                {
                    MainThreadDispatcher.Enqueue(() =>
                    {
                        Ped player = Game.Player.Character;
                        if (player.IsDead)
                        {
                            return;
                        }

                        Vector3 offsetPos = player.GetOffsetPosition(new Vector3(0, 5, 0));
                        Vector3 streetPos = World.GetNextPositionOnStreet(offsetPos);
                        float heading = player.Heading;

                        Vehicle policeCar = World.CreateVehicle(VehicleHash.Police, streetPos, heading);
                        if (policeCar != null)
                        {
                            policeCar.IsInvincible = true;
                            policeCar.PlaceOnGround();
                            _spawnedVehicles.Add(policeCar);
                        }
                    });
                }
            }
        }
    }
}