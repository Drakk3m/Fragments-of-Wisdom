# V3 Combat Foundation

This document describes the starting architecture for the V3 combat foundation.
The key decision is that V3 combat starts from the existing canonical `TurnBasedCombatEngine` path, not from a second parallel combat resolver.

## Canonical Combat Path

Current combat turn resolution flows through:

```text
TurnRequest
  -> TurnBasedCombatEngine
  -> CombatRuntimeContext
  -> CombatContext
  -> DamageResolver
  -> CombatEntity / ResourcePool / ResonanceProfile / GameEventLog
```

`TurnBasedCombatEngine` is the canonical owner of combat turn sequencing. It currently coordinates:

- turn start and turn end events
- basic attacks
- skill actions
- resource cost checks through `ResourcePool`
- active resonance cost checks through optional `ResonanceProfile`
- skill application
- damage resolution
- hit and miss results
- critical hit events
- HP damage
- defeat detection
- resource regeneration
- passive hooks
- debuff resolution hooks

Future V3 combat work should extend this path incrementally instead of adding another turn-resolution API.

## Canonical Classes

`TurnBasedCombatEngine` is the combat turn engine. New combat turn behavior should be integrated here or in services it already uses.

`TurnRequest` is the action request model for a turn. It distinguishes basic attacks from skill actions and carries the skill/progress data needed by the engine.

`CombatEntity` is the combatant runtime model. It owns identity, display name, `CombatStats`, `ResourcePool`, buffs, debuffs, and turn flags.

`CombatStats` is the combat stat model. It currently includes health, attack, defense, precision, evasion, critical stats, debuff stats, buff stats, and control resistance.

`ResourcePool` is the resource state model. Resource spending and regeneration should stay here unless a future refactor deliberately expands this abstraction.

`ActiveResonance` is the spendable resonance pool for skills. It wraps `ResourcePool` and exposes only small active-resonance facts and operations.

`ResonanceReserve` is the stored resonance pool used for refilling active resonance. It also wraps `ResourcePool`.

`ResonanceProfile` groups `ActiveResonance` and `ResonanceReserve`. It supports active resonance spending checks and reserve-to-active refill operations.

`ResonanceRefillAction` is the current turn-consuming action for moving reserve resonance into active resonance. It is separate from skill execution.

`SkillDefinition.ResonanceCost` declares the active resonance cost for a skill. Existing skills default to `0`, preserving current V1 behavior.

`CombatRuntimeContext.ResonanceProfile` is optional. When present, `TurnBasedCombatEngine` uses it to validate and spend active resonance during skill execution.

`CombatEntity.StatusEffects` is the runtime status-effect collection for a combatant. Status effects are pure domain state and are resolved by the canonical turn engine.

`StatusEffectType` defines the current status identifiers: `Poison`, `Burn`, `SkillSeal`, `RegenBlock`, and `Regen`.

`StatusEffectInstance` carries the status type, remaining duration, stacking behavior, and optional `Potency` value used by effect rules such as Regen, Poison, and Burn.

`StatusEffectCollection` owns active status instances. It applies non-stacking rules, exposes active-effect lookup, ticks durations, and removes expired effects.

`StatusEffectResolver` applies status instances and resolves the explicit status boundary used by `TurnBasedCombatEngine`.

`DamageResolver` is the current damage calculation service. It owns hit/miss checks, critical checks, and final damage math using `CombatContext`.

`IRandomSource` is the randomness boundary. Tests should inject deterministic implementations so combat smoke coverage remains stable.

## Intentionally Not Kept

The provisional parallel API was intentionally not kept:

- `CombatResolver`
- `CombatantProfile`
- `CombatAction`
- `CombatTurnResult`

These concepts duplicated existing engine responsibilities:

- `CombatResolver` overlapped with `TurnBasedCombatEngine.ExecuteTurn`.
- `CombatantProfile` overlapped with `CombatEntity` and `ResourcePool`.
- `CombatAction` overlapped with `TurnRequest` and `ICombatSkill`.
- `CombatTurnResult` overlapped with `DamageResult`, `GameEventLog`, and combat-ended state.

Do not reintroduce these names or equivalent parallel concepts unless there is a deliberate architectural decision to make them thin DTOs around the canonical engine without owning turn resolution.

## Current Smoke Coverage

Combat smoke coverage should exercise the canonical engine path.

Current expected combat foundation scenarios:

- basic attack damages target
- defeated target is detected
- miss causes no damage
- critical hit increases damage
- skill/resource cost failure is blocked safely

These checks should use deterministic `IRandomSource` values and should not depend on Unity, animation, UI, inventory, equipment, rewards, saves, or enemy AI.

## Resonance Foundation

Resonance is a V3 combat resource foundation layered onto the canonical combat and skill systems.

Current resonance types:

- `ActiveResonance`: the resonance currently available to spend on skills.
- `ResonanceReserve`: stored resonance that can refill active resonance.
- `ResonanceProfile`: the aggregate profile containing active and reserve resonance.
- `ResonanceRefillAction`: the explicit action that refills active resonance from reserve resonance.

Current skill integration:

- `SkillDefinition.ResonanceCost` declares a skill's active resonance cost.
- `CombatRuntimeContext` may carry an optional `ResonanceProfile`.
- `TurnBasedCombatEngine` is the canonical spend/block path for skill resonance costs.

Rules:

