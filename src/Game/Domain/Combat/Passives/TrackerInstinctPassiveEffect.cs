using Game.Domain.Common;
using Game.Domain.Combat.Resolvers;

namespace Game.Domain.Combat.Passives
{
    public class TrackerInstinctPassiveEffect : ICombatPassiveEffect
    {
        public void OnCombatStart(CombatContext context)
        {
            context.RevealOneEnemyTrait = true;
            context.EventLog.Add(GameEventType.PassiveTriggered, "Tracker Instinct reveals one enemy trait.");
        }

        public void OnBeforeSkillCostCalculated(CombatContext context)
        {
        }

        public void OnBeforeDebuffApplied(DebuffApplicationContext context)
        {
        }
    }
}
