using Game.Domain.Skills;

namespace Game.Domain.Classes
{
    public class SkillTreeUnlockApplicationService
    {
        private readonly SkillTreeUnlockPolicy unlockPolicy;

        public SkillTreeUnlockApplicationService()
            : this(new SkillTreeUnlockPolicy())
        {
        }

        public SkillTreeUnlockApplicationService(SkillTreeUnlockPolicy unlockPolicy)
        {
            this.unlockPolicy = unlockPolicy ?? new SkillTreeUnlockPolicy();
        }

        public SkillTreeUnlockApplicationResult Apply(
            string nodeId,
            SkillTreeDefinition skillTree,
            SkillTreeProgressProfile progressProfile,
            SkillDefinitionRegistry skillRegistry)
        {
            var policyResult = unlockPolicy.Evaluate(
                nodeId,
                skillTree,
                progressProfile,
                skillRegistry);

            if (!policyResult.CanUnlock)
            {
                return new SkillTreeUnlockApplicationResult(
                    false,
                    policyResult.NodeId,
                    policyResult.BlockReason,
                    policyResult.IsAlreadyUnlocked);
            }

            if (policyResult.IsAlreadyUnlocked)
            {
                return new SkillTreeUnlockApplicationResult(
                    false,
                    policyResult.NodeId,
                    SkillTreeUnlockBlockReason.None,
                    true);
            }

            bool applied = progressProfile != null
                && progressProfile.RecordUnlockedNode(policyResult.NodeId, skillTree);

            return new SkillTreeUnlockApplicationResult(
                applied,
                policyResult.NodeId,
                SkillTreeUnlockBlockReason.None,
                false);
        }
    }
}
