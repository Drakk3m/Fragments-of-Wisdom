using Game.Domain.Common;
using Game.Domain.Player;

namespace Game.Domain.Combat.Outcomes
{
    public class BattleRewardResolver
    {
        public void ApplyRewards(
            PlayerProfile profile,
            BattleOutcome outcome,
            BattleRewardDefinition reward,
            GameEventLog log)
        {
            if (outcome.Winner != BattleWinner.Player)
            {
                return;
            }

            profile.Wallet.AddCurrency(reward.CurrencyId, reward.CurrencyAmount);
            log.Add(GameEventType.BattleRewardGranted, "Battle reward granted: " + reward.CurrencyId + " x" + reward.CurrencyAmount + ".");
        }
    }
}
