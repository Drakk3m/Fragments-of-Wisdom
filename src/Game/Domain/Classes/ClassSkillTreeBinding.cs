namespace Game.Domain.Classes
{
    public class ClassSkillTreeBinding
    {
        public string ClassId { get; private set; }
        public string SkillTreeId { get; private set; }

        public ClassSkillTreeBinding(
            string classId,
            string skillTreeId)
        {
            ClassId = classId;
            SkillTreeId = skillTreeId;
        }
    }
}
