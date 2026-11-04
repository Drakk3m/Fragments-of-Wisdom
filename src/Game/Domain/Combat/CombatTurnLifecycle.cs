using Game.Domain.Common;
using Game.Domain.Combat.StatusEffects;

namespace Game.Domain.Combat
{
    // Shared by attack/skill execution and refill; action bodies do not own the combat clock.
    internal sealed class CombatTurnLifecycle
    {
        private readonly StatusEffectResolver statusEffectResolver = new StatusEffectResolver();

        internal bool Begin(CombatRuntimeContext runtime, CombatContext context)
        {
            runtime.EventLog.Add(GameEventType.TurnStarted,
                "Turn " + runtime.TurnNumber + ": " + context.Attacker.Name + " acts.");
            context.Attacker.Flags.ResetTurnFlags();
            context.Attacker.Debuffs.OnTurnStart(context.Attacker, context);
            context.Attacker.Debuffs.OnBeforeAction(context);
            return !EndIfDefeated(runtime, context);
        }

        internal void Complete(CombatRuntimeContext runtime, CombatContext context)
        {
            if (!context.Attacker.Flags.ResourceRegenBlocked)
            {
                context.Attacker.Resource.Regenerate(5);
                runtime.EventLog.Add(GameEventType.ResourceRegenerated, context.Attacker.Name + " regenerates 5 Essence.");
            }

            context.Attacker.Debuffs.OnAfterAction(context.Attacker, context);
            statusEffectResolver.TickTurnBoundary(context.Attacker);
            if (EndIfDefeated(runtime, context))
            {
                return;
            }

            runtime.EventLog.Add(GameEventType.TurnEnded, "Turn " + runtime.TurnNumber + " ends.");
            runtime.AdvanceTurn();
        }

        private static bool EndIfDefeated(CombatRuntimeContext runtime, CombatContext context)
        {
            if (context.Attacker.IsAlive)
            {
                return false;
            }

            runtime.EventLog.Add(GameEventType.CombatEnded, context.Attacker.Name + " is defeated by status effects.");
            runtime.EndCombat();
            return true;
        }
    }
}
