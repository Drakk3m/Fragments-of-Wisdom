using System.Collections.Generic;

namespace Game.Domain.Classes
{
    public class ClassSkillTreeBindingRegistry
    {
        private readonly Dictionary<string, ClassSkillTreeBinding> bindingsByClassId =
            new Dictionary<string, ClassSkillTreeBinding>();

        private readonly Dictionary<string, SkillTreeDefinition> skillTreesById =
            new Dictionary<string, SkillTreeDefinition>();

        public IReadOnlyDictionary<string, ClassSkillTreeBinding> Bindings
        {
            get { return bindingsByClassId; }
        }

        public IReadOnlyDictionary<string, SkillTreeDefinition> SkillTrees
        {
            get { return skillTreesById; }
        }

        public bool RegisterSkillTree(SkillTreeDefinition skillTree)
        {
            if (!IsValidSkillTree(skillTree) || skillTreesById.ContainsKey(skillTree.Id))
            {
                return false;
            }

            skillTreesById.Add(skillTree.Id, skillTree);
            return true;
        }

        public bool RegisterBinding(
            ClassSkillTreeBinding binding,
            ClassDefinitionRegistry classRegistry)
        {
            if (!IsValidBinding(binding)
                || classRegistry == null
                || !classRegistry.Contains(binding.ClassId)
                || !skillTreesById.ContainsKey(binding.SkillTreeId)
                || bindingsByClassId.ContainsKey(binding.ClassId))
            {
                return false;
            }

            bindingsByClassId.Add(binding.ClassId, binding);
            return true;
        }

        public bool ContainsSkillTree(string skillTreeId)
        {
            return !string.IsNullOrWhiteSpace(skillTreeId)
                && skillTreesById.ContainsKey(skillTreeId);
        }

        public SkillTreeDefinition GetSkillTree(string skillTreeId)
        {
            SkillTreeDefinition skillTree;
            return !string.IsNullOrWhiteSpace(skillTreeId)
                && skillTreesById.TryGetValue(skillTreeId, out skillTree)
                    ? skillTree
                    : null;
        }

        public ClassSkillTreeBinding GetBindingForClass(string classId)
        {
            ClassSkillTreeBinding binding;
            return !string.IsNullOrWhiteSpace(classId)
                && bindingsByClassId.TryGetValue(classId, out binding)
                    ? binding
                    : null;
        }

        public SkillTreeDefinition ResolveSkillTreeForClass(string classId)
        {
            var binding = GetBindingForClass(classId);
            return binding == null ? null : GetSkillTree(binding.SkillTreeId);
        }

        public SkillTreeDefinition ResolveCurrentClassSkillTree(ClassProfile profile)
        {
            if (profile == null)
            {
                return null;
            }

            return ResolveSkillTreeForClass(profile.CurrentClassId);
        }

        private static bool IsValidSkillTree(SkillTreeDefinition skillTree)
        {
            return skillTree != null
                && !string.IsNullOrWhiteSpace(skillTree.Id);
        }

        private static bool IsValidBinding(ClassSkillTreeBinding binding)
        {
            return binding != null
                && !string.IsNullOrWhiteSpace(binding.ClassId)
                && !string.IsNullOrWhiteSpace(binding.SkillTreeId);
        }
    }
}
