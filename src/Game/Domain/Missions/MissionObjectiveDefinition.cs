namespace Game.Domain.Missions
{
    public class MissionObjectiveDefinition
    {
        public string Id { get; private set; }
        public MissionObjectiveType Type { get; private set; }
        public string TargetValue { get; private set; }
        public int RequiredAmount { get; private set; }

        public MissionObjectiveDefinition(
            string id,
            MissionObjectiveType type,
            string targetValue,
            int requiredAmount)
        {
            Id = id;
            Type = type;
            TargetValue = targetValue;
            RequiredAmount = requiredAmount;
        }
    }
}
