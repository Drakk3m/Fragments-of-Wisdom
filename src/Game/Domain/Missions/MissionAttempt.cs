using System.Collections.Generic;

namespace Game.Domain.Missions
{
    public class MissionAttempt
    {
        private readonly List<MissionActionType> actions = new List<MissionActionType>();

        public IReadOnlyList<MissionActionType> Actions
        {
            get { return actions.AsReadOnly(); }
        }

        public void Record(MissionActionType action)
        {
            actions.Add(action);
        }

        public bool Contains(MissionActionType action)
        {
            return actions.Contains(action);
        }

        public int IndexOf(MissionActionType action)
        {
            return actions.IndexOf(action);
        }
    }
}
