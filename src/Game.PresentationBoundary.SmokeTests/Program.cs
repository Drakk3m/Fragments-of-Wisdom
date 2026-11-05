using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Game.Application.Runtime;
using Game.Domain.Common;

namespace Game.PresentationBoundary.SmokeTests
{
    internal static class Program
    {
        private static int assertions;

        private static int Main()
        {
            try
            {
                VerifyPublicSurface();
                var normal = CompleteEncounter(Scenario.Normal, false, true);
                var observed = CompleteEncounter(Scenario.Normal, true, false);
                Check(normal == observed, "Additional observations leave the entire deterministic RNG-driven outcome and event stream unchanged.");
                CompleteEncounter(Scenario.EnemyVictory, false, true);
                VerifyRecovery();
                VerifyBlockersAndRefill();
                VerifyOptionalResources();
                VerifyFault(Scenario.PlayerFault);
                VerifyFault(Scenario.EnemyFault);
                Console.WriteLine("External Presentation Boundary assertions: " + assertions);
                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception);
                Console.Error.WriteLine("Assertions passed before failure: " + assertions);
                return 1;
            }
        }

        private static string CompleteEncounter(Scenario scenario, bool noisyReads, bool report)
        {
            var owner = TrustedEncounterAssembly.Create(scenario, out _);
            var view = new PresentationConsumer(owner);
            Check(view.Flow.Phase == CombatTurnFlowPhase.NotStarted && view.Snapshot.State == CombatEncounterState.Prepared,
                "Public construction yields a prepared encounter without starting it.");
            view.Start();
            var initial = view.Snapshot;
            string initialValues = JsonSerializer.Serialize(initial);
            Check(view.Flow.Phase == CombatTurnFlowPhase.AwaitingPlayer && view.Cursor == 2
                && view.Displayed.Select(e => e.Type).SequenceEqual(new[] { GameEventType.PassiveTriggered, GameEventType.CombatStarted }),
                "Start events are processed before action, without parsing misleading display text.");
            Check(initial.PreparedSkills.Single().SkillId == TrustedEncounterAssembly.SkillId
                && initial.Player.Essence.Current == 40 && initial.PlayerResonance.Active.Current == 20,
                "Canonical prepared skill and initial resources are publicly observable.");
            Immutable(initial.PreparedSkills);
            Immutable(initial.Player.StatusEffects);
            var rejected = view.Submit(CombatIntent.Skill("unknown"));
            CheckRejected(rejected, CombatIntentAdmissionReason.SkillNotPrepared);
            Check(view.Cursor == 2 && view.Flow.Phase == CombatTurnFlowPhase.AwaitingPlayer, "Invalid skill preserves cursor/opportunity.");

            CombatIntentReceipt first = null;
            string firstValues = null;
            int playerActions = 0;
            int enemyActions = 0;
            for (int safety = 0; safety < 40 && !view.Flow.IsTerminal; safety++)
            {
                if (noisyReads)
                {
                    view.Observe();
                    view.Observe();
                    owner.ReadEvents(0);
                    owner.ReadStartEvents();
                }
                Check(view.Flow.Phase == CombatTurnFlowPhase.AwaitingPlayer, "Flow, not TurnNumber, controls player submission.");
                var intent = playerActions == 1 ? CombatIntent.Skill(TrustedEncounterAssembly.SkillId)
                    : playerActions == 2 ? CombatIntent.Refill(4) : CombatIntent.BasicAttack();
                int cursor = view.Cursor;
                var player = view.Submit(intent);
                playerActions++;
                Check(player.InvocationCompleted && player.OpportunityConsumed && player.ActorSide == CombatActorSide.Player
                    && player.FlowBefore.Phase == CombatTurnFlowPhase.AwaitingPlayer && player.EventBatch.StartIndex == cursor,
                    "Player invocation is consumed once with a contiguous range.");
                if (first == null) { first = player; firstValues = JsonSerializer.Serialize(first); Immutable(first.EventBatch.Events); }
                if (playerActions == 2)
                    Check(view.Snapshot.PreparedSkills[0].Experience == 10 && view.Snapshot.PlayerResonance.Active.Current == 18
                        && player.EventBatch.Events.Any(e => e.Type == GameEventType.SkillUsed), "Canonical skill executes with one XP grant and observed Resonance spend.");
                if (scenario == Scenario.Normal && playerActions == 1)
                    Check(view.Snapshot.Player.CurrentHealth == 99 && view.Snapshot.Player.StatusEffects.Single().RemainingDuration == 2,
                        "Existing poison lifecycle updates detached HP and status-duration facts.");
                if (view.Flow.IsTerminal) break;
                Check(view.Flow.Phase == CombatTurnFlowPhase.AwaitingEnemy, "Consumed player action leaves enemy pending, not automatically executed.");
                var beforeEnemy = Fingerprint(owner);
                int pendingCursor = view.Cursor;
                CheckRejected(view.Submit(CombatIntent.BasicAttack()), CombatIntentAdmissionReason.NotPlayerOpportunity);
                Check(view.Cursor == pendingCursor && Fingerprint(owner) == beforeEnemy, "Player input during enemy opportunity changes no events or gameplay.");
                var enemy = view.AdvanceEnemy();
                enemyActions++;
                Check(enemy.ActorSide == CombatActorSide.Enemy && enemy.InvocationCompleted && enemy.OpportunityConsumed
                    && enemy.EventBatch.StartIndex == pendingCursor && enemy.FlowBefore.Phase == CombatTurnFlowPhase.AwaitingEnemy,
                    "Enemy executes only by explicit public advancement.");
                if (view.Flow.IsTerminal) break;
                Check(view.Flow.Phase == CombatTurnFlowPhase.AwaitingPlayer, "Nonterminal enemy completion returns control to player.");
                beforeEnemy = Fingerprint(owner);
                CheckRejected(view.AdvanceEnemy(), CombatIntentAdmissionReason.NoEnemyOpportunity);
                Check(Fingerprint(owner) == beforeEnemy, "Duplicate enemy advancement invokes nothing.");
            }
            Check(view.Flow.IsTerminal && playerActions >= 3 && enemyActions >= 2, "Encounter reaches a legitimate terminal outcome through multiple cycles.");
            Check(scenario == Scenario.Normal ? view.Snapshot.Player.IsAlive && !view.Snapshot.Enemy.IsAlive
                : !view.Snapshot.Player.IsAlive && view.Snapshot.Enemy.IsAlive, "Expected winner comes from observed alive facts, never message parsing.");
            var terminal = view.Snapshot;
            var terminalValues = Fingerprint(owner);
            CheckRejected(view.Submit(CombatIntent.BasicAttack()), CombatIntentAdmissionReason.EncounterTerminal);
            CheckRejected(view.AdvanceEnemy(), CombatIntentAdmissionReason.EncounterTerminal);
            Check(Fingerprint(owner) == terminalValues && JsonSerializer.Serialize(terminal) == JsonSerializer.Serialize(view.Snapshot),
                "Terminal owner permits no phantom retaliation or state change.");
            Check(JsonSerializer.Serialize(initial) == initialValues && JsonSerializer.Serialize(first) == firstValues,
                "Old nested snapshots, flow values and batches remain detached through terminal combat.");
            CheckEvents(owner, view);
            if (report)
            {
                Console.WriteLine("Complete workflow: " + scenario);
                foreach (var line in view.Transcript) Console.WriteLine("  " + line);
            }
            return terminalValues;
        }

