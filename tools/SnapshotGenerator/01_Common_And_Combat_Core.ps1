
#requires -version 5.1
<#
01 Common And Combat Core
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
Write-Host "=== 01 Common And Combat Core ===" -ForegroundColor Cyan
Write-Host "Target: $Root"
Write-Host ""

Write-RepoFile -Path "src/Game/Domain/Common/GameEventType.cs" -Content @'
namespace Game.Domain.Common
{
    public enum GameEventType
    {
        CombatStarted,
        TurnStarted,
        TurnEnded,
        SkillUsed,
        BasicAttackUsed,
        DamageDealt,
        DamageMissed,
        CriticalHit,
        ResourceSpent,
        ResourceRegenerated,
        ResourceNotEnough,
        BuffApplied,
        BuffExpired,
        DebuffApplied,
        DebuffResisted,
        DebuffExpired,
        CleanseResolved,
        PassiveTriggered,
        FeatureTriggered,
        TraitRevealed,
        StatusTick,
        ActionBlocked,
        CombatEnded,
        CognitionTriggered,
        TeachingFixed,
        TeachingEquipped,
        MissionStarted,
        MissionCompleted,
        MissionFailed,
        RewardGranted,
        CodexEntryDiscovered,
        CodexEntryCompleted,
        BattleRewardGranted,
        AiDecisionMade,
        ObjectiveProgressed,
        PersistenceSaved,
        PersistenceLoaded
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Common/GameEvent.cs" -Content @'
namespace Game.Domain.Common
{
    public class GameEvent
    {
        public GameEventType Type { get; private set; }
        public string Message { get; private set; }

