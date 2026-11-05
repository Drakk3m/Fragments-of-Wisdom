namespace Game.Application.Runtime
{
    /// <summary>Player intent only. Actor and target are resolved by the encounter owner.</summary>
    public sealed class CombatIntent
    {
        public CombatIntentType Type { get; }
        public string SkillId { get; }
        public int RefillAmount { get; }

        private CombatIntent(CombatIntentType type, string skillId, int refillAmount)
        {
            Type = type;
            SkillId = skillId;
            RefillAmount = refillAmount;
        }

        public static CombatIntent BasicAttack() { return new CombatIntent(CombatIntentType.BasicAttack, null, 0); }
        public static CombatIntent Skill(string skillId) { return new CombatIntent(CombatIntentType.PreparedSkill, skillId, 0); }
        public static CombatIntent Refill(int amount) { return new CombatIntent(CombatIntentType.ResonanceRefill, null, amount); }
    }
}
