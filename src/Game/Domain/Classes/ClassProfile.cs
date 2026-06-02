namespace Game.Domain.Classes
{
    public class ClassProfile
    {
        private string currentClassId = string.Empty;

        public string CurrentClassId
        {
            get { return currentClassId; }
        }

        public bool AssignClass(ClassDefinition classDefinition)
        {
            if (!IsValidClass(classDefinition))
            {
                return false;
            }

            currentClassId = classDefinition.Id;
            return true;
        }

        public bool ChangeClass(ClassDefinition classDefinition)
        {
            return AssignClass(classDefinition);
        }

        public bool AssignClassId(
            string classId,
            ClassDefinitionRegistry registry)
        {
            if (registry == null || !registry.Contains(classId))
            {
                return false;
            }

            currentClassId = classId;
            return true;
        }

        public bool ChangeClassId(
            string classId,
            ClassDefinitionRegistry registry)
        {
            return AssignClassId(classId, registry);
        }

        public ClassDefinition ResolveCurrentClass(ClassDefinitionRegistry registry)
        {
            return registry == null ? null : registry.ResolveCurrentClass(this);
        }

        public bool CurrentClassAllowsSkill(
            string skillId,
            ClassDefinitionRegistry registry)
        {
            var classDefinition = ResolveCurrentClass(registry);
            return classDefinition != null && classDefinition.AllowsSkill(skillId);
        }

        private static bool IsValidClass(ClassDefinition classDefinition)
        {
            return classDefinition != null
                && !string.IsNullOrWhiteSpace(classDefinition.Id)
                && !string.IsNullOrWhiteSpace(classDefinition.Name);
        }
    }
}
