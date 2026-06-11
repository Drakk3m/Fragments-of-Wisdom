namespace Game.Domain.Progress
{
    public class V2ClassSkillTreeBindingSnapshot
    {
        public string ClassId { get; private set; }
        public string SkillTreeId { get; private set; }

        public V2ClassSkillTreeBindingSnapshot(
            string classId,
            string skillTreeId)
        {
            ClassId = classId;
            SkillTreeId = skillTreeId;
        }
    }
}
