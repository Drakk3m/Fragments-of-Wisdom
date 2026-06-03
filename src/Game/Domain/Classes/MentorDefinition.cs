namespace Game.Domain.Classes
{
    public class MentorDefinition
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public bool IsAvailable { get; private set; }

        public MentorDefinition(
            string id,
            string name,
            bool isAvailable)
        {
            Id = id;
            Name = name;
            IsAvailable = isAvailable;
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Id)
                && !string.IsNullOrWhiteSpace(Name);
        }
    }
}
