using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Game.Application.Runtime;
using Game.Domain.Classes;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.StatusEffects;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunCombatEncounterTurnFlowSmokeTests()
        {
            AssertFlowAlternationAndRecovery();
            AssertFlowBlockedSkills();
            AssertFlowRefills();
            AssertFlowTerminalBoundaries();
            AssertFlowReentrancyAndFaults();
            AssertFlowObservationFailures();
            AssertFlowPublicBoundary();
        }

        private static void AssertFlowAlternationAndRecovery()
        {
            var f = new EncounterFixture();
            var owner = CreateProjectionOwner(f);
            var initial = owner.GetTurnFlowSnapshot();
            Assert(initial.Phase == CombatTurnFlowPhase.NotStarted && initial.EligibleActor == null,
                "P9.4 preparation creates no acting opportunity.");
            AssertFlowRejected(owner.AdvanceEnemyTurn(), CombatIntentAdmissionReason.EncounterNotStarted, CombatActorSide.Enemy);
            AssertFlowRejected(owner.SubmitIntent(CombatIntent.BasicAttack()), CombatIntentAdmissionReason.EncounterNotStarted, CombatActorSide.Player);
            f.Passive.StartAction = context =>
            {
                Assert(owner.GetTurnFlowSnapshot().Phase == CombatTurnFlowPhase.NotStarted,
                    "P9.4 starting hooks cannot observe an eligible player yet.");
                AssertFlowRejected(owner.AdvanceEnemyTurn(), CombatIntentAdmissionReason.CommandInFlight, CombatActorSide.Enemy);
            };
            owner.Start();
            var awaiting = owner.GetTurnFlowSnapshot();
            Assert(awaiting.Phase == CombatTurnFlowPhase.AwaitingPlayer && awaiting.EligibleActor == CombatActorSide.Player,
                "P9.4 successful start yields the first player opportunity.");
            var source = CaptureProjectionSource(owner, f);
            AssertFlowRejected(owner.AdvanceEnemyTurn(), CombatIntentAdmissionReason.NoEnemyOpportunity, CombatActorSide.Enemy);
            AssertFlowRejected(owner.SubmitIntent(null), CombatIntentAdmissionReason.NullIntent, CombatActorSide.Player);
            AssertFlowRejected(owner.SubmitIntent(CombatIntent.Skill("missing")), CombatIntentAdmissionReason.SkillNotPrepared, CombatActorSide.Player);
            Assert(CaptureProjectionSource(owner, f) == source && owner.GetTurnFlowSnapshot().Phase == CombatTurnFlowPhase.AwaitingPlayer,
                "P9.4 admission failures retain opportunity, domain state, RNG and pending start events.");

            int next = 0;
            var observed = new System.Collections.Generic.List<int>();
            string firstReceipt = null;
            CombatIntentReceipt first = null;
            for (int cycle = 0; cycle < 3; cycle++)
            {
                var player = owner.SubmitIntent(cycle == 1 ? CombatIntent.Skill(f.Skill.Id) : CombatIntent.BasicAttack());
                AssertFlowConsumed(player, CombatActorSide.Player, CombatTurnFlowPhase.AwaitingPlayer, CombatTurnFlowPhase.AwaitingEnemy);
                Assert(player.Before.TurnNumber == 1 + cycle * 2 && player.After.TurnNumber == 2 + cycle * 2,
                    "P9.4 existing counter still advances once per nonterminal action, not per round.");
                Assert(player.EventBatch.Events.All(e => e.Type != GameEventType.CombatStarted),
                    "P9.4 command ranges exclude unconsumed start events.");
                var prefix = owner.ReadEvents(next, player.EventBatch.StartIndex);
                observed.AddRange(prefix.Events.Select(e => e.Index));
                observed.AddRange(player.EventBatch.Events.Select(e => e.Index));
                next = player.EventBatch.EndIndex;
                if (first == null) { first = player; firstReceipt = JsonSerializer.Serialize(first); }
                source = CaptureProjectionSource(owner, f);
                foreach (var intent in new[] { CombatIntent.BasicAttack(), CombatIntent.Skill(f.Skill.Id), CombatIntent.Refill(1), null })
                    AssertFlowRejected(owner.SubmitIntent(intent), CombatIntentAdmissionReason.NotPlayerOpportunity, CombatActorSide.Player);
                Assert(CaptureProjectionSource(owner, f) == source && owner.GetTurnFlowSnapshot().EligibleActor == CombatActorSide.Enemy,
                    "P9.4 SubmitIntent cannot bypass or consume the pending enemy opportunity.");

                int calls = f.Random.Calls;
                int xp = f.Progress.Experience;
                int resonance = f.Resonance.Active.Current;
                var enemy = owner.AdvanceEnemyTurn();
                AssertFlowConsumed(enemy, CombatActorSide.Enemy, CombatTurnFlowPhase.AwaitingEnemy, CombatTurnFlowPhase.AwaitingPlayer);
                Assert(enemy.EventBatch.StartIndex == player.EventBatch.EndIndex
                    && enemy.EventBatch.Events.Count(e => e.Type == GameEventType.BasicAttackUsed) == 1,
                    "P9.4 one pending enemy opportunity invokes one basic attack in a contiguous range.");
                Assert(enemy.After.Player.CurrentHealth == enemy.Before.Player.CurrentHealth - 6
                    && enemy.After.Enemy.CurrentHealth == enemy.Before.Enemy.CurrentHealth && f.Random.Calls == calls + 2,
                    "P9.4 retained enemy/player direction and deterministic hit/critical RNG use existing damage rules.");
                Assert(f.Progress.Experience == xp && f.Resonance.Active.Current == resonance && enemy.RefillResult == null,
                    "P9.4 enemy scheduling never grants skill XP or spends player Resonance.");
                source = CaptureProjectionSource(owner, f);
                AssertFlowRejected(owner.AdvanceEnemyTurn(), CombatIntentAdmissionReason.NoEnemyOpportunity, CombatActorSide.Enemy);
                Assert(CaptureProjectionSource(owner, f) == source, "P9.4 duplicate enemy advance is a pure rejection, never another invocation.");
                // Recover the enemy batch from the cursor, as if its receipt were lost.
                var recovery = owner.ReadEvents(next);
                Assert(JsonSerializer.Serialize(recovery) == JsonSerializer.Serialize(enemy.EventBatch),
                    "P9.4 lost enemy receipt is recoverable without replaying gameplay.");
                observed.AddRange(recovery.Events.Select(e => e.Index));
                next = recovery.EndIndex;
                Assert(owner.ReadEvents(next).Events.Count == 0 && CaptureProjectionSource(owner, f) == source,
                    "P9.4 recovery advances only the consumer cursor and produces no duplicate observations.");
            }
            Assert(observed.SequenceEqual(Enumerable.Range(0, f.Log.Events.Count)),
                "P9.4 start/player/enemy event processing has no gaps or duplicates across cycles.");
            Assert(owner.ReadStartEvents().Events.Single().Type == GameEventType.CombatStarted,
                "P9.4 original start batch remains separately observable after alternating actions.");
            Assert(JsonSerializer.Serialize(first) == firstReceipt && initial.Phase == CombatTurnFlowPhase.NotStarted
                && awaiting.Phase == CombatTurnFlowPhase.AwaitingPlayer,
                "P9.4 old receipts, combat snapshots and flow values stay detached.");
            Assert(f.Progress.Experience == 10 && f.Resonance.Active.Current == 18 && f.Passive.Costs == 1,
                "P9.4 integrated cycle applies the single skill XP and Resonance cost exactly once.");
        }

        private static void AssertFlowBlockedSkills()
        {
            foreach (string blocker in new[] { "class", "unmastered", "unlock", "essence", "resonance", "silence", "seal" })
            {
                var f = new EncounterFixture(requiredUnlock: blocker == "unlock" ? "locked" : "");
                var owner = CreateProjectionOwner(f);
                owner.Start();
                if (blocker == "class" || blocker == "unmastered")
                    f.Session.RuntimeState.ClassProfile.ChangeClassId(ClassId.Wanderer, f.Session.RuntimeState.ClassRegistry);
                if (blocker == "unmastered")
                    f.Session.RuntimeState.MasteryProfile.IncreaseProgress(f.Skill.Id, 99, f.Session.RuntimeState.SkillRegistry);
                if (blocker == "essence") f.Player.Resource.TrySpend(40);
                if (blocker == "resonance") f.Resonance.TrySpendActive(20);
                if (blocker == "silence") f.Player.Debuffs.Add(new SilenceDebuff(1), f.Player, new CombatContext(f.Player, f.Enemy, f.Log));
                if (blocker == "seal") f.Player.StatusEffects.Apply(new StatusEffectInstance(StatusEffectType.SkillSeal, 1, false));
                int active = f.Resonance.Active.Current;
                var blocked = owner.SubmitIntent(CombatIntent.Skill(f.Skill.Id));
                AssertFlowConsumed(blocked, CombatActorSide.Player, CombatTurnFlowPhase.AwaitingPlayer, CombatTurnFlowPhase.AwaitingEnemy);
                Assert(blocked.EventBatch.Events.Any(e => e.Type == GameEventType.ActionBlocked || e.Type == GameEventType.ResourceNotEnough)
                    && f.Progress.Experience == 0 && f.Enemy.Stats.CurrentHealth == 100 && f.Resonance.Active.Current == active,
                    "P9.4 existing skill blocker consumes lifecycle without skill effect/spending/XP: " + blocker);
                AssertFlowConsumed(owner.AdvanceEnemyTurn(), CombatActorSide.Enemy, CombatTurnFlowPhase.AwaitingEnemy, CombatTurnFlowPhase.AwaitingPlayer);
                if (blocker == "unmastered")
                {
                    f.Session.RuntimeState.MasteryProfile.IncreaseProgress(f.Skill.Id, 1, f.Session.RuntimeState.SkillRegistry);
                    var retained = owner.SubmitIntent(CombatIntent.Skill(f.Skill.Id));
                    AssertFlowConsumed(retained, CombatActorSide.Player, CombatTurnFlowPhase.AwaitingPlayer, CombatTurnFlowPhase.AwaitingEnemy);
                    Assert(f.Progress.Experience == 10, "P9.4 retained mastery still authorizes through the same engine, not the scheduler.");
                }
            }
        }

        private static void AssertFlowRefills()
        {
            foreach (string scenario in new[] { "early-denial", "late-denial", "transfer", "zero", "negative", "full" })
            {
                var f = new EncounterFixture();
                var owner = CreateProjectionOwner(f);
                owner.Start();
                if (scenario != "full") f.Resonance.TrySpendActive(8);
                if (scenario == "early-denial") f.Resonance.Reserve.TryConsume(50);
                if (scenario == "late-denial") f.Player.Debuffs.Add(new IntentBoundaryDebuff(
                    context => f.Resonance.Reserve.TryConsume(50)), f.Player, new CombatContext(f.Player, f.Enemy, f.Log));
                int amount = scenario == "zero" ? 0 : scenario == "negative" ? -1 : 4;
                var result = owner.SubmitIntent(CombatIntent.Refill(amount));
                AssertCompletedIntent(result);
                bool consumes = scenario != "early-denial";
                Assert(result.OpportunityConsumed == consumes && result.TurnStartedObserved == consumes
                    && result.TurnEndedObserved == consumes,
                    "P9.4 refill opportunity depends on lifecycle start, not admission or transfer bool: " + scenario);
                Assert(result.RefillResult == (scenario != "early-denial" && scenario != "late-denial")
                    && result.FlowAfter.Phase == (consumes ? CombatTurnFlowPhase.AwaitingEnemy : CombatTurnFlowPhase.AwaitingPlayer),
                    "P9.4 refill result and scheduling are independent facts: " + scenario);
                Assert(f.Progress.Experience == 0 && f.Random.Calls == 0 && f.Passive.Costs == 0,
                    "P9.4 refill scheduling adds no skill XP, cost hooks or damage RNG.");
                if (consumes)
                    AssertFlowConsumed(owner.AdvanceEnemyTurn(), CombatActorSide.Enemy, CombatTurnFlowPhase.AwaitingEnemy, CombatTurnFlowPhase.AwaitingPlayer);
                else
                {
                    var unchanged = CaptureProjectionSource(owner, f);
                    AssertFlowRejected(owner.AdvanceEnemyTurn(), CombatIntentAdmissionReason.NoEnemyOpportunity, CombatActorSide.Enemy);
                    Assert(CaptureProjectionSource(owner, f) == unchanged, "P9.4 early refill denial schedules no retaliation.");
                    AssertFlowConsumed(owner.SubmitIntent(CombatIntent.BasicAttack()), CombatActorSide.Player,
                        CombatTurnFlowPhase.AwaitingPlayer, CombatTurnFlowPhase.AwaitingEnemy);
                }
            }
        }

        private static void AssertFlowTerminalBoundaries()
        {
            foreach (string scenario in new[] { "player-kill", "player-start", "player-end", "refill-start", "refill-end",
                "enemy-kill", "enemy-start", "enemy-end" })
            {
                var f = new EncounterFixture();
                var owner = CreateProjectionOwner(f);
                owner.Start();
                bool enemy = scenario.StartsWith("enemy", StringComparison.Ordinal);
                if (enemy) owner.SubmitIntent(CombatIntent.Refill(0));
                var actor = enemy ? f.Enemy : f.Player;
                var target = enemy ? f.Player : f.Enemy;
                if (scenario.EndsWith("kill", StringComparison.Ordinal)) target.ReceiveDamage(99);
                if (scenario.EndsWith("start", StringComparison.Ordinal))
                    actor.Debuffs.Add(new PoisonDebuff(2, 200), actor, new CombatContext(actor, target, f.Log));
                if (scenario.EndsWith("end", StringComparison.Ordinal))
                    actor.StatusEffects.Apply(new StatusEffectInstance(StatusEffectType.Poison, 2, false, 200));
                int turn = owner.GetSnapshot().TurnNumber;
                var receipt = enemy ? owner.AdvanceEnemyTurn() : owner.SubmitIntent(
                    scenario.StartsWith("refill", StringComparison.Ordinal) ? CombatIntent.Refill(0) : CombatIntent.BasicAttack());
                AssertCompletedIntent(receipt);
                Assert(receipt.OpportunityConsumed && receipt.TurnStartedObserved && !receipt.TurnEndedObserved
                    && receipt.After.TurnNumber == turn, "P9.4 lethal action consumes opportunity without end event/counter advance: " + scenario);
                Assert(receipt.FlowAfter.Phase == CombatTurnFlowPhase.Terminal && receipt.FlowAfter.EligibleActor == null
                    && receipt.EncounterIsTerminal == true && owner.State == CombatEncounterState.Terminal,
                    "P9.4 death always suppresses the next actor, including lethal enemy completion: " + scenario);
                if (scenario == "enemy-start")
                    Assert(!receipt.EventBatch.Events.Any(e => e.Type == GameEventType.BasicAttackUsed),
                        "P9.4 dead enemy at Begin never attacks.");
                var before = CaptureProjectionSource(owner, f);
                AssertFlowRejected(owner.SubmitIntent(CombatIntent.BasicAttack()), CombatIntentAdmissionReason.EncounterTerminal, CombatActorSide.Player);
                AssertFlowRejected(owner.AdvanceEnemyTurn(), CombatIntentAdmissionReason.EncounterTerminal, CombatActorSide.Enemy);
                Assert(CaptureProjectionSource(owner, f) == before && !owner.Start(), "P9.4 terminal commands cannot retaliate, restart or mutate.");
            }

            var statuses = new EncounterFixture();
            var statusOwner = CreateProjectionOwner(statuses);
            statusOwner.Start();
            statuses.Enemy.Debuffs.Add(new PoisonDebuff(2, 3), statuses.Enemy, new CombatContext(statuses.Enemy, statuses.Player, statuses.Log));
            statuses.Enemy.StatusEffects.Apply(new StatusEffectInstance(StatusEffectType.Poison, 2, false, 4));
            statuses.Enemy.Resource.TrySpend(10);
            statusOwner.SubmitIntent(CombatIntent.Refill(0));
            var enemyStatus = statusOwner.AdvanceEnemyTurn();
            Assert(enemyStatus.Before.Enemy.CurrentHealth == 100 && enemyStatus.After.Enemy.CurrentHealth == 93
                && enemyStatus.After.Enemy.Essence.Current == 35 && enemyStatus.After.Enemy.StatusEffects.Single().RemainingDuration == 1
                && enemyStatus.After.Enemy.Debuffs.Single().RemainingTurns == 1,
                "P9.4 enemy uses canonical legacy poison, end DOT, duration ticking and actor Essence regeneration.");
            Assert(enemyStatus.After.Player.CurrentHealth == 94 && enemyStatus.TurnEndedObserved,
                "P9.4 enemy status lifecycle does not replace its existing damage action.");

            foreach (bool endedFlag in new[] { false, true })
            {
                var f = new EncounterFixture();
                var owner = CreateProjectionOwner(f);
                owner.Start();
                owner.SubmitIntent(CombatIntent.Refill(0));
                if (endedFlag) owner.Runtime.EndCombat(); else f.Enemy.ReceiveDamage(1000);
                Assert(owner.GetTurnFlowSnapshot().IsTerminal && owner.GetTurnFlowSnapshot().EligibleActor == null,
                    "P9.4 observed ended flag or stale defeated actor overrides a previously pending enemy phase.");
                AssertFlowRejected(owner.AdvanceEnemyTurn(), CombatIntentAdmissionReason.EncounterTerminal, CombatActorSide.Enemy);
            }
        }

        private static void AssertFlowReentrancyAndFaults()
        {
            foreach (bool enemy in new[] { false, true })
            {
                var f = new EncounterFixture();
                var owner = CreateProjectionOwner(f);
                owner.Start();
                if (enemy) owner.SubmitIntent(CombatIntent.Refill(0));
                var actor = enemy ? f.Enemy : f.Player;
                var target = enemy ? f.Player : f.Enemy;
                int hooks = 0;
                actor.Debuffs.Add(new IntentBoundaryDebuff(context =>
                {
                    hooks++;
                    var phase = enemy ? CombatTurnFlowPhase.ResolvingEnemy : CombatTurnFlowPhase.ResolvingPlayer;
                    Assert(owner.GetTurnFlowSnapshot().Phase == phase && owner.GetTurnFlowSnapshot().EligibleActor == null,
                        "P9.4 resolving phase is observable without exposing partial mutable combat state.");
                    AssertFlowRejected(owner.SubmitIntent(CombatIntent.BasicAttack()), CombatIntentAdmissionReason.CommandInFlight, CombatActorSide.Player);
                    AssertFlowRejected(owner.AdvanceEnemyTurn(), CombatIntentAdmissionReason.CommandInFlight, CombatActorSide.Enemy);
                    ExpectEncounterException<InvalidOperationException>(() => owner.GetSnapshot(), "P9.4 combat reads still reject in-flight capture.");
                    Assert(!owner.Start(), "P9.4 reentrant Start remains rejected.");
                }), actor, new CombatContext(actor, target, f.Log));
                var receipt = enemy ? owner.AdvanceEnemyTurn() : owner.SubmitIntent(CombatIntent.BasicAttack());
                AssertCompletedIntent(receipt);
                Assert(hooks == 1 && receipt.EventBatch.Events.Count(e => e.Type == GameEventType.BasicAttackUsed) == 1,
                    "P9.4 both reentrant entry points preserve exactly one outer invocation.");
            }

            foreach (bool enemy in new[] { false, true })
            {
                var f = new EncounterFixture();
                var owner = CreateProjectionOwner(f);
                owner.Start();
                if (enemy) owner.SubmitIntent(CombatIntent.Refill(0));
                var actor = enemy ? f.Enemy : f.Player;
                var target = enemy ? f.Player : f.Enemy;
                var error = new InvalidOperationException("original turn flow hook failure");
                int calls = 0;
                actor.Debuffs.Add(new IntentBoundaryDebuff(context =>
                {
                    calls++;
                    actor.ReceiveDamage(7);
                    actor.Resource.TrySpend(3);
                    context.EventLog.Add(GameEventType.PassiveTriggered, "partial turn flow mutation");
                    throw error;
                }), actor, new CombatContext(actor, target, f.Log));
                var receipt = enemy ? owner.AdvanceEnemyTurn() : owner.SubmitIntent(CombatIntent.BasicAttack());
                Assert(receipt.Disposition == CombatIntentDisposition.Faulted && receipt.InvocationStarted && !receipt.InvocationCompleted
                    && receipt.OpportunityConsumed && !receipt.TurnEndedObserved,
                    "P9.4 partial started fault is distinct from admission rejection or a completed invocation.");
                Assert(ReferenceEquals(owner.ExecutionFailure, error) && error.StackTrace.Contains("ExecuteTurn")
                    && receipt.FaultMessage == error.Message && receipt.FlowAfter.IsFaulted && receipt.FlowAfter.EligibleActor == null,
                    "P9.4 original exception and existing Faulted lifecycle suppress all later scheduling.");
                var partial = enemy ? receipt.After.Enemy : receipt.After.Player;
                Assert(partial.CurrentHealth == 93 && partial.Essence.Current == 37 && f.Progress.Experience == 0,
                    "P9.4 failure preserves partial health/resource mutation without rollback or completion regeneration.");
                string source = CaptureProjectionSource(owner, f);
                AssertFlowRejected(owner.AdvanceEnemyTurn(), CombatIntentAdmissionReason.EncounterFaulted, CombatActorSide.Enemy);
                AssertFlowRejected(owner.SubmitIntent(CombatIntent.BasicAttack()), CombatIntentAdmissionReason.EncounterFaulted, CombatActorSide.Player);
                Assert(calls == 1 && !owner.Start() && CaptureProjectionSource(owner, f) == source,
                    "P9.4 faulted operations neither retry nor queue another actor.");
                Assert(JsonSerializer.Serialize(owner.ReadEvents(receipt.EventBatch.StartIndex)) == JsonSerializer.Serialize(receipt.EventBatch),
                    "P9.4 failed enemy/player events remain recoverable independently of gameplay execution.");
            }
        }

        private static void AssertFlowObservationFailures()
        {
            foreach (bool beforeInvocation in new[] { true, false })
            {
                var f = new EncounterFixture();
                var owner = CreateProjectionOwner(f);
                owner.Start();
                owner.SubmitIntent(CombatIntent.Refill(0));
                var effect = new IntentBoundaryDebuff(null) { ObservationError = new InvalidOperationException("enemy observation fault") };
                f.Enemy.Debuffs.Add(effect, f.Enemy, new CombatContext(f.Enemy, f.Player, f.Log));
                if (beforeInvocation) effect.BrokenObservation = true;
                else f.Enemy.Debuffs.Add(new IntentBoundaryDebuff(context => effect.BrokenObservation = true),
                    f.Enemy, new CombatContext(f.Enemy, f.Player, f.Log));
                var receipt = owner.AdvanceEnemyTurn();
                Assert(receipt.Disposition == CombatIntentDisposition.Faulted && receipt.FlowAfter.IsFaulted
                    && receipt.After == null && receipt.ObservationFailureMessage == effect.ObservationError.Message,
                    "P9.4 unreadable enemy state remains an explicit fault, never a phantom player opportunity.");
                Assert(receipt.InvocationStarted == !beforeInvocation && receipt.InvocationCompleted == !beforeInvocation
                    && receipt.OpportunityConsumed == !beforeInvocation && ReferenceEquals(owner.ExecutionFailure, effect.ObservationError),
                    "P9.4 before/after projection failures preserve actual invocation and consumption facts.");
                AssertFlowRejected(owner.AdvanceEnemyTurn(), CombatIntentAdmissionReason.EncounterFaulted, CombatActorSide.Enemy);
            }
        }

        private static void AssertFlowPublicBoundary()
        {
            Assert(typeof(CombatTurnFlowSnapshot).IsSealed && typeof(CombatTurnFlowSnapshot).GetConstructors().Length == 0
                && typeof(CombatTurnFlowSnapshot).GetProperties().All(p => p.SetMethod == null
                    && (p.PropertyType.IsEnum || p.PropertyType == typeof(bool) || p.PropertyType == typeof(CombatActorSide?))),
                "P9.4 flow projection contains only immutable scheduling facts, not domain references.");
            Assert(typeof(CombatTurnFlowSnapshot).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).All(f => f.IsInitOnly),
                "P9.4 detached flow values retain no mutable scheduler state.");
            Assert(typeof(CombatEncounterComposition).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => m.ReturnType == typeof(CombatIntentReceipt)).Select(m => m.Name).OrderBy(n => n)
                .SequenceEqual(new[] { "AdvanceEnemyTurn", "SubmitIntent" }),
                "P9.4 the two centrally gated methods are the only public action entry points.");
        }

        private static void AssertFlowConsumed(CombatIntentReceipt receipt, CombatActorSide actor,
            CombatTurnFlowPhase before, CombatTurnFlowPhase after)
        {
            AssertCompletedIntent(receipt);
            Assert(receipt.ActorSide == actor && receipt.OpportunityConsumed && receipt.TurnStartedObserved
                && receipt.FlowBefore.Phase == before && receipt.FlowAfter.Phase == after,
                "P9.4 consumed opportunity exposes actual actor and immutable phase transition.");
        }

        private static void AssertFlowRejected(CombatIntentReceipt receipt, CombatIntentAdmissionReason reason, CombatActorSide actor)
        {
            AssertRejectedIntent(receipt, reason);
            Assert(receipt.ActorSide == actor && !receipt.OpportunityConsumed && receipt.FlowBefore.Phase == receipt.FlowAfter.Phase,
                "P9.4 rejected command identifies requested actor without consuming/changing opportunity.");
        }
    }
}