        public GameEvent(GameEventType type, string message)
        {
            Type = type;
            Message = message;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Common/GameEventLog.cs" -Content @'
using System.Collections.Generic;

namespace Game.Domain.Common
{
    public class GameEventLog
    {
        private readonly List<GameEvent> events = new List<GameEvent>();

        public IReadOnlyList<GameEvent> Events
        {
            get { return events.AsReadOnly(); }
        }

        public void Add(GameEventType type, string message)
        {
            events.Add(new GameEvent(type, message));
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Common/IRandomSource.cs" -Content @'
namespace Game.Domain.Common
{
    public interface IRandomSource
    {
        double NextDouble();

        int NextInt(
            int minInclusive,
            int maxExclusive);
    }
}
'@

Write-RepoFile -Path "src/Game/Infrastructure/Random/SeededRandomSource.cs" -Content @'
using Game.Domain.Common;

namespace Game.Infrastructure.Random
{
    public class SeededRandomSource : IRandomSource
    {
        private readonly System.Random random;

        public SeededRandomSource(int seed)
        {
            random = new System.Random(seed);
        }

        public double NextDouble()
        {
            return random.NextDouble();
        }

        public int NextInt(
            int minInclusive,
            int maxExclusive)
        {
            return random.Next(minInclusive, maxExclusive);
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/Stats/CombatStats.cs" -Content @'
namespace Game.Domain.Combat.Stats
{
    public class CombatStats
    {
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }

        public int Attack { get; set; }
        public int Defense { get; set; }

        public int Precision { get; set; }
        public int Evasion { get; set; }

        public int CriticalChance { get; set; }
        public int CriticalResistance { get; set; }
        public float CriticalDamageMultiplier { get; set; }

        public int DebuffChance { get; set; }
        public int DebuffResistance { get; set; }

        public int BuffPotency { get; set; }
        public int BuffResistance { get; set; }

        public int ControlResistance { get; set; }

        public CombatStats Clone()
        {
            return (CombatStats)MemberwiseClone();
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/Resources/ResourcePool.cs" -Content @'
using System;

namespace Game.Domain.Combat.Resources
{
    public class ResourcePool
    {
        public int Max { get; private set; }
        public int Current { get; private set; }

        public ResourcePool(int max)
        {
            Max = Math.Max(0, max);
            Current = Max;
        }

        public bool TrySpend(int amount)
        {
            if (amount > Current)
            {
                return false;
            }

            Current -= amount;
            return true;
        }

        public void Regenerate(int amount)
        {
            if (amount > 0)
            {
                Current = Math.Min(Max, Current + amount);
            }
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/Resources/ResourceRegenProfile.cs" -Content @'
namespace Game.Domain.Combat.Resources
{
    public class ResourceRegenProfile
    {
        public int PassiveRegenPerTurn { get; private set; }
        public int BasicAttackRegen { get; private set; }
        public int DefendRegen { get; private set; }
        public int WaitRegen { get; private set; }

        public ResourceRegenProfile(
            int passiveRegenPerTurn,
            int basicAttackRegen,
            int defendRegen,
            int waitRegen)
        {
            PassiveRegenPerTurn = passiveRegenPerTurn;
            BasicAttackRegen = basicAttackRegen;
            DefendRegen = defendRegen;
            WaitRegen = waitRegen;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/CombatEntityFlags.cs" -Content @'
namespace Game.Domain.Combat
{
    public class CombatEntityFlags
    {
        public bool FirstDebuffAttemptThisCombat { get; set; }
        public bool ResourceRegenBlocked { get; set; }

        public CombatEntityFlags()
        {
            FirstDebuffAttemptThisCombat = true;
        }

        public void ResetTurnFlags()
        {
            ResourceRegenBlocked = false;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/CombatEntity.cs" -Content @'
using System;
using Game.Domain.Combat.Resources;
using Game.Domain.Combat.Stats;
using Game.Domain.Combat.StatusEffects;

namespace Game.Domain.Combat
{
    public class CombatEntity
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public CombatStats Stats { get; private set; }
        public ResourcePool Resource { get; private set; }
        public DebuffContainer Debuffs { get; private set; }
        public BuffContainer Buffs { get; private set; }
        public CombatEntityFlags Flags { get; private set; }

        public bool IsAlive
        {
            get { return Stats.CurrentHealth > 0; }
        }

        public CombatEntity(
            string id,
            string name,
            CombatStats stats,
            int maxResource)
        {
            Id = id;
            Name = name;
            Stats = stats;
            Resource = new ResourcePool(maxResource);
            Debuffs = new DebuffContainer();
            Buffs = new BuffContainer();
            Flags = new CombatEntityFlags();

            if (Stats.CurrentHealth <= 0)
            {
                Stats.CurrentHealth = Stats.MaxHealth;
            }
        }

        public void ReceiveDamage(int amount)
        {
            if (amount > 0)
            {
                Stats.CurrentHealth = Math.Max(0, Stats.CurrentHealth - amount);
            }
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/TurnActionType.cs" -Content @'
namespace Game.Domain.Combat
{
    public enum TurnActionType
    {
        BasicAttack,
        Skill,
        Defend,
        Wait
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/CleanseType.cs" -Content @'
namespace Game.Domain.Combat
{
    public enum CleanseType
    {
        Partial,
        ByType,
        Full
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/ProbabilityCalculator.cs" -Content @'
using System;
using Game.Domain.Common;

namespace Game.Domain.Combat
{
    public static class ProbabilityCalculator
    {
        public static float CalculateChance(
            int attackerStat,
            int defenderStat,
            float baseChance,
            float factor,
            float min,
            float max)
        {
            return Math.Max(min, Math.Min(max, baseChance + ((attackerStat - defenderStat) * factor)));
        }

        public static bool Roll(
            IRandomSource random,
            int attackerStat,
            int defenderStat,
            float baseChance,
            float factor,
            float min,
            float max)
        {
            return random.NextDouble() <= CalculateChance(
                attackerStat,
                defenderStat,
                baseChance,
                factor,
                min,
                max);
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/CombatContext.cs" -Content @'
using System.Collections.Generic;
using Game.Domain.Common;
using Game.Domain.Combat.StatusEffects;

namespace Game.Domain.Combat
{
    public class CombatContext
    {
        public CombatEntity Attacker { get; private set; }
        public CombatEntity Defender { get; private set; }
        public GameEventLog EventLog { get; private set; }

        public int BaseAttack { get; set; }
        public int BaseDefense { get; set; }
        public float DamageMultiplier { get; set; }
        public int FlatDamageBonus { get; set; }

        public int ResourceCost { get; set; }
        public float ResourceCostMultiplier { get; set; }

        public float HitChanceModifier { get; set; }
        public float CriticalChanceModifier { get; set; }

        public bool RevealOneEnemyTrait { get; set; }
        public bool IsFirstSkillOfCombat { get; set; }

        public CleanseType? RequestedCleanse { get; set; }

        public List<StatusApplicationRequest> DebuffsToApply { get; private set; }

        public CombatContext(
            CombatEntity attacker,
            CombatEntity defender,
            GameEventLog eventLog)
        {
            Attacker = attacker;
            Defender = defender;
            EventLog = eventLog;
            BaseAttack = attacker.Stats.Attack;
            BaseDefense = defender.Stats.Defense;
            DamageMultiplier = 1f;
            ResourceCostMultiplier = 1f;
            DebuffsToApply = new List<StatusApplicationRequest>();
        }

        public void RequestDebuff(
            IDebuff debuff,
            float baseChance)
        {
            DebuffsToApply.Add(new StatusApplicationRequest(debuff, baseChance));
        }

        public void RequestCleanse(CleanseType cleanseType)
        {
            RequestedCleanse = cleanseType;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/StatusApplicationRequest.cs" -Content @'
using Game.Domain.Combat.StatusEffects;

namespace Game.Domain.Combat
{
    public class StatusApplicationRequest
    {
        public IDebuff Debuff { get; private set; }
        public float BaseChance { get; private set; }

        public StatusApplicationRequest(
            IDebuff debuff,
            float baseChance)
        {
            Debuff = debuff;
            BaseChance = baseChance;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/CombatRuntimeContext.cs" -Content @'
using Game.Domain.Common;
using Game.Domain.Player;

namespace Game.Domain.Combat
{
    public class CombatRuntimeContext
    {
        public CombatEntity Player { get; private set; }
        public CombatEntity Enemy { get; private set; }
        public GameEventLog EventLog { get; private set; }
        public PlayerProfile PlayerProfile { get; private set; }
        public int TurnNumber { get; private set; }
        public bool CombatEnded { get; private set; }

        public CombatRuntimeContext(
            CombatEntity player,
            CombatEntity enemy,
            GameEventLog eventLog,
            PlayerProfile playerProfile)
        {
            Player = player;
            Enemy = enemy;
            EventLog = eventLog;
            PlayerProfile = playerProfile;
            TurnNumber = 1;
        }

        public void AdvanceTurn()
        {
            TurnNumber++;
        }

        public void EndCombat()
        {
            CombatEnded = true;
        }
    }
}
'@

Write-Host ""
Write-Host "Done." -ForegroundColor Green
