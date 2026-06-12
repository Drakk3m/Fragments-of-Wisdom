using System.Collections.Generic;

namespace Game.Domain.Progress
{
    public class V2SkillTreeProgressSnapshot
    {
        public IReadOnlyList<string> DiscoveredNodeIds { get; private set; }
        public IReadOnlyList<string> UnlockedNodeIds { get; private set; }
        public IReadOnlyList<V2SkillTreeProgressScopeSnapshot> Scopes { get; private set; }

        public V2SkillTreeProgressSnapshot(
            IReadOnlyList<string> discoveredNodeIds,
            IReadOnlyList<string> unlockedNodeIds)
            : this(
                discoveredNodeIds,
                unlockedNodeIds,
                new List<V2SkillTreeProgressScopeSnapshot>())
        {
        }

        public V2SkillTreeProgressSnapshot(
            IReadOnlyList<V2SkillTreeProgressScopeSnapshot> scopes)
            : this(
                new List<string>(),
                new List<string>(),
                scopes)
        {
        }

        public V2SkillTreeProgressSnapshot(
            IReadOnlyList<string> discoveredNodeIds,
            IReadOnlyList<string> unlockedNodeIds,
            IReadOnlyList<V2SkillTreeProgressScopeSnapshot> scopes)
        {
            DiscoveredNodeIds = discoveredNodeIds == null
                ? new List<string>()
                : discoveredNodeIds;
            UnlockedNodeIds = unlockedNodeIds == null
                ? new List<string>()
                : unlockedNodeIds;
            Scopes = scopes == null
                ? new List<V2SkillTreeProgressScopeSnapshot>()
                : scopes;
        }
    }
}
