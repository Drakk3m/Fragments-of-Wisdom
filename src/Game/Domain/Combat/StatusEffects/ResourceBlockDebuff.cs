namespace Game.Domain.Combat.StatusEffects
{
    public class ResourceBlockDebuff : BaseDebuff
    {
        public override string Name
        {
            get { return "Resource Block"; }
        }

        public override DebuffType Type
        {
            get { return DebuffType.ResourceControl; }
        }

        public ResourceBlockDebuff(int duration)
            : base(duration)
        {
        }

        public override void OnBeforeAction(CombatContext context)
        {
            context.Attacker.Flags.ResourceRegenBlocked = true;
        }
    }
}
