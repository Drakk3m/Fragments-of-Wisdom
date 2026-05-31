using System;

namespace Game.Domain.Combat.StatusEffects
{
    public class StatusEffectResolver
    {
        public bool Apply(
            CombatEntity target,
            StatusEffectInstance statusEffect)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            return target.StatusEffects.Apply(statusEffect);
        }

        public void TickTurnBoundary(CombatEntity target)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.StatusEffects.TickDurations();
        }
    }
}
