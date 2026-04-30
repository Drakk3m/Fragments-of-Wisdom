using Game.Domain.Common;
using Game.Domain.Combat.Resolvers;

namespace Game.Domain.Combat.Passives
{
    public class TechnicalResonancePassiveEffect : ICombatPassiveEffect
    {
        public void OnCombatStart(CombatContext context)
        {
        }

        public void OnBeforeSkillCostCalculated(CombatContext context)
        {
            if (context.IsFirstSkillOfCombat)
            {
                context.ResourceCostMultiplier *= 0.9f;
                context.EventLog.Add(GameEventType.PassiveTriggered, "Technical Resonance reduces first skill cost.");
            }
        }

        public void OnBeforeDebuffApplied(DebuffApplicationContext context)
        {
        }
    }
}
