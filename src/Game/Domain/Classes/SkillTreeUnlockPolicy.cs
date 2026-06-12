using Game.Domain.Skills;

namespace Game.Domain.Classes
{
    public class SkillTreeUnlockPolicy
    {
        public SkillTreeUnlockResult Evaluate(
            string nodeId,
            SkillTreeDefinition skillTree,
            SkillTreeProgressProfile progressProfile,
            SkillDefinitionRegistry skillRegistry)
        {
            var node = skillTree == null ? null : skillTree.Get(nodeId);

            if (node == null)
            {
                return Blocked(
                    nodeId,
                    "",
                    false,
                    false,
                    "",
                    SkillTreeUnlockBlockReason.UnknownNode);
            }

            bool isDiscovered = progressProfile != null
                && progressProfile.IsDiscovered(node.Id, skillTree);
            bool isAlreadyUnlocked = progressProfile != null
                && progressProfile.IsUnlocked(node.Id, skillTree);

            if (skillRegistry == null || !skillRegistry.Contains(node.SkillId))
            {
                return Blocked(
                    node.Id,
                    node.SkillId,
                    isDiscovered,
                    isAlreadyUnlocked,
                    "",
                    SkillTreeUnlockBlockReason.UnknownSkill);
            }

            if (!isDiscovered)
            {
                return Blocked(
                    node.Id,
                    node.SkillId,
                    false,
                    isAlreadyUnlocked,
                    "",
                    SkillTreeUnlockBlockReason.NodeNotDiscovered);
            }

            foreach (var requiredNodeId in node.RequiredNodeIds)
            {
                if (progressProfile == null
                    || !progressProfile.IsUnlocked(requiredNodeId, skillTree))
                {
                    return Blocked(
                        node.Id,
                        node.SkillId,
                        true,
                        isAlreadyUnlocked,
                        requiredNodeId,
                        SkillTreeUnlockBlockReason.PrerequisiteMissing);
                }
            }

            return new SkillTreeUnlockResult(
                node.Id,
                node.SkillId,
                true,
                true,
                isAlreadyUnlocked,
                "",
                SkillTreeUnlockBlockReason.None);
        }

        private static SkillTreeUnlockResult Blocked(
            string nodeId,
            string skillId,
            bool isDiscovered,
            bool isAlreadyUnlocked,
            string missingPrerequisiteNodeId,
            SkillTreeUnlockBlockReason blockReason)
        {
            return new SkillTreeUnlockResult(
                nodeId,
                skillId,
                false,
                isDiscovered,
                isAlreadyUnlocked,
                missingPrerequisiteNodeId,
                blockReason);
        }
    }
}
