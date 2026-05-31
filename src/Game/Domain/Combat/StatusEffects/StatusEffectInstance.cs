namespace Game.Domain.Combat.StatusEffects
{
    public class StatusEffectInstance
    {
        public StatusEffectType Type { get; private set; }
        public int RemainingDuration { get; private set; }
        public bool AllowsStacking { get; private set; }

        public bool IsExpired
        {
            get { return RemainingDuration <= 0; }
        }

        public StatusEffectInstance(
            StatusEffectType type,
            int remainingDuration,
            bool allowsStacking)
        {
            Type = type;
            RemainingDuration = remainingDuration;
            AllowsStacking = allowsStacking;
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
