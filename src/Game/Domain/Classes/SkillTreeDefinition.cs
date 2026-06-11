using System.Collections.Generic;
using Game.Domain.Skills;

namespace Game.Domain.Classes
{
    public class SkillTreeDefinition
    {
        private readonly Dictionary<string, SkillTreeNodeDefinition> nodesById =
            new Dictionary<string, SkillTreeNodeDefinition>();

        public string Id { get; private set; }

        public IReadOnlyDictionary<string, SkillTreeNodeDefinition> Nodes
        {
            get { return nodesById; }
        }

        public SkillTreeDefinition()
            : this("")
        {
        }

        public SkillTreeDefinition(string id)
        {
            Id = id;
        }

        public bool RegisterNode(
            SkillTreeNodeDefinition node,
            SkillDefinitionRegistry skillRegistry)
        {
            if (!IsValidNode(node, skillRegistry)
                || nodesById.ContainsKey(node.Id)
                || !HasRegisteredPrerequisites(node))
            {
                return false;
            }

            nodesById.Add(node.Id, node);
            return true;
        }

        public bool Contains(string nodeId)
        {
            return !string.IsNullOrWhiteSpace(nodeId)
                && nodesById.ContainsKey(nodeId);
        }

        public SkillTreeNodeDefinition Get(string nodeId)
        {
            SkillTreeNodeDefinition node;
            return !string.IsNullOrWhiteSpace(nodeId)
                && nodesById.TryGetValue(nodeId, out node)
                    ? node
                    : null;
        }

        public static bool IsValidNode(
            SkillTreeNodeDefinition node,
            SkillDefinitionRegistry skillRegistry)
        {
            return node != null
                && !string.IsNullOrWhiteSpace(node.Id)
                && skillRegistry != null
                && skillRegistry.Contains(node.SkillId);
        }

        private bool HasRegisteredPrerequisites(SkillTreeNodeDefinition node)
        {
            foreach (var requiredNodeId in node.RequiredNodeIds)
            {
                if (!nodesById.ContainsKey(requiredNodeId))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
