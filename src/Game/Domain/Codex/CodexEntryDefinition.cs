namespace Game.Domain.Codex
{
    public class CodexEntryDefinition
    {
        public string Id { get; private set; }
        public CodexCategory Category { get; private set; }
        public string Name { get; private set; }

        public CodexEntryDefinition(
            string id,
            CodexCategory category,
            string name)
        {
            Id = id;
            Category = category;
            Name = name;
        }
    }
}
