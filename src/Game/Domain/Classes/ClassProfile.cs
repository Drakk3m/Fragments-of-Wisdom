using System;

namespace Game.Domain.Classes
{
    public class ClassProfile
    {
        public ClassDefinition CurrentClass { get; private set; }

        public string CurrentClassId
        {
            get { return CurrentClass == null ? string.Empty : CurrentClass.Id; }
        }

        public bool AssignClass(ClassDefinition classDefinition)
        {
            if (!IsValidClass(classDefinition))
            {
                return false;
            }

            CurrentClass = classDefinition;
            return true;
        }

        public bool ChangeClass(ClassDefinition classDefinition)
        {
            return AssignClass(classDefinition);
        }

        public bool CurrentClassAllowsSkill(string skillId)
        {
            return CurrentClass != null && CurrentClass.AllowsSkill(skillId);
        }

        private static bool IsValidClass(ClassDefinition classDefinition)
        {
            return classDefinition != null
                && !string.IsNullOrWhiteSpace(classDefinition.Id)
                && !string.IsNullOrWhiteSpace(classDefinition.Name);
        }
    }
}
