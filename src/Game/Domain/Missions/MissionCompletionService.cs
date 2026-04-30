using Game.Domain.Common;
using Game.Domain.Player;

namespace Game.Domain.Missions
{
    public class MissionCompletionService
    {
        private readonly MissionDefinitionRegistry definitions;
        private readonly MissionEvaluatorRegistry evaluators;
        private readonly MissionRewardApplicationService rewardService;
        private readonly GameEventLog log;

        public MissionCompletionService(
            MissionDefinitionRegistry definitions,
            MissionEvaluatorRegistry evaluators,
            MissionRewardApplicationService rewardService,
            GameEventLog log)
        {
            this.definitions = definitions;
            this.evaluators = evaluators;
            this.rewardService = rewardService;
            this.log = log;
        }

        public MissionOutcome Complete(
            PlayerProfile profile,
            string missionId,
            MissionAttempt attempt)
        {
            var definition = definitions.Get(missionId);
            var evaluator = evaluators.Get(missionId);

            log.Add(GameEventType.MissionStarted, "Mission started: " + definition.Name + ".");

            MissionOutcome outcome = evaluator.Evaluate(profile, attempt);

            if (outcome != MissionOutcome.Success)
            {
                log.Add(GameEventType.MissionFailed, "Mission failed/retryable: " + definition.Name + ".");
                return outcome;
            }

            foreach (var reward in definition.Rewards)
            {
                rewardService.ApplyReward(profile, reward);
                log.Add(GameEventType.RewardGranted, "Reward granted: " + reward.Type + " -> " + reward.Value + " x" + reward.Amount + ".");
            }

            log.Add(GameEventType.MissionCompleted, "Mission completed: " + definition.Name + ".");
            return outcome;
        }
    }
}
