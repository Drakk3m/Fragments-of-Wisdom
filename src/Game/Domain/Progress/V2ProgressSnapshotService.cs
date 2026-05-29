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
            return CreateSnapshot(
                codexProfile,
                missionJournalProfile,
                new UnlockProfile());
        }

        public V2ProgressSnapshot CreateSnapshot(
            CodexProfile codexProfile,
            MissionJournalProfile missionJournalProfile,
            UnlockProfile unlockProfile)
        {
            if (codexProfile == null)
            {
                throw new ArgumentNullException(nameof(codexProfile));
            }

            if (missionJournalProfile == null)
            {
                throw new ArgumentNullException(nameof(missionJournalProfile));
            }

            if (unlockProfile == null)
            {
                throw new ArgumentNullException(nameof(unlockProfile));
            }

            return new V2ProgressSnapshot(
                V2ProgressSnapshotVersions.CurrentSchemaVersion,
                BuildCodexSnapshots(codexProfile.DiscoveredEntries),
                BuildMissionLeadSnapshots(missionJournalProfile.AvailableLeads),
                BuildActiveMissionSnapshots(missionJournalProfile.ActiveMissions),
                BuildUnlockSnapshots(unlockProfile.Unlocks));
        }

        public bool CanRestore(V2ProgressSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return false;
            }

            return snapshot.IsCurrentVersion();
        }

        public void Restore(
            V2ProgressSnapshot snapshot,
            CodexProfile codexProfile,
            MissionJournalProfile missionJournalProfile)
        {
            Restore(
                snapshot,
                codexProfile,
                missionJournalProfile,
                new UnlockProfile());
        }

        public void Restore(
            V2ProgressSnapshot snapshot,
            CodexProfile codexProfile,
            MissionJournalProfile missionJournalProfile,
            UnlockProfile unlockProfile)
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

            if (unlockProfile == null)
            {
                throw new ArgumentNullException(nameof(unlockProfile));
            }

            if (!CanRestore(snapshot))
            {
                throw new InvalidOperationException("Unsupported V2 progress snapshot schema version.");
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

            foreach (var missionSnapshot in snapshot.ActiveMissions)
            {
                missionJournalProfile.RestoreMission(new ActiveMissionRecord(
                    missionSnapshot.TriggerId,
                    missionSnapshot.TriggerType,
                    missionSnapshot.State,
                    missionSnapshot.Priority,
                    missionSnapshot.SuggestedMissionFamily,
                    missionSnapshot.SourceHookId,
                    CopyTags(missionSnapshot.NarrativeTags)));
            }

            foreach (var unlockSnapshot in snapshot.Unlocks)
            {
                unlockProfile.RecordUnlock(new UnlockRecord(
                    unlockSnapshot.UnlockId,
                    unlockSnapshot.UnlockType,
                    unlockSnapshot.RequiredEntryType,
                    unlockSnapshot.RequiredEntryId,
                    unlockSnapshot.RequiredCount,
                    unlockSnapshot.RequiredPercentage,
                    CopyTags(unlockSnapshot.NarrativeTags)));
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

        private static IReadOnlyList<V2ActiveMissionSnapshot> BuildActiveMissionSnapshots(
            IReadOnlyList<ActiveMissionRecord> missions)
        {
            var snapshots = new List<V2ActiveMissionSnapshot>();

            foreach (var mission in missions)
            {
                snapshots.Add(new V2ActiveMissionSnapshot(
                    mission.TriggerId,
                    mission.State,
                    mission.TriggerType,
                    mission.Priority,
                    mission.SuggestedMissionFamily,
                    mission.SourceHookId,
                    CopyTags(mission.NarrativeTags)));
            }

            return snapshots;
        }

        private static IReadOnlyList<V2UnlockSnapshot> BuildUnlockSnapshots(
            IReadOnlyList<UnlockRecord> unlocks)
        {
            var snapshots = new List<V2UnlockSnapshot>();

            foreach (var unlock in unlocks)
            {
                snapshots.Add(new V2UnlockSnapshot(
                    unlock.UnlockId,
                    unlock.UnlockType,
                    unlock.RequiredEntryType,
                    unlock.RequiredEntryId,
                    unlock.RequiredCount,
                    unlock.RequiredPercentage,
                    CopyTags(unlock.NarrativeTags)));
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
