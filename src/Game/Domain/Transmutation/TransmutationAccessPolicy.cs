using System;
using System.Collections.Generic;
using Game.Domain.Alignment;
using Game.Domain.Narrative;
using Game.Domain.Player;

namespace Game.Domain.Transmutation
{
    public class TransmutationAccessPolicy
    {
        private const string RequiredPath = "nahual.first_teaching.accepted";

        private readonly FirstNahualTeachingEvaluator firstNahualTeachingEvaluator;

        public TransmutationAccessPolicy()
            : this(new FirstNahualTeachingEvaluator())
        {
        }

        public TransmutationAccessPolicy(FirstNahualTeachingEvaluator firstNahualTeachingEvaluator)
        {
            if (firstNahualTeachingEvaluator == null)
            {
                throw new ArgumentNullException(nameof(firstNahualTeachingEvaluator));
            }

            this.firstNahualTeachingEvaluator = firstNahualTeachingEvaluator;
        }

        public TransmutationAccessResult Evaluate(PlayerProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            return Evaluate(firstNahualTeachingEvaluator.Evaluate(profile));
        }

        public TransmutationAccessResult Evaluate(FirstNahualTeachingResult teachingResult)
        {
            if (teachingResult == null)
            {
                throw new ArgumentNullException(nameof(teachingResult));
            }

            if (teachingResult.AcceptedTeachingPath)
            {
                return new TransmutationAccessResult(
                    true,
                    "",
                    KnowledgeAlignment.NahualPath,
                    RequiredPath,
                    BuildNarrativeTags(teachingResult, true));
            }

            return new TransmutationAccessResult(
                false,
                "Requires accepted first Nahual teaching path.",
                KnowledgeAlignment.NahualPath,
                RequiredPath,
                BuildNarrativeTags(teachingResult, false));
        }

        private static IReadOnlyList<string> BuildNarrativeTags(
            FirstNahualTeachingResult teachingResult,
            bool canAccessTransmutation)
        {
            var tags = new List<string>();

            foreach (var narrativeTag in teachingResult.NarrativeTags)
            {
                tags.Add(narrativeTag);
            }

            tags.Add(canAccessTransmutation
                ? "transmutation.access.allowed"
                : "transmutation.access.blocked");

            return tags;
        }
    }
}
