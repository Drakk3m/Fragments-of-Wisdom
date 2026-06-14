namespace Game.Domain.Combat
{
    public enum CombatActorValidationClassificationFailureReason
    {
        None,
        RuntimeMissing,
        ActorMissing,
        IncompletePlayerDependencies,
        UnknownActor
    }
}
