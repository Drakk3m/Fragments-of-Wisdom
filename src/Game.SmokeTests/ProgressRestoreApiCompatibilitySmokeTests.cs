using System;
using System.Linq;
using System.Text.Json;
using Game.Domain.Classes;
using Game.Domain.Progress;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunProgressRestoreApiCompatibilityTests()
        {
            AssertRestoreApiSurfaceCompatibility();
            AssertRestoreApiConstructorCompatibility();
            foreach (bool bindings in new[] { false, true })
            {
                AssertRestoreApiSuccessfulMerge(bindings);
                AssertRestoreApiFailureEquivalence(bindings);
                AssertRestoreApiNullDestinationCompatibility(bindings);
            }
        }

        private static void AssertRestoreApiSurfaceCompatibility()
        {
            Assert(typeof(V4ProgressRestoreCoordinator).GetConstructors().Length == 2
                && typeof(ProgressRestoreCoordinator).GetConstructors().Length == 2,
                "P8A: both coordinator constructors remain public.");
            Assert(typeof(V4ProgressRestoreResult).GetConstructors().Length == 4
                && typeof(ProgressRestoreResult).GetConstructors().Length == 4,
                "P8A: all four public result constructors remain available.");
            var legacyMethods = typeof(V4ProgressRestoreCoordinator).GetMethods()
                .Where(method => method.Name == "Restore").ToArray();
            Assert(legacyMethods.Length == 2, "P8A: both legacy Restore overloads remain public.");
            foreach (var method in legacyMethods)
            {
                var parameters = method.GetParameters();
                var canonical = typeof(ProgressRestoreCoordinator).GetMethod("Restore",
                    parameters.Select(parameter => parameter.ParameterType).ToArray());
                Assert(method.ReturnType == typeof(V4ProgressRestoreResult)
                    && canonical != null && canonical.ReturnType == typeof(ProgressRestoreResult)
                    && parameters.Select(parameter => parameter.Name)
                        .SequenceEqual(canonical.GetParameters().Select(parameter => parameter.Name)),
                    "P8A: overload parameter types/names and each family's return type are preserved.");
            }
            var legacyProperties = typeof(V4ProgressRestoreResult).GetProperties();
            Assert(legacyProperties.Length == 17 && typeof(ProgressRestoreResult).GetProperties().Length == 17
                && legacyProperties.All(property =>
                {
                    var canonical = typeof(ProgressRestoreResult).GetProperty(property.Name);
                    var expectedType = property.PropertyType == typeof(V4ProgressRestoreFailureReason)
                        ? typeof(ProgressRestoreFailureReason) : property.PropertyType;
                    return canonical != null && canonical.PropertyType == expectedType
                        && property.GetGetMethod() != null && property.GetSetMethod() == null;
                }), "P8A: all result facts retain their public read-only surface.");
            string[] names = { "None", "ValidationFailed", "NotApplied", "ApplicationFailed" };
            Assert(Enum.GetNames<V4ProgressRestoreFailureReason>().SequenceEqual(names)
                && Enum.GetNames<ProgressRestoreFailureReason>().SequenceEqual(names),
                "P8A: no failure reason names are removed or added.");
            for (int value = 0; value < names.Length; value++)
            {
                Assert((int)Enum.Parse<V4ProgressRestoreFailureReason>(names[value]) == value
                    && (int)Enum.Parse<ProgressRestoreFailureReason>(names[value]) == value
                    && JsonSerializer.Serialize((V4ProgressRestoreFailureReason)value) == value.ToString()
                    && JsonSerializer.Serialize((ProgressRestoreFailureReason)value) == value.ToString(),
                    "P8A: failure reason ordinals and default numeric JSON remain stable.");
            }
        }

        private static void AssertRestoreApiConstructorCompatibility()
        {
            AssertRestoreApiResultsEquivalent(new V4ProgressRestoreResult(true, true, true, true),
                new ProgressRestoreResult(true, true, true, true));
            AssertRestoreApiResultsEquivalent(new V4ProgressRestoreResult(true, false, true, false, false),
                new ProgressRestoreResult(true, false, true, false, false));
            AssertRestoreApiResultsEquivalent(new V4ProgressRestoreResult(false, true, false, true,
                V4ProgressRestoreFailureReason.ValidationFailed, V4ProgressRestoreFailureReason.NotApplied,
                V4ProgressRestoreFailureReason.ApplicationFailed, (V4ProgressRestoreFailureReason)99,
                "class", null, "mentor", "tree"),
                new ProgressRestoreResult(false, true, false, true,
                    ProgressRestoreFailureReason.ValidationFailed, ProgressRestoreFailureReason.NotApplied,
                    ProgressRestoreFailureReason.ApplicationFailed, (ProgressRestoreFailureReason)99,
                    "class", null, "mentor", "tree"));
            AssertRestoreApiResultsEquivalent(new V4ProgressRestoreResult(true, false, true, false, false,
                V4ProgressRestoreFailureReason.None, V4ProgressRestoreFailureReason.ValidationFailed,
                V4ProgressRestoreFailureReason.NotApplied, V4ProgressRestoreFailureReason.ApplicationFailed,
                (V4ProgressRestoreFailureReason)99, null, "mastery", "mentor", "tree", "binding"),
                new ProgressRestoreResult(true, false, true, false, false,
                    ProgressRestoreFailureReason.None, ProgressRestoreFailureReason.ValidationFailed,
                    ProgressRestoreFailureReason.NotApplied, ProgressRestoreFailureReason.ApplicationFailed,
                    (ProgressRestoreFailureReason)99, null, "mastery", "mentor", "tree", "binding"));
            var oldError = CaptureRestoreApiArgumentError(() => new V4ProgressRestoreCoordinator(null));
            var newError = CaptureRestoreApiArgumentError(() => new ProgressRestoreCoordinator(null));
            Assert(oldError.ParamName == "snapshotService" && oldError.Message == newError.Message,
                "P8A: constructor null rejection retains parameter name and diagnostic.");
        }

        private static void AssertRestoreApiSuccessfulMerge(bool bindings)
        {
            var legacy = new RestoreAtomicityFixture();
            var canonical = new RestoreAtomicityFixture();
            var serializer = new V2ProgressSnapshotJsonSerializer();
            string json = serializer.Serialize(legacy.Snapshot);
            var snapshot = serializer.Deserialize(json);
            Assert(serializer.Serialize(snapshot) == json && snapshot.SchemaVersion == 1,
                "P8A: existing snapshot JSON round trip retains bytes and schema.");
            using (var document = JsonDocument.Parse(json))
            {
                Assert(document.RootElement.GetProperty("MasteryRecords")[0].GetProperty("Rank").ValueKind
                    == JsonValueKind.Number && !json.Contains("RestoreFailureReason"),
                    "P8A: persisted ranks remain numeric and restore diagnostics are not snapshot fields.");
            }
            for (int attempt = 0; attempt < 2; attempt++)
            {
                var oldResult = RestoreThroughLegacyApi(legacy, snapshot, bindings);
                var newResult = canonical.Restore(snapshot, bindings);
                AssertRestoreApiResultsEquivalent(oldResult, newResult);
                Assert(newResult.FullyRestored && newResult.ClassSkillTreeBindingsRequested == bindings
                    && newResult.ClassSkillTreeBindingsRestored == bindings,
                    "P8A: successful and repeated restore report only requested applications.");
                Assert(legacy.CaptureState() == canonical.CaptureState()
                    && canonical.Classes.CurrentClassId == ClassId.Tlamatini
                    && canonical.Mastery.GetRecord("skill_deep_sight").Progress == 25
                    && canonical.Mastery.IsMastered("skill_balance_ritual")
                    && canonical.Mentors.GetTrust(MentorId.Atlas) == 3
                    && canonical.Bindings.Bindings.Count == (bindings ? 2 : 1),
                    "P8A: both APIs apply identical values and preserve unrelated populated state.");
            }
            string before = canonical.CaptureState();
            var oldSnapshot = serializer.Deserialize("{\"SchemaVersion\":1}");
            AssertRestoreApiResultsEquivalent(RestoreThroughLegacyApi(legacy, oldSnapshot, bindings),
                canonical.Restore(oldSnapshot, bindings));
            Assert(legacy.CaptureState() == before && canonical.CaptureState() == before
                && serializer.Serialize(snapshot) == json,
                "P8A: missing legacy fields remain merge no-ops; restore does not rewrite its snapshot.");
        }

        private static void AssertRestoreApiFailureEquivalence(bool bindings)
        {
            // Representative adapter coverage; AUD-01/02/12 own the full semantic failure matrices.
            foreach (bool semanticTreeFailure in new[] { false, true })
            {
                var legacy = new RestoreAtomicityFixture();
                var canonical = new RestoreAtomicityFixture();
                var snapshot = semanticTreeFailure
                    ? WithSemanticTreeProgress(legacy.Snapshot, TreeProgress(true,
                        new[] { SemanticTreeRoot, SemanticTreeChild }, new[] { SemanticTreeChild }))
                    : BuildCompleteV4Snapshot("class_unknown", "skill_deep_sight", MentorId.Lyra, SemanticTreeRoot);
                string before = canonical.CaptureState();
                var oldResult = RestoreThroughLegacyApi(legacy, snapshot, bindings);
                var newResult = canonical.Restore(snapshot, bindings);
                AssertRestoreApiResultsEquivalent(oldResult, newResult);
                Assert(!newResult.FullyRestored && !newResult.ClassRestored && !newResult.MasteryRestored
                    && !newResult.MentorRelationshipsRestored && !newResult.SkillTreeProgressRestored
                    && !newResult.ClassSkillTreeBindingsRestored
                    && newResult.MasteryFailureReason == ProgressRestoreFailureReason.NotApplied,
                    "P8A: rejected preflight never turns feasibility into applied-state facts.");
                Assert(legacy.CaptureState() == before && canonical.CaptureState() == before,
                    "P8A: semantic/registry rejection remains preflight-only in both API families.");
            }
            var oldFixture = new RestoreAtomicityFixture();
            var newFixture = new RestoreAtomicityFixture();
            Func<RestoreAtomicityFixture, V2ProgressSnapshot> changingSnapshot = fixture =>
                Aud12Snapshot(fixture.Snapshot, mastery: new Aud12ChangingList<V2MasterySnapshot>(
                    fixture.Snapshot.MasteryRecords,
                    new[] { new V2MasterySnapshot("skill_unknown", 25, MasteryRank.Practiced) }, 2));
            var oldFailure = RestoreThroughLegacyApi(oldFixture, changingSnapshot(oldFixture), bindings);
            var newFailure = newFixture.Restore(changingSnapshot(newFixture), bindings);
            AssertRestoreApiResultsEquivalent(oldFailure, newFailure);
            Assert(!newFailure.FullyRestored && newFailure.ClassRestored && !newFailure.MasteryRestored
                && newFailure.MasteryFailureReason == ProgressRestoreFailureReason.ApplicationFailed
                && oldFixture.CaptureState() == newFixture.CaptureState(),
                "P8A: real application failure remains honestly partial, without adapter rollback claims.");
        }

        private static void AssertRestoreApiNullDestinationCompatibility(bool bindings)
        {
            foreach (string missing in bindings ? new[] { "masteryProfile", "classSkillTreeBindingRegistry" }
                : new[] { "masteryProfile" })
            {
                var legacy = new RestoreAtomicityFixture();
                var canonical = new RestoreAtomicityFixture();
                string before = canonical.CaptureState();
                var oldError = CaptureRestoreApiArgumentError(() =>
                    RestoreThroughLegacyApi(legacy, legacy.Snapshot, bindings, missing));
                var newError = CaptureRestoreApiArgumentError(() => canonical.Restore(canonical.Snapshot, bindings, missing));
                Assert(oldError.ParamName == missing && oldError.Message == newError.Message,
                    "P8A: destination exceptions remain identical through the compatibility adapter.");
                Assert(legacy.CaptureState() == before && canonical.CaptureState() == before,
                    "P8A: null destinations cause no mutation through either entry point.");
            }
        }

        private static V4ProgressRestoreResult RestoreThroughLegacyApi(RestoreAtomicityFixture fixture,
            V2ProgressSnapshot snapshot, bool bindings, string missing = "")
        {
            // Exercise injection as well as the canonical fixture's parameterless constructor.
            var coordinator = new V4ProgressRestoreCoordinator(new V2ProgressSnapshotService());
            return bindings
                ? coordinator.Restore(snapshot, fixture.Classes, missing == "masteryProfile" ? null : fixture.Mastery,
                    fixture.Mentors, fixture.Trees, fixture.ClassRegistry, fixture.SkillRegistry,
                    fixture.MentorRegistry, fixture.Tree, missing == "classSkillTreeBindingRegistry" ? null : fixture.Bindings)
                : coordinator.Restore(snapshot, fixture.Classes, missing == "masteryProfile" ? null : fixture.Mastery,
                    fixture.Mentors, fixture.Trees, fixture.ClassRegistry, fixture.SkillRegistry, fixture.MentorRegistry, fixture.Tree);
        }

        private static ArgumentNullException CaptureRestoreApiArgumentError(Func<object> action)
        {
            try { action(); }
            catch (ArgumentNullException exception) { return exception; }
            throw new InvalidOperationException("P8A: expected ArgumentNullException was not raised.");
        }

        private static void AssertRestoreApiResultsEquivalent(V4ProgressRestoreResult legacy, ProgressRestoreResult canonical)
        {
            // Compares all 17 public facts, including numeric reasons and exact diagnostic messages.
            Assert(JsonSerializer.Serialize(legacy) == JsonSerializer.Serialize(canonical),
                "P8A: old and canonical result facts must be identical, without conversion loss.");
        }
    }
}
