using System;
using System.Linq;
using System.Text.Json;
using Game.Domain.Classes;
using Game.Domain.Progress;
using Game.Domain.Skills;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunRestoreDestinationIntegrityTests()
        {
            foreach (bool includeBindings in new[] { false, true })
            {
                foreach (string destination in new[]
                {
                    "masteryProfile", "classProfile", "mentorRelationshipProfile",
                    "skillTreeProgressProfile"
                })
                {
                    AssertNullRestoreDestinationPreservesState(destination, includeBindings);
                }

                AssertValidRestoreIntoPopulatedDestinations(includeBindings);
                AssertInvalidRestoreInputPreservesState(includeBindings);
            }

            AssertNullRestoreDestinationPreservesState("classSkillTreeBindingRegistry", true);
        }

        private static void AssertNullRestoreDestinationPreservesState(
            string destination,
            bool includeBindings)
        {
            var fixture = new RestoreAtomicityFixture();
            string before = fixture.CaptureState();
            string classBefore = fixture.Classes.CurrentClassId;

            for (int attempt = 0; attempt < 2; attempt++)
            {
                ArgumentNullException failure = null;
                ProgressRestoreResult result = null;
                try
                {
                    result = fixture.Restore(fixture.Snapshot, includeBindings, destination);
                }
                catch (ArgumentNullException exception)
                {
                    failure = exception;
                }

                Assert(
                    failure != null && failure.ParamName == destination && result == null,
                    "AUD-01: null " + destination + " must throw its argument error, not return success.");
                Assert(
                    fixture.CaptureState() == before,
                    "AUD-01: null " + destination + " must preserve all destination values on attempt "
                    + (attempt + 1) + "; class before=" + classBefore
                    + ", after=" + fixture.Classes.CurrentClassId
                    + ", bindings overload=" + includeBindings + ".");
            }
        }

        private static void AssertValidRestoreIntoPopulatedDestinations(bool includeBindings)
        {
            var fixture = new RestoreAtomicityFixture();
            var serializer = new V2ProgressSnapshotJsonSerializer();
            var snapshot = serializer.Deserialize(serializer.Serialize(fixture.Snapshot));
            var result = fixture.Restore(snapshot, includeBindings);

            Assert(
                result.FullyRestored
                && fixture.Classes.CurrentClassId == ClassId.Tlamatini
                && fixture.Mastery.GetRecord("skill_deep_sight").Progress == 25
                && fixture.Mastery.IsMastered("skill_balance_ritual"),
                "AUD-01: valid JSON restore must apply class/mastery and preserve unrelated mastery.");
            Assert(
                fixture.Mentors.GetTrust(MentorId.Lyra) == 12
                && fixture.Mentors.IsAvailable(MentorId.Lyra)
                && fixture.Mentors.GetTrust(MentorId.Atlas) == 3
                && fixture.Trees.IsUnlocked("node_deep_sight", fixture.Tree)
                && fixture.Trees.IsUnlocked(
                    "node_deep_sight", fixture.Bindings.GetSkillTree(SkillTreeId.Wanderer)),
                "AUD-01: valid restore must apply mentor/tree facts without erasing existing scopes.");
            Assert(
                fixture.Bindings.Bindings.Count == (includeBindings ? 2 : 1)
                && (!includeBindings
                    || fixture.Bindings.ResolveCurrentClassSkillTree(fixture.Classes).Id == fixture.Tree.Id),
                "AUD-01: only the binding overload must apply the snapshot's new binding.");

            string after = fixture.CaptureState();
            Assert(
                fixture.Restore(snapshot, includeBindings).FullyRestored
                && fixture.CaptureState() == after,
                "AUD-01: repeating a valid restore must retain existing idempotent behavior.");
        }

        private static void AssertInvalidRestoreInputPreservesState(bool includeBindings)
        {
            var fixture = new RestoreAtomicityFixture();
            string before = fixture.CaptureState();
            var unsupported = new V2ProgressSnapshot(
                V2ProgressSnapshotVersions.CurrentSchemaVersion + 1,
                fixture.Snapshot.CodexEntries,
                fixture.Snapshot.MissionLeads);

            foreach (var snapshot in new[] { null, unsupported })
            {
                for (int attempt = 0; attempt < 2; attempt++)
                {
                    Exception failure = null;
                    ProgressRestoreResult result = null;
                    try
                    {
                        result = fixture.Restore(snapshot, includeBindings);
                    }
                    catch (ArgumentNullException exception)
                    {
                        failure = exception;
                    }
                    catch (InvalidOperationException exception)
                    {
                        failure = exception;
                    }

                    Assert(
                        result == null && (snapshot == null
                            ? failure is ArgumentNullException argument && argument.ParamName == "snapshot"
                            : failure is InvalidOperationException),
                        "AUD-01: invalid snapshot must retain its existing exception contract.");
                    Assert(
                        fixture.CaptureState() == before,
                        "AUD-01: invalid snapshot and repeated rejection must not mutate populated destinations.");
                }
            }

            var unknownClass = BuildCompleteV4Snapshot(
                "class_unknown", "skill_deep_sight", MentorId.Lyra, "node_deep_sight");
            for (int attempt = 0; attempt < 2; attempt++)
            {
                var result = fixture.Restore(unknownClass, includeBindings);
                Assert(
                    !result.FullyRestored && !result.ClassRestored
                    && result.ClassFailureReason == ProgressRestoreFailureReason.ValidationFailed,
                    "AUD-01: registry rejection must report overall failure and retain class failure details.");
                Assert(
                    fixture.CaptureState() == before,
                    "AUD-01: repeated registry rejection must preserve every destination value.");
            }
        }

        private sealed class RestoreAtomicityFixture
        {
            public readonly ClassDefinitionRegistry ClassRegistry = BuildTestClassRegistry();
            public readonly SkillDefinitionRegistry SkillRegistry = BuildTestSkillRegistry();
            public readonly MentorDefinitionRegistry MentorRegistry = BuildTestMentorRegistry();
            public readonly ClassProfile Classes = new ClassProfile();
            public readonly MasteryProfile Mastery = BuildMasteryProfileWithProgress();
            public readonly MentorRelationshipProfile Mentors = BuildMentorProfileWithRelationships();
            public readonly SkillTreeProgressProfile Trees = new SkillTreeProgressProfile();
            public readonly ClassSkillTreeBindingRegistry Bindings = BuildTestClassSkillTreeBindingRegistry();
            public readonly SkillTreeDefinition Tree;
            public readonly V2ProgressSnapshot Snapshot = BuildCompleteV4SnapshotWithBinding(
                ClassId.Tlamatini, SkillTreeId.Tlamatini);

            public RestoreAtomicityFixture()
            {
                Tree = Bindings.GetSkillTree(SkillTreeId.Tlamatini);
                Classes.AssignClassId(ClassId.Wanderer, ClassRegistry);
                Mastery.IncreaseProgress("skill_deep_sight", 25, SkillRegistry);
                Mentors.RestoreRelationship(MentorId.Lyra, true, 7, false, MentorRegistry);
                Bindings.RegisterBinding(
                    new ClassSkillTreeBinding(ClassId.Wanderer, SkillTreeId.Wanderer), ClassRegistry);
                var existingTree = Bindings.GetSkillTree(SkillTreeId.Wanderer);
                Trees.DiscoverNode("node_deep_sight", existingTree);
                new SkillTreeUnlockApplicationService().Apply(
                    "node_deep_sight", existingTree, Trees, SkillRegistry);
                Trees.DiscoverNode("node_balance_ritual", existingTree);
                Trees.DiscoverNode("node_deep_sight", Tree);
            }

            public ProgressRestoreResult Restore(
                V2ProgressSnapshot snapshot,
                bool includeBindings,
                string missingDestination = "")
            {
                var coordinator = new ProgressRestoreCoordinator();
                var classes = missingDestination == "classProfile" ? null : Classes;
                var mastery = missingDestination == "masteryProfile" ? null : Mastery;
                var mentors = missingDestination == "mentorRelationshipProfile" ? null : Mentors;
                var trees = missingDestination == "skillTreeProgressProfile" ? null : Trees;
                return includeBindings
                    ? coordinator.Restore(snapshot, classes, mastery, mentors, trees,
                        ClassRegistry, SkillRegistry, MentorRegistry, Tree,
                        missingDestination == "classSkillTreeBindingRegistry" ? null : Bindings)
                    : coordinator.Restore(snapshot, classes, mastery, mentors, trees,
                        ClassRegistry, SkillRegistry, MentorRegistry, Tree);
            }

            public string CaptureState()
            {
                // Materialize independent values, not references to mutable records or save DTOs.
                return JsonSerializer.Serialize(new
                {
                    Classes.CurrentClassId,
                    AllowedSkills = Classes.ResolveCurrentClassAllowedSkills(ClassRegistry, SkillRegistry)
                        .Select(skill => skill.Id).OrderBy(id => id).ToArray(),
                    Mastery = Mastery.Records.OrderBy(record => record.SkillId)
                        .Select(record => new { record.SkillId, record.Progress, record.Rank, record.IsMastered })
                        .ToArray(),
                    Mentors = Mentors.DiscoveredMentorIds.OrderBy(id => id)
                        .Select(id => new { Id = id, Discovered = Mentors.IsDiscovered(id),
                            Trust = Mentors.GetTrust(id), Available = Mentors.IsAvailable(id) }).ToArray(),
                    Trees = Trees.SkillTreeIds.OrderBy(id => id)
                        .Select(id => new { Id = id,
                            Discovered = Trees.GetDiscoveredNodeIds(id).OrderBy(node => node).ToArray(),
                            Unlocked = Trees.GetUnlockedNodeIds(id).OrderBy(node => node).ToArray() }).ToArray(),
                    Bindings = Bindings.Bindings.Values.OrderBy(binding => binding.ClassId)
                        .Select(binding => new { binding.ClassId, binding.SkillTreeId }).ToArray(),
                    RegisteredTrees = Bindings.SkillTrees.Keys.OrderBy(id => id).ToArray()
                });
            }
        }
    }
}
