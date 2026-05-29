using System.Collections.Generic;

namespace Game.Domain.Progress
{
    public class V2ProgressSnapshot
    {
        public IReadOnlyList<V2CodexEntrySnapshot> CodexEntries { get; private set; }
        public IReadOnlyList<V2MissionLeadSnapshot> MissionLeads { get; private set; }

        public V2ProgressSnapshot(
            IReadOnlyList<V2CodexEntrySnapshot> codexEntries,
            IReadOnlyList<V2MissionLeadSnapshot> missionLeads)
        {
            CodexEntries = codexEntries;
            MissionLeads = missionLeads;
        }
    }
}
