using Game.Domain.Common;
using Game.Domain.Combat.Passives;
using Game.Domain.Combat.Resolvers;
using Game.Domain.Combat.StatusEffects;

namespace Game.Domain.Combat
{
    public class TurnBasedCombatEngine
    {
        private readonly IRandomSource random;
        private readonly CombatPassivePipeline pipeline;
        private readonly DamageResolver damageResolver;
        private readonly DebuffResolver debuffResolver;
        private readonly StatusEffectResolver statusEffectResolver;
        private readonly ICombatSkillTurnValidator skillTurnValidator;

        public TurnBasedCombatEngine(
            IRandomSource random,
            CombatPassivePipeline pipeline)
            : this(random, pipeline, null)
        {
        }

        public TurnBasedCombatEngine(
            IRandomSource random,
            CombatPassivePipeline pipeline,
            ICombatSkillTurnValidator skillTurnValidator)
        {
            this.random = random;
            this.pipeline = pipeline;
            this.skillTurnValidator = skillTurnValidator;
            damageResolver = new DamageResolver(random);
            debuffResolver = new DebuffResolver(random, pipeline);
            statusEffectResolver = new StatusEffectResolver();
        }

        public void StartCombat(CombatRuntimeContext runtime)
        {
            var context = new CombatContext(runtime.Player, runtime.Enemy, runtime.EventLog);
            pipeline.OnCombatStart(context);

            if (runtime.PlayerProfile.Cognition.HasActiveTeaching("lyra_detect_weakness"))
            {
                context.RevealOneEnemyTrait = true;
                runtime.EventLog.Add(GameEventType.CognitionTriggered, "Lyra teaching Ojo de la Grieta reveals enemy structure.");
            }

            runtime.EventLog.Add(GameEventType.CombatStarted, "Combat starts: " + runtime.Player.Name + " vs " + runtime.Enemy.Name + ".");

            if (context.RevealOneEnemyTrait)
            {
                runtime.EventLog.Add(GameEventType.TraitRevealed, "Enemy Debuff Resistance = " + runtime.Enemy.Stats.DebuffResistance + ".");
            }
        }

