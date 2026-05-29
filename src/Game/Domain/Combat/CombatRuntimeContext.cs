using Game.Domain.Common;
using Game.Domain.Combat.Resources;
using Game.Domain.Player;

namespace Game.Domain.Combat
{
    public class CombatRuntimeContext
    {
        public CombatEntity Player { get; private set; }
        public CombatEntity Enemy { get; private set; }
        public GameEventLog EventLog { get; private set; }
        public PlayerProfile PlayerProfile { get; private set; }
        public ResonanceProfile ResonanceProfile { get; private set; }
        public int TurnNumber { get; private set; }
        public bool CombatEnded { get; private set; }

        public CombatRuntimeContext(
            CombatEntity player,
            CombatEntity enemy,
            GameEventLog eventLog,
            PlayerProfile playerProfile)
            : this(
                player,
                enemy,
                eventLog,
                playerProfile,
                null)
        {
        }

        public CombatRuntimeContext(
            CombatEntity player,
            CombatEntity enemy,
            GameEventLog eventLog,
            PlayerProfile playerProfile,
            ResonanceProfile resonanceProfile)
        {
            Player = player;
            Enemy = enemy;
            EventLog = eventLog;
            PlayerProfile = playerProfile;
            ResonanceProfile = resonanceProfile;
            TurnNumber = 1;
        }

        public void AdvanceTurn()
        {
            TurnNumber++;
        }

        public void EndCombat()
        {
            CombatEnded = true;
        }
    }
}
