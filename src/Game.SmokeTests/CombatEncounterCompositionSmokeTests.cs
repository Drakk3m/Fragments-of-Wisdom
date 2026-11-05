using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Game.Application.Runtime;
using Game.Domain.Classes;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.Passives;
using Game.Domain.Combat.Resolvers;
using Game.Domain.Combat.Resources;
using Game.Domain.Player;
using Game.Domain.Skills;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunCombatEncounterCompositionSmokeTests()
        {
            AssertEncounterOwnershipAndStart();
            AssertIndependentEncountersAndIntegratedAccess();
            AssertEncounterInvalidInputs();
            AssertEncounterBindingContracts();
            AssertEncounterStartFailureAndReentry();
            AssertEncounterPreparationChangesBeforeStart();
            AssertEncounterPublicBoundary();
        }

        private static void AssertEncounterOwnershipAndStart()
        {
            var f = new EncounterFixture();
            var before = CaptureEncounterFixture(f);
            var preparation = f.Prepare();
            var owner = new CombatEncounterComposition(f.Session, f.Profile, preparation);
            var runtime = owner.Runtime;
            var engine = owner.Engine;
            var state = f.Session.RuntimeState;

            Assert(ReferenceEquals(owner.Session, f.Session), "P9.1 retains the exact loaded session.");
            Assert(ReferenceEquals(owner.Progression, state), "P9.1 retains the session progression runtime.");
            Assert(ReferenceEquals(owner.PlayerProfile, f.Profile), "P9.1 retains the complementary profile.");
            Assert(ReferenceEquals(runtime.Player, f.Player), "P9.1 retains the explicit player actor.");
            Assert(ReferenceEquals(runtime.Enemy, f.Enemy), "P9.1 retains the explicit enemy actor.");
            Assert(ReferenceEquals(runtime.PlayerProfile, f.Profile), "P9.1 runtime uses the retained profile.");
            Assert(ReferenceEquals(runtime.EventLog, f.Log), "P9.1 runtime uses the prepared log.");
            Assert(ReferenceEquals(runtime.ResonanceProfile, f.Resonance), "P9.1 runtime uses the player-owned pool.");
            var classifier = new CombatActorValidationClassifier();
            Assert(classifier.Classify(f.Player, runtime, false, state.UnlockProfile, state.ClassProfile,
                state.MasteryProfile, state.ClassRegistry, state.SkillRegistry).Kind == CombatActorValidationKind.Player,
                "P9.1 explicit player slot is recognized by the existing classifier.");
            Assert(classifier.Classify(f.Enemy, runtime, false, null, null, null, null, null).Kind
                == CombatActorValidationKind.Enemy,
                "P9.1 explicit enemy slot does not require player progression.");
            Assert(ReferenceEquals(preparation.Random, f.Random), "P9.1 retains prepared RNG.");
            Assert(ReferenceEquals(preparation.Passives[0], f.Passive), "P9.1 retains passive instances.");
            Assert(ReferenceEquals(owner.GetSkillBinding(f.Skill.Id), f.Binding), "P9.1 retains trusted binding.");
            Assert(ReferenceEquals(owner.GetSkillBinding(f.Skill.Id).Progress, f.Progress), "P9.1 retains skill progress.");
            Assert(ReferenceEquals(owner.GetSkillBinding(f.Skill.Id).Skill, f.Skill), "P9.1 retains executable skill.");
            Assert(owner.State == CombatEncounterState.Prepared && owner.StartFailure == null,
                "P9.1 construction prepares but never starts.");
            Assert(CaptureEncounterFixture(f) == before, "P9.1 preparation neither mutates state nor runs hooks.");
            Assert(owner.GetSkillBinding(null) == null && owner.GetSkillBinding(" ") == null
                && owner.GetSkillBinding("unknown") == null, "P9.1 unknown binding lookup is safe.");

            // Mutating caller collections must not change the captured prepared content.
            f.Passives.Clear();
            f.Bindings.Clear();
            Assert(preparation.Passives.Count == 1 && preparation.Skills.Count == 1,
                "P9.1 snapshots collection membership.");
            Assert(owner.Start(), "P9.1 first start succeeds.");
            Assert(owner.State == CombatEncounterState.Started, "P9.1 owner reports started.");
            Assert(f.Passive.Starts == 1 && f.Passive.Costs == 0 && f.Passive.Debuffs == 0,
                "P9.1 only the start hook runs.");
            Assert(f.Log.Events.Count == 1 && f.Log.Events[0].Type == GameEventType.CombatStarted,
                "P9.1 one authoritative combat-start event is emitted.");
            Assert(f.Random.Calls == 0 && f.Progress.Experience == 0,
                "P9.1 composition does not roll RNG or award skill XP.");
            Assert(runtime.TurnNumber == 1 && !runtime.CombatEnded, "P9.1 starting does not advance a turn.");

            var afterStart = CaptureEncounterFixture(f);
            for (int i = 0; i < 3; i++)
            {
                Assert(!owner.Start(), "P9.1 repeated start is a no-op.");
                Assert(owner.State == CombatEncounterState.Started
                    && ReferenceEquals(owner.Runtime, runtime) && ReferenceEquals(owner.Engine, engine),
                    "P9.1 reading and repeated start never replace encounter objects.");
                Assert(CaptureEncounterFixture(f) == afterStart, "P9.1 repeat cannot duplicate hooks or events.");
            }

            var claimedFixture = new EncounterFixture(f.Session, f.Profile);
            var claimedPreparation = claimedFixture.Prepare();
            var originalOwner = new CombatEncounterComposition(f.Session, f.Profile, claimedPreparation);
            var beforeClaimRetry = CaptureEncounterFixture(claimedFixture);
            ExpectEncounterException<InvalidOperationException>(
                () => new CombatEncounterComposition(f.Session, f.Profile, claimedPreparation),
                "P9.1 a preparation cannot be owned twice.");
            Assert(originalOwner.State == CombatEncounterState.Prepared
                && CaptureEncounterFixture(claimedFixture) == beforeClaimRetry,
                "P9.1 failed ownership reuse does not disturb the first prepared owner.");
            runtime.EndCombat();
            Assert(owner.State == CombatEncounterState.Terminal && !owner.Start(),
                "P9.1 terminal runtime cannot be restarted.");

            var noPool = new EncounterFixture();
            var basicOnly = new CombatEncounterComposition(noPool.Session, noPool.Profile,
                new CombatEncounterPreparation(noPool.Player, noPool.Enemy, noPool.Log, noPool.Random,
                    Array.Empty<ICombatPassiveEffect>(), Array.Empty<PreparedCombatSkillBinding>()));
            Assert(basicOnly.Runtime.ResonanceProfile == null && basicOnly.Start(),
                "P9.1 existing absent-pool and basic-only preparation remain supported.");
            basicOnly.Runtime.Enemy.ReceiveDamage(1000);
            Assert(basicOnly.State == CombatEncounterState.Terminal && !basicOnly.Runtime.CombatEnded,
                "P9.1 terminal observation includes dead actors without rewriting domain flags.");
        }

        private static void AssertIndependentEncountersAndIntegratedAccess()
        {
            var first = new EncounterFixture();
            var state = first.Session.RuntimeState;
            state.MasteryProfile.IncreaseProgress(first.Skill.Id, 40, state.SkillRegistry);
            first.Profile.Wallet.AddCurrency("existing", 17);
            first.Profile.Inventory.AddItem("existing", 3);
            var second = new EncounterFixture(first.Session, first.Profile);
            var firstOwner = new CombatEncounterComposition(first.Session, first.Profile, first.Prepare());
            var secondOwner = new CombatEncounterComposition(second.Session, second.Profile, second.Prepare());

            Assert(ReferenceEquals(firstOwner.Session, secondOwner.Session)
                && ReferenceEquals(firstOwner.Progression, secondOwner.Progression),
                "P9.1 two encounters reuse one session, never reconstruct progression.");
            Assert(!ReferenceEquals(firstOwner.Engine, secondOwner.Engine), "P9.1 engines are per encounter.");
            Assert(!ReferenceEquals(firstOwner.Runtime, secondOwner.Runtime), "P9.1 runtimes are per encounter.");
            Assert(!ReferenceEquals(first.Player, second.Player) && !ReferenceEquals(first.Enemy, second.Enemy),
                "P9.1 prepared actors are independent.");
            Assert(!ReferenceEquals(first.Log, second.Log) && !ReferenceEquals(first.Random, second.Random),
                "P9.1 logs and RNG are independent.");
            Assert(!ReferenceEquals(first.Passive, second.Passive) && !ReferenceEquals(first.Resonance, second.Resonance),
                "P9.1 mutable passives and pools are independent.");
            Assert(!ReferenceEquals(first.Progress, second.Progress), "P9.1 no implicit cross-encounter skill XP pool.");
            Assert(ReferenceEquals(first.Progress.Definition, second.Progress.Definition),
                "P9.1 canonical immutable metadata can be shared.");
            Assert(state.MasteryProfile.GetRecord(first.Skill.Id).Progress == 40
                && state.ClassProfile.CurrentClassId == ClassId.Tlamatini,
                "P9.1 new encounters do not reset session state.");
            firstOwner.Start();
            var secondBefore = CaptureEncounterFixture(second);

            var binding = firstOwner.GetSkillBinding(first.Skill.Id);
            var request = TurnRequest.SkillAction(first.Player, first.Enemy, binding.Skill, binding.Progress);
            firstOwner.Engine.ExecuteTurn(firstOwner.Runtime, request);
            Assert(first.Progress.Experience == 10 && first.Enemy.Stats.CurrentHealth < 100,
                "P9.1 session-created engine executes the real class-allowed skill.");
            Assert(first.Resonance.Active.Current == 18 && first.Passive.Costs == 1 && first.Random.Calls > 0,
                "P9.1 execution uses the supplied pool, passive and deterministic RNG.");
            firstOwner.Engine.ExecuteTurn(firstOwner.Runtime, request);
            Assert(first.Progress.Experience == 20 && ReferenceEquals(first.Progress, binding.Progress),
                "P9.1 XP accumulates on one retained instance across existing engine calls.");
            Assert(CaptureEncounterFixture(second) == secondBefore,
                "P9.1 first encounter execution leaves second encounter and session progression untouched.");

            state.ClassProfile.ChangeClassId(ClassId.Wanderer, state.ClassRegistry);
            int hpBefore = first.Enemy.Stats.CurrentHealth;
            int resonanceBefore = first.Resonance.Active.Current;
            int costsBefore = first.Passive.Costs;
            firstOwner.Engine.ExecuteTurn(firstOwner.Runtime, request);
            Assert(first.Progress.Experience == 20 && first.Enemy.Stats.CurrentHealth == hpBefore
                && first.Resonance.Active.Current == resonanceBefore && first.Passive.Costs == costsBefore,
                "P9.1 existing factory gateway blocks cross-class skill before skill mutation.");
            Assert(first.Log.Events.Any(e => e.Type == GameEventType.ActionBlocked)
                && firstOwner.Runtime.TurnNumber == 4,
                "P9.1 blocked skill retains canonical consumed-turn behavior.");

            state.MasteryProfile.IncreaseProgress(first.Skill.Id, 60, state.SkillRegistry);
            firstOwner.Engine.ExecuteTurn(firstOwner.Runtime, request);
            Assert(first.Progress.Experience == 30 && first.Enemy.Stats.CurrentHealth < hpBefore,
                "P9.1 engine sees mastery on the original session, with no copied validation context.");
            Assert(secondOwner.Start() && second.Progress.Experience == 0 && second.Passive.Starts == 1,
                "P9.1 second encounter starts independently without inheriting XP.");
            Assert(state.MasteryProfile.IsMastered(first.Skill.Id) && state.ClassProfile.CurrentClassId == ClassId.Wanderer,
                "P9.1 starting second encounter preserves changed session progression.");
            Assert(first.Profile.Wallet.Currencies.Count == 1 && first.Profile.Wallet.Currencies["existing"] == 17
                && first.Profile.Inventory.Items.Count == 1 && first.Profile.Inventory.Items["existing"] == 3,
                "P9.1 encounter lifetime performs no automatic rewards.");
            Assert(!first.Profile.SkillBook.HasSkill(first.Skill.Id) && state.UnlockProfile.Unlocks.Count == 0,
                "P9.1 binding and execution do not grant unlocks or synchronize the legacy skill book.");
        }

        private static void AssertEncounterInvalidInputs()
        {
            var f = new EncounterFixture();
            var before = CaptureEncounterFixture(f);
            Action[] invalid =
            {
                () => new CombatEncounterComposition(null, f.Profile, f.Prepare()),
                () => new CombatEncounterComposition(f.Session, null, f.Prepare()),
                () => new CombatEncounterComposition(f.Session, f.Profile, null),
                () => new CombatEncounterPreparation(null, f.Enemy, f.Log, f.Random, f.Passives, f.Bindings),
                () => new CombatEncounterPreparation(f.Player, null, f.Log, f.Random, f.Passives, f.Bindings),
                () => new CombatEncounterPreparation(f.Player, f.Enemy, null, f.Random, f.Passives, f.Bindings),
                () => new CombatEncounterPreparation(f.Player, f.Enemy, f.Log, null, f.Passives, f.Bindings),
                () => new CombatEncounterPreparation(f.Player, f.Enemy, f.Log, f.Random, null, f.Bindings),
                () => new CombatEncounterPreparation(f.Player, f.Enemy, f.Log, f.Random, f.Passives, null),
                () => new CombatEncounterPreparation(f.Player, f.Player, f.Log, f.Random, f.Passives, f.Bindings),
                () => new CombatEncounterPreparation(f.Player, EncounterActor(f.Player.Id), f.Log, f.Random, f.Passives, f.Bindings),
                () => new CombatEncounterPreparation(EncounterActor(" "), f.Enemy, f.Log, f.Random, f.Passives, f.Bindings),
                () => new CombatEncounterPreparation(f.Player, EncounterActor(null), f.Log, f.Random, f.Passives, f.Bindings),
                () => new CombatEncounterPreparation(f.Player, new CombatEntity("other", "Other", f.Player.Stats, 20),
                    f.Log, f.Random, f.Passives, f.Bindings),
                () => new CombatEncounterPreparation(f.Player, f.Enemy, f.Log, f.Random,
                    new ICombatPassiveEffect[] { null }, f.Bindings),
                () => new CombatEncounterPreparation(f.Player, f.Enemy, f.Log, f.Random,
                    f.Passives, new PreparedCombatSkillBinding[] { null }),
                () => new CombatEncounterPreparation(f.Player, f.Enemy, f.Log, f.Random,
                    f.Passives, new[] { f.Binding, f.Binding }),
                () => new CombatEncounterPreparation(f.Player, f.Enemy, f.Log, f.Random,
                    f.Passives, new[] { f.Binding, new PreparedCombatSkillBinding(new BalanceRitualSkill(),
                        new SkillProgress(f.Progress.Definition)) }),
                () => new LoadedPlayerSession(null, f.Session.RuntimeState.SkillRegistry,
                    new MentorDefinitionRegistry(), new ClassSkillTreeBindingRegistry()),
                () => new LoadedPlayerSession(new ClassDefinitionRegistry(), null,
                    new MentorDefinitionRegistry(), new ClassSkillTreeBindingRegistry()),
                () => new LoadedPlayerSession(new ClassDefinitionRegistry(), f.Session.RuntimeState.SkillRegistry,
                    null, new ClassSkillTreeBindingRegistry()),
                () => new LoadedPlayerSession(new ClassDefinitionRegistry(), f.Session.RuntimeState.SkillRegistry,
                    new MentorDefinitionRegistry(), null)
            };
            foreach (var action in invalid)
            {
                ExpectEncounterException<ArgumentException>(action, "P9.1 malformed graph is rejected.");
                Assert(CaptureEncounterFixture(f) == before,
                    "P9.1 rejection precedes start hooks, events, RNG and state mutation.");
            }

            foreach (bool playerDead in new[] { true, false })
            {
                var dead = new EncounterFixture();
                (playerDead ? dead.Player : dead.Enemy).ReceiveDamage(1000);
                ExpectEncounterException<ArgumentException>(() => dead.Prepare(), "P9.1 dead combatant is rejected.");
                Assert(dead.Log.Events.Count == 0 && dead.Passive.Starts == 0, "P9.1 dead preparation does not start.");
            }

            f.Log.Add(GameEventType.CombatStarted, "preexisting encounter");
            ExpectEncounterException<ArgumentException>(() => f.Prepare(), "P9.1 reused log is rejected.");
            Assert(f.Log.Events.Count == 1 && f.Passive.Starts == 0, "P9.1 does not clear or append to reused log.");
        }

        private static void AssertEncounterBindingContracts()
        {
            var f = new EncounterFixture();
            var before = CaptureEncounterFixture(f);
            Action[] invalidBindings =
            {
                () => new PreparedCombatSkillBinding(null, f.Progress),
                () => new PreparedCombatSkillBinding(f.Skill, null),
                () => new PreparedCombatSkillBinding(f.Skill, new SkillProgress(null)),
                () => new PreparedCombatSkillBinding(new EncounterMutableSkill(" "), f.Progress),
                () => new PreparedCombatSkillBinding(new DeepSightSkill(), f.Progress),
                () => EncounterWithBinding(f, new PreparedCombatSkillBinding(new DeepSightSkill(),
                    new SkillProgress(new SkillDefinition("skill_deep_sight", "Deep", 3, new SkillCost(7, 0))))),
                () => EncounterWithBinding(f, new PreparedCombatSkillBinding(f.Skill,
                    new SkillProgress(EncounterDefinition(1)))),
                () => EncounterWithBinding(f, new PreparedCombatSkillBinding(f.Skill,
                    new SkillProgress(new SkillDefinition(f.Skill.Id, "Different name", 3, new SkillCost(7, 0))))),
                () => EncounterWithBinding(f, new PreparedCombatSkillBinding(f.Skill,
                    new SkillProgress(new SkillDefinition(f.Skill.Id, "Balance", 3, null))))
            };
            foreach (var action in invalidBindings)
            {
                ExpectEncounterException<ArgumentException>(action, "P9.1 null/unknown/mismatched binding is rejected.");
                Assert(CaptureEncounterFixture(f) == before, "P9.1 binding validation is nonmutating.");
            }

            var copyProgress = new SkillProgress(EncounterDefinition());
            var copyOwner = EncounterWithBinding(f, new PreparedCombatSkillBinding(f.Skill, copyProgress));
            Assert(!ReferenceEquals(copyProgress.Definition, f.Progress.Definition)
                && ReferenceEquals(copyOwner.GetSkillBinding(f.Skill.Id).Progress, copyProgress),
                "P9.1 preserves AUD-04 equivalent-value metadata and the caller's progress identity.");
            Assert(copyOwner.Start(), "P9.1 equivalent metadata can start.");

            var foreign = new EncounterFixture();
            var foreignRegistry = new SkillDefinitionRegistry();
            foreignRegistry.Register(EncounterDefinition(1));
            var foreignSession = new LoadedPlayerSession(new ClassDefinitionRegistry(), foreignRegistry,
                new MentorDefinitionRegistry(), new ClassSkillTreeBindingRegistry());
            ExpectEncounterException<ArgumentException>(
                () => new CombatEncounterComposition(foreignSession, foreign.Profile, foreign.Prepare()),
                "P9.1 compares bindings against the actual supplied session, not a substituted registry.");
            Assert(foreign.Log.Events.Count == 0 && foreign.Passive.Starts == 0, "P9.1 foreign metadata never starts.");

            var noCostRegistry = new SkillDefinitionRegistry();
            var noCost = new SkillDefinition(foreign.Skill.Id, "Balance", 3, null);
            noCostRegistry.Register(noCost);
            var noCostSession = new LoadedPlayerSession(new ClassDefinitionRegistry(), noCostRegistry,
                new MentorDefinitionRegistry(), new ClassSkillTreeBindingRegistry());
            ExpectEncounterException<ArgumentException>(() => new CombatEncounterComposition(noCostSession, foreign.Profile,
                new CombatEncounterPreparation(foreign.Player, foreign.Enemy, foreign.Log, foreign.Random, foreign.Passives,
                    new[] { new PreparedCombatSkillBinding(foreign.Skill, new SkillProgress(noCost)) })),
                "P9.1 registry presence alone does not make null cost metadata valid.");

            var locked = new EncounterFixture(requiredUnlock: "encounter.lock");
            var lockedOwner = new CombatEncounterComposition(locked.Session, locked.Profile, locked.Prepare());
            Assert(lockedOwner.Start(), "P9.1 preparation does not invent an access policy or grant required unlocks.");
            lockedOwner.Engine.ExecuteTurn(lockedOwner.Runtime,
                TurnRequest.SkillAction(locked.Player, locked.Enemy, locked.Skill, locked.Progress));
            Assert(locked.Progress.Experience == 0 && locked.Log.Events.Any(e => e.Type == GameEventType.ActionBlocked)
                && lockedOwner.Runtime.TurnNumber == 2,
                "P9.1 existing gateway still enforces the canonical unlock requirement.");
        }

        private static void AssertEncounterStartFailureAndReentry()
        {
            var f = new EncounterFixture();
            var owner = new CombatEncounterComposition(f.Session, f.Profile, f.Prepare());
            bool? reentrantResult = null;
            var failure = new InvalidOperationException("prepared passive start failure");
            f.Passive.StartAction = context =>
            {
                Assert(owner.State == CombatEncounterState.Starting, "P9.1 start marks in-flight before hooks.");
                reentrantResult = owner.Start();
                context.Attacker.ReceiveDamage(3);
                context.EventLog.Add(GameEventType.PassiveTriggered, "partial start");
                throw failure;
            };
            var caught = ExpectEncounterException<InvalidOperationException>(() => owner.Start(),
                "P9.1 start failure is propagated.");
            Assert(ReferenceEquals(caught, failure) && ReferenceEquals(owner.StartFailure, failure),
                "P9.1 original exception identity is preserved.");
            Assert(failure.StackTrace.Contains("StartCombat"), "P9.1 original engine exception stack is preserved.");
            Assert(owner.State == CombatEncounterState.Faulted && reentrantResult == false,
                "P9.1 reentry does not rerun start and a failed owner is unusable.");
            Assert(f.Player.Stats.CurrentHealth == 97 && f.Log.Events.Count == 1
                && f.Log.Events[0].Type == GameEventType.PassiveTriggered,
                "P9.1 partial mutations remain observable; no rollback or fabricated start event.");
            var afterFailure = CaptureEncounterFixture(f);
            Assert(!owner.Start() && !owner.Start(), "P9.1 faulted start is never retried.");
            Assert(f.Passive.Starts == 1 && CaptureEncounterFixture(f) == afterFailure,
                "P9.1 retry attempts preserve the faulted encounter exactly.");

            var success = new EncounterFixture();
            var successOwner = new CombatEncounterComposition(success.Session, success.Profile, success.Prepare());
            success.Passive.StartAction = context =>
                Assert(!successOwner.Start(), "P9.1 successful hook reentry is also rejected.");
            Assert(successOwner.Start() && success.Passive.Starts == 1 && success.Log.Events.Count == 1,
                "P9.1 reentrant no-op does not prevent the outer successful start.");
        }

        private static void AssertEncounterPreparationChangesBeforeStart()
        {
            for (int scenario = 0; scenario < 3; scenario++)
            {
                var f = new EncounterFixture();
                var mutableSkill = new EncounterMutableSkill(f.Skill.Id);
                var preparation = new CombatEncounterPreparation(f.Player, f.Enemy, f.Log, f.Random, f.Passives,
                    new[] { new PreparedCombatSkillBinding(mutableSkill, f.Progress) }, f.Resonance);
                var owner = new CombatEncounterComposition(f.Session, f.Profile, preparation);
                if (scenario == 0) f.Player.ReceiveDamage(1000);
                if (scenario == 1) f.Log.Add(GameEventType.CombatStarted, "external write");
                if (scenario == 2) mutableSkill.Id = "changed.id";
                var before = CaptureEncounterFixture(f);
                ExpectEncounterException<ArgumentException>(() => owner.Start(),
                    "P9.1 revalidates transferred dependencies before start hooks.");
                Assert(owner.State == CombatEncounterState.Faulted && !owner.Start(),
                    "P9.1 failed start precheck cannot be retried.");
                Assert(CaptureEncounterFixture(f) == before && f.Passive.Starts == 0,
                    "P9.1 stale preparation never invokes engine start.");
            }
        }

        private static void AssertEncounterPublicBoundary()
        {
            var publicProperties = typeof(CombatEncounterComposition).GetProperties();
            Assert(publicProperties.Length == 2
                && publicProperties.All(p => p.Name == "State" || p.Name == "StartFailure"),
                "P9.1 public owner exposes lifecycle/diagnostics only, not mutable gameplay objects.");
            Assert(typeof(CombatEncounterPreparation).GetProperties().Length == 0,
                "P9.1 preparation does not expose mutable dependency getters.");
            Assert(typeof(PreparedCombatSkillBinding).GetProperties().Single().Name == "SkillId",
                "P9.1 binding public view is ID only.");
        }

        private static T ExpectEncounterException<T>(Action action, string message) where T : Exception
        {
            try { action(); }
            catch (T exception) { Assert(true, message); return exception; }
            Assert(false, message);
            return null;
        }

        private static CombatEncounterComposition EncounterWithBinding(EncounterFixture f, PreparedCombatSkillBinding binding)
        {
            return new CombatEncounterComposition(f.Session, f.Profile,
                new CombatEncounterPreparation(f.Player, f.Enemy, f.Log, f.Random, f.Passives,
                    new[] { binding }, f.Resonance));
        }

        private static CombatEntity EncounterActor(string id)
        {
            return BuildCombatEntity(id, 10, 5, 100, 0, 0, 0, 2f, 40);
        }

        private static SkillDefinition EncounterDefinition(int cost = 7, string requiredUnlock = "")
        {
            return new SkillDefinition("skill_balance_ritual", "Balance", 3, new SkillCost(cost, 0),
                SkillCategory.Combat, SkillTargeting.SingleEnemy, SkillEffectType.Damage, requiredUnlock, 2);
        }

        private static string CaptureEncounterFixture(EncounterFixture f)
        {
            return JsonSerializer.Serialize(new
            {
                f.Player, f.Enemy, f.Resonance, Events = f.Log.Events,
                f.Progress.Level, f.Progress.Experience, f.Random.Calls,
                f.Passive.Starts, f.Passive.Costs, f.Passive.Debuffs,
                Progression = f.Session.RuntimeState, f.Profile
            });
        }

        private sealed class EncounterFixture
        {
            public readonly LoadedPlayerSession Session;
            public readonly PlayerProfile Profile;
            public readonly CombatEntity Player = EncounterActor("prepared.player");
            public readonly CombatEntity Enemy = EncounterActor("prepared.enemy");
            public readonly GameEventLog Log = new GameEventLog();
            public readonly EncounterRandom Random = new EncounterRandom();
            public readonly EncounterPassive Passive = new EncounterPassive();
            public readonly ResonanceProfile Resonance = new ResonanceProfile(new ActiveResonance(20), new ResonanceReserve(50));
            public readonly ICombatSkill Skill = new BalanceRitualSkill();
            public readonly SkillProgress Progress;
            public readonly PreparedCombatSkillBinding Binding;
            public readonly List<ICombatPassiveEffect> Passives;
            public readonly List<PreparedCombatSkillBinding> Bindings;

            public EncounterFixture(LoadedPlayerSession session = null, PlayerProfile profile = null, string requiredUnlock = "")
            {
                if (session == null)
                {
                    var skills = new SkillDefinitionRegistry();
                    skills.Register(EncounterDefinition(requiredUnlock: requiredUnlock));
                    var classes = new ClassDefinitionRegistry();
                    classes.Register(new ClassDefinition(ClassId.Tlamatini, "Prepared", new[] { Skill.Id }));
                    classes.Register(new ClassDefinition(ClassId.Wanderer, "Other", Array.Empty<string>()));
                    session = new LoadedPlayerSession(classes, skills, new MentorDefinitionRegistry(), new ClassSkillTreeBindingRegistry());
                    session.RuntimeState.ClassProfile.AssignClassId(ClassId.Tlamatini, classes);
                }

                Session = session;
                Profile = profile ?? new PlayerProfile();
                Progress = new SkillProgress(session.RuntimeState.SkillRegistry.Get(Skill.Id));
                Binding = new PreparedCombatSkillBinding(Skill, Progress);
                Passives = new List<ICombatPassiveEffect> { Passive };
                Bindings = new List<PreparedCombatSkillBinding> { Binding };
            }

            public CombatEncounterPreparation Prepare()
            {
                return new CombatEncounterPreparation(Player, Enemy, Log, Random, Passives, Bindings, Resonance);
            }

        }

        private sealed class EncounterRandom : IRandomSource
        {
            public int Calls { get; private set; }
            public double NextDouble() { Calls++; return 0.5; }
            public int NextInt(int minInclusive, int maxExclusive) { Calls++; return minInclusive; }
        }

        private sealed class EncounterPassive : ICombatPassiveEffect
        {
            public int Starts { get; private set; }
            public int Costs { get; private set; }
            public int Debuffs { get; private set; }
            public Action<CombatContext> StartAction;
            public void OnCombatStart(CombatContext context) { Starts++; StartAction?.Invoke(context); }
            public void OnBeforeSkillCostCalculated(CombatContext context) { Costs++; }
            public void OnBeforeDebuffApplied(DebuffApplicationContext context) { Debuffs++; }
        }

        private sealed class EncounterMutableSkill : ICombatSkill
        {
            public string Id { get; set; }
            public string Name { get { return "Structural test only"; } }
            public EncounterMutableSkill(string id) { Id = id; }
            public void Apply(CombatContext context, SkillProgress progress)
            {
                throw new InvalidOperationException("Validation must never execute a skill.");
            }
        }
    }
}
