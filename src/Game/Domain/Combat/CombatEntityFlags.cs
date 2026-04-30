namespace Game.Domain.Combat
{
    public class CombatEntityFlags
    {
        public bool FirstDebuffAttemptThisCombat { get; set; }
        public bool ResourceRegenBlocked { get; set; }

        public CombatEntityFlags()
        {
            FirstDebuffAttemptThisCombat = true;
        }

        public void ResetTurnFlags()
        {
            ResourceRegenBlocked = false;
        }
    }
}
