
#requires -version 5.1
<#
03 Player Cognition Classes Equipment Codex
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
Write-Host "=== 03 Player Cognition Classes Equipment Codex ===" -ForegroundColor Cyan
Write-Host "Target: $Root"
Write-Host ""

Write-RepoFile -Path "src/Game/Domain/Cognition/CognitiveArchetypeId.cs" -Content @'
namespace Game.Domain.Cognition
{
    public enum CognitiveArchetypeId
    {
        Gepetto,
        Lyra,
        Atlas,
        Spark
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Cognition/TeachingDefinition.cs" -Content @'
namespace Game.Domain.Cognition
{
    public class TeachingDefinition
    {
        public string Id { get; private set; }
        public CognitiveArchetypeId ArchetypeId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        public TeachingDefinition(
            string id,
            CognitiveArchetypeId archetypeId,
            string name,
            string description)
        {
            Id = id;
            ArchetypeId = archetypeId;
            Name = name;
            Description = description;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Cognition/NovaCognitionProfile.cs" -Content @'
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Domain.Cognition
{
    public class NovaCognitionProfile
    {
        private readonly Dictionary<CognitiveArchetypeId, HashSet<string>> unlocked =
            new Dictionary<CognitiveArchetypeId, HashSet<string>>();

        private readonly HashSet<string> temporary = new HashSet<string>();
        private readonly HashSet<string> fixedTeachings = new HashSet<string>();
        private readonly HashSet<string> equipped = new HashSet<string>();

        public CognitiveArchetypeId? ActiveArchetype { get; private set; }
        public int MaxEquippedFixedTeachings { get; private set; }

        public NovaCognitionProfile(int maxEquippedFixedTeachings)
        {
            MaxEquippedFixedTeachings = maxEquippedFixedTeachings;

            foreach (CognitiveArchetypeId id in Enum.GetValues(typeof(CognitiveArchetypeId)))
            {
                unlocked[id] = new HashSet<string>();
            }
        }

        public void UnlockTeaching(
            CognitiveArchetypeId archetype,
            string teachingId)
        {
            unlocked[archetype].Add(teachingId);

            if (ActiveArchetype == archetype)
            {
                temporary.Add(teachingId);
            }
        }

        public void SetActiveArchetype(CognitiveArchetypeId archetype)
        {
            ActiveArchetype = archetype;
            temporary.Clear();

            foreach (var teaching in unlocked[archetype])
            {
                temporary.Add(teaching);
            }
        }

        public bool FixTeaching(string teachingId)
        {
            return fixedTeachings.Add(teachingId);
        }

        public bool EquipFixedTeaching(string teachingId)
        {
            if (!fixedTeachings.Contains(teachingId))
            {
                return false;
            }

            if (equipped.Count >= MaxEquippedFixedTeachings)
            {
                return false;
            }

            return equipped.Add(teachingId);
        }

        public bool HasActiveTeaching(string teachingId)
        {
            return temporary.Contains(teachingId) || equipped.Contains(teachingId);
        }

        public IReadOnlyCollection<string> GetActiveTeachings()
        {
            return temporary.Concat(equipped).ToList().AsReadOnly();
        }
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
        Weapons
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Codex/CodexEntryDefinition.cs" -Content @'
namespace Game.Domain.Codex
{
    public class CodexEntryDefinition
    {
        public string Id { get; private set; }
        public CodexCategory Category { get; private set; }
        public string Name { get; private set; }

        public CodexEntryDefinition(
            string id,
            CodexCategory category,
            string name)
        {
            Id = id;
            Category = category;
            Name = name;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Codex/CodexProfile.cs" -Content @'
using System.Collections.Generic;

namespace Game.Domain.Codex
{
    public class CodexProfile
    {
        private readonly HashSet<string> discovered = new HashSet<string>();
        private readonly HashSet<string> completed = new HashSet<string>();

        public void DiscoverEntry(string entryId)
        {
            discovered.Add(entryId);
        }

        public void CompleteEntry(string entryId)
        {
            discovered.Add(entryId);
            completed.Add(entryId);
        }

        public bool IsDiscovered(string entryId)
        {
            return discovered.Contains(entryId);
        }

        public bool IsCompleted(string entryId)
        {
            return completed.Contains(entryId);
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Player/PlayerSkillBook.cs" -Content @'
using System.Collections.Generic;

namespace Game.Domain.Player
{
    public class PlayerSkillBook
    {
        private readonly HashSet<string> skills = new HashSet<string>();

        public bool UnlockSkill(string skillId)
        {
            return skills.Add(skillId);
        }

        public bool HasSkill(string skillId)
        {
            return skills.Contains(skillId);
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Player/PlayerClassCollection.cs" -Content @'
using System.Collections.Generic;

namespace Game.Domain.Player
{
    public class PlayerClassCollection
    {
        private readonly HashSet<string> classes = new HashSet<string>();

        public string ActiveClassId { get; private set; }

        public bool UnlockClass(string classId)
        {
            return classes.Add(classId);
        }

        public bool HasClass(string classId)
        {
            return classes.Contains(classId);
        }

        public bool SetActiveClass(string classId)
        {
            if (!classes.Contains(classId))
            {
                return false;
            }

            ActiveClassId = classId;
            return true;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Player/PlayerInventory.cs" -Content @'
using System.Collections.Generic;

namespace Game.Domain.Player
{
    public class PlayerInventory
    {
        private readonly Dictionary<string, int> items = new Dictionary<string, int>();

        public IReadOnlyDictionary<string, int> Items
        {
            get { return items; }
        }

        public void AddItem(
            string itemId,
            int amount)
        {
            if (!items.ContainsKey(itemId))
            {
                items[itemId] = 0;
            }

            items[itemId] += amount;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Player/PlayerWallet.cs" -Content @'
using System.Collections.Generic;

namespace Game.Domain.Player
{
    public class PlayerWallet
    {
        private readonly Dictionary<string, int> currencies = new Dictionary<string, int>();

        public IReadOnlyDictionary<string, int> Currencies
        {
            get { return currencies; }
        }

        public void AddCurrency(
            string currencyId,
            int amount)
        {
            if (!currencies.ContainsKey(currencyId))
            {
                currencies[currencyId] = 0;
            }

            currencies[currencyId] += amount;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Player/PlayerProfile.cs" -Content @'
using Game.Domain.Codex;
using Game.Domain.Cognition;

namespace Game.Domain.Player
{
    public class PlayerProfile
    {
        public PlayerSkillBook SkillBook { get; private set; }
        public PlayerClassCollection ClassCollection { get; private set; }
        public PlayerInventory Inventory { get; private set; }
        public PlayerWallet Wallet { get; private set; }
        public NovaCognitionProfile Cognition { get; private set; }
        public CodexProfile Codex { get; private set; }

        public PlayerProfile()
        {
            SkillBook = new PlayerSkillBook();
            ClassCollection = new PlayerClassCollection();
            Inventory = new PlayerInventory();
            Wallet = new PlayerWallet();
            Cognition = new NovaCognitionProfile(2);
            Codex = new CodexProfile();
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Classes/ClassDefinition.cs" -Content @'
using System.Collections.Generic;

namespace Game.Domain.Classes
{
    public class ClassDefinition
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public IReadOnlyList<string> FixedSkillIds { get; private set; }

        public ClassDefinition(
            string id,
            string name,
            IReadOnlyList<string> fixedSkillIds)
        {
            Id = id;
            Name = name;
            FixedSkillIds = fixedSkillIds;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Classes/SkillTreeNodeDefinition.cs" -Content @'
using System.Collections.Generic;

namespace Game.Domain.Classes
{
    public class SkillTreeNodeDefinition
    {
        public string Id { get; private set; }
        public string SkillId { get; private set; }
        public IReadOnlyList<string> RequiredNodeIds { get; private set; }

        public SkillTreeNodeDefinition(
            string id,
            string skillId,
            IReadOnlyList<string> requiredNodeIds)
        {
            Id = id;
            SkillId = skillId;
            RequiredNodeIds = requiredNodeIds;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Classes/ClassBootstrap.cs" -Content @'
using System.Collections.Generic;

namespace Game.Domain.Classes
{
    public static class ClassBootstrap
    {
        public static ClassDefinition BuildTlamatini()
        {
            return new ClassDefinition(
                "class_tlamatini",
                "Tlamatini",
                new[]
                {
                    "skill_deep_sight",
                    "skill_balance_ritual",
                    "skill_destiny_correction"
                });
        }

        public static IReadOnlyDictionary<string, SkillTreeNodeDefinition> BuildTlamatiniSkillTree()
        {
            return new Dictionary<string, SkillTreeNodeDefinition>
            {
                {
                    "node_deep_sight",
                    new SkillTreeNodeDefinition("node_deep_sight", "skill_deep_sight", new string[0])
                },
                {
                    "node_balance_ritual",
                    new SkillTreeNodeDefinition("node_balance_ritual", "skill_balance_ritual", new[] { "node_deep_sight" })
                },
                {
                    "node_destiny_correction",
                    new SkillTreeNodeDefinition("node_destiny_correction", "skill_destiny_correction", new[] { "node_balance_ritual" })
                }
            };
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Equipment/EquipmentSlot.cs" -Content @'
namespace Game.Domain.Equipment
{
    public enum EquipmentSlot
    {
        Weapon,
        Armor,
        Accessory1,
        Accessory2
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Equipment/EquipmentDefinition.cs" -Content @'
using Game.Domain.Combat.Stats;

namespace Game.Domain.Equipment
{
    public class EquipmentDefinition
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public EquipmentSlot Slot { get; private set; }
        public CombatStats StatBonus { get; private set; }
        public string TaughtSkillId { get; private set; }

        public EquipmentDefinition(
            string id,
            string name,
            EquipmentSlot slot,
            CombatStats statBonus,
            string taughtSkillId = null)
        {
            Id = id;
            Name = name;
            Slot = slot;
            StatBonus = statBonus;
            TaughtSkillId = taughtSkillId;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Equipment/EquipmentLoadout.cs" -Content @'
using System.Collections.Generic;

namespace Game.Domain.Equipment
{
    public class EquipmentLoadout
    {
        private readonly Dictionary<EquipmentSlot, EquipmentDefinition> equipped =
            new Dictionary<EquipmentSlot, EquipmentDefinition>();

        public IReadOnlyDictionary<EquipmentSlot, EquipmentDefinition> Equipped
        {
            get { return equipped; }
        }

        public void Equip(EquipmentDefinition item)
        {
            equipped[item.Slot] = item;
        }
    }
}
'@

Write-RepoFile -Path "src/Game/Domain/Equipment/EquipmentStatAggregator.cs" -Content @'
using Game.Domain.Combat.Stats;

namespace Game.Domain.Equipment
{
    public class EquipmentStatAggregator
    {
        public CombatStats Aggregate(
            CombatStats baseStats,
            EquipmentLoadout loadout)
        {
            var result = baseStats.Clone();

            foreach (var item in loadout.Equipped.Values)
            {
                result.MaxHealth += item.StatBonus.MaxHealth;
                result.Attack += item.StatBonus.Attack;
                result.Defense += item.StatBonus.Defense;
                result.Precision += item.StatBonus.Precision;
                result.Evasion += item.StatBonus.Evasion;
                result.CriticalChance += item.StatBonus.CriticalChance;
                result.CriticalResistance += item.StatBonus.CriticalResistance;
                result.DebuffChance += item.StatBonus.DebuffChance;
                result.DebuffResistance += item.StatBonus.DebuffResistance;
                result.BuffPotency += item.StatBonus.BuffPotency;
                result.BuffResistance += item.StatBonus.BuffResistance;
                result.ControlResistance += item.StatBonus.ControlResistance;
            }

            return result;
        }
    }
}
'@

Write-Host ""
Write-Host "Done." -ForegroundColor Green
