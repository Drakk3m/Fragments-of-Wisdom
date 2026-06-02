using System.Collections.Generic;

namespace Game.Domain.Classes
{
    public class ClassDefinitionRegistry
    {
        private readonly Dictionary<string, ClassDefinition> definitions =
            new Dictionary<string, ClassDefinition>();

        public IReadOnlyDictionary<string, ClassDefinition> Definitions
        {
            get { return definitions; }
        }

        public bool Register(ClassDefinition classDefinition)
        {
            if (!IsValidClass(classDefinition))
            {
                return false;
            }

            definitions[classDefinition.Id] = classDefinition;
            return true;
        }

        public bool Contains(string classId)
        {
            return !string.IsNullOrWhiteSpace(classId)
                && definitions.ContainsKey(classId);
        }

        public ClassDefinition Get(string classId)
        {
            ClassDefinition classDefinition;
            return !string.IsNullOrWhiteSpace(classId)
                && definitions.TryGetValue(classId, out classDefinition)
                    ? classDefinition
                    : null;
        }

        public ClassDefinition ResolveCurrentClass(ClassProfile profile)
        {
            if (profile == null)
            {
                return null;
            }

            return Get(profile.CurrentClassId);
        }

        private static bool IsValidClass(ClassDefinition classDefinition)
        {
            return classDefinition != null
                && !string.IsNullOrWhiteSpace(classDefinition.Id)
                && !string.IsNullOrWhiteSpace(classDefinition.Name);
        }
    }
}
