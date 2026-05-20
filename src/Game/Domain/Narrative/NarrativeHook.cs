using System.Collections.Generic;

namespace Game.Domain.Narrative
{
    public class NarrativeHook
    {
        public string HookId { get; private set; }
        public NarrativeHookType HookType { get; private set; }
        public int Priority { get; private set; }
        public IReadOnlyList<string> SourceTags { get; private set; }
        public string SuggestedSystem { get; private set; }

        public NarrativeHook(
            string hookId,
            NarrativeHookType hookType,
            int priority,
            IReadOnlyList<string> sourceTags,
            string suggestedSystem)
        {
            HookId = hookId;
            HookType = hookType;
            Priority = priority;
            SourceTags = sourceTags;
            SuggestedSystem = suggestedSystem;
        }
    }
}
