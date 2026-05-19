using System;
using System.Collections.Generic;

namespace Game.Domain.Alignment
{
    public class AlignmentProgression
    {
        private readonly Dictionary<KnowledgeAlignment, int> points =
            new Dictionary<KnowledgeAlignment, int>();

        public AlignmentProgression()
        {
            foreach (KnowledgeAlignment alignment in Enum.GetValues(typeof(KnowledgeAlignment)))
            {
                points[alignment] = 0;
            }
        }

        public IReadOnlyDictionary<KnowledgeAlignment, int> Points
        {
            get { return points; }
        }

        public void AddProgress(
            KnowledgeAlignment alignment,
            int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            points[alignment] = GetProgress(alignment) + amount;
        }

        public int GetProgress(KnowledgeAlignment alignment)
        {
            int value;

            if (!points.TryGetValue(alignment, out value))
            {
                return 0;
            }

            return value;
        }
    }
}
