using System;
using Game.Domain.Common;

namespace Game.Domain.Combat.Resources
{
    public class ResonanceRefillAction
    {
        public const string EmptyReserveBlockReason = "resonance.block.empty_reserve";
        private readonly CombatTurnLifecycle turnLifecycle = new CombatTurnLifecycle();

        public bool Execute(
            ResonanceProfile resonanceProfile,
            CombatRuntimeContext runtime,
            int amount)
        {
            if (resonanceProfile == null)
            {
                throw new ArgumentNullException(nameof(resonanceProfile));
            }

            if (runtime == null)
            {
                throw new ArgumentNullException(nameof(runtime));
            }

            if (runtime.CombatEnded || !runtime.Player.IsAlive || !runtime.Enemy.IsAlive)
            {
                return false;
            }

            if (!resonanceProfile.CanRefillFromReserve(amount))
            {
                runtime.EventLog.Add(GameEventType.ActionBlocked, EmptyReserveBlockReason);
                return false;
            }

            var context = new CombatContext(runtime.Player, runtime.Enemy, runtime.EventLog);
            if (!turnLifecycle.Begin(runtime, context))
            {
                return false;
            }

            // Hooks run before the action, so honor the real transfer result as well as admission.
            if (!resonanceProfile.TryRefillFromReserve(amount))
            {
                runtime.EventLog.Add(GameEventType.ActionBlocked, EmptyReserveBlockReason);
                turnLifecycle.Complete(runtime, context);
                return false;
            }

            runtime.EventLog.Add(GameEventType.ResourceSpent, "Reserve resonance refills active resonance.");
            turnLifecycle.Complete(runtime, context);
            return true;
        }
    }
}
