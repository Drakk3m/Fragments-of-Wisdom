using System.Collections.Generic;

namespace Game.Domain.Classes
{
    public class SkillTreeProgressProfile
    {
        private readonly HashSet<string> discoveredNodeIds = new HashSet<string>();
        private readonly HashSet<string> unlockedNodeIds = new HashSet<string>();

        public IReadOnlyCollection<string> DiscoveredNodeIds
        {
            get { return discoveredNodeIds; }
        }

        public IReadOnlyCollection<string> UnlockedNodeIds
        {
            get { return unlockedNodeIds; }
        }

        public bool DiscoverNode(
            string nodeId,
            SkillTreeDefinition skillTree)
        {
            if (skillTree == null || !skillTree.Contains(nodeId))
            {
                return false;
            }

            return discoveredNodeIds.Add(nodeId);
        }

        public bool UnlockNode(
            string nodeId,
            SkillTreeDefinition skillTree)
        {
            if (skillTree == null || !skillTree.Contains(nodeId))
            {
                return false;
            }

            discoveredNodeIds.Add(nodeId);
            return unlockedNodeIds.Add(nodeId);
        }

        public bool IsDiscovered(string nodeId)
        {
            return !string.IsNullOrWhiteSpace(nodeId)
                && discoveredNodeIds.Contains(nodeId);
        }

        public bool IsUnlocked(string nodeId)
        {
            return !string.IsNullOrWhiteSpace(nodeId)
                && unlockedNodeIds.Contains(nodeId);
        }
    }
}
