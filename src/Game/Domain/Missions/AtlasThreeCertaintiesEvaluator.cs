using Game.Domain.Player;

namespace Game.Domain.Missions
{
    public class AtlasThreeCertaintiesEvaluator : IMissionEvaluator
    {
        public const string Id = "mission_atlas_three_certainties";

        public string MissionId
        {
            get { return Id; }
        }

        public MissionOutcome Evaluate(
            PlayerProfile profile,
            MissionAttempt attempt)
        {
            return attempt.Contains(MissionActionType.ChooseStableAction)
                ? MissionOutcome.Success
                : MissionOutcome.FailedButRetryable;
        }
    }
}
