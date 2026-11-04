using System;
using Game.Domain.Classes;
using Game.Domain.Progress;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private const string SemanticTreeRoot = "node_deep_sight";
        private const string SemanticTreeChild = "node_balance_ritual";

        private static void RunSemanticSkillTreeRestoreTests()
        {
            foreach (bool includeBindings in new[] { true, false })
            {
                foreach (bool scoped in new[] { true, false })
                {
                    AssertSemanticTreeRestoreRejected(TreeProgress(scoped,
                        new[] { SemanticTreeRoot, SemanticTreeChild }, new[] { SemanticTreeChild }),
                        includeBindings, "missing prerequisite");
                    AssertSemanticTreeRestoreRejected(TreeProgress(scoped,
                        new[] { SemanticTreeRoot }, new[] { SemanticTreeChild }),
                        includeBindings, "unlocked but undiscovered child");
                    AssertSemanticTreeRestoreRejected(TreeProgress(scoped,
                        Array.Empty<string>(), new[] { SemanticTreeRoot }),
                        includeBindings, "unlocked but undiscovered root");
                    AssertSemanticTreeRestoreRejected(TreeProgress(scoped,
                        new[] { "node_unknown" }, Array.Empty<string>()),
                        includeBindings, "unknown discovered node");
                    AssertSemanticTreeRestoreRejected(TreeProgress(scoped,
                        new[] { SemanticTreeRoot }, new[] { "node_unknown" }),
                        includeBindings, "unknown unlocked node");
                    AssertValidSemanticTreeJsonRestore(includeBindings, scoped);
                }

                AssertSemanticTreeRestoreRejected(new V2SkillTreeProgressSnapshot(new[]
                {
                    new V2SkillTreeProgressScopeSnapshot("tree_unknown",
                        new[] { SemanticTreeRoot }, new[] { SemanticTreeRoot })
                }), includeBindings, "unknown scope");
                var scope = new V2SkillTreeProgressScopeSnapshot(SkillTreeId.Tlamatini,
                    new[] { SemanticTreeRoot }, new[] { SemanticTreeRoot });
                AssertSemanticTreeRestoreRejected(new V2SkillTreeProgressSnapshot(new[] { scope, scope }),
                    includeBindings, "duplicate scope");
                AssertSemanticTreeRestoreRejected(new V2SkillTreeProgressSnapshot(
                    new[] { SemanticTreeRoot }, new[] { SemanticTreeRoot }, new[] { scope }),
                    includeBindings, "mixed legacy and scoped progress");
                AssertSemanticTreeRestoreRejected(new V2SkillTreeProgressSnapshot(
                    new V2SkillTreeProgressScopeSnapshot[] { null }), includeBindings, "null scope");

                var fixture = new RestoreAtomicityFixture();
                string before = fixture.CaptureState();
                var oldSnapshot = new V2ProgressSnapshotJsonSerializer().Deserialize("{\"SchemaVersion\":1}");
                Assert(fixture.Restore(oldSnapshot, includeBindings).FullyRestored
                    && fixture.CaptureState() == before,
                    "AUD-02: old JSON without tree data must preserve populated state.");
            }

            AssertSemanticTreeRestoreUsesAuthoritativeContext();
            AssertSemanticMultiTreeRestore();
        }

        private static V2SkillTreeProgressSnapshot TreeProgress(
            bool scoped, string[] discovered, string[] unlocked)
        {
            return scoped
                ? new V2SkillTreeProgressSnapshot(new[]
                {
                    new V2SkillTreeProgressScopeSnapshot(SkillTreeId.Tlamatini, discovered, unlocked)
                })
                : new V2SkillTreeProgressSnapshot(discovered, unlocked);
        }

        private static V2ProgressSnapshot WithSemanticTreeProgress(
            V2ProgressSnapshot source, V2SkillTreeProgressSnapshot progress)
        {
            return new V2ProgressSnapshot(source.SchemaVersion, source.CodexEntries,
                source.MissionLeads, source.ActiveMissions, source.Unlocks, source.ClassProfile,
                source.MasteryRecords, source.MentorRelationships, progress, source.ClassSkillTreeBindings);
        }

        private static void AssertSemanticTreeRestoreRejected(
            V2SkillTreeProgressSnapshot progress, bool includeBindings, string scenario)
        {
            var fixture = new RestoreAtomicityFixture();
            var snapshot = WithSemanticTreeProgress(fixture.Snapshot, progress);
            string before = fixture.CaptureState();
            bool standaloneValid = new V2ProgressSnapshotValidationService().IsValid(snapshot, fixture.Bindings);

            for (int attempt = 0; attempt < 2; attempt++)
            {
                var result = fixture.Restore(snapshot, includeBindings);
                string after = fixture.CaptureState();
                Assert(!result.FullyRestored && !result.SkillTreeProgressRestored
                    && result.SkillTreeProgressFailureReason == ProgressRestoreFailureReason.ValidationFailed,
                    "AUD-02: " + scenario + " must fail tree restore; standalone valid=" + standaloneValid
                    + ", FullyRestored=" + result.FullyRestored + ", state changed=" + (before != after)
                    + ", child unlocked=" + fixture.Trees.IsUnlocked(SemanticTreeChild, fixture.Tree)
                    + ", root unlocked=" + fixture.Trees.IsUnlocked(SemanticTreeRoot, fixture.Tree)
                    + ", bindings overload=" + includeBindings + ".");
                Assert(after == before,
                    "AUD-02: " + scenario + " must preserve all populated destinations on attempt " + (attempt + 1));
            }
        }

        private static void AssertValidSemanticTreeJsonRestore(bool includeBindings, bool scoped)
        {
            var fixture = new RestoreAtomicityFixture();
            var snapshot = WithSemanticTreeProgress(fixture.Snapshot, TreeProgress(scoped,
                new[] { SemanticTreeRoot, SemanticTreeChild }, new[] { SemanticTreeRoot, SemanticTreeChild }));
            var serializer = new V2ProgressSnapshotJsonSerializer();
            snapshot = serializer.Deserialize(serializer.Serialize(snapshot));
            var result = fixture.Restore(snapshot, includeBindings);
            Assert(result.FullyRestored && result.SkillTreeProgressFailureReason == ProgressRestoreFailureReason.None
                && fixture.Trees.IsUnlocked(SemanticTreeRoot, fixture.Tree)
                && fixture.Trees.IsUnlocked(SemanticTreeChild, fixture.Tree),
                "AUD-02: valid closure must restore through JSON and either coordinator overload.");
            Assert(fixture.Classes.CurrentClassId == ClassId.Tlamatini
                && fixture.Mastery.GetRecord("skill_deep_sight").Progress == 25
                && fixture.Mentors.GetTrust(MentorId.Lyra) == 12
                && fixture.Bindings.Bindings.Count == (includeBindings ? 2 : 1),
                "AUD-02: valid tree restore must still apply the other requested subsystem facts.");
            string after = fixture.CaptureState();
            Assert(fixture.Restore(snapshot, includeBindings).FullyRestored && fixture.CaptureState() == after,
                "AUD-02: valid semantic restore must remain repeatable.");
        }

        private static void AssertSemanticTreeRestoreUsesAuthoritativeContext()
        {
            var fixture = new RestoreAtomicityFixture();
            var invalid = WithSemanticTreeProgress(fixture.Snapshot, TreeProgress(true,
                new[] { SemanticTreeChild }, new[] { SemanticTreeChild }));
            var permissiveTree = new SkillTreeDefinition(SkillTreeId.Tlamatini);
            permissiveTree.RegisterNode(new SkillTreeNodeDefinition(SemanticTreeChild,
                "skill_balance_ritual", Array.Empty<string>()), fixture.SkillRegistry);
            string before = fixture.CaptureState();
            var coordinator = new ProgressRestoreCoordinator();
            var result = coordinator.Restore(invalid, fixture.Classes, fixture.Mastery, fixture.Mentors,
                fixture.Trees, fixture.ClassRegistry, fixture.SkillRegistry, fixture.MentorRegistry,
                permissiveTree, fixture.Bindings);
            Assert(!result.FullyRestored && !result.SkillTreeProgressRestored && fixture.CaptureState() == before,
                "AUD-02: scoped binding restore must use registered definitions, not a permissive explicit tree.");

            var valid = WithSemanticTreeProgress(fixture.Snapshot, TreeProgress(true,
                new[] { SemanticTreeRoot, SemanticTreeChild }, new[] { SemanticTreeRoot, SemanticTreeChild }));
            result = coordinator.Restore(valid, fixture.Classes, fixture.Mastery, fixture.Mentors,
                fixture.Trees, fixture.ClassRegistry, fixture.SkillRegistry, fixture.MentorRegistry,
                null, fixture.Bindings);
            Assert(result.FullyRestored && fixture.Trees.IsUnlocked(SemanticTreeChild, fixture.Tree),
                "AUD-02: the registered scoped path does not require an unused explicit tree.");

            fixture = new RestoreAtomicityFixture();
            before = fixture.CaptureState();
            ArgumentNullException failure = null;
            try
            {
                coordinator.Restore(valid, fixture.Classes, fixture.Mastery, fixture.Mentors,
                    fixture.Trees, fixture.ClassRegistry, fixture.SkillRegistry, fixture.MentorRegistry, null);
            }
            catch (ArgumentNullException exception)
            {
                failure = exception;
            }
            Assert(failure != null && failure.ParamName == "skillTree" && fixture.CaptureState() == before,
                "AUD-02: missing required explicit tree must retain safe argument rejection.");

            var legacyTree = BuildTestSkillTreeDefinition();
            result = coordinator.Restore(fixture.Snapshot, fixture.Classes, fixture.Mastery, fixture.Mentors,
                fixture.Trees, fixture.ClassRegistry, fixture.SkillRegistry, fixture.MentorRegistry, legacyTree);
            Assert(result.FullyRestored && fixture.Trees.IsUnlocked(SemanticTreeRoot, legacyTree),
                "AUD-02: legacy explicit trees without stable scope IDs remain supported.");
        }

        private static void AssertSemanticMultiTreeRestore()
        {
            var rootScope = new V2SkillTreeProgressScopeSnapshot(SkillTreeId.Tlamatini,
                new[] { SemanticTreeRoot }, new[] { SemanticTreeRoot });
            var invalidSecond = new V2SkillTreeProgressScopeSnapshot(SkillTreeId.Wanderer,
                new[] { SemanticTreeRoot, SemanticTreeChild }, new[] { SemanticTreeChild });
            AssertSemanticTreeRestoreRejected(new V2SkillTreeProgressSnapshot(new[] { rootScope, invalidSecond }),
                true, "invalid second scope, even when its prerequisite exists in the destination");

            var validSecond = new V2SkillTreeProgressScopeSnapshot(SkillTreeId.Wanderer,
                new[] { SemanticTreeRoot, SemanticTreeChild }, new[] { SemanticTreeRoot, SemanticTreeChild });
            var progress = new V2SkillTreeProgressSnapshot(new[] { rootScope, validSecond });
            AssertSemanticTreeRestoreRejected(progress, false, "multiple scopes without a registry");
            var fixture = new RestoreAtomicityFixture();
            var serializer = new V2ProgressSnapshotJsonSerializer();
            var snapshot = serializer.Deserialize(serializer.Serialize(WithSemanticTreeProgress(fixture.Snapshot, progress)));
            var result = fixture.Restore(snapshot, true);
            Assert(result.FullyRestored
                && fixture.Trees.IsUnlocked(SemanticTreeChild, fixture.Bindings.GetSkillTree(SkillTreeId.Wanderer))
                && !fixture.Trees.IsUnlocked(SemanticTreeChild, fixture.Tree),
                "AUD-02: registered multi-tree JSON restore must preserve scope independence.");
        }
    }
}
