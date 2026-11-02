using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Domain.Classes;
using Game.Domain.Codex;
using Game.Domain.Missions;
using Game.Domain.Player;
using Game.Domain.Progress;

namespace Game.SmokeTests
{
    public static partial class Program
    {
        private static void RunCodexRoundTripIntegrityTests()
        {
            AssertSupportedCodexRoundTrips(BuildAuditCodex(true, true));
            AssertSupportedCodexRoundTrips(BuildAuditCodex(true, false));
            AssertSupportedCodexRoundTrips(BuildAuditCodex(false, true));
            AssertHistoricalCodexJsonCompatibility();
            AssertCodexMergeAndDuplicateSemantics();
            AssertInvalidCodexFactsRejectedBeforeMutation();
            AssertCodexFileRoundTripWithoutRewards();
            AssertCodexSnapshotCopiesFactValues();
        }

        private static void AssertHistoricalCodexJsonCompatibility()
        {
            var serializer = new V2ProgressSnapshotJsonSerializer();
            var service = new V2ProgressSnapshotService();
            foreach (string json in new[]
            {
                "{\"SchemaVersion\":1}",
                "{\"SchemaVersion\":1,\"CodexEntries\":[],\"MissionLeads\":[],\"ActiveMissions\":[]}",
                "{\"SchemaVersion\":1,\"CodexEntries\":null,\"CodexDiscoveredEntryIds\":null,\"CodexCompletedEntryIds\":null}",
                "{\"SchemaVersion\":1,\"CodexEntries\":[],\"CodexDiscoveredEntryIds\":[],\"CodexCompletedEntryIds\":[]}"
            })
            {
                var snapshot = serializer.Deserialize(json);
                var destination = BuildAuditCodex(true, true);
                string before = CaptureCodexFacts(destination);
                Assert(new V2ProgressSnapshotValidationService().IsValid(snapshot)
                    && snapshot.CodexDiscoveredEntryIds.Count == 0 && snapshot.CodexCompletedEntryIds.Count == 0,
                    "AUD-03: absent, null and empty optional lists must normalize compatibly.");
                service.Restore(snapshot, destination, new MissionJournalProfile());
                Assert(CaptureCodexFacts(destination) == before,
                    "AUD-03: missing or empty saved facts must not erase existing progression.");
            }

            string historicalJson = "{\"SchemaVersion\":1,\"CodexEntries\":[{\"EntryId\":\"old.entry\","
                + "\"EntryType\":2,\"ConceptKey\":\"old.concept\",\"FirstSourceHookId\":\"old.hook\","
                + "\"FirstSourceTriggerId\":\"old.trigger\",\"NarrativeTags\":[\"old.tag\"]}]}";
            var oldProfile = new CodexProfile();
            service.Restore(serializer.Deserialize(historicalJson), oldProfile, new MissionJournalProfile());
            var expected = new CodexProfile();
            expected.DiscoverEntry(new CodexEntryRecord("old.entry", CodexEntryType.Warning,
                "old.concept", "old.hook", "old.trigger", new[] { "old.tag" }));
            Assert(CaptureCodexFacts(oldProfile) == CaptureCodexFacts(expected),
                "AUD-03: old enriched-only JSON retains metadata without inventing completion.");

            var legacy = serializer.Deserialize("{\"SchemaVersion\":1,"
                + "\"CodexDiscoveredEntryIds\":[\"legacy.only\"],\"CodexCompletedEntryIds\":[\"complete.only\"]}");
            var legacyProfile = new CodexProfile();
            service.Restore(legacy, legacyProfile, new MissionJournalProfile());
            Assert(legacyProfile.IsDiscovered("legacy.only") && !legacyProfile.IsCompleted("legacy.only")
                && legacyProfile.IsDiscovered("complete.only") && legacyProfile.IsCompleted("complete.only")
                && legacyProfile.DiscoveredEntries.Count == 0,
                "AUD-03: completion implies discovery under the existing domain contract, not enriched metadata.");
        }

        private static void AssertCodexMergeAndDuplicateSemantics()
        {
            var destination = BuildAuditCodex(true, false);
            destination.DiscoverEntry(AuditCodexRecord("audit.enriched", "destination.first"));
            var source = BuildAuditCodex(false, true);
            source.DiscoverEntry(AuditCodexRecord("audit.legacy.discovered", "saved.enrichment"));
            var service = new V2ProgressSnapshotService();
            var snapshot = service.CreateSnapshot(source, new MissionJournalProfile());
            service.Restore(snapshot, destination, new MissionJournalProfile());
            Assert(destination.GetDiscoveredEntry("audit.enriched").ConceptKey == "destination.first"
                && destination.GetDiscoveredEntry("audit.legacy.discovered") == null
                && destination.IsCompleted("audit.legacy.completed")
                && destination.IsCompleted("audit.enriched.completed"),
                "AUD-03: merge preserves existing first-discovery precedence and unions completion facts.");
            string merged = CaptureCodexFacts(destination);
            service.Restore(snapshot, destination, new MissionJournalProfile());
            Assert(CaptureCodexFacts(destination) == merged, "AUD-03: populated merge remains idempotent.");

            var record = snapshot.CodexEntries[0];
            var duplicates = CodexFactsSnapshot(new[] { record, record },
                new[] { record.EntryId, record.EntryId }, new[] { record.EntryId, record.EntryId });
            var fresh = new CodexProfile();
            service.Restore(duplicates, fresh, new MissionJournalProfile());
            Assert(fresh.DiscoveredEntryIds.Count == 1 && fresh.DiscoveredEntries.Count == 1
                && fresh.IsCompleted(record.EntryId),
                "AUD-03: overlapping and repeated persisted facts do not duplicate domain entries.");
        }

