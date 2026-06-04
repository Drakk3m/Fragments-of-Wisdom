using System.Collections.Generic;

namespace Game.Domain.Progress
{
    public class V2ProgressSnapshot
    {
        public int SchemaVersion { get; private set; }
        public IReadOnlyList<V2CodexEntrySnapshot> CodexEntries { get; private set; }
        public IReadOnlyList<V2MissionLeadSnapshot> MissionLeads { get; private set; }
        public IReadOnlyList<V2ActiveMissionSnapshot> ActiveMissions { get; private set; }
        public IReadOnlyList<V2UnlockSnapshot> Unlocks { get; private set; }
        public V2ClassProfileSnapshot ClassProfile { get; private set; }
        public IReadOnlyList<V2MasterySnapshot> MasteryRecords { get; private set; }
        public IReadOnlyList<V2MentorRelationshipSnapshot> MentorRelationships { get; private set; }
        public V2SkillTreeProgressSnapshot SkillTreeProgress { get; private set; }

        public V2ProgressSnapshot(
            int schemaVersion,
            IReadOnlyList<V2CodexEntrySnapshot> codexEntries,
            IReadOnlyList<V2MissionLeadSnapshot> missionLeads)
            : this(
                schemaVersion,
                codexEntries,
                missionLeads,
                new List<V2ActiveMissionSnapshot>(),
                new List<V2UnlockSnapshot>(),
                null)
        {
        }

        public V2ProgressSnapshot(
            int schemaVersion,
            IReadOnlyList<V2CodexEntrySnapshot> codexEntries,
            IReadOnlyList<V2MissionLeadSnapshot> missionLeads,
            IReadOnlyList<V2ActiveMissionSnapshot> activeMissions)
            : this(
                schemaVersion,
                codexEntries,
                missionLeads,
                activeMissions,
                new List<V2UnlockSnapshot>(),
                null,
                new List<V2MasterySnapshot>())
        {
        }

        public V2ProgressSnapshot(
            int schemaVersion,
            IReadOnlyList<V2CodexEntrySnapshot> codexEntries,
            IReadOnlyList<V2MissionLeadSnapshot> missionLeads,
            IReadOnlyList<V2ActiveMissionSnapshot> activeMissions,
            IReadOnlyList<V2UnlockSnapshot> unlocks)
            : this(
                schemaVersion,
                codexEntries,
                missionLeads,
                activeMissions,
                unlocks,
                null,
                new List<V2MasterySnapshot>())
        {
        }

        public V2ProgressSnapshot(
            int schemaVersion,
            IReadOnlyList<V2CodexEntrySnapshot> codexEntries,
            IReadOnlyList<V2MissionLeadSnapshot> missionLeads,
            IReadOnlyList<V2ActiveMissionSnapshot> activeMissions,
            IReadOnlyList<V2UnlockSnapshot> unlocks,
            V2ClassProfileSnapshot classProfile)
            : this(
                schemaVersion,
                codexEntries,
                missionLeads,
                activeMissions,
                unlocks,
                classProfile,
                new List<V2MasterySnapshot>())
        {
        }

        public V2ProgressSnapshot(
            int schemaVersion,
            IReadOnlyList<V2CodexEntrySnapshot> codexEntries,
            IReadOnlyList<V2MissionLeadSnapshot> missionLeads,
            IReadOnlyList<V2ActiveMissionSnapshot> activeMissions,
            IReadOnlyList<V2UnlockSnapshot> unlocks,
            V2ClassProfileSnapshot classProfile,
            IReadOnlyList<V2MasterySnapshot> masteryRecords)
            : this(
                schemaVersion,
                codexEntries,
                missionLeads,
                activeMissions,
                unlocks,
                classProfile,
                masteryRecords,
                new List<V2MentorRelationshipSnapshot>())
        {
        }

        public V2ProgressSnapshot(
            int schemaVersion,
            IReadOnlyList<V2CodexEntrySnapshot> codexEntries,
            IReadOnlyList<V2MissionLeadSnapshot> missionLeads,
            IReadOnlyList<V2ActiveMissionSnapshot> activeMissions,
            IReadOnlyList<V2UnlockSnapshot> unlocks,
            V2ClassProfileSnapshot classProfile,
            IReadOnlyList<V2MasterySnapshot> masteryRecords,
            IReadOnlyList<V2MentorRelationshipSnapshot> mentorRelationships)
            : this(
                schemaVersion,
                codexEntries,
                missionLeads,
                activeMissions,
                unlocks,
                classProfile,
                masteryRecords,
                mentorRelationships,
                null)
        {
        }

        public V2ProgressSnapshot(
            int schemaVersion,
            IReadOnlyList<V2CodexEntrySnapshot> codexEntries,
            IReadOnlyList<V2MissionLeadSnapshot> missionLeads,
            IReadOnlyList<V2ActiveMissionSnapshot> activeMissions,
            IReadOnlyList<V2UnlockSnapshot> unlocks,
            V2ClassProfileSnapshot classProfile,
            IReadOnlyList<V2MasterySnapshot> masteryRecords,
            IReadOnlyList<V2MentorRelationshipSnapshot> mentorRelationships,
            V2SkillTreeProgressSnapshot skillTreeProgress)
        {
            SchemaVersion = schemaVersion;
            CodexEntries = codexEntries;
            MissionLeads = missionLeads;
            ActiveMissions = activeMissions;
            Unlocks = unlocks;
            ClassProfile = classProfile;
            MasteryRecords = masteryRecords == null
                ? new List<V2MasterySnapshot>()
                : masteryRecords;
            MentorRelationships = mentorRelationships == null
                ? new List<V2MentorRelationshipSnapshot>()
                : mentorRelationships;
            SkillTreeProgress = skillTreeProgress == null
                ? new V2SkillTreeProgressSnapshot(new List<string>(), new List<string>())
                : skillTreeProgress;
        }

        public bool IsCurrentVersion()
        {
            return SchemaVersion == V2ProgressSnapshotVersions.CurrentSchemaVersion;
        }
    }
}
