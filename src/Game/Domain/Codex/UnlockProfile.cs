using System.Collections.Generic;

namespace Game.Domain.Codex
{
    public class UnlockProfile
    {
        private readonly HashSet<string> unlockedIds = new HashSet<string>();
        private readonly List<UnlockRecord> unlocks = new List<UnlockRecord>();

        public IReadOnlyCollection<string> UnlockedIds
        {
            get { return unlockedIds; }
        }

        public IReadOnlyList<UnlockRecord> Unlocks
        {
            get { return unlocks; }
        }

        public bool RecordUnlock(UnlockRecord unlock)
        {
            if (unlock == null)
            {
                return false;
            }

            if (unlockedIds.Contains(unlock.UnlockId))
            {
                return false;
            }

            unlockedIds.Add(unlock.UnlockId);
            unlocks.Add(unlock);
            return true;
        }

        public bool HasUnlock(string unlockId)
        {
            return unlockedIds.Contains(unlockId);
        }

        public UnlockRecord GetUnlock(string unlockId)
        {
            foreach (var unlock in unlocks)
            {
                if (unlock.UnlockId == unlockId)
                {
                    return unlock;
                }
            }

            return null;
        }
    }
}
