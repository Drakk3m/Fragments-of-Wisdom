#requires -version 5.1
<#
06 Iteration 2 Nahual Path
PowerShell 5.1 compatible.

This script applies the Vertical Slice Iteration 2 increment:
- lightweight alignment branch
- Nahual discovery/unlock event
- Wisdom Fragments placeholder resource
- forbidden transmutation prototype
- lightweight NPC/vendor reactions
- minimal branching mission
- codex/documentation updates
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
Write-Host "=== 06 Iteration 2 Nahual Path ===" -ForegroundColor Cyan
Write-Host "Target: $Root"
Write-Host ""

Write-RepoFile -Path "src/Game/Domain/Alignment/KnowledgeAlignment.cs" -Content @'
namespace Game.Domain.Alignment
{
    public enum KnowledgeAlignment
    {
        SharedWisdom,
        NahualPath
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Alignment/AlignmentProgression.cs" -Content @'
using System.Collections.Generic;

namespace Game.Domain.Alignment
{
    public class AlignmentProgression
    {
        private readonly Dictionary<KnowledgeAlignment, int> points = new Dictionary<KnowledgeAlignment, int>();

        public AlignmentProgression()
        {
            points[KnowledgeAlignment.SharedWisdom] = 0;
            points[KnowledgeAlignment.NahualPath] = 0;
        }

        public IReadOnlyDictionary<KnowledgeAlignment, int> Points
        {
            get { return points; }
        }

        public void AddProgress(
            KnowledgeAlignment alignment,
            int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            points[alignment] = points[alignment] + amount;
        }

