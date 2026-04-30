using Game.Domain.Codex;
using Game.Domain.Cognition;

namespace Game.Domain.Player
{
    public class PlayerProfile
    {
        public PlayerSkillBook SkillBook { get; private set; }
        public PlayerClassCollection ClassCollection { get; private set; }
        public PlayerInventory Inventory { get; private set; }
        public PlayerWallet Wallet { get; private set; }
        public NovaCognitionProfile Cognition { get; private set; }
        public CodexProfile Codex { get; private set; }

        public PlayerProfile()
        {
            SkillBook = new PlayerSkillBook();
            ClassCollection = new PlayerClassCollection();
            Inventory = new PlayerInventory();
            Wallet = new PlayerWallet();
            Cognition = new NovaCognitionProfile(2);
            Codex = new CodexProfile();
        }
    }
}