        private static void VerifyRecovery()
        {
            var owner = TrustedEncounterAssembly.Create(Scenario.Normal, out _);
            var view = new PresentationConsumer(owner);
            view.Start(false);
            CheckRejected(view.Submit(null), CombatIntentAdmissionReason.NullIntent);
            Check(view.Cursor == 0 && view.Displayed.Count == 0, "Rejected receipt does not acknowledge unobserved start prefix.");
            var player = view.Submit(CombatIntent.BasicAttack(), false);
            view.Accept(player);
            Check(view.Displayed[0].Type == GameEventType.PassiveTriggered && view.Cursor == player.EventBatch.EndIndex,
                "Missing start prefix is recovered before delivering the player's range.");
            int cursor = view.Cursor;
            view.AdvanceEnemy(false); // Receipt lost; never replay the action to recover it.
            var afterLost = Fingerprint(owner);
            view.Recover();
            Check(view.Cursor > cursor && view.Flow.Phase == CombatTurnFlowPhase.AwaitingPlayer && Fingerprint(owner) == afterLost,
                "Lost enemy receipt recovery observes log and current state without replaying gameplay.");
            cursor = view.Cursor;
            var skill = view.Submit(CombatIntent.Skill(TrustedEncounterAssembly.SkillId), false);
            var beforeFailure = Fingerprint(owner);
            int shown = view.Displayed.Count;
            Throws<InvalidOperationException>(() => view.Accept(skill, 1), "Partial staging failure propagates before display acknowledgement.");
            Check(view.Cursor == cursor && view.Displayed.Count == shown && Fingerprint(owner) == beforeFailure,
                "Processing failure leaves cursor and presented events unchanged; gameplay is not rolled back or retried.");
            view.Recover();
            Check(view.Cursor == skill.EventBatch.EndIndex && view.Flow.Phase == CombatTurnFlowPhase.AwaitingEnemy,
                "Unacknowledged range is replayable after processing failure.");
            view.Accept(skill); // Late duplicate receipt must not redisplay already acknowledged indices.
            CheckEvents(owner, view);
            var historical = owner.ReadEvents(player.EventBatch.StartIndex, player.EventBatch.EndIndex);
            Check(JsonSerializer.Serialize(historical) == JsonSerializer.Serialize(player.EventBatch)
                && Fingerprint(owner) == beforeFailure, "Historical reads and duplicate receipts replay values, not engine execution.");
            Console.WriteLine("Recovery: missing prefix, lost enemy receipt, staged-processing failure and historical replay PASS; cursor=" + view.Cursor);
        }

