namespace Game.Domain.Common
{
    public class GameEvent
    {
        public GameEventType Type { get; private set; }
        public string Message { get; private set; }

        public GameEvent(GameEventType type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}
