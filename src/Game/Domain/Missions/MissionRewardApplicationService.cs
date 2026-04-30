using Game.Domain.Player;

namespace Game.Domain.Missions
{
    public class MissionRewardApplicationService
    {
        public void ApplyReward(
            PlayerProfile profile,
            MissionRewardDefinition reward)
        {
            switch (reward.Type)
            {
                case MissionRewardType.FixTeaching:
                    profile.Cognition.FixTeaching(reward.Value);
                    break;

                case MissionRewardType.UnlockSkill:
                    profile.SkillBook.UnlockSkill(reward.Value);
                    break;

                case MissionRewardType.UnlockClass:
                    profile.ClassCollection.UnlockClass(reward.Value);
                    break;

                case MissionRewardType.GrantItem:
                case MissionRewardType.GrantWeapon:
                case MissionRewardType.GrantArmor:
                case MissionRewardType.GrantAccessory:
                    profile.Inventory.AddItem(reward.Value, reward.Amount);
                    break;

                case MissionRewardType.GrantCurrency:
                    profile.Wallet.AddCurrency(reward.Value, reward.Amount);
                    break;

                case MissionRewardType.UnlockCodexEntry:
                    profile.Codex.DiscoverEntry(reward.Value);
                    break;
            }
        }
    }
}
