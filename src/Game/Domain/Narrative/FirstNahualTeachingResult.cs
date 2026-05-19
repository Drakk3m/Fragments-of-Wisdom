using System.Collections.Generic;

namespace Game.Domain.Narrative
{
    public class FirstNahualTeachingResult
    {
        public bool AcceptedTeachingPath { get; private set; }
        public bool RejectedTeachingPath { get; private set; }
        public IReadOnlyList<string> NarrativeTags { get; private set; }

        public FirstNahualTeachingResult(
            bool acceptedTeachingPath,
            bool rejectedTeachingPath,
            IReadOnlyList<string> narrativeTags)
        {
            AcceptedTeachingPath = acceptedTeachingPath;
            RejectedTeachingPath = rejectedTeachingPath;
            NarrativeTags = narrativeTags;
        }
    }
}
