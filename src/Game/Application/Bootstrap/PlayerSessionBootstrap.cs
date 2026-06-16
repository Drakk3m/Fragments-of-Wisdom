using System;
using System.Collections.Generic;
using Game.Application.Runtime;
using Game.Domain.Classes;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.Passives;
using Game.Domain.Skills;

namespace Game.Application.Bootstrap
{
    public class PlayerSessionBootstrap
    {
        private readonly PlayerSessionComposition composition;

        public PlayerSessionComposition Composition
        {
            get { return composition; }
        }

        public LoadedPlayerSession LoadedSession
        {
            get { return composition.LoadedSession; }
        }

        public PlayerProgressionRuntimeState RuntimeState
        {
            get { return composition.RuntimeState; }
        }

        public PlayerSessionBootstrap(
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry,
            MentorDefinitionRegistry mentorRegistry)
            : this(
                classRegistry,
                skillRegistry,
                mentorRegistry,
                new ClassSkillTreeBindingRegistry())
        {
        }

        public PlayerSessionBootstrap(
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry,
            MentorDefinitionRegistry mentorRegistry,
            ClassSkillTreeBindingRegistry classSkillTreeBindingRegistry)
        {
            composition = new PlayerSessionComposition(
                classRegistry,
                skillRegistry,
                mentorRegistry,
                classSkillTreeBindingRegistry);
        }

        public TurnBasedCombatEngine CreateCombatEngine(
            IRandomSource random,
            IReadOnlyList<ICombatPassiveEffect> passives)
        {
            return composition.CreateCombatEngine(random, passives);
        }
    }
}
