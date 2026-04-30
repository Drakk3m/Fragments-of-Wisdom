namespace Game.Domain.Combat.Stats
{
    public class CombatStats
    {
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }

        public int Attack { get; set; }
        public int Defense { get; set; }

        public int Precision { get; set; }
        public int Evasion { get; set; }

        public int CriticalChance { get; set; }
        public int CriticalResistance { get; set; }
        public float CriticalDamageMultiplier { get; set; }

        public int DebuffChance { get; set; }
        public int DebuffResistance { get; set; }

        public int BuffPotency { get; set; }
        public int BuffResistance { get; set; }

        public int ControlResistance { get; set; }

        public CombatStats Clone()
        {
            return (CombatStats)MemberwiseClone();
        }
    }
}
