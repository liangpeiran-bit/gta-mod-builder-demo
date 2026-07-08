using System;
using GTA;
using GTA.UI;
using ModProject.LiveStudio;

namespace ModProject
{
    public class Mod : Script
    {
        private readonly LiveStudioClient _client;
        private DateTime _lastAnyEffectAt = DateTime.MinValue;

        public Mod()
        {
            Tick += OnTick;
            Aborted += OnAborted;

            _client = new LiveStudioClient(
                onEvent: HandleEvent,
                onLog: msg => MainThreadDispatcher.Enqueue(() => Notification.Show("~y~" + msg)));
            _client.Start();

            Notification.Show("~g~LIVE Studio mod loaded");
        }

        private void OnTick(object sender, EventArgs e)
        {
            MainThreadDispatcher.DrainOnTick();
        }

        private void OnAborted(object sender, EventArgs e)
        {
            _client?.Dispose();
        }

        // Runs on the LIVE Studio WebSocket thread. Schedule game work via MainThreadDispatcher.
        private void HandleEvent(LiveStudioEvent evt)
        {
            switch (evt)
            {
                case ChatEvent chat:
                    OnChat(chat);
                    break;
                case GiftEvent gift:
                    OnGift(gift);
                    break;
            }
        }

        private void OnChat(ChatEvent chat)
        {
            if (string.IsNullOrEmpty(chat.Content)) return;

            // Replace this placeholder with DESIGN.md chat trigger routing.
            if (chat.Content.Trim().Equals("hello", StringComparison.OrdinalIgnoreCase))
            {
                EnqueueWithCooldown(() =>
                {
                    Notification.Show("~b~Hello from " + chat.Nickname);
                });
            }
        }

        private void OnGift(GiftEvent gift)
        {
            // Replace this placeholder with DESIGN.md gift_id routing.
            // One-shot gift effects normally wait for RepeatEnd so combo gifts do not fire N times.
            if (gift.GiftId == "5655" && gift.RepeatEnd) // Rose starter example
            {
                EnqueueWithCooldown(() =>
                {
                    Notification.Show("~p~" + gift.Nickname + " sent Rose");
                });
            }
        }

        private void EnqueueWithCooldown(Action action)
        {
            var now = DateTime.UtcNow;
            if ((now - _lastAnyEffectAt).TotalSeconds < 3)
            {
                return;
            }
            _lastAnyEffectAt = now;
            MainThreadDispatcher.Enqueue(action);
        }
    }
}
