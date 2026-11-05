using System;
using Game.Domain.Combat;
using Game.Domain.Skills;

namespace Game.Application.Runtime
{
    /// <summary>Trusted encounter content, not a skill grant or a second definition registry.</summary>
    public sealed class PreparedCombatSkillBinding
    {
        public string SkillId { get; }
        internal ICombatSkill Skill { get; }
        internal SkillProgress Progress { get; }

        public PreparedCombatSkillBinding(ICombatSkill skill, SkillProgress progress)
        {
            Skill = skill ?? throw new ArgumentNullException(nameof(skill));
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            SkillId = skill.Id;

            if (string.IsNullOrWhiteSpace(SkillId)
                || progress.Definition == null
                || SkillId != progress.Definition.Id)
            {
                throw new ArgumentException("Executable skill and progress must have the same nonempty ID.");
            }
        }

        internal void Validate(SkillDefinitionRegistry registry)
        {
            var canonical = registry.Get(SkillId);
            if (Skill.Id != SkillId
                || canonical == null
                || canonical.Cost == null
                || Progress.Definition.Cost == null
                || !CombatSkillUseValidator.HasCanonicalMetadata(Progress.Definition, canonical))
            {
                throw new ArgumentException("Prepared skill must match the session's canonical metadata: " + SkillId);
            }
        }
    }
}
