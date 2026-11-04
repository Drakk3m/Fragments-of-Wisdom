namespace Game.Domain.Combat.StatusEffects
{
    public class SilenceDebuff : BaseDebuff
    {
        public override string Name
        {
            get { return "Silence"; }
        }

        public override DebuffType Type
        {
            get { return DebuffType.ActionBlock; }
        }

        public SilenceDebuff(int duration)
            : base(duration)
        {
        }

        public override void OnBeforeAction(CombatContext context)
        {
            if (!IsExpired)
            {
                context.SkillsBlockedBySilence = true;
            }
        }
    }
}
