using System.Collections.Generic;
using Game.Application.Bootstrap;
using Game.Application.Integration;
using Game.Application.Runtime;
using Game.Domain.Classes;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.Passives;
using Game.Domain.Combat.Resources;
using Game.Domain.Combat.StatusEffects;
using Game.Domain.Codex;
using Game.Domain.Missions;
using Game.Domain.Player;
using Game.Domain.Progress;
using Game.Domain.Skills;
using Game.Slice;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void AssertProgressionRuntimeDemoBootstrapCreatesOneSessionBootstrap()
        {
            var demo = BuildTestProgressionRuntimeDemoBootstrap();
            var sessionBootstrap = demo.SessionBootstrap;
            var composition = demo.Composition;
            var loadedSession = demo.LoadedSession;
            var runtimeState = demo.RuntimeState;

            Assert(
                sessionBootstrap != null
                && object.ReferenceEquals(sessionBootstrap, demo.SessionBootstrap),
                "V5 runtime demo bootstrap should expose one stable player session bootstrap.");

            Assert(
                composition != null
                && object.ReferenceEquals(composition, demo.Composition)
                && object.ReferenceEquals(composition, sessionBootstrap.Composition),
                "V5 runtime demo bootstrap should expose the session bootstrap's composition.");

            Assert(
                loadedSession != null
                && object.ReferenceEquals(loadedSession, demo.LoadedSession)
                && object.ReferenceEquals(runtimeState, demo.RuntimeState)
                && object.ReferenceEquals(runtimeState, sessionBootstrap.RuntimeState),
                "V5 runtime demo bootstrap should expose the session bootstrap's loaded session and runtime state.");
        }

        private static void AssertProgressionRuntimeDemoBootstrapKeepsBindingRegistrySessionScoped()
        {
            var firstBindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var secondBindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var firstDemo = new ProgressionRuntimeDemoBootstrap(
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                firstBindingRegistry);
            var secondDemo = new ProgressionRuntimeDemoBootstrap(
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                secondBindingRegistry);

            Assert(
                object.ReferenceEquals(
                    firstBindingRegistry,
                    firstDemo.RuntimeState.ClassSkillTreeBindingRegistry)
                && object.ReferenceEquals(
                    firstBindingRegistry,
                    firstDemo.Composition.ClassSkillTreeBindingRegistry)
                && !object.ReferenceEquals(
                    firstDemo.RuntimeState.ClassSkillTreeBindingRegistry,
                    secondDemo.RuntimeState.ClassSkillTreeBindingRegistry),
                "V5 runtime demo bootstrap should keep mutable bindings scoped to one demo session.");

            Assert(
                object.ReferenceEquals(
                    firstDemo.Composition.ClassRegistry,
                    firstDemo.RuntimeState.ClassRegistry)
                && object.ReferenceEquals(
                    firstDemo.Composition.SkillRegistry,
                    firstDemo.RuntimeState.SkillRegistry)
                && object.ReferenceEquals(
                    firstDemo.Composition.MentorRegistry,
                    firstDemo.RuntimeState.MentorRegistry),
                "V5 runtime demo bootstrap should reuse canonical registries through the session bootstrap path.");
        }

        private static void AssertProgressionRuntimeDemoBootstrapRestoresProgressesAndCreatesCombat()
        {
            var demo = BuildTestProgressionRuntimeDemoBootstrap();
            var runtimeState = demo.RuntimeState;
            var classProfile = runtimeState.ClassProfile;
            var masteryProfile = runtimeState.MasteryProfile;
            var skillTree = runtimeState.ClassSkillTreeBindingRegistry.GetSkillTree(
                SkillTreeId.Tlamatini);
            var restoreResult = new ProgressRestoreCoordinator().Restore(
                BuildCompleteV4SnapshotWithBinding(
                    ClassId.Tlamatini,
                    SkillTreeId.Tlamatini),
                runtimeState.ClassProfile,
                runtimeState.MasteryProfile,
                runtimeState.MentorRelationshipProfile,
                runtimeState.SkillTreeProgressProfile,
                runtimeState.ClassRegistry,
                runtimeState.SkillRegistry,
                runtimeState.MentorRegistry,
                skillTree,
                runtimeState.ClassSkillTreeBindingRegistry);
            var firstRandom = new FixedRandomSource(0.0, 0.99);
            var secondRandom = new FixedRandomSource(0.0, 0.99);
            var firstEngine = demo.CreateCombatEngine(
                firstRandom,
                new List<ICombatPassiveEffect>());
            var secondEngine = demo.CreateCombatEngine(
                secondRandom,
                new List<ICombatPassiveEffect>());
            var pipelineField = typeof(TurnBasedCombatEngine).GetField(
                "pipeline",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var randomField = typeof(TurnBasedCombatEngine).GetField(
                "random",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var validatorField = typeof(TurnBasedCombatEngine).GetField(
                "skillTurnValidator",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var firstPlayer = BuildCombatEntity(
                "v5_demo_allowed_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var firstEnemy = BuildCombatEntity(
                "v5_demo_allowed_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var firstDefinition = runtimeState.SkillRegistry.Get(
                "skill_balance_ritual");
            var firstSkill = new TrackingCombatSkill(firstDefinition.Id);

            firstEngine.ExecuteTurn(
                BuildCombatRuntime(firstPlayer, firstEnemy),
                TurnRequest.SkillAction(
                    firstPlayer,
                    firstEnemy,
                    firstSkill,
                    new SkillProgress(firstDefinition)));

            runtimeState.MasteryProfile.IncreaseProgress(
                "skill_deep_sight",
                MasteryRecord.MasteredProgress
                    - runtimeState.MasteryProfile.GetRecord(
                        "skill_deep_sight").Progress,
                runtimeState.SkillRegistry);
            runtimeState.ClassProfile.ChangeClassId(
                ClassId.Wanderer,
                runtimeState.ClassRegistry);
            var secondPlayer = BuildCombatEntity(
                "v5_demo_retained_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var secondEnemy = BuildCombatEntity(
                "v5_demo_retained_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var retainedDefinition = runtimeState.SkillRegistry.Get(
                "skill_deep_sight");
            var retainedSkill = new TrackingCombatSkill(retainedDefinition.Id);

            secondEngine.ExecuteTurn(
                BuildCombatRuntime(secondPlayer, secondEnemy),
                TurnRequest.SkillAction(
                    secondPlayer,
                    secondEnemy,
                    retainedSkill,
                    new SkillProgress(retainedDefinition)));

            bool ownsTurnBasedCombatEngine = false;
            foreach (var field in typeof(ProgressionRuntimeDemoBootstrap).GetFields(
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Public))
            {
                if (field.FieldType == typeof(TurnBasedCombatEngine))
                {
                    ownsTurnBasedCombatEngine = true;
                }
            }

            Assert(
                restoreResult.FullyRestored
                && firstSkill.Applied
                && retainedSkill.Applied
                && object.ReferenceEquals(classProfile, runtimeState.ClassProfile)
                && object.ReferenceEquals(masteryProfile, runtimeState.MasteryProfile),
                "V5 runtime demo bootstrap should restore and progress through the same runtime state used by combat.");

            Assert(
                !ownsTurnBasedCombatEngine
                && firstEngine != secondEngine
                && pipelineField != null
                && !object.ReferenceEquals(
                    pipelineField.GetValue(firstEngine),
                    pipelineField.GetValue(secondEngine))
                && randomField != null
                && object.ReferenceEquals(
                    firstRandom,
                    randomField.GetValue(firstEngine))
                && object.ReferenceEquals(
                    secondRandom,
                    randomField.GetValue(secondEngine))
                && validatorField != null
                && validatorField.GetValue(firstEngine)
                    is CombatSkillTurnValidatorGateway,
                "V5 runtime demo bootstrap should create combat through PlayerSessionBootstrap without owning TurnBasedCombatEngine.");
        }

        private static void AssertProgressionRuntimeDemoBootstrapAllowsMasteredRetainedSkill()
        {
            var demo = BuildTestProgressionRuntimeDemoBootstrap();
            var runtimeState = demo.RuntimeState;
            runtimeState.ClassProfile.AssignClassId(
                ClassId.Tlamatini,
                runtimeState.ClassRegistry);
            runtimeState.MasteryProfile.IncreaseProgress(
                "skill_deep_sight",
                MasteryRecord.MasteredProgress,
                runtimeState.SkillRegistry);
            runtimeState.ClassProfile.ChangeClassId(
                ClassId.Wanderer,
                runtimeState.ClassRegistry);
            var player = BuildCombatEntity(
                "v5_demo_mastered_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var enemy = BuildCombatEntity(
                "v5_demo_mastered_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var definition = runtimeState.SkillRegistry.Get("skill_deep_sight");
            var skill = new TrackingCombatSkill(definition.Id);

            demo.CreateCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                new List<ICombatPassiveEffect>()).ExecuteTurn(
                    BuildCombatRuntime(player, enemy),
                    TurnRequest.SkillAction(
                        player,
                        enemy,
                        skill,
                        new SkillProgress(definition)));

            Assert(
                skill.Applied,
                "V5 runtime demo bootstrap should allow mastered retained skills after progression mutation.");
        }

        private static void AssertProgressionRuntimeDemoBootstrapBlocksCrossClassSkillSafely()
        {
            var demo = BuildTestProgressionRuntimeDemoBootstrap();
            var runtimeState = demo.RuntimeState;
            runtimeState.ClassProfile.AssignClassId(
                ClassId.Wanderer,
                runtimeState.ClassRegistry);
            var resonance = new ResonanceProfile(
                new ActiveResonance(6),
                new ResonanceReserve(10));
            var player = BuildCombatEntity(
                "v5_demo_blocked_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var enemy = BuildCombatEntity(
                "v5_demo_blocked_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            int startingHealth = enemy.Stats.CurrentHealth;
            var runtime = BuildCombatRuntime(player, enemy, resonance);
            var definition = new SkillDefinition(
                "skill_balance_ritual",
                "V5 Demo Blocked",
                3,
                new SkillCost(10, 0),
                SkillCategory.Combat,
                SkillTargeting.SingleEnemy,
                SkillEffectType.Damage,
                "",
                2);
            var progress = new SkillProgress(definition);
            var skill = new TrackingCombatSkill(definition.Id);

            demo.CreateCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                new List<ICombatPassiveEffect>()).ExecuteTurn(
                    runtime,
                    TurnRequest.SkillAction(
                        player,
                        enemy,
                        skill,
                        progress));

            Assert(
                !skill.Applied
                && CountEvents(
                    runtime.EventLog,
                    GameEventType.ActionBlocked) == 1
                && runtime.TurnNumber == 2,
                "V5 runtime demo bootstrap should block non-mastered cross-class skills and consume the turn.");

            Assert(
                player.Resource.Current == 20
                && resonance.Active.Current == 6
                && resonance.Reserve.Current == 10
                && CountEvents(
                    runtime.EventLog,
                    GameEventType.ResourceSpent) == 0,
                "V5 runtime demo bootstrap blocked validation should spend neither Essence nor Resonance.");

            Assert(
                progress.Experience == 0
                && enemy.Stats.CurrentHealth == startingHealth
                && CountEvents(
                    runtime.EventLog,
                    GameEventType.SkillUsed) == 0,
                "V5 runtime demo bootstrap blocked validation should grant no experience or skill effects.");
        }

        private static void AssertProgressionRuntimeDemoBootstrapPreservesLegacySlice()
        {
            Assert(
                FoundationVerticalSlice.Run().Events.Count == 38,
                "V5 runtime demo bootstrap should preserve the legacy 38-event slice.");
        }

        private static void AssertPlayerSessionBootstrapCreatesOneComposition()
        {
            var bootstrap = BuildTestPlayerSessionBootstrap();
            var composition = bootstrap.Composition;
            var loadedSession = bootstrap.LoadedSession;
            var runtimeState = bootstrap.RuntimeState;

            Assert(
                composition != null
                && object.ReferenceEquals(composition, bootstrap.Composition),
                "Player session bootstrap should expose one stable player session composition.");

            Assert(
                loadedSession != null
                && object.ReferenceEquals(loadedSession, bootstrap.LoadedSession)
                && object.ReferenceEquals(
                    loadedSession,
                    composition.LoadedSession),
                "Player session bootstrap should expose the composition's one loaded session.");

            Assert(
                runtimeState != null
                && object.ReferenceEquals(runtimeState, bootstrap.RuntimeState)
                && object.ReferenceEquals(runtimeState, composition.RuntimeState),
                "Player session bootstrap should expose the composition's one runtime state.");
        }

        private static void AssertPlayerSessionBootstrapKeepsBindingRegistrySessionScoped()
        {
            var firstBindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var secondBindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var firstBootstrap = new PlayerSessionBootstrap(
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                firstBindingRegistry);
            var secondBootstrap = new PlayerSessionBootstrap(
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                secondBindingRegistry);

            Assert(
                object.ReferenceEquals(
                    firstBindingRegistry,
                    firstBootstrap.RuntimeState.ClassSkillTreeBindingRegistry)
                && object.ReferenceEquals(
                    firstBindingRegistry,
                    firstBootstrap.Composition.ClassSkillTreeBindingRegistry)
                && !object.ReferenceEquals(
                    firstBootstrap.RuntimeState.ClassSkillTreeBindingRegistry,
                    secondBootstrap.RuntimeState.ClassSkillTreeBindingRegistry),
                "Player session bootstrap should keep mutable bindings scoped to one loaded player session.");

            Assert(
                object.ReferenceEquals(
                    firstBootstrap.Composition.ClassRegistry,
                    firstBootstrap.RuntimeState.ClassRegistry)
                && object.ReferenceEquals(
                    firstBootstrap.Composition.SkillRegistry,
                    firstBootstrap.RuntimeState.SkillRegistry)
                && object.ReferenceEquals(
                    firstBootstrap.Composition.MentorRegistry,
                    firstBootstrap.RuntimeState.MentorRegistry),
                "Player session bootstrap should reuse canonical registries through its composition.");
        }

        private static void AssertPlayerSessionBootstrapRestoresProgressesAndCreatesCombat()
        {
            var bootstrap = BuildTestPlayerSessionBootstrap();
            var runtimeState = bootstrap.RuntimeState;
            var classProfile = runtimeState.ClassProfile;
            var masteryProfile = runtimeState.MasteryProfile;
            var skillTree = runtimeState.ClassSkillTreeBindingRegistry.GetSkillTree(
                SkillTreeId.Tlamatini);
            var restoreResult = new ProgressRestoreCoordinator().Restore(
                BuildCompleteV4SnapshotWithBinding(
                    ClassId.Tlamatini,
                    SkillTreeId.Tlamatini),
                runtimeState.ClassProfile,
                runtimeState.MasteryProfile,
                runtimeState.MentorRelationshipProfile,
                runtimeState.SkillTreeProgressProfile,
                runtimeState.ClassRegistry,
                runtimeState.SkillRegistry,
                runtimeState.MentorRegistry,
                skillTree,
                runtimeState.ClassSkillTreeBindingRegistry);
            var firstRandom = new FixedRandomSource(0.0, 0.99);
            var secondRandom = new FixedRandomSource(0.0, 0.99);
            var firstEngine = bootstrap.CreateCombatEngine(
                firstRandom,
                new List<ICombatPassiveEffect>());
            var secondEngine = bootstrap.CreateCombatEngine(
                secondRandom,
                new List<ICombatPassiveEffect>());
            var pipelineField = typeof(TurnBasedCombatEngine).GetField(
                "pipeline",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var randomField = typeof(TurnBasedCombatEngine).GetField(
                "random",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var validatorField = typeof(TurnBasedCombatEngine).GetField(
                "skillTurnValidator",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var firstPlayer = BuildCombatEntity(
                "session_bootstrap_allowed_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var firstEnemy = BuildCombatEntity(
                "session_bootstrap_allowed_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var firstDefinition = runtimeState.SkillRegistry.Get(
                "skill_balance_ritual");
            var firstSkill = new TrackingCombatSkill(firstDefinition.Id);

            firstEngine.ExecuteTurn(
                BuildCombatRuntime(firstPlayer, firstEnemy),
                TurnRequest.SkillAction(
                    firstPlayer,
                    firstEnemy,
                    firstSkill,
                    new SkillProgress(firstDefinition)));

            runtimeState.MasteryProfile.IncreaseProgress(
                "skill_deep_sight",
                MasteryRecord.MasteredProgress
                    - runtimeState.MasteryProfile.GetRecord(
                        "skill_deep_sight").Progress,
                runtimeState.SkillRegistry);
            runtimeState.ClassProfile.ChangeClassId(
                ClassId.Wanderer,
                runtimeState.ClassRegistry);
            var secondPlayer = BuildCombatEntity(
                "session_bootstrap_retained_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var secondEnemy = BuildCombatEntity(
                "session_bootstrap_retained_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var retainedDefinition = runtimeState.SkillRegistry.Get(
                "skill_deep_sight");
            var retainedSkill = new TrackingCombatSkill(retainedDefinition.Id);

            secondEngine.ExecuteTurn(
                BuildCombatRuntime(secondPlayer, secondEnemy),
                TurnRequest.SkillAction(
                    secondPlayer,
                    secondEnemy,
                    retainedSkill,
                    new SkillProgress(retainedDefinition)));

            Assert(
                restoreResult.FullyRestored
                && firstSkill.Applied
                && retainedSkill.Applied
                && object.ReferenceEquals(classProfile, runtimeState.ClassProfile)
                && object.ReferenceEquals(masteryProfile, runtimeState.MasteryProfile),
                "Player session bootstrap should restore and progress through the same runtime state used by combat.");

            Assert(
                firstEngine != secondEngine
                && pipelineField != null
                && !object.ReferenceEquals(
                    pipelineField.GetValue(firstEngine),
                    pipelineField.GetValue(secondEngine))
                && randomField != null
                && object.ReferenceEquals(
                    firstRandom,
                    randomField.GetValue(firstEngine))
                && object.ReferenceEquals(
                    secondRandom,
                    randomField.GetValue(secondEngine))
                && validatorField != null
                && validatorField.GetValue(firstEngine)
                    is CombatSkillTurnValidatorGateway,
                "Player session bootstrap should create progression-aware combat through PlayerSessionComposition.");
        }

        private static void AssertPlayerSessionBootstrapAllowsMasteredRetainedSkill()
        {
            var bootstrap = BuildTestPlayerSessionBootstrap();
            var runtimeState = bootstrap.RuntimeState;
            runtimeState.ClassProfile.AssignClassId(
                ClassId.Tlamatini,
                runtimeState.ClassRegistry);
            runtimeState.MasteryProfile.IncreaseProgress(
                "skill_deep_sight",
                MasteryRecord.MasteredProgress,
                runtimeState.SkillRegistry);
            runtimeState.ClassProfile.ChangeClassId(
                ClassId.Wanderer,
                runtimeState.ClassRegistry);
            var player = BuildCombatEntity(
                "session_bootstrap_mastered_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var enemy = BuildCombatEntity(
                "session_bootstrap_mastered_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var definition = runtimeState.SkillRegistry.Get("skill_deep_sight");
            var skill = new TrackingCombatSkill(definition.Id);

            bootstrap.CreateCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                new List<ICombatPassiveEffect>()).ExecuteTurn(
                    BuildCombatRuntime(player, enemy),
                    TurnRequest.SkillAction(
                        player,
                        enemy,
                        skill,
                        new SkillProgress(definition)));

            Assert(
                skill.Applied,
                "Player session bootstrap should allow mastered retained skills after progression mutation.");
        }

        private static void AssertPlayerSessionBootstrapBlocksCrossClassSkillSafely()
        {
            var bootstrap = BuildTestPlayerSessionBootstrap();
            var runtimeState = bootstrap.RuntimeState;
            runtimeState.ClassProfile.AssignClassId(
                ClassId.Wanderer,
                runtimeState.ClassRegistry);
            var resonance = new ResonanceProfile(
                new ActiveResonance(6),
                new ResonanceReserve(10));
            var player = BuildCombatEntity(
                "session_bootstrap_blocked_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var enemy = BuildCombatEntity(
                "session_bootstrap_blocked_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            int startingHealth = enemy.Stats.CurrentHealth;
            var runtime = BuildCombatRuntime(player, enemy, resonance);
            var definition = new SkillDefinition(
                "skill_balance_ritual",
                "Session Bootstrap Blocked",
                3,
                new SkillCost(10, 0),
                SkillCategory.Combat,
                SkillTargeting.SingleEnemy,
                SkillEffectType.Damage,
                "",
                2);
            var progress = new SkillProgress(definition);
            var skill = new TrackingCombatSkill(definition.Id);

            bootstrap.CreateCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                new List<ICombatPassiveEffect>()).ExecuteTurn(
                    runtime,
                    TurnRequest.SkillAction(
                        player,
                        enemy,
                        skill,
                        progress));

            Assert(
                !skill.Applied
                && CountEvents(
                    runtime.EventLog,
                    GameEventType.ActionBlocked) == 1
                && runtime.TurnNumber == 2,
                "Player session bootstrap should block non-mastered cross-class skills and consume the turn.");

            Assert(
                player.Resource.Current == 20
                && resonance.Active.Current == 6
                && resonance.Reserve.Current == 10
                && CountEvents(
                    runtime.EventLog,
                    GameEventType.ResourceSpent) == 0,
                "Player session bootstrap blocked validation should spend neither Essence nor Resonance.");

            Assert(
                progress.Experience == 0
                && enemy.Stats.CurrentHealth == startingHealth
                && CountEvents(
                    runtime.EventLog,
                    GameEventType.SkillUsed) == 0,
                "Player session bootstrap blocked validation should grant no experience or skill effects.");
        }

        private static void AssertPlayerSessionBootstrapPreservesLegacySlice()
        {
            Assert(
                FoundationVerticalSlice.Run().Events.Count == 38,
                "Player session bootstrap should preserve the legacy 38-event slice.");
        }

        private static void AssertPlayerSessionCompositionCreatesOneLoadedSession()
        {
            var composition = BuildTestPlayerSessionComposition();
            var loadedSession = composition.LoadedSession;
            var runtimeState = composition.RuntimeState;

            Assert(
                loadedSession != null
                && object.ReferenceEquals(loadedSession, composition.LoadedSession),
                "Player session composition should expose one stable loaded player session.");

            Assert(
                runtimeState != null
                && object.ReferenceEquals(runtimeState, composition.RuntimeState)
                && object.ReferenceEquals(
                    runtimeState,
                    loadedSession.RuntimeState),
                "Player session composition should expose one stable runtime state from its loaded session.");
        }

        private static void AssertPlayerSessionCompositionKeepsBindingRegistrySessionScoped()
        {
            var firstBindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var secondBindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var firstComposition = new PlayerSessionComposition(
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                firstBindingRegistry);
            var secondComposition = new PlayerSessionComposition(
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                secondBindingRegistry);
            var firstEngine = firstComposition.CreateCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                new List<ICombatPassiveEffect>());
            var secondEngine = firstComposition.CreateCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                new List<ICombatPassiveEffect>());

            Assert(
                object.ReferenceEquals(
                    firstBindingRegistry,
                    firstComposition.ClassSkillTreeBindingRegistry)
                && object.ReferenceEquals(
                    firstBindingRegistry,
                    firstComposition.RuntimeState.ClassSkillTreeBindingRegistry)
                && !object.ReferenceEquals(
                    firstComposition.ClassSkillTreeBindingRegistry,
                    secondComposition.ClassSkillTreeBindingRegistry),
                "Player session composition should keep mutable class-skill-tree bindings scoped to one loaded session.");

            Assert(
                !object.ReferenceEquals(firstEngine, secondEngine)
                && object.ReferenceEquals(
                    firstComposition.ClassRegistry,
                    firstComposition.RuntimeState.ClassRegistry)
                && object.ReferenceEquals(
                    firstComposition.SkillRegistry,
                    firstComposition.RuntimeState.SkillRegistry)
                && object.ReferenceEquals(
                    firstComposition.MentorRegistry,
                    firstComposition.RuntimeState.MentorRegistry),
                "Player session composition should reuse canonical registries while creating fresh combat engines.");
        }

        private static void AssertPlayerSessionCompositionRestoreAndProgressionMutateRuntimeState()
        {
            var composition = BuildTestPlayerSessionComposition();
            var runtimeState = composition.RuntimeState;
            var classProfile = runtimeState.ClassProfile;
            var masteryProfile = runtimeState.MasteryProfile;
            var skillTree = runtimeState.ClassSkillTreeBindingRegistry.GetSkillTree(
                SkillTreeId.Tlamatini);

            var restoreResult = new ProgressRestoreCoordinator().Restore(
                BuildCompleteV4SnapshotWithBinding(
                    ClassId.Tlamatini,
                    SkillTreeId.Tlamatini),
                runtimeState.ClassProfile,
                runtimeState.MasteryProfile,
                runtimeState.MentorRelationshipProfile,
                runtimeState.SkillTreeProgressProfile,
                runtimeState.ClassRegistry,
                runtimeState.SkillRegistry,
                runtimeState.MentorRegistry,
                skillTree,
                runtimeState.ClassSkillTreeBindingRegistry);

            runtimeState.MasteryProfile.IncreaseProgress(
                "skill_deep_sight",
                MasteryRecord.MasteredProgress
                    - runtimeState.MasteryProfile.GetRecord(
                        "skill_deep_sight").Progress,
                runtimeState.SkillRegistry);

            Assert(
                restoreResult.FullyRestored
                && object.ReferenceEquals(runtimeState, composition.RuntimeState)
                && object.ReferenceEquals(
                    runtimeState,
                    composition.LoadedSession.RuntimeState)
                && object.ReferenceEquals(classProfile, runtimeState.ClassProfile)
                && object.ReferenceEquals(masteryProfile, runtimeState.MasteryProfile)
                && runtimeState.MasteryProfile.IsMastered("skill_deep_sight"),
                "Player session composition restore and progression should mutate the same runtime state exposed to bootstrap.");
        }

        private static void AssertPlayerSessionCompositionCreatesProgressionAwareCombat()
        {
            var composition = BuildTestPlayerSessionComposition();
            var runtimeState = composition.RuntimeState;
            runtimeState.ClassProfile.AssignClassId(
                ClassId.Tlamatini,
                runtimeState.ClassRegistry);
            var firstRandom = new FixedRandomSource(0.0, 0.99);
            var secondRandom = new FixedRandomSource(0.0, 0.99);
            var firstEngine = composition.CreateCombatEngine(
                firstRandom,
                new List<ICombatPassiveEffect>());
            var secondEngine = composition.CreateCombatEngine(
                secondRandom,
                new List<ICombatPassiveEffect>());
            var pipelineField = typeof(TurnBasedCombatEngine).GetField(
                "pipeline",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var randomField = typeof(TurnBasedCombatEngine).GetField(
                "random",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var validatorField = typeof(TurnBasedCombatEngine).GetField(
                "skillTurnValidator",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var player = BuildCombatEntity(
                "session_composition_allowed_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var enemy = BuildCombatEntity(
                "session_composition_allowed_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var definition = runtimeState.SkillRegistry.Get(
                "skill_balance_ritual");
            var skill = new TrackingCombatSkill(definition.Id);

            firstEngine.ExecuteTurn(
                BuildCombatRuntime(player, enemy),
                TurnRequest.SkillAction(
                    player,
                    enemy,
                    skill,
                    new SkillProgress(definition)));

            Assert(
                firstEngine != secondEngine
                && pipelineField != null
                && !object.ReferenceEquals(
                    pipelineField.GetValue(firstEngine),
                    pipelineField.GetValue(secondEngine))
                && randomField != null
                && object.ReferenceEquals(
                    firstRandom,
                    randomField.GetValue(firstEngine))
                && object.ReferenceEquals(
                    secondRandom,
                    randomField.GetValue(secondEngine)),
                "Player session composition should create fresh per-combat dependencies through its loaded session.");

            Assert(
                validatorField != null
                && validatorField.GetValue(firstEngine)
                    is CombatSkillTurnValidatorGateway
                && skill.Applied,
                "Player session composition should create progression-aware combat that allows current-class skills.");
        }

        private static void AssertPlayerSessionCompositionAllowsMasteredRetainedSkill()
        {
            var composition = BuildTestPlayerSessionComposition();
            var runtimeState = composition.RuntimeState;
            runtimeState.ClassProfile.AssignClassId(
                ClassId.Tlamatini,
                runtimeState.ClassRegistry);
            runtimeState.MasteryProfile.IncreaseProgress(
                "skill_deep_sight",
                MasteryRecord.MasteredProgress,
                runtimeState.SkillRegistry);
            runtimeState.ClassProfile.ChangeClassId(
                ClassId.Wanderer,
                runtimeState.ClassRegistry);
            var player = BuildCombatEntity(
                "session_composition_retained_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var enemy = BuildCombatEntity(
                "session_composition_retained_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var definition = runtimeState.SkillRegistry.Get("skill_deep_sight");
            var skill = new TrackingCombatSkill(definition.Id);

            composition.CreateCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                new List<ICombatPassiveEffect>()).ExecuteTurn(
                    BuildCombatRuntime(player, enemy),
                    TurnRequest.SkillAction(
                        player,
                        enemy,
                        skill,
                        new SkillProgress(definition)));

            Assert(
                skill.Applied,
                "Player session composition should allow mastered retained skills after progression mutates its runtime state.");
        }

        private static void AssertPlayerSessionCompositionBlocksCrossClassSkillSafely()
        {
            var composition = BuildTestPlayerSessionComposition();
            var runtimeState = composition.RuntimeState;
            runtimeState.ClassProfile.AssignClassId(
                ClassId.Wanderer,
                runtimeState.ClassRegistry);
            var resonance = new ResonanceProfile(
                new ActiveResonance(6),
                new ResonanceReserve(10));
            var player = BuildCombatEntity(
                "session_composition_blocked_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var enemy = BuildCombatEntity(
                "session_composition_blocked_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            int startingHealth = enemy.Stats.CurrentHealth;
            var runtime = BuildCombatRuntime(player, enemy, resonance);
            var definition = new SkillDefinition(
                "skill_balance_ritual",
                "Session Composition Blocked",
                3,
                new SkillCost(10, 0),
                SkillCategory.Combat,
                SkillTargeting.SingleEnemy,
                SkillEffectType.Damage,
                "",
                2);
            var progress = new SkillProgress(definition);
            var skill = new TrackingCombatSkill(definition.Id);

            composition.CreateCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                new List<ICombatPassiveEffect>()).ExecuteTurn(
                    runtime,
                    TurnRequest.SkillAction(
                        player,
                        enemy,
                        skill,
                        progress));

            Assert(
                !skill.Applied
                && CountEvents(
                    runtime.EventLog,
                    GameEventType.ActionBlocked) == 1
                && runtime.TurnNumber == 2,
                "Player session composition should block non-mastered cross-class skills and consume the turn.");

            Assert(
                player.Resource.Current == 20
                && resonance.Active.Current == 6
                && resonance.Reserve.Current == 10
                && CountEvents(
                    runtime.EventLog,
                    GameEventType.ResourceSpent) == 0,
                "Player session composition blocked validation should spend neither Essence nor Resonance.");

            Assert(
                progress.Experience == 0
                && enemy.Stats.CurrentHealth == startingHealth
                && CountEvents(
                    runtime.EventLog,
                    GameEventType.SkillUsed) == 0,
                "Player session composition blocked validation should grant no experience or skill effects.");
        }

        private static void AssertPlayerSessionCompositionPreservesLegacySlice()
        {
            Assert(
                FoundationVerticalSlice.Run().Events.Count == 38,
                "Player session composition should preserve the legacy 38-event slice.");
        }

        private static void AssertLoadedSessionOwnsSingleRuntimeState()
        {
            var classRegistry = BuildTestClassRegistry();
            var skillRegistry = BuildTestSkillRegistry();
            var mentorRegistry = BuildTestMentorRegistry();
            var bindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var session = new LoadedPlayerSession(
                classRegistry,
                skillRegistry,
                mentorRegistry,
                bindingRegistry);
            var runtimeState = session.RuntimeState;

            Assert(
                runtimeState != null
                && object.ReferenceEquals(runtimeState, session.RuntimeState),
                "Loaded session should expose one stable player progression runtime state.");

            Assert(
                object.ReferenceEquals(classRegistry, runtimeState.ClassRegistry)
                && object.ReferenceEquals(skillRegistry, runtimeState.SkillRegistry)
                && object.ReferenceEquals(mentorRegistry, runtimeState.MentorRegistry)
                && object.ReferenceEquals(
                    bindingRegistry,
                    runtimeState.ClassSkillTreeBindingRegistry),
                "Loaded session should pass canonical registries into its owned runtime state without rebuilding them.");
        }

        private static void AssertLoadedSessionRestoreAndProgressionMutateOwnedState()
        {
            var session = BuildTestLoadedPlayerSession();
            var runtimeState = session.RuntimeState;
            var classProfile = runtimeState.ClassProfile;
            var masteryProfile = runtimeState.MasteryProfile;
            var skillTree = runtimeState.ClassSkillTreeBindingRegistry.GetSkillTree(
                SkillTreeId.Tlamatini);

            var restoreResult = new ProgressRestoreCoordinator().Restore(
                BuildCompleteV4SnapshotWithBinding(
                    ClassId.Tlamatini,
                    SkillTreeId.Tlamatini),
                runtimeState.ClassProfile,
                runtimeState.MasteryProfile,
                runtimeState.MentorRelationshipProfile,
                runtimeState.SkillTreeProgressProfile,
                runtimeState.ClassRegistry,
                runtimeState.SkillRegistry,
                runtimeState.MentorRegistry,
                skillTree,
                runtimeState.ClassSkillTreeBindingRegistry);

            runtimeState.MasteryProfile.IncreaseProgress(
                "skill_deep_sight",
                MasteryRecord.MasteredProgress
                    - runtimeState.MasteryProfile.GetRecord(
                        "skill_deep_sight").Progress,
                runtimeState.SkillRegistry);

            Assert(
                restoreResult.FullyRestored
                && object.ReferenceEquals(runtimeState, session.RuntimeState)
                && object.ReferenceEquals(classProfile, runtimeState.ClassProfile)
                && object.ReferenceEquals(masteryProfile, runtimeState.MasteryProfile)
                && runtimeState.MasteryProfile.IsMastered("skill_deep_sight"),
                "Loaded session restore and progression should mutate the same owned runtime state.");
        }

        private static void AssertLoadedSessionCreatesProgressionAwareEngines()
        {
            var session = BuildTestLoadedPlayerSession();
            var runtimeState = session.RuntimeState;
            runtimeState.ClassProfile.AssignClassId(
                ClassId.Tlamatini,
                runtimeState.ClassRegistry);
            var firstRandom = new FixedRandomSource(0.0, 0.99);
            var secondRandom = new FixedRandomSource(0.0, 0.99);
            var firstEngine = session.CreateCombatEngine(
                firstRandom,
                new List<ICombatPassiveEffect>());
            var secondEngine = session.CreateCombatEngine(
                secondRandom,
                new List<ICombatPassiveEffect>());
            var pipelineField = typeof(TurnBasedCombatEngine).GetField(
                "pipeline",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var randomField = typeof(TurnBasedCombatEngine).GetField(
                "random",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var validatorField = typeof(TurnBasedCombatEngine).GetField(
                "skillTurnValidator",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var player = BuildCombatEntity(
                "loaded_session_allowed_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var enemy = BuildCombatEntity(
                "loaded_session_allowed_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var definition = runtimeState.SkillRegistry.Get(
                "skill_balance_ritual");
            var skill = new TrackingCombatSkill(definition.Id);

            firstEngine.ExecuteTurn(
                BuildCombatRuntime(player, enemy),
                TurnRequest.SkillAction(
                    player,
                    enemy,
                    skill,
                    new SkillProgress(definition)));

            Assert(
                firstEngine != secondEngine
                && pipelineField != null
                && !object.ReferenceEquals(
                    pipelineField.GetValue(firstEngine),
                    pipelineField.GetValue(secondEngine))
                && randomField != null
                && object.ReferenceEquals(
                    firstRandom,
                    randomField.GetValue(firstEngine))
                && object.ReferenceEquals(
                    secondRandom,
                    randomField.GetValue(secondEngine)),
                "Loaded session should create fresh per-combat engine dependencies.");

            Assert(
                validatorField != null
                && validatorField.GetValue(firstEngine)
                    is CombatSkillTurnValidatorGateway
                && skill.Applied,
                "Loaded session should create progression-aware engines that allow current-class skills.");
        }

        private static void AssertLoadedSessionAllowsMasteredRetainedSkill()
        {
            var session = BuildTestLoadedPlayerSession();
            var runtimeState = session.RuntimeState;
            runtimeState.ClassProfile.AssignClassId(
                ClassId.Tlamatini,
                runtimeState.ClassRegistry);
            runtimeState.MasteryProfile.IncreaseProgress(
                "skill_deep_sight",
                MasteryRecord.MasteredProgress,
                runtimeState.SkillRegistry);
            runtimeState.ClassProfile.ChangeClassId(
                ClassId.Wanderer,
                runtimeState.ClassRegistry);
            var player = BuildCombatEntity(
                "loaded_session_retained_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var enemy = BuildCombatEntity(
                "loaded_session_retained_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var definition = runtimeState.SkillRegistry.Get("skill_deep_sight");
            var skill = new TrackingCombatSkill(definition.Id);

            session.CreateCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                new List<ICombatPassiveEffect>()).ExecuteTurn(
                    BuildCombatRuntime(player, enemy),
                    TurnRequest.SkillAction(
                        player,
                        enemy,
                        skill,
                        new SkillProgress(definition)));

            Assert(
                skill.Applied,
                "Loaded session should allow mastered retained skills after progression mutates its runtime state.");
        }

        private static void AssertLoadedSessionBlocksCrossClassSkillSafely()
        {
            var session = BuildTestLoadedPlayerSession();
            var runtimeState = session.RuntimeState;
            runtimeState.ClassProfile.AssignClassId(
                ClassId.Wanderer,
                runtimeState.ClassRegistry);
            var resonance = new ResonanceProfile(
                new ActiveResonance(6),
                new ResonanceReserve(10));
            var player = BuildCombatEntity(
                "loaded_session_blocked_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var enemy = BuildCombatEntity(
                "loaded_session_blocked_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            int startingHealth = enemy.Stats.CurrentHealth;
            var runtime = BuildCombatRuntime(player, enemy, resonance);
            var definition = new SkillDefinition(
                "skill_balance_ritual",
                "Loaded Session Blocked",
                3,
                new SkillCost(10, 0),
                SkillCategory.Combat,
                SkillTargeting.SingleEnemy,
                SkillEffectType.Damage,
                "",
                2);
            var progress = new SkillProgress(definition);
            var skill = new TrackingCombatSkill(definition.Id);

            session.CreateCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                new List<ICombatPassiveEffect>()).ExecuteTurn(
                    runtime,
                    TurnRequest.SkillAction(
                        player,
                        enemy,
                        skill,
                        progress));

            Assert(
                !skill.Applied
                && CountEvents(
                    runtime.EventLog,
                    GameEventType.ActionBlocked) == 1
                && runtime.TurnNumber == 2,
                "Loaded session should block non-mastered cross-class skills and consume the turn.");

            Assert(
                player.Resource.Current == 20
                && resonance.Active.Current == 6
                && resonance.Reserve.Current == 10
                && CountEvents(
                    runtime.EventLog,
                    GameEventType.ResourceSpent) == 0,
                "Loaded session blocked validation should spend neither Essence nor Resonance.");

            Assert(
                progress.Experience == 0
                && enemy.Stats.CurrentHealth == startingHealth
                && CountEvents(
                    runtime.EventLog,
                    GameEventType.SkillUsed) == 0,
                "Loaded session blocked validation should grant no experience or skill effects.");
        }

        private static void AssertLoadedSessionPreservesLegacySlice()
        {
            Assert(
                FoundationVerticalSlice.Run().Events.Count == 38,
                "Loaded session foundation should preserve the legacy 38-event slice.");
        }

        private static void AssertPlayerProgressionRuntimeStateExposesLiveProfiles()
        {
            var state = BuildTestPlayerProgressionRuntimeState();

            Assert(
                state.UnlockProfile != null
                && state.ClassProfile != null
                && state.MasteryProfile != null
                && state.MentorRelationshipProfile != null
                && state.SkillTreeProgressProfile != null
                && state.ClassSkillTreeBindingRegistry != null,
                "V4 runtime state should expose all required live progression and binding state.");
        }

        private static void AssertPlayerProgressionRuntimeStateKeepsProfileIdentity()
        {
            var state = BuildTestPlayerProgressionRuntimeState();
            var unlockProfile = state.UnlockProfile;
            var classProfile = state.ClassProfile;
            var masteryProfile = state.MasteryProfile;
            var mentorProfile = state.MentorRelationshipProfile;
            var skillTreeProfile = state.SkillTreeProgressProfile;

            Assert(
                object.ReferenceEquals(unlockProfile, state.UnlockProfile)
                && object.ReferenceEquals(classProfile, state.ClassProfile)
                && object.ReferenceEquals(masteryProfile, state.MasteryProfile)
                && object.ReferenceEquals(
                    mentorProfile,
                    state.MentorRelationshipProfile)
                && object.ReferenceEquals(
                    skillTreeProfile,
                    state.SkillTreeProgressProfile),
                "Repeated V4 runtime state access should return the same live profile instances.");
        }

        private static void AssertPlayerProgressionRuntimeStateExposesCanonicalRegistries()
        {
            var classRegistry = BuildTestClassRegistry();
            var skillRegistry = BuildTestSkillRegistry();
            var mentorRegistry = BuildTestMentorRegistry();
            var bindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var state = new PlayerProgressionRuntimeState(
                classRegistry,
                skillRegistry,
                mentorRegistry,
                bindingRegistry);

            Assert(
                object.ReferenceEquals(classRegistry, state.ClassRegistry)
                && object.ReferenceEquals(skillRegistry, state.SkillRegistry)
                && object.ReferenceEquals(mentorRegistry, state.MentorRegistry)
                && object.ReferenceEquals(
                    bindingRegistry,
                    state.ClassSkillTreeBindingRegistry),
                "V4 runtime state should expose injected canonical registries without rebuilding them.");
        }

        private static void AssertPlayerProgressionRuntimeStateRestoreMutatesOwnedProfiles()
        {
            var state = BuildTestPlayerProgressionRuntimeState();
            var classProfile = state.ClassProfile;
            var masteryProfile = state.MasteryProfile;
            var mentorProfile = state.MentorRelationshipProfile;
            var skillTreeProfile = state.SkillTreeProgressProfile;
            var skillTree = state.ClassSkillTreeBindingRegistry.GetSkillTree(
                SkillTreeId.Tlamatini);
            var snapshot = BuildCompleteV4SnapshotWithBinding(
                ClassId.Tlamatini,
                SkillTreeId.Tlamatini);

            var result = new ProgressRestoreCoordinator().Restore(
                snapshot,
                state.ClassProfile,
                state.MasteryProfile,
                state.MentorRelationshipProfile,
                state.SkillTreeProgressProfile,
                state.ClassRegistry,
                state.SkillRegistry,
                state.MentorRegistry,
                skillTree,
                state.ClassSkillTreeBindingRegistry);

            Assert(
                result.FullyRestored
                && object.ReferenceEquals(classProfile, state.ClassProfile)
                && object.ReferenceEquals(masteryProfile, state.MasteryProfile)
                && object.ReferenceEquals(
                    mentorProfile,
                    state.MentorRelationshipProfile)
                && object.ReferenceEquals(
                    skillTreeProfile,
                    state.SkillTreeProgressProfile),
                "V4 restore should mutate the runtime state's existing live profile instances.");

            Assert(
                state.ClassProfile.CurrentClassId == ClassId.Tlamatini
                && state.MasteryProfile.GetRecord("skill_deep_sight").Progress
                    == 25
                && state.MentorRelationshipProfile.IsDiscovered(MentorId.Lyra)
                && state.MentorRelationshipProfile.GetTrust(MentorId.Lyra) == 12
                && state.SkillTreeProgressProfile.IsUnlocked(
                    "node_deep_sight",
                    skillTree)
                && state.ClassSkillTreeBindingRegistry.GetBindingForClass(
                    ClassId.Tlamatini) != null,
                "V4 restore should populate all owned progression and binding state.");
        }

        private static void AssertPlayerProgressionRuntimeStateFeedsLiveProfilesToValidation()
        {
            var state = BuildTestPlayerProgressionRuntimeState();
            ICombatSkillTurnValidator validator =
                CombatSkillTurnValidationComposition.CreateValidator(
                    state.UnlockProfile,
                    state.ClassProfile,
                    state.MasteryProfile,
                    state.ClassRegistry,
                    state.SkillRegistry);

            state.ClassProfile.AssignClassId(
                ClassId.Tlamatini,
                state.ClassRegistry);

            var player = BuildCombatEntity(
                "runtime_state_validation_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var enemy = BuildCombatEntity(
                "runtime_state_validation_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var definition = state.SkillRegistry.Get("skill_deep_sight");

            var result = validator.Validate(
                TurnRequest.SkillAction(
                    player,
                    enemy,
                    new V4ValidatorCombatSkill(definition.Id),
                    new SkillProgress(definition)),
                BuildCombatRuntime(player, enemy));

            Assert(
                result.IsAllowed
                && result.ValidationMode == CombatSkillUseValidationMode.Player,
                "Future validation composition should observe the runtime state's live profile instances without cloning.");
        }

        private static void AssertPlayerProgressionRuntimeStateDoesNotOwnCombatEngines()
        {
            bool ownsEngine = false;
            var runtimeStateType = typeof(PlayerProgressionRuntimeState);

            foreach (var property in runtimeStateType.GetProperties())
            {
                if (property.PropertyType == typeof(TurnBasedCombatEngine)
                    || property.PropertyType == typeof(CombatRuntimeContext))
                {
                    ownsEngine = true;
                }
            }

            foreach (var method in runtimeStateType.GetMethods())
            {
                if (method.DeclaringType == runtimeStateType
                    && (method.ReturnType == typeof(TurnBasedCombatEngine)
                        || method.ReturnType == typeof(CombatRuntimeContext)))
                {
                    ownsEngine = true;
                }
            }

            Assert(
                !ownsEngine,
                "V4 runtime state should leave engines and combat runtime objects per-combat.");
        }

        private static PlayerProgressionRuntimeState BuildTestPlayerProgressionRuntimeState()
        {
            return new PlayerProgressionRuntimeState(
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                BuildTestClassSkillTreeBindingRegistry());
        }

        private static LoadedPlayerSession BuildTestLoadedPlayerSession()
        {
            return new LoadedPlayerSession(
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                BuildTestClassSkillTreeBindingRegistry());
        }

        private static PlayerSessionComposition BuildTestPlayerSessionComposition()
        {
            return new PlayerSessionComposition(
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                BuildTestClassSkillTreeBindingRegistry());
        }

        private static PlayerSessionBootstrap BuildTestPlayerSessionBootstrap()
        {
            return new PlayerSessionBootstrap(
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                BuildTestClassSkillTreeBindingRegistry());
        }

        private static ProgressionRuntimeDemoBootstrap BuildTestProgressionRuntimeDemoBootstrap()
        {
            return new ProgressionRuntimeDemoBootstrap(
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                BuildTestClassSkillTreeBindingRegistry());
        }

        private static void AssertPlayerProgressionRuntimeStateCombatValidationIntegratedWorkflow()
        {
            var state = BuildTestPlayerProgressionRuntimeState();
            var classProfile = state.ClassProfile;
            var masteryProfile = state.MasteryProfile;
            var skillTree = state.ClassSkillTreeBindingRegistry.GetSkillTree(
                SkillTreeId.Tlamatini);
            var restoreResult = new ProgressRestoreCoordinator().Restore(
                BuildCompleteV4SnapshotWithBinding(
                    ClassId.Tlamatini,
                    SkillTreeId.Tlamatini),
                state.ClassProfile,
                state.MasteryProfile,
                state.MentorRelationshipProfile,
                state.SkillTreeProgressProfile,
                state.ClassRegistry,
                state.SkillRegistry,
                state.MentorRegistry,
                skillTree,
                state.ClassSkillTreeBindingRegistry);

            Assert(
                restoreResult.FullyRestored
                && object.ReferenceEquals(classProfile, state.ClassProfile)
                && object.ReferenceEquals(masteryProfile, state.MasteryProfile)
                && state.ClassProfile.CurrentClassId == ClassId.Tlamatini,
                "Runtime-state workflow should restore into the same live profile instances.");

            var classSkillDefinition = state.SkillRegistry.Get(
                "skill_balance_ritual");
            var classSkill = new TrackingCombatSkill(classSkillDefinition.Id);
            var classSkillProgress = new SkillProgress(classSkillDefinition);
            var classSkillPlayer = BuildCombatEntity(
                "runtime_workflow_class_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var classSkillEnemy = BuildCombatEntity(
                "runtime_workflow_class_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var classSkillRuntime = BuildCombatRuntime(
                classSkillPlayer,
                classSkillEnemy);

            CombatSkillTurnValidationComposition.CreateProgressionAwareEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline(),
                state.UnlockProfile,
                state.ClassProfile,
                state.MasteryProfile,
                state.ClassRegistry,
                state.SkillRegistry).ExecuteTurn(
                    classSkillRuntime,
                    TurnRequest.SkillAction(
                        classSkillPlayer,
                        classSkillEnemy,
                        classSkill,
                        classSkillProgress));

            Assert(
                classSkill.Applied
                && classSkillProgress.Experience == 10
                && CountEvents(
                    classSkillRuntime.EventLog,
                    GameEventType.ActionBlocked) == 0,
                "Runtime-state workflow should allow a restored current-class skill.");

            state.MasteryProfile.IncreaseProgress(
                "skill_deep_sight",
                MasteryRecord.MasteredProgress
                    - state.MasteryProfile.GetRecord("skill_deep_sight").Progress,
                state.SkillRegistry);
            state.ClassProfile.ChangeClassId(
                ClassId.Wanderer,
                state.ClassRegistry);

            Assert(
                state.MasteryProfile.IsMastered("skill_deep_sight")
                && state.ClassProfile.CurrentClassId == ClassId.Wanderer,
                "Runtime-state workflow should retain progression mutations on the live profiles.");

            var retainedDefinition = state.SkillRegistry.Get("skill_deep_sight");
            var retainedSkill = new TrackingCombatSkill(retainedDefinition.Id);
            var retainedPlayer = BuildCombatEntity(
                "runtime_workflow_retained_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var retainedEnemy = BuildCombatEntity(
                "runtime_workflow_retained_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);

            CombatSkillTurnValidationComposition.CreateProgressionAwareEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline(),
                state.UnlockProfile,
                state.ClassProfile,
                state.MasteryProfile,
                state.ClassRegistry,
                state.SkillRegistry).ExecuteTurn(
                    BuildCombatRuntime(retainedPlayer, retainedEnemy),
                    TurnRequest.SkillAction(
                        retainedPlayer,
                        retainedEnemy,
                        retainedSkill,
                        new SkillProgress(retainedDefinition)));

            Assert(
                retainedSkill.Applied,
                "Runtime-state workflow should allow a mastered retained skill after the live class switch.");

            var resonance = new ResonanceProfile(
                new ActiveResonance(6),
                new ResonanceReserve(10));
            var blockedPlayer = BuildCombatEntity(
                "runtime_workflow_blocked_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var blockedEnemy = BuildCombatEntity(
                "runtime_workflow_blocked_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var blockedRuntime = BuildCombatRuntime(
                blockedPlayer,
                blockedEnemy,
                resonance);
            var blockedDefinition = new SkillDefinition(
                "skill_balance_ritual",
                "Runtime Workflow Blocked",
                3,
                new SkillCost(10, 0),
                SkillCategory.Combat,
                SkillTargeting.SingleEnemy,
                SkillEffectType.Damage,
                "",
                2);
            var blockedProgress = new SkillProgress(blockedDefinition);
            var blockedSkill = new TrackingCombatSkill(blockedDefinition.Id);

            CombatSkillTurnValidationComposition.CreateProgressionAwareEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline(),
                state.UnlockProfile,
                state.ClassProfile,
                state.MasteryProfile,
                state.ClassRegistry,
                state.SkillRegistry).ExecuteTurn(
                    blockedRuntime,
                    TurnRequest.SkillAction(
                        blockedPlayer,
                        blockedEnemy,
                        blockedSkill,
                        blockedProgress));

            Assert(
                CountEvents(
                    blockedRuntime.EventLog,
                    GameEventType.ActionBlocked) == 1
                && blockedRuntime.TurnNumber == 2,
                "Runtime-state workflow should block a non-mastered cross-class skill and consume the turn.");

            Assert(
                blockedPlayer.Resource.Current == 20
                && resonance.Active.Current == 6
                && resonance.Reserve.Current == 10
                && CountEvents(
                    blockedRuntime.EventLog,
                    GameEventType.ResourceSpent) == 0,
                "Runtime-state workflow blocking should spend neither Essence nor Resonance.");

            Assert(
                blockedProgress.Experience == 0
                && !blockedSkill.Applied
                && CountEvents(
                    blockedRuntime.EventLog,
                    GameEventType.SkillUsed) == 0
                && CountEvents(
                    blockedRuntime.EventLog,
                    GameEventType.ResourceNotEnough) == 0,
                "Runtime-state workflow blocking should grant no experience or skill effects.");

            var missingPlayer = BuildCombatEntity(
                "runtime_workflow_missing_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var missingEnemy = BuildCombatEntity(
                "runtime_workflow_missing_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var missingRuntime = BuildCombatRuntime(
                missingPlayer,
                missingEnemy);
            var missingSkill = new TrackingCombatSkill(
                classSkillDefinition.Id);

            CombatSkillTurnValidationComposition.CreateProgressionAwareEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline(),
                null,
                null,
                null,
                null,
                null).ExecuteTurn(
                    missingRuntime,
                    TurnRequest.SkillAction(
                        missingPlayer,
                        missingEnemy,
                        missingSkill,
                        new SkillProgress(classSkillDefinition)));

            Assert(
                !missingSkill.Applied
                && CountEvents(
                    missingRuntime.EventLog,
                    GameEventType.ActionBlocked) == 1,
                "Runtime-state workflow should fail closed when player context is missing.");

            var enemyPathPlayer = BuildCombatEntity(
                "runtime_workflow_enemy_path_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var enemyPathEnemy = BuildCombatEntity(
                "runtime_workflow_enemy_path_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                20);
            var enemyPathRuntime = BuildCombatRuntime(
                enemyPathPlayer,
                enemyPathEnemy);
            var enemyPathSkill = new TrackingCombatSkill(
                classSkillDefinition.Id);

            CombatSkillTurnValidationComposition.CreateProgressionAwareEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline(),
                null,
                null,
                null,
                null,
                null).ExecuteTurn(
                    enemyPathRuntime,
                    TurnRequest.SkillAction(
                        enemyPathEnemy,
                        enemyPathPlayer,
                        enemyPathSkill,
                        new SkillProgress(classSkillDefinition)));

            Assert(
                enemyPathSkill.Applied
                && CountEvents(
                    enemyPathRuntime.EventLog,
                    GameEventType.ActionBlocked) == 0,
                "Runtime-state workflow enemy validation should require no player progression context.");

            var legacyPlayer = BuildCombatEntity(
                "runtime_workflow_legacy_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var legacyEnemy = BuildCombatEntity(
                "runtime_workflow_legacy_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var legacyRuntime = BuildCombatRuntime(legacyPlayer, legacyEnemy);
            var legacySkill = new TrackingCombatSkill(
                classSkillDefinition.Id);

            new TurnBasedCombatEngine(
                new FixedRandomSource(0.0, 0.99),
                BuildEmptyPassivePipeline()).ExecuteTurn(
                    legacyRuntime,
                    TurnRequest.SkillAction(
                        legacyPlayer,
                        legacyEnemy,
                        legacySkill,
                        new SkillProgress(classSkillDefinition)));

            Assert(
                legacySkill.Applied
                && CountEvents(
                    legacyRuntime.EventLog,
                    GameEventType.ActionBlocked) == 0,
                "Runtime-state workflow should preserve legacy no-validator pass-through behavior.");
        }

        private static void AssertLoadedPlayerSessionIntegratedWorkflow()
        {
            var session = BuildTestLoadedPlayerSession();
            var runtimeState = session.RuntimeState;
            var classProfile = runtimeState.ClassProfile;
            var masteryProfile = runtimeState.MasteryProfile;
            var classRegistry = runtimeState.ClassRegistry;
            var skillRegistry = runtimeState.SkillRegistry;
            var skillTree = runtimeState.ClassSkillTreeBindingRegistry.GetSkillTree(
                SkillTreeId.Tlamatini);
            var restoreResult = new ProgressRestoreCoordinator().Restore(
                BuildCompleteV4SnapshotWithBinding(
                    ClassId.Tlamatini,
                    SkillTreeId.Tlamatini),
                runtimeState.ClassProfile,
                runtimeState.MasteryProfile,
                runtimeState.MentorRelationshipProfile,
                runtimeState.SkillTreeProgressProfile,
                runtimeState.ClassRegistry,
                runtimeState.SkillRegistry,
                runtimeState.MentorRegistry,
                skillTree,
                runtimeState.ClassSkillTreeBindingRegistry);

            Assert(
                restoreResult.FullyRestored
                && object.ReferenceEquals(runtimeState, session.RuntimeState)
                && object.ReferenceEquals(classProfile, runtimeState.ClassProfile)
                && object.ReferenceEquals(masteryProfile, runtimeState.MasteryProfile)
                && object.ReferenceEquals(classRegistry, runtimeState.ClassRegistry)
                && object.ReferenceEquals(skillRegistry, runtimeState.SkillRegistry),
                "Loaded-session workflow should restore into one stable runtime state and canonical registries.");

            var firstRandom = new FixedRandomSource(0.0, 0.99);
            TurnBasedCombatEngine firstEngine = session.CreateCombatEngine(
                firstRandom,
                new List<ICombatPassiveEffect>());
            var firstPlayer = BuildCombatEntity(
                "loaded_workflow_first_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var firstEnemy = BuildCombatEntity(
                "loaded_workflow_first_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var firstDefinition = runtimeState.SkillRegistry.Get(
                "skill_balance_ritual");
            var firstSkill = new TrackingCombatSkill(firstDefinition.Id);
            var firstProgress = new SkillProgress(firstDefinition);

            firstEngine.ExecuteTurn(
                BuildCombatRuntime(firstPlayer, firstEnemy),
                TurnRequest.SkillAction(
                    firstPlayer,
                    firstEnemy,
                    firstSkill,
                    firstProgress));

            Assert(
                firstSkill.Applied
                && firstProgress.Experience == 10,
                "Loaded-session workflow should allow a restored current-class skill through the first created engine.");

            runtimeState.MasteryProfile.IncreaseProgress(
                "skill_deep_sight",
                MasteryRecord.MasteredProgress
                    - runtimeState.MasteryProfile.GetRecord(
                        "skill_deep_sight").Progress,
                runtimeState.SkillRegistry);
            runtimeState.ClassProfile.ChangeClassId(
                ClassId.Wanderer,
                runtimeState.ClassRegistry);

            Assert(
                object.ReferenceEquals(runtimeState, session.RuntimeState)
                && object.ReferenceEquals(classProfile, runtimeState.ClassProfile)
                && object.ReferenceEquals(masteryProfile, runtimeState.MasteryProfile)
                && runtimeState.MasteryProfile.IsMastered("skill_deep_sight")
                && runtimeState.ClassProfile.CurrentClassId == ClassId.Wanderer,
                "Loaded-session workflow progression should mutate the same runtime state before later combats.");

            var secondRandom = new FixedRandomSource(0.0, 0.99);
            TurnBasedCombatEngine secondEngine = session.CreateCombatEngine(
                secondRandom,
                new List<ICombatPassiveEffect>());
            var secondPlayer = BuildCombatEntity(
                "loaded_workflow_second_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var secondEnemy = BuildCombatEntity(
                "loaded_workflow_second_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var retainedDefinition = runtimeState.SkillRegistry.Get(
                "skill_deep_sight");
            var retainedSkill = new TrackingCombatSkill(
                retainedDefinition.Id);

            secondEngine.ExecuteTurn(
                BuildCombatRuntime(secondPlayer, secondEnemy),
                TurnRequest.SkillAction(
                    secondPlayer,
                    secondEnemy,
                    retainedSkill,
                    new SkillProgress(retainedDefinition)));

            Assert(
                retainedSkill.Applied,
                "Loaded-session workflow should let later engines observe mastered retained progression.");

            var pipelineField = typeof(TurnBasedCombatEngine).GetField(
                "pipeline",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var randomField = typeof(TurnBasedCombatEngine).GetField(
                "random",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);

            Assert(
                firstEngine != secondEngine
                && pipelineField != null
                && !object.ReferenceEquals(
                    pipelineField.GetValue(firstEngine),
                    pipelineField.GetValue(secondEngine))
                && randomField != null
                && object.ReferenceEquals(
                    firstRandom,
                    randomField.GetValue(firstEngine))
                && object.ReferenceEquals(
                    secondRandom,
                    randomField.GetValue(secondEngine)),
                "Loaded-session workflow should create fresh engine dependencies per combat.");

            var thirdRandom = new FixedRandomSource(0.0, 0.99);
            TurnBasedCombatEngine thirdEngine = session.CreateCombatEngine(
                thirdRandom,
                new List<ICombatPassiveEffect>());
            var resonance = new ResonanceProfile(
                new ActiveResonance(6),
                new ResonanceReserve(10));
            var blockedPlayer = BuildCombatEntity(
                "loaded_workflow_blocked_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var blockedEnemy = BuildCombatEntity(
                "loaded_workflow_blocked_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            int startingHealth = blockedEnemy.Stats.CurrentHealth;
            var blockedRuntime = BuildCombatRuntime(
                blockedPlayer,
                blockedEnemy,
                resonance);
            var blockedDefinition = new SkillDefinition(
                "skill_balance_ritual",
                "Loaded Workflow Blocked",
                3,
                new SkillCost(10, 0),
                SkillCategory.Combat,
                SkillTargeting.SingleEnemy,
                SkillEffectType.Damage,
                "",
                2);
            var blockedProgress = new SkillProgress(blockedDefinition);
            var blockedSkill = new TrackingCombatSkill(blockedDefinition.Id);

            thirdEngine.ExecuteTurn(
                blockedRuntime,
                TurnRequest.SkillAction(
                    blockedPlayer,
                    blockedEnemy,
                    blockedSkill,
                    blockedProgress));

            Assert(
                CountEvents(
                    blockedRuntime.EventLog,
                    GameEventType.ActionBlocked) == 1
                && blockedRuntime.TurnNumber == 2,
                "Loaded-session workflow should block a non-mastered cross-class skill with one ActionBlocked and consume the turn.");

            Assert(
                blockedPlayer.Resource.Current == 20
                && resonance.Active.Current == 6
                && resonance.Reserve.Current == 10
                && CountEvents(
                    blockedRuntime.EventLog,
                    GameEventType.ResourceSpent) == 0,
                "Loaded-session workflow blocking should spend neither Essence nor Resonance.");

            Assert(
                blockedProgress.Experience == 0
                && !blockedSkill.Applied
                && blockedEnemy.Stats.CurrentHealth == startingHealth
                && CountEvents(
                    blockedRuntime.EventLog,
                    GameEventType.SkillUsed) == 0,
                "Loaded-session workflow blocking should grant no experience or skill effects.");

            Assert(
                pipelineField != null
                && !object.ReferenceEquals(
                    pipelineField.GetValue(secondEngine),
                    pipelineField.GetValue(thirdEngine))
                && object.ReferenceEquals(
                    runtimeState,
                    session.RuntimeState)
                && object.ReferenceEquals(
                    classRegistry,
                    runtimeState.ClassRegistry)
                && object.ReferenceEquals(
                    skillRegistry,
                    runtimeState.SkillRegistry),
                "Loaded-session workflow should keep canonical registries and runtime state stable across repeated combat creation.");

            Assert(
                FoundationVerticalSlice.Run().Events.Count == 38,
                "Loaded-session workflow should preserve the legacy 38-event slice.");
        }

        private static void AssertPlayerSessionCompositionIntegratedWorkflow()
        {
            var suppliedBindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var composition = new PlayerSessionComposition(
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                suppliedBindingRegistry);
            var secondComposition = BuildTestPlayerSessionComposition();
            var loadedSession = composition.LoadedSession;
            var runtimeState = composition.RuntimeState;
            var classProfile = runtimeState.ClassProfile;
            var masteryProfile = runtimeState.MasteryProfile;
            var classRegistry = runtimeState.ClassRegistry;
            var skillRegistry = runtimeState.SkillRegistry;
            var bindingRegistry = runtimeState.ClassSkillTreeBindingRegistry;
            var skillTree = bindingRegistry.GetSkillTree(SkillTreeId.Tlamatini);

            Assert(
                loadedSession != null
                && object.ReferenceEquals(loadedSession, composition.LoadedSession)
                && object.ReferenceEquals(runtimeState, composition.RuntimeState)
                && object.ReferenceEquals(
                    runtimeState,
                    loadedSession.RuntimeState),
                "Player-session composition workflow should keep one loaded session and one runtime state by reference.");

            Assert(
                object.ReferenceEquals(suppliedBindingRegistry, bindingRegistry)
                && !object.ReferenceEquals(
                    bindingRegistry,
                    secondComposition.RuntimeState.ClassSkillTreeBindingRegistry),
                "Player-session composition workflow should keep mutable bindings session-scoped.");

            var restoreResult = new ProgressRestoreCoordinator().Restore(
                BuildCompleteV4SnapshotWithBinding(
                    ClassId.Tlamatini,
                    SkillTreeId.Tlamatini),
                runtimeState.ClassProfile,
                runtimeState.MasteryProfile,
                runtimeState.MentorRelationshipProfile,
                runtimeState.SkillTreeProgressProfile,
                runtimeState.ClassRegistry,
                runtimeState.SkillRegistry,
                runtimeState.MentorRegistry,
                skillTree,
                runtimeState.ClassSkillTreeBindingRegistry);

            Assert(
                restoreResult.FullyRestored
                && object.ReferenceEquals(classProfile, runtimeState.ClassProfile)
                && object.ReferenceEquals(masteryProfile, runtimeState.MasteryProfile)
                && object.ReferenceEquals(classRegistry, runtimeState.ClassRegistry)
                && object.ReferenceEquals(skillRegistry, runtimeState.SkillRegistry)
                && object.ReferenceEquals(
                    bindingRegistry,
                    runtimeState.ClassSkillTreeBindingRegistry),
                "Player-session composition workflow should restore into the same live runtime-state profiles and registries.");

            var firstRandom = new FixedRandomSource(0.0, 0.99);
            TurnBasedCombatEngine firstEngine = composition.CreateCombatEngine(
                firstRandom,
                new List<ICombatPassiveEffect>());
            var firstPlayer = BuildCombatEntity(
                "session_composition_workflow_first_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var firstEnemy = BuildCombatEntity(
                "session_composition_workflow_first_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var firstDefinition = runtimeState.SkillRegistry.Get(
                "skill_balance_ritual");
            var firstSkill = new TrackingCombatSkill(firstDefinition.Id);
            var firstProgress = new SkillProgress(firstDefinition);

            firstEngine.ExecuteTurn(
                BuildCombatRuntime(firstPlayer, firstEnemy),
                TurnRequest.SkillAction(
                    firstPlayer,
                    firstEnemy,
                    firstSkill,
                    firstProgress));

            Assert(
                firstSkill.Applied
                && firstProgress.Experience == 10,
                "Player-session composition workflow should allow a restored current-class skill through the first combat.");

            runtimeState.MasteryProfile.IncreaseProgress(
                "skill_deep_sight",
                MasteryRecord.MasteredProgress
                    - runtimeState.MasteryProfile.GetRecord(
                        "skill_deep_sight").Progress,
                runtimeState.SkillRegistry);
            runtimeState.ClassProfile.ChangeClassId(
                ClassId.Wanderer,
                runtimeState.ClassRegistry);

            Assert(
                object.ReferenceEquals(runtimeState, composition.RuntimeState)
                && object.ReferenceEquals(classProfile, runtimeState.ClassProfile)
                && object.ReferenceEquals(masteryProfile, runtimeState.MasteryProfile)
                && runtimeState.MasteryProfile.IsMastered("skill_deep_sight")
                && runtimeState.ClassProfile.CurrentClassId == ClassId.Wanderer,
                "Player-session composition workflow progression should mutate the same runtime state before later combats.");

            var secondRandom = new FixedRandomSource(0.0, 0.99);
            TurnBasedCombatEngine secondEngine = composition.CreateCombatEngine(
                secondRandom,
                new List<ICombatPassiveEffect>());
            var secondPlayer = BuildCombatEntity(
                "session_composition_workflow_second_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var secondEnemy = BuildCombatEntity(
                "session_composition_workflow_second_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var retainedDefinition = runtimeState.SkillRegistry.Get(
                "skill_deep_sight");
            var retainedSkill = new TrackingCombatSkill(
                retainedDefinition.Id);

            secondEngine.ExecuteTurn(
                BuildCombatRuntime(secondPlayer, secondEnemy),
                TurnRequest.SkillAction(
                    secondPlayer,
                    secondEnemy,
                    retainedSkill,
                    new SkillProgress(retainedDefinition)));

            Assert(
                retainedSkill.Applied,
                "Player-session composition workflow should let later combats observe mastered retained progression.");

            var pipelineField = typeof(TurnBasedCombatEngine).GetField(
                "pipeline",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var randomField = typeof(TurnBasedCombatEngine).GetField(
                "random",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var validatorField = typeof(TurnBasedCombatEngine).GetField(
                "skillTurnValidator",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);

            Assert(
                firstEngine != secondEngine
                && pipelineField != null
                && !object.ReferenceEquals(
                    pipelineField.GetValue(firstEngine),
                    pipelineField.GetValue(secondEngine))
                && randomField != null
                && object.ReferenceEquals(
                    firstRandom,
                    randomField.GetValue(firstEngine))
                && object.ReferenceEquals(
                    secondRandom,
                    randomField.GetValue(secondEngine))
                && validatorField != null
                && validatorField.GetValue(firstEngine)
                    is CombatSkillTurnValidatorGateway
                && validatorField.GetValue(secondEngine)
                    is CombatSkillTurnValidatorGateway,
                "Player-session composition workflow should create fresh progression-aware combat dependencies per combat.");

            var thirdRandom = new FixedRandomSource(0.0, 0.99);
            TurnBasedCombatEngine thirdEngine = composition.CreateCombatEngine(
                thirdRandom,
                new List<ICombatPassiveEffect>());
            var resonance = new ResonanceProfile(
                new ActiveResonance(6),
                new ResonanceReserve(10));
            var blockedPlayer = BuildCombatEntity(
                "session_composition_workflow_blocked_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var blockedEnemy = BuildCombatEntity(
                "session_composition_workflow_blocked_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            int startingHealth = blockedEnemy.Stats.CurrentHealth;
            var blockedRuntime = BuildCombatRuntime(
                blockedPlayer,
                blockedEnemy,
                resonance);
            var blockedDefinition = new SkillDefinition(
                "skill_balance_ritual",
                "Session Composition Workflow Blocked",
                3,
                new SkillCost(10, 0),
                SkillCategory.Combat,
                SkillTargeting.SingleEnemy,
                SkillEffectType.Damage,
                "",
                2);
            var blockedProgress = new SkillProgress(blockedDefinition);
            var blockedSkill = new TrackingCombatSkill(blockedDefinition.Id);

            thirdEngine.ExecuteTurn(
                blockedRuntime,
                TurnRequest.SkillAction(
                    blockedPlayer,
                    blockedEnemy,
                    blockedSkill,
                    blockedProgress));

            Assert(
                !blockedSkill.Applied
                && CountEvents(
                    blockedRuntime.EventLog,
                    GameEventType.ActionBlocked) == 1
                && blockedRuntime.TurnNumber == 2,
                "Player-session composition workflow should block non-mastered cross-class skills and consume the turn.");

            Assert(
                blockedPlayer.Resource.Current == 20
                && resonance.Active.Current == 6
                && resonance.Reserve.Current == 10
                && CountEvents(
                    blockedRuntime.EventLog,
                    GameEventType.ResourceSpent) == 0,
                "Player-session composition workflow blocking should spend neither Essence nor Resonance.");

            Assert(
                blockedProgress.Experience == 0
                && blockedEnemy.Stats.CurrentHealth == startingHealth
                && CountEvents(
                    blockedRuntime.EventLog,
                    GameEventType.SkillUsed) == 0,
                "Player-session composition workflow blocking should grant no experience or skill effects.");

            Assert(
                pipelineField != null
                && !object.ReferenceEquals(
                    pipelineField.GetValue(secondEngine),
                    pipelineField.GetValue(thirdEngine))
                && randomField != null
                && object.ReferenceEquals(
                    thirdRandom,
                    randomField.GetValue(thirdEngine))
                && object.ReferenceEquals(
                    loadedSession,
                    composition.LoadedSession)
                && object.ReferenceEquals(runtimeState, composition.RuntimeState)
                && object.ReferenceEquals(classRegistry, runtimeState.ClassRegistry)
                && object.ReferenceEquals(skillRegistry, runtimeState.SkillRegistry)
                && object.ReferenceEquals(bindingRegistry, runtimeState.ClassSkillTreeBindingRegistry),
                "Player-session composition workflow should keep canonical registries and session state stable across repeated combat creation.");

            Assert(
                FoundationVerticalSlice.Run().Events.Count == 38,
                "Player-session composition workflow should preserve the legacy 38-event slice.");
        }

        private static void AssertPlayerSessionBootstrapIntegratedWorkflow()
        {
            var suppliedBindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var bootstrap = new PlayerSessionBootstrap(
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                suppliedBindingRegistry);
            var secondBootstrap = BuildTestPlayerSessionBootstrap();
            var composition = bootstrap.Composition;
            var loadedSession = bootstrap.LoadedSession;
            var runtimeState = bootstrap.RuntimeState;
            var classProfile = runtimeState.ClassProfile;
            var masteryProfile = runtimeState.MasteryProfile;
            var classRegistry = runtimeState.ClassRegistry;
            var skillRegistry = runtimeState.SkillRegistry;
            var bindingRegistry = runtimeState.ClassSkillTreeBindingRegistry;
            var skillTree = bindingRegistry.GetSkillTree(SkillTreeId.Tlamatini);

            Assert(
                composition != null
                && object.ReferenceEquals(composition, bootstrap.Composition)
                && object.ReferenceEquals(loadedSession, bootstrap.LoadedSession)
                && object.ReferenceEquals(runtimeState, bootstrap.RuntimeState)
                && object.ReferenceEquals(loadedSession, composition.LoadedSession)
                && object.ReferenceEquals(runtimeState, composition.RuntimeState),
                "Player-session bootstrap workflow should keep one composition, loaded session, and runtime state by reference.");

            Assert(
                object.ReferenceEquals(suppliedBindingRegistry, bindingRegistry)
                && object.ReferenceEquals(
                    bindingRegistry,
                    bootstrap.Composition.ClassSkillTreeBindingRegistry)
                && !object.ReferenceEquals(
                    bindingRegistry,
                    secondBootstrap.RuntimeState.ClassSkillTreeBindingRegistry),
                "Player-session bootstrap workflow should keep mutable bindings session-scoped and separate between bootstraps.");

            var restoreResult = new ProgressRestoreCoordinator().Restore(
                BuildCompleteV4SnapshotWithBinding(
                    ClassId.Tlamatini,
                    SkillTreeId.Tlamatini),
                runtimeState.ClassProfile,
                runtimeState.MasteryProfile,
                runtimeState.MentorRelationshipProfile,
                runtimeState.SkillTreeProgressProfile,
                runtimeState.ClassRegistry,
                runtimeState.SkillRegistry,
                runtimeState.MentorRegistry,
                skillTree,
                runtimeState.ClassSkillTreeBindingRegistry);

            Assert(
                restoreResult.FullyRestored
                && object.ReferenceEquals(classProfile, runtimeState.ClassProfile)
                && object.ReferenceEquals(masteryProfile, runtimeState.MasteryProfile)
                && object.ReferenceEquals(classRegistry, runtimeState.ClassRegistry)
                && object.ReferenceEquals(skillRegistry, runtimeState.SkillRegistry)
                && object.ReferenceEquals(bindingRegistry, runtimeState.ClassSkillTreeBindingRegistry),
                "Player-session bootstrap workflow should restore into the same live runtime-state profiles and registries.");

            var firstRandom = new FixedRandomSource(0.0, 0.99);
            TurnBasedCombatEngine firstEngine = bootstrap.CreateCombatEngine(
                firstRandom,
                new List<ICombatPassiveEffect>());
            var firstPlayer = BuildCombatEntity(
                "session_bootstrap_workflow_first_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var firstEnemy = BuildCombatEntity(
                "session_bootstrap_workflow_first_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var firstDefinition = runtimeState.SkillRegistry.Get(
                "skill_balance_ritual");
            var firstSkill = new TrackingCombatSkill(firstDefinition.Id);
            var firstProgress = new SkillProgress(firstDefinition);

            firstEngine.ExecuteTurn(
                BuildCombatRuntime(firstPlayer, firstEnemy),
                TurnRequest.SkillAction(
                    firstPlayer,
                    firstEnemy,
                    firstSkill,
                    firstProgress));

            Assert(
                firstSkill.Applied
                && firstProgress.Experience == 10,
                "Player-session bootstrap workflow should allow a restored current-class skill through the first combat.");

            runtimeState.MasteryProfile.IncreaseProgress(
                "skill_deep_sight",
                MasteryRecord.MasteredProgress
                    - runtimeState.MasteryProfile.GetRecord(
                        "skill_deep_sight").Progress,
                runtimeState.SkillRegistry);
            runtimeState.ClassProfile.ChangeClassId(
                ClassId.Wanderer,
                runtimeState.ClassRegistry);

            Assert(
                object.ReferenceEquals(runtimeState, bootstrap.RuntimeState)
                && object.ReferenceEquals(classProfile, runtimeState.ClassProfile)
                && object.ReferenceEquals(masteryProfile, runtimeState.MasteryProfile)
                && runtimeState.MasteryProfile.IsMastered("skill_deep_sight")
                && runtimeState.ClassProfile.CurrentClassId == ClassId.Wanderer,
                "Player-session bootstrap workflow progression should mutate the same runtime state before later combats.");

            var secondRandom = new FixedRandomSource(0.0, 0.99);
            TurnBasedCombatEngine secondEngine = bootstrap.CreateCombatEngine(
                secondRandom,
                new List<ICombatPassiveEffect>());
            var secondPlayer = BuildCombatEntity(
                "session_bootstrap_workflow_second_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var secondEnemy = BuildCombatEntity(
                "session_bootstrap_workflow_second_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var retainedDefinition = runtimeState.SkillRegistry.Get(
                "skill_deep_sight");
            var retainedSkill = new TrackingCombatSkill(
                retainedDefinition.Id);

            secondEngine.ExecuteTurn(
                BuildCombatRuntime(secondPlayer, secondEnemy),
                TurnRequest.SkillAction(
                    secondPlayer,
                    secondEnemy,
                    retainedSkill,
                    new SkillProgress(retainedDefinition)));

            Assert(
                retainedSkill.Applied,
                "Player-session bootstrap workflow should let later combats observe mastered retained progression.");

            var pipelineField = typeof(TurnBasedCombatEngine).GetField(
                "pipeline",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var randomField = typeof(TurnBasedCombatEngine).GetField(
                "random",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var validatorField = typeof(TurnBasedCombatEngine).GetField(
                "skillTurnValidator",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);

            Assert(
                firstEngine != secondEngine
                && pipelineField != null
                && !object.ReferenceEquals(
                    pipelineField.GetValue(firstEngine),
                    pipelineField.GetValue(secondEngine))
                && randomField != null
                && object.ReferenceEquals(
                    firstRandom,
                    randomField.GetValue(firstEngine))
                && object.ReferenceEquals(
                    secondRandom,
                    randomField.GetValue(secondEngine))
                && validatorField != null
                && validatorField.GetValue(firstEngine)
                    is CombatSkillTurnValidatorGateway
                && validatorField.GetValue(secondEngine)
                    is CombatSkillTurnValidatorGateway,
                "Player-session bootstrap workflow should create fresh progression-aware combat dependencies per combat.");

            var thirdRandom = new FixedRandomSource(0.0, 0.99);
            TurnBasedCombatEngine thirdEngine = bootstrap.CreateCombatEngine(
                thirdRandom,
                new List<ICombatPassiveEffect>());
            var resonance = new ResonanceProfile(
                new ActiveResonance(6),
                new ResonanceReserve(10));
            var blockedPlayer = BuildCombatEntity(
                "session_bootstrap_workflow_blocked_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var blockedEnemy = BuildCombatEntity(
                "session_bootstrap_workflow_blocked_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            int startingHealth = blockedEnemy.Stats.CurrentHealth;
            var blockedRuntime = BuildCombatRuntime(
                blockedPlayer,
                blockedEnemy,
                resonance);
            var blockedDefinition = new SkillDefinition(
                "skill_balance_ritual",
                "Session Bootstrap Workflow Blocked",
                3,
                new SkillCost(10, 0),
                SkillCategory.Combat,
                SkillTargeting.SingleEnemy,
                SkillEffectType.Damage,
                "",
                2);
            var blockedProgress = new SkillProgress(blockedDefinition);
            var blockedSkill = new TrackingCombatSkill(blockedDefinition.Id);

            thirdEngine.ExecuteTurn(
                blockedRuntime,
                TurnRequest.SkillAction(
                    blockedPlayer,
                    blockedEnemy,
                    blockedSkill,
                    blockedProgress));

            Assert(
                !blockedSkill.Applied
                && CountEvents(
                    blockedRuntime.EventLog,
                    GameEventType.ActionBlocked) == 1
                && blockedRuntime.TurnNumber == 2,
                "Player-session bootstrap workflow should block non-mastered cross-class skills and consume the turn.");

            Assert(
                blockedPlayer.Resource.Current == 20
                && resonance.Active.Current == 6
                && resonance.Reserve.Current == 10
                && CountEvents(
                    blockedRuntime.EventLog,
                    GameEventType.ResourceSpent) == 0,
                "Player-session bootstrap workflow blocking should spend neither Essence nor Resonance.");

            Assert(
                blockedProgress.Experience == 0
                && blockedEnemy.Stats.CurrentHealth == startingHealth
                && CountEvents(
                    blockedRuntime.EventLog,
                    GameEventType.SkillUsed) == 0,
                "Player-session bootstrap workflow blocking should grant no experience or skill effects.");

            Assert(
                pipelineField != null
                && !object.ReferenceEquals(
                    pipelineField.GetValue(secondEngine),
                    pipelineField.GetValue(thirdEngine))
                && randomField != null
                && object.ReferenceEquals(
                    thirdRandom,
                    randomField.GetValue(thirdEngine))
                && object.ReferenceEquals(composition, bootstrap.Composition)
                && object.ReferenceEquals(loadedSession, bootstrap.LoadedSession)
                && object.ReferenceEquals(runtimeState, bootstrap.RuntimeState)
                && object.ReferenceEquals(classRegistry, runtimeState.ClassRegistry)
                && object.ReferenceEquals(skillRegistry, runtimeState.SkillRegistry)
                && object.ReferenceEquals(bindingRegistry, runtimeState.ClassSkillTreeBindingRegistry),
                "Player-session bootstrap workflow should keep canonical registries and session state stable across repeated combat creation.");

            Assert(
                FoundationVerticalSlice.Run().Events.Count == 38,
                "Player-session bootstrap workflow should preserve the legacy 38-event slice.");
        }

        private static void AssertProgressionRuntimeDemoBootstrapIntegratedWorkflow()
        {
            var suppliedBindingRegistry = BuildTestClassSkillTreeBindingRegistry();
            var demo = new ProgressionRuntimeDemoBootstrap(
                BuildTestClassRegistry(),
                BuildTestSkillRegistry(),
                BuildTestMentorRegistry(),
                suppliedBindingRegistry);
            var secondDemo = BuildTestProgressionRuntimeDemoBootstrap();
            var sessionBootstrap = demo.SessionBootstrap;
            var composition = demo.Composition;
            var loadedSession = demo.LoadedSession;
            var runtimeState = demo.RuntimeState;
            var classProfile = runtimeState.ClassProfile;
            var masteryProfile = runtimeState.MasteryProfile;
            var classRegistry = runtimeState.ClassRegistry;
            var skillRegistry = runtimeState.SkillRegistry;
            var bindingRegistry = runtimeState.ClassSkillTreeBindingRegistry;
            var skillTree = bindingRegistry.GetSkillTree(SkillTreeId.Tlamatini);

            Assert(
                sessionBootstrap != null
                && object.ReferenceEquals(sessionBootstrap, demo.SessionBootstrap)
                && object.ReferenceEquals(composition, demo.Composition)
                && object.ReferenceEquals(loadedSession, demo.LoadedSession)
                && object.ReferenceEquals(runtimeState, demo.RuntimeState)
                && object.ReferenceEquals(composition, sessionBootstrap.Composition)
                && object.ReferenceEquals(loadedSession, sessionBootstrap.LoadedSession)
                && object.ReferenceEquals(runtimeState, sessionBootstrap.RuntimeState),
                "V5 runtime demo workflow should keep one session bootstrap, composition, loaded session, and runtime state by reference.");

            Assert(
                object.ReferenceEquals(suppliedBindingRegistry, bindingRegistry)
                && object.ReferenceEquals(
                    bindingRegistry,
                    demo.Composition.ClassSkillTreeBindingRegistry)
                && !object.ReferenceEquals(
                    bindingRegistry,
                    secondDemo.RuntimeState.ClassSkillTreeBindingRegistry),
                "V5 runtime demo workflow should keep mutable bindings session-scoped and separate between demo bootstraps.");

            var restoreResult = new ProgressRestoreCoordinator().Restore(
                BuildCompleteV4SnapshotWithBinding(
                    ClassId.Tlamatini,
                    SkillTreeId.Tlamatini),
                runtimeState.ClassProfile,
                runtimeState.MasteryProfile,
                runtimeState.MentorRelationshipProfile,
                runtimeState.SkillTreeProgressProfile,
                runtimeState.ClassRegistry,
                runtimeState.SkillRegistry,
                runtimeState.MentorRegistry,
                skillTree,
                runtimeState.ClassSkillTreeBindingRegistry);

            Assert(
                restoreResult.FullyRestored
                && object.ReferenceEquals(classProfile, runtimeState.ClassProfile)
                && object.ReferenceEquals(masteryProfile, runtimeState.MasteryProfile)
                && object.ReferenceEquals(classRegistry, runtimeState.ClassRegistry)
                && object.ReferenceEquals(skillRegistry, runtimeState.SkillRegistry)
                && object.ReferenceEquals(bindingRegistry, runtimeState.ClassSkillTreeBindingRegistry),
                "V5 runtime demo workflow should restore into the same live runtime-state profiles and registries.");

            var firstRandom = new FixedRandomSource(0.0, 0.99);
            TurnBasedCombatEngine firstEngine = demo.CreateCombatEngine(
                firstRandom,
                new List<ICombatPassiveEffect>());
            var firstPlayer = BuildCombatEntity(
                "v5_demo_workflow_first_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var firstEnemy = BuildCombatEntity(
                "v5_demo_workflow_first_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var firstDefinition = runtimeState.SkillRegistry.Get(
                "skill_balance_ritual");
            var firstSkill = new TrackingCombatSkill(firstDefinition.Id);
            var firstProgress = new SkillProgress(firstDefinition);

            firstEngine.ExecuteTurn(
                BuildCombatRuntime(firstPlayer, firstEnemy),
                TurnRequest.SkillAction(
                    firstPlayer,
                    firstEnemy,
                    firstSkill,
                    firstProgress));

            Assert(
                firstSkill.Applied
                && firstProgress.Experience == 10,
                "V5 runtime demo workflow should allow a restored current-class skill through the first combat.");

            runtimeState.MasteryProfile.IncreaseProgress(
                "skill_deep_sight",
                MasteryRecord.MasteredProgress
                    - runtimeState.MasteryProfile.GetRecord(
                        "skill_deep_sight").Progress,
                runtimeState.SkillRegistry);
            runtimeState.ClassProfile.ChangeClassId(
                ClassId.Wanderer,
                runtimeState.ClassRegistry);

            Assert(
                object.ReferenceEquals(runtimeState, demo.RuntimeState)
                && object.ReferenceEquals(classProfile, runtimeState.ClassProfile)
                && object.ReferenceEquals(masteryProfile, runtimeState.MasteryProfile)
                && runtimeState.MasteryProfile.IsMastered("skill_deep_sight")
                && runtimeState.ClassProfile.CurrentClassId == ClassId.Wanderer,
                "V5 runtime demo workflow progression should mutate the same runtime state before later combats.");

            var secondRandom = new FixedRandomSource(0.0, 0.99);
            TurnBasedCombatEngine secondEngine = demo.CreateCombatEngine(
                secondRandom,
                new List<ICombatPassiveEffect>());
            var secondPlayer = BuildCombatEntity(
                "v5_demo_workflow_second_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var secondEnemy = BuildCombatEntity(
                "v5_demo_workflow_second_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            var retainedDefinition = runtimeState.SkillRegistry.Get(
                "skill_deep_sight");
            var retainedSkill = new TrackingCombatSkill(
                retainedDefinition.Id);

            secondEngine.ExecuteTurn(
                BuildCombatRuntime(secondPlayer, secondEnemy),
                TurnRequest.SkillAction(
                    secondPlayer,
                    secondEnemy,
                    retainedSkill,
                    new SkillProgress(retainedDefinition)));

            Assert(
                retainedSkill.Applied,
                "V5 runtime demo workflow should let later combats observe mastered retained progression.");

            var pipelineField = typeof(TurnBasedCombatEngine).GetField(
                "pipeline",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var randomField = typeof(TurnBasedCombatEngine).GetField(
                "random",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            var validatorField = typeof(TurnBasedCombatEngine).GetField(
                "skillTurnValidator",
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic);
            bool exposesTurnBasedCombatEngine = false;
            foreach (var field in typeof(ProgressionRuntimeDemoBootstrap).GetFields(
                System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Public))
            {
                if (field.FieldType == typeof(TurnBasedCombatEngine))
                {
                    exposesTurnBasedCombatEngine = true;
                }
            }

            Assert(
                !exposesTurnBasedCombatEngine
                && firstEngine != secondEngine
                && pipelineField != null
                && !object.ReferenceEquals(
                    pipelineField.GetValue(firstEngine),
                    pipelineField.GetValue(secondEngine))
                && randomField != null
                && object.ReferenceEquals(
                    firstRandom,
                    randomField.GetValue(firstEngine))
                && object.ReferenceEquals(
                    secondRandom,
                    randomField.GetValue(secondEngine))
                && validatorField != null
                && validatorField.GetValue(firstEngine)
                    is CombatSkillTurnValidatorGateway
                && validatorField.GetValue(secondEngine)
                    is CombatSkillTurnValidatorGateway,
                "V5 runtime demo workflow should create fresh progression-aware combat dependencies without exposing direct engine construction.");

            var thirdRandom = new FixedRandomSource(0.0, 0.99);
            TurnBasedCombatEngine thirdEngine = demo.CreateCombatEngine(
                thirdRandom,
                new List<ICombatPassiveEffect>());
            var resonance = new ResonanceProfile(
                new ActiveResonance(6),
                new ResonanceReserve(10));
            var blockedPlayer = BuildCombatEntity(
                "v5_demo_workflow_blocked_player",
                10,
                5,
                100,
                0,
                0,
                0,
                2f,
                20);
            var blockedEnemy = BuildCombatEntity(
                "v5_demo_workflow_blocked_enemy",
                5,
                5,
                100,
                0,
                0,
                0,
                1f,
                10);
            int startingHealth = blockedEnemy.Stats.CurrentHealth;
            var blockedRuntime = BuildCombatRuntime(
                blockedPlayer,
                blockedEnemy,
                resonance);
            var blockedDefinition = new SkillDefinition(
                "skill_balance_ritual",
                "V5 Demo Workflow Blocked",
                3,
                new SkillCost(10, 0),
                SkillCategory.Combat,
                SkillTargeting.SingleEnemy,
                SkillEffectType.Damage,
                "",
                2);
            var blockedProgress = new SkillProgress(blockedDefinition);
            var blockedSkill = new TrackingCombatSkill(blockedDefinition.Id);

            thirdEngine.ExecuteTurn(
                blockedRuntime,
                TurnRequest.SkillAction(
                    blockedPlayer,
                    blockedEnemy,
                    blockedSkill,
                    blockedProgress));

            Assert(
                !blockedSkill.Applied
                && CountEvents(
                    blockedRuntime.EventLog,
                    GameEventType.ActionBlocked) == 1
                && blockedRuntime.TurnNumber == 2,
                "V5 runtime demo workflow should block non-mastered cross-class skills and consume the turn.");

            Assert(
                blockedPlayer.Resource.Current == 20
                && resonance.Active.Current == 6
                && resonance.Reserve.Current == 10
                && CountEvents(
                    blockedRuntime.EventLog,
                    GameEventType.ResourceSpent) == 0,
                "V5 runtime demo workflow blocking should spend neither Essence nor Resonance.");

            Assert(
                blockedProgress.Experience == 0
                && blockedEnemy.Stats.CurrentHealth == startingHealth
                && CountEvents(
                    blockedRuntime.EventLog,
                    GameEventType.SkillUsed) == 0,
                "V5 runtime demo workflow blocking should grant no experience or skill effects.");

            Assert(
                pipelineField != null
                && !object.ReferenceEquals(
                    pipelineField.GetValue(secondEngine),
                    pipelineField.GetValue(thirdEngine))
                && randomField != null
                && object.ReferenceEquals(
                    thirdRandom,
                    randomField.GetValue(thirdEngine))
                && object.ReferenceEquals(sessionBootstrap, demo.SessionBootstrap)
                && object.ReferenceEquals(composition, demo.Composition)
                && object.ReferenceEquals(loadedSession, demo.LoadedSession)
                && object.ReferenceEquals(runtimeState, demo.RuntimeState)
                && object.ReferenceEquals(classRegistry, runtimeState.ClassRegistry)
                && object.ReferenceEquals(skillRegistry, runtimeState.SkillRegistry)
                && object.ReferenceEquals(bindingRegistry, runtimeState.ClassSkillTreeBindingRegistry),
                "V5 runtime demo workflow should keep canonical registries and session state stable across repeated combat creation.");

            Assert(
                FoundationVerticalSlice.Run().Events.Count == 38,
                "V5 runtime demo workflow should preserve the legacy 38-event slice.");
        }
    }
}