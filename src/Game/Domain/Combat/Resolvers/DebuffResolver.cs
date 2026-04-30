using Game.Domain.Common;
using Game.Domain.Combat.Passives;
using Game.Domain.Combat.StatusEffects;

namespace Game.Domain.Combat.Resolvers
{
    public class DebuffResolver
    {
        private readonly IRandomSource random;
        private readonly CombatPassivePipeline pipeline;

        public DebuffResolver(
            IRandomSource random,
            CombatPassivePipeline pipeline)
        {
            this.random = random;
            this.pipeline = pipeline;
        }

        public bool TryApply(
            CombatEntity source,
            CombatEntity target,
            IDebuff debuff,
            float baseChance,
            CombatContext context)
        {
            var debuffContext = new DebuffApplicationContext(source, target, baseChance);
            pipeline.OnBeforeDebuffApplied(debuffContext);

            bool success = ProbabilityCalculator.Roll(
                random,
                source.Stats.DebuffChance,
                target.Stats.DebuffResistance,
                debuffContext.FinalChance,
                0.005f,
                0.1f,
                0.9f);

            if (!success)
            {
                context.EventLog.Add(GameEventType.DebuffResisted, target.Name + " resists " + debuff.Name + ".");
                return false;
            }

            source.Flags.FirstDebuffAttemptThisCombat = false;
            target.Debuffs.Add(debuff, target, context);
            return true;
        }
    }
}
