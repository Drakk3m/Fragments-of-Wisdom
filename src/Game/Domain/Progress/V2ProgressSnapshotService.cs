using System;
using System.Collections.Generic;
using Game.Domain.Classes;
using Game.Domain.Codex;
using Game.Domain.Missions;
using Game.Domain.Skills;

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

        public V2ProgressSnapshot CreateSnapshot(
            CodexProfile codexProfile,
            MissionJournalProfile missionJournalProfile,
            UnlockProfile unlockProfile,
            ClassProfile classProfile)
        {
            if (classProfile == null)
            {
                throw new ArgumentNullException(nameof(classProfile));
            }

            var snapshot = CreateSnapshot(
                codexProfile,
                missionJournalProfile,
                unlockProfile);

            return new V2ProgressSnapshot(
                snapshot.SchemaVersion,
                snapshot.CodexEntries,
                snapshot.MissionLeads,
                snapshot.ActiveMissions,
                snapshot.Unlocks,
                BuildClassProfileSnapshot(classProfile));
        }

        public V2ProgressSnapshot CreateSnapshot(
            CodexProfile codexProfile,
            MissionJournalProfile missionJournalProfile,
            UnlockProfile unlockProfile,
            ClassProfile classProfile,
            MasteryProfile masteryProfile)
        {
            if (masteryProfile == null)
            {
                throw new ArgumentNullException(nameof(masteryProfile));
            }

            var snapshot = CreateSnapshot(
                codexProfile,
                missionJournalProfile,
                unlockProfile,
                classProfile);

            return new V2ProgressSnapshot(
                snapshot.SchemaVersion,
                snapshot.CodexEntries,
                snapshot.MissionLeads,
                snapshot.ActiveMissions,
                snapshot.Unlocks,
                snapshot.ClassProfile,
                BuildMasterySnapshots(masteryProfile.Records));
        }

        public V2ProgressSnapshot CreateSnapshot(
            CodexProfile codexProfile,
            MissionJournalProfile missionJournalProfile,
            UnlockProfile unlockProfile,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            MentorRelationshipProfile mentorRelationshipProfile)
        {
            if (mentorRelationshipProfile == null)
            {
                throw new ArgumentNullException(nameof(mentorRelationshipProfile));
            }

            var snapshot = CreateSnapshot(
                codexProfile,
                missionJournalProfile,
                unlockProfile,
                classProfile,
                masteryProfile);

            return new V2ProgressSnapshot(
                snapshot.SchemaVersion,
                snapshot.CodexEntries,
                snapshot.MissionLeads,
                snapshot.ActiveMissions,
                snapshot.Unlocks,
                snapshot.ClassProfile,
                snapshot.MasteryRecords,
                BuildMentorRelationshipSnapshots(mentorRelationshipProfile));
        }

        public V2ProgressSnapshot CreateSnapshot(
            CodexProfile codexProfile,
            MissionJournalProfile missionJournalProfile,
            UnlockProfile unlockProfile,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            MentorRelationshipProfile mentorRelationshipProfile,
            SkillTreeProgressProfile skillTreeProgressProfile)
        {
            if (skillTreeProgressProfile == null)
            {
                throw new ArgumentNullException(nameof(skillTreeProgressProfile));
            }

            var snapshot = CreateSnapshot(
                codexProfile,
                missionJournalProfile,
                unlockProfile,
                classProfile,
                masteryProfile,
                mentorRelationshipProfile);

            return new V2ProgressSnapshot(
                snapshot.SchemaVersion,
                snapshot.CodexEntries,
                snapshot.MissionLeads,
                snapshot.ActiveMissions,
                snapshot.Unlocks,
                snapshot.ClassProfile,
                snapshot.MasteryRecords,
                snapshot.MentorRelationships,
                BuildSkillTreeProgressSnapshot(skillTreeProgressProfile));
        }

        public V2ProgressSnapshot CreateSnapshot(
            CodexProfile codexProfile,
            MissionJournalProfile missionJournalProfile,
            UnlockProfile unlockProfile,
            ClassProfile classProfile,
            MasteryProfile masteryProfile,
            MentorRelationshipProfile mentorRelationshipProfile,
            SkillTreeProgressProfile skillTreeProgressProfile,
            ClassSkillTreeBindingRegistry classSkillTreeBindingRegistry)
        {
            if (classSkillTreeBindingRegistry == null)
            {
                throw new ArgumentNullException(nameof(classSkillTreeBindingRegistry));
            }

            var snapshot = CreateSnapshot(
                codexProfile,
                missionJournalProfile,
                unlockProfile,
                classProfile,
                masteryProfile,
                mentorRelationshipProfile,
                skillTreeProgressProfile);

            return new V2ProgressSnapshot(
                snapshot.SchemaVersion,
                snapshot.CodexEntries,
                snapshot.MissionLeads,
                snapshot.ActiveMissions,
                snapshot.Unlocks,
                snapshot.ClassProfile,
                snapshot.MasteryRecords,
                snapshot.MentorRelationships,
                snapshot.SkillTreeProgress,
                BuildClassSkillTreeBindingSnapshots(classSkillTreeBindingRegistry));
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

        public bool RestoreClassProfile(
            V2ProgressSnapshot snapshot,
            ClassProfile classProfile,
            ClassDefinitionRegistry registry)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            if (classProfile == null)
            {
                throw new ArgumentNullException(nameof(classProfile));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (!CanRestore(snapshot))
            {
                throw new InvalidOperationException("Unsupported V2 progress snapshot schema version.");
            }

            if (snapshot.ClassProfile == null
                || string.IsNullOrWhiteSpace(snapshot.ClassProfile.CurrentClassId))
            {
                return true;
            }

            return classProfile.AssignClassId(
                snapshot.ClassProfile.CurrentClassId,
                registry);
        }

        public bool RestoreMasteryProfile(
            V2ProgressSnapshot snapshot,
            MasteryProfile masteryProfile,
            SkillDefinitionRegistry skillRegistry)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            if (masteryProfile == null)
            {
                throw new ArgumentNullException(nameof(masteryProfile));
            }

            if (skillRegistry == null)
            {
                throw new ArgumentNullException(nameof(skillRegistry));
            }

            if (!CanRestore(snapshot))
            {
                throw new InvalidOperationException("Unsupported V2 progress snapshot schema version.");
            }

            bool allRestored = true;

            foreach (var masterySnapshot in snapshot.MasteryRecords)
            {
                bool restored = masteryProfile.RestoreRecord(
                    masterySnapshot.SkillId,
                    masterySnapshot.Progress,
                    skillRegistry);

                if (!restored)
                {
                    allRestored = false;
                }
            }

            return allRestored;
        }

        public bool RestoreMentorRelationshipProfile(
            V2ProgressSnapshot snapshot,
            MentorRelationshipProfile mentorRelationshipProfile,
            MentorDefinitionRegistry mentorRegistry)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            if (mentorRelationshipProfile == null)
            {
                throw new ArgumentNullException(nameof(mentorRelationshipProfile));
            }

            if (mentorRegistry == null)
            {
                throw new ArgumentNullException(nameof(mentorRegistry));
            }

            if (!CanRestore(snapshot))
            {
                throw new InvalidOperationException("Unsupported V2 progress snapshot schema version.");
            }

            bool allRestored = true;

            foreach (var mentorSnapshot in snapshot.MentorRelationships)
            {
                bool restored = mentorRelationshipProfile.RestoreRelationship(
                    mentorSnapshot.MentorId,
                    mentorSnapshot.IsDiscovered,
                    mentorSnapshot.Trust,
                    mentorSnapshot.IsAvailable,
                    mentorRegistry);

                if (!restored)
                {
                    allRestored = false;
                }
            }

            return allRestored;
        }

        public bool RestoreSkillTreeProgressProfile(
            V2ProgressSnapshot snapshot,
            SkillTreeProgressProfile skillTreeProgressProfile,
            SkillTreeDefinition skillTree)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            if (skillTreeProgressProfile == null)
            {
                throw new ArgumentNullException(nameof(skillTreeProgressProfile));
            }

            if (skillTree == null)
            {
                throw new ArgumentNullException(nameof(skillTree));
            }

            if (!CanRestore(snapshot))
            {
                throw new InvalidOperationException("Unsupported V2 progress snapshot schema version.");
            }

            foreach (var nodeId in snapshot.SkillTreeProgress.DiscoveredNodeIds)
            {
                if (!skillTree.Contains(nodeId))
                {
                    return false;
                }
            }

            foreach (var nodeId in snapshot.SkillTreeProgress.UnlockedNodeIds)
            {
                if (!skillTree.Contains(nodeId))
                {
                    return false;
                }
            }

            foreach (var nodeId in snapshot.SkillTreeProgress.DiscoveredNodeIds)
            {
                skillTreeProgressProfile.DiscoverNode(nodeId, skillTree);
            }

            foreach (var nodeId in snapshot.SkillTreeProgress.UnlockedNodeIds)
            {
                skillTreeProgressProfile.UnlockNode(nodeId, skillTree);
            }

            return true;
        }

        public bool RestoreClassSkillTreeBindings(
            V2ProgressSnapshot snapshot,
            ClassSkillTreeBindingRegistry bindingRegistry,
            ClassDefinitionRegistry classRegistry)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            if (bindingRegistry == null)
            {
                throw new ArgumentNullException(nameof(bindingRegistry));
            }

            if (classRegistry == null)
            {
                throw new ArgumentNullException(nameof(classRegistry));
            }

            if (!CanRestore(snapshot))
            {
                throw new InvalidOperationException("Unsupported V2 progress snapshot schema version.");
            }

            var restoredClassIds = new HashSet<string>();

            foreach (var bindingSnapshot in snapshot.ClassSkillTreeBindings)
            {
                if (bindingSnapshot == null
                    || string.IsNullOrWhiteSpace(bindingSnapshot.ClassId)
                    || string.IsNullOrWhiteSpace(bindingSnapshot.SkillTreeId)
                    || !classRegistry.Contains(bindingSnapshot.ClassId)
                    || !bindingRegistry.ContainsSkillTree(bindingSnapshot.SkillTreeId)
                    || !restoredClassIds.Add(bindingSnapshot.ClassId))
                {
                    return false;
                }

                var existingBinding = bindingRegistry.GetBindingForClass(bindingSnapshot.ClassId);

                if (existingBinding != null
                    && existingBinding.SkillTreeId != bindingSnapshot.SkillTreeId)
                {
                    return false;
                }
            }

            foreach (var bindingSnapshot in snapshot.ClassSkillTreeBindings)
            {
                if (bindingRegistry.GetBindingForClass(bindingSnapshot.ClassId) != null)
                {
                    continue;
                }

                if (!bindingRegistry.RegisterBinding(
                    new ClassSkillTreeBinding(
                        bindingSnapshot.ClassId,
                        bindingSnapshot.SkillTreeId),
                    classRegistry))
                {
                    return false;
                }
            }

            return true;
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

        private static V2ClassProfileSnapshot BuildClassProfileSnapshot(ClassProfile classProfile)
        {
            return new V2ClassProfileSnapshot(classProfile.CurrentClassId);
        }

        private static IReadOnlyList<V2MasterySnapshot> BuildMasterySnapshots(
            IReadOnlyList<MasteryRecord> records)
        {
            var snapshots = new List<V2MasterySnapshot>();

            foreach (var record in records)
            {
                snapshots.Add(new V2MasterySnapshot(
                    record.SkillId,
                    record.Progress,
                    record.Rank));
            }

            return snapshots;
        }

        private static IReadOnlyList<V2MentorRelationshipSnapshot> BuildMentorRelationshipSnapshots(
            MentorRelationshipProfile mentorRelationshipProfile)
        {
            var snapshots = new List<V2MentorRelationshipSnapshot>();

            foreach (var mentorId in mentorRelationshipProfile.DiscoveredMentorIds)
            {
                snapshots.Add(new V2MentorRelationshipSnapshot(
                    mentorId,
                    true,
                    mentorRelationshipProfile.GetTrust(mentorId),
                    mentorRelationshipProfile.IsAvailable(mentorId)));
            }

            return snapshots;
        }

        private static V2SkillTreeProgressSnapshot BuildSkillTreeProgressSnapshot(
            SkillTreeProgressProfile skillTreeProgressProfile)
        {
            return new V2SkillTreeProgressSnapshot(
                CopyIds(skillTreeProgressProfile.DiscoveredNodeIds),
                CopyIds(skillTreeProgressProfile.UnlockedNodeIds));
        }

        private static IReadOnlyList<V2ClassSkillTreeBindingSnapshot> BuildClassSkillTreeBindingSnapshots(
            ClassSkillTreeBindingRegistry bindingRegistry)
        {
            var snapshots = new List<V2ClassSkillTreeBindingSnapshot>();

            foreach (var binding in bindingRegistry.Bindings.Values)
            {
                snapshots.Add(new V2ClassSkillTreeBindingSnapshot(
                    binding.ClassId,
                    binding.SkillTreeId));
            }

            return snapshots;
        }

        private static IReadOnlyList<string> CopyIds(IReadOnlyCollection<string> sourceIds)
        {
            var ids = new List<string>();

            foreach (var sourceId in sourceIds)
            {
                ids.Add(sourceId);
            }

            return ids;
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
