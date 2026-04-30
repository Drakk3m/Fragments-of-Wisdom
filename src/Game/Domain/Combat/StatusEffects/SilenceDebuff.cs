using Game.Domain.Common;

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
            context.EventLog.Add(GameEventType.ActionBlocked, context.Attacker.Name + " is silenced. Skill blocking hook is active.");
        }
    }
}
