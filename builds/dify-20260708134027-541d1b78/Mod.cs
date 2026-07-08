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
        }

        private void HandleEvent(LiveStudioEvent evt)
        {
            if (evt is GiftEvent gift && gift.GiftId == "5655")
            {
                MainThreadDispatcher.Enqueue(SpawnSportsCar);
            }
        }

        private void SpawnSportsCar()
        {
            Ped playerPed = Game.Player.Character;
            if (playerPed == null || !playerPed.Exists()) return;

            Vector3 playerPos = playerPed.Position;
            Vector3 streetPos = World.GetNextPositionOnStreet(playerPos);
            float heading = playerPed.Heading;

            Vehicle vehicle = World.CreateVehicle(VehicleHash.Comet2, streetPos, heading);
            if (vehicle != null && vehicle.Exists())
            {
                vehicle.IsEngineRunning = true;
                GTA.UI.Notification.Show("A sports car has been gifted!");
            }
        }
    }
}