using System;
using System.Collections.Generic;
using Game.Domain.Classes;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.Passives;
using Game.Domain.Skills;

namespace Game.Application.Runtime
{
    public class PlayerSessionComposition
    {
        private readonly LoadedPlayerSession loadedSession;

        public ClassDefinitionRegistry ClassRegistry { get; private set; }
        public SkillDefinitionRegistry SkillRegistry { get; private set; }
        public MentorDefinitionRegistry MentorRegistry { get; private set; }
        public ClassSkillTreeBindingRegistry ClassSkillTreeBindingRegistry { get; private set; }

        public LoadedPlayerSession LoadedSession
        {
            get { return loadedSession; }
        }

        public PlayerProgressionRuntimeState RuntimeState
        {
            get { return loadedSession.RuntimeState; }
        }

        public PlayerSessionComposition(
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

        public PlayerSessionComposition(
            ClassDefinitionRegistry classRegistry,
            SkillDefinitionRegistry skillRegistry,
            MentorDefinitionRegistry mentorRegistry,
            ClassSkillTreeBindingRegistry classSkillTreeBindingRegistry)
        {
            ClassRegistry = classRegistry
                ?? throw new ArgumentNullException(nameof(classRegistry));
            SkillRegistry = skillRegistry
                ?? throw new ArgumentNullException(nameof(skillRegistry));
            MentorRegistry = mentorRegistry
                ?? throw new ArgumentNullException(nameof(mentorRegistry));
            ClassSkillTreeBindingRegistry = classSkillTreeBindingRegistry
                ?? throw new ArgumentNullException(
                    nameof(classSkillTreeBindingRegistry));

            loadedSession = new LoadedPlayerSession(
                ClassRegistry,
                SkillRegistry,
                MentorRegistry,
                ClassSkillTreeBindingRegistry);
        }

        public TurnBasedCombatEngine CreateCombatEngine(
            IRandomSource random,
            IReadOnlyList<ICombatPassiveEffect> passives)
        {
            return loadedSession.CreateCombatEngine(random, passives);
        }
    }
}
