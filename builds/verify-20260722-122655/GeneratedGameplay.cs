using System;
using ModProject.LiveStudio;

namespace ModProject
{
    // This file is replaced by the Dify-generated gameplay implementation.
    public partial class Mod
    {
        private DateTime _lastAnyEffectAt = DateTime.MinValue;

        partial void InitializeGameplay()
        {
        }

        partial void OnGameplayTick()
        {
        }

        partial void OnGameplayAborted()
        {
        }

        partial void OnChat(ChatEvent chat)
        {
            if (string.IsNullOrEmpty(chat.Content)) return;

            if (chat.Content.Trim().Equals("hello", StringComparison.OrdinalIgnoreCase))
            {
                EnqueueWithCooldown(() =>
                    GTA.UI.Notification.Show("~b~Hello from " + chat.Nickname));
            }
        }

        partial void OnGift(GiftEvent gift)
        {
            if (gift.GiftId == "5655" && gift.RepeatEnd)
            {
                EnqueueWithCooldown(() =>
                    GTA.UI.Notification.Show("~p~" + gift.Nickname + " sent Rose"));
            }
        }

        private void EnqueueWithCooldown(Action action)
        {
            var now = DateTime.UtcNow;
            if ((now - _lastAnyEffectAt).TotalSeconds < 3) return;
            _lastAnyEffectAt = now;
            EnqueueGameplay(action);
        }
    }
}
