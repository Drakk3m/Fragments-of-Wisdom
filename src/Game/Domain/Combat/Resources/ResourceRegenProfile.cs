namespace Game.Domain.Combat.Resources
{
    public class ResourceRegenProfile
    {
        public int PassiveRegenPerTurn { get; private set; }
        public int BasicAttackRegen { get; private set; }
        public int DefendRegen { get; private set; }
        public int WaitRegen { get; private set; }

        public ResourceRegenProfile(
            int passiveRegenPerTurn,
            int basicAttackRegen,
            int defendRegen,
            int waitRegen)
        {
            PassiveRegenPerTurn = passiveRegenPerTurn;
            BasicAttackRegen = basicAttackRegen;
            DefendRegen = defendRegen;
            WaitRegen = waitRegen;
        }
    }
}
