using System;
using Game.Domain.Classes;
using Game.Domain.Codex;
using Game.Domain.Missions;
using Game.Domain.Progress;
using Game.Domain.Skills;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunMasteryProgressContractSmokeTests()
        {
            AssertTeachingReportsActualMasteryDelta();
            AssertMasteryOperationAcceptanceMatrix();
            AssertMasteryOperationRegistryRejections();
            AssertTeachingRejectionContracts();
            AssertMasteryResultIsImmutableFact();
            AssertMasteryDeltaMentorSnapshotWorkflow();
        }

        private static void AssertTeachingReportsActualMasteryDelta()
        {
            const string skillId = "skill_deep_sight";
            var skills = BuildTestSkillRegistry();
            var profile = new MasteryProfile();
            profile.IncreaseProgress(skillId, 90, skills);
            var teaching = new MentorTeachingResult(true, MentorId.Lyra, skillId,
                0, 0, 50, MentorTeachingBlockReason.None, "");
            var result = new MentorTeachingApplicationService().Apply(teaching, profile, skills);
            Console.WriteLine("  AUD-10: initial=90, requested=50, final={0}, reported={1}, actual={2}",
                profile.GetRecord(skillId).Progress, result.ProgressApplied,
                profile.GetRecord(skillId).Progress - 90);
            Assert(result.Applied && result.ProgressApplied == 10 && profile.GetRecord(skillId).Progress == 100,
                "AUD-10: teaching at mastery 90 requesting 50 must report actual delta 10, not requested 50.");
        }

        private static void AssertMasteryOperationAcceptanceMatrix()
        {
            const string skillId = "skill_deep_sight";
            var skills = BuildTestSkillRegistry();
            // Initial, requested, final, applied, raw overflow, first mastery (0/1).
            int[][] cases =
            {
                new[] { 0, 20, 20, 20, 0, 0 },
                new[] { 40, 30, 70, 30, 0, 0 },
                new[] { 90, 10, 100, 10, 0, 1 },
                new[] { 90, 50, 100, 10, 40, 1 },
                new[] { 99, 2, 100, 1, 1, 1 },
                new[] { 0, int.MaxValue, 100, 100, int.MaxValue - 100, 1 },
                new[] { 50, int.MaxValue, 100, 50, int.MaxValue - 50, 1 },
                new[] { 100, 50, 100, 0, 0, 0 },
                new[] { 40, 0, 40, 0, 0, 0 },
                new[] { 40, -10, 40, 0, 0, 0 },
                new[] { 40, int.MinValue, 40, 0, 0, 0 }
            };
            foreach (var row in cases)
            {
                var record = new MasteryRecord(skillId, row[0]);
                var result = record.AddProgressWithResult(row[1]);
                AssertMasteryOperationFacts(result, row[1], row[3], row[4], row[5] == 1);
                Assert(record.Progress == row[2] && record.Rank == ExpectedMasteryRank(row[2]),
                    "AUD-10 record state agrees with operation facts and unchanged rank thresholds.");
                var legacy = new MasteryRecord(skillId, row[0]);
                Assert(legacy.AddProgress(row[1]) == result.Applied && legacy.Progress == record.Progress,
                    "AUD-10 bool record API delegates to the same mutation.");

                var profile = new MasteryProfile();
                profile.RestoreRecord(skillId, row[0], skills);
                var original = profile.GetRecord(skillId);
                var profiled = profile.IncreaseProgressWithResult(skillId, row[1], skills);
                AssertMasteryOperationFacts(profiled, row[1], row[3], row[4], row[5] == 1);
                Assert(original.Progress == row[2] && profile.Records.Count == 1
                    && ReferenceEquals(original, profile.GetRecord(skillId)),
                    "AUD-10 profile mutates its one existing record exactly once.");
                var legacyProfile = new MasteryProfile();
                legacyProfile.RestoreRecord(skillId, row[0], skills);
                Assert(legacyProfile.IncreaseProgress(skillId, row[1], skills) == profiled.Applied
                    && legacyProfile.GetRecord(skillId).Progress == row[2],
                    "AUD-10 legacy profile API has the same actual delta.");

                var taught = new MasteryProfile();
                taught.RestoreRecord(skillId, row[0], skills);
                var application = new MentorTeachingApplicationService().Apply(
                    Aud10Teaching(row[1]), taught, skills);
                var expectedReason = row[0] == 100 ? MentorTeachingApplicationBlockReason.AlreadyMastered
                    : row[1] <= 0 ? MentorTeachingApplicationBlockReason.NoProgressGranted
                    : MentorTeachingApplicationBlockReason.None;
                Assert(application.Applied == result.Applied && application.ProgressApplied == row[3]
                    && taught.GetRecord(skillId).Progress == row[2] && application.SkillId == skillId,
                    "AUD-10 teaching reports actual progress, never requested amount or multiplied overflow.");
                Assert(application.BlockReason == expectedReason && application.WasAlreadyMastered == (row[0] == 100),
                    "AUD-10 teaching preserves block reasons and already-mastered facts.");
            }
        }

        private static void AssertMasteryOperationFacts(
            MasteryProgressResult result, int requested, int applied, int overflow, bool reached)
        {
            Assert(result.RequestedProgress == requested, "AUD-10 requested input is retained diagnostically.");
            Assert(result.AppliedProgress == applied, "AUD-10 applied progress is the actual bounded delta.");
            Assert(result.OverflowProgress == overflow, "AUD-10 overflow is raw, unmultiplied and zero for rejections.");
            Assert(result.ReachedMastery == reached, "AUD-10 mastery transition occurs only on first reaching the cap.");
            Assert(result.Applied == (applied > 0), "AUD-10 Applied requires a legitimate positive mutation.");
            Assert(!result.Applied || (long)result.AppliedProgress + result.OverflowProgress == requested,
                "AUD-10 successful grant accounting conserves requested progress without overflow.");
        }

        private static void AssertMasteryOperationRegistryRejections()
        {
            var skills = BuildTestSkillRegistry();
            foreach (bool populated in new[] { false, true })
            {
                var profile = new MasteryProfile();
                if (populated) profile.RestoreRecord("skill_deep_sight", 40, skills);
                foreach (string id in new[] { "skill_unknown", "", " ", null })
                {
                    AssertMasteryOperationFacts(profile.IncreaseProgressWithResult(id, 50, skills), 50, 0, 0, false);
                    Assert(profile.Records.Count == (populated ? 1 : 0)
                        && (!populated || profile.GetRecord("skill_deep_sight").Progress == 40),
                        "AUD-10 invalid skill ids leave both fresh and populated profiles untouched.");
                }
                AssertMasteryOperationFacts(profile.IncreaseProgressWithResult("skill_deep_sight", 50, null),
                    50, 0, 0, false);
                foreach (int amount in new[] { 0, -1, int.MinValue })
                {
                    AssertMasteryOperationFacts(profile.IncreaseProgressWithResult("skill_deep_sight", amount, skills),
                        amount, 0, 0, false);
                }
                Assert(profile.Records.Count == (populated ? 1 : 0)
                    && (!populated || profile.GetRecord("skill_deep_sight").Progress == 40),
                    "AUD-10 missing registry and nonpositive detailed grants create or mutate no records.");
            }
            // The legacy first-nonpositive-record side effect is intentionally compatibility-only.
            foreach (int amount in new[] { 0, -10, int.MinValue })
            {
                var legacy = new MasteryProfile();
                Assert(!legacy.IncreaseProgress("skill_deep_sight", amount, skills)
                    && legacy.Records.Count == 1 && legacy.GetRecord("skill_deep_sight").Progress == 0,
                    "AUD-10 legacy bool profile API preserves AUD-09's empty-record compatibility.");
            }
        }

        private static MentorTeachingResult Aud10Teaching(int amount, string skillId = "skill_deep_sight", bool available = true)
        {
            return new MentorTeachingResult(available, MentorId.Lyra, skillId, 0, 0, amount,
                available ? MentorTeachingBlockReason.None : MentorTeachingBlockReason.TrustInsufficient, "");
        }

        private static void AssertTeachingRejectionContracts()
        {
            var skills = BuildTestSkillRegistry();
            foreach (bool populated in new[] { false, true })
            {
                var profile = new MasteryProfile();
                if (populated) profile.RestoreRecord("skill_deep_sight", 40, skills);
                AssertRejectedAud10Teaching(null, profile, skills, MentorTeachingApplicationBlockReason.TeachingBlocked);
                AssertRejectedAud10Teaching(Aud10Teaching(50, available: false), profile, skills,
                    MentorTeachingApplicationBlockReason.TeachingBlocked);
                AssertRejectedAud10Teaching(Aud10Teaching(50, "skill_unknown"), profile, skills,
                    MentorTeachingApplicationBlockReason.UnknownSkill);
                AssertRejectedAud10Teaching(Aud10Teaching(50, null), profile, skills,
                    MentorTeachingApplicationBlockReason.UnknownSkill);
                AssertRejectedAud10Teaching(Aud10Teaching(50), profile, null,
                    MentorTeachingApplicationBlockReason.UnknownSkill);
                foreach (int amount in new[] { 0, -10, int.MinValue })
                    AssertRejectedAud10Teaching(Aud10Teaching(amount), profile, skills,
                        MentorTeachingApplicationBlockReason.NoProgressGranted);
                Assert(profile.Records.Count == (populated ? 1 : 0)
                    && (!populated || profile.GetRecord("skill_deep_sight").Progress == 40),
                    "AUD-10 rejected teachings do not create or mutate profile records.");
            }
            AssertRejectedAud10Teaching(Aud10Teaching(50), null, skills,
                MentorTeachingApplicationBlockReason.MasteryProfileMissing);
            AssertRejectedAud10Teaching(null, null, null, MentorTeachingApplicationBlockReason.TeachingBlocked);
            var mastered = new MasteryProfile();
            mastered.RestoreRecord("skill_deep_sight", 100, skills);
            foreach (int amount in new[] { 50, int.MaxValue, 0, -10 })
            {
                var result = new MentorTeachingApplicationService().Apply(Aud10Teaching(amount), mastered, skills);
                Assert(!result.Applied && result.ProgressApplied == 0 && result.WasAlreadyMastered
                    && result.BlockReason == MentorTeachingApplicationBlockReason.AlreadyMastered
                    && mastered.GetRecord("skill_deep_sight").Progress == 100,
                    "AUD-10 already-mastered precedence and repeated rejection remain compatible.");
            }
        }

        private static void AssertRejectedAud10Teaching(MentorTeachingResult teaching, MasteryProfile profile,
            SkillDefinitionRegistry skills, MentorTeachingApplicationBlockReason reason)
        {
            var result = new MentorTeachingApplicationService().Apply(teaching, profile, skills);
            Assert(!result.Applied && result.ProgressApplied == 0 && !result.WasAlreadyMastered
                && result.BlockReason == reason && result.SkillId == (teaching == null ? "" : teaching.TaughtSkillId),
                "AUD-10 preserves safe rejection facts: " + reason);
        }

        private static void AssertMasteryResultIsImmutableFact()
        {
            Assert(typeof(MasteryProgressResult).IsSealed, "AUD-10 operation result cannot be subclassed.");
            foreach (var property in typeof(MasteryProgressResult).GetProperties())
                Assert(!property.CanWrite, "AUD-10 operation result property is immutable: " + property.Name);
            var record = new MasteryRecord("skill_deep_sight", 90);
            var result = record.AddProgressWithResult(50);
            record.SetProgress(20);
            AssertMasteryOperationFacts(result, 50, 10, 40, true);
            Assert(record.Progress == 20, "AUD-10 result is a detached operation fact, not live state or a claim token.");
        }

        private static void AssertMasteryDeltaMentorSnapshotWorkflow()
        {
            const string skillId = "skill_deep_sight";
            var skills = BuildTestSkillRegistry();
            var mentors = BuildTestMentorRegistry();
            var relationships = new MentorRelationshipProfile();
            relationships.Discover(MentorId.Lyra, mentors);
            relationships.IncreaseTrust(MentorId.Lyra, 25, mentors);
            var profile = new MasteryProfile();
            profile.IncreaseProgress(skillId, 90, skills);
            var teaching = new MentorTeachingService().Evaluate(
                new MentorTeachingDefinition(MentorId.Lyra, skillId, 20, 50), relationships, mentors, skills);
            Assert(teaching.IsAvailable, "AUD-10 integrated workflow resolves a real available mentor teaching.");
            var application = new MentorTeachingApplicationService().Apply(teaching, profile, skills);
            Assert(application.Applied && application.ProgressApplied == 10 && profile.IsMastered(skillId),
                "AUD-10 integrated teaching caps once and reports only the ten applied points.");
            Assert(relationships.GetTrust(MentorId.Lyra) == 25,
                "AUD-10 applying mastery does not mutate mentor relationship state.");

            var snapshots = new V2ProgressSnapshotService();
            var serializer = new V2ProgressSnapshotJsonSerializer();
            var captured = snapshots.CreateSnapshot(new CodexProfile(), new MissionJournalProfile(),
                new UnlockProfile(), new ClassProfile(), profile);
            var loaded = serializer.Deserialize(serializer.Serialize(captured));
            Assert(loaded.SchemaVersion == 1 && loaded.MasteryRecords.Count == 1
                && loaded.MasteryRecords[0].Progress == 100 && loaded.MasteryRecords[0].Rank == MasteryRank.Mastered
                && new V2ProgressSnapshotValidationService().IsValid(loaded, skills),
                "AUD-10 operation facts do not alter the existing mastery snapshot contract.");
            foreach (bool populated in new[] { false, true })
            {
                var restored = new MasteryProfile();
                if (populated) restored.IncreaseProgress(skillId, 40, skills);
                Assert(snapshots.RestoreMasteryProfile(loaded, restored, skills)
                    && restored.GetRecord(skillId).Progress == 100,
                    "AUD-10 mastery JSON round trip restores into fresh and populated state.");
                AssertMasteryOperationFacts(restored.IncreaseProgressWithResult(skillId, int.MaxValue, skills),
                    int.MaxValue, 0, 0, false);
                var repeated = new MentorTeachingApplicationService().Apply(teaching, restored, skills);
                Assert(!repeated.Applied && repeated.ProgressApplied == 0 && repeated.WasAlreadyMastered
                    && repeated.BlockReason == MentorTeachingApplicationBlockReason.AlreadyMastered,
                    "AUD-10 restored mastered records cannot produce another delta or raw overflow.");
            }
            var legacy = serializer.Deserialize("{\"SchemaVersion\":1,\"MasteryRecords\":[{\"SkillId\":\"skill_deep_sight\",\"Progress\":90,\"Rank\":0}]}");
            var legacyProfile = new MasteryProfile();
            Assert(snapshots.RestoreMasteryProfile(legacy, legacyProfile, skills)
                && legacyProfile.GetRecord(skillId).Rank == MasteryRank.Practiced,
                "AUD-10 legacy snapshots retain progress-derived rank restoration.");
            AssertMasteryOperationFacts(legacyProfile.IncreaseProgressWithResult(skillId, 50, skills), 50, 10, 40, true);
            var empty = new MasteryProfile();
            Assert(snapshots.RestoreMasteryProfile(serializer.Deserialize("{\"SchemaVersion\":1}"), empty, skills)
                && empty.Records.Count == 0, "AUD-10 old JSON without mastery remains safe.");
        }
    }
}
