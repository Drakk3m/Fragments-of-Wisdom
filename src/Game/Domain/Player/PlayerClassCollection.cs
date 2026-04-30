using System.Collections.Generic;

namespace Game.Domain.Player
{
    public class PlayerClassCollection
    {
        private readonly HashSet<string> classes = new HashSet<string>();

        public string ActiveClassId { get; private set; }

        public bool UnlockClass(string classId)
        {
            return classes.Add(classId);
        }

        public bool HasClass(string classId)
        {
            return classes.Contains(classId);
        }

        public bool SetActiveClass(string classId)
        {
            if (!classes.Contains(classId))
            {
                return false;
            }

            ActiveClassId = classId;
            return true;
        }
    }
}
