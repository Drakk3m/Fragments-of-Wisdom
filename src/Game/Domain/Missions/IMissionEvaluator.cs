using Game.Domain.Player;

namespace Game.Domain.Missions
{
    public interface IMissionEvaluator
    {
        string MissionId { get; }

        MissionOutcome Evaluate(
            PlayerProfile profile,
            MissionAttempt attempt);
    }
}
