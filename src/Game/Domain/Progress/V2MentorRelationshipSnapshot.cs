namespace Game.Domain.Progress
{
    public class V2MentorRelationshipSnapshot
    {
        public string MentorId { get; private set; }
        public bool IsDiscovered { get; private set; }
        public int Trust { get; private set; }
        public bool IsAvailable { get; private set; }

        public V2MentorRelationshipSnapshot(
            string mentorId,
            bool isDiscovered,
            int trust,
            bool isAvailable)
        {
            MentorId = mentorId;
            IsDiscovered = isDiscovered;
            Trust = trust;
            IsAvailable = isAvailable;
        }
    }
}
