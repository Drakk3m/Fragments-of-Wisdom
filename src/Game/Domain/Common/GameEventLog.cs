using System.Collections.Generic;

namespace Game.Domain.Common
{
    public class GameEventLog
    {
        private readonly List<GameEvent> events = new List<GameEvent>();

        public IReadOnlyList<GameEvent> Events
        {
            get { return events.AsReadOnly(); }
        }

        public void Add(GameEventType type, string message)
        {
            events.Add(new GameEvent(type, message));
        }
    }
}
