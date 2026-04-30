using Game.Domain.Skills;

namespace Game.Domain.Combat
{
    public class TurnRequest
    {
        public CombatEntity Attacker { get; private set; }
        public CombatEntity Defender { get; private set; }
        public TurnActionType ActionType { get; private set; }
        public ICombatSkill Skill { get; private set; }
        public SkillProgress SkillProgress { get; private set; }

        private TurnRequest(
            CombatEntity attacker,
            CombatEntity defender,
            TurnActionType actionType,
            ICombatSkill skill,
            SkillProgress progress)
        {
            Attacker = attacker;
            Defender = defender;
            ActionType = actionType;
            Skill = skill;
            SkillProgress = progress;
        }

        public static TurnRequest BasicAttack(
            CombatEntity attacker,
            CombatEntity defender)
        {
            return new TurnRequest(attacker, defender, TurnActionType.BasicAttack, null, null);
        }

        public static TurnRequest SkillAction(
            CombatEntity attacker,
            CombatEntity defender,
            ICombatSkill skill,
            SkillProgress progress)
        {
            return new TurnRequest(attacker, defender, TurnActionType.Skill, skill, progress);
        }
    }
}
