using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Domain.Cognition
{
    public class NovaCognitionProfile
    {
        private readonly Dictionary<CognitiveArchetypeId, HashSet<string>> unlocked =
            new Dictionary<CognitiveArchetypeId, HashSet<string>>();

        private readonly HashSet<string> temporary = new HashSet<string>();
        private readonly HashSet<string> fixedTeachings = new HashSet<string>();
        private readonly HashSet<string> equipped = new HashSet<string>();

        public CognitiveArchetypeId? ActiveArchetype { get; private set; }
        public int MaxEquippedFixedTeachings { get; private set; }

        public NovaCognitionProfile(int maxEquippedFixedTeachings)
        {
            MaxEquippedFixedTeachings = maxEquippedFixedTeachings;

            foreach (CognitiveArchetypeId id in Enum.GetValues(typeof(CognitiveArchetypeId)))
            {
                unlocked[id] = new HashSet<string>();
            }
        }

        public void UnlockTeaching(
            CognitiveArchetypeId archetype,
            string teachingId)
        {
            unlocked[archetype].Add(teachingId);

            if (ActiveArchetype == archetype)
            {
                temporary.Add(teachingId);
            }
        }

        public void SetActiveArchetype(CognitiveArchetypeId archetype)
        {
            ActiveArchetype = archetype;
            temporary.Clear();

            foreach (var teaching in unlocked[archetype])
            {
                temporary.Add(teaching);
            }
        }

        public bool FixTeaching(string teachingId)
        {
            return fixedTeachings.Add(teachingId);
        }

        public bool EquipFixedTeaching(string teachingId)
        {
            if (!fixedTeachings.Contains(teachingId))
            {
                return false;
            }

            if (equipped.Count >= MaxEquippedFixedTeachings)
            {
                return false;
            }

            return equipped.Add(teachingId);
        }

        public bool HasActiveTeaching(string teachingId)
        {
            return temporary.Contains(teachingId) || equipped.Contains(teachingId);
        }

        public IReadOnlyCollection<string> GetActiveTeachings()
        {
            return temporary.Concat(equipped).ToList().AsReadOnly();
        }
    }
}