        private static void VerifyBlockersAndRefill()
        {
            foreach (var scenario in new[] { Scenario.ClassBlocked, Scenario.Unmastered, Scenario.Retained,
                Scenario.EarlyRefillDenial, Scenario.LateRefillDenial })
            {
                var owner = TrustedEncounterAssembly.Create(scenario, out _);
                var view = new PresentationConsumer(owner);
                view.Start();
                bool refill = scenario == Scenario.EarlyRefillDenial || scenario == Scenario.LateRefillDenial;
                var result = view.Submit(refill ? CombatIntent.Refill(4) : CombatIntent.Skill(TrustedEncounterAssembly.SkillId));
                bool early = scenario == Scenario.EarlyRefillDenial;
                Check(result.InvocationCompleted && result.OpportunityConsumed == !early
                    && view.Flow.Phase == (early ? CombatTurnFlowPhase.AwaitingPlayer : CombatTurnFlowPhase.AwaitingEnemy),
                    "Existing blocked/retained/refill lifecycle determines opportunity: " + scenario);
                if (refill)
                    Check(result.RefillResult == false && result.After.PlayerResonance.Active.Current == 12
                        && result.TurnStartedObserved == !early, "Early/late denied transfer preserves bool and differing lifecycle facts.");
                else
                    Check(result.After.PreparedSkills[0].Experience == (scenario == Scenario.Retained ? 10 : 0)
                        && result.After.PlayerResonance.Active.Current == (scenario == Scenario.Retained ? 18 : 20)
                        && result.EventBatch.Events.Any(e => e.Type == (scenario == Scenario.Retained ? GameEventType.SkillUsed : GameEventType.ActionBlocked)),
                        "Class/mastery gate preserves canonical skill effects/XP/resource ownership.");
                if (early) CheckRejected(view.AdvanceEnemy(), CombatIntentAdmissionReason.NoEnemyOpportunity);
                else Check(view.AdvanceEnemy().InvocationCompleted && view.Flow.Phase == CombatTurnFlowPhase.AwaitingPlayer,
                    "Consumed blocked opportunity still permits the explicit enemy response.");
                CheckEvents(owner, view);
                Console.WriteLine(scenario + ": consumed=" + result.OpportunityConsumed + " after=" + result.FlowAfter.Phase + " PASS");
            }
        }

        private static void VerifyOptionalResources()
        {
            foreach (var scenario in new[] { Scenario.NoResonance, Scenario.ZeroResonance })
            {
                var owner = TrustedEncounterAssembly.Create(scenario, out _);
                var view = new PresentationConsumer(owner);
                view.Start();
                Check(scenario == Scenario.NoResonance ? !view.Snapshot.HasPlayerResonance && view.Snapshot.PlayerResonance == null
                    : view.Snapshot.HasPlayerResonance && view.Snapshot.PlayerResonance.Active.Current == 0 && view.Snapshot.PlayerResonance.Active.Max == 0,
                    "Missing optional resource is distinguishable from a present zero-capacity pool.");
                if (scenario == Scenario.NoResonance) CheckRejected(view.Submit(CombatIntent.Refill(1)), CombatIntentAdmissionReason.ResonanceUnavailable);
            }
        }

        private static void VerifyFault(Scenario scenario)
        {
            var owner = TrustedEncounterAssembly.Create(scenario, out var original);
            var view = new PresentationConsumer(owner);
            view.Start();
            bool enemy = scenario == Scenario.EnemyFault;
            if (enemy) view.Submit(CombatIntent.Refill(0));
            int cursor = view.Cursor;
            var result = enemy ? view.AdvanceEnemy(false) : view.Submit(CombatIntent.BasicAttack(), false);
            Check(result.Disposition == CombatIntentDisposition.Faulted && result.InvocationStarted && !result.InvocationCompleted
                && result.OpportunityConsumed && result.FlowAfter.IsFaulted, "Fault preserves actual invocation/lifecycle facts.");
            Check(ReferenceEquals(owner.ExecutionFailure, original) && original.StackTrace.Contains("ExecuteTurn"),
                "Original configured exception and stack survive through public diagnostic access.");
            var before = enemy ? result.Before.Enemy : result.Before.Player;
            var after = enemy ? result.After.Enemy : result.After.Player;
            Check(after.CurrentHealth == before.CurrentHealth - 7 && after.Essence.Current == before.Essence.Current - 3
                && result.After.PreparedSkills[0].Experience == 0, "Available partial HP/resource state survives without rollback, XP or end-turn regeneration.");
            var fault = Fingerprint(owner);
            CheckRejected(view.Submit(CombatIntent.BasicAttack()), CombatIntentAdmissionReason.EncounterFaulted);
            CheckRejected(view.AdvanceEnemy(), CombatIntentAdmissionReason.EncounterFaulted);
            Check(view.Cursor == cursor, "Post-fault admission rejection does not acknowledge pending fault events.");
            view.Recover();
            Check(view.Flow.IsFaulted && view.Cursor == result.EventBatch.EndIndex && Fingerprint(owner) == fault,
                "Fault events and partial state remain recoverable with no further execution or retry.");
            CheckEvents(owner, view);
            Console.WriteLine(scenario + ": original exception retained; HP=" + after.CurrentHealth + " Essence=" + after.Essence.Current
                + " fault range=[" + result.EventBatch.StartIndex + "," + result.EventBatch.EndIndex + ") cursor=" + view.Cursor + " PASS");
        }

