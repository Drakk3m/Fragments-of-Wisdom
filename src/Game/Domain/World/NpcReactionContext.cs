using System;
using System.Collections.Generic;

namespace Game.Domain.World
{
    public class NpcReactionContext
    {
        public IReadOnlyList<string> NarrativeTags { get; private set; }

        public NpcReactionContext(IReadOnlyList<string> narrativeTags)
        {
            if (narrativeTags == null)
            {
                throw new ArgumentNullException(nameof(narrativeTags));
            }

            NarrativeTags = narrativeTags;
        }
    }
}
