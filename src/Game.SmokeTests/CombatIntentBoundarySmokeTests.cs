using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Game.Application.Runtime;
using Game.Domain.Classes;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.StatusEffects;
using Game.Domain.Skills;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunCombatIntentBoundarySmokeTests()
        {
            AssertIntentExecutionAndRecovery();
            AssertIntentAdmission();
            AssertIntentAuthorization();
            AssertIntentRefillAndLethality();
            AssertIntentReentrancyAndFaults();
            AssertIntentObservationFailure();
            AssertIntentValueSurface();
        }

        private static void AssertIntentExecutionAndRecovery()
        {
            var f = new EncounterFixture();
            var owner = CreateProjectionOwner(f);
            Assert(owner.ReadEvents(0).Events.Count == 0, "P9.3 prepared log can be observed without starting.");
            ExpectEncounterException<InvalidOperationException>(() => owner.ReadStartEvents(), "P9.3 no invented start receipt before Start.");
            f.Passive.StartAction = context =>
            {
                context.EventLog.Add(GameEventType.PassiveTriggered, "TurnStarted ActionBlocked 999 damage: display only");
                context.RevealOneEnemyTrait = true;
            };
            owner.Start();
            var rejected = owner.SubmitIntent(null);
            Assert(rejected.EventBatch.Events.Count == 0 && rejected.EventBatch.StartIndex == 3,
                "P9.3 rejected receipt never absorbs unobserved start events.");
            var first = owner.SubmitIntent(CombatIntent.BasicAttack());
            AssertCompletedIntent(first);
            Assert(first.Before.TurnNumber == 1 && first.After.TurnNumber == 2 && first.After.Enemy.CurrentHealth < first.Before.Enemy.CurrentHealth,
                "P9.3 basic intent executes one real player-to-enemy turn.");
            Assert(first.EventBatch.StartIndex == 3 && !first.EventBatch.Events.Any(e => e.Type == GameEventType.CombatStarted),
                "P9.3 first action excludes start events even when they were not read yet.");
            Assert(first.EventBatch.Events.Count(e => e.Type == GameEventType.BasicAttackUsed) == 1
                && first.TurnStartedObserved && first.TurnEndedObserved && first.RefillResult == null,
                "P9.3 receipt reports one invocation's actual lifecycle facts.");
            string firstValues = JsonSerializer.Serialize(first);
            var start = owner.ReadStartEvents();
            Assert(start.StartIndex == 0 && start.EndIndex == 3
                && start.Events.Select(e => e.Type).SequenceEqual(new[] { GameEventType.PassiveTriggered, GameEventType.CombatStarted, GameEventType.TraitRevealed }),
                "P9.3 start range preserves hook/start/trait ordering independently of later actions.");
            Assert(start.Events[0].Message == "TurnStarted ActionBlocked 999 damage: display only",
                "P9.3 messages are copied verbatim, not parsed into gameplay facts.");
            AssertEventBatchMatchesLog(start, f.Log);
            AssertEventBatchMatchesLog(first.EventBatch, f.Log);

            // A consumer processes a missing prefix before the action range, then advances its cursor.
            int next = 0;
            var prefix = owner.ReadEvents(next, first.EventBatch.StartIndex);
            next = prefix.EndIndex;
            Assert(next == first.EventBatch.StartIndex, "P9.3 explicit ranges recover the unconsumed start prefix without skipping it.");
            next = first.EventBatch.EndIndex;
            Assert(owner.ReadEvents(next).Events.Count == 0, "P9.3 successive reads at EndIndex contain no duplicate events.");
            var enemy = owner.AdvanceEnemyTurn();
            next = enemy.EventBatch.EndIndex;
            var skill = owner.SubmitIntent(CombatIntent.Skill(f.Skill.Id));
            AssertCompletedIntent(skill);
            Assert(skill.Before.TurnNumber == 3 && skill.After.TurnNumber == 4
                && skill.EventBatch.StartIndex == next && f.Progress.Experience == 10,
                "P9.3 skill receipt begins at the previous end and grants XP once.");
            Assert(f.Resonance.Active.Current == 18 && f.Passive.Costs == 1
                && ReferenceEquals(owner.GetSkillBinding(f.Skill.Id).Progress, f.Progress),
                "P9.3 skill resolves exact retained progress and spends player Resonance once.");
            Assert(skill.EventBatch.Events.Count(e => e.Type == GameEventType.SkillUsed) == 1,
                "P9.3 prepared executable is applied once.");
            AssertEventBatchMatchesLog(skill.EventBatch, f.Log);
            next = skill.EventBatch.EndIndex;
            owner.AdvanceEnemyTurn(); // Simulate losing this receipt.
            var beforeRecovery = CaptureProjectionSource(owner, f);
            var recovered = owner.ReadEvents(next);
            Assert(recovered.Events.Count(e => e.Type == GameEventType.BasicAttackUsed) == 1
                && recovered.StartIndex == next && recovered.EndIndex == f.Log.Events.Count,
                "P9.3 lost receipt is recoverable from the existing log without submitting another action.");
            Assert(owner.ReadEvents(recovered.EndIndex).Events.Count == 0
                && CaptureProjectionSource(owner, f) == beforeRecovery,
                "P9.3 recovery and empty follow-up do not execute, consume RNG or alter state.");
            Assert(prefix.Events.Concat(first.EventBatch.Events).Concat(enemy.EventBatch.Events).Concat(skill.EventBatch.Events).Concat(recovered.Events)
                .Select(e => e.Index).SequenceEqual(Enumerable.Range(0, recovered.EndIndex)),
                "P9.3 consumer cursor protocol delivers every index once, without gaps or duplicates.");
            Assert(JsonSerializer.Serialize(owner.ReadEvents(first.EventBatch.StartIndex, first.EventBatch.EndIndex))
                == JsonSerializer.Serialize(first.EventBatch), "P9.3 explicit historical replay returns only detached observations.");
            Assert(JsonSerializer.Serialize(first) == firstValues, "P9.3 old receipt snapshots and events stay unchanged.");
            AssertProjectionCollectionImmutable(first.EventBatch.Events);
            Assert(!owner.Start() && JsonSerializer.Serialize(owner.ReadStartEvents()) == JsonSerializer.Serialize(start),
                "P9.3 repeated Start cannot replace or duplicate the start range.");
            Action[] invalidCursors = { () => owner.ReadEvents(-1), () => owner.ReadEvents(f.Log.Events.Count + 1),
                () => owner.ReadEvents(1, 0), () => owner.ReadEvents(0, f.Log.Events.Count + 1) };
            foreach (var action in invalidCursors) ExpectEncounterException<ArgumentOutOfRangeException>(action, "P9.3 invalid cursor ranges fail explicitly.");
            Assert(CaptureProjectionSource(owner, f) == beforeRecovery, "P9.3 invalid ranges do not move a hidden cursor or mutate state.");
        }

        private static void AssertIntentAdmission()
        {
            var f = new EncounterFixture();
            var owner = CreateProjectionOwner(f);
            var before = CaptureProjectionSource(owner, f);
            var prepared = owner.SubmitIntent(CombatIntent.BasicAttack());
            AssertRejectedIntent(prepared, CombatIntentAdmissionReason.EncounterNotStarted);
            Assert(prepared.Before.State == CombatEncounterState.Prepared && CaptureProjectionSource(owner, f) == before,
                "P9.3 admission never starts an encounter implicitly.");
            owner.Start();
            f.Session.RuntimeState.SkillRegistry.Register(new SkillDefinition("registered.not.prepared", "Not prepared", 1, new SkillCost(0, 0)));
            var unknown = (CombatIntent)typeof(CombatIntent).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                null, new[] { typeof(CombatIntentType), typeof(string), typeof(int) }, null)
                .Invoke(new object[] { (CombatIntentType)999, null, 0 });
            var cases = new (CombatIntent Intent, CombatIntentAdmissionReason Reason)[]
            {
                (Intent: (CombatIntent)null, Reason: CombatIntentAdmissionReason.NullIntent),
                (CombatIntent.Skill(null), CombatIntentAdmissionReason.InvalidSkillId),
                (CombatIntent.Skill(""), CombatIntentAdmissionReason.InvalidSkillId),
                (CombatIntent.Skill(" \t"), CombatIntentAdmissionReason.InvalidSkillId),
                (CombatIntent.Skill("unknown"), CombatIntentAdmissionReason.SkillNotPrepared),
                (CombatIntent.Skill(f.Skill.Id.ToUpperInvariant()), CombatIntentAdmissionReason.SkillNotPrepared),
                (CombatIntent.Skill("registered.not.prepared"), CombatIntentAdmissionReason.SkillNotPrepared),
                (unknown, CombatIntentAdmissionReason.UnknownIntentType)
            };
            before = CaptureProjectionSource(owner, f);
            foreach (var item in cases)
            {
                var result = owner.SubmitIntent(item.Intent);
                AssertRejectedIntent(result, item.Reason);
                Assert(ReferenceEquals(result.Before, result.After) && result.Before.TurnNumber == 1
                    && CaptureProjectionSource(owner, f) == before, "P9.3 rejected admission neither invokes hooks nor changes state.");
                Assert(owner.ReadEvents(0).Events.Count == 1, "P9.3 admission rejection does not consume the pending start event.");
            }
            f.Enemy.ReceiveDamage(1000);
            var dead = owner.SubmitIntent(CombatIntent.BasicAttack());
            AssertRejectedIntent(dead, CombatIntentAdmissionReason.EncounterTerminal);
            Assert(dead.EncounterIsTerminal == true && !dead.After.CombatEnded, "P9.3 dead actors reject without fabricating the ended flag.");
            var ended = new EncounterFixture();
            var endedOwner = CreateProjectionOwner(ended);
            endedOwner.Start();
            endedOwner.Runtime.EndCombat();
            AssertRejectedIntent(endedOwner.SubmitIntent(CombatIntent.Refill(4)), CombatIntentAdmissionReason.EncounterTerminal);

            var noPool = new EncounterFixture();
            var noPoolOwner = new CombatEncounterComposition(noPool.Session, noPool.Profile,
                new CombatEncounterPreparation(noPool.Player, noPool.Enemy, noPool.Log, noPool.Random, noPool.Passives, noPool.Bindings));
            noPoolOwner.Start();
            before = CaptureProjectionSource(noPoolOwner, noPool);
            AssertRejectedIntent(noPoolOwner.SubmitIntent(CombatIntent.Refill(4)), CombatIntentAdmissionReason.ResonanceUnavailable);
            Assert(CaptureProjectionSource(noPoolOwner, noPool) == before, "P9.3 absent pool is rejected without inventing one.");
        }

        private static void AssertIntentAuthorization()
        {
            var f = new EncounterFixture();
            var owner = CreateProjectionOwner(f);
            owner.Start();
            var state = f.Session.RuntimeState;
            state.ClassProfile.ChangeClassId(ClassId.Wanderer, state.ClassRegistry);
            var blocked = owner.SubmitIntent(CombatIntent.Skill(f.Skill.Id));
            AssertCompletedIntent(blocked);
            Assert(blocked.TurnStartedObserved && blocked.TurnEndedObserved && blocked.After.TurnNumber == 2
                && blocked.EventBatch.Events.Any(e => e.Type == GameEventType.ActionBlocked),
                "P9.3 class-blocked prepared skill reaches the engine and consumes its normal turn.");
            Assert(f.Progress.Experience == 0 && f.Resonance.Active.Current == 20 && f.Enemy.Stats.CurrentHealth == 100 && f.Passive.Costs == 0,
                "P9.3 no skill spending/effect/XP occurs for engine authorization rejection.");
            state.MasteryProfile.IncreaseProgress(f.Skill.Id, 100, state.SkillRegistry);
            owner.AdvanceEnemyTurn();
            var retained = owner.SubmitIntent(CombatIntent.Skill(f.Skill.Id));
            AssertCompletedIntent(retained);
            Assert(f.Progress.Experience == 10 && f.Resonance.Active.Current == 18,
                "P9.3 existing retained mastery authorization is observed through the same engine.");
            var locked = new EncounterFixture(requiredUnlock: "locked");
            var lockedOwner = CreateProjectionOwner(locked);
            lockedOwner.Start();
            var unlockBlock = lockedOwner.SubmitIntent(CombatIntent.Skill(locked.Skill.Id));
            AssertCompletedIntent(unlockBlock);
            Assert(unlockBlock.TurnEndedObserved && locked.Progress.Experience == 0
                && unlockBlock.EventBatch.Events.Any(e => e.Type == GameEventType.ActionBlocked),
                "P9.3 canonical unlock rejection is not converted into a free admission failure.");
        }

        private static void AssertIntentRefillAndLethality()
        {
            var f = new EncounterFixture();
            var owner = CreateProjectionOwner(f);
            owner.Start();
            f.Resonance.TrySpendActive(8);
            var filled = owner.SubmitIntent(CombatIntent.Refill(4));
            AssertCompletedIntent(filled);
            Assert(filled.RefillResult == true && filled.TurnStartedObserved && filled.TurnEndedObserved
                && filled.After.TurnNumber == 2 && f.Resonance.Active.Current == 16 && f.Resonance.Reserve.Current == 46,
                "P9.3 refill delegates one transfer and the shared turn lifecycle to the existing action.");
            Assert(filled.EventBatch.Events.Select(e => e.Type).SequenceEqual(new[] { GameEventType.TurnStarted,
                GameEventType.ResourceSpent, GameEventType.ResourceRegenerated, GameEventType.TurnEnded }),
                "P9.3 refill preserves exact existing event ordering.");
            foreach (int amount in new[] { 0, -1, int.MinValue })
            {
                owner.AdvanceEnemyTurn();
                var noop = owner.SubmitIntent(CombatIntent.Refill(amount));
                AssertCompletedIntent(noop);
                Assert(noop.RefillResult == true && noop.TurnEndedObserved && f.Resonance.Active.Current == 16
                    && f.Resonance.Reserve.Current == 46, "P9.3 nonpositive refill keeps existing no-transfer consumed-turn semantics.");
            }
            owner.AdvanceEnemyTurn();
            owner.SubmitIntent(CombatIntent.Refill(int.MaxValue));
            owner.AdvanceEnemyTurn();
            var full = owner.SubmitIntent(CombatIntent.Refill(10));
            Assert(full.RefillResult == true && full.TurnEndedObserved && f.Resonance.Active.Current == 20 && f.Resonance.Reserve.Current == 42,
                "P9.3 full-pool no-op and oversized refill preserve domain clamping.");

            var denied = new EncounterFixture();
            var deniedOwner = CreateProjectionOwner(denied);
            deniedOwner.Start();
            denied.Resonance.TrySpendActive(10);
            denied.Resonance.Reserve.TryConsume(50);
            var denial = deniedOwner.SubmitIntent(CombatIntent.Refill(4));
            AssertCompletedIntent(denial);
            Assert(denial.RefillResult == false && !denial.TurnStartedObserved && !denial.TurnEndedObserved
                && denial.Before.TurnNumber == denial.After.TurnNumber
                && denial.EventBatch.Events.Single().Type == GameEventType.ActionBlocked,
                "P9.3 reserve denial is a completed invocation but no started/completed turn.");

            var late = new EncounterFixture();
            var lateOwner = CreateProjectionOwner(late);
            lateOwner.Start();
            late.Resonance.TrySpendActive(10);
            late.Player.Debuffs.Add(new IntentBoundaryDebuff(context => late.Resonance.Reserve.TryConsume(50)),
                late.Player, new CombatContext(late.Player, late.Enemy, late.Log));
            var lateDenial = lateOwner.SubmitIntent(CombatIntent.Refill(4));
            AssertCompletedIntent(lateDenial);
            Assert(lateDenial.RefillResult == false && lateDenial.TurnStartedObserved && lateDenial.TurnEndedObserved
                && lateDenial.After.TurnNumber == 2 && late.Resonance.Active.Current == 10,
                "P9.3 post-hook refill denial has the same bool but different actual lifecycle facts.");

            foreach (bool refill in new[] { false, true })
            {
                var lethal = new EncounterFixture();
                var lethalOwner = CreateProjectionOwner(lethal);
                lethalOwner.Start();
                lethal.Resonance.TrySpendActive(8);
                lethal.Player.Debuffs.Add(new PoisonDebuff(2, 200), lethal.Player,
                    new CombatContext(lethal.Player, lethal.Enemy, lethal.Log));
                var result = lethalOwner.SubmitIntent(refill ? CombatIntent.Refill(4) : CombatIntent.Skill(lethal.Skill.Id));
                AssertCompletedIntent(result);
                Assert(result.EncounterIsTerminal == true && result.TurnStartedObserved && !result.TurnEndedObserved
                    && result.After.TurnNumber == 1 && result.After.State == CombatEncounterState.Terminal,
                    "P9.3 lethal start is terminal even without counter advance or TurnEnded.");
                Assert(lethal.Resonance.Active.Current == 12 && lethal.Resonance.Reserve.Current == 50
                    && lethal.Progress.Experience == 0 && (refill ? result.RefillResult == false : result.RefillResult == null),
                    "P9.3 lethal start prevents action effects and spending through the existing lifecycle.");
            }
            var lethalEnd = new EncounterFixture();
            var endOwner = CreateProjectionOwner(lethalEnd);
            endOwner.Start();
            lethalEnd.Resonance.TrySpendActive(8);
            lethalEnd.Player.StatusEffects.Apply(new StatusEffectInstance(StatusEffectType.Poison, 2, false, 200));
            var end = endOwner.SubmitIntent(CombatIntent.Refill(4));
            AssertCompletedIntent(end);
            Assert(end.RefillResult == true && end.EncounterIsTerminal == true && !end.TurnEndedObserved
                && end.After.TurnNumber == 1 && lethalEnd.Resonance.Active.Current == 16,
                "P9.3 successful refill transfer may end lethally without a TurnEnded event.");
            var victory = new EncounterFixture();
            var victoryOwner = CreateProjectionOwner(victory);
            victoryOwner.Start();
            victory.Enemy.ReceiveDamage(99);
            var kill = victoryOwner.SubmitIntent(CombatIntent.BasicAttack());
            AssertCompletedIntent(kill);
            Assert(kill.EncounterIsTerminal == true && !kill.TurnEndedObserved && kill.After.TurnNumber == 1
                && kill.EventBatch.Events.Last().Type == GameEventType.CombatEnded,
                "P9.3 lethal attack preserves the engine's terminal early return.");
        }

        private static void AssertIntentReentrancyAndFaults()
        {
            var f = new EncounterFixture();
            var probe = new IntentBoundarySkill();
            var owner = CreateIntentProbeOwner(f, probe);
            CombatIntentReceipt startReentry = null;
            f.Passive.StartAction = context => startReentry = owner.SubmitIntent(CombatIntent.BasicAttack());
            owner.Start();
            AssertRejectedIntent(startReentry, CombatIntentAdmissionReason.CommandInFlight);
            Assert(startReentry.Before == null && startReentry.After == null,
                "P9.3 reentry rejects without trying to capture a partial Starting state.");
            var intent = CombatIntent.Skill(probe.Id);
            probe.BeforeApply = context =>
            {
                var before = CaptureProjectionSource(owner, f);
                var reentrant = owner.SubmitIntent(intent);
                AssertRejectedIntent(reentrant, CombatIntentAdmissionReason.CommandInFlight);
                Assert(reentrant.Before == null && reentrant.After == null && CaptureProjectionSource(owner, f) == before,
                    "P9.3 in-flight duplicate executes nothing and does not disturb the outer invocation.");
                ExpectEncounterException<InvalidOperationException>(() => owner.GetSnapshot(), "P9.3 public snapshot cannot leak mid-command state.");
                ExpectEncounterException<InvalidOperationException>(() => owner.ReadEvents(0), "P9.3 event reads wait for command completion.");
                ExpectEncounterException<InvalidOperationException>(() => owner.ReadStartEvents(), "P9.3 start range read is also rejected in-flight.");
                Assert(!owner.Start(), "P9.3 start cannot be reentered from an executing skill.");
            };
            var result = owner.SubmitIntent(intent);
            AssertCompletedIntent(result);
            Assert(probe.Calls == 1 && f.Progress.Experience == 10 && f.Resonance.Active.Current == 18
                && result.EventBatch.Events.Count(e => e.Type == GameEventType.TurnStarted) == 1,
                "P9.3 one admitted call means one apply, one XP grant and one resource spend.");
            probe.BeforeApply = null;
            owner.AdvanceEnemyTurn();
            AssertCompletedIntent(owner.SubmitIntent(intent));
            Assert(probe.Calls == 2 && f.Progress.Experience == 20, "P9.3 a later explicit submission is a new action, not a hidden retry.");

            var fault = new EncounterFixture();
            var throwing = new IntentBoundarySkill();
            var faultOwner = CreateIntentProbeOwner(fault, throwing);
            faultOwner.Start();
            var exception = new InvalidOperationException("original skill failure");
            throwing.BeforeApply = context =>
            {
                context.Attacker.ReceiveDamage(7);
                context.EventLog.Add(GameEventType.PassiveTriggered, "partial mutation");
                throw exception;
            };
            var failed = faultOwner.SubmitIntent(CombatIntent.Skill(throwing.Id));
            Assert(failed.Disposition == CombatIntentDisposition.Faulted && failed.AcceptedForExecution
                && failed.InvocationStarted && !failed.InvocationCompleted, "P9.3 execution fault is not an admission rejection.");
            Assert(ReferenceEquals(faultOwner.ExecutionFailure, exception) && faultOwner.StartFailure == null
                && exception.StackTrace.Contains("ExecuteTurn") && failed.FaultMessage == exception.Message,
                "P9.3 original execution exception/stack is retained separately from start failures.");
            Assert(failed.Before.Player.CurrentHealth == 100 && failed.After.Player.CurrentHealth == 93
                && failed.After.Player.Essence.Current == 33 && failed.After.PlayerResonance.Active.Current == 18
                && failed.After.State == CombatEncounterState.Faulted && fault.Progress.Experience == 0,
                "P9.3 receipt shows partial spending/damage without rollback, XP grant or normal end-turn regeneration.");
            var beforeRetry = CaptureProjectionSource(faultOwner, fault);
            AssertRejectedIntent(faultOwner.SubmitIntent(intent), CombatIntentAdmissionReason.EncounterFaulted);
            Assert(!faultOwner.Start() && throwing.Calls == 1 && CaptureProjectionSource(faultOwner, fault) == beforeRetry,
                "P9.3 faulted owner rejects later work without automatically retrying.");
            Assert(JsonSerializer.Serialize(faultOwner.ReadEvents(failed.EventBatch.StartIndex)) == JsonSerializer.Serialize(failed.EventBatch),
                "P9.3 fault-time events remain recoverable without another execution.");

            var startFault = new EncounterFixture();
            var startFaultOwner = CreateProjectionOwner(startFault);
            startFault.Passive.StartAction = context => { context.EventLog.Add(GameEventType.PassiveTriggered, "start fault"); throw exception; };
            ExpectEncounterException<InvalidOperationException>(() => startFaultOwner.Start(), "P9.3 existing Start exception contract remains unchanged.");
            AssertRejectedIntent(startFaultOwner.SubmitIntent(CombatIntent.BasicAttack()), CombatIntentAdmissionReason.EncounterFaulted);
            Assert(startFaultOwner.ReadStartEvents().Events.Single().Message == "start fault"
                && startFaultOwner.ExecutionFailure == null, "P9.3 start-fault events have their own recoverable range.");
        }

        private static void AssertIntentObservationFailure()
        {
            foreach (int phase in new[] { 0, 1, 2 })
            {
                var f = new EncounterFixture();
                var probe = new IntentBoundarySkill();
                var owner = CreateIntentProbeOwner(f, probe);
                owner.Start();
                var observationError = new InvalidOperationException("unreadable effect diagnostic");
                var effect = new IntentBoundaryDebuff(null) { ObservationError = observationError };
                f.Player.Debuffs.Add(effect, f.Player, new CombatContext(f.Player, f.Enemy, f.Log));
                var original = new InvalidOperationException("execution before observation failure");
                if (phase == 0) effect.BrokenObservation = true;
                probe.BeforeApply = context =>
                {
                    effect.BrokenObservation = true;
                    if (phase == 1) throw original;
                };
                var result = owner.SubmitIntent(CombatIntent.Skill(probe.Id));
                Assert(result.Disposition == CombatIntentDisposition.Faulted && result.After == null
                    && result.ObservationFailureMessage == observationError.Message,
                    "P9.3 inability to project corrupt partial state is explicit, never a stale after snapshot.");
                Assert(result.InvocationStarted == (phase != 0) && result.InvocationCompleted == (phase == 2),
                    "P9.3 receipt distinguishes pre-read, execution and post-read failures.");
                Assert(ReferenceEquals(owner.ExecutionFailure, phase == 1 ? original : observationError),
                    "P9.3 observation failure cannot overwrite an earlier execution exception.");
                int count = f.Log.Events.Count;
                AssertRejectedIntent(owner.SubmitIntent(CombatIntent.BasicAttack()), CombatIntentAdmissionReason.EncounterFaulted);
                Assert(owner.ReadEvents(0).EndIndex == count && f.Log.Events.Count == count && probe.Calls == (phase == 0 ? 0 : 1),
                    "P9.3 event recovery remains available when state projection cannot complete.");
            }
        }

        private static void AssertIntentValueSurface()
        {
            Assert(typeof(CombatIntent).GetProperties().All(p => p.SetMethod == null
                && (p.PropertyType == typeof(string) || p.PropertyType == typeof(int) || p.PropertyType == typeof(CombatIntentType)))
                && typeof(CombatIntent).GetConstructors().Length == 0,
                "P9.3 typed intent factories accept values only; foreign actors/targets are not representable.");
            foreach (var type in new[] { typeof(CombatIntentReceipt), typeof(CombatEventBatch), typeof(CombatEventSnapshot) })
            {
                Assert(type.IsSealed && type.GetProperties().All(p => p.SetMethod == null), "P9.3 receipts and event values are immutable.");
                Assert(type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).All(field => field.IsInitOnly
                    && field.FieldType != typeof(GameEvent) && field.FieldType != typeof(Exception)
                    && field.FieldType != typeof(CombatRuntimeContext)), "P9.3 detached result graph does not retain domain events, exceptions or runtimes.");
            }
        }

        private static void AssertCompletedIntent(CombatIntentReceipt result)
        {
            Assert(result.Disposition == CombatIntentDisposition.Completed && result.AcceptedForExecution
                && result.InvocationStarted && result.InvocationCompleted && result.AdmissionReason == CombatIntentAdmissionReason.None,
                "P9.3 completed invocation facts do not claim hit/effect success.");
        }

        private static void AssertRejectedIntent(CombatIntentReceipt result, CombatIntentAdmissionReason reason)
        {
            Assert(result.Disposition == CombatIntentDisposition.AdmissionRejected && result.AdmissionReason == reason
                && !result.AcceptedForExecution && !result.InvocationStarted && !result.InvocationCompleted,
                "P9.3 rejection exposes the stable admission reason: " + reason);
            Assert(result.EventBatch.StartIndex == result.EventBatch.EndIndex && result.EventBatch.Events.Count == 0
                && !result.TurnStartedObserved && !result.TurnEndedObserved,
                "P9.3 rejection returns an empty range, never preceding unobserved events.");
        }

        private static void AssertEventBatchMatchesLog(CombatEventBatch batch, GameEventLog log)
        {
            Assert(batch.Events.Count == batch.EndIndex - batch.StartIndex, "P9.3 half-open range matches copied membership.");
            for (int i = 0; i < batch.Events.Count; i++)
            {
                var item = batch.Events[i];
                Assert(item.Index == batch.StartIndex + i && item.Type == log.Events[item.Index].Type
                    && item.Message == log.Events[item.Index].Message, "P9.3 event order/index/type/message match the authoritative log.");
            }
        }

        private static CombatEncounterComposition CreateIntentProbeOwner(EncounterFixture f, ICombatSkill skill)
        {
            return new CombatEncounterComposition(f.Session, f.Profile,
                new CombatEncounterPreparation(f.Player, f.Enemy, f.Log, f.Random, f.Passives,
                    new[] { new PreparedCombatSkillBinding(skill, f.Progress) }, f.Resonance));
        }

        private sealed class IntentBoundarySkill : ICombatSkill
        {
            public string Id { get { return "skill_balance_ritual"; } }
            public string Name { get { return "test wrapper"; } }
            public int Calls { get; private set; }
            public Action<CombatContext> BeforeApply;
            public void Apply(CombatContext context, SkillProgress progress)
            {
                Calls++;
                BeforeApply?.Invoke(context);
                new BalanceRitualSkill().Apply(context, progress);
            }
        }

        private sealed class IntentBoundaryDebuff : IDebuff
        {
            private readonly Action<CombatContext> before;
            public bool BrokenObservation;
            public Exception ObservationError;
            public string Name { get { if (BrokenObservation) throw ObservationError; return "test hook"; } }
            public DebuffType Type { get { return DebuffType.ActionBlock; } }
            public int RemainingTurns { get { return 10; } }
            public bool IsExpired { get { return false; } }
            public IntentBoundaryDebuff(Action<CombatContext> before) { this.before = before; }
            public void OnTurnStart(CombatEntity target, CombatContext context) { }
            public void OnBeforeAction(CombatContext context) { before?.Invoke(context); }
            public void Tick() { }
        }
    }
}
