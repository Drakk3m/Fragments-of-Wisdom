using System.Collections.Generic;

namespace Game.Domain.Combat.StatusEffects
{
    public class StatusEffectCollection
    {
        private readonly List<StatusEffectInstance> active = new List<StatusEffectInstance>();

        public IReadOnlyList<StatusEffectInstance> Active
        {
            get { return active; }
        }

        public bool Apply(StatusEffectInstance statusEffect)
        {
            if (statusEffect == null)
            {
                return false;
            }

            if (!statusEffect.AllowsStacking && HasActive(statusEffect.Type))
            {
                return false;
            }

            active.Add(statusEffect);
            return true;
        }

        public bool HasActive(StatusEffectType type)
        {
            foreach (var statusEffect in active)
            {
                if (statusEffect.Type == type && !statusEffect.IsExpired)
                {
                    return true;
                }
            }

            return false;
        }

        public StatusEffectInstance GetFirst(StatusEffectType type)
        {
            foreach (var statusEffect in active)
            {
                if (statusEffect.Type == type && !statusEffect.IsExpired)
                {
                    return statusEffect;
                }
            }

            return null;
        }

        public void TickDurations()
        {
            for (int index = active.Count - 1; index >= 0; index--)
            {
                active[index].TickDuration();

                if (active[index].IsExpired)
                {
                    active.RemoveAt(index);
                }
            }
        }
    }
}