        public int GetProgress(KnowledgeAlignment alignment)
        {
            return points[alignment];
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Alignment/AlignmentFlag.cs" -Content @'
namespace Game.Domain.Alignment
{
    public static class AlignmentFlag
    {
        public const string DiscoveredNahualPath = "alignment.nahual.discovered";
        public const string AcceptedFirstNahualTeaching = "alignment.nahual.accepted_first_teaching";
        public const string MentorDistanceStarted = "world.mentor.distance_started";
        public const string VendorSuspicionStarted = "world.vendor.suspicion_started";
        public const string FutureForbiddenTeacherHook = "future.nahual.forbidden_teacher_hook";
        public const string FutureTransmutationCraftingHook = "future.nahual.transmutation_crafting_hook";
        public const string FutureAlternativeEndingHook = "future.nahual.alternative_ending_hook";
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Player/PlayerProfile.cs" -Content @'
using System.Collections.Generic;
using Game.Domain.Alignment;
using Game.Domain.Codex;
using Game.Domain.Cognition;

namespace Game.Domain.Player
{
    public class PlayerProfile
    {
        private readonly HashSet<string> alignmentFlags = new HashSet<string>();

        public PlayerSkillBook SkillBook { get; private set; }
        public PlayerClassCollection ClassCollection { get; private set; }
        public PlayerInventory Inventory { get; private set; }
        public PlayerWallet Wallet { get; private set; }
        public NovaCognitionProfile Cognition { get; private set; }
        public CodexProfile Codex { get; private set; }
        public KnowledgeAlignment CurrentAlignment { get; private set; }
        public AlignmentProgression AlignmentProgression { get; private set; }

        public IReadOnlyCollection<string> AlignmentFlags
        {
            get { return alignmentFlags; }
        }

        public PlayerProfile()
        {
            SkillBook = new PlayerSkillBook();
            ClassCollection = new PlayerClassCollection();
            Inventory = new PlayerInventory();
            Wallet = new PlayerWallet();
            Cognition = new NovaCognitionProfile(2);
            Codex = new CodexProfile();
            CurrentAlignment = KnowledgeAlignment.SharedWisdom;
            AlignmentProgression = new AlignmentProgression();
        }

        public void SetAlignment(KnowledgeAlignment alignment)
        {
            CurrentAlignment = alignment;
        }

        public void AddAlignmentProgress(
            KnowledgeAlignment alignment,
            int amount)
        {
            AlignmentProgression.AddProgress(alignment, amount);
        }

        public void AddAlignmentFlag(string flag)
        {
            if (!string.IsNullOrEmpty(flag))
            {
                alignmentFlags.Add(flag);
            }
        }

        public bool HasAlignmentFlag(string flag)
        {
            return alignmentFlags.Contains(flag);
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Resources/GameResourceId.cs" -Content @'
namespace Game.Domain.Resources
{
    public static class GameResourceId
    {
        public const string Gold = "gold";
        public const string WisdomFragments = "wisdom_fragments";
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Transmutation/TransmutationMaterialQuality.cs" -Content @'
namespace Game.Domain.Transmutation
{
    public enum TransmutationMaterialQuality
    {
        Stable,
        Unstable,
        Corrupted
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Transmutation/EnemyTransmutationProfile.cs" -Content @'
namespace Game.Domain.Transmutation
{
    public class EnemyTransmutationProfile
    {
        public bool Immune { get; private set; }
        public bool BossResistant { get; private set; }
        public double BaseSuccessChance { get; private set; }
        public string StableMaterialId { get; private set; }
        public string CorruptedMaterialId { get; private set; }
        public int WisdomFragmentReward { get; private set; }

        public EnemyTransmutationProfile(
            bool immune,
            bool bossResistant,
            double baseSuccessChance,
            string stableMaterialId,
            string corruptedMaterialId,
            int wisdomFragmentReward)
        {
            Immune = immune;
            BossResistant = bossResistant;
            BaseSuccessChance = baseSuccessChance;
            StableMaterialId = stableMaterialId;
            CorruptedMaterialId = corruptedMaterialId;
            WisdomFragmentReward = wisdomFragmentReward;
        }

        public static EnemyTransmutationProfile LesserBeing(
            string stableMaterialId,
            string corruptedMaterialId)
        {
            return new EnemyTransmutationProfile(false, false, 0.55d, stableMaterialId, corruptedMaterialId, 1);
        }

        public static EnemyTransmutationProfile Immune()
        {
            return new EnemyTransmutationProfile(true, false, 0d, "", "", 0);
        }

        public static EnemyTransmutationProfile BossResistant(
            string stableMaterialId,
            string corruptedMaterialId)
        {
            return new EnemyTransmutationProfile(false, true, 0.12d, stableMaterialId, corruptedMaterialId, 1);
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Transmutation/TransmutationResult.cs" -Content @'
namespace Game.Domain.Transmutation
{
    public class TransmutationResult
    {
        public bool Attempted { get; private set; }
        public bool Success { get; private set; }
        public string Message { get; private set; }
        public string MaterialId { get; private set; }
        public TransmutationMaterialQuality Quality { get; private set; }
        public int WisdomFragments { get; private set; }

        private TransmutationResult(
            bool attempted,
            bool success,
            string message,
            string materialId,
            TransmutationMaterialQuality quality,
            int wisdomFragments)
        {
            Attempted = attempted;
            Success = success;
            Message = message;
            MaterialId = materialId;
            Quality = quality;
            WisdomFragments = wisdomFragments;
        }

        public static TransmutationResult NotAttempted(string message)
        {
            return new TransmutationResult(false, false, message, "", TransmutationMaterialQuality.Corrupted, 0);
        }

        public static TransmutationResult Failed(string message)
        {
            return new TransmutationResult(true, false, message, "", TransmutationMaterialQuality.Corrupted, 0);
        }

        public static TransmutationResult Produced(
            string materialId,
            TransmutationMaterialQuality quality,
            int wisdomFragments,
            string message)
        {
            return new TransmutationResult(true, true, message, materialId, quality, wisdomFragments);
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Transmutation/TransmutationResolver.cs" -Content @'
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Player;
using Game.Domain.Resources;

namespace Game.Domain.Transmutation
{
    public class TransmutationResolver
    {
        private readonly IRandomSource random;

        public TransmutationResolver(IRandomSource random)
        {
            this.random = random;
        }

        public TransmutationResult TryTransmuteDefeatedEnemy(
            PlayerProfile profile,
            CombatEntity enemy,
            EnemyTransmutationProfile transmutationProfile)
        {
            if (profile == null || enemy == null || transmutationProfile == null)
            {
                return TransmutationResult.NotAttempted("The ritual lacks a valid subject.");
            }

            if (enemy.IsAlive)
            {
                return TransmutationResult.NotAttempted("Transmutation requires a defeated subject.");
            }

            if (transmutationProfile.Immune)
            {
                return TransmutationResult.Failed(enemy.Name + " leaves no readable remnant.");
            }

            double successChance = transmutationProfile.BaseSuccessChance;

            if (transmutationProfile.BossResistant)
            {
                successChance = successChance * 0.5d;
            }

            if (random.NextDouble() > successChance)
            {
                profile.Inventory.AddItem(transmutationProfile.CorruptedMaterialId, 1);
                return TransmutationResult.Produced(
                    transmutationProfile.CorruptedMaterialId,
                    TransmutationMaterialQuality.Corrupted,
                    0,
                    "The pattern collapses into corrupted material.");
            }

            bool unstable = random.NextDouble() < 0.35d;
            var quality = unstable ? TransmutationMaterialQuality.Unstable : TransmutationMaterialQuality.Stable;
            int fragments = unstable ? 0 : transmutationProfile.WisdomFragmentReward;

            profile.Inventory.AddItem(transmutationProfile.StableMaterialId, 1);

            if (fragments > 0)
            {
                profile.Wallet.AddCurrency(GameResourceId.WisdomFragments, fragments);
            }

            return TransmutationResult.Produced(
                transmutationProfile.StableMaterialId,
                quality,
                fragments,
                unstable
                    ? "The remnant answers, but the meaning is incomplete."
                    : "The remnant condenses into a usable fragment of understanding.");
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Skills/ForbiddenSkillMarker.cs" -Content @'
namespace Game.Domain.Skills
{
    public interface ForbiddenSkillMarker
    {
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Skills/TransmuteLesserBeingSkill.cs" -Content @'
using Game.Domain.Combat;
using Game.Domain.Common;

namespace Game.Domain.Skills
{
    public class TransmuteLesserBeingSkill : ICombatSkill, ForbiddenSkillMarker
    {
        public const string SkillId = "skill_transmute_lesser_being";

        public string Id
        {
            get { return SkillId; }
        }

        public string Name
        {
            get { return "Transmute Lesser Being"; }
        }

        public void Apply(
            CombatContext context,
            SkillProgress progress)
        {
            context.DamageMultiplier = 0f;
            context.EventLog.Add(
                GameEventType.TransmutationAttempted,
                context.Attacker.Name + " traces a forbidden pattern over the defeated form.");
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/World/NpcReactionType.cs" -Content @'
namespace Game.Domain.World
{
    public enum NpcReactionType
    {
        Neutral,
        Cautious,
        Distant,
        Restricted
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/World/NpcReaction.cs" -Content @'
namespace Game.Domain.World
{
    public class NpcReaction
    {
        public NpcReactionType Type { get; private set; }
        public string DialogueLine { get; private set; }
        public decimal VendorPriceMultiplier { get; private set; }
        public bool MentorTeachingAvailable { get; private set; }

        public NpcReaction(
            NpcReactionType type,
            string dialogueLine,
            decimal vendorPriceMultiplier,
            bool mentorTeachingAvailable)
        {
            Type = type;
            DialogueLine = dialogueLine;
            VendorPriceMultiplier = vendorPriceMultiplier;
            MentorTeachingAvailable = mentorTeachingAvailable;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/World/NpcReactionService.cs" -Content @'
using Game.Domain.Alignment;
using Game.Domain.Player;

namespace Game.Domain.World
{
    public class NpcReactionService
    {
        public NpcReaction ResolveVendorReaction(PlayerProfile profile)
        {
            if (profile.CurrentAlignment == KnowledgeAlignment.NahualPath)
            {
                return new NpcReaction(
                    NpcReactionType.Cautious,
                    "Your coins are ordinary. The silence around you is not.",
                    1.20m,
                    true);
            }

            return new NpcReaction(
                NpcReactionType.Neutral,
                "Take what you need, and return with what you learn.",
                1.00m,
                true);
        }

        public NpcReaction ResolveMentorReaction(PlayerProfile profile)
        {
            if (profile.HasAlignmentFlag(AlignmentFlag.AcceptedFirstNahualTeaching))
            {
                return new NpcReaction(
                    NpcReactionType.Distant,
                    "I can still teach you, but I no longer know what you are listening to.",
                    1.00m,
                    false);
            }

            return new NpcReaction(
                NpcReactionType.Neutral,
                "Wisdom survives because it is carried by many hands.",
                1.00m,
                true);
        }
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
        ChooseStableAction,
        CollectItem,
        ConsultMentor,
        StudyForbiddenFragment,
        AcceptNahualTeaching,
        RefuseNahualTeaching
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Missions/FirstNahualTeachingEvaluator.cs" -Content @'
using Game.Domain.Alignment;
using Game.Domain.Player;

namespace Game.Domain.Missions
{
    public class FirstNahualTeachingEvaluator : IMissionEvaluator
    {
        public const string Id = "mission_first_nahual_teaching";

        public string MissionId
        {
            get { return Id; }
        }

        public MissionOutcome Evaluate(
            PlayerProfile profile,
            MissionAttempt attempt)
        {
            bool discovered = attempt.Contains(MissionActionType.StudyForbiddenFragment);
            bool accepted = attempt.Contains(MissionActionType.AcceptNahualTeaching);
            bool refused = attempt.Contains(MissionActionType.RefuseNahualTeaching);

            if (!discovered || (accepted == refused))
            {
                return MissionOutcome.FailedButRetryable;
            }

            profile.AddAlignmentFlag(AlignmentFlag.DiscoveredNahualPath);
            profile.Codex.DiscoverEntry("codex_nahual_path");
            profile.Codex.DiscoverEntry("codex_wisdom_fragments");
            profile.Codex.DiscoverEntry("codex_transmutation");
            profile.Codex.DiscoverEntry("codex_forbidden_teachings");

            if (accepted)
            {
                profile.SetAlignment(KnowledgeAlignment.NahualPath);
                profile.AddAlignmentProgress(KnowledgeAlignment.NahualPath, 1);
                profile.AddAlignmentFlag(AlignmentFlag.AcceptedFirstNahualTeaching);
                profile.AddAlignmentFlag(AlignmentFlag.MentorDistanceStarted);
                profile.AddAlignmentFlag(AlignmentFlag.VendorSuspicionStarted);
                profile.AddAlignmentFlag(AlignmentFlag.FutureForbiddenTeacherHook);
                profile.AddAlignmentFlag(AlignmentFlag.FutureTransmutationCraftingHook);
                profile.AddAlignmentFlag(AlignmentFlag.FutureAlternativeEndingHook);
                profile.SkillBook.UnlockSkill("skill_transmute_lesser_being");
            }
            else
            {
                profile.SetAlignment(KnowledgeAlignment.SharedWisdom);
                profile.AddAlignmentProgress(KnowledgeAlignment.SharedWisdom, 1);
            }

            return MissionOutcome.Success;
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
                "Descubre la contradiccion real antes de intentar controlar al enemigo.",
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
                "Elige la opcion estable, no la mas vistosa ni la mas caotica.",
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
                "Recupera una reliquia danada y descubre su primera propiedad.",
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

            registry.Register(new MissionDefinition(
                FirstNahualTeachingEvaluator.Id,
                MissionType.Exploration,
                "The Silent Method",
                "A forbidden codex fragment suggests that wisdom may be extracted directly from reality.",
                new[]
                {
                    new MissionObjectiveDefinition(
                        "objective_study_forbidden_fragment",
                        MissionObjectiveType.CollectItem,
                        "codex_fragment_silent_method",
                        1),
                    new MissionObjectiveDefinition(
                        "objective_choose_method",
                        MissionObjectiveType.PerformAction,
                        "AcceptOrRefuseNahualTeaching",
                        1)
                },
                new[]
                {
                    new MissionRewardDefinition(MissionRewardType.UnlockCodexEntry, "codex_nahual_path"),
                    new MissionRewardDefinition(MissionRewardType.UnlockCodexEntry, "codex_wisdom_fragments"),
                    new MissionRewardDefinition(MissionRewardType.UnlockCodexEntry, "codex_transmutation"),
                    new MissionRewardDefinition(MissionRewardType.UnlockCodexEntry, "codex_forbidden_teachings")
                }));

            return registry;
        }

        public static MissionEvaluatorRegistry BuildEvaluatorRegistry()
        {
            var registry = new MissionEvaluatorRegistry();

            registry.Register(new LyraJudgementCrackEvaluator());
            registry.Register(new AtlasThreeCertaintiesEvaluator());
            registry.Register(new FirstRelicSideQuestEvaluator());
            registry.Register(new FirstNahualTeachingEvaluator());

            return registry;
        }
    }
}
'@

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
        PersistenceLoaded,
        AlignmentChanged,
        NahualPathDiscovered,
        TransmutationAttempted,
        TransmutationResolved,
        NpcReactionChanged,
        FutureHookPrepared
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Codex/CodexCategory.cs" -Content @'
namespace Game.Domain.Codex
{
    public enum CodexCategory
    {
        Bestiary,
        Classes,
        Skills,
        Items,
        Weapons,
        Paths,
        Teachings,
        Resources,
        Transmutation
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Codex/CodexBootstrap.cs" -Content @'
using System.Collections.Generic;

namespace Game.Domain.Codex
{
    public static class CodexBootstrap
    {
        public static IReadOnlyList<CodexEntryDefinition> BuildIteration2Entries()
        {
            return new List<CodexEntryDefinition>
            {
                new CodexEntryDefinition(
                    "codex_nahual_path",
                    CodexCategory.Paths,
                    "Nahual Path"),
                new CodexEntryDefinition(
                    "codex_wisdom_fragments",
                    CodexCategory.Resources,
                    "Wisdom Fragments"),
                new CodexEntryDefinition(
                    "codex_transmutation",
                    CodexCategory.Transmutation,
                    "Transmutation"),
                new CodexEntryDefinition(
                    "codex_forbidden_teachings",
                    CodexCategory.Teachings,
                    "Forbidden Teachings")
            };
        }
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
        public string CurrentAlignment { get; set; }
        public List<string> AlignmentFlags { get; set; }
        public Dictionary<string, int> AlignmentProgression { get; set; }
        public List<string> FixedTeachings { get; set; }
        public List<string> EquippedTeachings { get; set; }
        public Dictionary<string, int> Items { get; set; }
        public Dictionary<string, int> Currencies { get; set; }

        public PlayerSaveData()
        {
            CurrentAlignment = "SharedWisdom";
            AlignmentFlags = new List<string>();
            AlignmentProgression = new Dictionary<string, int>();
            FixedTeachings = new List<string>();
            EquippedTeachings = new List<string>();
            Items = new Dictionary<string, int>();
            Currencies = new Dictionary<string, int>();
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Slice/FoundationVerticalSlice.cs" -Content @'
using System;
using System.Collections.Generic;
using Game.Application.Integration;
using Game.Domain.Alignment;
using Game.Domain.Classes;
using Game.Domain.Cognition;
using Game.Domain.Common;
using Game.Domain.Combat;
using Game.Domain.Combat.AI;
using Game.Domain.Combat.Outcomes;
using Game.Domain.Combat.Passives;
using Game.Domain.Combat.Stats;
using Game.Domain.Equipment;
using Game.Domain.Missions;
using Game.Domain.Player;
using Game.Domain.Resources;
using Game.Domain.Skills;
using Game.Domain.Transmutation;
using Game.Domain.World;
using Game.Infrastructure.Random;

namespace Game.Slice
{
    public static class FoundationVerticalSlice
    {
        public static GameEventLog Run()
        {
            var log = new GameEventLog();
            var profile = CreateProfile();

            CompleteSharedWisdomMissions(profile, log);
            DiscoverNahualPath(profile, log);
            RunCombatAndTransmutation(profile, log);
            ResolveWorldReactions(profile, log);
            RegisterFutureHooks(profile, log);

            return log;
        }

        public static void RunToConsole()
        {
            foreach (var gameEvent in Run().Events)
            {
                Console.WriteLine("[" + gameEvent.Type + "] " + gameEvent.Message);
            }
        }

        private static PlayerProfile CreateProfile()
        {
            var profile = new PlayerProfile();
            var tlamatini = ClassBootstrap.BuildTlamatini();

            profile.ClassCollection.UnlockClass(tlamatini.Id);
            profile.ClassCollection.SetActiveClass(tlamatini.Id);

            foreach (var skillId in tlamatini.FixedSkillIds)
            {
                profile.SkillBook.UnlockSkill(skillId);
            }

            profile.Codex.DiscoverEntry("class_tlamatini");
            return profile;
        }

        private static void CompleteSharedWisdomMissions(
            PlayerProfile profile,
            GameEventLog log)
        {
            var service = BuildMissionService(log);

            profile.Cognition.UnlockTeaching(CognitiveArchetypeId.Lyra, "lyra_detect_weakness");
            profile.Cognition.SetActiveArchetype(CognitiveArchetypeId.Lyra);

            var lyra = new MissionAttempt();
            lyra.Record(MissionActionType.Analyze);
            lyra.Record(MissionActionType.ApplyDebuff);
            service.Complete(profile, LyraJudgementCrackEvaluator.Id, lyra);
            profile.Cognition.EquipFixedTeaching("lyra_detect_weakness");

            profile.Cognition.UnlockTeaching(CognitiveArchetypeId.Atlas, "atlas_stable_cast");
            profile.Cognition.SetActiveArchetype(CognitiveArchetypeId.Atlas);

            var atlas = new MissionAttempt();
            atlas.Record(MissionActionType.ChooseStableAction);
            service.Complete(profile, AtlasThreeCertaintiesEvaluator.Id, atlas);
            profile.Cognition.EquipFixedTeaching("atlas_stable_cast");

            var relic = new MissionAttempt();
            relic.Record(MissionActionType.CollectItem);
            service.Complete(profile, FirstRelicSideQuestEvaluator.Id, relic);
        }

        private static void DiscoverNahualPath(
            PlayerProfile profile,
            GameEventLog log)
        {
            var service = BuildMissionService(log);

            log.Add(
                GameEventType.NahualPathDiscovered,
                "In a hidden ruin, a broken codex fragment suggests that wisdom can be extracted, not only inherited.");

            var attempt = new MissionAttempt();
            attempt.Record(MissionActionType.StudyForbiddenFragment);
            attempt.Record(MissionActionType.AcceptNahualTeaching);

            service.Complete(profile, FirstNahualTeachingEvaluator.Id, attempt);

            log.Add(
                GameEventType.AlignmentChanged,
                "Alignment is now " + profile.CurrentAlignment + "; the world will begin to answer differently.");
        }

        private static MissionCompletionService BuildMissionService(GameEventLog log)
        {
            return new MissionCompletionService(
                MissionBootstrap.BuildDefinitionRegistry(),
                MissionBootstrap.BuildEvaluatorRegistry(),
                new MissionRewardApplicationService(),
                log);
        }

        private static void RunCombatAndTransmutation(
            PlayerProfile profile,
            GameEventLog log)
        {
            var player = CreatePlayer();
            var enemy = CreateEnemy();

            profile.Codex.DiscoverEntry(enemy.Id);

            var passives = new List<ICombatPassiveEffect>();
            passives.AddRange(new CognitionCombatPassiveResolver().Resolve(profile));

            var random = new SeededRandomSource(2026);
            var engine = new TurnBasedCombatEngine(
                random,
                new CombatPassivePipeline(passives));

            var runtime = new CombatRuntimeContext(player, enemy, log, profile);
            engine.StartCombat(runtime);

            var balance = new SkillDefinition("skill_balance_ritual", "Ritual de Balance", 3, new SkillCost(18, 3));
            var destiny = new SkillDefinition("skill_destiny_correction", "Correccion del Destino", 3, new SkillCost(20, 4));

            engine.ExecuteTurn(
                runtime,
                TurnRequest.SkillAction(player, enemy, new BalanceRitualSkill(), new SkillProgress(balance)));

            var ai = new BasicAttackEnemyAiStrategy();
            engine.ExecuteTurn(runtime, ai.ChooseAction(runtime));

            engine.ExecuteTurn(
                runtime,
                TurnRequest.SkillAction(player, enemy, new DestinyCorrectionSkill(), new SkillProgress(destiny)));

            if (enemy.IsAlive)
            {
                enemy.ReceiveDamage(enemy.Stats.CurrentHealth);
                runtime.EndCombat();
                log.Add(GameEventType.CombatEnded, enemy.Name + " is forced into defeat for the transmutation prototype demonstration.");
            }

            var outcome = new BattleOutcomeResolver().Resolve(runtime);

            new BattleRewardResolver().ApplyRewards(
                profile,
                outcome,
                new BattleRewardDefinition(GameResourceId.Gold, 25, 10),
                log);

            if (!enemy.IsAlive)
            {
                profile.Codex.CompleteEntry(enemy.Id);
            }

            ResolveFirstTransmutation(profile, enemy, random, log);
        }

        private static void ResolveFirstTransmutation(
            PlayerProfile profile,
            CombatEntity enemy,
            SeededRandomSource random,
            GameEventLog log)
        {
            if (profile.CurrentAlignment != KnowledgeAlignment.NahualPath
                || !profile.SkillBook.HasSkill(TransmuteLesserBeingSkill.SkillId))
            {
                return;
            }

            log.Add(
                GameEventType.TransmutationAttempted,
                "Nova attempts Transmute Lesser Being on the defeated scout.");

            var resolver = new TransmutationResolver(random);
            var result = resolver.TryTransmuteDefeatedEnemy(
                profile,
                enemy,
                EnemyTransmutationProfile.LesserBeing("material_echo_bone", "material_corrupted_echo"));

            log.Add(
                GameEventType.TransmutationResolved,
                result.Message + " Material=" + result.MaterialId + ", Quality=" + result.Quality + ", WisdomFragments=" + result.WisdomFragments + ".");
        }

        private static void ResolveWorldReactions(
            PlayerProfile profile,
            GameEventLog log)
        {
            var reactions = new NpcReactionService();
            var vendor = reactions.ResolveVendorReaction(profile);
            var mentor = reactions.ResolveMentorReaction(profile);

            log.Add(
                GameEventType.NpcReactionChanged,
                "Vendor reaction: " + vendor.Type + ", price multiplier x" + vendor.VendorPriceMultiplier + ". " + vendor.DialogueLine);

            log.Add(
                GameEventType.NpcReactionChanged,
                "Mentor reaction: " + mentor.Type + ", teaching available=" + mentor.MentorTeachingAvailable + ". " + mentor.DialogueLine);
        }

        private static void RegisterFutureHooks(
            PlayerProfile profile,
            GameEventLog log)
        {
            if (profile.HasAlignmentFlag(AlignmentFlag.FutureForbiddenTeacherHook))
            {
                log.Add(GameEventType.FutureHookPrepared, "Future hook prepared: forbidden teachers.");
            }

            if (profile.HasAlignmentFlag(AlignmentFlag.FutureTransmutationCraftingHook))
            {
                log.Add(GameEventType.FutureHookPrepared, "Future hook prepared: transmutation crafting and enemy harvesting.");
            }

            if (profile.HasAlignmentFlag(AlignmentFlag.FutureAlternativeEndingHook))
            {
                log.Add(GameEventType.FutureHookPrepared, "Future hook prepared: alignment-aware endings and cities.");
            }
        }

        private static CombatEntity CreatePlayer()
        {
            var stats = new CombatStats
            {
                MaxHealth = 120,
                CurrentHealth = 120,
                Attack = 18,
                Defense = 10,
                Precision = 16,
                Evasion = 10,
                CriticalChance = 8,
                CriticalResistance = 4,
                CriticalDamageMultiplier = 1.5f,
                DebuffChance = 22,
                DebuffResistance = 10
            };

            var loadout = new EquipmentLoadout();

            loadout.Equip(new EquipmentDefinition(
                "weapon_training_blade",
                "Training Blade",
                EquipmentSlot.Weapon,
                new CombatStats
                {
                    Attack = 3,
                    Precision = 2
                }));

            var finalStats = new EquipmentStatAggregator().Aggregate(stats, loadout);
            finalStats.CurrentHealth = finalStats.MaxHealth;

            return new CombatEntity("player_nova_tlamatini", "Nova / Tlamatini", finalStats, 100);
        }

        private static CombatEntity CreateEnemy()
        {
            return new CombatEntity(
                "enemy_goblin_scout",
                "Goblin Scout",
                new CombatStats
                {
                    MaxHealth = 90,
                    CurrentHealth = 90,
                    Attack = 14,
                    Defense = 6,
                    Precision = 12,
                    Evasion = 8,
                    CriticalChance = 5,
                    CriticalResistance = 3,
                    CriticalDamageMultiplier = 1.4f,
                    DebuffChance = 8,
                    DebuffResistance = 16
                },
                40);
        }
    }
}
'@

Write-RepoFile -Path "docs/VERTICAL_SLICE_ITERATION_2.md" -Content @'
# Fragments of Wisdom - Vertical Slice Iteration 2

## Purpose

Iteration 2 introduces the Nahual Path without creating a second campaign. The goal is to prove that the foundation can support philosophical branching, systemic consequences, and forbidden mechanics without rewriting the combat engine or duplicating progression systems.

Core question:

> Is wisdom something that is shared, or extracted?

## Added Systems

### 1. Alignment System

Added `KnowledgeAlignment` with two values:

- `SharedWisdom`
- `NahualPath`

`PlayerProfile` now owns:

- `CurrentAlignment`
- `AlignmentFlags`
- `AlignmentProgression`

This supports future NPC reactions, vendor restrictions, skill restrictions, mission branching, alignment cities, alternative endings, and forbidden teachers.

### 2. Nahual Unlock Event

The vertical slice now includes `The Silent Method`, an optional mission where Nova discovers a forbidden codex fragment in a hidden ruin. The event is intentionally mysterious and intellectually seductive, not demonic or cartoonishly evil.

### 3. First Forbidden Skill

Added `Transmute Lesser Being` as the first Nahual-exclusive skill. The prototype is implemented through:

- `TransmuteLesserBeingSkill`
- `TransmutationResolver`
- `EnemyTransmutationProfile`
- `TransmutationResult`

The skill does not generate infinite value. It includes failure, unstable output, corrupted material, immunity hooks, and boss resistance hooks.

### 4. Wisdom Fragments

Added `GameResourceId.WisdomFragments` as a placeholder resource. It represents incomplete understanding and extracted cognition, not crystals.

Current use is minimal: successful stable transmutation may grant a small amount.

### 5. NPC Reaction System

Added lightweight world reactions through `NpcReactionService`:

- vendors become cautious and increase prices
- a mentor becomes distant
- a teaching can become unavailable through reaction data

This is not a full reputation system yet.

### 6. Minimal Branching Mission

Added `FirstNahualTeachingEvaluator`.

The mission supports two branches:

- refuse the fragment and stay on Shared Wisdom
- accept the fragment and enter the Nahual Path

The current scripted slice chooses the Nahual branch to demonstrate the new systems.

### 7. Codex Expansion

Added codex definitions for:

- Nahual Path
- Wisdom Fragments
- Transmutation
- Forbidden Teachings

### 8. Future Hooks

Prepared flags for:

- forbidden teachers
- transmutation crafting
- enemy harvesting
- alternative endings
- alignment-aware cities

These are intentionally placeholders only.

## Architectural Notes

This iteration extends the foundation instead of forking it:

- no separate save structure
- no duplicated combat engine
- no separate progression tree
- no second campaign implementation
- no full reputation system yet

The new branch is expressed with flags, alignment state, mission outcomes, codex entries, and resolver services.
'@

Write-RepoFile -Path "docs/TODO_BACKLOG.md" -Content @'
# TODO Backlog

## Vertical Slice V2 Completed Foundation

- Alignment system introduced.
- Nahual unlock mission added.
- First forbidden skill stub/prototype added.
- Transmutation resolver added.
- Wisdom Fragments resource id added.
- Lightweight NPC reaction service added.
- Iteration 2 codex entries added.
- Future hooks prepared without full implementation.

## Next Iteration Candidates

1. Add a real combat action timing window for defeated-enemy transmutation.
2. Add transmutation profiles to enemy definitions instead of passing them manually.
3. Add a vendor price calculation service using `NpcReactionService`.
4. Add mentor teaching availability checks to mission/teaching UI.
5. Add Wisdom Fragment spend rules for cognition repairs and forbidden nodes.
6. Add codex completion percentages per category.
7. Add save/load mapping for `CurrentAlignment`, `AlignmentFlags`, and `AlignmentProgression`.
8. Add corrupted cognition as a risk system, not as a morality meter.
9. Add a Shared Wisdom counter-branch reward so the normal path remains desirable.
10. Add tests for transmutation failure, immunity, boss resistance, and unstable output.
'@

Write-Host ""
Write-Host "Iteration 2 increment applied." -ForegroundColor Green
