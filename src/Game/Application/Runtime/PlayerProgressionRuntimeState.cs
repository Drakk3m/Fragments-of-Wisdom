using System;
using Game.Domain.Classes;
using Game.Domain.Codex;
using Game.Domain.Skills;

namespace Game.Application.Runtime
{
    public class PlayerProgressionRuntimeState
    {
        public UnlockProfile UnlockProfile { get; private set; }
        public ClassProfile ClassProfile { get; private set; }
        public MasteryProfile MasteryProfile { get; private set; }
        public MentorRelationshipProfile MentorRelationshipProfile { get; private set; }
        public SkillTreeProgressProfile SkillTreeProgressProfile { get; private set; }

        public ClassDefinitionRegistry ClassRegistry { get; private set; }
        public SkillDefinitionRegistry SkillRegistry { get; private set; }
        public MentorDefinitionRegistry MentorRegistry { get; private set; }
        public ClassSkillTreeBindingRegistry ClassSkillTreeBindingRegistry { get; private set; }

        public PlayerProgressionRuntimeState(
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

            UnlockProfile = new UnlockProfile();
            ClassProfile = new ClassProfile();
            MasteryProfile = new MasteryProfile();
            MentorRelationshipProfile = new MentorRelationshipProfile();
            SkillTreeProgressProfile = new SkillTreeProgressProfile();
        }
    }
}
