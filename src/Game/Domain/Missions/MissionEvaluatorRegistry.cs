using System.Collections.Generic;

namespace Game.Domain.Missions
{
    public class MissionEvaluatorRegistry
    {
        private readonly Dictionary<string, IMissionEvaluator> evaluators =
            new Dictionary<string, IMissionEvaluator>();

        public void Register(IMissionEvaluator evaluator)
        {
            evaluators[evaluator.MissionId] = evaluator;
        }

        public IMissionEvaluator Get(string missionId)
        {
            return evaluators[missionId];
        }
    }
}
