using System.Collections.Generic;

namespace Game.Domain.Progress
{
    public class V2ProgressSnapshot
    {
        public int SchemaVersion { get; private set; }
        public IReadOnlyList<V2CodexEntrySnapshot> CodexEntries { get; private set; }
        public IReadOnlyList<V2MissionLeadSnapshot> MissionLeads { get; private set; }
        public IReadOnlyList<V2ActiveMissionSnapshot> ActiveMissions { get; private set; }

        public V2ProgressSnapshot(
            int schemaVersion,
            IReadOnlyList<V2CodexEntrySnapshot> codexEntries,
            IReadOnlyList<V2MissionLeadSnapshot> missionLeads)
            : this(
                schemaVersion,
                codexEntries,
                missionLeads,
                new List<V2ActiveMissionSnapshot>())
        {
        }

        public V2ProgressSnapshot(
            int schemaVersion,
            IReadOnlyList<V2CodexEntrySnapshot> codexEntries,
            IReadOnlyList<V2MissionLeadSnapshot> missionLeads,
            IReadOnlyList<V2ActiveMissionSnapshot> activeMissions)
        {
            SchemaVersion = schemaVersion;
            CodexEntries = codexEntries;
            MissionLeads = missionLeads;
            ActiveMissions = activeMissions;
        }

        public bool IsCurrentVersion()
        {
            return SchemaVersion == V2ProgressSnapshotVersions.CurrentSchemaVersion;
        }
    }
}