        private static V2ProgressSnapshot CodexFactsSnapshot(
            V2CodexEntrySnapshot[] entries, string[] discovered, string[] completed)
        {
            return new V2ProgressSnapshot(1, entries, Array.Empty<V2MissionLeadSnapshot>(),
                Array.Empty<V2ActiveMissionSnapshot>(), Array.Empty<V2UnlockSnapshot>(),
                null, null, null, null, null, discovered, completed);
        }

        private static void AssertInvalidCodexFactsRejectedBeforeMutation()
        {
            var good = new V2CodexEntrySnapshot("new.valid", CodexEntryType.Concept,
                "concept", "hook", "trigger", Array.Empty<string>());
            var invalidSnapshots = new[]
            {
                CodexFactsSnapshot(new[] { good }, new[] { "new.legacy", "" }, Array.Empty<string>()),
                CodexFactsSnapshot(new[] { good }, Array.Empty<string>(), new[] { "new.complete", " " }),
                CodexFactsSnapshot(new[] { good }, new string[] { null }, Array.Empty<string>()),
                CodexFactsSnapshot(new[] { good }, Array.Empty<string>(), new string[] { null }),
                CodexFactsSnapshot(new[] { good, null }, Array.Empty<string>(), Array.Empty<string>()),
                CodexFactsSnapshot(new[] { good, new V2CodexEntrySnapshot("", CodexEntryType.Concept,
                    "", "", "", Array.Empty<string>()) }, Array.Empty<string>(), Array.Empty<string>()),
                CodexFactsSnapshot(new[] { good, new V2CodexEntrySnapshot("bad.tags", CodexEntryType.Concept,
                    "", "", "", null) }, Array.Empty<string>(), Array.Empty<string>()),
                CodexFactsSnapshot(null, Array.Empty<string>(), Array.Empty<string>())
            };
            foreach (var snapshot in invalidSnapshots)
            {
                var destination = BuildAuditCodex(true, true);
                string before = CaptureCodexFacts(destination);
                Assert(!new V2ProgressSnapshotValidationService().IsValid(snapshot),
                    "AUD-03: invalid Codex facts must fail semantic validation.");
                for (int attempt = 0; attempt < 2; attempt++)
                {
                    bool rejected = false;
                    try
                    {
                        new V2ProgressSnapshotService().Restore(snapshot, destination, new MissionJournalProfile());
                    }
                    catch (InvalidOperationException)
                    {
                        rejected = true;
                    }
                    Assert(rejected && CaptureCodexFacts(destination) == before,
                        "AUD-03: even a late invalid Codex fact must reject before any mutation, including retries.");
                }
            }

            var invalidJson = new V2ProgressSnapshotJsonSerializer().Deserialize(
                "{\"SchemaVersion\":1,\"CodexDiscoveredEntryIds\":[\"valid\",\" \"],\"CodexCompletedEntryIds\":[\"done\"]}");
            Assert(!new V2ProgressSnapshotValidationService().IsValid(invalidJson),
                "AUD-03: JSON must not silently discard invalid legacy IDs before validation.");
            string path = BuildTempSnapshotPath();
            try
            {
                File.WriteAllText(path, "{\"SchemaVersion\":1,\"CodexCompletedEntryIds\":[\"\"]}");
                bool rejected = false;
                try { new V2ProgressSnapshotFileRepository().Load(path); }
                catch (InvalidOperationException) { rejected = true; }
                Assert(rejected, "AUD-03: the real file loader rejects invalid Codex IDs through shared validation.");
            }
            finally { DeleteTempSnapshotFile(path); }
        }

        private static void AssertCodexFileRoundTripWithoutRewards()
        {
            var source = BuildAuditCodex(true, true);
            var service = new V2ProgressSnapshotService();
            var repository = new V2ProgressSnapshotFileRepository();
            string path = BuildTempSnapshotPath();
            try
            {
                repository.Save(path, service.CreateSnapshot(source, new MissionJournalProfile()));
                var player = new PlayerProfile();
                player.Wallet.AddCurrency("audit.currency", 17);
                string walletBefore = JsonSerializer.Serialize(player.Wallet.Currencies);
                var unlocks = new UnlockProfile();
                for (int attempt = 0; attempt < 2; attempt++)
                {
                    service.Restore(repository.Load(path), player.Codex, new MissionJournalProfile(), unlocks);
                    Assert(CaptureCodexFacts(player.Codex) == CaptureCodexFacts(source),
                        "AUD-03: file save/load into the player-owned Codex preserves all supported facts.");
                    Assert(JsonSerializer.Serialize(player.Wallet.Currencies) == walletBefore && unlocks.Unlocks.Count == 0,
                        "AUD-03: loading completion facts does not award currency or unlocks.");
                }
            }
            finally { DeleteTempSnapshotFile(path); }
        }