- Skills spend active resonance only.
- Reserve resonance is not auto-spent for skill costs.
- Refill remains a separate action.
- Refill consumes a combat turn when successful.
- Empty reserve blocks refill without consuming a turn.
- A skill with insufficient active resonance is blocked before skill effects or damage are applied.
- Existing skills with no declared resonance cost continue to behave as before.

The resonance foundation does not currently include buffs, status effects, equipment, inventory, classes, saves, Unity integration, or any parallel combat resolver.

## Status Effect Foundation

Status effects are part of the canonical V3 combat path. They are stored on `CombatEntity.StatusEffects` and resolved through `StatusEffectResolver`, which is called by `TurnBasedCombatEngine` at the acting combatant's turn boundary.

Current status timing:

- status effects resolve for the acting combatant
- resolution happens during the engine's turn boundary after the action and resource regeneration flow
- durations tick after status effects apply
- expired status effects are removed during duration ticking

Current effects:

- `SkillSeal` blocks skill actions before Essence or active Resonance is spent.
- `SkillSeal` does not block basic attacks.
- `Regen` heals the affected combatant by `StatusEffectInstance.Potency`.
- `Regen` healing cannot exceed max HP.
- `RegenBlock` prevents `Regen` healing while active.
- `Poison` deals damage-over-time by `StatusEffectInstance.Potency`.
- `Burn` deals damage-over-time by `StatusEffectInstance.Potency`.
- Poison/Burn DOT damage cannot reduce HP below `0`.
- Poison/Burn DOT can defeat the affected combatant through the canonical engine path.

Explicit status boundary order:

```text
Poison/Burn DOT
  -> Regen
  -> duration tick / expire
```

This ordering is intentional and covered by smoke tests. Future effects should document whether they run before DOT, between DOT and Regen, after Regen, or during duration ticking.

The current status foundation does not include stat modifiers, cleanse rules, resistance/chance application, UI, Unity adapters, equipment, classes, inventory, save changes, or a parallel combat resolver.

## Stat Modifier Foundation

The current stat modifier foundation is a pure domain layer around `CombatStats`.

Current types:

- `CombatStatModifier`
- `CombatStatModifierResolver`
- `CombatStatType`
- `StatModifierSourceType`

Rules:

- Flat modifiers only.
- Multiple flat modifiers stack additively.
- Resolving modifiers returns effective cloned `CombatStats`.
- Base `CombatStats` remains unchanged.
- Effective stats can be passed into `CombatEntity` and consumed by the canonical `TurnBasedCombatEngine` / `DamageResolver` path.

The stat modifier foundation does not currently include inventory, classes, save changes, Unity, UI, percentage modifiers, buff integration, status-effect integration, or a parallel combat resolver.

## Equipment Foundation

Equipment is a small pure-domain layer on top of the stat modifier system.

Current types:

- `EquipmentProfile`
- `EquipmentDefinition`
- `EquipmentLoadout`
- `EquipmentStatModifierService`
- `EquipmentStatAggregator`
- `EquipmentSlot`

Responsibilities:

- `EquipmentProfile` owns equipped items by slot.
- `EquipmentDefinition` describes one equippable item and exposes flat `CombatStatModifier` values.
- `EquipmentLoadout` is a compatibility wrapper over `EquipmentProfile` for older V1 slice code.
- `EquipmentStatModifierService` gathers flat modifiers from currently equipped items.
- `EquipmentStatModifierService.ResolveEffectiveStats` delegates final effective stat calculation to `CombatStatModifierResolver`.
- `EquipmentStatAggregator` remains as a compatibility adapter and delegates to `EquipmentStatModifierService`.

Rules:

- Equipment is equipped by `EquipmentSlot`.
- Equipping another item in the same slot replaces the previous item.
- Unequip removes the item in the requested slot and reports whether anything was removed.
- Null items are rejected safely.
- Equipped items expose flat `CombatStatModifier` values only.
- Base `CombatStats` is not mutated when resolving equipment stats.
- Effective equipment stats are produced through `CombatStatModifierResolver`, so stat invariant clamping still applies.

Current non-goals:

- no inventory
- no item quantities
- no rarity
- no durability
- no skill providers
- no class restrictions
- no save integration
- no UI
- no Unity adapters
- no percentage modifiers
- no rewards

## Boundaries

Allowed:

- pure C# domain classes
- deterministic smoke coverage
- small services used by `TurnBasedCombatEngine`
- focused refactors that preserve V1 event output

Forbidden for the V3 combat foundation:

- Unity `MonoBehaviour` combat logic
- UI, animation, or scene coupling
- inventory or equipment integration unless explicitly requested
- rewards or save changes
- enemy AI beyond supplying a `TurnRequest`
- generator-wide rewrites
- a second combat turn-resolution system

## Validation Expectations

After changing combat foundation code or tests, run:

```powershell
dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal
dotnet run --project src\Game.SmokeTests\Game.SmokeTests.csproj --no-build
git diff --check
```

Expected smoke output:

- V1 slice emitted event count: `38`
- Smoke output is grouped into `V1 Legacy Slice`, `Shared Domain Foundations`, `V3 Combat / Skills / Resonance / Status / Stats`, and `V2 Narrative / Progression / Persistence`.
- V1 Legacy Slice assertions: `2`
- Shared Domain Foundations assertions: `16`
- V2 Narrative / Progression / Persistence assertions: `239`
- V3 Combat / Skills / Resonance / Status / Stats assertions: `100`
- total smoke assertions executed: `357`

The V1 event count is a guardrail. If it changes, treat that as a behavior change unless the user explicitly requested a V1 loop update.
