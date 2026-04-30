using System.Collections.Generic;

namespace Game.Application.Persistence
{
    public class PlayerSaveData
    {
        public string Version { get; set; }
        public string ActiveArchetype { get; set; }
        public List<string> FixedTeachings { get; set; }
        public List<string> EquippedTeachings { get; set; }
        public Dictionary<string, int> Items { get; set; }
        public Dictionary<string, int> Currencies { get; set; }

        public PlayerSaveData()
        {
            FixedTeachings = new List<string>();
            EquippedTeachings = new List<string>();
            Items = new Dictionary<string, int>();
            Currencies = new Dictionary<string, int>();
        }
    }
}
