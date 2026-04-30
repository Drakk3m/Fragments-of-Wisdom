using Game.Domain.Player;

namespace Game.Domain.Missions
{
    public class FirstRelicSideQuestEvaluator : IMissionEvaluator
    {
        public const string Id = "sidequest_first_relic";

        public string MissionId
        {
            get { return Id; }
        }

        public MissionOutcome Evaluate(
            PlayerProfile profile,
            MissionAttempt attempt)
        {
            return attempt.Contains(MissionActionType.CollectItem)
                ? MissionOutcome.Success
                : MissionOutcome.FailedButRetryable;
        }
    }
}
