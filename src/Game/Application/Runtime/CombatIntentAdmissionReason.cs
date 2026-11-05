namespace Game.Application.Runtime
{
    public enum CombatIntentAdmissionReason
    {
        None,
        NullIntent,
        UnknownIntentType,
        InvalidSkillId,
        SkillNotPrepared,
        EncounterNotStarted,
        EncounterTerminal,
        EncounterFaulted,
        CommandInFlight,
        ResonanceUnavailable,
        NotPlayerOpportunity,
        NoEnemyOpportunity
    }
}
