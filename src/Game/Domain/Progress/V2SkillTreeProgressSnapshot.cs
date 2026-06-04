using System.Collections.Generic;

namespace Game.Domain.Progress
{
    public class V2SkillTreeProgressSnapshot
    {
        public IReadOnlyList<string> DiscoveredNodeIds { get; private set; }
        public IReadOnlyList<string> UnlockedNodeIds { get; private set; }

        public V2SkillTreeProgressSnapshot(
            IReadOnlyList<string> discoveredNodeIds,
            IReadOnlyList<string> unlockedNodeIds)
        {
            DiscoveredNodeIds = discoveredNodeIds == null
                ? new List<string>()
                : discoveredNodeIds;
            UnlockedNodeIds = unlockedNodeIds == null
                ? new List<string>()
                : unlockedNodeIds;
        }
    }
}
