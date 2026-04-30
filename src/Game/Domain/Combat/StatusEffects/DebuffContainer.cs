using System.Collections.Generic;
using System.Linq;
using Game.Domain.Common;

namespace Game.Domain.Combat.StatusEffects
{
    public class DebuffContainer
    {
        private readonly List<IDebuff> active = new List<IDebuff>();

        public IReadOnlyList<IDebuff> Active
        {
            get { return active.AsReadOnly(); }
        }

        public void Add(
            IDebuff debuff,
            CombatEntity target,
            CombatContext context)
        {
            active.Add(debuff);
            context.EventLog.Add(GameEventType.DebuffApplied, target.Name + " receives " + debuff.Name + ".");
        }

        public void OnTurnStart(
            CombatEntity target,
            CombatContext context)
        {
            foreach (var debuff in active.ToList())
            {
                debuff.OnTurnStart(target, context);
            }
        }

        public void OnBeforeAction(CombatContext context)
        {
            foreach (var debuff in active.ToList())
            {
                debuff.OnBeforeAction(context);
            }
        }

        public void OnAfterAction(
            CombatEntity target,
            CombatContext context)
        {
            foreach (var debuff in active.ToList())
            {
                debuff.Tick();

                if (debuff.IsExpired)
                {
                    context.EventLog.Add(GameEventType.DebuffExpired, debuff.Name + " expires on " + target.Name + ".");
                    active.Remove(debuff);
                }
            }
        }

        public int RemoveOne(Game.Domain.Common.IRandomSource random)
        {
            if (active.Count == 0)
            {
                return 0;
            }

            active.RemoveAt(random.NextInt(0, active.Count));
            return 1;
        }
    }
}
