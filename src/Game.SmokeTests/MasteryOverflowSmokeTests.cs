using System;
using System.Text.Json;
using Game.Domain.Classes;
using Game.Domain.Codex;
using Game.Domain.Missions;
using Game.Domain.Progress;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunMasteryOverflowSmokeTests()
        {
            AssertMasteryProgressCannotOverflow();
            AssertMasteryBoundsAndThresholds();
            AssertMasteryPositiveIncrementMatrix();
            AssertMasteryNonPositiveIncrementsStayCompatible();
            AssertMasterySequenceIsMonotonic();
            AssertMasteryProfileIncrementContracts();
            AssertMasteryOverflowMentorSnapshotWorkflow();
            AssertMasteryOverflowLegacySnapshotCompatibility();
        }

        private static void AssertMasteryProgressCannotOverflow()
        {
            var record = new MasteryRecord("skill_aud09", 50);
            var rankBefore = record.Rank;
            bool increased = record.AddProgress(int.MaxValue);
            Console.WriteLine("  AUD-09 50 + int.MaxValue: progress={0}, rank={1} -> {2}, increased={3}",
                record.Progress, rankBefore, record.Rank, increased);
            Assert(increased && record.Progress == MasteryRecord.MasteredProgress
                && record.Rank == MasteryRank.Mastered && record.IsMastered,
                "AUD-09: positive mastery progress must saturate at 100 without overflow or rank regression.");
        }

        private static void AssertMasteryBoundsAndThresholds()
        {
            var fresh = new MasteryRecord("skill_aud09");
            Assert(fresh.Progress == 0 && fresh.Rank == MasteryRank.Unranked && !fresh.IsMastered,
                "AUD-09 new mastery records start at zero and Unranked.");

            int[] inputs = { int.MinValue, -1, 0, 1, 2, 98, 99, 100, 101, int.MaxValue };
            int[] expected = { 0, 0, 0, 1, 2, 98, 99, 100, 100, 100 };
            for (int index = 0; index < inputs.Length; index++)
            {
                var record = new MasteryRecord("skill_aud09", inputs[index]);
                Assert(record.Progress == expected[index] && record.Rank == ExpectedMasteryRank(expected[index])
                    && record.IsMastered == (expected[index] == 100),
                    "AUD-09 constructor preserves bounds and rank thresholds for " + inputs[index]);
                fresh.SetProgress(inputs[index]);
                Assert(fresh.Progress == expected[index] && fresh.Rank == record.Rank,
                    "AUD-09 SetProgress retains existing replacement/clamping semantics for " + inputs[index]);
            }
        }

        private static void AssertMasteryPositiveIncrementMatrix()
        {
            foreach (int start in new[] { 0, 1, 50, 98, 99, 100 })
            {
                foreach (int increment in new[] { 1, 2, 50, 100, int.MaxValue - 50, int.MaxValue })
                {
                    var record = new MasteryRecord("skill_aud09", start);
                    var previousRank = record.Rank;
                    // Subtraction-based oracle is independent of the production widened sum.
                    int expected = increment >= 100 - start ? 100 : start + increment;
                    bool changed = record.AddProgress(increment);
                    Assert(changed == (start < 100),
                        "AUD-09 return value must reflect whether a non-mastered record advances.");
                    Assert(record.Progress == expected && record.Progress >= start && record.Progress <= 100,
                        "AUD-09 positive increment bounds: " + start + " + " + increment);
                    Assert(record.Rank == ExpectedMasteryRank(expected)
                        && (int)record.Rank >= (int)previousRank && record.IsMastered == (expected == 100),
                        "AUD-09 positive increments must preserve rank monotonicity and mastery facts.");
                }
            }
        }

        private static void AssertMasteryNonPositiveIncrementsStayCompatible()
        {
            foreach (int start in new[] { 0, 50, 100 })
            {
                foreach (int increment in new[] { 0, -1, int.MinValue })
                {
                    var record = new MasteryRecord("skill_aud09", start);
                    var previousRank = record.Rank;
                    Assert(!record.AddProgress(increment),
                        "AUD-09 zero and negative increments remain rejected.");
                    Assert(record.Progress == start && record.Rank == previousRank,
                        "AUD-09 rejected increments must not change progress or rank.");
                }
            }
        }

        private static void AssertMasterySequenceIsMonotonic()
        {
            var record = new MasteryRecord("skill_aud09");
            int[] increments = { 1, 1, 47, 1, 48, 1, 1, int.MaxValue, 1 };
            int[] expected = { 1, 2, 49, 50, 98, 99, 100, 100, 100 };
            for (int index = 0; index < increments.Length; index++)
            {
                int previousProgress = record.Progress;
                var previousRank = record.Rank;
                bool changed = record.AddProgress(increments[index]);
                Assert(record.Progress == expected[index] && record.Progress >= previousProgress
                    && changed == (previousProgress < 100),
                    "AUD-09 a sequence must advance normally and stay idempotent after mastery.");
                Assert(record.Rank == ExpectedMasteryRank(expected[index])
                    && (int)record.Rank >= (int)previousRank,
                    "AUD-09 rank must not regress across repeated positive increments.");
            }
        }

        private static MasteryRank ExpectedMasteryRank(int boundedProgress)
        {
            return boundedProgress == 0 ? MasteryRank.Unranked
                : boundedProgress < 100 ? MasteryRank.Practiced : MasteryRank.Mastered;
        }

        private static void AssertMasteryProfileIncrementContracts()
        {
            const string skillId = "skill_deep_sight";
            var registry = BuildTestSkillRegistry();
            var profile = new MasteryProfile();
            Assert(profile.IncreaseProgress(skillId, 50, registry),
                "AUD-09 profile must accept normal registered progress.");
            var original = profile.GetRecord(skillId);
            Assert(profile.IncreaseProgress(skillId, int.MaxValue, registry)
                && profile.IsMastered(skillId) && original.Progress == 100,
                "AUD-09 profile must inherit overflow-safe accumulation.");
            Assert(!profile.IncreaseProgress(skillId, int.MaxValue, registry)
                && ReferenceEquals(profile.GetRecord(skillId), original) && profile.Records.Count == 1,
                "AUD-09 repeated profile application after mastery remains false without duplicate records.");
            Assert(!profile.IncreaseProgress("skill_unknown", int.MaxValue, registry)
                && !profile.IncreaseProgress(skillId, int.MaxValue, null) && profile.Records.Count == 1,
                "AUD-09 registry validation remains unchanged.");

            foreach (int increment in new[] { 0, -1, int.MinValue })
            {
                var empty = new MasteryProfile();
                Assert(!empty.IncreaseProgress(skillId, increment, registry)
                    && empty.Records.Count == 1 && empty.GetRecord(skillId).Progress == 0,
                    "AUD-09 preserve profile's existing unranked-record creation on rejected first increments.");
            }
        }

        private static void AssertMasteryOverflowMentorSnapshotWorkflow()
        {
            const string skillId = "skill_deep_sight";
            var skills = BuildTestSkillRegistry();
            var mentors = BuildTestMentorRegistry();
            var relationship = new MentorRelationshipProfile();
            relationship.Discover(MentorId.Lyra, mentors);
            relationship.IncreaseTrust(MentorId.Lyra, 25, mentors);
            var profile = new MasteryProfile();
            profile.IncreaseProgress(skillId, 50, skills);
            var teaching = new MentorTeachingService().Evaluate(new MentorTeachingDefinition(
                MentorId.Lyra, skillId, 20, int.MaxValue), relationship, mentors, skills);
            Assert(teaching.IsAvailable,
                "AUD-09 workflow resolves the existing mentor/registry/trust teaching path.");
            var application = new MentorTeachingApplicationService().Apply(teaching, profile, skills);
            Assert(application.Applied && !application.WasAlreadyMastered
                && profile.GetRecord(skillId).Progress == 100 && profile.IsMastered(skillId),
                "AUD-09 large mentor grants must reach mastery without overflow.");
            // ProgressApplied's capped-delta defect is AUD-10, not this regression's contract.
            var repeated = new MentorTeachingApplicationService().Apply(teaching, profile, skills);
            Assert(!repeated.Applied && repeated.WasAlreadyMastered
                && repeated.BlockReason == MentorTeachingApplicationBlockReason.AlreadyMastered
                && profile.GetRecord(skillId).Progress == 100 && relationship.GetTrust(MentorId.Lyra) == 25,
                "AUD-09 repeated teaching after mastery is safe and does not mutate mentor trust.");

            var service = new V2ProgressSnapshotService();
            var snapshot = service.CreateSnapshot(new CodexProfile(), new MissionJournalProfile(),
                new UnlockProfile(), new ClassProfile(), profile);
            Assert(snapshot.SchemaVersion == 1 && snapshot.MasteryRecords.Count == 1
                && snapshot.MasteryRecords[0].SkillId == skillId && snapshot.MasteryRecords[0].Progress == 100
                && snapshot.MasteryRecords[0].Rank == MasteryRank.Mastered,
                "AUD-09 snapshot captures bounded mastery using the existing schema and rank.");
            var serializer = new V2ProgressSnapshotJsonSerializer();
            string json = serializer.Serialize(snapshot);
            using (var parsed = JsonDocument.Parse(json))
            {
                Assert(parsed.RootElement.GetProperty("MasteryRecords")[0].GetProperty("Progress").GetInt32() == 100,
                    "AUD-09 persistence retains the existing integer progress representation.");
            }

            var loaded = serializer.Deserialize(json);
            Assert(new V2ProgressSnapshotValidationService().IsValid(loaded, skills),
                "AUD-09 saturated mastery snapshots remain registry-valid.");
            foreach (bool populated in new[] { false, true })
            {
                var restored = new MasteryProfile();
                if (populated)
                {
                    restored.IncreaseProgress(skillId, 25, skills);
                }
                Assert(service.RestoreMasteryProfile(loaded, restored, skills)
                    && restored.Records.Count == 1 && restored.GetRecord(skillId).Progress == 100
                    && restored.GetRecord(skillId).Rank == MasteryRank.Mastered,
                    "AUD-09 saturated mastery restores into fresh and populated profiles.");
                Assert(!restored.IncreaseProgress(skillId, int.MaxValue, skills) && restored.IsMastered(skillId),
                    "AUD-09 restored maximum remains stable under another oversized increment.");
            }
        }

        private static void AssertMasteryOverflowLegacySnapshotCompatibility()
        {
            const string skillId = "skill_deep_sight";
            var skills = BuildTestSkillRegistry();
            var serializer = new V2ProgressSnapshotJsonSerializer();
            var service = new V2ProgressSnapshotService();
            string legacyJson = JsonSerializer.Serialize(new
            {
                SchemaVersion = 1,
                MasteryRecords = new[] { new { SkillId = skillId, Progress = 50, Rank = MasteryRank.Unranked } }
            });
            var legacy = serializer.Deserialize(legacyJson);
            var profile = new MasteryProfile();
            Assert(service.RestoreMasteryProfile(legacy, profile, skills)
                && profile.GetRecord(skillId).Progress == 50 && profile.GetRecord(skillId).Rank == MasteryRank.Practiced,
                "AUD-09 legacy restore still derives rank from progress, not saved rank.");
            Assert(profile.IncreaseProgress(skillId, int.MaxValue, skills) && profile.IsMastered(skillId),
                "AUD-09 legacy restored progress gains overflow-safe arithmetic.");

            var missing = serializer.Deserialize("{\"SchemaVersion\":1}");
            var empty = new MasteryProfile();
            Assert(new V2ProgressSnapshotValidationService().IsValid(missing, skills)
                && service.RestoreMasteryProfile(missing, empty, skills) && empty.Records.Count == 0,
                "AUD-09 older JSON without mastery records still restores safely.");
        }
    }
}
