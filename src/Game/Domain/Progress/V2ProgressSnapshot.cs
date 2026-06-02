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
                null)
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
                null)
        {
        }

        public V2ProgressSnapshot(
            int schemaVersion,
            IReadOnlyList<V2CodexEntrySnapshot> codexEntries,
            IReadOnlyList<V2MissionLeadSnapshot> missionLeads,
            IReadOnlyList<V2ActiveMissionSnapshot> activeMissions,
            IReadOnlyList<V2UnlockSnapshot> unlocks,
            V2ClassProfileSnapshot classProfile)
        {
            SchemaVersion = schemaVersion;
            CodexEntries = codexEntries;
            MissionLeads = missionLeads;
            ActiveMissions = activeMissions;
            Unlocks = unlocks;
            ClassProfile = classProfile;
        }

        public bool IsCurrentVersion()
        {
            return SchemaVersion == V2ProgressSnapshotVersions.CurrentSchemaVersion;
        }
    }
}
