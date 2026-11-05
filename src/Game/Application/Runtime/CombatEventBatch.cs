using System.Collections.Generic;
using Game.Domain.Common;

namespace Game.Application.Runtime
{
    /// <summary>Half-open range in one encounter's log. Cursor ownership stays with the consumer.</summary>
    public sealed class CombatEventBatch
    {
        public int StartIndex { get; }
        public int EndIndex { get; }
        public IReadOnlyList<CombatEventSnapshot> Events { get; }

        internal CombatEventBatch(int startIndex, int endIndex, IReadOnlyList<GameEvent> log)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
            var events = new List<CombatEventSnapshot>();
            for (int i = startIndex; i < endIndex; i++) events.Add(new CombatEventSnapshot(i, log[i]));
            Events = events.AsReadOnly();
        }

        internal bool Contains(GameEventType type)
        {
            foreach (var item in Events)
                if (item.Type == type) return true;
            return false;
        }
    }
}
