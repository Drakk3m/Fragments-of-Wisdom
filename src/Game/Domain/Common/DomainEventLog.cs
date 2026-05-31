using System.Collections.Generic;

namespace Game.Domain.Common
{
    public class DomainEventLog
    {
        private readonly List<DomainEvent> events = new List<DomainEvent>();

        public IReadOnlyList<DomainEvent> Events
        {
            get { return events.AsReadOnly(); }
        }

        public void Add(DomainEvent domainEvent)
        {
            if (domainEvent != null)
            {
                events.Add(domainEvent);
            }
        }

        public void Record(
            DomainEventType type,
            string subjectId,
            string message)
        {
            Add(new DomainEvent(type, subjectId, message));
        }

        public void Record(
            DomainEventType type,
            string subjectId,
            string message,
            IDictionary<string, string> metadata)
        {
            Add(new DomainEvent(type, subjectId, message, metadata));
        }
    }
}
