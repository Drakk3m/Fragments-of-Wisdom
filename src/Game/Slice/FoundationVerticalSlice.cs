using System;
using System.Collections.Generic;
using Game.Application.Integration;
using Game.Domain.Classes;
using Game.Domain.Cognition;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.AI;
using Game.Domain.Combat.Outcomes;
using Game.Domain.Combat.Passives;
using Game.Domain.Combat.Stats;
using Game.Domain.Equipment;
using Game.Domain.Missions;
using Game.Domain.Player;
using Game.Domain.Skills;
using Game.Infrastructure.Random;


namespace Game.Slice
{
    public static class FoundationVerticalSlice
    {
        public static GameEventLog Run()
        {
            var log = new GameEventLog();
            var profile = CreateProfile();

            CompleteMissions(profile, log);
            RunCombat(profile, log);

            return log;
        }

        public static void RunToConsole()
        {
            foreach (var gameEvent in Run().Events)
            {
                Console.WriteLine("[" + gameEvent.Type + "] " + gameEvent.Message);
            }
        }

        private static PlayerProfile CreateProfile()
        {
            var profile = new PlayerProfile();
            var tlamatini = ClassBootstrap.BuildTlamatini();

            profile.ClassCollection.UnlockClass(tlamatini.Id);
            profile.ClassCollection.SetActiveClass(tlamatini.Id);

            foreach (var skillId in tlamatini.FixedSkillIds)
            {
                profile.SkillBook.UnlockSkill(skillId);
            }

            profile.Codex.DiscoverEntry("class_tlamatini");
            return profile;
        }

        private static void CompleteMissions(
            PlayerProfile profile,
            GameEventLog log)
        {
            var service = new MissionCompletionService(
                MissionBootstrap.BuildDefinitionRegistry(),
                MissionBootstrap.BuildEvaluatorRegistry(),
                new MissionRewardApplicationService(),
                log);

            profile.Cognition.UnlockTeaching(CognitiveArchetypeId.Lyra, "lyra_detect_weakness");
            profile.Cognition.SetActiveArchetype(CognitiveArchetypeId.Lyra);

            var lyra = new MissionAttempt();
            lyra.Record(MissionActionType.Analyze);
            lyra.Record(MissionActionType.ApplyDebuff);
            service.Complete(profile, LyraJudgementCrackEvaluator.Id, lyra);
            profile.Cognition.EquipFixedTeaching("lyra_detect_weakness");

            profile.Cognition.UnlockTeaching(CognitiveArchetypeId.Atlas, "atlas_stable_cast");
            profile.Cognition.SetActiveArchetype(CognitiveArchetypeId.Atlas);

            var atlas = new MissionAttempt();
            atlas.Record(MissionActionType.ChooseStableAction);
            service.Complete(profile, AtlasThreeCertaintiesEvaluator.Id, atlas);
            profile.Cognition.EquipFixedTeaching("atlas_stable_cast");

            var relic = new MissionAttempt();
            relic.Record(MissionActionType.CollectItem);
            service.Complete(profile, FirstRelicSideQuestEvaluator.Id, relic);
        }

        private static void RunCombat(
            PlayerProfile profile,
            GameEventLog log)
        {
            var player = CreatePlayer();
            var enemy = CreateEnemy();

            profile.Codex.DiscoverEntry(enemy.Id);

            var passives = new List<ICombatPassiveEffect>();
            passives.AddRange(new CognitionCombatPassiveResolver().Resolve(profile));

            var engine = new TurnBasedCombatEngine(
                new SeededRandomSource(2026),
                new CombatPassivePipeline(passives));

            var runtime = new CombatRuntimeContext(player, enemy, log, profile);
            engine.StartCombat(runtime);

            var balance = new SkillDefinition("skill_balance_ritual", "Ritual de Balance", 3, new SkillCost(18, 3));
            var destiny = new SkillDefinition("skill_destiny_correction", "CorrecciÃ³n del Destino", 3, new SkillCost(20, 4));

            engine.ExecuteTurn(
                runtime,
                TurnRequest.SkillAction(player, enemy, new BalanceRitualSkill(), new SkillProgress(balance)));

            var ai = new BasicAttackEnemyAiStrategy();
            engine.ExecuteTurn(runtime, ai.ChooseAction(runtime));

            engine.ExecuteTurn(
                runtime,
                TurnRequest.SkillAction(player, enemy, new DestinyCorrectionSkill(), new SkillProgress(destiny)));

            var outcome = new BattleOutcomeResolver().Resolve(runtime);

            new BattleRewardResolver().ApplyRewards(
                profile,
                outcome,
                new BattleRewardDefinition("gold", 25, 10),
                log);

            if (!enemy.IsAlive)
            {
                profile.Codex.CompleteEntry(enemy.Id);
            }
        }

        private static CombatEntity CreatePlayer()
        {
            var stats = new CombatStats
            {
                MaxHealth = 120,
                CurrentHealth = 120,
                Attack = 18,
                Defense = 10,
                Precision = 16,
                Evasion = 10,
                CriticalChance = 8,
                CriticalResistance = 4,
                CriticalDamageMultiplier = 1.5f,
                DebuffChance = 22,
                DebuffResistance = 10
            };

            var loadout = new EquipmentLoadout();

            loadout.Equip(new EquipmentDefinition(
                "weapon_training_blade",
                "Training Blade",
                EquipmentSlot.Weapon,
                new CombatStats
                {
                    Attack = 3,
                    Precision = 2
                }));

            var finalStats = new EquipmentStatAggregator().Aggregate(stats, loadout);
            finalStats.CurrentHealth = finalStats.MaxHealth;

            return new CombatEntity("player_nova_tlamatini", "Nova / Tlamatini", finalStats, 100);
        }

        private static CombatEntity CreateEnemy()
        {
            return new CombatEntity(
                "enemy_goblin_scout",
                "Goblin Scout",
                new CombatStats
                {
                    MaxHealth = 90,
                    CurrentHealth = 90,
                    Attack = 14,
                    Defense = 6,
                    Precision = 12,
                    Evasion = 8,
                    CriticalChance = 5,
                    CriticalResistance = 3,
                    CriticalDamageMultiplier = 1.4f,
                    DebuffChance = 8,
                    DebuffResistance = 16
                },
                40);
        }
    }
}
