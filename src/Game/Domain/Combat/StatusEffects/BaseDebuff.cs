namespace Game.Domain.Combat.StatusEffects
{
    public abstract class BaseDebuff : IDebuff
    {
        public abstract string Name { get; }
        public abstract DebuffType Type { get; }

        public int RemainingTurns { get; protected set; }

        public bool IsExpired
        {
            get { return RemainingTurns <= 0; }
        }

        protected BaseDebuff(int duration)
        {
            RemainingTurns = duration;
        }

        public virtual void OnTurnStart(
            CombatEntity target,
            CombatContext context)
        {
        }

        public virtual void OnBeforeAction(CombatContext context)
        {
        }

        public void Tick()
        {
            RemainingTurns--;
        }
    }
}
