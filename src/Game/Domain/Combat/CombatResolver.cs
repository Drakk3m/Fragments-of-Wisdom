using System;
using Game.Domain.Common;
using Game.Domain.Combat.Resolvers;

namespace Game.Domain.Combat
{
    public class CombatResolver
    {
        public const string NotEnoughResourceBlockReason = "combat.block.not_enough_resource";

        private readonly DamageResolver damageResolver;

        public CombatResolver(IRandomSource random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            damageResolver = new DamageResolver(random);
        }

        public CombatTurnResult Resolve(
            CombatantProfile attacker,
            CombatantProfile defender,
            CombatAction action)
        {
            if (attacker == null)
            {
                throw new ArgumentNullException(nameof(attacker));
            }

            if (defender == null)
            {
                throw new ArgumentNullException(nameof(defender));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (action.ActionType == TurnActionType.Skill
                && !attacker.TrySpendResource(action.ResourceCost))
            {
                return new CombatTurnResult(
                    false,
                    NotEnoughResourceBlockReason,
                    false,
                    false,
                    0,
                    defender.IsDefeated);
            }

            var attackerEntity = new CombatEntity(attacker.Id, attacker.Name, attacker.Stats, attacker.MaxResource);
            var defenderEntity = new CombatEntity(defender.Id, defender.Name, defender.Stats, defender.MaxResource);
            var context = new CombatContext(attackerEntity, defenderEntity, new GameEventLog());
            context.DamageMultiplier = action.DamageMultiplier;

            DamageResult damageResult = damageResolver.Resolve(context);

            if (damageResult.Hit)
            {
                defender.ReceiveDamage(damageResult.Damage);
            }

            return new CombatTurnResult(
                true,
                "",
                damageResult.Hit,
                damageResult.Critical,
                damageResult.Damage,
                defender.IsDefeated);
        }
    }
}
