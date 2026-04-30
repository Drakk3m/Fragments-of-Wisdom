namespace Game.Domain.Missions
{
    public class MissionRewardDefinition
    {
        public MissionRewardType Type { get; private set; }
        public string Value { get; private set; }
        public int Amount { get; private set; }

        public MissionRewardDefinition(
            MissionRewardType type,
            string value,
            int amount = 1)
        {
            Type = type;
            Value = value;
            Amount = amount;
        }
    }
}
