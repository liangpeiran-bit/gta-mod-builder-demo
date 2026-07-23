using System;
using GTA;
using ModProject.LiveStudio;

namespace ModProject
{
    public partial class Mod
    {
        // ── 状态机 ────────────────────────────────────────
        private enum SurvivalState
        {
            Idle,
            Active,
            Victory,
            Failure
        }

        private SurvivalState _survivalState = SurvivalState.Idle;
        private int _survivalStartTime = 0;
        private const int SurvivalDurationMs = 120000; // 120 秒

        // ── 冷却 ──────────────────────────────────────────
        private DateTime _lastTriggerTime = DateTime.MinValue;
        private const double CooldownSeconds = 300.0;

        // ── 关系组（用于清理恢复） ─────────────────────────
        private RelationshipGroup[] _civGroups = null;
        private RelationshipGroup _playerGroup;
        private bool _groupsSaved = false;

        // ── 礼物 ID ───────────────────────────────────────
        private const string RoseGiftId = "5655";

        // ──────────────────────────────────────────────────
        // partial hooks
        // ──────────────────────────────────────────────────

        partial void InitializeGameplay()
        {
            // 字段已有默认值，无需额外初始化
        }

        partial void OnGameplayTick()
        {
            if (_survivalState == SurvivalState.Active)
            {
                int elapsed = Game.GameTime - _survivalStartTime;

                // 检查玩家死亡 → 失败
                if (Game.Player.Character.IsDead)
                {
                    _survivalState = SurvivalState.Failure;
                    LogGameplay("Rose survival FAILED — player died after " + (elapsed / 1000) + "s");
                    CleanupSurvival(false); // 死亡时不清除通缉，由 GTA 自然处理
                    return;
                }

                // 检查计时到期 → 胜利
                if (elapsed >= SurvivalDurationMs)
                {
                    _survivalState = SurvivalState.Victory;
                    LogGameplay("Rose survival VICTORY! Survived full 120 seconds!");
                    CleanupSurvival(true); // 胜利时清除通缉
                    return;
                }
            }
        }

        partial void OnGameplayAborted()
        {
            // 强制终止时完全清理
            CleanupSurvival(true);
            _survivalState = SurvivalState.Idle;
            LogGameplay("Rose survival aborted and cleaned up");
        }

        partial void OnChat(ChatEvent chat)
        {
            // 本玩法不处理聊天事件
        }

        partial void OnGift(GiftEvent gift)
        {
            TriggerGiftOnce(gift, RoseGiftId, matchedGift =>
            {
                // 双重检查：状态互斥
                if (_survivalState != SurvivalState.Idle)
                {
                    LogGameplay("Rose survival already active or pending cleanup, ignoring gift from " + matchedGift.Nickname);
                    return;
                }

                // 冷却检查
                double secondsSinceLastTrigger = (DateTime.UtcNow - _lastTriggerTime).TotalSeconds;
                if (secondsSinceLastTrigger < CooldownSeconds)
                {
                    double remaining = CooldownSeconds - secondsSinceLastTrigger;
                    LogGameplay("Rose survival on cooldown (" + (int)remaining + "s remaining), ignoring gift from " + matchedGift.Nickname);
                    return;
                }

                // 启动生存模式
                StartSurvival(matchedGift);
            });
        }

        // ──────────────────────────────────────────────────
        // 核心玩法逻辑
        // ──────────────────────────────────────────────────

        private void StartSurvival(GiftEvent gift)
        {
            // 记录冷却起始时间
            _lastTriggerTime = DateTime.UtcNow;

            // 保存关系组引用
            _playerGroup = Game.Player.Character.RelationshipGroup;

            // 覆盖常见平民关系组
            _civGroups = new RelationshipGroup[]
            {
                new RelationshipGroup(Game.GenerateHash("CIVMALE")),
                new RelationshipGroup(Game.GenerateHash("CIVFEMALE"))
            };
            _groupsSaved = true;

            // 1. 通缉等级 → 5 星
            Game.Player.WantedLevel = 5;

            // 2. 平民关系组对玩家敌对
            for (int i = 0; i < _civGroups.Length; i++)
            {
                _civGroups[i].SetRelationshipBetweenGroups(_playerGroup, Relationship.Hate, true);
            }

            // 3. 给予 SMG 冲锋枪 + 1000 发子弹，立即装备
            Game.Player.Character.Weapons.Give(WeaponHash.SMG, 1000, true, true);

            // 4. 补满护甲
            Game.Player.Character.Armor = Game.Player.MaxArmor;

            // 5. 启动生存计时
            _survivalStartTime = Game.GameTime;
            _survivalState = SurvivalState.Active;

            LogGameplay("ROSES ARE RED... " + gift.Nickname + " triggered Rose Survival! 5 stars, SMG equipped, survive 120s!");
        }

        private void CleanupSurvival(bool clearWantedLevel)
        {
            // 恢复平民关系组
            if (_groupsSaved && _civGroups != null)
            {
                for (int i = 0; i < _civGroups.Length; i++)
                {
                    _civGroups[i].SetRelationshipBetweenGroups(_playerGroup, Relationship.Neutral, true);
                }
                _civGroups = null;
                _groupsSaved = false;
            }

            // 通缉等级处理
            if (clearWantedLevel)
            {
                Game.Player.WantedLevel = 0;
            }

            // 重置状态
            _survivalState = SurvivalState.Idle;
            _survivalStartTime = 0;
        }
    }
}