        private static void AssertCodexSnapshotCopiesFactValues()
        {
            var source = BuildAuditCodex(true, true);
            string before = CaptureCodexFacts(source);
            var service = new V2ProgressSnapshotService();
            var snapshot = service.CreateSnapshot(source, new MissionJournalProfile());
            source.CompleteEntry("later.entry");
            source.CompleteEntry("audit.enriched");
            var fresh = new CodexProfile();
            service.Restore(snapshot, fresh, new MissionJournalProfile());
            Assert(CaptureCodexFacts(fresh) == before,
                "AUD-03: snapshot ID lists must not alias later changes to domain discovery/completion sets.");
        }

        private static CodexProfile BuildAuditCodex(bool legacy, bool enriched)
        {
            var profile = new CodexProfile();
            if (legacy)
            {
                profile.DiscoverEntry("audit.legacy.discovered");
                profile.CompleteEntry("audit.legacy.completed");
            }

            if (enriched)
            {
                profile.DiscoverEntry(AuditCodexRecord("audit.enriched", "concept.first"));
                profile.DiscoverEntry(AuditCodexRecord("audit.enriched.completed", "concept.second"));
                profile.CompleteEntry("audit.enriched.completed");
            }

            if (legacy && enriched)
            {
                profile.DiscoverEntry("audit.enriched");
                profile.DiscoverEntry(AuditCodexRecord("audit.legacy.discovered", "ignored.by.domain"));
            }

            return profile;
        }

        private static CodexEntryRecord AuditCodexRecord(string id, string concept)
        {
            return new CodexEntryRecord(id, CodexEntryType.Practice, concept,
                "audit.hook", "audit.trigger", new[] { "tag.first", "tag.second" });
        }

        private static string CaptureCodexFacts(CodexProfile profile)
        {
            return JsonSerializer.Serialize(new
            {
                Discovered = profile.DiscoveredEntryIds.OrderBy(id => id, StringComparer.Ordinal).ToArray(),
                Completed = profile.DiscoveredEntryIds.Where(profile.IsCompleted)
                    .OrderBy(id => id, StringComparer.Ordinal).ToArray(),
                Records = profile.DiscoveredEntries.Select(record => new
                {
                    record.EntryId, record.EntryType, record.ConceptKey,
                    record.FirstSourceHookId, record.FirstSourceTriggerId,
                    Tags = record.NarrativeTags.ToArray()
                }).ToArray()
            });
        }

        private static void AssertSupportedCodexRoundTrips(CodexProfile source)
        {
            var service = new V2ProgressSnapshotService();
            var serializer = new V2ProgressSnapshotJsonSerializer();
            var journal = new MissionJournalProfile();
            var unlocks = new UnlockProfile();
            var classes = new ClassProfile();
            var mastery = new MasteryProfile();
            var mentors = new MentorRelationshipProfile();
            var trees = new SkillTreeProgressProfile();
            string before = CaptureCodexFacts(source);
            var snapshots = new[]
            {
                service.CreateSnapshot(source, journal),
                service.CreateSnapshot(source, journal, unlocks),
                service.CreateSnapshot(source, journal, unlocks, classes),
                service.CreateSnapshot(source, journal, unlocks, classes, mastery),
                service.CreateSnapshot(source, journal, unlocks, classes, mastery, mentors),
                service.CreateSnapshot(source, journal, unlocks, classes, mastery, mentors, trees),
                service.CreateSnapshot(source, journal, unlocks, classes, mastery, mentors, trees,
                    new ClassSkillTreeBindingRegistry())
            };

            foreach (var snapshot in snapshots)
            {
                var restoredSnapshot = serializer.Deserialize(serializer.Serialize(snapshot));
                var destination = new CodexProfile();
                service.Restore(restoredSnapshot, destination, new MissionJournalProfile());
                Assert(CaptureCodexFacts(destination) == before,
                    "AUD-03: all Codex facts must survive JSON round trip; discovered before="
                    + source.DiscoveredEntryIds.Count + ", after=" + destination.DiscoveredEntryIds.Count
                    + ", legacy completion=" + destination.IsCompleted("audit.legacy.completed")
                    + ", enriched records=" + destination.DiscoveredEntries.Count + ".");
                service.Restore(restoredSnapshot, destination, new MissionJournalProfile(), unlocks);
                Assert(CaptureCodexFacts(destination) == before && unlocks.Unlocks.Count == 0,
                    "AUD-03: repeated restore through either overload must not duplicate facts or grant unlocks.");
                Assert(CaptureCodexFacts(source) == before,
                    "AUD-03: creating snapshots must not mutate the source Codex.");
            }
        }
    }
}
