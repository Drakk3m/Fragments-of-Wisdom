using System.Collections.Generic;

namespace Game.Domain.Classes
{
    public class SkillTreeProgressProfile
    {
        private readonly Dictionary<string, HashSet<string>> discoveredNodeIdsByTreeId =
            new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> unlockedNodeIdsByTreeId =
            new Dictionary<string, HashSet<string>>();

        public IReadOnlyCollection<string> DiscoveredNodeIds
        {
            get { return BuildAggregate(discoveredNodeIdsByTreeId); }
        }

        public IReadOnlyCollection<string> UnlockedNodeIds
        {
            get { return BuildAggregate(unlockedNodeIdsByTreeId); }
        }

        public IReadOnlyCollection<string> SkillTreeIds
        {
            get
            {
                var skillTreeIds = new HashSet<string>();

                foreach (var skillTreeId in discoveredNodeIdsByTreeId.Keys)
                {
                    skillTreeIds.Add(skillTreeId);
                }

                foreach (var skillTreeId in unlockedNodeIdsByTreeId.Keys)
                {
                    skillTreeIds.Add(skillTreeId);
                }

                return skillTreeIds;
            }
        }

        public bool DiscoverNode(
            string nodeId,
            SkillTreeDefinition skillTree)
        {
            if (skillTree == null || !skillTree.Contains(nodeId))
            {
                return false;
            }

            return GetOrCreate(discoveredNodeIdsByTreeId, skillTree.Id).Add(nodeId);
        }

        // Low-level mutation for policy-enforced application and snapshot restoration.
        internal bool RecordUnlockedNode(
            string nodeId,
            SkillTreeDefinition skillTree)
        {
            if (skillTree == null || !skillTree.Contains(nodeId))
            {
                return false;
            }

            GetOrCreate(discoveredNodeIdsByTreeId, skillTree.Id).Add(nodeId);
            return GetOrCreate(unlockedNodeIdsByTreeId, skillTree.Id).Add(nodeId);
        }

        public bool IsDiscovered(string nodeId)
        {
            return ContainsInAnyScope(discoveredNodeIdsByTreeId, nodeId);
        }

        public bool IsDiscovered(
            string nodeId,
            SkillTreeDefinition skillTree)
        {
            return skillTree != null
                && ContainsInScope(discoveredNodeIdsByTreeId, skillTree.Id, nodeId);
        }

        public bool IsUnlocked(string nodeId)
        {
            return ContainsInAnyScope(unlockedNodeIdsByTreeId, nodeId);
        }

        public bool IsUnlocked(
            string nodeId,
            SkillTreeDefinition skillTree)
        {
            return skillTree != null
                && ContainsInScope(unlockedNodeIdsByTreeId, skillTree.Id, nodeId);
        }

        public IReadOnlyCollection<string> GetDiscoveredNodeIds(string skillTreeId)
        {
            return CopyScope(discoveredNodeIdsByTreeId, skillTreeId);
        }

        public IReadOnlyCollection<string> GetUnlockedNodeIds(string skillTreeId)
        {
            return CopyScope(unlockedNodeIdsByTreeId, skillTreeId);
        }

        private static HashSet<string> GetOrCreate(
            Dictionary<string, HashSet<string>> nodeIdsByTreeId,
            string skillTreeId)
        {
            string scopeId = skillTreeId ?? "";
            HashSet<string> nodeIds;

            if (!nodeIdsByTreeId.TryGetValue(scopeId, out nodeIds))
            {
                nodeIds = new HashSet<string>();
                nodeIdsByTreeId.Add(scopeId, nodeIds);
            }

            return nodeIds;
        }

        private static bool ContainsInAnyScope(
            Dictionary<string, HashSet<string>> nodeIdsByTreeId,
            string nodeId)
        {
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                return false;
            }

            foreach (var nodeIds in nodeIdsByTreeId.Values)
            {
                if (nodeIds.Contains(nodeId))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsInScope(
            Dictionary<string, HashSet<string>> nodeIdsByTreeId,
            string skillTreeId,
            string nodeId)
        {
            HashSet<string> nodeIds;
            return !string.IsNullOrWhiteSpace(nodeId)
                && nodeIdsByTreeId.TryGetValue(skillTreeId ?? "", out nodeIds)
                && nodeIds.Contains(nodeId);
        }

        private static IReadOnlyCollection<string> CopyScope(
            Dictionary<string, HashSet<string>> nodeIdsByTreeId,
            string skillTreeId)
        {
            HashSet<string> nodeIds;
            return nodeIdsByTreeId.TryGetValue(skillTreeId ?? "", out nodeIds)
                ? new List<string>(nodeIds)
                : new List<string>();
        }

        private static IReadOnlyCollection<string> BuildAggregate(
            Dictionary<string, HashSet<string>> nodeIdsByTreeId)
        {
            var aggregate = new HashSet<string>();

            foreach (var nodeIds in nodeIdsByTreeId.Values)
            {
                foreach (var nodeId in nodeIds)
                {
                    aggregate.Add(nodeId);
                }
            }

            return aggregate;
        }
    }
}
