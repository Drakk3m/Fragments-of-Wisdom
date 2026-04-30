# FOUNDATION MASTER SPEC â€” NOVA RPG

## Vision

Nova RPG is a domain-first, turn-based strategic RPG.

The initial idea came from simple mobile-ad games where a hero grows by defeating lower-level enemies, but the real design evolved into:

```text
Explore node -> Fight -> Gain rewards -> Progress missions/codex/skills/classes/cognition -> Unlock new options
```

The player should not fight only to inflate numbers. Combat must produce discovery, progression and new decisions.

## Architecture Rules

1. No gameplay rules in MonoBehaviours.
2. Domain is pure C#.
3. Unity is only an adapter/presentation layer.
4. Systems communicate through explicit services and event logs.
5. Every feature should be testable outside Unity.

## Combat

Turn-based strategic combat.

Antagonistic stats:

- Attack / Defense
- Precision / Evasion
- CriticalChance / CriticalResistance
- DebuffChance / DebuffResistance

Resource:

- Essence
- Skills consume Essence
- Basic attacks are free
- Essence regenerates over time/actions

## Skills

Skills have:

- Definition
- Cost
- Level
- Experience
- Mastery

Mastered skills can later be retained across class changes.

Initial skills:

- Mirada Profunda
- Ritual de Balance
- CorrecciÃ³n del Destino
- RefutaciÃ³n Menor

## Classes

Classes define how Nova fights.

Initial class:

- Tlamatini

## Cognition

Cognition is separate from classes.

Classes = how Nova fights.  
Cognition = how Nova understands and modifies systems.

Initial archetypes:

- Lyra: analysis, weakness reveal, cleanse/counterplay
- Atlas: stability, prediction, cost/variance control
- Gepetto: future system manipulation/crafting
- Spark: future chaos/high variance

Teachings can be:

- temporary
- fixed
- equipped

## Missions

Mission types:

- MainStory
- SideQuest
- MasterTraining
- CodexChallenge
- Bounty
- Exploration

Implemented examples:

- Lyra â€” La Grieta del Juicio
- Atlas â€” El Problema de las Tres Certezas
- Sidequest â€” El Relicario Partido

## Codex

Codex categories:

- Bestiary
- Classes
- Skills
- Items
- Weapons

## Equipment

Slots:

- Weapon
- Armor
- Accessory1
- Accessory2

## Foundation Slice

The slice demonstrates:

1. Nova starts as Tlamatini.
2. Lyra teaching is unlocked temporarily.
3. Lyra mission is completed.
4. Lyra teaching is fixed/equipped.
5. Atlas teaching is unlocked temporarily.
6. Atlas mission is completed.
7. Atlas teaching is fixed/equipped.
8. A sidequest grants item/currency/codex entry.
9. Nova fights Goblin Scout.
10. Cognition passives affect combat.
11. Enemy AI acts.
12. Codex and battle rewards are updated.
