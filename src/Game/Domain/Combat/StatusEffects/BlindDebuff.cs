namespace Game.Domain.Combat.StatusEffects
{
    public class BlindDebuff : BaseDebuff
    {
        private readonly float hitPenalty;

        public override string Name
        {
            get { return "Blind"; }
        }

        public override DebuffType Type
        {
            get { return DebuffType.Accuracy; }
        }

        public BlindDebuff(
            int duration,
            float hitPenalty)
            : base(duration)
        {
            this.hitPenalty = hitPenalty;
        }

        public override void OnBeforeAction(CombatContext context)
        {
            context.HitChanceModifier -= hitPenalty;
        }
    }
}
