using System;
using Game.Domain.Common;

namespace Game.Domain.Combat.Resources
{
    public class ResonanceRefillAction
    {
        public const string EmptyReserveBlockReason = "resonance.block.empty_reserve";

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

            if (!resonanceProfile.TryRefillFromReserve(amount))
            {
                runtime.EventLog.Add(GameEventType.ActionBlocked, EmptyReserveBlockReason);
                return false;
            }

            runtime.EventLog.Add(GameEventType.ResourceSpent, "Reserve resonance refills active resonance.");
            runtime.EventLog.Add(GameEventType.TurnEnded, "Resonance refill consumes the turn.");
            runtime.AdvanceTurn();
            return true;
        }
    }
}
