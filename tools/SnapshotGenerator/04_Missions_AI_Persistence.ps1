
#requires -version 5.1
<#
04 Missions AI Persistence
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
Write-Host "=== 04 Missions AI Persistence ===" -ForegroundColor Cyan
Write-Host "Target: $Root"
Write-Host ""

Write-RepoFile -Path "src/Game/Domain/Missions/MissionType.cs" -Content @'
namespace Game.Domain.Missions
{
    public enum MissionType
    {
        MainStory,
        SideQuest,
        MasterTraining,
        CodexChallenge,
        Bounty,
        Exploration
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/MissionOutcome.cs" -Content @'
namespace Game.Domain.Missions
{
    public enum MissionOutcome
    {
        Incomplete,
        Success,
        FailedButRetryable,
        FailedPermanent
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/MissionActionType.cs" -Content @'
namespace Game.Domain.Missions
{
    public enum MissionActionType
    {
        Analyze,
        ApplyDebuff,
        Cleanse,
        ChooseStableAction,
        ChooseHighVarianceAction,
        ChooseRandomAction,
        DefeatEnemy,
        CollectItem,
        TalkToNpc,
        ReachLocation,
        UseSkill,
        Finish
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/MissionRewardType.cs" -Content @'
namespace Game.Domain.Missions
{
    public enum MissionRewardType
    {
        FixTeaching,
        UnlockSkill,
        UnlockClass,
        GrantItem,
        GrantWeapon,
        GrantArmor,
        GrantAccessory,
        GrantCurrency,
        GrantExperience,
        UnlockCodexEntry,
        UnlockFeature
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/MissionObjectiveType.cs" -Content @'
namespace Game.Domain.Missions
{
    public enum MissionObjectiveType
    {
        PerformAction,
        DefeatEnemy,
        CollectItem,
        ReachLocation,
        TalkToNpc
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/MissionRewardDefinition.cs" -Content @'
namespace Game.Domain.Missions
{
    public class MissionRewardDefinition
    {
        public MissionRewardType Type { get; private set; }
        public string Value { get; private set; }
        public int Amount { get; private set; }

