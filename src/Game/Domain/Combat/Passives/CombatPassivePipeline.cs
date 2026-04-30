using System.Collections.Generic;
using Game.Domain.Combat.Resolvers;

namespace Game.Domain.Combat.Passives
{
    public class CombatPassivePipeline
    {
        private readonly IReadOnlyList<ICombatPassiveEffect> passives;

        public CombatPassivePipeline(IReadOnlyList<ICombatPassiveEffect> passives)
        {
            this.passives = passives;
        }

        public void OnCombatStart(CombatContext context)
        {
            foreach (var passive in passives)
            {
                passive.OnCombatStart(context);
            }
        }

        public void OnBeforeSkillCostCalculated(CombatContext context)
        {
            foreach (var passive in passives)
            {
                passive.OnBeforeSkillCostCalculated(context);
            }
        }

        public void OnBeforeDebuffApplied(DebuffApplicationContext context)
        {
            foreach (var passive in passives)
            {
                passive.OnBeforeDebuffApplied(context);
            }
        }
    }
}
