using Game.Domain.Common;

namespace Game.Domain.Combat.AI
{
    public class BasicAttackEnemyAiStrategy : IEnemyAiStrategy
    {
        public TurnRequest ChooseAction(CombatRuntimeContext runtime)
        {
            runtime.EventLog.Add(GameEventType.AiDecisionMade, runtime.Enemy.Name + " chooses Basic Attack.");
            return TurnRequest.BasicAttack(runtime.Enemy, runtime.Player);
        }
    }
}
