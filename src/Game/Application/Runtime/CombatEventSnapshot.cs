using Game.Domain.Common;

namespace Game.Application.Runtime
{
    public sealed class CombatEventSnapshot
    {
        public int Index { get; }
        public GameEventType Type { get; }
        public string Message { get; }

        internal CombatEventSnapshot(int index, GameEvent source)
        {
            Index = index;
            Type = source.Type;
            Message = source.Message;
        }
    }
}
