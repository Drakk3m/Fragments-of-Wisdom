using System;
using Game.Domain.Codex;

namespace Game.Domain.Skills
{
    public class SkillAccessPolicy
    {
        public const string MissingUnlockBlockReason = "skill.block.missing_unlock";

        public SkillAccessResult Evaluate(
            SkillDefinition skill,
            UnlockProfile unlockProfile)
        {
            if (skill == null)
            {
                throw new ArgumentNullException(nameof(skill));
            }

            if (unlockProfile == null)
            {
                throw new ArgumentNullException(nameof(unlockProfile));
            }

            if (string.IsNullOrEmpty(skill.RequiredUnlockId)
                || unlockProfile.HasUnlock(skill.RequiredUnlockId))
            {
                return new SkillAccessResult(true, "", skill.RequiredUnlockId);
            }

            return new SkillAccessResult(
                false,
                MissingUnlockBlockReason,
                skill.RequiredUnlockId);
        }

        public SkillAccessResult Evaluate(
            SkillDefinition skill,
            UnlockRecord unlock)
        {
            if (skill == null)
            {
                throw new ArgumentNullException(nameof(skill));
            }

            if (unlock == null)
            {
                return new SkillAccessResult(
                    string.IsNullOrEmpty(skill.RequiredUnlockId),
                    string.IsNullOrEmpty(skill.RequiredUnlockId) ? "" : MissingUnlockBlockReason,
                    skill.RequiredUnlockId);
            }

            return new SkillAccessResult(
                string.IsNullOrEmpty(skill.RequiredUnlockId) || unlock.UnlockId == skill.RequiredUnlockId,
                string.IsNullOrEmpty(skill.RequiredUnlockId) || unlock.UnlockId == skill.RequiredUnlockId
                    ? ""
                    : MissingUnlockBlockReason,
                skill.RequiredUnlockId);
        }
    }
}
