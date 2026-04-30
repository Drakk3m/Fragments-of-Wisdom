using Game.Domain.Player;

namespace Game.Domain.Missions
{
    public class LyraJudgementCrackEvaluator : IMissionEvaluator
    {
        public const string Id = "mission_lyra_judgement_crack";

        public string MissionId
        {
            get { return Id; }
        }

        public MissionOutcome Evaluate(
            PlayerProfile profile,
            MissionAttempt attempt)
        {
            int analyze = attempt.IndexOf(MissionActionType.Analyze);
            int debuff = attempt.IndexOf(MissionActionType.ApplyDebuff);

            if (analyze < 0)
            {
                return MissionOutcome.FailedButRetryable;
            }

            if (debuff >= 0 && debuff < analyze)
            {
                return MissionOutcome.FailedButRetryable;
            }

            return MissionOutcome.Success;
        }
    }
}