        public void ExecuteTurn(
            CombatRuntimeContext runtime,
            TurnRequest request)
        {
            if (runtime.CombatEnded || !request.Attacker.IsAlive || !request.Defender.IsAlive)
            {
                return;
            }

            var context = new CombatContext(request.Attacker, request.Defender, runtime.EventLog);
            context.IsFirstSkillOfCombat = runtime.TurnNumber == 1 && request.ActionType == TurnActionType.Skill;

            if (runtime.PlayerProfile.Cognition.HasActiveTeaching("atlas_stable_cast")
                && request.Attacker == runtime.Player
                && request.ActionType == TurnActionType.Skill)
            {
                context.ResourceCostMultiplier *= 0.9f;
                runtime.EventLog.Add(GameEventType.CognitionTriggered, "Atlas teaching FÃ³rmula Estable reduces skill cost.");
            }

            runtime.EventLog.Add(GameEventType.TurnStarted, "Turn " + runtime.TurnNumber + ": " + request.Attacker.Name + " acts.");

            request.Attacker.Flags.ResetTurnFlags();
            request.Attacker.Debuffs.OnTurnStart(request.Attacker, context);
            request.Attacker.Debuffs.OnBeforeAction(context);

            if (!request.Attacker.IsAlive)
            {
                runtime.EventLog.Add(GameEventType.CombatEnded, request.Attacker.Name + " is defeated by status effects.");
                runtime.EndCombat();
                return;
            }

            if (request.ActionType == TurnActionType.Skill)
            {
                if (skillTurnValidator != null)
                {
                    var validation = skillTurnValidator.Validate(request, runtime);

                    if (!validation.IsAllowed)
                    {
                        runtime.EventLog.Add(
                            GameEventType.ActionBlocked,
                            BuildSkillValidationBlockedMessage(
                                request.Attacker,
                                validation));
                        EndTurn(runtime, context);
                        return;
                    }
                }

                if (request.Attacker.StatusEffects.HasActive(StatusEffectType.SkillSeal))
                {
                    runtime.EventLog.Add(GameEventType.ActionBlocked, request.Attacker.Name + " is sealed and cannot use skills.");
                    EndTurn(runtime, context);
                    return;
                }

                context.ResourceCost = request.SkillProgress.GetBaseCost();
                context.ResourceCostMultiplier *= request.SkillProgress.GetMasteryCostMultiplier();
                pipeline.OnBeforeSkillCostCalculated(context);

                int finalCost = (int)System.Math.Ceiling(context.ResourceCost * context.ResourceCostMultiplier);
                int resonanceCost = request.SkillProgress.Definition.ResonanceCost;

                if (resonanceCost > 0
                    && (runtime.ResonanceProfile == null || !runtime.ResonanceProfile.CanSpendActive(resonanceCost)))
                {
                    runtime.EventLog.Add(GameEventType.ResourceNotEnough, request.Attacker.Name + " lacks Resonance.");
                    EndTurn(runtime, context);
                    return;
                }

                if (!request.Attacker.Resource.TrySpend(finalCost))
                {
                    runtime.EventLog.Add(GameEventType.ResourceNotEnough, request.Attacker.Name + " lacks Essence.");
                    EndTurn(runtime, context);
                    return;
                }

                runtime.EventLog.Add(GameEventType.ResourceSpent, request.Attacker.Name + " spends " + finalCost + " Essence.");
                if (resonanceCost > 0)
                {
                    runtime.ResonanceProfile.TrySpendActive(resonanceCost);
                    runtime.EventLog.Add(GameEventType.ResourceSpent, request.Attacker.Name + " spends " + resonanceCost + " Resonance.");
                }

                request.Skill.Apply(context, request.SkillProgress);
                request.SkillProgress.GainExperience(10);
            }
            else
            {
                runtime.EventLog.Add(GameEventType.BasicAttackUsed, request.Attacker.Name + " uses a basic attack.");
            }

            if (context.RequestedCleanse.HasValue)
            {
                int removed = request.Attacker.Debuffs.RemoveOne(random);
                runtime.EventLog.Add(GameEventType.CleanseResolved, request.Attacker.Name + " cleanses " + removed + " debuff(s).");
            }

            DamageResult result = damageResolver.Resolve(context);

            if (!result.Hit)
            {
                runtime.EventLog.Add(GameEventType.DamageMissed, request.Attacker.Name + " misses.");
            }
            else
            {
                request.Defender.ReceiveDamage(result.Damage);

                if (result.Critical)
                {
                    runtime.EventLog.Add(GameEventType.CriticalHit, request.Attacker.Name + " lands a critical hit.");
                }

                runtime.EventLog.Add(GameEventType.DamageDealt, request.Attacker.Name + " deals " + result.Damage + " damage.");
            }

            foreach (var debuff in context.DebuffsToApply)
            {
                debuffResolver.TryApply(request.Attacker, request.Defender, debuff.Debuff, debuff.BaseChance, context);
            }

            if (!request.Defender.IsAlive)
            {
                runtime.EventLog.Add(GameEventType.CombatEnded, request.Defender.Name + " is defeated.");
                runtime.EndCombat();
                return;
            }

            EndTurn(runtime, context);
        }

        private static string BuildSkillValidationBlockedMessage(
            CombatEntity attacker,
            CombatSkillTurnValidationResult validation)
        {
            string reason;

            if (validation.BlockReason != CombatSkillUseBlockReason.None)
            {
                reason = validation.BlockReason.ToString();
            }
            else if (validation.ClassificationFailureReason
                != CombatActorValidationClassificationFailureReason.None)
            {
                reason = validation.ClassificationFailureReason.ToString();
            }
            else
            {
                reason = validation.FailureStage.ToString();
            }

            return attacker.Name
                + " cannot use the skill. Validation reason: "
                + reason
                + ".";
        }

        private void EndTurn(
            CombatRuntimeContext runtime,
            CombatContext context)
        {
            if (!context.Attacker.Flags.ResourceRegenBlocked)
            {
                context.Attacker.Resource.Regenerate(5);
                runtime.EventLog.Add(GameEventType.ResourceRegenerated, context.Attacker.Name + " regenerates 5 Essence.");
            }

            context.Attacker.Debuffs.OnAfterAction(context.Attacker, context);
            statusEffectResolver.TickTurnBoundary(context.Attacker);
            if (!context.Attacker.IsAlive)
            {
                runtime.EventLog.Add(GameEventType.CombatEnded, context.Attacker.Name + " is defeated by status effects.");
                runtime.EndCombat();
                return;
            }

            runtime.EventLog.Add(GameEventType.TurnEnded, "Turn " + runtime.TurnNumber + " ends.");
            runtime.AdvanceTurn();
        }
    }
}
