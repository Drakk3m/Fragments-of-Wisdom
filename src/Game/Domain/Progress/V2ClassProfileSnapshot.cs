namespace Game.Domain.Progress
{
    public class V2ClassProfileSnapshot
    {
        public string CurrentClassId { get; private set; }

        public V2ClassProfileSnapshot(string currentClassId)
        {
            CurrentClassId = currentClassId ?? string.Empty;
        }
    }
}
