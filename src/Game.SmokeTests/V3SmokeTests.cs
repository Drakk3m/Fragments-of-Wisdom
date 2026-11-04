using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Game.Domain.Alignment;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.Passives;
using Game.Domain.Combat.Resources;
using Game.Domain.Combat.Stats;
using Game.Domain.Combat.StatusEffects;
using Game.Domain.Codex;
using Game.Domain.Discovery;
using Game.Domain.Equipment;
using Game.Domain.Missions;
using Game.Domain.Narrative;
using Game.Domain.Player;
using Game.Domain.Progress;
using Game.Domain.Skills;
using Game.Domain.Transmutation;
using Game.Domain.World;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunV3SmokeTests()
        {
            RunSection("AUD-06 refill turn lifecycle assertions", RunRefillTurnLifecycleSmokeTests);

            RunSection(
                "Combat core assertions",
                delegate
                {
                    AssertBasicAttackDamagesTarget();
                    AssertDefeatedTargetIsDetected();
                    AssertMissCausesNoDamage();
                    AssertCriticalHitIncreasesDamage();
                    AssertSkillCannotRunWhenCostCannotBePaid();
                });

            RunSection(
                "Stat modifier foundation assertions",
                delegate
                {
                    AssertFlatAttackModifierIncreasesEffectiveAttack();
                    AssertFlatDefenseModifierIncreasesEffectiveDefense();
                    AssertMultipleStatModifiersStackAdditively();
                    AssertBaseCombatStatsRemainUnchanged();
                    AssertEffectiveStatsUseCanonicalCombatPath();
                    AssertNegativeModifierCannotInvalidateMaxHealth();
                    AssertCurrentHealthClampsToEffectiveMaxHealth();
                    AssertNegativeAttackAndDefenseModifiersClampToZero();
                    AssertStatModifierInvariantClampingDoesNotMutateBaseStats();
                });

            RunSection(
                "Equipment foundation assertions",
                delegate
                {
                    AssertEquipmentItemCanBeEquippedInSlot();
                    AssertEquipmentReplacesItemInSameSlot();
                    AssertEquipmentUnequipClearsSlot();
                    AssertEquippedItemModifiersAffectEffectiveStats();
                    AssertEquipmentEffectiveStatsDoNotMutateBaseStats();
                    AssertInvalidEquipmentIsHandledSafely();
                });

            RunSection(
                "Skill foundation assertions",
                delegate
                {
                    AssertUnlockedSkillIsAvailable();
                    AssertLockedSkillIsBlocked();
                    AssertSkillDefinitionExposesFacts();
                    AssertSkillUsesCanonicalTurnEnginePath();
                });

            RunSection(
                "Resonance foundation assertions",
                delegate
                {
                    AssertSpendResonance();
                    AssertRefillResonance();
                    AssertRefillConsumesTurn();
                    AssertCannotRefillWithEmptyReserve();
                });

            RunSection(
                "Resonance skill integration assertions",
                delegate
                {
                    AssertSkillSpendsActiveResonance();
                    AssertSkillBlockedWhenActiveResonanceIsInsufficient();
                    AssertReserveIsNotConsumedAutomaticallyForSkillCost();
                    AssertRefillThenSkillWorks();
                });

            RunSection(
                "Status effects foundation assertions",
                delegate
                {
                    AssertStatusEffectCanBeApplied();
                    AssertDuplicateNonStackingStatusEffectDoesNotDuplicate();
                    AssertStatusEffectDurationTicksDown();
                    AssertExpiredStatusEffectIsRemoved();
                    AssertSkillSealCanBeDetectedAsActive();
                });

            RunSection(
                "SkillSeal integration assertions",
                delegate
                {
                    AssertSkillSealBlocksSkillAction();
                    AssertSkillSealDoesNotBlockBasicAttack();
                    AssertBlockedSealedSkillDoesNotSpendEssence();
                    AssertBlockedSealedSkillDoesNotSpendActiveResonance();
                    AssertExpiredSkillSealNoLongerBlocksSkillAction();
                });

            RunSection(
                "Regen status integration assertions",
                delegate
                {
                    AssertRegenHealsAtTurnBoundary();
                    AssertRegenDoesNotExceedMaxHp();
                    AssertRegenBlockPreventsRegenHealing();
                    AssertExpiredRegenBlockAllowsRegenAgain();
                    AssertRegenAndRegenBlockDurationsTickAndExpire();
                });

            RunSection(
                "Damage-over-time status integration assertions",
                delegate
                {
                    AssertPoisonDealsPotencyDamage();
                    AssertBurnDealsPotencyDamage();
                    AssertDotCannotReduceHpBelowZero();
                    AssertDotCanDefeatTarget();
                    AssertDotDurationTicksAndExpires();
                    AssertDotResolvesBeforeRegen();
                });
        }
        private static void AssertBasicAttackDamagesTarget()
        {
            var attacker = BuildCombatEntity("attacker", 30, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            var runtime = BuildCombatRuntime(attacker, defender);

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    runtime,
                    TurnRequest.BasicAttack(attacker, defender));

            Assert(
                ContainsEvent(runtime.EventLog, GameEventType.BasicAttackUsed),
                "Basic attack should be accepted by the canonical turn engine.");

            Assert(
                ContainsEvent(runtime.EventLog, GameEventType.DamageDealt)
                && defender.Stats.CurrentHealth < defender.Stats.MaxHealth,
                "Basic attack should damage the target.");
        }

        private static void AssertDefeatedTargetIsDetected()
        {
            var attacker = BuildCombatEntity("attacker", 30, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            defender.Stats.CurrentHealth = 5;
            var runtime = BuildCombatRuntime(attacker, defender);

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    runtime,
                    TurnRequest.BasicAttack(attacker, defender));

            Assert(
                ContainsEvent(runtime.EventLog, GameEventType.CombatEnded),
                "Canonical turn engine should emit combat ended when the target is defeated.");

            Assert(
                !defender.IsAlive && runtime.CombatEnded,
                "Combat entity and runtime should report defeated state after lethal damage.");
        }

        private static void AssertMissCausesNoDamage()
        {
            var attacker = BuildCombatEntity("attacker", 30, 5, 0, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 100, 0, 0, 1f, 10);
            int startingHealth = defender.Stats.CurrentHealth;
            var runtime = BuildCombatRuntime(attacker, defender);

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    runtime,
                    TurnRequest.BasicAttack(attacker, defender));

            Assert(
                ContainsEvent(runtime.EventLog, GameEventType.DamageMissed)
                && !ContainsEvent(runtime.EventLog, GameEventType.DamageDealt),
                "Missed attacks should emit missed damage without dealt damage.");

            Assert(
                defender.Stats.CurrentHealth == startingHealth,
                "Missed attacks should not change target health.");
        }

        private static void AssertCriticalHitIncreasesDamage()
        {
            var normalAttacker = BuildCombatEntity("attacker", 30, 5, 100, 0, 100, 0, 2f, 10);
            var normalDefender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            var criticalAttacker = BuildCombatEntity("attacker", 30, 5, 100, 0, 100, 0, 2f, 10);
            var criticalDefender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            var normalRuntime = BuildCombatRuntime(normalAttacker, normalDefender);
            var criticalRuntime = BuildCombatRuntime(criticalAttacker, criticalDefender);

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    normalRuntime,
                    TurnRequest.BasicAttack(normalAttacker, normalDefender));
            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.0),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    criticalRuntime,
                    TurnRequest.BasicAttack(criticalAttacker, criticalDefender));

            Assert(
                !ContainsEvent(normalRuntime.EventLog, GameEventType.CriticalHit)
                && ContainsEvent(criticalRuntime.EventLog, GameEventType.CriticalHit),
                "Critical hit check should distinguish normal and critical hits through the canonical turn engine.");

            Assert(
                criticalDefender.Stats.CurrentHealth < normalDefender.Stats.CurrentHealth,
                "Critical hit should increase damage.");
        }

        private static void AssertSkillCannotRunWhenCostCannotBePaid()
        {
            var attacker = BuildCombatEntity("attacker", 30, 5, 100, 0, 0, 0, 2f, 0);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            int startingHealth = defender.Stats.CurrentHealth;
            var runtime = BuildCombatRuntime(attacker, defender);
            var skill = new TestDamageSkill();
            var progress = new SkillProgress(BuildHeavyStrikeDefinition());

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.0),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    runtime,
                    TurnRequest.SkillAction(attacker, defender, skill, progress));

            Assert(
                ContainsEvent(runtime.EventLog, GameEventType.ResourceNotEnough)
                && !ContainsEvent(runtime.EventLog, GameEventType.ResourceSpent),
                "Skill action should be blocked when resource cost cannot be paid.");

            Assert(
                defender.Stats.CurrentHealth == startingHealth
                && !ContainsEvent(runtime.EventLog, GameEventType.DamageDealt),
                "Blocked skill action should not damage the target.");
        }

        private static void AssertFlatAttackModifierIncreasesEffectiveAttack()
        {
            var baseStats = BuildCombatStats(10, 5, 100, 0, 0, 0, 2f);
            var effectiveStats = new CombatStatModifierResolver().Resolve(
                baseStats,
                new[]
                {
                    new CombatStatModifier(CombatStatType.Attack, 7)
                });

            Assert(
                effectiveStats.Attack == 17,
                "Flat attack modifier should increase effective attack.");
        }

        private static void AssertFlatDefenseModifierIncreasesEffectiveDefense()
        {
            var baseStats = BuildCombatStats(10, 5, 100, 0, 0, 0, 2f);
            var effectiveStats = new CombatStatModifierResolver().Resolve(
                baseStats,
                new[]
                {
                    new CombatStatModifier(CombatStatType.Defense, 4)
                });

            Assert(
                effectiveStats.Defense == 9,
                "Flat defense modifier should increase effective defense.");
        }

        private static void AssertMultipleStatModifiersStackAdditively()
        {
            var baseStats = BuildCombatStats(10, 5, 100, 0, 0, 0, 2f);
            var effectiveStats = new CombatStatModifierResolver().Resolve(
                baseStats,
                new[]
                {
                    new CombatStatModifier(CombatStatType.Attack, 3),
                    new CombatStatModifier(CombatStatType.Attack, 4),
                    new CombatStatModifier(CombatStatType.Defense, 2)
                });

            Assert(
                effectiveStats.Attack == 17,
                "Multiple flat modifiers for the same stat should stack additively.");

            Assert(
                effectiveStats.Defense == 7,
                "Flat modifiers for different stats should apply independently.");
        }

        private static void AssertBaseCombatStatsRemainUnchanged()
        {
            var baseStats = BuildCombatStats(10, 5, 100, 0, 0, 0, 2f);

            new CombatStatModifierResolver().Resolve(
                baseStats,
                new[]
                {
                    new CombatStatModifier(CombatStatType.Attack, 8),
                    new CombatStatModifier(CombatStatType.Defense, 6)
                });

            Assert(
                baseStats.Attack == 10,
                "Resolving modifiers should not mutate base attack.");

            Assert(
                baseStats.Defense == 5,
                "Resolving modifiers should not mutate base defense.");
        }

        private static void AssertEffectiveStatsUseCanonicalCombatPath()
        {
            var baseAttackerStats = BuildCombatStats(10, 5, 100, 0, 0, 0, 2f);
            var effectiveAttackerStats = new CombatStatModifierResolver().Resolve(
                baseAttackerStats,
                new[]
                {
                    new CombatStatModifier(CombatStatType.Attack, 20)
                });
            var attacker = new CombatEntity("attacker", "attacker", effectiveAttackerStats, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            var runtime = BuildCombatRuntime(attacker, defender);

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    runtime,
                    TurnRequest.BasicAttack(attacker, defender));

            Assert(
                ContainsEvent(runtime.EventLog, GameEventType.BasicAttackUsed)
                && ContainsEvent(runtime.EventLog, GameEventType.DamageDealt),
                "Effective stats should be usable by the canonical TurnBasedCombatEngine path.");

            Assert(
                baseAttackerStats.Attack == 10
                && attacker.Stats.Attack == 30
                && defender.Stats.CurrentHealth < defender.Stats.MaxHealth,
                "Effective combat stats should feed canonical damage without mutating base stats or creating a parallel engine.");
        }

        private static void AssertNegativeModifierCannotInvalidateMaxHealth()
        {
            var baseStats = BuildCombatStats(10, 5, 100, 0, 0, 0, 2f);
            var effectiveStats = new CombatStatModifierResolver().Resolve(
                baseStats,
                new[]
                {
                    new CombatStatModifier(CombatStatType.MaxHealth, -500)
                });

            Assert(
                effectiveStats.MaxHealth == 1,
                "Negative MaxHealth modifier should clamp effective MaxHealth to at least one.");
        }

        private static void AssertCurrentHealthClampsToEffectiveMaxHealth()
        {
            var baseStats = BuildCombatStats(10, 5, 100, 0, 0, 0, 2f);
            var effectiveStats = new CombatStatModifierResolver().Resolve(
                baseStats,
                new[]
                {
                    new CombatStatModifier(CombatStatType.MaxHealth, -40)
                });

            Assert(
                effectiveStats.MaxHealth == 60,
                "Effective MaxHealth should apply flat modifiers before health clamping.");

            Assert(
                effectiveStats.CurrentHealth == 60,
                "CurrentHealth should clamp to effective MaxHealth.");
        }

        private static void AssertNegativeAttackAndDefenseModifiersClampToZero()
        {
            var baseStats = BuildCombatStats(10, 5, 100, 0, 0, 0, 2f);
            var effectiveStats = new CombatStatModifierResolver().Resolve(
                baseStats,
                new[]
                {
                    new CombatStatModifier(CombatStatType.Attack, -50),
                    new CombatStatModifier(CombatStatType.Defense, -50),
                    new CombatStatModifier(CombatStatType.Precision, -150),
                    new CombatStatModifier(CombatStatType.CriticalChance, -50)
                });

            Assert(
                effectiveStats.Attack == 0 && effectiveStats.Defense == 0,
                "Negative attack and defense modifiers should clamp to zero.");

            Assert(
                effectiveStats.Precision == 0 && effectiveStats.CriticalChance == 0,
                "Negative combat chance stats should clamp to zero.");
        }

        private static void AssertStatModifierInvariantClampingDoesNotMutateBaseStats()
        {
            var baseStats = BuildCombatStats(10, 5, 100, 0, 0, 0, 2f);

            new CombatStatModifierResolver().Resolve(
                baseStats,
                new[]
                {
                    new CombatStatModifier(CombatStatType.MaxHealth, -500),
                    new CombatStatModifier(CombatStatType.CurrentHealth, -500),
                    new CombatStatModifier(CombatStatType.Attack, -500),
                    new CombatStatModifier(CombatStatType.Defense, -500)
                });

            Assert(
                baseStats.MaxHealth == 100
                && baseStats.CurrentHealth == 100,
                "Invariant clamping should not mutate base health stats.");

            Assert(
                baseStats.Attack == 10
                && baseStats.Defense == 5,
                "Invariant clamping should not mutate base attack or defense stats.");
        }

        private static void AssertEquipmentItemCanBeEquippedInSlot()
        {
            var profile = new EquipmentProfile();
            var item = BuildEquipmentItem(
                "weapon.training_blade",
                "Training Blade",
                EquipmentSlot.Weapon,
                CombatStatType.Attack,
                3);

            bool equipped = profile.Equip(item);

            Assert(
                equipped,
                "Valid equipment item should be accepted.");

            Assert(
                profile.GetEquipped(EquipmentSlot.Weapon) == item,
                "Equipment profile should track the item by slot.");
        }

        private static void AssertEquipmentReplacesItemInSameSlot()
        {
            var profile = new EquipmentProfile();
            var firstItem = BuildEquipmentItem(
                "weapon.training_blade",
                "Training Blade",
                EquipmentSlot.Weapon,
                CombatStatType.Attack,
                3);
            var secondItem = BuildEquipmentItem(
                "weapon.iron_blade",
                "Iron Blade",
                EquipmentSlot.Weapon,
                CombatStatType.Attack,
                5);

            profile.Equip(firstItem);
            bool replaced = profile.Equip(secondItem);

            Assert(
                replaced,
                "Equipping a valid item into an occupied slot should be accepted.");

            Assert(
                profile.Equipped.Count == 1
                && profile.GetEquipped(EquipmentSlot.Weapon) == secondItem,
                "Equipping another item in the same slot should replace the previous item.");
        }

        private static void AssertEquipmentUnequipClearsSlot()
        {
            var profile = new EquipmentProfile();
            var item = BuildEquipmentItem(
                "armor.training_vest",
                "Training Vest",
                EquipmentSlot.Armor,
                CombatStatType.Defense,
                4);

            profile.Equip(item);
            bool unequipped = profile.Unequip(EquipmentSlot.Armor);

            Assert(
                unequipped,
                "Unequip should report success when a slot was occupied.");

            Assert(
                profile.GetEquipped(EquipmentSlot.Armor) == null
                && profile.Equipped.Count == 0,
                "Unequip should clear the equipped slot.");
        }

        private static void AssertEquippedItemModifiersAffectEffectiveStats()
        {
            var baseStats = BuildCombatStats(10, 5, 100, 0, 0, 0, 2f);
            var profile = new EquipmentProfile();
            profile.Equip(BuildEquipmentItem(
                "weapon.training_blade",
                "Training Blade",
                EquipmentSlot.Weapon,
                CombatStatType.Attack,
                3));
            profile.Equip(BuildEquipmentItem(
                "armor.training_vest",
                "Training Vest",
                EquipmentSlot.Armor,
                CombatStatType.Defense,
                4));

            var effectiveStats = new EquipmentStatModifierService().ResolveEffectiveStats(
                baseStats,
                profile);

            Assert(
                effectiveStats.Attack == 13,
                "Equipped weapon modifier should increase effective attack.");

            Assert(
                effectiveStats.Defense == 9,
                "Equipped armor modifier should increase effective defense.");
        }

        private static void AssertEquipmentEffectiveStatsDoNotMutateBaseStats()
        {
            var baseStats = BuildCombatStats(10, 5, 100, 0, 0, 0, 2f);
            var profile = new EquipmentProfile();
            profile.Equip(BuildEquipmentItem(
                "weapon.training_blade",
                "Training Blade",
                EquipmentSlot.Weapon,
                CombatStatType.Attack,
                3));

            new EquipmentStatModifierService().ResolveEffectiveStats(
                baseStats,
                profile);

            Assert(
                baseStats.Attack == 10,
                "Equipment effective stats should not mutate base attack.");

            Assert(
                baseStats.Defense == 5,
                "Equipment effective stats should not mutate base defense.");
        }

        private static void AssertInvalidEquipmentIsHandledSafely()
        {
            var baseStats = BuildCombatStats(10, 5, 100, 0, 0, 0, 2f);
            var profile = new EquipmentProfile();

            bool equipped = profile.Equip(null);
            bool unequipped = profile.Unequip(EquipmentSlot.Accessory1);
            var effectiveStats = new EquipmentStatModifierService().ResolveEffectiveStats(
                baseStats,
                null);

            Assert(
                !equipped && !unequipped,
                "Null equipment and empty-slot unequip should fail safely.");

            Assert(
                effectiveStats.Attack == baseStats.Attack
                && effectiveStats.Defense == baseStats.Defense,
                "Null equipment profile should resolve to unchanged effective stats.");
        }

        private static EquipmentDefinition BuildEquipmentItem(
            string id,
            string name,
            EquipmentSlot slot,
            CombatStatType statType,
            int amount)
        {
            return new EquipmentDefinition(
                id,
                name,
                slot,
                new List<CombatStatModifier>
                {
                    new CombatStatModifier(
                        statType,
                        amount,
                        StatModifierSourceType.Equipment,
                        id)
                });
        }

        private static void AssertUnlockedSkillIsAvailable()
        {
            var unlockProfile = new UnlockProfile();
            unlockProfile.RecordUnlock(new UnlockRecord(
                CodexUnlockIds.ForbiddenPracticeCount,
                CodexUnlockType.EntryTypeCount,
                CodexEntryType.Practice,
                "",
                1,
                0,
                new List<string>()));

            var result = new SkillAccessPolicy().Evaluate(
                BuildHeavyStrikeDefinition(),
                unlockProfile);

            Assert(
                result.IsAvailable,
                "Unlocked skill should be available.");

            Assert(
                result.RequiredUnlockId == CodexUnlockIds.ForbiddenPracticeCount,
                "Skill access result should preserve the required unlock id.");
        }

        private static void AssertLockedSkillIsBlocked()
        {
            var result = new SkillAccessPolicy().Evaluate(
                BuildHeavyStrikeDefinition(),
                new UnlockProfile());

            Assert(
                !result.IsAvailable,
                "Locked skill should be blocked.");

            Assert(
                result.BlockReason == SkillAccessPolicy.MissingUnlockBlockReason,
                "Locked skill should report missing unlock as the block reason.");
        }

        private static void AssertSkillDefinitionExposesFacts()
        {
            var skill = BuildHeavyStrikeDefinition();

            Assert(
                skill.Category == SkillCategory.Combat
                && skill.Targeting == SkillTargeting.SingleEnemy
                && skill.EffectType == SkillEffectType.Damage,
                "Skill definition should expose category, targeting, and effect facts.");

            Assert(
                skill.Cost.GetCost(1) == 5
                && skill.RequiredUnlockId == CodexUnlockIds.ForbiddenPracticeCount,
                "Skill definition should expose cost and access requirement facts.");
        }

        private static void AssertSkillUsesCanonicalTurnEnginePath()
        {
            var attacker = BuildCombatEntity("attacker", 30, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            var runtime = BuildCombatRuntime(attacker, defender);
            var progress = new SkillProgress(BuildHeavyStrikeDefinition());

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    runtime,
                    TurnRequest.SkillAction(attacker, defender, new TestDamageSkill(), progress));

            Assert(
                ContainsEvent(runtime.EventLog, GameEventType.ResourceSpent)
                && ContainsEvent(runtime.EventLog, GameEventType.SkillUsed),
                "Skill execution should go through TurnBasedCombatEngine resource and skill events.");

            Assert(
                ContainsEvent(runtime.EventLog, GameEventType.DamageDealt)
                && defender.Stats.CurrentHealth < defender.Stats.MaxHealth,
                "Skill execution should use the canonical turn engine damage path.");
        }

        private static void AssertSpendResonance()
        {
            var resonance = new ResonanceProfile(
                new ActiveResonance(6),
                new ResonanceReserve(10));

            bool spent = resonance.TrySpendActive(2);

            Assert(
                spent,
                "Active resonance should be spendable.");

            Assert(
                resonance.Active.Current == 4,
                "Spending active resonance should reduce active resonance.");
        }

        private static void AssertRefillResonance()
        {
            var resonance = new ResonanceProfile(
                new ActiveResonance(6),
                new ResonanceReserve(10));
            resonance.TrySpendActive(4);

            bool refilled = resonance.TryRefillFromReserve(3);

            Assert(
                refilled,
                "Reserve resonance should refill active resonance.");

            Assert(
                resonance.Active.Current == 5
                && resonance.Reserve.Current == 7,
                "Refill should move resonance from reserve to active.");
        }

        private static void AssertRefillConsumesTurn()
        {
            var resonance = new ResonanceProfile(
                new ActiveResonance(6),
                new ResonanceReserve(10));
            resonance.TrySpendActive(3);
            var runtime = BuildCombatRuntime(
                BuildCombatEntity("player", 10, 5, 100, 0, 0, 0, 2f, 10),
                BuildCombatEntity("enemy", 5, 5, 0, 0, 0, 0, 1f, 10));

            bool executed = new ResonanceRefillAction().Execute(
                resonance,
                runtime,
                2);

            Assert(
                executed,
                "Resonance refill action should execute when reserve is available.");

            Assert(
                runtime.TurnNumber == 2
                && ContainsEvent(runtime.EventLog, GameEventType.TurnEnded),
                "Resonance refill action should consume a combat turn.");
        }

        private static void AssertCannotRefillWithEmptyReserve()
        {
            var resonance = new ResonanceProfile(
                new ActiveResonance(6),
                new ResonanceReserve(0));
            resonance.TrySpendActive(3);
            var runtime = BuildCombatRuntime(
                BuildCombatEntity("player", 10, 5, 100, 0, 0, 0, 2f, 10),
                BuildCombatEntity("enemy", 5, 5, 0, 0, 0, 0, 1f, 10));

            bool executed = new ResonanceRefillAction().Execute(
                resonance,
                runtime,
                2);

            Assert(
                !executed,
                "Resonance refill action should fail when reserve is empty.");

            Assert(
                resonance.Active.Current == 3
                && runtime.TurnNumber == 1
                && ContainsEvent(runtime.EventLog, GameEventType.ActionBlocked),
                "Failed resonance refill should not refill active resonance or consume the turn.");
        }

        private static void AssertSkillSpendsActiveResonance()
        {
            var resonance = new ResonanceProfile(
                new ActiveResonance(6),
                new ResonanceReserve(10));
            var attacker = BuildCombatEntity("attacker", 30, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            var runtime = BuildCombatRuntime(attacker, defender, resonance);

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    runtime,
                    TurnRequest.SkillAction(
                        attacker,
                        defender,
                        new TestDamageSkill(),
                        new SkillProgress(BuildResonanceStrikeDefinition())));

            Assert(
                resonance.Active.Current == 4,
                "Accepted resonance skill should spend active resonance.");

            Assert(
                ContainsEvent(runtime.EventLog, GameEventType.SkillUsed)
                && ContainsEvent(runtime.EventLog, GameEventType.DamageDealt),
                "Accepted resonance skill should continue through the canonical combat engine.");
        }

        private static void AssertSkillBlockedWhenActiveResonanceIsInsufficient()
        {
            var resonance = new ResonanceProfile(
                new ActiveResonance(1),
                new ResonanceReserve(10));
            var attacker = BuildCombatEntity("attacker", 30, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            int startingHealth = defender.Stats.CurrentHealth;
            var runtime = BuildCombatRuntime(attacker, defender, resonance);

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    runtime,
                    TurnRequest.SkillAction(
                        attacker,
                        defender,
                        new TestDamageSkill(),
                        new SkillProgress(BuildResonanceStrikeDefinition())));

            Assert(
                ContainsEvent(runtime.EventLog, GameEventType.ResourceNotEnough)
                && !ContainsEvent(runtime.EventLog, GameEventType.SkillUsed),
                "Insufficient active resonance should block skill execution.");

            Assert(
                defender.Stats.CurrentHealth == startingHealth
                && !ContainsEvent(runtime.EventLog, GameEventType.DamageDealt),
                "Resonance-blocked skill should not damage the target.");
        }

        private static void AssertReserveIsNotConsumedAutomaticallyForSkillCost()
        {
            var resonance = new ResonanceProfile(
                new ActiveResonance(1),
                new ResonanceReserve(10));
            var attacker = BuildCombatEntity("attacker", 30, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            var runtime = BuildCombatRuntime(attacker, defender, resonance);

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    runtime,
                    TurnRequest.SkillAction(
                        attacker,
                        defender,
                        new TestDamageSkill(),
                        new SkillProgress(BuildResonanceStrikeDefinition())));

            Assert(
                resonance.Active.Current == 1,
                "Insufficient active resonance should remain unchanged when skill is blocked.");

            Assert(
                resonance.Reserve.Current == 10,
                "Reserve resonance should not be consumed automatically for skill cost.");
        }

        private static void AssertRefillThenSkillWorks()
        {
            var resonance = new ResonanceProfile(
                new ActiveResonance(6),
                new ResonanceReserve(10));
            resonance.TrySpendActive(6);
            var attacker = BuildCombatEntity("attacker", 30, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            var runtime = BuildCombatRuntime(attacker, defender, resonance);

            bool refilled = new ResonanceRefillAction().Execute(
                resonance,
                runtime,
                2);
            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    runtime,
                    TurnRequest.SkillAction(
                        attacker,
                        defender,
                        new TestDamageSkill(),
                        new SkillProgress(BuildResonanceStrikeDefinition())));

            Assert(
                refilled
                && resonance.Active.Current == 0
                && resonance.Reserve.Current == 8,
                "Refill should enable a later skill without auto-consuming reserve during the skill.");

            Assert(
                runtime.TurnNumber == 3
                && ContainsEvent(runtime.EventLog, GameEventType.SkillUsed)
                && ContainsEvent(runtime.EventLog, GameEventType.DamageDealt),
                "Refill then skill should consume separate turns and execute through the canonical engine.");
        }

        private static void AssertStatusEffectCanBeApplied()
        {
            var target = BuildCombatEntity("target", 10, 5, 100, 0, 0, 0, 2f, 10);

            bool applied = new StatusEffectResolver().Apply(
                target,
                new StatusEffectInstance(StatusEffectType.Poison, 2, false));

            Assert(
                applied,
                "Status effect should be applicable.");

            Assert(
                target.StatusEffects.HasActive(StatusEffectType.Poison),
                "Applied status effect should be detectable as active.");
        }

        private static void AssertDuplicateNonStackingStatusEffectDoesNotDuplicate()
        {
            var target = BuildCombatEntity("target", 10, 5, 100, 0, 0, 0, 2f, 10);
            var resolver = new StatusEffectResolver();

            bool firstApply = resolver.Apply(
                target,
                new StatusEffectInstance(StatusEffectType.Burn, 2, false));
            bool duplicateApply = resolver.Apply(
                target,
                new StatusEffectInstance(StatusEffectType.Burn, 2, false));

            Assert(
                firstApply && !duplicateApply,
                "Duplicate non-stacking status effect should be rejected.");

            Assert(
                target.StatusEffects.Active.Count == 1,
                "Duplicate non-stacking status effect should not add another instance.");
        }

        private static void AssertStatusEffectDurationTicksDown()
        {
            var attacker = BuildCombatEntity("attacker", 10, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.RegenBlock, 2, false));

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    BuildCombatRuntime(attacker, defender),
                    TurnRequest.BasicAttack(attacker, defender));

            var statusEffect = attacker.StatusEffects.GetFirst(StatusEffectType.RegenBlock);

            Assert(
                statusEffect != null,
                "Status effect should remain active before duration reaches zero.");

            Assert(
                statusEffect.RemainingDuration == 1,
                "Status effect duration should tick down at the canonical turn boundary.");
        }

        private static void AssertExpiredStatusEffectIsRemoved()
        {
            var attacker = BuildCombatEntity("attacker", 10, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.Poison, 1, false));

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    BuildCombatRuntime(attacker, defender),
                    TurnRequest.BasicAttack(attacker, defender));

            Assert(
                !attacker.StatusEffects.HasActive(StatusEffectType.Poison),
                "Expired status effect should be removed.");

            Assert(
                attacker.StatusEffects.Active.Count == 0,
                "Expired status effect should leave no active instance.");
        }

        private static void AssertSkillSealCanBeDetectedAsActive()
        {
            var target = BuildCombatEntity("target", 10, 5, 100, 0, 0, 0, 2f, 10);

            new StatusEffectResolver().Apply(
                target,
                new StatusEffectInstance(StatusEffectType.SkillSeal, 2, false));

            Assert(
                target.StatusEffects.HasActive(StatusEffectType.SkillSeal),
                "SkillSeal status effect should be detectable as active.");

            Assert(
                target.StatusEffects.GetFirst(StatusEffectType.SkillSeal).RemainingDuration == 2,
                "SkillSeal status effect should preserve duration facts.");
        }

        private static void AssertSkillSealBlocksSkillAction()
        {
            var attacker = BuildCombatEntity("attacker", 30, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            var runtime = BuildCombatRuntime(attacker, defender);
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.SkillSeal, 2, false));

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    runtime,
                    TurnRequest.SkillAction(
                        attacker,
                        defender,
                        new TestDamageSkill(),
                        new SkillProgress(BuildHeavyStrikeDefinition())));

            Assert(
                ContainsEvent(runtime.EventLog, GameEventType.ActionBlocked),
                "SkillSeal should emit an action-blocked event for skill actions.");

            Assert(
                !ContainsEvent(runtime.EventLog, GameEventType.SkillUsed)
                && !ContainsEvent(runtime.EventLog, GameEventType.DamageDealt),
                "SkillSeal should block skill effects and damage.");
        }

        private static void AssertSkillSealDoesNotBlockBasicAttack()
        {
            var attacker = BuildCombatEntity("attacker", 30, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            var runtime = BuildCombatRuntime(attacker, defender);
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.SkillSeal, 2, false));

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    runtime,
                    TurnRequest.BasicAttack(attacker, defender));

            Assert(
                ContainsEvent(runtime.EventLog, GameEventType.BasicAttackUsed),
                "SkillSeal should not block basic attacks.");

            Assert(
                ContainsEvent(runtime.EventLog, GameEventType.DamageDealt)
                && defender.Stats.CurrentHealth < defender.Stats.MaxHealth,
                "Basic attack should still resolve damage while SkillSeal is active.");
        }

        private static void AssertBlockedSealedSkillDoesNotSpendEssence()
        {
            var attacker = BuildCombatEntity("attacker", 30, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            var runtime = BuildCombatRuntime(attacker, defender);
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.SkillSeal, 2, false));

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    runtime,
                    TurnRequest.SkillAction(
                        attacker,
                        defender,
                        new TestDamageSkill(),
                        new SkillProgress(BuildHeavyStrikeDefinition())));

            Assert(
                attacker.Resource.Current == 10,
                "SkillSeal should block before Essence is spent.");

            Assert(
                !ContainsEvent(runtime.EventLog, GameEventType.ResourceSpent),
                "SkillSeal should not emit resource-spent events for blocked skills.");
        }

        private static void AssertBlockedSealedSkillDoesNotSpendActiveResonance()
        {
            var resonance = new ResonanceProfile(
                new ActiveResonance(6),
                new ResonanceReserve(10));
            var attacker = BuildCombatEntity("attacker", 30, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            var runtime = BuildCombatRuntime(attacker, defender, resonance);
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.SkillSeal, 2, false));

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    runtime,
                    TurnRequest.SkillAction(
                        attacker,
                        defender,
                        new TestDamageSkill(),
                        new SkillProgress(BuildResonanceStrikeDefinition())));

            Assert(
                resonance.Active.Current == 6,
                "SkillSeal should block before active resonance is spent.");

            Assert(
                resonance.Reserve.Current == 10,
                "SkillSeal should not consume reserve resonance.");
        }

        private static void AssertExpiredSkillSealNoLongerBlocksSkillAction()
        {
            var attacker = BuildCombatEntity("attacker", 30, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            var runtime = BuildCombatRuntime(attacker, defender);
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.SkillSeal, 1, false));
            var engine = new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99, 0.0, 0.99),
                BuildEmptyPassivePipeline());

            engine.ExecuteTurn(
                runtime,
                TurnRequest.BasicAttack(attacker, defender));
            engine.ExecuteTurn(
                runtime,
                TurnRequest.SkillAction(
                    attacker,
                    defender,
                    new TestDamageSkill(),
                    new SkillProgress(BuildHeavyStrikeDefinition())));

            Assert(
                !attacker.StatusEffects.HasActive(StatusEffectType.SkillSeal),
                "Expired SkillSeal should no longer be active.");

            Assert(
                ContainsEvent(runtime.EventLog, GameEventType.SkillUsed)
                && ContainsEvent(runtime.EventLog, GameEventType.ResourceSpent),
                "Expired SkillSeal should no longer block skill actions.");
        }

        private static void AssertRegenHealsAtTurnBoundary()
        {
            var attacker = BuildCombatEntity("attacker", 10, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            attacker.Stats.CurrentHealth = 50;
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.Regen, 2, false, 10));

            ExecuteBasicAttackTurn(attacker, defender);

            Assert(
                attacker.Stats.CurrentHealth == 60,
                "Regen should heal at the canonical turn boundary.");

            Assert(
                attacker.StatusEffects.HasActive(StatusEffectType.Regen),
                "Regen should remain active before duration reaches zero.");
        }

        private static void AssertRegenDoesNotExceedMaxHp()
        {
            var attacker = BuildCombatEntity("attacker", 10, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            attacker.Stats.CurrentHealth = 95;
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.Regen, 2, false, 20));

            ExecuteBasicAttackTurn(attacker, defender);

            Assert(
                attacker.Stats.CurrentHealth == attacker.Stats.MaxHealth,
                "Regen should not heal above max HP.");

            Assert(
                attacker.Stats.CurrentHealth == 100,
                "Regen should cap healing at the combatant's max HP value.");
        }

        private static void AssertRegenBlockPreventsRegenHealing()
        {
            var attacker = BuildCombatEntity("attacker", 10, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            attacker.Stats.CurrentHealth = 50;
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.Regen, 3, false, 10));
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.RegenBlock, 2, false));

            ExecuteBasicAttackTurn(attacker, defender);

            Assert(
                attacker.Stats.CurrentHealth == 50,
                "RegenBlock should prevent Regen healing while active.");

            Assert(
                attacker.StatusEffects.HasActive(StatusEffectType.Regen)
                && attacker.StatusEffects.HasActive(StatusEffectType.RegenBlock),
                "Regen and RegenBlock should both tick normally while active.");
        }

        private static void AssertExpiredRegenBlockAllowsRegenAgain()
        {
            var attacker = BuildCombatEntity("attacker", 10, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            attacker.Stats.CurrentHealth = 50;
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.Regen, 3, false, 10));
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.RegenBlock, 1, false));

            ExecuteBasicAttackTurn(attacker, defender);
            ExecuteBasicAttackTurn(attacker, defender);

            Assert(
                !attacker.StatusEffects.HasActive(StatusEffectType.RegenBlock),
                "Expired RegenBlock should be removed.");

            Assert(
                attacker.Stats.CurrentHealth == 60,
                "Regen should heal again once RegenBlock has expired.");
        }

        private static void AssertRegenAndRegenBlockDurationsTickAndExpire()
        {
            var attacker = BuildCombatEntity("attacker", 10, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            attacker.Stats.CurrentHealth = 50;
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.Regen, 1, false, 10));
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.RegenBlock, 1, false));

            ExecuteBasicAttackTurn(attacker, defender);

            Assert(
                !attacker.StatusEffects.HasActive(StatusEffectType.Regen),
                "Regen should expire when duration reaches zero.");

            Assert(
                !attacker.StatusEffects.HasActive(StatusEffectType.RegenBlock),
                "RegenBlock should expire when duration reaches zero.");
        }

        private static void AssertPoisonDealsPotencyDamage()
        {
            var attacker = BuildCombatEntity("attacker", 10, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            attacker.Stats.CurrentHealth = 70;
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.Poison, 2, false, 12));

            ExecuteBasicAttackTurn(attacker, defender);

            Assert(
                attacker.Stats.CurrentHealth == 58,
                "Poison should deal potency damage at the canonical turn boundary.");
        }

        private static void AssertBurnDealsPotencyDamage()
        {
            var attacker = BuildCombatEntity("attacker", 10, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            attacker.Stats.CurrentHealth = 70;
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.Burn, 2, false, 9));

            ExecuteBasicAttackTurn(attacker, defender);

            Assert(
                attacker.Stats.CurrentHealth == 61,
                "Burn should deal potency damage at the canonical turn boundary.");
        }

        private static void AssertDotCannotReduceHpBelowZero()
        {
            var attacker = BuildCombatEntity("attacker", 10, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            attacker.Stats.CurrentHealth = 3;
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.Poison, 2, false, 10));

            ExecuteBasicAttackTurn(attacker, defender);

            Assert(
                attacker.Stats.CurrentHealth == 0,
                "DOT damage should not reduce HP below zero.");
        }

        private static void AssertDotCanDefeatTarget()
        {
            var attacker = BuildCombatEntity("attacker", 10, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            var runtime = BuildCombatRuntime(attacker, defender);
            attacker.Stats.CurrentHealth = 5;
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.Burn, 2, false, 8));

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    runtime,
                    TurnRequest.BasicAttack(attacker, defender));

            Assert(
                !attacker.IsAlive,
                "DOT damage should be able to defeat the affected combatant.");

            Assert(
                runtime.CombatEnded && ContainsEvent(runtime.EventLog, GameEventType.CombatEnded),
                "DOT defeat should end combat through the canonical engine.");
        }

        private static void AssertDotDurationTicksAndExpires()
        {
            var attacker = BuildCombatEntity("attacker", 10, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            attacker.Stats.CurrentHealth = 70;
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.Poison, 1, false, 6));

            ExecuteBasicAttackTurn(attacker, defender);

            Assert(
                !attacker.StatusEffects.HasActive(StatusEffectType.Poison),
                "DOT status effect should expire when duration reaches zero.");

            Assert(
                attacker.Stats.CurrentHealth == 64,
                "DOT should still apply on the boundary where its final duration tick expires.");
        }

        private static void AssertDotResolvesBeforeRegen()
        {
            var attacker = BuildCombatEntity("attacker", 10, 5, 100, 0, 0, 0, 2f, 10);
            var defender = BuildCombatEntity("defender", 5, 5, 0, 0, 0, 0, 1f, 10);
            attacker.Stats.CurrentHealth = 50;
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.Poison, 2, false, 15));
            new StatusEffectResolver().Apply(
                attacker,
                new StatusEffectInstance(StatusEffectType.Regen, 2, false, 10));

            ExecuteBasicAttackTurn(attacker, defender);

            Assert(
                attacker.Stats.CurrentHealth == 45,
                "Explicit boundary order should apply DOT before Regen healing.");

            Assert(
                attacker.StatusEffects.HasActive(StatusEffectType.Poison)
                && attacker.StatusEffects.HasActive(StatusEffectType.Regen),
                "DOT and Regen should both tick normally after ordered boundary effects resolve.");
        }

        private static void ExecuteBasicAttackTurn(
            CombatEntity attacker,
            CombatEntity defender)
        {
            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    BuildCombatRuntime(attacker, defender),
                    TurnRequest.BasicAttack(attacker, defender));
        }

        private static CombatEntity BuildCombatEntity(
            string id,
            int attack,
            int defense,
            int precision,
            int evasion,
            int criticalChance,
            int criticalResistance,
            float criticalDamageMultiplier,
            int maxResource)
        {
            return new CombatEntity(
                id,
                id,
                BuildCombatStats(
                    attack,
                    defense,
                    precision,
                    evasion,
                    criticalChance,
                    criticalResistance,
                    criticalDamageMultiplier),
                maxResource);
        }

        private static CombatStats BuildCombatStats(
            int attack,
            int defense,
            int precision,
            int evasion,
            int criticalChance,
            int criticalResistance,
            float criticalDamageMultiplier)
        {
            return new CombatStats
            {
                MaxHealth = 100,
                CurrentHealth = 100,
                Attack = attack,
                Defense = defense,
                Precision = precision,
                Evasion = evasion,
                CriticalChance = criticalChance,
                CriticalResistance = criticalResistance,
                CriticalDamageMultiplier = criticalDamageMultiplier
            };
        }

        private static CombatRuntimeContext BuildCombatRuntime(
            CombatEntity player,
            CombatEntity enemy)
        {
            return new CombatRuntimeContext(
                player,
                enemy,
                new GameEventLog(),
                new PlayerProfile());
        }

        private static CombatRuntimeContext BuildCombatRuntime(
            CombatEntity player,
            CombatEntity enemy,
            ResonanceProfile resonanceProfile)
        {
            return new CombatRuntimeContext(
                player,
                enemy,
                new GameEventLog(),
                new PlayerProfile(),
                resonanceProfile);
        }

        private static CombatPassivePipeline BuildEmptyPassivePipeline()
        {
            return new CombatPassivePipeline(new List<ICombatPassiveEffect>());
        }

        private static SkillDefinition BuildHeavyStrikeDefinition()
        {
            return new SkillDefinition(
                "skill.heavy_strike",
                "Heavy Strike",
                1,
                new SkillCost(5, 0),
                SkillCategory.Combat,
                SkillTargeting.SingleEnemy,
                SkillEffectType.Damage,
                CodexUnlockIds.ForbiddenPracticeCount);
        }

        private static SkillDefinition BuildResonanceStrikeDefinition()
        {
            return new SkillDefinition(
                "skill.resonance_strike",
                "Resonance Strike",
                1,
                new SkillCost(5, 0),
                SkillCategory.Combat,
                SkillTargeting.SingleEnemy,
                SkillEffectType.Damage,
                CodexUnlockIds.ForbiddenPracticeCount,
                2);
        }

        private static bool ContainsEvent(
            GameEventLog eventLog,
            GameEventType eventType)
        {
            foreach (var gameEvent in eventLog.Events)
            {
                if (gameEvent.Type == eventType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

