using Game.Domain.Combat.Resolvers;

namespace Game.Domain.Combat.Passives
{
    public interface ICombatPassiveEffect
    {
        void OnCombatStart(CombatContext context);
        void OnBeforeSkillCostCalculated(CombatContext context);
        void OnBeforeDebuffApplied(DebuffApplicationContext context);
    }
}
