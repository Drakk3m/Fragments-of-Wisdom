using System;
using System.Collections.Generic;
using Game.Application.Runtime;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.Passives;

namespace Game.Application.Integration
{
    public class ProgressionAwareCombatEngineFactory
    {
        public TurnBasedCombatEngine CreateEngine(
            PlayerProgressionRuntimeState runtimeState,
            IRandomSource random,
            IReadOnlyList<ICombatPassiveEffect> passives)
        {
            if (runtimeState == null)
            {
                throw new ArgumentNullException(nameof(runtimeState));
            }

            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (passives == null)
            {
                throw new ArgumentNullException(nameof(passives));
            }

            var pipeline = new CombatPassivePipeline(passives);

            return CombatSkillTurnValidationComposition.CreateProgressionAwareEngine(
                random,
                pipeline,
                runtimeState.UnlockProfile,
                runtimeState.ClassProfile,
                runtimeState.MasteryProfile,
                runtimeState.ClassRegistry,
                runtimeState.SkillRegistry);
        }
    }
}
