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

            ApplyTurnBoundaryEffects(target);
            target.StatusEffects.TickDurations();
        }

        private static void ApplyTurnBoundaryEffects(CombatEntity target)
        {
            foreach (var statusEffect in target.StatusEffects.Active)
            {
                if (statusEffect.Type == StatusEffectType.Regen
                    && !target.StatusEffects.HasActive(StatusEffectType.RegenBlock))
                {
                    target.Heal(statusEffect.Potency);
                }
            }
        }
    }
}
