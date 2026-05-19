using System;
using System.Collections.Generic;
using Game.Domain.Alignment;
using Game.Domain.Player;

namespace Game.Domain.Narrative
{
    public class FirstNahualTeachingEvaluator
    {
        public FirstNahualTeachingResult Evaluate(PlayerProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            bool hasDiscoveredTeaching = profile.HasAlignmentFlag(AlignmentFlag.DiscoveredNahualPath);
            bool hasAcceptedTeaching = profile.HasAlignmentFlag(AlignmentFlag.AcceptedFirstNahualTeaching);
            int nahualProgress = profile.AlignmentProgression.GetProgress(KnowledgeAlignment.NahualPath);
            int sharedWisdomProgress = profile.AlignmentProgression.GetProgress(KnowledgeAlignment.SharedWisdom);

            bool acceptedTeachingPath =
                profile.CurrentAlignment == KnowledgeAlignment.NahualPath
                || hasAcceptedTeaching
                || nahualProgress > 0;

            bool rejectedTeachingPath =
                !acceptedTeachingPath
                && profile.CurrentAlignment == KnowledgeAlignment.SharedWisdom
                && (hasDiscoveredTeaching || sharedWisdomProgress > 0);

            return new FirstNahualTeachingResult(
                acceptedTeachingPath,
                rejectedTeachingPath,
                BuildNarrativeTags(
                    profile.CurrentAlignment,
                    hasDiscoveredTeaching,
                    acceptedTeachingPath,
                    rejectedTeachingPath));
        }

        private static IReadOnlyList<string> BuildNarrativeTags(
            KnowledgeAlignment currentAlignment,
            bool hasDiscoveredTeaching,
            bool acceptedTeachingPath,
            bool rejectedTeachingPath)
        {
            var tags = new List<string>();

            tags.Add("alignment." + currentAlignment);

            if (hasDiscoveredTeaching)
            {
                tags.Add("nahual.first_teaching.discovered");
            }

            if (acceptedTeachingPath)
            {
                tags.Add("nahual.first_teaching.accepted");
            }
            else if (rejectedTeachingPath)
            {
                tags.Add("nahual.first_teaching.rejected");
            }

            return tags;
        }
    }
}
