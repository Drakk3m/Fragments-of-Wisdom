
#requires -version 5.1
<#
02 Status Skills Resolvers Battle
PowerShell 5.1 compatible.
#>

param(
    [string]$Root = (Join-Path (Get-Location) "NovaFoundation")
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = "Stop"

function Write-RepoFile
{
    param(
        [string]$Path,
        [string]$Content
    )

    $fullPath = Join-Path $Root $Path
    $folder = Split-Path -Parent $fullPath

    if (-not (Test-Path -LiteralPath $folder))
    {
        New-Item -ItemType Directory -Path $folder -Force | Out-Null
    }

    $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($fullPath, $Content.Trim() + [Environment]::NewLine, $utf8NoBom)
    Write-Host "[WRITE] $Path"
}

if (-not (Test-Path -LiteralPath $Root))
{
    New-Item -ItemType Directory -Path $Root -Force | Out-Null
}

Write-Host ""
Write-Host "=== 02 Status Skills Resolvers Battle ===" -ForegroundColor Cyan
Write-Host "Target: $Root"
Write-Host ""

Write-RepoFile -Path "src/Game/Domain/Combat/StatusEffects/DebuffType.cs" -Content @'
namespace Game.Domain.Combat.StatusEffects
{
    public enum DebuffType
    {
        DamageOverTime,
        ResourceControl,
        ActionBlock,
        Accuracy,
        CostIncrease,
        Vulnerability
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/StatusEffects/IDebuff.cs" -Content @'
namespace Game.Domain.Combat.StatusEffects
{
    public interface IDebuff
    {
        string Name { get; }
        DebuffType Type { get; }
        int RemainingTurns { get; }
        bool IsExpired { get; }

        void OnTurnStart(
            CombatEntity target,
            CombatContext context);

        void OnBeforeAction(CombatContext context);

        void Tick();
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/StatusEffects/BaseDebuff.cs" -Content @'
namespace Game.Domain.Combat.StatusEffects
{
    public abstract class BaseDebuff : IDebuff
    {
        public abstract string Name { get; }
        public abstract DebuffType Type { get; }

        public int RemainingTurns { get; protected set; }

        public bool IsExpired
        {
            get { return RemainingTurns <= 0; }
        }

        protected BaseDebuff(int duration)
        {
            RemainingTurns = duration;
        }

        public virtual void OnTurnStart(
            CombatEntity target,
            CombatContext context)
        {
        }

        public virtual void OnBeforeAction(CombatContext context)
        {
        }

        public void Tick()
        {
            RemainingTurns--;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/StatusEffects/PoisonDebuff.cs" -Content @'
using Game.Domain.Common;

namespace Game.Domain.Combat.StatusEffects
{
    public class PoisonDebuff : BaseDebuff
    {
        private readonly int damagePerTurn;

        public override string Name
        {
            get { return "Poison"; }
        }

        public override DebuffType Type
        {
            get { return DebuffType.DamageOverTime; }
        }

        public PoisonDebuff(
            int duration,
            int damagePerTurn)
            : base(duration)
        {
            this.damagePerTurn = damagePerTurn;
        }

        public override void OnTurnStart(
            CombatEntity target,
            CombatContext context)
        {
            target.ReceiveDamage(damagePerTurn);
            context.EventLog.Add(GameEventType.StatusTick, target.Name + " suffers " + damagePerTurn + " poison damage.");
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/StatusEffects/BurnDebuff.cs" -Content @'
using Game.Domain.Common;

namespace Game.Domain.Combat.StatusEffects
{
    public class BurnDebuff : BaseDebuff
    {
        private readonly int damagePerTurn;
        private readonly float attackMultiplier;

        public override string Name
        {
            get { return "Burn"; }
        }

        public override DebuffType Type
        {
            get { return DebuffType.DamageOverTime; }
        }

        public BurnDebuff(
            int duration,
            int damagePerTurn,
            float attackMultiplier)
            : base(duration)
        {
            this.damagePerTurn = damagePerTurn;
            this.attackMultiplier = attackMultiplier;
        }

        public override void OnTurnStart(
            CombatEntity target,
            CombatContext context)
        {
            target.ReceiveDamage(damagePerTurn);
            context.EventLog.Add(GameEventType.StatusTick, target.Name + " suffers " + damagePerTurn + " burn damage.");
        }

        public override void OnBeforeAction(CombatContext context)
        {
            context.BaseAttack = (int)(context.BaseAttack * attackMultiplier);
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/StatusEffects/BlindDebuff.cs" -Content @'
namespace Game.Domain.Combat.StatusEffects
{
    public class BlindDebuff : BaseDebuff
    {
        private readonly float hitPenalty;

        public override string Name
        {
            get { return "Blind"; }
        }

        public override DebuffType Type
        {
            get { return DebuffType.Accuracy; }
        }

        public BlindDebuff(
            int duration,
            float hitPenalty)
            : base(duration)
        {
            this.hitPenalty = hitPenalty;
        }

        public override void OnBeforeAction(CombatContext context)
        {
            context.HitChanceModifier -= hitPenalty;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/StatusEffects/SilenceDebuff.cs" -Content @'
using Game.Domain.Common;

namespace Game.Domain.Combat.StatusEffects
{
    public class SilenceDebuff : BaseDebuff
    {
        public override string Name
        {
            get { return "Silence"; }
        }

        public override DebuffType Type
        {
            get { return DebuffType.ActionBlock; }
        }

        public SilenceDebuff(int duration)
            : base(duration)
        {
        }

        public override void OnBeforeAction(CombatContext context)
        {
            context.EventLog.Add(GameEventType.ActionBlocked, context.Attacker.Name + " is silenced. Skill blocking hook is active.");
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/StatusEffects/ResourceBlockDebuff.cs" -Content @'
namespace Game.Domain.Combat.StatusEffects
{
    public class ResourceBlockDebuff : BaseDebuff
    {
        public override string Name
        {
            get { return "Resource Block"; }
        }

        public override DebuffType Type
        {
            get { return DebuffType.ResourceControl; }
        }

        public ResourceBlockDebuff(int duration)
            : base(duration)
        {
        }

        public override void OnBeforeAction(CombatContext context)
        {
            context.Attacker.Flags.ResourceRegenBlocked = true;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/StatusEffects/DebuffContainer.cs" -Content @'
using System.Collections.Generic;
using System.Linq;
using Game.Domain.Common;

namespace Game.Domain.Combat.StatusEffects
{
    public class DebuffContainer
    {
        private readonly List<IDebuff> active = new List<IDebuff>();

        public IReadOnlyList<IDebuff> Active
        {
            get { return active.AsReadOnly(); }
        }

        public void Add(
            IDebuff debuff,
            CombatEntity target,
            CombatContext context)
        {
            active.Add(debuff);
            context.EventLog.Add(GameEventType.DebuffApplied, target.Name + " receives " + debuff.Name + ".");
        }

        public void OnTurnStart(
            CombatEntity target,
            CombatContext context)
        {
            foreach (var debuff in active.ToList())
            {
                debuff.OnTurnStart(target, context);
            }
        }

        public void OnBeforeAction(CombatContext context)
        {
            foreach (var debuff in active.ToList())
            {
                debuff.OnBeforeAction(context);
            }
        }

        public void OnAfterAction(
            CombatEntity target,
            CombatContext context)
        {
            foreach (var debuff in active.ToList())
            {
                debuff.Tick();

                if (debuff.IsExpired)
                {
                    context.EventLog.Add(GameEventType.DebuffExpired, debuff.Name + " expires on " + target.Name + ".");
                    active.Remove(debuff);
                }
            }
        }

        public int RemoveOne(Game.Domain.Common.IRandomSource random)
        {
            if (active.Count == 0)
            {
                return 0;
            }

            active.RemoveAt(random.NextInt(0, active.Count));
            return 1;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/StatusEffects/IBuff.cs" -Content @'
namespace Game.Domain.Combat.StatusEffects
{
    public interface IBuff
    {
        string Name { get; }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/StatusEffects/BuffContainer.cs" -Content @'
namespace Game.Domain.Combat.StatusEffects
{
    public class BuffContainer
    {
        public void OnTurnStart(
            CombatEntity target,
            CombatContext context)
        {
        }

        public void OnBeforeAction(CombatContext context)
        {
        }

        public void OnAfterAction(
            CombatEntity target,
            CombatContext context)
        {
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Skills/SkillCost.cs" -Content @'
namespace Game.Domain.Skills
{
    public class SkillCost
    {
        public int BaseCost { get; private set; }
        public int CostPerLevel { get; private set; }

        public SkillCost(
            int baseCost,
            int costPerLevel)
        {
            BaseCost = baseCost;
            CostPerLevel = costPerLevel;
        }

        public int GetCost(int skillLevel)
        {
            return BaseCost + (CostPerLevel * skillLevel);
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Skills/SkillDefinition.cs" -Content @'
namespace Game.Domain.Skills
{
    public class SkillDefinition
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public int MaxLevel { get; private set; }
        public SkillCost Cost { get; private set; }

        public SkillDefinition(
            string id,
            string name,
            int maxLevel,
            SkillCost cost)
        {
            Id = id;
            Name = name;
            MaxLevel = maxLevel;
            Cost = cost;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Skills/SkillProgress.cs" -Content @'
namespace Game.Domain.Skills
{
    public class SkillProgress
    {
        public SkillDefinition Definition { get; private set; }
        public int Level { get; private set; }
        public int Experience { get; private set; }

        public bool IsMastered
        {
            get { return Level >= Definition.MaxLevel; }
        }

        public SkillProgress(SkillDefinition definition)
        {
            Definition = definition;
            Level = 1;
        }

        public void GainExperience(int amount)
        {
            if (IsMastered)
            {
                return;
            }

            Experience += amount;

            if (Experience >= 100)
            {
                Experience = 0;
                Level++;

                if (Level > Definition.MaxLevel)
                {
                    Level = Definition.MaxLevel;
                }
            }
        }

        public int GetBaseCost()
        {
            return Definition.Cost.GetCost(Level);
        }

        public float GetMasteryCostMultiplier()
        {
            return IsMastered ? 0.8f : 1f;
        }

        public float GetMasteryEffectBonus()
        {
            return IsMastered ? 0.15f : 0f;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Skills/ICombatSkill.cs" -Content @'
using Game.Domain.Combat;

namespace Game.Domain.Skills
{
    public interface ICombatSkill
    {
        string Id { get; }
        string Name { get; }

        void Apply(
            CombatContext context,
            SkillProgress progress);
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Skills/DeepSightSkill.cs" -Content @'
using Game.Domain.Common;
using Game.Domain.Combat;

namespace Game.Domain.Skills
{
    public class DeepSightSkill : ICombatSkill
    {
        public string Id
        {
            get { return "skill_deep_sight"; }
        }

        public string Name
        {
            get { return "Mirada Profunda"; }
        }

        public void Apply(
            CombatContext context,
            SkillProgress progress)
        {
            context.RevealOneEnemyTrait = true;
            context.EventLog.Add(GameEventType.SkillUsed, context.Attacker.Name + " uses Mirada Profunda.");
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Skills/BalanceRitualSkill.cs" -Content @'
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.StatusEffects;

namespace Game.Domain.Skills
{
    public class BalanceRitualSkill : ICombatSkill
    {
        public string Id
        {
            get { return "skill_balance_ritual"; }
        }

        public string Name
        {
            get { return "Ritual de Balance"; }
        }

        public void Apply(
            CombatContext context,
            SkillProgress progress)
        {
            context.DamageMultiplier += 0.15f + progress.GetMasteryEffectBonus();
            context.RequestDebuff(new BurnDebuff(2, 5, 0.9f), 0.45f);
            context.EventLog.Add(GameEventType.SkillUsed, context.Attacker.Name + " uses Ritual de Balance.");
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Skills/DestinyCorrectionSkill.cs" -Content @'
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.StatusEffects;

namespace Game.Domain.Skills
{
    public class DestinyCorrectionSkill : ICombatSkill
    {
        public string Id
        {
            get { return "skill_destiny_correction"; }
        }

        public string Name
        {
            get { return "Corrección del Destino"; }
        }

        public void Apply(
            CombatContext context,
            SkillProgress progress)
        {
            context.DamageMultiplier += 0.05f;
            context.RequestDebuff(new PoisonDebuff(3, 4), 0.65f + progress.GetMasteryEffectBonus());
            context.EventLog.Add(GameEventType.SkillUsed, context.Attacker.Name + " uses Corrección del Destino.");
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Skills/MinorCleanseSkill.cs" -Content @'
using Game.Domain.Common;
using Game.Domain.Combat;

namespace Game.Domain.Skills
{
    public class MinorCleanseSkill : ICombatSkill
    {
        public string Id
        {
            get { return "skill_minor_cleanse"; }
        }

        public string Name
        {
            get { return "Refutación Menor"; }
        }

        public void Apply(
            CombatContext context,
            SkillProgress progress)
        {
            context.RequestCleanse(CleanseType.Partial);
            context.EventLog.Add(GameEventType.SkillUsed, context.Attacker.Name + " uses Refutación Menor.");
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/Passives/ICombatPassiveEffect.cs" -Content @'
using Game.Domain.Combat.Resolvers;

namespace Game.Domain.Combat.Passives
{
    public interface ICombatPassiveEffect
    {
        void OnCombatStart(CombatContext context);
        void OnBeforeSkillCostCalculated(CombatContext context);
        void OnBeforeDebuffApplied(DebuffApplicationContext context);
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/Passives/CombatPassivePipeline.cs" -Content @'
using System.Collections.Generic;
using Game.Domain.Combat.Resolvers;

namespace Game.Domain.Combat.Passives
{
    public class CombatPassivePipeline
    {
        private readonly IReadOnlyList<ICombatPassiveEffect> passives;

        public CombatPassivePipeline(IReadOnlyList<ICombatPassiveEffect> passives)
        {
            this.passives = passives;
        }

        public void OnCombatStart(CombatContext context)
        {
            foreach (var passive in passives)
            {
                passive.OnCombatStart(context);
            }
        }

        public void OnBeforeSkillCostCalculated(CombatContext context)
        {
            foreach (var passive in passives)
            {
                passive.OnBeforeSkillCostCalculated(context);
            }
        }

        public void OnBeforeDebuffApplied(DebuffApplicationContext context)
        {
            foreach (var passive in passives)
            {
                passive.OnBeforeDebuffApplied(context);
            }
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/Passives/TrackerInstinctPassiveEffect.cs" -Content @'
using Game.Domain.Common;
using Game.Domain.Combat.Resolvers;

namespace Game.Domain.Combat.Passives
{
    public class TrackerInstinctPassiveEffect : ICombatPassiveEffect
    {
        public void OnCombatStart(CombatContext context)
        {
            context.RevealOneEnemyTrait = true;
            context.EventLog.Add(GameEventType.PassiveTriggered, "Tracker Instinct reveals one enemy trait.");
        }

        public void OnBeforeSkillCostCalculated(CombatContext context)
        {
        }

        public void OnBeforeDebuffApplied(DebuffApplicationContext context)
        {
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/Passives/TechnicalResonancePassiveEffect.cs" -Content @'
using Game.Domain.Common;
using Game.Domain.Combat.Resolvers;

namespace Game.Domain.Combat.Passives
{
    public class TechnicalResonancePassiveEffect : ICombatPassiveEffect
    {
        public void OnCombatStart(CombatContext context)
        {
        }

        public void OnBeforeSkillCostCalculated(CombatContext context)
        {
            if (context.IsFirstSkillOfCombat)
            {
                context.ResourceCostMultiplier *= 0.9f;
                context.EventLog.Add(GameEventType.PassiveTriggered, "Technical Resonance reduces first skill cost.");
            }
        }

        public void OnBeforeDebuffApplied(DebuffApplicationContext context)
        {
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/Resolvers/DebuffApplicationContext.cs" -Content @'
namespace Game.Domain.Combat.Resolvers
{
    public class DebuffApplicationContext
    {
        public CombatEntity Source { get; private set; }
        public CombatEntity Target { get; private set; }
        public float BaseChance { get; set; }
        public float BonusChance { get; set; }

        public float FinalChance
        {
            get { return BaseChance + BonusChance; }
        }

        public DebuffApplicationContext(
            CombatEntity source,
            CombatEntity target,
            float baseChance)
        {
            Source = source;
            Target = target;
            BaseChance = baseChance;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/Resolvers/DamageResult.cs" -Content @'
namespace Game.Domain.Combat.Resolvers
{
    public class DamageResult
    {
        public bool Hit { get; private set; }
        public bool Critical { get; private set; }
        public int Damage { get; private set; }

        public DamageResult(
            bool hit,
            bool critical,
            int damage)
        {
            Hit = hit;
            Critical = critical;
            Damage = damage;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/Resolvers/DamageResolver.cs" -Content @'
using System;
using Game.Domain.Common;

namespace Game.Domain.Combat.Resolvers
{
    public class DamageResolver
    {
        private readonly IRandomSource random;

        public DamageResolver(IRandomSource random)
        {
            this.random = random;
        }

        public DamageResult Resolve(CombatContext context)
        {
            float hitChance = ProbabilityCalculator.CalculateChance(
                context.Attacker.Stats.Precision,
                context.Defender.Stats.Evasion,
                0.75f + context.HitChanceModifier,
                0.005f,
                0.1f,
                0.95f);

            bool hit = random.NextDouble() <= hitChance;

            if (!hit)
            {
                return new DamageResult(false, false, 0);
            }

            int attack = Math.Max(1, context.BaseAttack + context.FlatDamageBonus);
            int defense = Math.Max(0, context.BaseDefense);
            int damage = Math.Max(1, (int)(attack * ((float)attack / (attack + defense))));
            damage = Math.Max(1, (int)(damage * context.DamageMultiplier));

            float critChance = ProbabilityCalculator.CalculateChance(
                context.Attacker.Stats.CriticalChance,
                context.Defender.Stats.CriticalResistance,
                0.10f + context.CriticalChanceModifier,
                0.003f,
                0.02f,
                0.65f);

            bool critical = random.NextDouble() <= critChance;

            if (critical)
            {
                damage = Math.Max(1, (int)(damage * context.Attacker.Stats.CriticalDamageMultiplier));
            }

            return new DamageResult(true, critical, damage);
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/Resolvers/DebuffResolver.cs" -Content @'
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
'@

Write-RepoFile -Path "src/Game/Domain/Combat/TurnRequest.cs" -Content @'
using Game.Domain.Skills;

namespace Game.Domain.Combat
{
    public class TurnRequest
    {
        public CombatEntity Attacker { get; private set; }
        public CombatEntity Defender { get; private set; }
        public TurnActionType ActionType { get; private set; }
        public ICombatSkill Skill { get; private set; }
        public SkillProgress SkillProgress { get; private set; }

        private TurnRequest(
            CombatEntity attacker,
            CombatEntity defender,
            TurnActionType actionType,
            ICombatSkill skill,
            SkillProgress progress)
        {
            Attacker = attacker;
            Defender = defender;
            ActionType = actionType;
            Skill = skill;
            SkillProgress = progress;
        }

        public static TurnRequest BasicAttack(
            CombatEntity attacker,
            CombatEntity defender)
        {
            return new TurnRequest(attacker, defender, TurnActionType.BasicAttack, null, null);
        }

        public static TurnRequest SkillAction(
            CombatEntity attacker,
            CombatEntity defender,
            ICombatSkill skill,
            SkillProgress progress)
        {
            return new TurnRequest(attacker, defender, TurnActionType.Skill, skill, progress);
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/TurnBasedCombatEngine.cs" -Content @'
using Game.Domain.Common;
using Game.Domain.Combat.Passives;
using Game.Domain.Combat.Resolvers;

namespace Game.Domain.Combat
{
    public class TurnBasedCombatEngine
    {
        private readonly IRandomSource random;
        private readonly CombatPassivePipeline pipeline;
        private readonly DamageResolver damageResolver;
        private readonly DebuffResolver debuffResolver;

        public TurnBasedCombatEngine(
            IRandomSource random,
            CombatPassivePipeline pipeline)
        {
            this.random = random;
            this.pipeline = pipeline;
            damageResolver = new DamageResolver(random);
            debuffResolver = new DebuffResolver(random, pipeline);
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
                runtime.EventLog.Add(GameEventType.CognitionTriggered, "Atlas teaching Fórmula Estable reduces skill cost.");
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
                context.ResourceCost = request.SkillProgress.GetBaseCost();
                context.ResourceCostMultiplier *= request.SkillProgress.GetMasteryCostMultiplier();
                pipeline.OnBeforeSkillCostCalculated(context);

                int finalCost = (int)System.Math.Ceiling(context.ResourceCost * context.ResourceCostMultiplier);

                if (!request.Attacker.Resource.TrySpend(finalCost))
                {
                    runtime.EventLog.Add(GameEventType.ResourceNotEnough, request.Attacker.Name + " lacks Essence.");
                    EndTurn(runtime, context);
                    return;
                }

                runtime.EventLog.Add(GameEventType.ResourceSpent, request.Attacker.Name + " spends " + finalCost + " Essence.");
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
            runtime.EventLog.Add(GameEventType.TurnEnded, "Turn " + runtime.TurnNumber + " ends.");
            runtime.AdvanceTurn();
        }
    }
}
'@

Write-Host ""
Write-Host "Done." -ForegroundColor Green
