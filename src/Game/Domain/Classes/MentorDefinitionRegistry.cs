using System.Collections.Generic;

namespace Game.Domain.Classes
{
    public class MentorDefinitionRegistry
    {
        private readonly Dictionary<string, MentorDefinition> definitionsById =
            new Dictionary<string, MentorDefinition>();

        public bool Register(MentorDefinition mentor)
        {
            if (!IsValidMentor(mentor) || definitionsById.ContainsKey(mentor.Id))
            {
                return false;
            }

            definitionsById.Add(mentor.Id, mentor);
            return true;
        }

        public bool Contains(string mentorId)
        {
            return !string.IsNullOrWhiteSpace(mentorId)
                && definitionsById.ContainsKey(mentorId);
        }

        public MentorDefinition Get(string mentorId)
        {
            MentorDefinition mentor;
            return !string.IsNullOrWhiteSpace(mentorId)
                && definitionsById.TryGetValue(mentorId, out mentor)
                    ? mentor
                    : null;
        }

        private static bool IsValidMentor(MentorDefinition mentor)
        {
            return mentor != null && mentor.IsValid();
        }
    }
}
