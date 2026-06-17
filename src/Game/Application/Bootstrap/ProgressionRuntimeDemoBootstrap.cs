using System.Collections.Generic;
using Game.Application.Runtime;
using Game.Domain.Classes;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.Passives;
using Game.Domain.Skills;

namespace Game.Application.Bootstrap
{
    public class ProgressionRuntimeDemoBootstrap
    {
        private readonly PlayerSessionBootstrap sessionBootstrap;

        public PlayerSessionBootstrap SessionBootstrap
        {
            get { return sessionBootstrap; }
        }

        public PlayerSessionComposition Composition
        {
            get { return sessionBootstrap.Composition; }
        }

        public LoadedPlayerSession LoadedSession
        {
            get { return sessionBootstrap.LoadedSession; }
        }

        public PlayerProgressionRuntimeState RuntimeState
        {
            get { return sessionBootstrap.RuntimeState; }
        }

        public ProgressionRuntimeDemoBootstrap(
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

        public ProgressionRuntimeDemoBootstrap(
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry,
            MentorDefinitionRegistry mentorRegistry,
            ClassSkillTreeBindingRegistry classSkillTreeBindingRegistry)
        {
            sessionBootstrap = new PlayerSessionBootstrap(
                classRegistry,
                skillRegistry,
                mentorRegistry,
                classSkillTreeBindingRegistry);
        }

        public TurnBasedCombatEngine CreateCombatEngine(
            IRandomSource random,
            IReadOnlyList<ICombatPassiveEffect> passives)
        {
            return sessionBootstrap.CreateCombatEngine(random, passives);
        }
    }
}
