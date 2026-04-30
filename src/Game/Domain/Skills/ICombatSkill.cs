using Game.Domain.Combat;

namespace Game.Domain.Skills
{
    public interface ICombatSkill
    {
        string Id { get; }
        string Name { get; }

        void Apply(
            CombatContext context,
            SkillProgress progress);
    }
}
