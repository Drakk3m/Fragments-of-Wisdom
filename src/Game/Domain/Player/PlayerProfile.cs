using System.Collections.Generic;
using Game.Domain.Alignment;
using Game.Domain.Codex;
using Game.Domain.Cognition;

namespace Game.Domain.Player
{
    public class PlayerProfile
    {
        private readonly HashSet<string> alignmentFlags = new HashSet<string>();

        public PlayerSkillBook SkillBook { get; private set; }
        public PlayerClassCollection ClassCollection { get; private set; }
        public PlayerInventory Inventory { get; private set; }
        public PlayerWallet Wallet { get; private set; }
        public NovaCognitionProfile Cognition { get; private set; }
        public CodexProfile Codex { get; private set; }
        public KnowledgeAlignment CurrentAlignment { get; private set; }
        public AlignmentProgression AlignmentProgression { get; private set; }

        public IReadOnlyCollection<string> AlignmentFlags
        {
            get { return alignmentFlags; }
        }

        public PlayerProfile()
        {
            SkillBook = new PlayerSkillBook();
            ClassCollection = new PlayerClassCollection();
            Inventory = new PlayerInventory();
            Wallet = new PlayerWallet();
            Cognition = new NovaCognitionProfile(2);
            Codex = new CodexProfile();
            CurrentAlignment = KnowledgeAlignment.SharedWisdom;
            AlignmentProgression = new AlignmentProgression();
        }

        public void SetAlignment(KnowledgeAlignment alignment)
        {
            CurrentAlignment = alignment;
        }

        public void AddAlignmentProgress(
            KnowledgeAlignment alignment,
            int amount)
        {
            AlignmentProgression.AddProgress(alignment, amount);
        }

        public void AddAlignmentFlag(string flag)
        {
            if (!string.IsNullOrEmpty(flag))
            {
                alignmentFlags.Add(flag);
            }
        }

        public bool HasAlignmentFlag(string flag)
        {
            return alignmentFlags.Contains(flag);
        }
    }
}
