using System;
using GTA;
using ModProject.LiveStudio;

namespace ModProject
{
    // Stable runtime shell. Dify-generated gameplay lives in GeneratedGameplay.cs.
    public partial class Mod : Script
    {
        private readonly LiveStudioClient _client;

        public Mod()
        {
            Tick += OnTick;
            Aborted += OnAborted;

            InitializeGameplay();

            _client = new LiveStudioClient(
                onEvent: HandleEvent,
                onLog: message => MainThreadDispatcher.Enqueue(
                    () => GTA.UI.Notification.Show("~y~" + message)));
            _client.Start();

            GTA.UI.Notification.Show("~g~LIVE Studio mod loaded");
        }

        private void OnTick(object sender, EventArgs e)
        {
            MainThreadDispatcher.DrainOnTick();
            OnGameplayTick();
        }

        private void OnAborted(object sender, EventArgs e)
        {
            try
            {
                OnGameplayAborted();
            }
            finally
            {
                _client?.Dispose();
                Tick -= OnTick;
                Aborted -= OnAborted;
            }
        }

        // Runs on the LIVE Studio WebSocket thread. Generated handlers must enqueue GTA work.
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

        private static bool EnqueueGameplay(Action action)
        {
            return MainThreadDispatcher.Enqueue(action);
        }

        private static void LogGameplay(string message)
        {
            MainThreadDispatcher.Enqueue(
                () => GTA.UI.Notification.Show("~b~" + (message ?? string.Empty)));
        }

        partial void InitializeGameplay();
        partial void OnGameplayTick();
        partial void OnGameplayAborted();
        partial void OnChat(ChatEvent chat);
        partial void OnGift(GiftEvent gift);
    }
}
