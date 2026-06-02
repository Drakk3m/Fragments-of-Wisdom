using Game.Domain.Classes;

namespace Game.Domain.Progress
{
    public class V2MasterySnapshot
    {
        public string SkillId { get; private set; }
        public int Progress { get; private set; }
        public MasteryRank Rank { get; private set; }

        public V2MasterySnapshot(
            string skillId,
            int progress,
            MasteryRank rank)
        {
            SkillId = skillId;
            Progress = progress;
            Rank = rank;
        }
    }
}
