using System;
using GTA;
using ModProject.LiveStudio;

namespace ModProject
{
    public partial class Mod
    {
        private enum SurvivalState
        {
            Idle,
            Survival,
            Cooldown
        }

        private SurvivalState _state = SurvivalState.Idle;
        private DateTime _cooldownEndTime;
        private const int CooldownSeconds = 30;

        partial void InitializeGameplay()
        {
            _state = SurvivalState.Idle;
            LogGameplay("Rose Survival initialized. Waiting for Rose gift (id=5655)...");
        }

        partial void OnGameplayTick()
        {
            if (_state == SurvivalState.Survival)
            {
                if (Game.Player.Character.IsDead)
                {
                    _state = SurvivalState.Cooldown;
                    _cooldownEndTime = DateTime.UtcNow.AddSeconds(CooldownSeconds);
                    LogGameplay("Player died. Entering cooldown for " + CooldownSeconds + " seconds.");
                }
            }
            else if (_state == SurvivalState.Cooldown)
            {
                if (DateTime.UtcNow >= _cooldownEndTime)
                {
                    _state = SurvivalState.Idle;
                    LogGameplay("Cooldown ended. Ready for next Rose survival challenge.");
                }
            }
        }

        partial void OnGameplayAborted()
        {
            _state = SurvivalState.Idle;
            LogGameplay("Rose Survival aborted. State reset to Idle.");
        }

        partial void OnChat(ChatEvent chat)
        {
        }

        partial void OnGift(GiftEvent gift)
        {
            TriggerGiftOnce(gift, "5655", matchedGift =>
            {
                if (_state != SurvivalState.Idle)
                {
                    LogGameplay("Rose from " + matchedGift.Nickname + " ignored (current state: " + _state + ").");
                    return;
                }

                _state = SurvivalState.Survival;

                try
                {
                    Game.Player.WantedLevel = 5;
                }
                catch (Exception ex)
                {
                    LogGameplay("Failed to set wanted level: " + ex.Message + ". Returning to Idle.");
                    _state = SurvivalState.Idle;
                    return;
                }

                try
                {
                    Game.Player.Character.Weapons.Give(WeaponHash.SMG, 1000, true, true);
                }
                catch (Exception ex)
                {
                    LogGameplay("Failed to give SMG: " + ex.Message + ". Wanted level still applied.");
                }

                LogGameplay("Rose from " + matchedGift.Nickname
                    + " | 5 stars + SMG (1000 rounds). Survive as long as possible!");
            });
        }
    }
}