        public MissionRewardDefinition(
            MissionRewardType type,
            string value,
            int amount = 1)
        {
            Type = type;
            Value = value;
            Amount = amount;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/MissionObjectiveDefinition.cs" -Content @'
namespace Game.Domain.Missions
{
    public class MissionObjectiveDefinition
    {
        public string Id { get; private set; }
        public MissionObjectiveType Type { get; private set; }
        public string TargetValue { get; private set; }
        public int RequiredAmount { get; private set; }

        public MissionObjectiveDefinition(
            string id,
            MissionObjectiveType type,
            string targetValue,
            int requiredAmount)
        {
            Id = id;
            Type = type;
            TargetValue = targetValue;
            RequiredAmount = requiredAmount;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/MissionDefinition.cs" -Content @'
using System.Collections.Generic;

namespace Game.Domain.Missions
{
    public class MissionDefinition
    {
        public string Id { get; private set; }
        public MissionType Type { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public IReadOnlyList<MissionObjectiveDefinition> Objectives { get; private set; }
        public IReadOnlyList<MissionRewardDefinition> Rewards { get; private set; }

        public MissionDefinition(
            string id,
            MissionType type,
            string name,
            string description,
            IReadOnlyList<MissionObjectiveDefinition> objectives,
            IReadOnlyList<MissionRewardDefinition> rewards)
        {
            Id = id;
            Type = type;
            Name = name;
            Description = description;
            Objectives = objectives;
            Rewards = rewards;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/MissionAttempt.cs" -Content @'
using System.Collections.Generic;

namespace Game.Domain.Missions
{
    public class MissionAttempt
    {
        private readonly List<MissionActionType> actions = new List<MissionActionType>();

        public IReadOnlyList<MissionActionType> Actions
        {
            get { return actions.AsReadOnly(); }
        }

        public void Record(MissionActionType action)
        {
            actions.Add(action);
        }

        public bool Contains(MissionActionType action)
        {
            return actions.Contains(action);
        }

        public int IndexOf(MissionActionType action)
        {
            return actions.IndexOf(action);
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/IMissionEvaluator.cs" -Content @'
using Game.Domain.Player;

namespace Game.Domain.Missions
{
    public interface IMissionEvaluator
    {
        string MissionId { get; }

        MissionOutcome Evaluate(
            PlayerProfile profile,
            MissionAttempt attempt);
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/LyraJudgementCrackEvaluator.cs" -Content @'
using Game.Domain.Player;

namespace Game.Domain.Missions
{
    public class LyraJudgementCrackEvaluator : IMissionEvaluator
    {
        public const string Id = "mission_lyra_judgement_crack";

        public string MissionId
        {
            get { return Id; }
        }

        public MissionOutcome Evaluate(
            PlayerProfile profile,
            MissionAttempt attempt)
        {
            int analyze = attempt.IndexOf(MissionActionType.Analyze);
            int debuff = attempt.IndexOf(MissionActionType.ApplyDebuff);

            if (analyze < 0)
            {
                return MissionOutcome.FailedButRetryable;
            }

            if (debuff >= 0 && debuff < analyze)
            {
                return MissionOutcome.FailedButRetryable;
            }

            return MissionOutcome.Success;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/AtlasThreeCertaintiesEvaluator.cs" -Content @'
using Game.Domain.Player;

namespace Game.Domain.Missions
{
    public class AtlasThreeCertaintiesEvaluator : IMissionEvaluator
    {
        public const string Id = "mission_atlas_three_certainties";

        public string MissionId
        {
            get { return Id; }
        }

        public MissionOutcome Evaluate(
            PlayerProfile profile,
            MissionAttempt attempt)
        {
            return attempt.Contains(MissionActionType.ChooseStableAction)
                ? MissionOutcome.Success
                : MissionOutcome.FailedButRetryable;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/FirstRelicSideQuestEvaluator.cs" -Content @'
using Game.Domain.Player;

namespace Game.Domain.Missions
{
    public class FirstRelicSideQuestEvaluator : IMissionEvaluator
    {
        public const string Id = "sidequest_first_relic";

        public string MissionId
        {
            get { return Id; }
        }

        public MissionOutcome Evaluate(
            PlayerProfile profile,
            MissionAttempt attempt)
        {
            return attempt.Contains(MissionActionType.CollectItem)
                ? MissionOutcome.Success
                : MissionOutcome.FailedButRetryable;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/MissionDefinitionRegistry.cs" -Content @'
using System.Collections.Generic;

namespace Game.Domain.Missions
{
    public class MissionDefinitionRegistry
    {
        private readonly Dictionary<string, MissionDefinition> definitions =
            new Dictionary<string, MissionDefinition>();

        public void Register(MissionDefinition definition)
        {
            definitions[definition.Id] = definition;
        }

        public MissionDefinition Get(string missionId)
        {
            return definitions[missionId];
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/MissionEvaluatorRegistry.cs" -Content @'
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
'@

Write-RepoFile -Path "src/Game/Domain/Missions/MissionBootstrap.cs" -Content @'
namespace Game.Domain.Missions
{
    public static class MissionBootstrap
    {
        public static MissionDefinitionRegistry BuildDefinitionRegistry()
        {
            var registry = new MissionDefinitionRegistry();

            registry.Register(new MissionDefinition(
                LyraJudgementCrackEvaluator.Id,
                MissionType.MasterTraining,
                "La Grieta del Juicio",
                "Descubre la contradicción real antes de intentar controlar al enemigo.",
                new[]
                {
                    new MissionObjectiveDefinition(
                        "objective_analyze_before_debuff",
                        MissionObjectiveType.PerformAction,
                        "AnalyzeBeforeDebuff",
                        1)
                },
                new[]
                {
                    new MissionRewardDefinition(MissionRewardType.FixTeaching, "lyra_detect_weakness")
                }));

            registry.Register(new MissionDefinition(
                AtlasThreeCertaintiesEvaluator.Id,
                MissionType.MasterTraining,
                "El Problema de las Tres Certezas",
                "Elige la opción estable, no la más vistosa ni la más caótica.",
                new[]
                {
                    new MissionObjectiveDefinition(
                        "objective_choose_stable_action",
                        MissionObjectiveType.PerformAction,
                        "ChooseStableAction",
                        1)
                },
                new[]
                {
                    new MissionRewardDefinition(MissionRewardType.FixTeaching, "atlas_stable_cast")
                }));

            registry.Register(new MissionDefinition(
                FirstRelicSideQuestEvaluator.Id,
                MissionType.SideQuest,
                "El Relicario Partido",
                "Recupera una reliquia dañada y descubre su primera propiedad.",
                new[]
                {
                    new MissionObjectiveDefinition(
                        "objective_collect_broken_relic",
                        MissionObjectiveType.CollectItem,
                        "item_broken_reliquary",
                        1)
                },
                new[]
                {
                    new MissionRewardDefinition(MissionRewardType.GrantAccessory, "item_broken_reliquary"),
                    new MissionRewardDefinition(MissionRewardType.GrantCurrency, "gold", 100),
                    new MissionRewardDefinition(MissionRewardType.UnlockCodexEntry, "item_broken_reliquary")
                }));

            return registry;
        }

        public static MissionEvaluatorRegistry BuildEvaluatorRegistry()
        {
            var registry = new MissionEvaluatorRegistry();

            registry.Register(new LyraJudgementCrackEvaluator());
            registry.Register(new AtlasThreeCertaintiesEvaluator());
            registry.Register(new FirstRelicSideQuestEvaluator());

            return registry;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/MissionRewardApplicationService.cs" -Content @'
using Game.Domain.Player;

namespace Game.Domain.Missions
{
    public class MissionRewardApplicationService
    {
        public void ApplyReward(
            PlayerProfile profile,
            MissionRewardDefinition reward)
        {
            switch (reward.Type)
            {
                case MissionRewardType.FixTeaching:
                    profile.Cognition.FixTeaching(reward.Value);
                    break;

                case MissionRewardType.UnlockSkill:
                    profile.SkillBook.UnlockSkill(reward.Value);
                    break;

                case MissionRewardType.UnlockClass:
                    profile.ClassCollection.UnlockClass(reward.Value);
                    break;

                case MissionRewardType.GrantItem:
                case MissionRewardType.GrantWeapon:
                case MissionRewardType.GrantArmor:
                case MissionRewardType.GrantAccessory:
                    profile.Inventory.AddItem(reward.Value, reward.Amount);
                    break;

                case MissionRewardType.GrantCurrency:
                    profile.Wallet.AddCurrency(reward.Value, reward.Amount);
                    break;

                case MissionRewardType.UnlockCodexEntry:
                    profile.Codex.DiscoverEntry(reward.Value);
                    break;
            }
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/MissionCompletionService.cs" -Content @'
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
'@

Write-RepoFile -Path "src/Game/Domain/Combat/AI/IEnemyAiStrategy.cs" -Content @'
namespace Game.Domain.Combat.AI
{
    public interface IEnemyAiStrategy
    {
        TurnRequest ChooseAction(CombatRuntimeContext runtime);
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/AI/BasicAttackEnemyAiStrategy.cs" -Content @'
using Game.Domain.Common;

namespace Game.Domain.Combat.AI
{
    public class BasicAttackEnemyAiStrategy : IEnemyAiStrategy
    {
        public TurnRequest ChooseAction(CombatRuntimeContext runtime)
        {
            runtime.EventLog.Add(GameEventType.AiDecisionMade, runtime.Enemy.Name + " chooses Basic Attack.");
            return TurnRequest.BasicAttack(runtime.Enemy, runtime.Player);
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/Outcomes/BattleWinner.cs" -Content @'
namespace Game.Domain.Combat.Outcomes
{
    public enum BattleWinner
    {
        None,
        Player,
        Enemy
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/Outcomes/BattleOutcome.cs" -Content @'
namespace Game.Domain.Combat.Outcomes
{
    public class BattleOutcome
    {
        public BattleWinner Winner { get; private set; }
        public int TurnCount { get; private set; }

        public BattleOutcome(
            BattleWinner winner,
            int turnCount)
        {
            Winner = winner;
            TurnCount = turnCount;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/Outcomes/BattleOutcomeResolver.cs" -Content @'
namespace Game.Domain.Combat.Outcomes
{
    public class BattleOutcomeResolver
    {
        public BattleOutcome Resolve(CombatRuntimeContext runtime)
        {
            if (!runtime.Player.IsAlive)
            {
                return new BattleOutcome(BattleWinner.Enemy, runtime.TurnNumber);
            }

            if (!runtime.Enemy.IsAlive)
            {
                return new BattleOutcome(BattleWinner.Player, runtime.TurnNumber);
            }

            return new BattleOutcome(BattleWinner.None, runtime.TurnNumber);
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/Outcomes/BattleRewardDefinition.cs" -Content @'
namespace Game.Domain.Combat.Outcomes
{
    public class BattleRewardDefinition
    {
        public string CurrencyId { get; private set; }
        public int CurrencyAmount { get; private set; }
        public int Experience { get; private set; }

        public BattleRewardDefinition(
            string currencyId,
            int currencyAmount,
            int experience)
        {
            CurrencyId = currencyId;
            CurrencyAmount = currencyAmount;
            Experience = experience;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Combat/Outcomes/BattleRewardResolver.cs" -Content @'
using Game.Domain.Common;
using Game.Domain.Player;

namespace Game.Domain.Combat.Outcomes
{
    public class BattleRewardResolver
    {
        public void ApplyRewards(
            PlayerProfile profile,
            BattleOutcome outcome,
            BattleRewardDefinition reward,
            GameEventLog log)
        {
            if (outcome.Winner != BattleWinner.Player)
            {
                return;
            }

            profile.Wallet.AddCurrency(reward.CurrencyId, reward.CurrencyAmount);
            log.Add(GameEventType.BattleRewardGranted, "Battle reward granted: " + reward.CurrencyId + " x" + reward.CurrencyAmount + ".");
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Application/Integration/CognitionCombatPassiveResolver.cs" -Content @'
using System.Collections.Generic;
using Game.Domain.Combat.Passives;
using Game.Domain.Player;

namespace Game.Application.Integration
{
    public class CognitionCombatPassiveResolver
    {
        public IReadOnlyList<ICombatPassiveEffect> Resolve(PlayerProfile profile)
        {
            var passives = new List<ICombatPassiveEffect>();

            if (profile.Cognition.HasActiveTeaching("lyra_detect_weakness"))
            {
                passives.Add(new TrackerInstinctPassiveEffect());
            }

            if (profile.Cognition.HasActiveTeaching("atlas_stable_cast"))
            {
                passives.Add(new TechnicalResonancePassiveEffect());
            }

            return passives;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Application/Persistence/ISaveRepository.cs" -Content @'
namespace Game.Application.Persistence
{
    public interface ISaveRepository<T>
    {
        void Save(
            string slotId,
            T data);

        T Load(string slotId);

        bool Exists(string slotId);
    }
}
'@

Write-RepoFile -Path "src/Game/Application/Persistence/PlayerSaveData.cs" -Content @'
using System.Collections.Generic;

namespace Game.Application.Persistence
{
    public class PlayerSaveData
    {
        public string Version { get; set; }
        public string ActiveArchetype { get; set; }
        public List<string> FixedTeachings { get; set; }
        public List<string> EquippedTeachings { get; set; }
        public Dictionary<string, int> Items { get; set; }
        public Dictionary<string, int> Currencies { get; set; }

        public PlayerSaveData()
        {
            FixedTeachings = new List<string>();
            EquippedTeachings = new List<string>();
            Items = new Dictionary<string, int>();
            Currencies = new Dictionary<string, int>();
        }
    }
}
'@

Write-Host ""
Write-Host "Done." -ForegroundColor Green
