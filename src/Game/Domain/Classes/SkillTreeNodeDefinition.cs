using System.Collections.Generic;

namespace Game.Domain.Classes
{
    public class SkillTreeNodeDefinition
    {
        public string Id { get; private set; }
        public string SkillId { get; private set; }
        public IReadOnlyList<string> RequiredNodeIds { get; private set; }

        public SkillTreeNodeDefinition(
            string id,
            string skillId,
            IReadOnlyList<string> requiredNodeIds)
        {
            Id = id;
            SkillId = skillId;
            RequiredNodeIds = CopyRequiredNodeIds(requiredNodeIds);
        }

        private static IReadOnlyList<string> CopyRequiredNodeIds(IReadOnlyList<string> sourceNodeIds)
        {
            var nodeIds = new List<string>();

            if (sourceNodeIds == null)
            {
                return nodeIds;
            }

            foreach (var nodeId in sourceNodeIds)
            {
                if (!string.IsNullOrWhiteSpace(nodeId))
                {
                    nodeIds.Add(nodeId);
                }
            }

            return nodeIds;
        }
    }
}
