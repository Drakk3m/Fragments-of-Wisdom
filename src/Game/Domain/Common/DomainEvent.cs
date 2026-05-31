using System;
using System.Collections.Generic;

namespace Game.Domain.Common
{
    public class DomainEvent
    {
        private readonly Dictionary<string, string> metadata;

        public DomainEventType Type { get; private set; }
        public string SubjectId { get; private set; }
        public string Message { get; private set; }

        public IReadOnlyDictionary<string, string> Metadata
        {
            get { return metadata; }
        }

        public DomainEvent(
            DomainEventType type,
            string subjectId,
            string message)
            : this(type, subjectId, message, null)
        {
        }

        public DomainEvent(
            DomainEventType type,
            string subjectId,
            string message,
            IDictionary<string, string> metadata)
        {
            Type = type;
            SubjectId = subjectId ?? string.Empty;
            Message = message ?? string.Empty;
            this.metadata = metadata == null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>(metadata, StringComparer.Ordinal);
        }
    }
}
