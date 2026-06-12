using System.Collections.Generic;

namespace Game.Domain.Progress
{
    public class V2SkillTreeProgressScopeSnapshot
    {
        public string SkillTreeId { get; private set; }
        public IReadOnlyList<string> DiscoveredNodeIds { get; private set; }
        public IReadOnlyList<string> UnlockedNodeIds { get; private set; }

        public V2SkillTreeProgressScopeSnapshot(
            string skillTreeId,
            IReadOnlyList<string> discoveredNodeIds,
            IReadOnlyList<string> unlockedNodeIds)
        {
            SkillTreeId = skillTreeId;
            DiscoveredNodeIds = discoveredNodeIds == null
                ? new List<string>()
                : discoveredNodeIds;
            UnlockedNodeIds = unlockedNodeIds == null
                ? new List<string>()
                : unlockedNodeIds;
        }
    }
}
