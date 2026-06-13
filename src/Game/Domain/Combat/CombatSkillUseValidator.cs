using Game.Domain.Classes;
using Game.Domain.Codex;
using Game.Domain.Skills;

namespace Game.Domain.Combat
{
    public class CombatSkillUseValidator
    {
        private readonly SkillAccessPolicy skillAccessPolicy;
        private readonly CombatSkillAccessService combatSkillAccessService;

        public CombatSkillUseValidator()
            : this(
                new SkillAccessPolicy(),
                new CombatSkillAccessService())
        {
        }

        public CombatSkillUseValidator(
            SkillAccessPolicy skillAccessPolicy,
            CombatSkillAccessService combatSkillAccessService)
        {
            this.skillAccessPolicy = skillAccessPolicy ?? new SkillAccessPolicy();
            this.combatSkillAccessService = combatSkillAccessService
                ?? new CombatSkillAccessService();
        }

        public CombatSkillUseValidationResult Validate(
            TurnRequest request,
            bool isPlayerAction,
            UnlockProfile unlockProfile,
            ClassProfile classProfile,
            ClassDefinitionRegistry classRegistry,
            MasteryProfile masteryProfile,
            SkillDefinitionRegistry skillRegistry)
        {
            if (request == null)
            {
                return Block(
                    string.Empty,
                    isPlayerAction,
                    CombatSkillUseBlockReason.RequestMissing);
            }

            if (request.ActionType != TurnActionType.Skill)
            {
                return Block(
                    string.Empty,
                    isPlayerAction,
                    CombatSkillUseBlockReason.NotSkillAction);
            }

            if (request.Skill == null)
            {
                return Block(
                    string.Empty,
                    isPlayerAction,
                    CombatSkillUseBlockReason.SkillMissing);
            }

            if (request.SkillProgress == null)
            {
                return Block(
                    request.Skill.Id,
                    isPlayerAction,
                    CombatSkillUseBlockReason.SkillProgressMissing);
            }

            if (request.SkillProgress.Definition == null)
            {
                return Block(
                    request.Skill.Id,
                    isPlayerAction,
                    CombatSkillUseBlockReason.SkillDefinitionMissing);
            }

            string skillId = request.Skill.Id;

            if (string.IsNullOrWhiteSpace(skillId)
                || string.IsNullOrWhiteSpace(request.SkillProgress.Definition.Id))
            {
                return Block(
                    skillId,
                    isPlayerAction,
                    CombatSkillUseBlockReason.SkillIdMissing);
            }

            if (skillId != request.SkillProgress.Definition.Id)
            {
                return Block(
                    skillId,
                    isPlayerAction,
                    CombatSkillUseBlockReason.SkillProgressIdMismatch);
            }

            if (!isPlayerAction)
            {
                return Allow(skillId, false, false, false);
            }

            bool v3AccessChecked = !string.IsNullOrEmpty(
                request.SkillProgress.Definition.RequiredUnlockId);

            if (v3AccessChecked)
            {
                if (unlockProfile == null)
                {
                    return Block(
                        skillId,
                        true,
                        CombatSkillUseBlockReason.V3UnlockProfileMissing,
                        true,
                        false);
                }

                var v3Result = skillAccessPolicy.Evaluate(
                    request.SkillProgress.Definition,
                    unlockProfile);

                if (!v3Result.IsAvailable)
                {
                    return Block(
                        skillId,
                        true,
                        CombatSkillUseBlockReason.V3SkillLocked,
                        true,
                        false,
                        v3Result.BlockReason);
                }
            }

            bool hasAnyV4Context = classProfile != null
                || classRegistry != null
                || masteryProfile != null
                || skillRegistry != null;
            bool hasCompleteV4Context = classProfile != null
                && classRegistry != null
                && masteryProfile != null
                && skillRegistry != null;

            if (hasAnyV4Context && !hasCompleteV4Context)
            {
                return Block(
                    skillId,
                    true,
                    CombatSkillUseBlockReason.V4ContextIncomplete,
                    v3AccessChecked,
                    false);
            }

            if (hasCompleteV4Context)
            {
                var v4Result = combatSkillAccessService.Evaluate(
                    skillId,
                    classProfile,
                    classRegistry,
                    masteryProfile,
                    skillRegistry);

                if (!v4Result.IsUsable)
                {
                    return Block(
                        skillId,
                        true,
                        CombatSkillUseBlockReason.V4SkillBlocked,
                        v3AccessChecked,
                        true,
                        string.Empty,
                        v4Result.BlockReason);
                }

                return Allow(skillId, true, v3AccessChecked, true);
            }

            return Allow(skillId, true, v3AccessChecked, false);
        }

        private static CombatSkillUseValidationResult Allow(
            string skillId,
            bool isPlayerAction,
            bool v3AccessChecked,
            bool v4AccessChecked)
        {
            return new CombatSkillUseValidationResult(
                true,
                skillId,
                isPlayerAction,
                v3AccessChecked,
                v4AccessChecked,
                CombatSkillUseBlockReason.None,
                string.Empty,
                MasteredSkillRetentionBlockReason.None);
        }

        private static CombatSkillUseValidationResult Block(
            string skillId,
            bool isPlayerAction,
            CombatSkillUseBlockReason blockReason,
            bool v3AccessChecked = false,
            bool v4AccessChecked = false,
            string v3BlockReason = "",
            MasteredSkillRetentionBlockReason v4BlockReason =
                MasteredSkillRetentionBlockReason.None)
        {
            return new CombatSkillUseValidationResult(
                false,
                skillId,
                isPlayerAction,
                v3AccessChecked,
                v4AccessChecked,
                blockReason,
                v3BlockReason,
                v4BlockReason);
        }
    }
}
