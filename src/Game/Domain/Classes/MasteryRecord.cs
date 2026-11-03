namespace Game.Domain.Classes
{
    public class MasteryRecord
    {
        public const int MasteredProgress = 100;

        public string SkillId { get; private set; }
        public int Progress { get; private set; }

        public MasteryRank Rank
        {
            get
            {
                if (IsMastered)
                {
                    return MasteryRank.Mastered;
                }

                return Progress > 0 ? MasteryRank.Practiced : MasteryRank.Unranked;
            }
        }

        public bool IsMastered
        {
            get { return Progress >= MasteredProgress; }
        }

        public MasteryRecord(string skillId)
        {
            SkillId = skillId;
        }

        public MasteryRecord(
            string skillId,
            int progress)
        {
            SkillId = skillId;
            SetProgress(progress);
        }

        public bool AddProgress(int amount)
        {
            return AddProgressWithResult(amount).Applied;
        }

        public MasteryProgressResult AddProgressWithResult(int amount)
        {
            if (amount <= 0 || IsMastered)
            {
                return MasteryProgressResult.Rejected(amount);
            }

            int previousProgress = Progress;
            // Widen before adding so even int.MaxValue cannot wrap before capping.
            Progress = (int)System.Math.Min((long)Progress + amount, MasteredProgress);

            int appliedProgress = Progress - previousProgress;
            return new MasteryProgressResult(amount, appliedProgress,
                amount - appliedProgress, IsMastered);
        }

        public void SetProgress(int progress)
        {
            Progress = progress;

            if (Progress < 0)
            {
                Progress = 0;
            }

            if (Progress > MasteredProgress)
            {
                Progress = MasteredProgress;
            }
        }
    }
}
