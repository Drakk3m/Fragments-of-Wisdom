namespace Game.Domain.Missions
{
    public enum MissionOutcome
    {
        Incomplete,
        Success,
        FailedButRetryable,
        FailedPermanent
    }
}
