using System;
using System.Collections;
using System.Collections.Generic;
using Game.Domain.Classes;
using Game.Domain.Codex;
using Game.Domain.Missions;
using Game.Domain.Progress;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunRestoreResultIntegritySmokeTests()
        {
            AssertRestorePreflightDoesNotReportAppliedState();
            AssertRestorePreflightFailureMatrix();
            AssertRestoreSuccessfulApplicationMatrix();
            AssertRestoreApplicationFailureIsReported();
            AssertRestoreResultConstructorCompatibility();
        }

        private static void AssertRestorePreflightDoesNotReportAppliedState()
        {
            bool allFlagsAccurate = true;
            foreach (bool bindings in new[] { false, true })
            {
                var fixture = new RestoreAtomicityFixture();
                fixture.ClassRegistry.Register(new ClassDefinition("audit.before", "Before", new string[0]));
                fixture.ClassRegistry.Register(new ClassDefinition("audit.after", "After", new string[0]));
                fixture.Classes.AssignClassId("audit.before", fixture.ClassRegistry);
                string before = fixture.CaptureState();
                var snapshot = BuildCompleteV4Snapshot("audit.after", "skill_unknown", MentorId.Lyra, "node_deep_sight");
                var result = fixture.Restore(snapshot, bindings);
                Console.WriteLine("  AUD-12 bindings={0}: class={1}, ClassRestored={2}, MasteryRestored={3}, FullyRestored={4}",
                    bindings, fixture.Classes.CurrentClassId, result.ClassRestored, result.MasteryRestored, result.FullyRestored);
                Assert(fixture.CaptureState() == before,
                    "AUD-12 invalid mastery preflight preserves all populated destination values.");
                Assert(result.MasteryFailureReason == V4ProgressRestoreFailureReason.ValidationFailed
                    && result.MasteryFailureMessage.Contains("MasteryProfile"),
                    "AUD-12 the true mastery failure must remain identifiable.");
                allFlagsAccurate &= !result.FullyRestored && !result.ClassRestored && !result.MasteryRestored
                    && !result.MentorRelationshipsRestored && !result.SkillTreeProgressRestored
                    && !result.ClassSkillTreeBindingsRestored;
            }
            Assert(allFlagsAccurate,
                "AUD-12: rejected preflight must report all applied-state flags false in both public overloads.");
        }

        private static void AssertRestorePreflightFailureMatrix()
        {
            foreach (bool bindings in new[] { false, true })
            {
                for (int failure = 0; failure < 7; failure++)
                {
                    if ((failure == 4 || failure == 6) && !bindings) continue;
                    var fixture = new RestoreAtomicityFixture();
                    var snapshot = BuildCompleteV4Snapshot(
                        failure == 0 || failure == 5 ? "class_unknown" : ClassId.Tlamatini,
                        failure == 1 || failure == 5 ? "skill_unknown" : "skill_deep_sight",
                        failure == 2 ? "mentor_unknown" : MentorId.Lyra,
                        failure == 3 ? "node_unknown" : "node_deep_sight");
                    snapshot = Aud12Snapshot(snapshot, bindings: new[]
                    {
                        new V2ClassSkillTreeBindingSnapshot(failure == 6 ? ClassId.Wanderer : ClassId.Tlamatini,
                            failure == 4 ? "tree_unknown" : SkillTreeId.Tlamatini)
                    });
                    string before = fixture.CaptureState();
                    for (int attempt = 0; attempt < 2; attempt++)
                    {
                        var result = fixture.Restore(snapshot, bindings);
                        Assert(!result.FullyRestored && !result.ClassRestored && !result.MasteryRestored
                            && !result.MentorRelationshipsRestored && !result.SkillTreeProgressRestored
                            && !result.ClassSkillTreeBindingsRestored,
                            "AUD-12 every unapplied subsystem reports false on failed preflight.");
                        Assert(fixture.CaptureState() == before,
                            "AUD-12 failed preflight and retry preserve every populated destination.");
                        Assert(result.ClassSkillTreeBindingsRequested == bindings,
                            "AUD-12 result distinguishes binding overload participation.");
                        var failed = new[] { failure == 0 || failure == 5, failure == 1 || failure == 5,
                            failure == 2, failure == 3, failure == 4 || failure == 6 };
                        var reasons = Aud12Reasons(result);
                        var messages = Aud12Messages(result);
                        string[] names = { "ClassProfile", "MasteryProfile", "MentorRelationshipProfile",
                            "SkillTreeProgressProfile", "ClassSkillTreeBindings" };
                        for (int index = 0; index < 5; index++)
                        {
                            bool requested = index < 4 || bindings;
                            var expected = !requested ? V4ProgressRestoreFailureReason.None
                                : failed[index] ? V4ProgressRestoreFailureReason.ValidationFailed
                                : V4ProgressRestoreFailureReason.NotApplied;
                            Assert(reasons[index] == expected,
                                "AUD-12 invalid data and feasible-but-unapplied state have distinct diagnostics.");
                            Assert(!requested ? messages[index] == ""
                                : messages[index].Contains(names[index]) && messages[index].Contains("preflight"),
                                "AUD-12 diagnostics name the subsystem and phase without hiding the actual failure.");
                        }
                    }
                }
            }
        }

        private static void AssertRestoreSuccessfulApplicationMatrix()
        {
            foreach (bool bindings in new[] { false, true })
            foreach (bool populated in new[] { false, true })
            foreach (int format in new[] { 0, 1, 2 })
            {
                var fixture = new RestoreAtomicityFixture();
                var classes = populated ? fixture.Classes : new ClassProfile();
                var mastery = populated ? fixture.Mastery : new MasteryProfile();
                var mentors = populated ? fixture.Mentors : new MentorRelationshipProfile();
                var trees = populated ? fixture.Trees : new SkillTreeProgressProfile();
                var registry = populated ? fixture.Bindings : BuildTestClassSkillTreeBindingRegistry();
                var serializer = new V2ProgressSnapshotJsonSerializer();
                var snapshot = fixture.Snapshot;
                if (format == 1)
                {
                    var scopes = new List<V2SkillTreeProgressScopeSnapshot>
                    {
                        new V2SkillTreeProgressScopeSnapshot(fixture.Tree.Id,
                            new[] { "node_deep_sight" }, new[] { "node_deep_sight" })
                    };
                    if (bindings)
                        scopes.Add(new V2SkillTreeProgressScopeSnapshot(SkillTreeId.Wanderer,
                            new[] { "node_deep_sight" }, new[] { "node_deep_sight" }));
                    snapshot = Aud12Snapshot(snapshot, tree: new V2SkillTreeProgressSnapshot(scopes));
                }
                snapshot = format == 2 ? serializer.Deserialize("{\"SchemaVersion\":1}")
                    : serializer.Deserialize(serializer.Serialize(snapshot));
                string before = CaptureAud12State(classes, mastery, mentors, trees, registry);
                var coordinator = new V4ProgressRestoreCoordinator();
                Func<V4ProgressRestoreResult> restore = () => bindings
                    ? coordinator.Restore(snapshot, classes, mastery, mentors, trees, fixture.ClassRegistry,
                        fixture.SkillRegistry, fixture.MentorRegistry, fixture.Tree, registry)
                    : coordinator.Restore(snapshot, classes, mastery, mentors, trees, fixture.ClassRegistry,
                        fixture.SkillRegistry, fixture.MentorRegistry, fixture.Tree);
                var result = restore();
                AssertAud12SuccessfulFlags(result, bindings);
                if (format == 2)
                {
                    Assert(CaptureAud12State(classes, mastery, mentors, trees, registry) == before,
                        "AUD-12 legacy missing fields complete as supported no-ops, not erasure.");
                }
                else
                {
                    Assert(classes.CurrentClassId == ClassId.Tlamatini && mastery.GetRecord("skill_deep_sight").Progress == 25,
                        "AUD-12 successful applied flags match real class/mastery state.");
                    Assert(mentors.IsDiscovered(MentorId.Lyra) && mentors.GetTrust(MentorId.Lyra) == 12
                        && mentors.IsAvailable(MentorId.Lyra) && trees.IsUnlocked("node_deep_sight", fixture.Tree),
                        "AUD-12 successful applied flags match real mentor/tree state.");
                    Assert(registry.Bindings.Count == (populated ? 1 : 0) + (bindings ? 1 : 0)
                        && (!bindings || registry.ResolveCurrentClassSkillTree(classes).Id == fixture.Tree.Id),
                        "AUD-12 only a requested binding restore is applied, including populated registries.");
                    if (format == 1 && bindings)
                        Assert(trees.IsUnlocked("node_deep_sight", registry.GetSkillTree(SkillTreeId.Wanderer)),
                            "AUD-12 multi-scope application restores both registered tree scopes.");
                    if (populated)
                        Assert(mastery.IsMastered("skill_balance_ritual") && mentors.GetTrust(MentorId.Atlas) == 3
                            && trees.IsUnlocked("node_deep_sight", registry.GetSkillTree(SkillTreeId.Wanderer)),
                            "AUD-12 merge restore retains unrelated existing mastery, mentor and scoped tree facts.");
                }
                string after = CaptureAud12State(classes, mastery, mentors, trees, registry);
                AssertAud12SuccessfulFlags(restore(), bindings);
                Assert(CaptureAud12State(classes, mastery, mentors, trees, registry) == after,
                    "AUD-12 repeating application is a successfully completed idempotent operation.");
            }
        }

        private static void AssertAud12SuccessfulFlags(V4ProgressRestoreResult result, bool bindings)
        {
            Assert(result.FullyRestored && result.ClassRestored && result.MasteryRestored
                && result.MentorRelationshipsRestored && result.SkillTreeProgressRestored
                && result.ClassSkillTreeBindingsRequested == bindings && result.ClassSkillTreeBindingsRestored == bindings,
                "AUD-12 success reports exactly the requested subsystems, without a fabricated binding application.");
            var messages = Aud12Messages(result);
            var reasons = Aud12Reasons(result);
            for (int index = 0; index < 5; index++)
                Assert(reasons[index] == V4ProgressRestoreFailureReason.None && messages[index] == "",
                    "AUD-12 successful/no-op/unrequested sections have no spurious failure diagnostics.");
        }

        private static void AssertRestoreApplicationFailureIsReported()
        {
            foreach (bool bindings in new[] { false, true })
            {
                var fixture = new RestoreAtomicityFixture();
                var changing = new Aud12ChangingList<V2MasterySnapshot>(fixture.Snapshot.MasteryRecords,
                    new[] { new V2MasterySnapshot("skill_deep_sight", 30, MasteryRank.Practiced),
                        new V2MasterySnapshot("skill_unknown", 25, MasteryRank.Practiced) }, 2);
                var result = fixture.Restore(Aud12Snapshot(fixture.Snapshot, mastery: changing), bindings);
                Assert(!result.FullyRestored && !result.MasteryRestored
                    && result.MasteryFailureReason == V4ProgressRestoreFailureReason.ApplicationFailed
                    && result.MasteryFailureMessage.Contains("MasteryProfile")
                    && result.MasteryFailureMessage.Contains("partial changes"),
                    "AUD-12 a real false return after feasible preflight must be reported as application failure.");
                Assert(result.ClassRestored && result.MentorRelationshipsRestored && result.SkillTreeProgressRestored
                    && result.ClassSkillTreeBindingsRestored == bindings
                    && fixture.Classes.CurrentClassId == ClassId.Tlamatini
                    && fixture.Mastery.GetRecord("skill_deep_sight").Progress == 30
                    && fixture.Mastery.GetRecord("skill_unknown") == null
                    && fixture.Mentors.GetTrust(MentorId.Lyra) == 12,
                    "AUD-12 application failure preserves truthful partial outcomes, not a rollback claim.");
            }

            var bindingFixture = new RestoreAtomicityFixture();
            // Bindings are enumerated twice during preflight, then validated again on the real destination.
            var changingBindings = new Aud12ChangingList<V2ClassSkillTreeBindingSnapshot>(
                bindingFixture.Snapshot.ClassSkillTreeBindings,
                new[] { new V2ClassSkillTreeBindingSnapshot(ClassId.Wanderer, SkillTreeId.Tlamatini) }, 3);
            var bindingResult = bindingFixture.Restore(Aud12Snapshot(bindingFixture.Snapshot, bindings: changingBindings), true);
            Assert(!bindingResult.FullyRestored && !bindingResult.ClassSkillTreeBindingsRestored
                && bindingResult.ClassSkillTreeBindingsFailureReason == V4ProgressRestoreFailureReason.ApplicationFailed
                && bindingResult.ClassSkillTreeBindingsFailureMessage.Contains("ClassSkillTreeBindings"),
                "AUD-12 real binding conflict after preflight is not silently reported as restored.");
            Assert(bindingResult.ClassRestored && bindingResult.MasteryRestored && bindingResult.MentorRelationshipsRestored
                && bindingResult.SkillTreeProgressRestored && bindingFixture.Bindings.Bindings.Count == 1
                && bindingFixture.Bindings.GetBindingForClass(ClassId.Wanderer).SkillTreeId == SkillTreeId.Wanderer,
                "AUD-12 later binding failure preserves existing bindings and reports earlier real applications.");
        }

        private static void AssertRestoreResultConstructorCompatibility()
        {
            Assert((int)V4ProgressRestoreFailureReason.None == 0 && (int)V4ProgressRestoreFailureReason.ValidationFailed == 1,
                "AUD-12 stable existing failure reason values are not renumbered.");
            var simple = new V4ProgressRestoreResult(true, true, true, true);
            Assert(simple.FullyRestored && simple.ClassSkillTreeBindingsRestored && simple.ClassSkillTreeBindingsRequested,
                "AUD-12 legacy four-boolean constructor retains its historical binding-success default.");
            var explicitBinding = new V4ProgressRestoreResult(true, true, true, true, false);
            Assert(!explicitBinding.FullyRestored
                && explicitBinding.ClassSkillTreeBindingsFailureReason == V4ProgressRestoreFailureReason.ValidationFailed,
                "AUD-12 legacy five-boolean constructor retains its failure mapping.");
            var detailed = new V4ProgressRestoreResult(false, false, false, false,
                V4ProgressRestoreFailureReason.ValidationFailed, V4ProgressRestoreFailureReason.None,
                V4ProgressRestoreFailureReason.None, V4ProgressRestoreFailureReason.None, "class", null, null, null);
            Assert(!detailed.FullyRestored && detailed.ClassSkillTreeBindingsRestored
                && detailed.ClassFailureMessage == "class" && detailed.MasteryFailureMessage == "",
                "AUD-12 existing detailed constructor preserves caller facts and null-message normalization.");
            var detailedBinding = new V4ProgressRestoreResult(true, true, true, true, false,
                V4ProgressRestoreFailureReason.None, V4ProgressRestoreFailureReason.None,
                V4ProgressRestoreFailureReason.None, V4ProgressRestoreFailureReason.None,
                V4ProgressRestoreFailureReason.ValidationFailed, null, null, null, null, "binding");
            Assert(!detailedBinding.FullyRestored && detailedBinding.ClassSkillTreeBindingsFailureMessage == "binding"
                && detailedBinding.ClassFailureMessage == "",
                "AUD-12 full public constructor remains source/behavior compatible.");
        }

        private static V4ProgressRestoreFailureReason[] Aud12Reasons(V4ProgressRestoreResult result)
        {
            return new[] { result.ClassFailureReason, result.MasteryFailureReason, result.MentorRelationshipsFailureReason,
                result.SkillTreeProgressFailureReason, result.ClassSkillTreeBindingsFailureReason };
        }

        private static string[] Aud12Messages(V4ProgressRestoreResult result)
        {
            return new[] { result.ClassFailureMessage, result.MasteryFailureMessage, result.MentorRelationshipsFailureMessage,
                result.SkillTreeProgressFailureMessage, result.ClassSkillTreeBindingsFailureMessage };
        }

        private static V2ProgressSnapshot Aud12Snapshot(V2ProgressSnapshot source,
            IReadOnlyList<V2MasterySnapshot> mastery = null, V2SkillTreeProgressSnapshot tree = null,
            IReadOnlyList<V2ClassSkillTreeBindingSnapshot> bindings = null)
        {
            return new V2ProgressSnapshot(source.SchemaVersion, source.CodexEntries, source.MissionLeads,
                source.ActiveMissions, source.Unlocks, source.ClassProfile, mastery ?? source.MasteryRecords,
                source.MentorRelationships, tree ?? source.SkillTreeProgress, bindings ?? source.ClassSkillTreeBindings,
                source.CodexDiscoveredEntryIds, source.CodexCompletedEntryIds);
        }

        private static string CaptureAud12State(ClassProfile classes, MasteryProfile mastery,
            MentorRelationshipProfile mentors, SkillTreeProgressProfile trees, ClassSkillTreeBindingRegistry bindings)
        {
            return new V2ProgressSnapshotJsonSerializer().Serialize(new V2ProgressSnapshotService().CreateSnapshot(
                new CodexProfile(), new MissionJournalProfile(), new UnlockProfile(), classes, mastery, mentors, trees, bindings));
        }

        // Deterministic fault injection for mutable snapshot input, not a normal stable-input workflow.
        private sealed class Aud12ChangingList<T> : IReadOnlyList<T>
        {
            private readonly IReadOnlyList<T> before;
            private readonly IReadOnlyList<T> after;
            private readonly int changeOnEnumeration;
            private int enumerations;

            public Aud12ChangingList(IReadOnlyList<T> before, IReadOnlyList<T> after, int changeOnEnumeration)
            {
                this.before = before;
                this.after = after;
                this.changeOnEnumeration = changeOnEnumeration;
            }

            public int Count { get { return before.Count; } }
            public T this[int index] { get { return before[index]; } }

            public IEnumerator<T> GetEnumerator()
            {
                enumerations++;
                return (enumerations >= changeOnEnumeration ? after : before).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        }
    }
}
