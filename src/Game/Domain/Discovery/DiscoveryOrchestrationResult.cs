using System.Collections.Generic;

namespace Game.Domain.Discovery
{
    public class DiscoveryOrchestrationResult
    {
        public int CodexEntriesDiscoveredCount { get; private set; }
        public IReadOnlyList<string> CodexEntryIdsDiscovered { get; private set; }
        public int MissionLeadsDiscoveredCount { get; private set; }
        public IReadOnlyList<string> MissionLeadTriggerIdsDiscovered { get; private set; }
        public int CodexEntriesAlreadyKnownCount { get; private set; }
        public int MissionLeadsAlreadyKnownCount { get; private set; }

        public DiscoveryOrchestrationResult(
            IReadOnlyList<string> codexEntryIdsDiscovered,
            IReadOnlyList<string> missionLeadTriggerIdsDiscovered,
            int codexEntriesAlreadyKnownCount,
            int missionLeadsAlreadyKnownCount)
        {
            CodexEntryIdsDiscovered = codexEntryIdsDiscovered;
            CodexEntriesDiscoveredCount = codexEntryIdsDiscovered.Count;
            MissionLeadTriggerIdsDiscovered = missionLeadTriggerIdsDiscovered;
            MissionLeadsDiscoveredCount = missionLeadTriggerIdsDiscovered.Count;
            CodexEntriesAlreadyKnownCount = codexEntriesAlreadyKnownCount;
            MissionLeadsAlreadyKnownCount = missionLeadsAlreadyKnownCount;
        }
    }
}
