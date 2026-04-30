namespace Game.Domain.Cognition
{
    public class TeachingDefinition
    {
        public string Id { get; private set; }
        public CognitiveArchetypeId ArchetypeId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        public TeachingDefinition(
            string id,
            CognitiveArchetypeId archetypeId,
            string name,
            string description)
        {
            Id = id;
            ArchetypeId = archetypeId;
            Name = name;
            Description = description;
        }
    }
}
