using System.Collections.Generic;

namespace Game.Domain.Progress
{
    public class V2ProgressSnapshot
    {
        public int SchemaVersion { get; private set; }
        public IReadOnlyList<V2CodexEntrySnapshot> CodexEntries { get; private set; }
        public IReadOnlyList<V2MissionLeadSnapshot> MissionLeads { get; private set; }

        public V2ProgressSnapshot(
            int schemaVersion,
            IReadOnlyList<V2CodexEntrySnapshot> codexEntries,
            IReadOnlyList<V2MissionLeadSnapshot> missionLeads)
        {
            SchemaVersion = schemaVersion;
            CodexEntries = codexEntries;
            MissionLeads = missionLeads;
        }

        public bool IsCurrentVersion()
        {
            return SchemaVersion == V2ProgressSnapshotVersions.CurrentSchemaVersion;
        }
    }
}