        private static void VerifyPublicSurface()
        {
            // Public metadata inspection only: never reflect internal state or invoke hidden members.
            string name = typeof(Program).Assembly.GetName().Name;
            Check(!typeof(CombatEncounterComposition).Assembly.GetCustomAttributes<InternalsVisibleToAttribute>()
                .Any(a => a.AssemblyName.Split(',')[0] == name), "External executable has no friend-assembly privilege.");
            var types = new[] { typeof(CombatEncounterSnapshot), typeof(CombatantSnapshot), typeof(CombatResourceSnapshot),
                typeof(CombatResonanceSnapshot), typeof(CombatDebuffSnapshot), typeof(CombatStatusEffectSnapshot), typeof(PreparedSkillSnapshot),
                typeof(CombatTurnFlowSnapshot), typeof(CombatIntentReceipt), typeof(CombatEventBatch), typeof(CombatEventSnapshot), typeof(CombatIntent) };
            Check(types.All(t => t.IsSealed && t.GetProperties().All(p => p.SetMethod == null && IsValue(p.PropertyType, types))),
                "Public projection/intent/receipt graph exposes only detached values, not entities, progress, pools, engine or runtime.");
            Check(typeof(CombatEncounterComposition).GetProperties().Select(p => p.Name).OrderBy(n => n)
                .SequenceEqual(new[] { "ExecutionFailure", "StartFailure", "State" }),
                "Consumer owner exposes diagnostics/lifecycle, not mutable runtime/session/engine properties.");
        }

        private static bool IsValue(Type type, Type[] values)
        {
            if (type.IsEnum || type == typeof(string) || type == typeof(int) || type == typeof(bool) || values.Contains(type)) return true;
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)
                || type.GetGenericTypeDefinition() == typeof(IReadOnlyList<>)) && IsValue(type.GetGenericArguments()[0], values);
        }

        private static void Immutable<T>(IReadOnlyList<T> values)
        {
            var list = values as IList<T>;
            Check(list != null && list.IsReadOnly, "Nested collection is a readonly view over detached values.");
            Throws<NotSupportedException>(() => list.Clear(), "Collection mutation through IList is rejected.");
            if (values.Count > 0) Throws<NotSupportedException>(() => list[0] = default(T), "Element replacement is rejected.");
        }

        private static void CheckEvents(CombatEncounterComposition owner, PresentationConsumer view)
        {
            var log = owner.ReadEvents(0);
            Check(view.Cursor == log.EndIndex && view.Displayed.Select(e => e.Index).SequenceEqual(Enumerable.Range(0, log.EndIndex)),
                "Every authoritative index was presented exactly once in chronological order.");
            Check(JsonSerializer.Serialize(view.Displayed) == JsonSerializer.Serialize(log.Events),
                "Displayed type/message/index values exactly match the public authoritative log projection.");
        }

        private static string Fingerprint(CombatEncounterComposition owner)
        {
            return JsonSerializer.Serialize(new { State = owner.GetSnapshot(), Flow = owner.GetTurnFlowSnapshot(), Events = owner.ReadEvents(0) });
        }

        private static void CheckRejected(CombatIntentReceipt result, CombatIntentAdmissionReason reason)
        {
            Check(result.Disposition == CombatIntentDisposition.AdmissionRejected && result.AdmissionReason == reason
                && !result.InvocationStarted && !result.OpportunityConsumed && result.EventBatch.Events.Count == 0,
                "Rejected without execution, consumption or new events: " + reason);
        }

        private static void Throws<T>(Action action, string message) where T : Exception
        {
            try { action(); } catch (T) { Check(true, message); return; }
            Check(false, message);
        }

        private static void Check(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
            assertions++;
        }
    }
}
