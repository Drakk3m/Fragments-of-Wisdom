using System;
using System.Collections.Generic;
using Game.Application.Integration;
using Game.Domain.Classes;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.Passives;
using Game.Domain.Skills;

namespace Game.Application.Runtime
{
    public class LoadedPlayerSession
    {
        private readonly ProgressionAwareCombatEngineFactory combatEngineFactory;

        public PlayerProgressionRuntimeState RuntimeState { get; private set; }

        public LoadedPlayerSession(
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry,
            MentorDefinitionRegistry mentorRegistry,
            ClassSkillTreeBindingRegistry classSkillTreeBindingRegistry)
            : this(
                classRegistry,
                skillRegistry,
                mentorRegistry,
                classSkillTreeBindingRegistry,
                new ProgressionAwareCombatEngineFactory())
        {
        }

        public LoadedPlayerSession(
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry,
            MentorDefinitionRegistry mentorRegistry,
            ClassSkillTreeBindingRegistry classSkillTreeBindingRegistry,
            ProgressionAwareCombatEngineFactory combatEngineFactory)
        {
            this.combatEngineFactory = combatEngineFactory
                ?? throw new ArgumentNullException(nameof(combatEngineFactory));

            RuntimeState = new PlayerProgressionRuntimeState(
                classRegistry,
                skillRegistry,
                mentorRegistry,
                classSkillTreeBindingRegistry);
        }

        public TurnBasedCombatEngine CreateCombatEngine(
            IRandomSource random,
            IReadOnlyList<ICombatPassiveEffect> passives)
        {
            return combatEngineFactory.CreateEngine(
                RuntimeState,
                random,
                passives);
        }
    }
}
