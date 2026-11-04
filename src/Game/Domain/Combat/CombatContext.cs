using System.Collections.Generic;
using Game.Domain.Common;
using Game.Domain.Combat.StatusEffects;

namespace Game.Domain.Combat
{
    public class CombatContext
    {
        public CombatEntity Attacker { get; private set; }
        public CombatEntity Defender { get; private set; }
        public GameEventLog EventLog { get; private set; }

        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public float DamageMultiplier { get; set; }
        public int FlatDamageBonus { get; set; }

        public int ResourceCost { get; set; }
        public float ResourceCostMultiplier { get; set; }

        public float HitChanceModifier { get; set; }
        public float CriticalChanceModifier { get; set; }

        public bool RevealOneEnemyTrait { get; set; }
        public bool IsFirstSkillOfCombat { get; set; }

        // Per-action capability restriction; only the engine's Skill branch consumes it.
        internal bool SkillsBlockedBySilence { get; set; }

        public CleanseType? RequestedCleanse { get; set; }

        public List<StatusApplicationRequest> DebuffsToApply { get; private set; }

        public CombatContext(
            CombatEntity attacker,
            CombatEntity defender,
            GameEventLog eventLog)
        {
            Attacker = attacker;
            Defender = defender;
            EventLog = eventLog;
            BaseAttack = attacker.Stats.Attack;
            BaseDefense = defender.Stats.Defense;
            DamageMultiplier = 1f;
            ResourceCostMultiplier = 1f;
            DebuffsToApply = new List<StatusApplicationRequest>();
        }

        public void RequestDebuff(
            IDebuff debuff,
            float baseChance)
        {
            DebuffsToApply.Add(new StatusApplicationRequest(debuff, baseChance));
        }

        public void RequestCleanse(CleanseType cleanseType)
        {
            RequestedCleanse = cleanseType;
        }
    }
}
