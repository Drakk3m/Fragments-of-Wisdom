namespace Game.Domain.Combat.StatusEffects
{
    public class StatusEffectInstance
    {
        public StatusEffectType Type { get; private set; }
        public int RemainingDuration { get; private set; }
        public bool AllowsStacking { get; private set; }
        public int Potency { get; private set; }

        public bool IsExpired
        {
            get { return RemainingDuration <= 0; }
        }

        public StatusEffectInstance(
            StatusEffectType type,
            int remainingDuration,
            bool allowsStacking)
            : this(type, remainingDuration, allowsStacking, 0)
        {
        }

        public StatusEffectInstance(
            StatusEffectType type,
            int remainingDuration,
            bool allowsStacking,
            int potency)
        {
            Type = type;
            RemainingDuration = remainingDuration;
            AllowsStacking = allowsStacking;
            Potency = potency;
        }

        public void TickDuration()
        {
            if (RemainingDuration > 0)
            {
                RemainingDuration--;
            }
        }
    }
}
