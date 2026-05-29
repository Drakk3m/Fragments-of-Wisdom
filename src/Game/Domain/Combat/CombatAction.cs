namespace Game.Domain.Combat
{
    public class CombatAction
    {
        public string ActionId { get; private set; }
        public string Name { get; private set; }
        public TurnActionType ActionType { get; private set; }
        public float DamageMultiplier { get; private set; }
        public int ResourceCost { get; private set; }

        private CombatAction(
            string actionId,
            string name,
            TurnActionType actionType,
            float damageMultiplier,
            int resourceCost)
        {
            ActionId = actionId;
            Name = name;
            ActionType = actionType;
            DamageMultiplier = damageMultiplier;
            ResourceCost = resourceCost;
        }

        public static CombatAction BasicAttack()
        {
            return new CombatAction(
                "combat.basic_attack",
                "Basic Attack",
                TurnActionType.BasicAttack,
                1f,
                0);
        }

        public static CombatAction SkillAction(
            string actionId,
            string name,
            float damageMultiplier,
            int resourceCost)
        {
            return new CombatAction(
                actionId,
                name,
                TurnActionType.Skill,
                damageMultiplier,
                resourceCost);
        }
    }
}
