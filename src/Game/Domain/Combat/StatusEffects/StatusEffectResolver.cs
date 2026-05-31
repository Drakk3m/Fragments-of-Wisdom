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
            ApplyDamageOverTime(target);
            ApplyRegen(target);
        }

        private static void ApplyDamageOverTime(CombatEntity target)
        {
            foreach (var statusEffect in target.StatusEffects.Active)
            {
                if (statusEffect.Type == StatusEffectType.Poison
                    || statusEffect.Type == StatusEffectType.Burn)
                {
                    target.ReceiveDamage(statusEffect.Potency);
                }
            }
        }

        private static void ApplyRegen(CombatEntity target)
        {
            if (target.StatusEffects.HasActive(StatusEffectType.RegenBlock))
            {
                return;
            }

            foreach (var statusEffect in target.StatusEffects.Active)
            {
                // Explicit boundary order: Poison/Burn DOT is applied before Regen healing.
                if (statusEffect.Type == StatusEffectType.Regen)
                {
                    target.Heal(statusEffect.Potency);
                }
            }
        }
    }
}
