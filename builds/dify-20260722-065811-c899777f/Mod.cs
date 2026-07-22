using System;
using System.Collections.Generic;
using System.Threading;
using GTA;
using ModProject.LiveStudio;

namespace ModProject
{
    // Stable runtime shell. Dify-generated gameplay lives in GeneratedGameplay.cs.
    public partial class Mod : Script
    {
        private static readonly TimeSpan GiftTerminalFallbackDelay = TimeSpan.FromMilliseconds(1200);
        private const int MaxPendingGiftTriggers = 256;

        private readonly LiveStudioClient _client;
        private readonly Dictionary<string, PendingGiftTrigger> _pendingGiftTriggers =
            new Dictionary<string, PendingGiftTrigger>(StringComparer.Ordinal);
        private int _droppedRuntimeMessages;

        public Mod()
        {
            Tick += OnTick;
            Aborted += OnAborted;

            InitializeGameplay();

            _client = new LiveStudioClient(
                onEvent: HandleEvent,
                onLog: QueueRuntimeLog);
            _client.Start();

            GTA.UI.Notification.Show("~g~LIVE Studio mod loaded");
        }

        private void OnTick(object sender, EventArgs e)
        {
            MainThreadDispatcher.DrainOnTick();
            FlushPendingGiftTriggers();
            ReportDroppedRuntimeMessages();
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
                _pendingGiftTriggers.Clear();
                _client?.Dispose();
                Tick -= OnTick;
                Aborted -= OnAborted;
            }
        }

        // Runs on the LIVE Studio WebSocket thread. The fixed runtime owns the thread hop.
        private void HandleEvent(LiveStudioEvent evt)
        {
            if (evt == null) return;

            if (!MainThreadDispatcher.Enqueue(() => DispatchEventOnMainThread(evt)))
            {
                Interlocked.Increment(ref _droppedRuntimeMessages);
            }
        }

        // Runs only from Script.Tick on the GTA main thread.
        private void DispatchEventOnMainThread(LiveStudioEvent evt)
        {
            switch (evt)
            {
                case ChatEvent chat:
                    OnChat(chat);
                    break;
                case GiftEvent gift:
                    ShowRuntimeLog(
                        "Gift dispatched: " + GiftLabel(gift) +
                        " repeat=" + gift.RepeatCount +
                        " end=" + gift.RepeatEnd);
                    OnGift(gift);
                    break;
            }
        }

        // Preferred helper for one-shot gift effects. It waits for repeatEnd, but also
        // falls back after a short quiet period when a LIVE Studio build omits it.
        private void TriggerGiftOnce(GiftEvent gift, string expectedGiftId, Action<GiftEvent> action)
        {
            if (!GiftMatches(gift, expectedGiftId) || action == null) return;

            var key = GiftKey(gift, expectedGiftId);
            if (gift.RepeatEnd)
            {
                _pendingGiftTriggers.Remove(key);
                ExecuteGiftAction(gift, action, "terminal event");
                return;
            }

            if (!_pendingGiftTriggers.ContainsKey(key) &&
                _pendingGiftTriggers.Count >= MaxPendingGiftTriggers)
            {
                ShowRuntimeLog("Gift ignored: pending gift queue is full");
                return;
            }

            _pendingGiftTriggers[key] = new PendingGiftTrigger(
                gift,
                action,
                DateTime.UtcNow + GiftTerminalFallbackDelay);
            ShowRuntimeLog("Gift matched; waiting for combo end: " + GiftLabel(gift));
        }

        // Use only when DESIGN.md explicitly requests an effect for every combo event.
        private void TriggerGiftEveryEvent(GiftEvent gift, string expectedGiftId, Action<GiftEvent> action)
        {
            if (!GiftMatches(gift, expectedGiftId) || action == null) return;
            ExecuteGiftAction(gift, action, "every event");
        }

        private void FlushPendingGiftTriggers()
        {
            if (_pendingGiftTriggers.Count == 0) return;

            var now = DateTime.UtcNow;
            var dueKeys = new List<string>();
            foreach (var pair in _pendingGiftTriggers)
            {
                if (pair.Value.DueAt <= now) dueKeys.Add(pair.Key);
            }

            foreach (var key in dueKeys)
            {
                if (!_pendingGiftTriggers.TryGetValue(key, out var pending)) continue;
                _pendingGiftTriggers.Remove(key);
                ExecuteGiftAction(pending.Gift, pending.Action, "terminal fallback");
            }
        }

        private static bool GiftMatches(GiftEvent gift, string expectedGiftId)
        {
            if (gift == null || string.IsNullOrWhiteSpace(expectedGiftId)) return false;
            return string.Equals(expectedGiftId, "*", StringComparison.Ordinal) ||
                   string.Equals(gift.GiftId, expectedGiftId, StringComparison.Ordinal);
        }

        private static string GiftKey(GiftEvent gift, string expectedGiftId)
        {
            var resolvedGiftId = string.Equals(expectedGiftId, "*", StringComparison.Ordinal)
                ? (gift.GiftId ?? string.Empty)
                : expectedGiftId;
            return (gift.UserId ?? string.Empty) + "|" + resolvedGiftId;
        }

        private void ExecuteGiftAction(
            GiftEvent gift,
            Action<GiftEvent> action,
            string triggerReason)
        {
            try
            {
                ShowRuntimeLog("Gift executing (" + triggerReason + "): " + GiftLabel(gift));
                action(gift);
                ShowRuntimeLog("Gift executed: " + GiftLabel(gift));
            }
            catch (Exception ex)
            {
                ShowRuntimeLog("Gift action failed: " + ex.Message);
            }
        }

        private void QueueRuntimeLog(string message)
        {
            if (!MainThreadDispatcher.Enqueue(() => ShowRuntimeLog(message)))
            {
                Interlocked.Increment(ref _droppedRuntimeMessages);
            }
        }

        private void ReportDroppedRuntimeMessages()
        {
            var dropped = Interlocked.Exchange(ref _droppedRuntimeMessages, 0);
            if (dropped > 0)
            {
                ShowRuntimeLog("Dropped " + dropped + " LIVE Studio event/log item(s): queue full");
            }
        }

        private static string GiftLabel(GiftEvent gift)
        {
            if (gift == null) return "unknown gift";
            var name = string.IsNullOrWhiteSpace(gift.GiftName) ? "gift" : gift.GiftName;
            return name + " [" + (gift.GiftId ?? "?") + "] from " +
                   (gift.Nickname ?? "viewer");
        }

        private static void ShowRuntimeLog(string message)
        {
            GTA.UI.Notification.Show("~y~" + (message ?? string.Empty));
        }

        private static void LogGameplay(string message)
        {
            GTA.UI.Notification.Show("~b~" + (message ?? string.Empty));
        }

        private sealed class PendingGiftTrigger
        {
            public GiftEvent Gift { get; }
            public Action<GiftEvent> Action { get; }
            public DateTime DueAt { get; }

            public PendingGiftTrigger(GiftEvent gift, Action<GiftEvent> action, DateTime dueAt)
            {
                Gift = gift;
                Action = action;
                DueAt = dueAt;
            }
        }

        partial void InitializeGameplay();
        partial void OnGameplayTick();
        partial void OnGameplayAborted();
        partial void OnChat(ChatEvent chat);
        partial void OnGift(GiftEvent gift);
    }
}
