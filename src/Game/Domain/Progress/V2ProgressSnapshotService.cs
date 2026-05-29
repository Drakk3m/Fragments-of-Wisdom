using System;
using System.Collections.Generic;
using Game.Domain.Codex;
using Game.Domain.Missions;

namespace Game.Domain.Progress
{
    public class V2ProgressSnapshotService
    {
        public V2ProgressSnapshot CreateSnapshot(
            CodexProfile codexProfile,
            MissionJournalProfile missionJournalProfile)
        {
            if (codexProfile == null)
            {
                throw new ArgumentNullException(nameof(codexProfile));
            }

            if (missionJournalProfile == null)
            {
                throw new ArgumentNullException(nameof(missionJournalProfile));
            }

            return new V2ProgressSnapshot(
                BuildCodexSnapshots(codexProfile.DiscoveredEntries),
                BuildMissionLeadSnapshots(missionJournalProfile.AvailableLeads));
        }

        public void Restore(
            V2ProgressSnapshot snapshot,
            CodexProfile codexProfile,
            MissionJournalProfile missionJournalProfile)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            if (codexProfile == null)
            {
                throw new ArgumentNullException(nameof(codexProfile));
            }

            if (missionJournalProfile == null)
            {
                throw new ArgumentNullException(nameof(missionJournalProfile));
            }

            foreach (var entrySnapshot in snapshot.CodexEntries)
            {
                codexProfile.DiscoverEntry(new CodexEntryRecord(
                    entrySnapshot.EntryId,
                    entrySnapshot.EntryType,
                    entrySnapshot.ConceptKey,
                    entrySnapshot.FirstSourceHookId,
                    entrySnapshot.FirstSourceTriggerId,
                    CopyTags(entrySnapshot.NarrativeTags)));
            }

            foreach (var leadSnapshot in snapshot.MissionLeads)
            {
                missionJournalProfile.DiscoverLead(new MissionLeadRecord(
                    leadSnapshot.TriggerId,
                    leadSnapshot.TriggerType,
                    leadSnapshot.Priority,
                    leadSnapshot.SuggestedMissionFamily,
                    leadSnapshot.SourceHookId,
                    CopyTags(leadSnapshot.NarrativeTags)));
            }
        }

        private static IReadOnlyList<V2CodexEntrySnapshot> BuildCodexSnapshots(
            IReadOnlyList<CodexEntryRecord> entries)
        {
            var snapshots = new List<V2CodexEntrySnapshot>();

            foreach (var entry in entries)
            {
                snapshots.Add(new V2CodexEntrySnapshot(
                    entry.EntryId,
                    entry.EntryType,
                    entry.ConceptKey,
                    entry.FirstSourceHookId,
                    entry.FirstSourceTriggerId,
                    CopyTags(entry.NarrativeTags)));
            }

            return snapshots;
        }

        private static IReadOnlyList<V2MissionLeadSnapshot> BuildMissionLeadSnapshots(
            IReadOnlyList<MissionLeadRecord> leads)
        {
            var snapshots = new List<V2MissionLeadSnapshot>();

            foreach (var lead in leads)
            {
                snapshots.Add(new V2MissionLeadSnapshot(
                    lead.TriggerId,
                    lead.TriggerType,
                    lead.Priority,
                    lead.SuggestedMissionFamily,
                    lead.SourceHookId,
                    CopyTags(lead.NarrativeTags)));
            }

            return snapshots;
        }

        private static IReadOnlyList<string> CopyTags(IReadOnlyList<string> sourceTags)
        {
            var tags = new List<string>();

            foreach (var sourceTag in sourceTags)
            {
                tags.Add(sourceTag);
            }

            return tags;
        }
    }
}
