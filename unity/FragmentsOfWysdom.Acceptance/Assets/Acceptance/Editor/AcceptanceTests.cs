using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text.Json;
using Game.Application.Runtime;
using Game.Domain.Classes;
using Game.Domain.Common;
using Game.Domain.Progress;
using Game.PresentationBoundary.SmokeTests;
using NUnit.Framework;

namespace FragmentsOfWysdom.Acceptance
{
    [TestFixture]
    public sealed class AcceptanceTests
    {
        [OneTimeSetUp]
        public void EnvironmentGate()
        {
#if UNITY_EDITOR
            AcceptanceEnvironment.CaptureAndValidate();
#else
            TestContext.WriteLine("HOST CHECK ONLY: Unity Editor APIs/import/Mono remain UNVERIFIED.");
#endif
        }

        [Test]
        public void BackendIdentityAndPublicBoundary()
        {
            var assembly = typeof(CombatEncounterComposition).Assembly;
            Assert.That(assembly.GetName().Name, Is.EqualTo("FragmentsOfWisdom.Game"));
            Assert.That(assembly.GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName,
                Is.EqualTo(".NETStandard,Version=v2.1"));
            Assert.That(assembly.GetCustomAttributes<InternalsVisibleToAttribute>()
                .Any(a => a.AssemblyName.Split(',')[0] == GetType().Assembly.GetName().Name), Is.False);
            Assert.That(typeof(CombatEncounterComposition).GetProperties().Select(p => p.Name).OrderBy(n => n),
                Is.EqualTo(new[] { "ExecutionFailure", "StartFailure", "State" }));
            TestContext.WriteLine(assembly.FullName);
            TestContext.WriteLine(typeof(JsonSerializer).Assembly.FullName);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PublicEncounterThroughTerminal(bool enemyWins)
        {
            var owner = TrustedEncounterAssembly.Create(enemyWins ? Scenario.EnemyVictory : Scenario.Normal, out _);
            var view = new PresentationConsumer(owner);
            Assert.That(view.Flow.Phase, Is.EqualTo(CombatTurnFlowPhase.NotStarted));
            view.Start();
            Assert.That(view.Flow.Phase, Is.EqualTo(CombatTurnFlowPhase.AwaitingPlayer));
            Assert.That(view.Displayed.Select(e => e.Type),
                Is.EqualTo(new[] { GameEventType.PassiveTriggered, GameEventType.CombatStarted }));
            var initial = view.Snapshot;
            string initialJson = JsonSerializer.Serialize(initial);
            Assert.That(initial.PreparedSkills.Single().SkillId, Is.EqualTo(TrustedEncounterAssembly.SkillId));
            int actions = 0;
            bool skillObserved = false;
            bool refillObserved = false;
            while (!view.Flow.IsTerminal && actions < 40)
            {
                var intent = actions == 1 ? CombatIntent.Skill(TrustedEncounterAssembly.SkillId)
                    : actions == 2 ? CombatIntent.Refill(4) : CombatIntent.BasicAttack();
                int cursor = view.Cursor;
                var receipt = view.Submit(intent);
                Assert.That(receipt.InvocationCompleted && receipt.OpportunityConsumed, Is.True);
                Assert.That(receipt.EventBatch.StartIndex, Is.EqualTo(cursor));
                if (actions == 1)
                {
                    Assert.That(receipt.EventBatch.Events.Any(e => e.Type == GameEventType.SkillUsed), Is.True);
                    Assert.That(receipt.After.PreparedSkills[0].Experience, Is.EqualTo(10));
                    Assert.That(receipt.After.PlayerResonance.Active.Current, Is.EqualTo(18));
                    skillObserved = true;
                }
                if (actions == 2)
                {
                    Assert.That(receipt.RefillResult, Is.EqualTo(true));
                    Assert.That(receipt.After.PlayerResonance.Active.Current, Is.EqualTo(20));
                    refillObserved = true;
                }
                actions++;
                if (view.Flow.IsTerminal) break;
                Assert.That(view.Flow.Phase, Is.EqualTo(CombatTurnFlowPhase.AwaitingEnemy));
                Rejected(view.Submit(CombatIntent.BasicAttack()), CombatIntentAdmissionReason.NotPlayerOpportunity);
                cursor = view.Cursor;
                var enemy = view.AdvanceEnemy();
                Assert.That(enemy.InvocationCompleted && enemy.OpportunityConsumed, Is.True);
                Assert.That(enemy.ActorSide, Is.EqualTo(CombatActorSide.Enemy));
                Assert.That(enemy.EventBatch.StartIndex, Is.EqualTo(cursor));
                if (!view.Flow.IsTerminal) Assert.That(view.Flow.Phase, Is.EqualTo(CombatTurnFlowPhase.AwaitingPlayer));
            }
            Assert.That(skillObserved && refillObserved && view.Flow.IsTerminal, Is.True);
            Assert.That(view.Snapshot.Enemy.IsAlive, Is.EqualTo(enemyWins));
            Assert.That(view.Snapshot.Player.IsAlive, Is.EqualTo(!enemyWins));
            string terminal = Fingerprint(owner);
            Rejected(view.Submit(CombatIntent.BasicAttack()), CombatIntentAdmissionReason.EncounterTerminal);
            Rejected(view.AdvanceEnemy(), CombatIntentAdmissionReason.EncounterTerminal);
            Assert.That(Fingerprint(owner), Is.EqualTo(terminal));
            Assert.That(JsonSerializer.Serialize(initial), Is.EqualTo(initialJson), "Detached old snapshot changed.");
            Events(owner, view);
            foreach (var line in view.Transcript) TestContext.WriteLine(line);
        }

        [Test]
        public void LostReceiptRecoveryDoesNotReplayOrDuplicate()
        {
            var owner = TrustedEncounterAssembly.Create(Scenario.Normal, out _);
            var view = new PresentationConsumer(owner);
            view.Start(false);
            var player = view.Submit(CombatIntent.BasicAttack(), false);
            view.Accept(player);
            view.AdvanceEnemy(false);
            string after = Fingerprint(owner);
            view.Recover();
            view.Accept(player);
            view.Recover();
            Assert.That(Fingerprint(owner), Is.EqualTo(after));
            Events(owner, view);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void OriginalFaultPreservesPartialStateWithoutRetry(bool enemyFault)
        {
            var owner = TrustedEncounterAssembly.Create(enemyFault ? Scenario.EnemyFault : Scenario.PlayerFault, out var error);
            var view = new PresentationConsumer(owner);
            view.Start();
            if (enemyFault) view.Submit(CombatIntent.Refill(0));
            var receipt = enemyFault ? view.AdvanceEnemy(false) : view.Submit(CombatIntent.BasicAttack(), false);
            Assert.That(receipt.Disposition, Is.EqualTo(CombatIntentDisposition.Faulted));
            Assert.That(receipt.InvocationStarted && !receipt.InvocationCompleted && receipt.OpportunityConsumed, Is.True);
            Assert.That(owner.ExecutionFailure, Is.SameAs(error));
            Assert.That(error.StackTrace, Does.Contain("ExecuteTurn"));
            var before = enemyFault ? receipt.Before.Enemy : receipt.Before.Player;
            var after = enemyFault ? receipt.After.Enemy : receipt.After.Player;
            Assert.That(after.CurrentHealth, Is.EqualTo(before.CurrentHealth - 7));
            Assert.That(after.Essence.Current, Is.EqualTo(before.Essence.Current - 3));
            Assert.That(receipt.After.PreparedSkills[0].Experience, Is.Zero);
            string state = Fingerprint(owner);
            Rejected(view.Submit(CombatIntent.BasicAttack()), CombatIntentAdmissionReason.EncounterFaulted);
            Rejected(view.AdvanceEnemy(), CombatIntentAdmissionReason.EncounterFaulted);
            view.Recover();
            Assert.That(Fingerprint(owner), Is.EqualTo(state));
            Assert.That(view.Flow.IsFaulted, Is.True);
            Events(owner, view);
        }

        [Test]
        public void ProductionJsonSnapshotRoundTripAndLegacyDefaults()
        {
            // Real production serializer, including its reflection-backed nested DTOs.
            var snapshot = new V2ProgressSnapshot(V2ProgressSnapshotVersions.CurrentSchemaVersion,
                Array.Empty<V2CodexEntrySnapshot>(), Array.Empty<V2MissionLeadSnapshot>(),
                Array.Empty<V2ActiveMissionSnapshot>(), Array.Empty<V2UnlockSnapshot>(),
                new V2ClassProfileSnapshot("class.acceptance"),
                new[] { new V2MasterySnapshot("skill.acceptance", 100, MasteryRank.Mastered) },
                new[] { new V2MentorRelationshipSnapshot("mentor.acceptance", true, 17, false) },
                new V2SkillTreeProgressSnapshot(new[] { new V2SkillTreeProgressScopeSnapshot("tree.acceptance",
                    new[] { "root", "child" }, new[] { "root" }) }),
                new[] { new V2ClassSkillTreeBindingSnapshot("class.acceptance", "tree.acceptance") },
                new[] { "codex.acceptance" }, new[] { "codex.acceptance" });
            var serializer = new V2ProgressSnapshotJsonSerializer();
            string json = serializer.Serialize(snapshot);
            var restored = serializer.Deserialize(json);
            Assert.That(restored, Is.Not.SameAs(snapshot));
            Assert.That(serializer.Serialize(restored), Is.EqualTo(json));
            Assert.That(restored.ClassProfile.CurrentClassId, Is.EqualTo("class.acceptance"));
            Assert.That(restored.MasteryRecords.Single().Progress, Is.EqualTo(100));
            Assert.That(restored.MasteryRecords.Single().Rank, Is.EqualTo(MasteryRank.Mastered));
            Assert.That(restored.MentorRelationships.Single().Trust, Is.EqualTo(17));
            Assert.That(restored.MentorRelationships.Single().IsAvailable, Is.False);
            Assert.That(restored.SkillTreeProgress.Scopes.Single().UnlockedNodeIds, Is.EqualTo(new[] { "root" }));
            Assert.That(restored.CodexCompletedEntryIds, Is.EqualTo(new[] { "codex.acceptance" }));
            var legacy = serializer.Deserialize("{\"SchemaVersion\":1}");
            Assert.That(legacy.MasteryRecords, Is.Empty);
            Assert.That(legacy.SkillTreeProgress.Scopes, Is.Empty);
            TestContext.WriteLine("Production snapshot JSON round trip completed; no gameplay restore or schema change.");
        }

        private static string Fingerprint(CombatEncounterComposition owner)
        {
            return JsonSerializer.Serialize(new { Snapshot = owner.GetSnapshot(), Flow = owner.GetTurnFlowSnapshot(), Events = owner.ReadEvents(0) });
        }

        private static void Events(CombatEncounterComposition owner, PresentationConsumer view)
        {
            var log = owner.ReadEvents(0);
            Assert.That(view.Cursor, Is.EqualTo(log.EndIndex));
            Assert.That(view.Displayed.Select(e => e.Index), Is.EqualTo(Enumerable.Range(0, log.EndIndex)));
            Assert.That(JsonSerializer.Serialize(view.Displayed), Is.EqualTo(JsonSerializer.Serialize(log.Events)));
        }

        private static void Rejected(CombatIntentReceipt receipt, CombatIntentAdmissionReason reason)
        {
            Assert.That(receipt.Disposition, Is.EqualTo(CombatIntentDisposition.AdmissionRejected));
            Assert.That(receipt.AdmissionReason, Is.EqualTo(reason));
            Assert.That(receipt.InvocationStarted || receipt.OpportunityConsumed, Is.False);
            Assert.That(receipt.EventBatch.Events, Is.Empty);
        }
    }
}
