# Fragments of Wysdom: Game Design, Architecture & Development Bible

**Version 4.1, consolidated 2026-10-31. Single authoritative project specification.**
This is a documentation-only consolidation, not approval to implement a new mechanic.
The user's latest instruction authorizes this pass on `main`, superseding the earlier branch-only mission.
Editing baseline: `ed6ddf54a3541d87d051dd7f6411d86f6f2df837`.

## How to Read This Bible

Parts I-VI are the current interpretation. Part VII preserves complete historical source contracts, examples and decisions; it is **not a second backlog or a set of executable instructions**. Where evidence conflicts, the reproduced audit and explicit corrections in Parts II-V override old claims. An unresolved design choice remains OPEN DECISION, not an implicit gameplay requirement. Contract details not summarized in active chapters remain available in the archive; do not silently discard or implement them.

Keep two independent dimensions:
- **Design:** LOCKED / ACCEPTED, PROMISING / PROPOSED, OPEN, REJECTED, or DEFERRED. Accepted direction is not a completed mechanic.
- **Implementation:** VERIFIED (the specified executed scenario only), PARTIAL (implemented with known gaps), CODE FOUNDATION (source exists, not a complete loop), CONCEPT ONLY, or NOT VERIFIED.
- **Evidence:** executed smoke, reproduced audit probe, source inspection, historical contract, or conversation design. Inferences and proposed remedies are labelled separately.

A passing backend suite does not certify Unity, balance, complete content or a playable game.

## Contents

- [Part I: Game Design & Creative Vision](#creative-vision)
- [Echoes of Wysdom: MUST HAVE design](#echoes-of-wysdom)
- [The Path Unwalked: MUST HAVE narrative vision](#path-unwalked)
- [Part II: Game Systems & Rules](#systems)
- [Combat, resources and skill access](#combat-rules)
- [Classes, mastery, mentors, trees and equipment](#progression-rules)
- [Narrative, Codex, missions and unlocks](#narrative-rules)
- [Part III: Software Architecture](#architecture)
- [Runtime ownership and integration](#runtime-ownership)
- [Persistence and compatibility](#persistence)
- [Historical tooling and Unity boundaries](#tooling)
- [Part IV: Implementation Status](#implementation-status)
- [Verification commands, counts and evidence](#verification)
- [Part V: Audit & Technical Debt](#audit)
- [Reproduced findings](#reproduced-findings)
- [Contradictions and open decisions](#reconciliation)
- [Part VI: Development Governance](#governance)
- [Workflow verification checklist](#workflow-verification)
- [Single current backlog](#current-backlog)
- [P9 visual boundary and playable slice architecture audit](#p9-visual-boundary)
- [P9.1 combat encounter composition](#combat-encounter-composition)
- [P9.2 read-only combat projections](#combat-readonly-projections)
- [P9.3 combat intent and event batches](#combat-intent-event-boundary)
- [P9.4 combat encounter turn flow](#combat-turn-flow)
- [P9.5 external presentation workflow verification](#combat-presentation-verification)
- [Part VII: Historical Decisions & Reference Archive](#historical-archive)
- [Preservation matrix and source inventory](#preservation-matrix)

- [Recovered visual and Codex details](#recovered-visual-details)
- [Recovered historical system decisions](#recovered-system-decisions)
- [Canonical Markdown policy](#documentation-format)
- [Binary document inventory and migrations](#binary-document-ledger)

<a id="creative-vision"></a>
# Part I: Game Design & Creative Vision

Creative direction below is retained from the previous Bible. Implementation descriptions are qualified by Parts II-V; design vocabulary is not evidence of production readiness.

### 1. Vision, player fantasy and emotional pillars

**LOCKED DESIGN.** An HD-2D-inspired, strategic RPG about wisdom, cognition, mentorship, memory, mastery, philosophical choice and restoration. The core loop is:

`Explore → Encounter / Fight → Rewards and discoveries → Missions / Codex / Skills / Classes / Cognition → New choices → Explore`

Combat should create discovery, progression or a meaningful next decision, not just bigger numbers. The experience should evoke curiosity, reverence, adventure, melancholy and hope. The world feels like **memories of a civilization that forgot its wisdom**.

**Anti-goals:** mobile gacha, generic fantasy reskin, arbitrary grinding, direct imitation of Octopath or FFVII Remake, and philosophy detached from mechanics.

### 1.1 Desired player experience

The player should feel that learning changes what Nova can perceive, what Nova can do, whom Nova can trust, and what the world can recover. Combat, exploration, mentorship, Codex and restoration should reinforce one another rather than behave like unrelated minigames.

**Creative north stars:** discovery before checklist completion; knowledge with consequences; mastery earned through practice; community and memory as tangible progress; philosophical ambiguity without simplistic good/evil scores.

### 2. World, culture and narrative identity

**LOCKED DESIGN.** Taíno/Caribbean and Mesoamerican visual and cultural references are inspiration for a fictional setting, not interchangeable decorations. Research motifs, architecture, material culture and context before adapting them. Do not turn sacred symbols into generic UI wallpaper. `Nahual` is a **working name**, subject to cultural and narrative review.

**UNRESOLVED:** canonical map, timeline, factions, complete plot, cast, endings, and full Nova biography. Early sample quests, enemies and class names are illustrative unless separately approved.

### 2.1 Voice, language and naming

**ACCEPTED CREATIVE DIRECTION:** an archaizing English register for names, lore and certain ritual dialogue, intelligible rather than faux-Shakespearean in every menu. `Wysdom` is deliberate archaic styling. Exact localization, orthography and dialogue style guide remain **OPEN**. Preserve existing technical/repository `Wisdom` spellings until a dedicated compatibility decision.

### 2.2 Cultural design guardrail

Taíno/Caribbean petroglyphs, carvings and materials and broader Mesoamerican references require research and differentiation. They are not one culture; avoid mixing sacred iconography carelessly. Fictional symbols should be original, readable and grounded in the setting.

### 3. Art direction, environments and interface

**LOCKED DESIGN:** HD-2D-inspired pixel characters, layered environments, atmospheric light and depth, with a distinct identity rather than an Octopath copy. Color semantics: jade/turquoise for knowledge, worn gold for mastery, warm stone for history, deep blue for mystery, obsidian/violet for corrupted/forbidden knowledge, restrained red for danger. Shapes: circles and symmetry for order, fractures/broken circles for loss, roots/water for living knowledge.

UI should feel like a ritual/scholarly instrument, not a mobile app. It must remain readable, responsive and controller-friendly. Repeated animations should be brief/skippable. Proposed first visual probes: Codex prototype, combat HUD and environment mood piece. Full UI implementation remains **DEFERRED**.

### 3.1 Visual design language — ACCEPTED DIRECTION

- **Rendering:** HD-2D-inspired layering, pixel-art character silhouettes, environmental depth, atmospheric lighting, weather and restrained post-processing; no direct imitation of Octopath assets.
- **Materials:** weathered stone, carved wood, jade, turquoise, obsidian, aged gold, water, roots and organic growth. Ruin and restoration should be visible in the same place over time.
- **Symbols:** petroglyph-inspired glyph grammar, fractured circles, radial geometry, knowledge fragments and subtle luminous pathways; culturally reviewed, not copied from sacred artifacts.
- **Emotional contrast:** ancient monumental spaces vs intimate living villages; uncertainty and melancholy vs warmth and renewal.
- **Interface:** Codex as ceremonial knowledge map, not a flat wiki; tactile readable panels, controller navigation, skippable animation, minimal HUD noise.
- **Practical visual audit:** silhouettes at actual gameplay scale, contrast for accessibility, localization-safe layout, performance on target hardware and Unity serialization stability.

### 3.2 Visual content still OPEN

Canonical character sheets, enemy silhouettes, animation bible, tileset specification, camera rules, screen resolutions, HUD mockups, sound palette, soundtrack direction, accessibility targets and platform targets. None should be presented as already approved assets.

<a id="recovered-visual-details"></a>
### 3.3 Recovered Visual and Codex Detail

**Documentation-format addendum, 2026-10-31.** These details supplement, rather than replace, the existing consolidated chapters. Sources: [Codex UI concept](#binary-codex), [visual concept](#binary-visual), [local Bible comparison](#binary-local-bible). Their proposed layouts, example content and gameplay bridges remain **PROMISING / OPEN DECISION**, not implemented systems or newly approved work.

Visual acceptance adds: sacred geometry plus living writing, not random particle noise; strong small-scale silhouettes for mentors and major characters; restrained saturation/glow with clear meaning; animation that assembles, aligns and reveals knowledge, versus fractured/misaligned corruption. Optional shadow motifs use missing light/sound or incomplete glyphs, not generic black fog. Logo candidates use readable fragmented lettering, carved forms and a restrained ritual emblem; no generic medieval font, metal-band lettering or glossy mobile treatment. Icon families must share readable stroke/shape logic across knowledge, classes, skills, status and corruption.

Every cultural reference must have an in-world region, school, philosophy or material-history explanation. The visual source explicitly connects care for Taino motifs to Cuban/Caribbean identity. Selective depth of field must not sacrifice gameplay readability. Detailed pixel illustrations for important mentors, bosses or story moments are an option, not a mandatory asset pipeline.

The Codex concept recommends **folded panels + a central knowledge map + altar-like framing**. Alternatives remain the knowledge wheel (distinctive but usability-heavy), folded panels (clearer but risks generic parchment) and altar map (strong identity but risks static presentation). Navigation separates category selection, knowledge map/list and selected-entry details. Relationship lines reveal connections; future search/filter complements rather than defines its identity.

| Proposed entry state | Visual cues | Meaning / possible sources |
|---|---|---|
| Unknown | Empty fragment, erased glyph, incomplete border | Not encountered |
| Glimpsed | Partial silhouette/glyph, dim pulse, a few clues | Rumors, sightings, teacher mentions |
| Discovered | Readable title, basic illustration, restored panel | Basic discovery |
| Studied | Expanded diagram, relationship lines, gameplay facts | Observation, repeated use, tasks, lore or puzzles |
| Mastered / Completed | Closed glyph, jade/gold seal, brief restoration | Fully reconstructed knowledge; thresholds remain open |

Category detail preserved from the concept:
- World Memory: eras, cosmology, catastrophes, foundational teachings and mysteries, expressed as mural/timeline fragments.
- Bestiary: region, behavior, weaknesses/resistances, observed attacks, drops, myths and mastery; proposed stages Encountered -> Defeated -> Studied -> Pattern understood -> Fully mastered.
- Classes & Masters: school, teacher, philosophy, known skills, prerequisites, mastery and hidden paths, with sigil/teacher mark/seal.
- Skills & Teachings: type, cost, effect, mastery condition, required knowledge, teacher and related enemies/field uses.
- Artifacts & Items: origin, effect, crafting/upgrade information, cultural context and hidden properties; annotated relics rather than shop tiles.
- Regions & Sites: map fragment, history, teachers, creatures, artifacts and unresolved mysteries.
- Factions & Philosophies: belief, members, regions, player relationship, conflicts and hidden doctrine.
- Corrupted Knowledge: sealed/overwritten records, incomplete cues and elegant distortion; tempting rather than obviously evil.
- Myths & Echoes: oral traditions, dreams, visions, songs and symbolic fragments, not only dry lore.
- Wisdom Reconstruction: a central fragmented diagram gradually restored, not a bare checklist.

Entry detail composition: title/sigil/state above; illustration at one side, facts and hidden fields centrally, relationships/requirements/sources beside them; progress, teacher notes and myth echoes below. Player annotations are optional future work. Suggested completion vocabulary includes Fragment Restored, Memory Recovered, Teaching Understood, Pattern Recognized and Doctrine Clarified. **OPEN DECISION:** Codex may expose percentages within a reconstruction metaphor; Sanctuary explicitly must not, so these rules must not be merged.

Proposed interaction sounds include stone motion, paper folds, ink strokes, jade clicks, wooden percussion and regional water echoes. Avoid casino/reward chimes and sci-fi beeps. Fast controller navigation, legible hidden information and short/skippable repeated animations take precedence over ornament.

The first Codex prototype proposal is four categories (World Memory, Bestiary, Classes & Masters, Skills & Teachings), one multi-state bestiary entry, class entry, skill entry, unknown entry, one relationship and one reconstruction marker. It remains behind the existing visual-boundary approval gate. Sample-only content is preserved in the archive: Stone-Shell Burrower, Keeper of Echoes and The Perfected Shortcut; none is a newly registered game definition.

### 4. Combat fantasy, rhythm and tactical economy

**CURRENT BACKEND CONTRACT:** `TurnBasedCombatEngine` is the canonical attack/skill authority. Its internal `CombatTurnLifecycle` owns shared turn-start/completion behavior for attacks, skills and explicit player refill (AUD-06 resolved). AUD-07 independently verifies that terminal refill is a complete no-op. Do not introduce a second `CombatResolver` or parallel action engine. Existing flow:

```text
TurnRequest
  -> TurnBasedCombatEngine
  -> CombatRuntimeContext
  -> CombatContext
  -> DamageResolver
  -> CombatEntity / ResourcePool / ResonanceProfile / GameEventLog
```

The foundation covers basic attacks, skills, hit/evasion, criticals, HP, defeat, resource checks, passive hooks, regeneration and status resolution. Opposed stats: Attack/Defense; Precision/Evasion; Critical Chance/Critical Resistance; Debuff Chance/Debuff Resistance.

**Resonance:** `ActiveResonance` pays for skills; `ResonanceReserve` is stored capacity. A successful refill transfers reserve into active and consumes a turn. Skills never silently consume reserve. Empty reserve and insufficient active resonance must be blocked consistently. **Current code uses both resources:** actor-owned Essence remains active; optional runtime Resonance adds a separate skill cost. Essence has not been replaced. **Approved AUD-05 contract (Option B):** the runtime ResonanceProfile is player-owned. Non-player skills with positive ResonanceCost are blocked; zero-cost enemy skills still pay their normal Essence costs. This slice compatibility decision does not prohibit future explicitly approved per-actor resources.

**Status foundation:** Poison, Burn, SkillSeal, RegenBlock and Regen; advanced stacking, immunity and interactions are **DEFERRED**. Current ordering and limitations are in [Combat rules](#combat-rules); full historical formulas and contracts are preserved in [Archive 02](#archive-02).

#### Hybrid combat exploration — PROPOSED, not implemented

A recent design conversation proposed keeping **turn-based tactical authority** while moving *basic attacks* from an `Attack` menu to a direct button, with menus reserved for skills, Resonance refill, items, summons and other strategic actions. The intended audience includes fans of action-oriented games such as Ys and tactical RPGs such as Final Fantasy.

**Hypothesis:** direct basic attacks could create an `ACT → BUILD → DECIDE → EXECUTE` rhythm, with slow/pause tactical selection. Classes might have distinct basic-action styles, and enemy tells/counters could reward understanding over button mashing.

**Not yet decided:** whether free movement, dodging, blocking, animation canceling or real-time enemy actions exist; whether basics generate Resonance; how turns are counted; Speed; DOT/Regen timing; menu pause semantics; action frequency; target selection; input/controller support; fairness between motor skill and strategic builds.

**Architecture gate:** do not code or change combat policy before a dedicated `Combat System Redesign v0` compares conservative, hybrid and action-heavy variants, defines a single time/turn authority and event contract, proves no parallel combat engine, and gains explicit design approval. This proposal **does not override** the existing combat foundation.

### 4.2 Refill as a strategic moment — detailed design record

**ACCEPTED:** active Resonance behaves like MP, reserve tanks provide refill, and refill costs a turn in the current turn-based foundation. Reserve growth *could* derive from character growth, Codex completion and special-skill mastery; exact formulas remain **PROMISING / TO REFINE**.

**PROMISING / TO REFINE:** refill may provide a random buff; an example discussed is “next skill costs less.” A mentor/class skill could unlock `auto-refill` without spending the usual turn. These were explored positively but **have no verified implementation or finalized balance contract**. Decide buff pool, probability, stacking, activation conditions and interaction with the hybrid proposal before coding.

### 4.3 Hybrid combat decision record

Basic attacks on a direct button and tactical menus for skills/items/refill are **PROMISING / TO REFINE**, not approved replacement of the canonical turn engine. A real-time input loop cannot silently create a second authoritative combat system. Compare conservative, hybrid and action-heavy prototypes on timing, enemy fairness, strategic depth and accessibility first.

### 5. Character progression, classes and mentors

**LOCKED DESIGN:** *Classes determine how Nova fights; cognition determines how Nova understands and modifies systems.*

**IMPLEMENTED — DOCUMENTED:** stable class IDs include `Tlamatini` and `Wanderer`; class definitions and registries, skill definitions, skill trees, scoped unlock progress, class-to-tree bindings, mastery profiles, mentor definitions/relationships, teaching evaluation and application, mastered-skill retention across class changes, progression-aware combat validation and runtime/session ownership. See [Progression rules](#progression-rules) and the preserved signatures and workflow history in [Archive 05](#archive-05).

Mentor archetypes: **Lyra** (analysis), **Atlas** (stability), **Gepetto** (crafting/system manipulation) and **Spark** (variance/chaos). The latter two are design directions rather than evidence of complete systems.

Teaching concepts: temporary, fixed and equipped; the entire final persistence/equipment-slot UX is not complete. Mentors may require missions to fix teachings permanently. **DEFERRED:** full class roster, Tlamatini tree content, equipment-taught skills, hybrid mentor teachings, cognition UI and expanded mentor systems.

### 5.1 Learning through equipment — PROMISING / TO REFINE

A Final Fantasy IX-like idea: equipment exposes learnable skills; practice/mastery can retain eligible abilities after unequipping and changing classes. **Proposed refinement:** class identity should still matter and not every mastered skill should necessarily remain usable in every class. **Current implementation differs:** every registered skill mastered in `MasteryProfile` passes retention regardless of class. Restricting that policy requires an approved gameplay change. The existing mastery foundation does **not** establish a complete equipment-learning loop.

### 5.2 Mentor commitments and branching

Mentor teachings may begin temporary; a mentor mission or meaningful commitment can “fix” a teaching in Nova. Paths should alter access to skills and world capabilities, not just grant passive numbers. Exact mentors, missions, slot limits and branching consequences are content/design work, not verified completed quests.

**MUST HAVE / CONCEPT ONLY:** the special mentor mission and ancient teaching in [Echoes of Wysdom](#echoes-of-wysdom) reveal knowledge accumulated before Nova understood its purpose and unlock its village facility. This is a future narrative commitment, not an implemented mentor mission or teaching reward.

<a id="echoes-of-wysdom"></a>
### 5.3 Echoes of Wysdom

**Design status: MUST HAVE / LOCKED DESIGN. Implementation status: CONCEPT ONLY.** Preserved from the creator-approved mission "Echoes of Wysdom: MUST HAVE Design Preservation", received 2026-11-03. This chapter authorizes no implementation, does not alter the canonical audit backlog and does not claim implemented or tested Echoes behavior. The following commitments are future design requirements; explicitly open choices remain undecided.

#### Core Design Commitment

**Knowledge is never truly wasted.**

When Nova receives a valid mastery-progress grant that exceeds the threshold required to master a skill, the unused portion becomes **Echoes of Wysdom**.

Echoes represent knowledge that Nova has acquired but does not yet understand how to apply.

They are accumulated from the beginning of the game, before Nova discovers their purpose.

Example: a skill has 90/100 mastery; a legitimate progress grant awards 35 points.

- Mastery applied: 10.
- Final mastery: 100/100.
- Echoes generated: 25.

The excess must not disappear from the game state, even though it initially appears to disappear visually.

#### Single-Generation Invariant

**MUST HAVE:** A skill may generate mastery-overflow Echoes **only once**, during the valid grant that first takes it to Mastered.

After reaching Mastered:

- The skill cannot receive further mastery-progress grants.
- Repeated use cannot generate additional Echoes.
- Repeated rewards cannot farm additional Echoes.
- Replaying the same reward or restoring a saved game must not duplicate already claimed Echoes within the same progression history.

Do not introduce an infinite resource-generation loop. This invariant is fundamental to the design.

#### Narrative Presentation and Revelation

The generation and transfer clarifications below do not change the hidden accumulation, revelation, teaching or village commitments in this chapter.

**MUST HAVE, before the teaching:** When mastery reaches 100 and produces excess progress, show the unused points as small luminous fragments that gradually fade away.

The animation should suggest the points have been lost or discarded.

The player may notice this recurring phenomenon without immediately understanding its meaning.

Do not reveal the Echoes balance or explain the hidden mechanic prematurely.

**MUST HAVE, the revelation:** A special mentor offers a mission and an ancient teaching about the knowledge that remains after mastery.

Upon completing the mission, Nova discovers the meaning of the apparently lost fragments.

A special animation brings the previously accumulated Echoes back to Nova from across the world, symbolically reclaiming knowledge gained throughout the journey.

The total accumulated Echoes becomes visible.

The visual layer only presents domain events and state; it does not calculate, award or own Echoes.

Provisional teaching name: **The Wisdom That Remains**.

Suggested dialogue:

*"Didst thou believe thy wisdom lost, merely because thou couldst no longer see it?"*

Exact mentor identity, dialogue and teaching name remain OPEN DECISIONS.

#### Village of Memories Facility

**MUST HAVE:** The special teaching unlocks an appropriate restored facility in the **Village of Memories**.

This facility is the primary place where Nova manages Echoes.

The building is not freely placed. Its restoration is tied to knowledge, mentor progression and the established village-restoration philosophy.

Its functions are:

1. Transfer Echoes to eligible skills.
2. Convert a chosen quantity of Echoes into general character XP.
3. Leave any remaining Echoes stored for future use.

The player is never forced to spend the entire balance.

#### Primary Use: Mastery Transfer

After unlocking the special teaching, Nova may spend Echoes to advance another eligible skill.

Requirements:

- Target skill is already legitimately unlocked.
- Target skill has not reached Mastered.
- No bypass of mentor missions, class access rules, skill-tree prerequisites or retention requirements.
- Transferred mastery cannot independently grant permanent access to a skill.
- Transfers cannot generate new Echoes from their own excess. Any transfer must be bounded by the recipient's remaining mastery capacity, with the charged amount and resulting progress explicitly defined.
- The player chooses the target skill and quantity within permitted limits.

**OPEN DECISIONS:** Transfer efficiency, whether some skill families are restricted, minimum quantities, and any future mentor upgrades.

An illustrative 50% transfer efficiency is NOT an approved balance constant.

**LOCKED DESIGN clarification (AUD-10 mission, 2026-11-03):** transfers are strictly bounded by `RemainingMastery = 100 - CurrentMastery`. Never charge Echoes for progress that cannot actually be applied or consume more than required for the accepted transfer. Preserve the unused balance; mastered skills cannot receive transfers. Validate cost and capacity before committing, and keep spending and mastery application atomic. Transfers never generate Echoes and cannot bypass unlocks, mentor missions, class restrictions, tree prerequisites or retention rules, nor independently grant permanent skill access.

Illustrative capacity examples **assuming hypothetical 1:1 transfer efficiency**, not an approved efficiency:

| Current mastery | Available Echoes | Maximum mastery increase |
|---|---:|---:|
| 90 | 50 | 10 |
| 75 | 100 | 25 |
| 99 | 200 | 1 |
| 100 | 200 | 0 |

At 90/100 with 50 Echoes and hypothetical 1:1 transfer efficiency, spend at most 10, reach 100 and retain 40 Echoes. The approved 1:1 **generation** rate does not establish transfer efficiency. If efficiency differs, cost must follow the actual accepted mastery increase with an explicitly defined rounding policy; do not silently discard fractional value or charge for unapplied progress. Efficiency and rounding remain OPEN DECISIONS.

#### Approved Generation Policy and Future Passive

**MUST HAVE / LOCKED DESIGN; implementation CONCEPT ONLY (2026-11-03):** initial generation is 1:1. Each eligible unused mastery point generates exactly one Echo of Wysdom. An eligible ordinary grant of +50 at 90/100 applies 10 and generates 40 Echoes; +20 at 99/100 applies 1 and generates 19; +50 at 100/100 generates zero. Echoes transfers are excluded.

**LOCKED DESIGN:** the future generation policy must support a configurable multiplier with initial/default `GenerationMultiplier = 1`, using the conceptual formula `EchoesGenerated = EligibleOverflow × GenerationMultiplier`. Validate the multiplier, reject negative rewards and use overflow-safe arithmetic. Configuration storage, upgrade sources and numerical limits remain OPEN DECISIONS. The multiplier belongs to the future Echoes policy, never MasteryRecord or ProgressApplied; the mastery result always reports raw, unmultiplied overflow. No runtime configuration, multiplier field/service or unused Echoes policy is introduced by AUD-10.

**FUTURE DESIGN, NOT YET LOCKED MECHANIC:** a mentor may teach a passive that probabilistically multiplies an eligible transition's Echoes. Conceptual sequence:

1. An eligible mastery transition produces raw overflow.
2. The future generation policy reads its configured multiplier.
3. An optional mentor passive may trigger an additional multiplier.
4. Resolve the final award once.
5. Persist the award so repeating or restoring the operation cannot reroll it.

Probability, passive multiplier, mentor identity, unlock conditions and interaction with the base multiplier remain OPEN DECISIONS. Future implementation requires deterministic testing and save/reload exploit prevention. AUD-10 adds no RNG, passive, additional multiplier or reward persistence.

**Eligibility boundary:** distinguish raw unused mastery progress, eligible overflow from an approved grant origin, base generation at the configured multiplier, any passive bonus, and the award actually persisted. Raw overflow alone is not proof of eligibility or a spendable reward. The future application/domain boundary must validate grant origin and claiming. Only the eligible ordinary grant first reaching Mastered may generate once per skill; subsequent grants/use, duplicate rewards and save/restore cannot generate another award within that progression history. The mastery operation fact is not an exactly-once claim token.

#### Secondary Use: General XP Conversion

Nova may voluntarily convert a selected quantity of stored Echoes into general character experience.

This is an alternative of last resort when the player has no eligible skills or simply prefers not to advance any.

The player may:

- Convert some Echoes.
- Convert all Echoes.
- Convert none.
- Preserve the remainder indefinitely.

**Do not lock in `Echoes × character level` as the conversion formula.**

Exact XP rates, scaling, caps and economic balance remain OPEN DECISIONS.

The conversion must not undermine the normal experience curve.

#### Storage and Persistence Commitments

**MUST HAVE:**

- Accumulate Echoes before the mentor revelation.
- Preserve accumulated Echoes across normal save/restore operations.
- Preserve unspent balances.
- Track generation so that each skill's first mastery transition cannot be claimed repeatedly.
- Support partial spending without duplicating or losing the remaining balance.
- Maintain atomicity between mastery progression, Echo generation and claiming/spending.

No arbitrary maximum storage capacity is currently approved.

**OPEN DECISIONS:** Exact persistence representation, migration strategy, ownership model and how to handle historical saves created before Echoes exist.

#### Architectural Boundaries and Current Compatibility

AUD-09 already guarantees overflow-safe mastery accumulation with a maximum of 100. Do not modify that correction.

The future Echoes implementation must calculate eligible overflow safely before capping mastery progress, without reintroducing integer overflow.

Distinguish:

- Mastery progression and retention.
- Skill XP and levels.
- General character XP.
- Echoes as a future spendable resource.

These are not interchangeable concepts.

Do not introduce a parallel mastery engine, duplicated progression system or UI-owned gameplay logic. Do not implement Echoes in this documentation branch.

The current capped mastery APIs do not establish an Echoes ledger, claim history, transfer service or reveal state. The future no-independent-access-grant constraint must be reconciled explicitly with the current retention policy, which admits registered mastered skills; this chapter does not change that policy or invent an eligibility rule. See [progression rules](#progression-rules), [narrative boundaries](#narrative-rules), [architecture](#architecture) and [persistence](#persistence). The [canonical audit backlog](#current-backlog), including AUD-10 through AUD-12, remains unchanged.

### 6. Philosophy, routes, choice and transmutation

**LOCKED DESIGN:** Shared Wisdom emphasizes mentorship, relationships, knowledge transmitted and community. The alternative **Nahual Path** (working name) explores knowledge extracted or forced from reality; it should be intellectually tempting, dangerous and isolating, **not** a generic evil route.

**PARTIAL — DOCUMENTED:** alignment/progression flags and fact-producing evaluators for teaching acceptance, NPC reactions, vendor restrictions, narrative hooks, mission triggers, Codex candidates and transmutation access. These must not mutate unrelated systems implicitly.

**DEFERRED:** full route content, alternate endings, faction/world consequences and economy balancing. `Transmute Lesser Being` is an early concept: defeated enemies may convert into materials/items with failure, resistance and costs. Wisdom Fragments represent **conceptual knowledge remnants**, not generic collectible crystals. Exact economy is unresolved.

### 6.1 Dark route is a design trade-off

The darker path may reduce or remove access to mentor-based permanent skills while opening transmutation/knowledge extraction. This is a **PROMISING DESIGN DIRECTION** that needs explicit economy, reversibility and narrative-cost decisions; do not assert all such restrictions are implemented. The player should understand the allure before facing consequences.

<a id="path-unwalked"></a>
### 6.2 The Path Unwalked: Three Distinct Encounters

**Decision date: 2026-11-03. Design status: MUST HAVE / LOCKED DESIGN. Implementation status: CONCEPT ONLY, not authorized for implementation.** Source: creator-supplied `The_Path_Unwalked_MUST_HAVE.md` (SHA-256 `48e30f70ddd67c6957c58aeb911a1c2d88a71e3c297d28a06a0b6805155aa314`). This integrates the source's creative commitments into the canonical Bible; it does not create a parallel specification or alter the audit backlog. Its reference to AUD-09 records the source's development context, not an instruction to modify that implementation branch or change any audit status.

> **Central question:** What did you sacrifice to become who you are?

#### The Resonance Devourer

**MUST HAVE / LOCKED DESIGN:** a distinct resource-theft encounter testing management and tactical adaptability. **OPEN DECISIONS:** The Resonance Devourer is a provisional name; the following ability names and exact effects are candidates, not implemented mechanics.

An advanced boss that turns Nova's resources against them. This is a test of resource management, active Resonance, reserve tanks, Essence and tactical adaptability. Candidate mechanics:

- **Resonant Leech:** steals active Resonance into an explicitly owned temporary pool.
- **Essence Parasite:** makes Nova's Essence pay for a clearly telegraphed enemy action.
- **Hollow Refill:** intercepts a portion of Nova's reserve-to-active refill.
- **Severed Vessel:** temporarily restricts access to reserve tanks.
- **Resonant Collapse:** converts captured energy into a dangerous, announced attack.

**Important:** Transferring a resource and paying directly from the opponent's resource are different contracts. Any implementation must explicitly model authorized ownership and source-of-cost rules; never bypass AUD-05's current player-owned Resonance protection. Preserve readable telegraphs, counterplay and player agency. The above skill names and exact effects are provisional, but the resource-theft encounter is a MUST HAVE.

#### The Living Archive

**MUST HAVE / LOCKED DESIGN:** a second, distinct encounter testing broad strategic combat mastery. **OPEN DECISIONS:** The Living Archive is a provisional name; exact encounter mechanics and placement remain open.

A powerful being embodying accumulated knowledge, able in principle to access every spell or skill discipline in the game. This is a test of combat mastery, not random skill spam.

- **Phase I:** survey and probe Nova with varied disciplines.
- **Phase II:** recognize repeated tactics and counter predictable patterns.
- **Phase III:** combine normally incompatible disciplines in a climactic examination.

Reuse canonical skill definitions. Design a bounded, legible selection strategy and explicit enemy access policy. Narrative access to the entire catalog does not mean executing every skill in a single fight. A strong candidate for the main story's final boss, though its exact placement remains open.

#### The Path Unwalked / The Unchosen Self

**MUST HAVE / LOCKED DESIGN:** a third, distinct encounter with the opposite-path Nova. **OPEN DECISIONS:** The Path Unwalked / The Unchosen Self are provisional names; exact encounter mechanics and metaphysics are not fixed by these titles.

**MUST HAVE: a secret, optional post-credits encounter with the authentic Nova who chose the opposite life path.** Not a clone, illusion, palette swap, or automatically evil twin.

The protagonist and alternate Nova are equally real outcomes of different choices. Both paths carry defensible truths and meaningful losses. The confrontation must be emotionally and philosophically consequential, asking what Nova sacrificed to become who they are.

- If the player pursued mentors, mastery, bonds and restoration of the Village of Memories, the other Nova followed forbidden transmutation, isolation and power through sacrifice.
- If the player pursued the forbidden path, the other Nova preserved mentorship, community and restoration.
- The opposing Nova's abilities, visual identity, dialogue, worldview and relationship to the village must reflect the path **not chosen**.
- Neither version is declared categorically right or wrong. The other Nova may argue that the player's choices were mistaken, with credible reasons.
- The primary story must have its own satisfying resolution. The alternate-Nova confrontation deepens it **after the credits** instead of replacing it.
- Codex knowledge, choices and discovery may unlock the encounter, but literal 100% Codex completion must not be mandatory.

**Provisional opening line:** *“Thou hast walked thy path to its end. Yet mine remains untold.”*

*“Has recorrido tu camino hasta su final. Pero el mío aún no ha sido contado.”*

#### Narrative Promise and Open Metaphysics

The story is not simply good versus evil. The other Nova is a life the player genuinely might have lived. The player should wonder whether wisdom also existed on the road abandoned. This gives **Fragments of Wysdom** its emotional meaning: no single journey contains every fragment of wisdom.

**Candidate metaphysical twist, NOT YET CANON:** both Novas discover that the world can preserve only one of the two paths. The origin of the two realities, whether reconciliation is possible, endings and final consequences remain OPEN DECISIONS.

#### Structural Promise

Three bosses, three different tests:

1. Resource predator: management and adaptability.
2. Living Archive: broad strategic mastery.
3. Unchosen Nova: identity, choices and sacrifices.

Their final names, lore identities, chronology and balance remain open; the **three distinct dramatic functions** and the **post-credits opposite-path Nova** are locked creative commitments.

#### Architecture and Governance Boundaries

- **Do not implement now.** The narrative record does not authorize scope on AUD-09 or any other implementation branch; the [canonical audit backlog](#current-backlog) remains unchanged.
- No duplicate combat engine, skill registry, resource pool ownership system or progression subsystem.
- Any resource hijacking must be an explicit, approved domain mechanic with regression tests; AUD-05 remains intact.
- The all-spells boss selects from canonical definitions under an explicit enemy authorization policy.
- The other Nova is built from opposite-path narrative/progression rules rather than blindly copying mutable player state.
- Unity only presents domain events and owns no combat rules.
- Exact encounter design, tuning, rewards, quest chain and cultural references require later design reviews.

**This record preserves the creator's MUST HAVE vision, not a completed implementation or permission to add scope to the current branch.** Future work must respect the existing [combat and resource contracts](#combat-rules), [narrative fact-producing boundaries](#narrative-rules), [progression rules](#progression-rules) and [development governance](#governance). The proposed resource interactions and catalog-wide enemy access require later explicit design approval; none overrides the current backend protections.

### 7. Codex, discovery and missions

**IMPLEMENTED — DOCUMENTED:** the V2 narrative foundation favors small evaluators that return facts, tags, hooks and candidate records. Explicit downstream services apply Codex and mission-journal discovery idempotently. No narrative evaluator should directly execute combat, UI, dialogue or reward logic.

**LOCKED DESIGN:** the Codex is where the world reconstructs its memory through Nova. It is more than a wiki: a scholarly, ceremonial, fragmented knowledge instrument. Proposed knowledge domains include world memory, bestiary, classes/masters, skills/teachings, artifacts, regions, factions/philosophies, corrupted knowledge and myths/echoes. Entry stages may progress from unknown to glimpsed, discovered, studied and mastered/reconstructed.

**DEFERRED:** full UI, thresholds, reward bridge, discovery-stage implementation, mission chains, partial objectives and story content. See [Narrative and discovery rules](#narrative-rules) and [Archive 03](#archive-03).

### 7.1 Codex visual experience — PROMISING / TO REFINE

The Codex is a **ceremonial map of reconstructed knowledge** with discovery stages, interlinked topics and subtle animated reveals. Its content may later contribute to Resonance reserve growth and town restoration, but those bridges are not verified in code. Quote from the earlier concept: *“The Codex is not where the player stores information. It is where the world remembers itself through the player.”*

### 8. Villa de las Memorias and Sanctuary

**LOCKED DESIGN / DEFERRED IMPLEMENTATION:** the **Villa de las Memorias** is a restoration of culture, roots, knowledge and function, **not** a free-placement city builder. Learning/mastering teachings can restore or deepen buildings that eventually host shops, mentors, workshops, cooking, alchemy or transmutation. No town implementation until the visual smoke-demo boundary is designed and audited.

**LOCKED DESIGN / DEFERRED IMPLEMENTATION:** a **Sanctuary / Refuge of Reflection** within the restoration fantasy offers quiet, emotionally meaningful remembrance. An NPC may naturally recall a *small selection of actual significant milestones* from the player's journey. Never fabricate remembered events, turn reflection into XP/achievements, show percentages, or force exhaustive recaps. The Codex remembers the world through Nova; the Sanctuary helps Nova remember the journey. Event inputs, persistence and UI ownership remain **UNRESOLVED**.

### 8.1 Restoration, not city construction

Buildings are manifestations of recovered cultural capabilities, not free-placement structures. A mastered skill may restore a workshop, mentor house, kitchen or alchemical/transmutation function. **OPEN:** whether one skill maps to one building, a building upgrade or a shared capability family; how quest gates and visual stages work.

**MUST HAVE / CONCEPT ONLY:** the [Echoes of Wysdom facility](#echoes-of-wysdom) is restored through its special teaching, knowledge and mentor progression, not free placement. It is the primary location for eligible mastery transfers, voluntary general-XP conversion and retaining unspent Echoes; it does not change the Sanctuary's no-XP reflection contract.

### 8.2 Sanctuary as emotional counterweight

**ACCEPTED DIRECTION / DEFERRED:** it recalls a few actual, meaningful player events in natural dialogue. No XP, score, trophy wall, percentages or moral evaluation. **Design maxim:** *“The Sanctuary never evaluates the player. It remembers them.”* It needs a stable milestone/event model before any UI.

### 8.3 Audio, pacing and exploration — OPEN

Music, environmental audio, encounter pacing, exploration layout, travel systems, encounter presentation and interaction affordances require their own design pass. Do not invent decisions from an absence of records.

<a id="systems"></a>
# Part II: Game Systems & Rules

<a id="combat-rules"></a>
## Combat, Resources and Skill Access

**Current source contract:** [TurnBasedCombatEngine](../src/Game/Domain/Combat/TurnBasedCombatEngine.cs) owns the canonical attack/skill sequence. Preserve it rather than adding a parallel resolver. [Archive 02](#archive-02) retains formulas, passive/status contracts and historical acceptance cases.

**Locked future vision, not current execution rules:** [The Path Unwalked's three encounters](#path-unwalked) include resource theft and catalog-wide combat mastery. Resource transfer versus direct opponent-funded costs must have separate approved ownership/source-of-cost contracts; the Living Archive must use canonical definitions with bounded selection and explicit enemy authorization. These concepts do not bypass AUD-05 or implement new enemy capabilities.

Current skill sequence:
1. Reject ended combat or defeated attacker/defender; construct context and apply the player Atlas cost modifier when relevant.
2. Emit `TurnStarted`; reset flags; run legacy debuff turn-start and before-action hooks; stop if the attacker died.
3. If configured, run `ICombatSkillTurnValidator`. Null/blocked results emit `ActionBlocked`, call end-turn and return.
4. Check the per-turn SkillsBlockedBySilence fact, then SkillSeal; the first blocker emits one ActionBlocked, calls end-turn and returns. Legacy Silence hooks declare the fact without emitting diagnostics; basic attacks and refill do not consume the skill-only restriction.
5. Read Resonance cost from the supplied progress definition. A positive cost with Attacker other than runtime.Player emits ActionBlocked / UnsupportedResonanceOwnership, ends the turn and returns before skill cost hooks or spending. Otherwise calculate Essence cost from `SkillProgress`, mastery multiplier and passive hooks.
6. Check optional active Resonance capacity, then spend actor Essence, then active Resonance. Failure ends the turn before skill execution.
7. Apply skill, grant 10 skill XP, process requested cleanse, resolve hit/critical/damage and pending debuffs, then check defender defeat.
8. If combat continues, end-turn regenerates Essence unless blocked, runs after-action debuffs and status tick, checks attacker defeat, emits `TurnEnded` and advances turn.

Blocked validation spends no skill Essence/Resonance and grants no skill XP, but normal end-turn consequences can still occur. Earlier hooks may already have run. Optional validation preserves the two-argument engine's legacy path and its 38-event slice. Do not infer that legacy bypass guarantees structurally malformed requests are safe.

`CombatSkillValidationOrchestrator` composes actor classification, context provision and `CombatSkillUseValidator`; `CombatSkillTurnValidatorGateway` adapts this to the engine validator. Player mode needs UnlockProfile, ClassProfile, MasteryProfile, class registry and skill registry. Enemy mode needs no player progression profiles. Legacy bypass is explicit in context construction. Foreign actors and incomplete contexts fail with structured stage/reason facts. Identity matching precedes canonical registry resolution and full metadata value comparison, then canonical V3 unlock and V4 retention checks. **AUD-04 resolved:** same-ID conflicting definitions are rejected; distinct value-identical copies remain valid. The engine keeps the existing SkillProgress and consumes equivalent metadata without replacing its XP/level state. See the AUD-04 closure for compatibility boundaries and trusted-execution limitations.

`CombatSkillAccessService` delegates to `MasteredSkillRetentionPolicy`: a registered skill is usable when the current class allows it OR it is mastered in `MasteryProfile`. It returns access facts without mutation. A class-allowed skill can still be blocked by the separate V3 required-unlock policy. Tree discovery/unlocking alone grants neither access nor mastery.

**Resource ownership:** Essence belongs to each combat entity. The runtime's optional active/reserve ResonanceProfile is explicitly player-owned; only runtime.Player may pay positive skill costs from it. Non-player requests are blocked before skill cost calculation/spending, effects and XP, even without an optional validator. The engine reports UnsupportedResonanceOwnership independently of whether the player's pool exists or has enough resource. Zero-Resonance enemy skills still pay enemy Essence. `ResonanceRefillAction` preserves its existing explicit player refill contract and separately supplied profile compatibility; it gains no enemy capability or ownership resolver. AUD-06 integrates refill with the canonical shared lifecycle described below; AUD-07 verifies its existing terminal guard without production changes. Future per-actor resources require separate approval. Random refill buffs, auto-refill and reserve-growth bridges remain proposed. Atlas' current recurring cost modifier is not the proposed next-skill refill buff.

**Terminal refill boundary (AUD-07 verified):** after null-argument checks, CombatEnded OR a defeated player OR a defeated enemy rejects with false before resource feasibility or lifecycle execution. Positive, zero, negative and extreme requests all leave resources, HP, statuses, debuffs, flags, turn, ended state, skill XP, passives and events unchanged, including repeated calls and stale defeat states with CombatEnded false. No ActionBlocked or other new event is emitted after terminal admission failure. Public EndCombat can mark two living actors ended, while public ReceiveDamage can defeat an actor without setting the runtime flag; both states are covered. This is verification of the guard introduced by AUD-06, not a new production repair.

**Refill lifecycle (AUD-06 resolved):** null arguments retain their exceptions. After live-combat admission, unaffordable transfers emit the existing empty-reserve ActionBlocked reason without starting a turn. Admitted refill constructs a player context, then shares TurnStarted, flag reset, legacy OnTurnStart/OnBeforeAction and start-defeat detection with the engine. If alive, it transfers reserve to active and emits the existing ResourceSpent event. Shared completion regenerates 5 Essence unless ResourceBlock prevents it, expires legacy debuffs, runs canonical DOT then Regen then status-duration ticking, checks defeat, and only if alive emits TurnEnded and advances once. No skill execution, cost passives, XP, hit/damage resolution or enemy status ticking occurs. Nonpositive amounts and full active pools remain successful resource no-ops that consume a turn and now obey its consequences. Neither SkillSeal nor Silence blocks refill or emits a false block diagnostic (AUD-08 resolved). Start-debuff death prevents transfer and returns false; boundary death follows the transfer and returns true, ends combat and suppresses TurnEnded/advancement. The public bool describes transfer/action completion, not survival. See the AUD-06 closure for event and compatibility evidence.

**Status boundaries:** `StatusEffectCollection` and resolver cover Poison, Burn, SkillSeal, RegenBlock and Regen; legacy `Debuffs`/flags coexist. They are not interchangeable. AUD-08 makes active legacy Silence prohibit skill execution through a per-context internal fact; it does not replace SkillSeal or its status model. Precedence is start-boundary death, configured validator rejection, Silence, SkillSeal, then existing ownership/cost checks. The selected skill blocker emits one ActionBlocked and prevents Apply, cost-passive execution, spending, XP, damage, debuff application and skill-triggered cleanse; normal completion consequences remain. One-turn Silence blocks before expiring during completion. Basic attacks and refill remain permitted. Stacking, immunity and synergy expansion remain deferred. `Defend` and `Wait` enum values have no public TurnRequest factories; they are incomplete contracts, not reproduced public-action failures.

**Canonical resource bounds (AUD-11 resolved):** ResourcePool clamps construction maximum to at least zero and starts full. TrySpend rejects negative or unaffordable amounts before subtraction, preserves Current/Max on rejection and accepts zero as a successful no-op. Successful spending never increases Current. Regenerate ignores nonpositive values and widens positive addition before capping at Max. These operations preserve `0 <= Current <= Max` and `Max >= 0` for sequential use from construction. Essence and active/reserve Resonance reuse this container; no resource resolver, cost formula, event or refill lifecycle was changed. See the AUD-11 closure for reproduction and compatibility limits.

<a id="progression-rules"></a>
## Classes, Mastery, Mentors, Trees and Equipment

Source ownership: `src/Game/Domain/Classes/`, `src/Game/Domain/Skills/`, `src/Game/Domain/Cognition/` and `src/Game/Domain/Equipment/`. Full overload contracts and chronological workflow evidence are in [Archive 05](#archive-05).

| Contract | Current rule | Boundary / edge case |
|---|---|---|
| ClassDefinitionRegistry | Stable ClassId lookup; valid registration; duplicate IDs rejected without replacing the original | ClassProfile owns assignment, not registries or mastery |
| SkillDefinitionRegistry | Stable skill ID registration/resolution; null/invalid/duplicate definitions fail safely | Shared content catalog, not ownership or canonical execution enforcement by itself |
| MasteryProfile / MasteryRecord | Registry-validated integer progress in 0..100; positive accumulation widens before capping; ranks are Unranked at 0, Practiced at 1..99 and Mastered at 100 | AUD-09/AUD-10 resolved; detailed operation returns requested/applied/raw-overflow/first-mastery facts; bool APIs delegate; no automatic unlocks, bonuses or Echoes |
| SkillProgress | XP/level-based skill progression with its own IsMastered and cost/effect modifiers | Distinct from retention mastery; no automatic synchronization between the two models |
| MentorDefinitionRegistry | Stable MentorId, valid name/definition, duplicate rejection | No teaching or mission execution |
| MentorRelationshipProfile | Discovery, trust/progress and availability facts; registry-aware APIs; repeated discovery is idempotent | Does not replace the legacy cognition teaching/slot profile |
| MentorTeachingService | Checks mentor/skill registries, discovery, availability and required trust | Returns taught skill ID, optional mastery grant and stable block reasons; does not mutate mastery |
| MentorTeachingApplicationService | Explicit application of available teaching through skill registry; nonpositive grant rejected with NoProgressGranted | AUD-10 resolved: ProgressApplied is actual bounded delta; no mentor mutation, skill grants or mission starts; repeat after mastery safe |
| MasteredSkillRetentionPolicy | Registered AND (class-allowed OR retention-mastered) | Read-only; no class needed for registered mastered skills; class eligibility restrictions are future design |
| SkillTreeDefinition | Stable SkillTreeId; stable node IDs referencing registered skills | Duplicate nodes rejected; every prerequisite must already exist in that same tree |
| SkillTreeUnlockPolicy | Requires known/discovered node and unlocked prerequisites in that tree scope | Prerequisite-first insertion rejects forward/self references and prevents incremental cycles; not a full arbitrary graph import validator |
| SkillTreeUnlockApplicationService | Evaluates policy before recording unlock; blocked/duplicate applications are safe | Recommended mutation path; internal RecordUnlockedNode exists for trusted assembly restore/application paths, not a public grant API |
| SkillTreeProgressProfile | Discovery/unlock facts keyed by SkillTreeId and node ID | Same node ID in two trees is independent; compatibility paths are not permission to merge scopes |
| ClassSkillTreeBindingRegistry | One validated class-to-registered-tree binding per class; duplicates rejected | Resolution after class switch changes neither tree progress nor skill ownership |
| Equipment | Definition/loadout/stat aggregation and TaughtSkillId metadata | No complete equip-to-learn-to-master-to-retain loop; no automatic equipment mastery grant |

Legacy `SkillProgress` mastery uses its own maximum-level test, 0.8 cost multiplier and +0.15 effect bonus. Retention mastery uses the separate 100-point record. A future unification or eligibility restriction needs an explicit migration/design decision; do not conflate terminology.

**MUST HAVE / CONCEPT ONLY:** [Echoes of Wysdom](#echoes-of-wysdom) preserves eligible excess from a skill's first mastery transition, once per skill, with no repeated-grant or transfer-overflow farming. Its resource, general character XP and skill XP remain distinct from retention mastery. No Echoes award, spend or access-rule change is implemented or authorized here; AUD-09's safe cap remains intact.

Current teaching and tree results are lightweight facts. Positive teaching grants may be applied explicitly, while tree unlocks and class bindings do not grant skills, rewards or mastery. No mentor ranks, complete skill roster, item skill learning or reward trees are certified by these foundations.

<a id="narrative-rules"></a>
## Narrative, Codex, Missions and Unlocks

**Locked future narrative, not implemented progression:** [The Path Unwalked](#path-unwalked) requires a secret, optional post-credits confrontation with an equally real opposite-path Nova, after a satisfying primary-story resolution. Its abilities, presentation and narrative facts must reflect the path not chosen, not a blind copy of mutable player state. Codex knowledge, choices and discovery are candidate gates; literal 100% Codex completion cannot be mandatory. Exact gates, metaphysics and endings remain OPEN DECISIONS.

The fact-producing chain reads player alignment and runs `FirstNahualTeachingEvaluator`, `TransmutationAccessPolicy`, `NpcReactionService`, `NarrativeHookService`, `MissionTriggerService` and `CodexEntryService`, with candidate deduplication. Results are acceptance/rejection, tags, preview reactions, hooks and candidates, not implicit combat, dialogue, inventory or reward changes.

Explicit `CodexDiscoveryService`, `MissionDiscoveryService` and `DiscoveryOrchestrationService` apply candidate facts idempotently by stable entry/trigger ID. Enriched Codex records preserve source hooks/triggers, type, concept and tags; mission leads preserve priority/family/source metadata. Active mission and unlock foundations extend journal facts, but are not a complete quest or economy system. Legacy mission evaluation/rewards are not automatically coupled to the journal and may replay numeric rewards without a claim guard.

**Codex persistence (AUD-03 resolved):** legacy string-based discovery/completion and enriched records now survive valid snapshot/JSON/restore round trips. Optional schema-1 ID lists preserve discovery and completion without inventing enriched metadata. Existing first-discovery precedence and merge behavior remain; discovery stages, completion reward mechanics and UI are not added. See the fact-ownership matrix in the AUD-03 closure.

Alignment/Nahual gates and vendor reaction facts do not implement enemy-to-item conversion, shops, loot, crafting, cooking, alchemy or world consequences. Legacy cognition teachings/slots and newer mentor relationship facts have separate responsibilities and no automatic mentor-mission bridge. Detailed narrative mappings and edge cases remain in [Archive 03](#archive-03); the historical V2 slice design is in [Archive 01](#archive-01).

<a id="recovered-system-decisions"></a>
## Recovered Historical Design Decisions

This is a provenance/decision supplement, not a replacement roadmap. [Iteration 2](#binary-iteration2), the [foundation PDF](#binary-foundation-pdf), the [local Bible](#binary-local-bible) and retained [conception conversation](#binary-conversation) were compared with the active chapters and eleven technical archives.

**Narrative/economy constraints retained:** the early Nahual discovery may use a hidden ruin, forbidden fragment, isolated NPC, ancient device or cognition anomaly; no two complete campaigns are required by that slice. Transmute Lesser Being was proposed as chance-based conversion of a defeated enemy with failure, unstable/corrupted outputs, immune enemies and highly resistant bosses. It must not generate infinite value or invalidate ordinary vendors/loot. These are design constraints for deferred execution, not claims that current access policy implements conversion. Wisdom Fragments may eventually fix teachings, upgrade cognition, unlock forbidden nodes or contribute to Codex progress; no numeric economy is approved.

**Teaching concepts:** temporary knowledge is active under the relevant cognitive archetype; fixed teaching is retained; equipped fixed teaching is selected into limited active slots. Historical user choices favored free archetype switching outside combat, slots rather than all teachings simultaneously active, and a mixture of narrative, mechanical challenges and puzzles for mentor missions (conversation pp. 326-341). The proposed initial one-archetype/two-fixed-slot balance is not a new universal runtime invariant. Lyra's La Grieta del Juicio teaches analysis before control; Atlas' El Problema de las Tres Certezas teaches stable choices; El Relicario Partido is a historical sidequest example. No new mission bridge is authorized here.

```text
Nova
  + Class: current combat discipline
  + Skills: learned / equipped / mastered concepts
  + Codex: discovered world knowledge
  + Cognition
      + active archetype and affinity
      + temporary teachings
      + fixed teachings selected into slots
      + proposed cross-archetype synthesis
```

This is the historical conceptual relationship, not a replacement object model. Current actual ownership remains Part III.

| Recovered topic | Source | Current disposition |
|---|---|---|
| Node-based exploration and soft exponential growth preference | Conversation pp. 29-35 | Historical preference; map implementation, exact XP formula and current applicability are OPEN DECISION |
| Weapon, Armor, two Accessories; impactful extensible builds | Conversation pp. 40-42; foundation PDF pp. 6-7 | Existing equipment direction, not authorization for a loot system |
| Recoverable death losses by returning to the location | User choice p. 41; proposal pp. 45-46 | Previously omitted design direction, OPEN DECISION; loss percentages, durability, timers and multiplayer theft were assistant proposals, not locked requirements |
| Prerequisite mastery versus unlocking prerequisite nodes | User request p. 49; alternate proposal p. 53 | OPEN DECISION: current tree policy requires unlocked nodes, not mastered prerequisite skills; preserve both historical proposals |
| Item-taught skill scaling and equipped-skill limits | Conversation pp. 51-54 | Proposed balance constraints, not current automatic grants or a completed loadout system |
| Strategic AI reads state rather than player inputs/class labels | Conversation pp. 70-81 | Deferred design: threat, phase, risk tolerance and action-selection responsibilities; engine executes the selected intent |
| New classes, adaptive enemies and Xolotl example | Conversation pp. 52-81 | Historical content candidates needing cultural/design/balance review; not canon or implemented roster |
| Combat rewards create access, understanding and choices | Conversation pp. 82-86 | Consistent direction; conditional spawns/drops and rule-changing fragments remain proposals |
| Codex completion rewards at 25/50/75/100 percent | Conversation pp. 236-253 | Historical table retained below; thresholds/effects require reconciliation with current unlock facts, never an implied full reward system |
| Nova synthesis tracks | Conversation pp. 315-325 | Sintetizador, Catalizador, Conductor are proposed specializations; permanency/stances remain OPEN DECISION |
| Mentor doctrine versus physical party members | Conversation pp. 315-327 | Separate cognition/class layers and mentor narrative presence; no party-combat expansion authorized |

The early composite power formulas and 60/25/10/4/1 rarity weights are historical examples, not approved balance. The conversation itself contradicts "level only grants access" with level contributing to power (pp. 35-36); keep this OPEN DECISION rather than choosing silently. Likewise the old PDF's universal mastery language and Silence guarantee do not override the two mastery models or AUD-08.

Design acceptance requires knowledge-driven choices, distinct class/cognition/mastery/Codex/alignment responsibilities, testability outside presentation and meaningful world reactions. No isolated reward economy, uncontrolled early power spikes or currency proliferation. Future content hooks from Iteration 2 include forbidden teachers, corrupted cognition, alternate endings, alignment-based cities, transmutation crafting, enemy harvesting and cognition instability; all remain deferred.

<a id="architecture"></a>
# Part III: Software Architecture

## Domain and Application Boundaries

Domain/application are pure C#; Unity adapts inputs, storage paths and presentation. Combat, cognition/progression, missions, Codex, inventory/equipment, narrative and presentation keep explicit ownership. Use focused services, data/results, events and adapters; avoid hidden cross-domain mutation and parallel engines.

**Future design boundary:** [Echoes of Wysdom](#echoes-of-wysdom) requires safe pre-cap overflow accounting and atomic progression/generation/claiming/spending without a parallel mastery engine. Ownership remains an OPEN DECISION. Its fading-fragment and revelation animations only present domain state/events; presentation cannot calculate or award the resource.

The solution builds `src/Game/Game.csproj` and the smoke runner. Domain and Application currently share one `net10.0` assembly, so folders express boundaries rather than enforcing them. No Unity dependency was found in these layers. `src/Game.Unity/` is outside that solution: .NET smoke success does not verify its compilation or runtime compatibility in Unity.

Two event contracts coexist: combat `GameEventLog`/`GameEventType` records and typed domain discovery/unlock events. Neither is a complete UI/milestone bridge. EquipmentLoadout/EquipmentProfile and EquipmentStatAggregator/modifier helpers include delegating compatibility APIs, not independent gameplay engines. The actual duplication risks are divergent state paths and validation policies, not merely repeated names.

<a id="runtime-ownership"></a>
## Runtime Ownership and Integration

The neutral application path is:

`ProgressionRuntimeDemoBootstrap -> PlayerSessionBootstrap -> PlayerSessionComposition -> LoadedPlayerSession -> PlayerProgressionRuntimeState`

The wrappers delegate to the same session/state; they must not allocate competing progression owners. `ProgressionAwareCombatEngineFactory` creates progression-aware engines through the existing gateway/orchestrator/validator path. Registries contain shared content definitions; mutable class-tree bindings and player profiles are session-owned. Combat entities, runtime context, events and passive pipelines are combat-scoped. Definitions may be shared; mutable binding registries must not leak between sessions.

The term `LoadedPlayerSession` does not imply automatic disk loading. Composition accepts prepared state; filesystem loading and schema compatibility remain explicit persistence responsibilities. A future visual adapter consumes state/results/events through the demo/bootstrap boundary, not by constructing TurnBasedCombatEngine, new sessions or duplicate profiles. `FoundationVerticalSlice` remains a deliberate compatibility path; migration requires its own authorization.

**P9.1 through P9.4 implemented:** `CombatEncounterComposition` receives that existing LoadedPlayerSession, one supplied complementary PlayerProfile and single-use CombatEncounterPreparation. It constructs one runtime and calls the session's existing engine factory, retaining prepared encounter dependencies behind internal access. Its public handle exposes start/lifecycle/failure diagnostics, detached `GetSnapshot()` reads, value-only player intents, explicit enemy basic-attack advancement and indexed immutable event batches. The same owner schedules alternating opportunities; the existing engine alone resolves actions. See [ownership, binding and start contracts](#combat-encounter-composition), [read-model contracts](#combat-readonly-projections), [command/observation protocol](#combat-intent-event-boundary) and [turn-flow contracts](#combat-turn-flow). No initiative, availability evaluation, enemy AI or visual integration is implied.

<a id="persistence"></a>
## Persistence, Serialization and Compatibility

**Future requirement, not current save support:** [Echoes of Wysdom](#echoes-of-wysdom) must preserve hidden and visible accumulated balances, per-skill generation/claim history and partially spent remainders across normal save/restore without duplication or loss. Representation, migration and pre-Echoes historical-save handling remain OPEN DECISIONS; this documentation adds no DTO, schema change or atomicity guarantee to the existing pipeline.

Current schema is **1**, centralized in `V2ProgressSnapshotVersions`; this consolidation changes neither DTOs nor schema. Pipeline:

`Profiles -> V2ProgressSnapshotService -> V2ProgressSnapshot -> JSON serializer -> file repository`

Snapshot facts include enriched Codex entries, Codex discovered/completed IDs, mission leads, active missions, unlocks, class profile, mastery records, mentor relationships, scoped/legacy skill-tree progress and class-tree bindings. Serializer uses System.Text.Json; malformed/unsupported data fails through existing safe APIs or documented exceptions. Older JSON missing optional collections restores empty/default facts where existing overloads allow. Missing class data is not a request to erase an assigned class. There is no general migration engine.

**Codex:** schema 1 adds optional `CodexDiscoveredEntryIds` and `CodexCompletedEntryIds`; `CodexEntries` retains its existing enriched shape. Missing/null lists normalize to empty lists, which mean no additional facts, never deletion. Restore validates Codex IDs/records before any mutation, then merges enriched records, discovered IDs and completed IDs in that order. Completion implies discovery through the existing domain method. Duplicate facts remain idempotent; previously known IDs retain their existing enriched metadata or lack of metadata. No completion reward, unlock evaluation or discovery event service is invoked. All existing constructors remain available. This is backward-compatible reading, not a guarantee that older binaries can rewrite new saves without losing the new fields.

**Mastery:** persist skill ID, progress and rank; restore validates skill IDs through SkillDefinitionRegistry and recreates progress-derived mastery. Unknown IDs fail before that subsystem applies supported records. Mentor snapshots persist ID, discovered/trust/availability; restore uses MentorDefinitionRegistry. Bindings persist class/tree IDs and validate both against class and registered tree definitions. Individual restore methods retain their current merge/idempotency behavior; they are not whole-player replacement transactions.

**Skill-tree compatibility policy:**
- New scoped snapshots store tree IDs plus discovered/unlocked node IDs. Same node ID across trees remains independent.
- Nonempty legacy unscoped progress and nonempty scoped progress in one snapshot are semantically invalid.
- Legacy-only progress remains supported with an explicit tree or an unambiguous single registered tree, according to the restore overload. Do not guess across multiple trees.
- Older JSON without tree progress is safe empty/default data.
- No-registry validation retains structural compatibility. Registry-aware validation additionally checks known scope/node IDs, unlocked nodes also discovered, and unlocked prerequisite closure.
- Coordinator preflight now enforces these semantic checks for scoped and legacy tree progress (AUD-02 resolved). File loading still uses structural validation, and direct individual restore methods are unchanged; a valid JSON file alone is not proof of valid gameplay state.

**Coordinator:** ProgressRestoreCoordinator is the canonical responsibility-based API; V4ProgressRestoreCoordinator remains a compatibility adapter (see [P8A](#restore-api-naming)). It first rejects null real destination profiles with `ArgumentNullException`; the binding overload also rejects a null destination binding registry. It then preflights class, mastery, mentor relationships, tree progress and bindings using temporary profiles and a copied binding registry. Before temporary tree restoration, it reuses V2ProgressSnapshotValidationService to check tree structure, discovery and prerequisite closure. The single-tree overload uses its explicit definition for legacy progress or one matching scoped tree. The binding overload uses registered definitions for scoped progress and its explicit tree for legacy progress. No missing definitions are synthesized. Semantic rejection sets `SkillTreeProgressRestored=false`, `SkillTreeProgressFailureReason=ValidationFailed` and `FullyRestored=false`. If any preflight fails, it returns structured subsystem facts without applying target profiles. Successful apply order remains:
1. ClassProfile.
2. MasteryProfile.
3. MentorRelationshipProfile.
4. SkillTreeProgressProfile.
5. ClassSkillTreeBindingRegistry, when that overload is used.

The result retains ClassRestored, MasteryRestored, MentorRelationshipsRestored, SkillTreeProgressRestored, ClassSkillTreeBindingsRestored, FullyRestored and subsystem reasons/messages. **AUD-12 resolved:** preflight feasibility is local, never an applied flag. Any rejected preflight returns all five restored flags false; invalid subsystems report ValidationFailed, feasible requested subsystems report NotApplied. After successful preflight, flags reflect each real restore operation's return, including supported no-op/idempotent completion; a false return reports ApplicationFailed. ClassSkillTreeBindingsRequested identifies overload participation: without bindings, the binding restored flag is false with no failure, and FullyRestored evaluates the four requested profiles. Existing public result constructors retain their historical defaults/mappings. AUD-01 destination guards and AUD-02 semantic tree checks remain intact. **It is not a guaranteed atomic transaction:** application remains sequential with no rollback or thread-safety guarantee; a failed subsystem may already have partial changes, and exceptions still propagate. Existing individual restore methods are unchanged.

**File boundary:** V2ProgressSnapshotFileRepository validates, writes a temporary file and replaces/moves the main file after the write, with backup support and fallback for missing/corrupt/unsupported main data. It uses structural validation without a complete content registry. Atomic file replacement does not imply semantic integrity, durable recovery under every filesystem/power-loss condition, multi-writer coordination, or atomic in-memory restore. Encryption, compression, slots, cloud and autosave are not implemented here.

**Coverage limits:** complete cognition, inventory, equipment, wallet and alignment persistence is not supplied by this snapshot pipeline. Generic PlayerSaveData/ISaveRepository are not a complete mapped player save. UnityV2SaveService uses a narrower Codex/mission/unlock path; it has not been upgraded to a complete progression save, and its Exists check only considers the main file. Preserve stable IDs and serialization names; V2 API renames need a dedicated compatibility task. See [Archive 04](#archive-04) and [Archive 05](#archive-05) for full historical signatures.

<a id="tooling"></a>
## Historical Tooling and Unity Boundaries

`tools/SnapshotGenerator/Generate-NovaFoundation.ps1` and its modules are historical V1/V2 snapshot reproduction tooling, not the current build or development workflow. The consolidated entry selects VerticalSliceV1 or VerticalSliceV2; V2 adds the Nahual module. The old sequential command examples remain archival, not instructions to regenerate this checkout.

Inspection found documentation writers in modules 00 (master specification, backlog, generator usage), 05 (structure notes) and 06 (backlog). They would recreate competing historical documents. **Do not run generators over this repository.** The entry point's `-Force` recursively deletes its target before generation. If historical reproduction is explicitly authorized, first verify an isolated disposable target outside this checkout; never interpret the old README as permission to overwrite production. No generator was executed or rewritten in this consolidation. Any future operationalization needs a separate tooling task.

The two tools README files are navigation-only pointers, not independent specifications. Historical usage prose is retained in the source scripts and [Archive 10](#archive-10).

Unity work must preserve .meta/GUIDs, scenes, prefabs, ScriptableObjects and serialized references. Do not casually rename/move assets or hand-edit generated YAML. The old instruction to copy source into Assets/Scripts is historical, not a validated integration procedure for the current .NET target. No Unity project/assets/editor run was established in this audit. An approved adapter task must define compatible assembly/build boundaries, inspect affected scenes/assets and verify Play Mode before claiming playable integration.

<a id="implementation-status"></a>
# Part IV: Implementation Status

## Evidence-Based Ledger

One implementation status per row; VERIFIED is deliberately limited to the named backend scenario, not the whole feature. Source paths are repository-relative; test files are under `src/Game.SmokeTests/`. Current execution counts are recorded below. The original documentation consolidation repaired no findings; subsequent verified corrections are recorded explicitly in Part V.

| Area / scenario | Design maturity | Implementation status | Source evidence | Test evidence / known limit |
|---|---|---|---|---|
| Legacy slice contract | Accepted compatibility | VERIFIED | `src/Game/Slice/FoundationVerticalSlice.cs` | V1SmokeTests: 2 assertions, 38 events |
| Attack/skill foundation | Accepted | PARTIAL | TurnBasedCombatEngine, CombatTurnLifecycle, DamageResolver, StatusEffectResolver | Original 100 V3, 119 AUD-06, 247 AUD-07 and 220 AUD-08 assertions pass; reproduced combat findings resolved, not complete combat/content certification |
| Resonance active/reserve transfer | Accepted | PARTIAL | `src/Game/Domain/Combat/Resources/`, CombatTurnLifecycle | Player-only ownership verified by AUD-05; shared refill turn/status lifecycle verified by AUD-06; terminal no-op boundary verified by AUD-07 |
| ResourcePool spend / bounded regeneration | Accepted foundation | VERIFIED | `src/Game/Domain/Combat/Resources/ResourcePool.cs` | AUD-11 negative/zero/extreme spending and bounded regeneration; serial pool invariants, not complete resource-economy validation |
| Registry/class/mastery/mentor operations | Accepted foundation | PARTIAL | `src/Game/Domain/Classes/` | AUD-09 safe mastery and AUD-10 actual delta/operation facts verified; Echoes economy remains CONCEPT ONLY |
| Scoped tree isolation and policy application | Accepted foundation | VERIFIED | SkillTreeProgressProfile, SkillTreeUnlockApplicationService | Two-tree same-node workflow; not arbitrary corrupt-save integrity |
| Class-tree binding / supported JSON round trip | Accepted foundation | VERIFIED | ClassSkillTreeBindingRegistry, V2ProgressSnapshotService | Binding save and class-switch workflows; no automatic grants |
| Retained access and validated combat composition | Accepted foundation | VERIFIED | CombatSkillUseValidator, CombatSkillTurnValidatorGateway | AUD-04 canonical metadata regressions and mentor-retention engine flow; explicit legacy/enemy paths are outside player authorization |
| Shared session/runtime ownership | Accepted foundation | VERIFIED | `src/Game/Application/Runtime/`, `Bootstrap/`, `Integration/` | ProgressionRuntimeBootstrapSmokeTests; not automatic disk loading or Unity |
| Encounter composition / exactly-once start | Accepted P9.1 scope | VERIFIED | CombatEncounterComposition, CombatEncounterPreparation, PreparedCombatSkillBinding, CombatEncounterState | 156 composition assertions, including real-engine class/mastery workflow; trusted single-threaded ownership, no visual facade or automatic persistence |
| Detached combat observations | Accepted P9.2 scope | VERIFIED | CombatEncounterComposition.GetSnapshot and seven immutable application read models | 149 projection assertions; no mutable domain references, caching, action availability, commands or Unity HUD |
| Combat intent and event boundary | Accepted P9.3 scope | VERIFIED | Existing owner SubmitIntent, ReadEvents and ReadStartEvents | 174 assertions; authoritative engine/refill delegation, detached receipts, explicit consumer cursor, reentry/fault guards; no gameplay rule changes |
| Encounter turn flow | Accepted P9.4 scope | VERIFIED | Same owner gates SubmitIntent and AdvanceEnemyTurn; immutable flow facts | 336 assertions; player/enemy basic-attack alternation, lifecycle-evidenced consumption, terminal/fault suppression; no Domain changes |
| External headless presentation workflow | Accepted P9.5 verification | VERIFIED | Separate non-friend executable drives public P9.1-P9.4 APIs | 230 external assertions; legitimate victory/defeat, consumer cursor recovery and partial faults; no production changes or Unity claim |
| Narrative evaluators and candidate discovery | Accepted foundation | PARTIAL | `src/Game/Domain/Narrative/`, `Codex/`, `Missions/` | V2SmokeTests; complete quests, dialogue and route consequences absent |
| Snapshot/file persistence | Accepted foundation | PARTIAL | `src/Game/Domain/Progress/` | V2/progression save cases pass; AUD-01/AUD-02/AUD-03/AUD-12 resolved; no general transactional rollback guarantee |
| Equipment-taught skill metadata | Promising full loop | CODE FOUNDATION | `src/Game/Domain/Equipment/EquipmentDefinition.cs` | TaughtSkillId is not a verified equipment-learning workflow |
| Unity persistence adapter | Accepted boundary | NOT VERIFIED | `src/Game.Unity/Persistence/` | Outside .NET solution; narrower profile coverage; no Editor/Play Mode run |
| Refill buffs, auto-refill, growth bridges | Promising | CONCEPT ONLY | Conversation design | No approved full rules or implementation evidence |
| Villa, Sanctuary, hybrid combat | Deferred implementation | CONCEPT ONLY | Part I | Require design/audit gates; no production implementation certified |
| Full playable game | Accepted vision | NOT VERIFIED | Backend foundations only | No verified visual/content/audio/playable loop |

**Missing long-term systems:** full class and mentor content; cognition UI; equipment-learning loop; complete quest/event/reward lifecycle; item/equipment catalogs, loot and economy; transmutation execution; restoration and reflection; visual/game-loop adapters; presentation, audio, accessibility, localization and platform decisions. These are not authorized by this list.

<a id="verification"></a>
## Verification Commands and Current Evidence

Run from the repository root using the solution's .NET SDK:

```powershell
dotnet restore Fragments-of-Wisdom.sln
dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal
dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build
dotnet run --project src/Game.PresentationBoundary.SmokeTests/Game.PresentationBoundary.SmokeTests.csproj --no-build
git diff --check
git status --short
```

| Reporting group | Assertions |
|---|---:|
| V1 Legacy Slice | 2 |
| Shared Domain Foundations | 332 |
| V2 Narrative / Progression / Persistence | 371 |
| V3 Combat / Skills / Resonance / Status / Stats | 686 |
| Player Progression / Combat Validation | 3186 |
| **Core smoke subtotal (Game.SmokeTests)** | **4577** |
| External Presentation Boundary (separate executable) | 230 |
| **Combined verification total (both commands)** | **4807** |

Legacy V1 events: **38**, a separate guardrail, not additional assertions. The Progression integrated-workflow subsection contains **186**, AUD-01 restore destination integrity contains **68**, AUD-02 semantic skill tree restore contains **139**, AUD-04 canonical skill definition contains **199**, AUD-05 player Resonance ownership contains **175**, AUD-09 overflow-safe mastery contains **186**, AUD-10 actual mastery delta contains **373**, AUD-12 restore result integrity contains **518**, P8A restore API compatibility contains **58**, P8B combat diagnostic compatibility contains **138**, P9.1 encounter composition contains **156**, P9.2 combat projections contain **149**, P9.3 combat intent boundary contains **174**, and P9.4 encounter turn flow contains **336**, all already included in 3186. Historical V4/V5 labels and earlier counts are not current reporting groups.

The V2 group includes **107** AUD-03 Codex round-trip assertions; they are included in 371, not an additional reporting group.

Shared Domain Foundations contains the original **16** domain-event assertions plus **316** AUD-11 resource-pool assertions. V3 contains the original **100** assertions plus **119** AUD-06 refill lifecycle, **247** AUD-07 terminal boundary and **220** AUD-08 Silence integrity assertions. The unchanged core suite still reports 4577; P9.5 independently adds 230 external-consumer assertions, for 4807 across both required runs. Solution build compiles both executables but does not run either automatically. Earlier closure counts below remain historical evidence. AUD-08 corrects one AUD-06 expectation that previously required a misleading Silence diagnostic on refill; the other AUD-06 assertions and its count remain intact.

**Executed audit baseline (2026-10-30):** implementation `45fb4548ab18984940471b79d5643e16f5aa50f4`, Bible v3 `c11c74d1317f8ca279d8c0698b88d4af96350f94`; restore/build succeeded with 0 warnings and 0 errors; smoke 899 and V1 events 38; diff checks passed. Twelve additional in-memory probes against the compiled assembly reproduced the findings in Part V. They were not added to the suite and must not be counted as smoke assertions.

**Consolidation rerun (2026-10-31):** restore exit 0; build exit 0 with 0 warnings and 0 errors; complete smoke exit 0 with the historical 899 total (Progression 517, other groups unchanged). The first sandboxed smoke attempt failed at File.Move in the file-repository tests with UnauthorizedAccessException (exit -532462766); the identical command passed with expanded execution permission, without code/test changes. The twelve probes are preserved historical execution evidence, not claimed as newly rerun in this docs-only pass.

**AUD-01 correction (2026-11-01):** latest main baseline `634085f` passed 899 assertions and 38 V1 events. The committed regression was first executed against unchanged production code and failed on the class mutation before null mastery rejection. After the correction, the full suite passed 967 assertions (Progression 585, other groups unchanged); restore/build exit 0, build 0 warnings/errors, smoke exit 0, and `git diff --check` exit 0. See the AUD-01 closure below for exact reproduction and environment limits.

**AUD-02 correction (2026-11-02):** latest main baseline `c9fd0f2` independently passed 967 assertions, including all 68 AUD-01 assertions, and 38 V1 events. The new regression failed against unchanged production code: standalone validation false, coordinator `FullyRestored=true`, destination changed, child unlocked and prerequisite still locked. After the correction, the full suite passed 1106 assertions (V2 264, other groups as above); restore/build/smoke/diff-check exit 0, build 0 warnings/errors. See the AUD-02 closure below for scope and limitations.

**AUD-03 correction (2026-11-02):** latest main baseline `e74eb52` independently passed 1106 assertions and 38 V1 events. The new regression failed before production edits: discovered IDs fell from 4 to 2, legacy completion was false and both enriched records survived. After correction, 107 new V2 assertions pass and the full suite passes 1213; restore/build/smoke/diff-check exit 0, build 0 warnings/errors. AUD-01 and AUD-02 regressions remain unchanged and green.

**AUD-04 correction (2026-11-02):** latest main baseline `98ec42c` independently passed 1213 assertions and 38 V1 events. Before production edits, the new real-engine regression failed: weaker same-ID definition allowed, turn 2, Essence 14, active Resonance 4, enemy HP 94, skill applied and XP 10. After correction it is blocked before costs/effects/XP; 199 new Progression assertions pass, full suite 1412, V1 events 38. Restore/build/smoke/diff-check exit 0; final build 0 warnings/errors. AUD-01 through AUD-03 regression sections remain unchanged and green.

**AUD-05 investigation and correction (2026-11-02; final verification 2026-11-03):** baseline `c9c0f81` contains merged AUD-01 through AUD-04 and independently passed 1412 assertions / 38 V1 events. Before production edits, the opt-in `--aud-05-repro` failed: enemy skill spent 10 from the caller-supplied player's active pool, 30 -> 20. Work paused until the user explicitly approved Option B. The engine now enforces that ownership boundary; the probe is converted to permanent normal smoke coverage and its temporary flag removed. Final suite: 1587 assertions, including 175 AUD-05 assertions; V1 events 38; build 0 warnings/errors. See the historical investigation and approved closure below.

**AUD-09 correction (2026-11-03):** latest main baseline `bfdc8c1` independently passed 1587 assertions and 38 V1 events, including merged AUD-01 through AUD-05. Before production edits, the new regression failed with progress -2147483599, rank Practiced -> Unranked and AddProgress returning true for 50 + int.MaxValue. Widening the sum before capping now produces 100 / Mastered. The full suite passes 1773 assertions, including 186 new AUD-09 assertions; V1 events 38; restore/build/smoke/diff-check exit 0, build 0 warnings/errors. Existing audit regression sections remain unchanged and green.

**Consolidation documentation checks (2026-10-31):** all 11 archived sources match with the documented whitespace normalization; 67 live local Markdown links resolve; all 31 explicit anchors are unique; git diff --check exits 0. Production code, tests, solution/build configuration and generator scripts had no diff in that documentation pass. All 10 newly retired originals were verified before deletion; the master specification was already absent. No generator ran and no Unity files changed. The five remaining old-path references outside this Bible are historical generator output declarations in modules 00, 05 and 06, not live documentation links; their behavior is explicitly described in the tooling section. No GitHub Actions configuration or Unity Assets/ProjectSettings tree was present in the tracked inventory. Unity Editor, Play Mode and playable-game validation remain unperformed.

Integrated smoke covers class/mentor/teaching/mastery, retention after switching class, binding save/JSON/restore, prerequisite-first tree unlock application, scoped trees with identical node IDs, validation modes/provider/classification/orchestration, optional engine integration and shared loaded-session/bootstrap ownership. See [progression tests](../src/Game.SmokeTests/PlayerProgressionFoundationSmokeTests.cs), [runtime workflow tests](../src/Game.SmokeTests/ProgressionRuntimeBootstrapSmokeTests.cs), [V2 tests](../src/Game.SmokeTests/V2SmokeTests.cs), [V3 tests](../src/Game.SmokeTests/V3SmokeTests.cs) and [runner](../src/Game.SmokeTests/SmokeTestRunner.cs).

Coverage gaps include full-state save replacement, general rollback/isolation and exception handling during restore application, other hostile/extreme numeric values, platform/filesystem fault injection, Unity/editor integration and a production playable flow. AUD-01 covers null coordinator destinations with nonempty state; AUD-02 covers semantic tree preflight through both coordinator overloads; AUD-03 covers supported Codex facts through JSON and file round trips; AUD-04 covers canonical player skill metadata and actual engine rejection/execution; AUD-05 covers the approved player-only Resonance skill ownership boundary; AUD-06 covers shared refill status/debuff lifecycle, lethal boundaries, resource compatibility and refill-to-combat workflows; AUD-07 independently verifies terminal refill rejection, no mutation/events/hooks and null precedence; AUD-08 covers authoritative Silence skill blocking, precedence, events, expiration and permitted non-skill actions; AUD-09 covers bounded mastery accumulation through direct, profile, mentor and snapshot workflows; AUD-10 covers actual teaching delta, immutable raw operation facts and mentor/JSON/restore compatibility; AUD-11 covers canonical pool spending/regeneration bounds and representative combat/refill workflows; AUD-12 covers honest preflight/application result facts, successful restore matrices and controlled mutable-input application failures. Closing the twelve reproduced findings does not close these other gaps or verify an Echoes economy.

<a id="audit"></a>
# Part V: Audit & Technical Debt

<a id="reproduced-findings"></a>
## Reproduced Integrity Findings

AUD-01 through AUD-12 are **RESOLVED** within their documented acceptance boundaries. AUD-07 verifies a fix already introduced by AUD-06; the other closures include targeted corrections. The following table preserves the previous audit's reproduced observations and severity, not a claim that every original defect still exists or that the whole backend is defect-free.

| ID | Severity from executed probe | Reproduced behavior | Affected code | Required acceptance direction |
|---|---|---|---|---|
| AUD-01 | HIGH | Restore with null mastery destination changes class, then throws `ArgumentNullException` | `src/Game/Domain/Progress/V4ProgressRestoreCoordinator.cs` | RESOLVED 2026-11-01: prevalidate real destinations; null destination rejection causes zero target mutations |
| AUD-02 | HIGH | Unlocked tree node without prerequisite: semantic validator returns false, coordinator returns `FullyRestored=true` | `V2ProgressSnapshotService.cs`, validation service, coordinator | RESOLVED 2026-11-02: existing semantic tree validation reused before coordinator target mutation |
| AUD-03 | HIGH | Codex snapshot→JSON→restore loses legacy discovery and completion; enriched record survives | `V2ProgressSnapshotService.cs`, Codex profile | RESOLVED 2026-11-02: optional discovery/completion ID lists preserve supported Codex facts alongside enriched records |
| AUD-04 | HIGH | Alternate definition with same skill ID bypasses canonical unlock requirement | `src/Game/Domain/Combat/CombatSkillUseValidator.cs` | RESOLVED 2026-11-02: resolve canonical registered definition, reject conflicting metadata before execution and retain value-identical copies |
| AUD-05 | MEDIUM | Enemy skill spends 10 from the runtime's single Resonance pool | `TurnBasedCombatEngine.cs` | RESOLVED 2026-11-03: approved player-owned pool; positive non-player costs blocked before skill spending/effects/XP; zero-cost enemy Essence behavior preserved |
| AUD-06 | MEDIUM | Refill with Poison advances turn but neither damages nor decrements duration | `ResonanceRefillAction.cs` | RESOLVED 2026-11-03: shared canonical turn lifecycle; status/debuff consequences, defeat and exactly-once advancement; 119 regression assertions |
| AUD-07 | MEDIUM | Refill after combat end succeeds, restores resource and advances turn | `ResonanceRefillAction.cs` | RESOLVED 2026-11-03: AUD-06 guard already rejects terminal refill; 247 independent assertions verify complete no-op behavior; no production correction required |
| AUD-08 | MEDIUM | `SilenceDebuff` emits `ActionBlocked`, yet `SkillUsed`, XP and damage still occur | `SilenceDebuff.cs`, combat engine | RESOLVED 2026-11-04: per-context skill restriction consumed by engine before costs/effects; deterministic single diagnostic; 220 new assertions |
| AUD-09 | MEDIUM | Mastery 50 + `int.MaxValue` wraps to `-2147483599` / `Unranked` | `MasteryRecord.cs` | RESOLVED 2026-11-03: widen the sum before capping at 100; preserve integer storage, rank thresholds and no-op behavior |
| AUD-10 | MEDIUM | Teaching +50 at mastery 90 caps at 100 but reports `ProgressApplied=50` rather than 10 | `MentorTeachingApplicationService.cs` | RESOLVED 2026-11-03: report actual delta from canonical immutable mastery operation; raw overflow is not an Echoes award |
| AUD-11 | MEDIUM | `ResourcePool.TrySpend(-10)` succeeds, `Current=110` when `Max=100` | `ResourcePool.cs` | RESOLVED 2026-11-03: reject negative spending before arithmetic; preserve zero-cost success and pool bounds, including safely capped regeneration |
| AUD-12 | MEDIUM | Failed preflight reports `ClassRestored=true` despite unchanged destination | `V4ProgressRestoreCoordinator.cs` | RESOLVED 2026-11-03: local preflight facts; rejected preflight has all applied flags false; real application returns drive results and phase-specific diagnostics |

**Correction:** `Defend` and `Wait` exist in an enum, but `TurnRequest` only publicly constructs basic attacks and skills. Classify these as **incomplete contracts**, not reproduced failures through the current public API.

### Probe Observations and Reproduction Boundaries

| ID | Observed inputs / output supporting the finding |
|---|---|
| AUD-01 | Destination class changed from audit.before to audit.after, then null mastery destination threw ArgumentNullException; temporary-profile preflight had passed |
| AUD-02 | Child unlocked without root: registry-aware semantic validation false; coordinator FullyRestored true; child true and root false afterward |
| AUD-03 | Legacy DiscoverEntry(string) and CompleteEntry facts lost in JSON round trip; candidate-backed enriched discovery survived |
| AUD-04 | Canonical skill with required unlock blocked as V3SkillLocked; alternate definition with same ID but no requirement accepted |
| AUD-05 | Enemy skill with ResonanceCost 10 reduced the runtime's single active pool from 30 to 20 |
| AUD-06 | Refill advanced turn 1 to 2 with Poison duration 2/potency 7; HP remained 100 and duration remained 2 |
| AUD-07 | Refill succeeded after EndCombat, restored resource and advanced turn 2 to 3 |
| AUD-08 | Silence produced one ActionBlocked and one SkillUsed; XP increased by 10 and defender HP fell from 100 to 95 |
| AUD-09 | Progress 50 plus int.MaxValue became -2147483599 and Unranked |
| AUD-10 | Teaching requested 50 at progress 90; profile capped at 100 but result said ProgressApplied 50, not actual delta 10 |
| AUD-11 | ResourcePool at Current=Max=100 accepted TrySpend(-10), leaving Current=110 |
| AUD-12 | Invalid mastery stopped preflight application; ClassRestored true despite destination still audit.before |

These original probes were ad-hoc in-memory checks, not committed test additions. AUD-01 through AUD-06 and AUD-08 through AUD-12 now have permanent failing-then-passing regressions. AUD-07 has permanent passing verification against already-corrected main, not a new red phase. Historical probe outputs remain preserved below their original scope and date.

### AUD-01 Closure: Real Destination Validation

**Original failure and root cause:** both public coordinator overloads delegate to `RestoreInternal`. Preflight restores into new class/mastery/mentor/tree profiles and a copied binding registry, so it cannot detect null real profile references. On a valid preflight, `RestoreClassProfile` assigns the requested class; `RestoreMasteryProfile` then throws `ArgumentNullException(nameof(masteryProfile))`. A null mentor or tree destination could likewise reject after earlier subsystems had applied. The binding overload already checked its binding destination before delegation.

**Correction:** validate all four real profile references at the start of `RestoreInternal`, before building temporary state or applying any target. Reuse the existing `ArgumentNullException` type and parameter names; null rejection produces no restore result. Existing snapshot/registry preflight, public signatures, per-subsystem methods, apply order, merge semantics and schema 1 are unchanged. No parallel restore owner, transaction framework, combat change or Unity dependency was introduced. When several arguments are invalid, destination guards now run before snapshot/content preflight; no precedence guarantee for multiple invalid arguments is added.

**Regression evidence:** [ProgressRestoreAtomicitySmokeTests.cs](../src/Game.SmokeTests/ProgressRestoreAtomicitySmokeTests.cs) runs inside the existing Progression group. On unchanged production code at baseline `634085f`, the first null-mastery case caught the expected `ArgumentNullException` but failed the independent value comparison: `class before=class_wanderer, after=class_tlamatini, bindings overload=False`. The smoke command exited `-532462766`; the build preceding it exited 0 with no warnings/errors. This was the real coordinator, not a mocked approximation.

After correction, 68 new assertions pass: null class/mastery/mentor/tree destinations through both overloads; null binding destination through the binding overload; two attempts per rejection; null and unsupported-schema snapshots; unknown-class rejection with overall failure and class reason; valid JSON round trips into populated profiles; and repeat valid application. Value captures materialize class/allowed-skill IDs, mastery progress/rank/mastered facts, mentor discovery/trust/availability, per-tree discovered/unlocked nodes, bindings and registered tree IDs independently of production snapshot creation. Valid restore checks preserve unrelated mastery, mentor and tree-scope state while applying the requested facts. Existing old-JSON, scoped-tree, registry and file-save compatibility cases also pass in the complete suite.

**Executed verification (2026-11-01):**
- `dotnet restore Fragments-of-Wisdom.sln`: exit 0; dependencies already up to date.
- `dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal`: exit 0; 0 warnings, 0 errors, before and after the correction.
- `dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build`: baseline exit 0 / 899 assertions; intentional red regression exit `-532462766`; corrected full suite exit 0 / 967 assertions (2 V1, 16 Shared, 264 V2, 100 V3, 585 Progression); V1 events remain 38.
- `git diff --check`: exit 0.
- Environment: the first sandboxed baseline smoke exited 1 after `UnauthorizedAccessException` at the existing file-repository `File.Move` test. The identical baseline command passed with expanded execution permission; subsequent red/green runs used that permission. No test was weakened to work around this restriction. Unity Editor/Play Mode was not run; no Unity assets or adapters changed.

**Demonstrated guarantee and remaining limits:** rejected null destinations do not mutate any existing target, including on retries. Existing null/version and unknown-class rejection paths also preserve populated state in the tested serial calls. Input/content checks still use the existing temporary restore path; this change does not wire in the broader semantic validator (AUD-02). Once application begins, profiles are mutated sequentially with no rollback: allocation/runtime failures or concurrent changes to mutable snapshots, registries or profiles are outside the guarantee. The existing application phase still ignores individual boolean returns, and preflight success booleans are not redesigned (AUD-12). A broader all-or-nothing application contract needs a separately approved follow-up; do not infer full transactional atomicity from this fix. Direct individual restore calls retain their existing behavior.

**Focused self-audit:** both public paths reach the shared guards before any real profile mutation; binding null rejection remains at its existing entry point. Valid nonempty JSON restoration and repeated application pass. Only the coordinator's destination guards changed in production. AUD-02 through AUD-12, their severity and their backlog order remain untouched; no further audit repair is authorized by this closure.

### AUD-02 Closure: Semantic Skill Tree Restore Preflight

**Root cause and original evidence:** the coordinator's temporary tree restore checked node/scope membership but not discovery or prerequisite closure. The standalone registry-aware validator already rejected the same scoped data. The permanent regression in [SemanticSkillTreeRestoreSmokeTests.cs](../src/Game.SmokeTests/SemanticSkillTreeRestoreSmokeTests.cs) used registered root/child definitions, otherwise valid class/mastery/mentor/binding records, and populated destinations captured as independent values using the unchanged AUD-01 fixture. Before the production fix, the smoke failed with `standalone valid=False, FullyRestored=True, state changed=True, child unlocked=True, root unlocked=False, bindings overload=True`; exit `-532462766`. The original audit observations above and Part VII remain preserved.

**Correction and reuse:** V2ProgressSnapshotValidationService exposes an internal tree-only restore check, reusing its structural checks and extracting its existing node/discovery/prerequisite algorithm into `HasValidSkillTreeNodes`. Both the public registry-aware validator and the coordinator use that same checker; there is no second prerequisite algorithm. The coordinator gates temporary tree restoration and the final all-subsystem application on that result. It does not run unrelated Codex/mission validation or change their behavior. Public signatures, schema 1, individual restore methods, destination guards and apply order are unchanged.

**Context and compatibility contracts:**
- Without the binding registry, legacy progress is validated against the explicit tree, including historical definitions with an empty tree ID. Scoped data requires exactly one scope whose ID matches the explicit nonempty tree ID. Multiple scopes are rejected; unavailable context is not guessed.
- With the binding registry, every scoped ID resolves through registered definitions, even if the separately supplied explicit tree is more permissive. The explicit tree may remain null for this scoped path. Legacy-only data continues to use the explicit tree, not an arbitrary tree selected from bindings; a required missing explicit tree retains `ArgumentNullException("skillTree")` before mutation.
- Unknown nodes/scopes, missing prerequisite closure, unlocked-but-undiscovered nodes, null/duplicate scopes and mixed legacy/scoped progress are rejected. Prerequisite closure is evaluated within each saved scope, not repaired using destination state or another scope. Valid legacy/scoped JSON and missing optional tree data remain compatible. Existing public no-registry validation retains its structural-only behavior; no schema migration is introduced.
- Semantic failure returns tree `ValidationFailed` and `FullyRestored=false`, leaving all real destinations untouched on repeated calls. Binding records still use the existing class/tree registry preflight. Other success flags retain their preflight-feasibility meaning (AUD-12); this mission does not relabel them as applied facts.

**Executed verification (2026-11-02):**
- `dotnet restore Fragments-of-Wisdom.sln`: exit 0; dependencies up to date.
- `dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal`: exit 0, 0 warnings/errors for baseline, red regression and corrected build.
- `dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build`: baseline exit 0 / 967 assertions; intentional red regression exit `-532462766`; corrected full suite exit 0 / 1106 assertions (V1 2, Shared 16, V2 264, V3 100, Progression 724). V1 events remain 38. Smoke runs used expanded execution permission for the existing temporary-file persistence tests; no new environment failure was observed.
- The 139 new assertions cover both overloads, scoped/legacy invalid data, repeated zero-mutation rejection, valid closure and JSON round trips, old JSON defaults, multi-tree scope independence, authoritative registry selection and missing context. All 68 AUD-01 assertions remain unchanged and green.
- `git diff --check`: exit 0. No Unity Editor/Play Mode run; no Unity, combat, reward, schema or generator changes.

**Verified boundary and limitations:** semantic rejection now occurs before any real coordinator destination mutation. This is a serial preflight guarantee, not rollback or isolation: callers must keep definitions, snapshots and target profiles stable during the call. Direct V2ProgressSnapshotService restore methods and structural file loading remain unchanged lower-level APIs, not substitutes for the validated coordinator. Pre-existing corrupt destination state is not repaired by merging a valid snapshot. Sequential application, ignored application boolean returns and AUD-12 reporting limitations remain; AUD-03 through AUD-12 are not addressed. Future stronger transaction or individual-service guarantees require separate approval.

### AUD-03 Closure: Codex Snapshot Round-Trip Integrity

**Original loss and root cause:** `CodexProfile` owns a discovered-ID set, a completed-ID set and an enriched-record list. `CreateSnapshot` previously enumerated only `DiscoveredEntries`, and `Restore` only replayed enriched discovery. Neither the DTO nor JSON carried ID-only discovery or any completion facts. The new regression first ran against unchanged production code at `e74eb52` and failed with `discovered before=4, after=2, legacy completion=False, enriched records=2` (exit `-532462766`). Independent value capture also covers enriched completion, record order, type, concept, first-source IDs and tag order. Original audit evidence above and Part VII are unchanged.

#### Codex Fact Ownership

| Fact | Existing source of truth | Before AUD-03 | Corrected persistence / interpretation |
|---|---|---|---|
| Legacy and enriched discovery membership | `CodexProfile.discovered`, exposed as `DiscoveredEntryIds`; both DiscoverEntry overloads and CompleteEntry add IDs | Only IDs represented by enriched records survived | Copy all IDs into `CodexDiscoveredEntryIds`; restore through DiscoverEntry(string) after enriched records |
| Completion membership, including enriched entries | `CodexProfile.completed`, queried by IsCompleted; CompleteEntry also adds discovery | Not captured or restored | Copy completed members by iterating discovered IDs and querying IsCompleted; `CodexCompletedEntryIds` restores through CompleteEntry without a new domain owner/API |
| Enriched discovery records and order | `CodexProfile.discoveredEntries`, exposed as DiscoveredEntries | Preserved through CodexEntries | Existing CodexEntries payload remains; restore records first so the accompanying ID lists cannot suppress enrichment in a fresh profile |
| Record metadata | CodexEntryRecord: EntryId, EntryType, ConceptKey, FirstSourceHookId, FirstSourceTriggerId, NarrativeTags | Preserved for enriched entries | Same fields and tag order preserved; no invented metadata for legacy-only IDs |
| Discovery stages | No persisted stage property in CodexProfile; only discovery/completion membership exists | Not implemented as the proposed stage ladder | Glimpsed/studied/mastered stages remain design, not new save facts |
| Progress counts and percentage | CodexProgressService derives CodexProgressSummary from enriched records plus caller-supplied totalKnownEntries | Derived, not profile-owned persistence | Unchanged; no new meaning assigned to legacy completion or summary percentages |
| Definition name/category | CodexEntryDefinition content, not mutable CodexProfile state | Not player progression state | No new definition serialization |
| Applied unlock facts | Separate UnlockProfile/UnlockRecord owner; snapshot Unlocks already carries IDs, requirements and tags | Independently persisted | Unchanged; restore does not run CodexUnlockService or UnlockApplicationService to generate new unlocks |

**Implementation and compatibility decision:** retain schema version 1 and all existing public constructors/methods. Add two optional root lists, `CodexDiscoveredEntryIds` and `CodexCompletedEntryIds`, plus an additive full snapshot constructor. Every CreateSnapshot overload forwards them; JSON serializes them and defaults absent/null lists to empty. No migration is required to read existing enriched-only JSON. Missing and explicitly empty fields both mean no additional saved facts under existing merge semantics, never erasure. Completion-only input implies discovery using the existing CompleteEntry contract. Previously supported old JSON and old constructors remain accepted. Older executables ignore the new optional fields and can lose them on rewrite; downgrade round-trip safety is not promised. Facts omitted by historical buggy saves cannot be reconstructed automatically.

**Restore and validation boundary:** the existing validation service now shares an internal Codex fact check with direct Restore and file-repository validation. Empty/whitespace/null IDs, null enriched records, null direct-DTO CodexEntries collections and null direct-DTO tag collections are rejected before Codex mutation; JSON's existing missing/null tag normalization remains unchanged. Direct invalid Codex restore raises InvalidOperationException; schema and destination argument checks retain their existing contracts. Valid enriched records are applied first, ID discoveries second and completions last. The existing domain sets/list enforce idempotency and first-discovery precedence. No new profile, registry, reward service, event emission or completion mechanic is introduced.

**Merge decisions and OPEN DECISION:** a destination that already discovered an ID keeps its current metadata, including no enriched record when legacy discovery came first. Incoming enrichment does not upgrade that ID; completion still unions with existing completion. This is existing executable first-wins behavior, not a new preference rule. A future policy for enriching previously legacy-only discoveries or reconciling conflicting content versions remains OPEN DECISION and requires separate approval. Exact fresh-destination round trips and compatible merges are distinct guarantees; merge does not replace the whole profile.

**Regression coverage:** [CodexRoundTripIntegritySmokeTests.cs](../src/Game.SmokeTests/CodexRoundTripIntegritySmokeTests.cs) adds 107 assertions in the V2 group: mixed/overlapping, legacy-only and enriched-record state across all seven snapshot creation overloads and both Restore overloads; independent before/after comparison; repeated application; historical enriched JSON; missing/null/empty lists; completion-implies-discovery; duplicate IDs/records; populated first-wins merges; late malformed facts rejected without partial Codex mutation and on retries; invalid file-load rejection; file save/load into PlayerProfile.Codex with unchanged wallet and no generated unlocks; and snapshots not aliasing later domain set mutations. Existing snapshot, old-JSON, schema, file fallback and progression tests all remain green.

**Executed verification (2026-11-02):**
- `dotnet restore Fragments-of-Wisdom.sln`: exit 0; dependencies up to date.
- `dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal`: exit 0; 0 warnings/errors before and after correction.
- `dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build`: baseline exit 0 / 1106; intentional red regression exit `-532462766`; final exit 0 / 1213 assertions (V1 2, Shared 16, V2 371, V3 100, Progression 724), V1 events 38. AUD-01's 68 and AUD-02's 139 assertions remain unchanged and pass.
- `git diff --check`: exit 0. Smoke used expanded execution permission for existing temporary-file tests; no new environment test failure occurred.

**Self-audit and limitations:** no domain state owner or Codex rule changed; existing enriched metadata and ordered records survive, and generated snapshots copy all supported discovery/completion membership. Valid fact IDs are nonempty; externally corrupted sets or concurrent mutations are not supported transactions. Invalid Codex validation precedes mutation, but later failures in unrelated mission/unlock application still have no rollback; this is not a general save transaction repair. PlayerProgressionRuntimeState/LoadedPlayerSession do not own or disk-load Codex state. Tests exercise the actual file repository and snapshot service used by UnityV2SaveService, but UnityV2SaveService itself is outside the solution and no Unity Editor/Play Mode run was performed. No gameplay rewards, combat, UI, Unity, generators, schema version, AUD-04 through AUD-12 or their priorities were changed.

### AUD-04 Closure: Canonical Skill Definition Validation

**Status: RESOLVED (2026-11-02).** Scope is authoritative skill metadata in the existing player combat authorization path, not a new registry, combat executor, balance rule or progression restriction. The original reproduced observation above remains historical evidence. AUD-05 through AUD-12 are unchanged and open.

**Ownership and root cause:** PlayerProgressionRuntimeState owns the live profiles and supplied canonical SkillDefinitionRegistry; ProgressionAwareCombatEngineFactory wires those references through CombatSkillTurnValidatorGateway, the orchestrator and CombatSkillUseValidator. The validator previously compared only ICombatSkill.Id with SkillProgress.Definition.Id, then evaluated the caller's RequiredUnlockId. Retention checked registration by ID, so a same-ID definition without the canonical requirement passed both checks. TurnBasedCombatEngine subsequently used that supplied progress for costs, mastery modifiers, skill Apply and XP. ID equality did not prove metadata equivalence.

**Failing reproduction before production edits:** [CanonicalSkillDefinitionSmokeTests.cs](../src/Game.SmokeTests/CanonicalSkillDefinitionSmokeTests.cs) registers a canonical class-allowed skill requiring `unlock_aud04`, with cost 10 + 1 per level and ResonanceCost 2. An otherwise identical definition omits that unlock. The actual application factory/gateway/engine path accepted it with an empty UnlockProfile: turn 1 -> 2, Essence 20 -> 14 (11 spent, 5 regenerated), active Resonance 6 -> 4, enemy HP 100 -> 94, Apply executed and XP 0 -> 10. The regression threw on the expected unauthorized-execution assertion, exit `-532462766`. No cooldown field or cooldown mutation exists in the inspected skill/progress/engine contract; no cooldown guarantee is invented.

**Authoritative metadata contract:** after structural ID checks, player validation with a SkillDefinitionRegistry resolves the canonical definition from that registry. Unknown ID returns `SkillDefinitionUnregistered`; missing caller/canonical Cost returns `SkillDefinitionInvalid`; differing metadata returns `SkillDefinitionConflict`. These enum values are appended without renumbering prior reasons. Matching requires every current SkillDefinition field by value: Id, Name, MaxLevel, Cost.BaseCost, Cost.CostPerLevel, Category, Targeting, EffectType, RequiredUnlockId and ResonanceCost. Strings use exact ordinal equality (null and empty are distinct metadata). Distinct definitions and SkillCost objects with identical values are accepted; reference identity is not required. Both player validator overloads enforce this when a registry is supplied. V3 unlock evaluation then uses the resolved canonical definition, followed by the unchanged class/mastery retention policy.

**Authorization/execution consistency:** no engine or public API signature changed. SkillDefinition and SkillCost expose private setters, SkillProgress keeps its definition, and TurnRequest keeps its progress. Under the current single-threaded public API, the accepted metadata remains value-equivalent while the engine reads costs, max-level mastery modifiers and applies the skill. It does not validate one set of metadata and execute a weaker one. Preserving the original SkillProgress also preserves its existing level/XP rather than replacing it with a new progression object. Registry ownership remains unchanged. New definition fields must be included in this comparison and its regressions if added later.

**Rejected-action semantics:** the existing engine emits one ActionBlocked with the structured reason, skips skill costs/Apply/XP/damage and performs normal end-turn processing. It still consumes a turn, regenerates Essence when allowed and ticks existing statuses; earlier cognition/debuff hooks can already have run. This is authorization rejection, not rollback of the entire turn. The corrected reproduction ends at turn 2, Essence 20, Resonance 6, enemy HP 100, Apply false and XP 0. A partially depleted resource test confirms end-turn regeneration remains in place.

**Regression coverage:** 199 assertions in the existing Player Progression / Combat Validation group cover canonical locked requests; omitted/null/different unlock metadata (including an already-satisfied weaker requirement); each remaining metadata field independently; missing Cost on either side; null skill/progress/definition/context/request and unregistered definitions; stable gateway/event reasons; no skill spend/effects/XP on rejection; canonical and separate identical-copy execution; pre-existing level and per-level costs; legitimate unlocks; class switching, unavailable/available mentor teaching and retained mastery; mastery not overriding the canonical unlock; basic-attack validator bypass; explicit Enemy and LegacyBypass behavior; and both player validator overloads. All existing workflow assertions remain unchanged.

**Executed verification (2026-11-02):**
- `dotnet restore Fragments-of-Wisdom.sln`: exit 0.
- `dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal`: baseline and final exit 0, 0 warnings/errors. One intermediate test-only build exited 1 for a missing Game.Domain.Common import; corrected before final verification.
- `dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build`: baseline exit 0 / 1213; intentional red regression exit `-532462766`; final exit 0 / 1412 (V1 2, Shared 16, V2 371, V3 100, Progression 923), V1 events 38. Unchanged AUD-01 68, AUD-02 139 and AUD-03 107 assertions pass.
- `git diff --check`: exit 0. Smoke runs used expanded execution permission for existing temporary-file persistence tests.

**Self-audit and limitations:** no duplicate registry, validator or state owner was added. The production change is confined to CombatSkillUseValidator and its block-reason enum. Basic attacks, trusted Enemy/LegacyBypass contexts, the optional unvalidated engine constructor and the historical V3-only overload without a registry retain their existing contracts; they do not certify canonical player access. Application composition must select the progression-aware path for players. ICombatSkill implementations/passive hooks remain trusted executable code, not registry-owned code or a security sandbox; arbitrary malicious Apply logic, reflection and concurrent mutation are outside this metadata guarantee. No new skill-tree combat prerequisite is invented: tree unlocks remain facts with no automatic grant. Canonical numeric range/overflow validation is not expanded (AUD-09/AUD-11 remain open). No persistence, schemas, Unity assets, balance, other audit fix or generator changed. Unity Editor/Play Mode was not exercised.

### AUD-05 Historical Investigation: Resonance Ownership Decision Gate

**Pre-approval evidence (2026-11-02), superseded by the approved closure below.** Investigation originally stopped at OPEN DECISION: the then-current Parts I/II left enemy Resonance ownership undecided and the archived V3 contract only established an optional runtime pool. No production correction was made before approval. The diagram, findings, alternatives and questions in this historical subsection describe that investigation baseline, not the final contract or current instructions.

**Current ownership and dependency diagram (references, not approved actor ownership):**

```text
Caller / combat setup
  +-- CombatEntity player ---- ResourcePool (actor Essence)
  +-- CombatEntity enemy ----- ResourcePool (actor Essence)
  +-- ResonanceProfile ------- ActiveResonance -> ResourcePool
  |                        \- ResonanceReserve -> ResourcePool
  \-- CombatRuntimeContext(player, enemy, log, PlayerProfile, optional profile)
       +-- Player / Enemy references
       \-- ResonanceProfile reference (no actor owner)

TurnRequest.Attacker -> actor Essence
TurnBasedCombatEngine -> runtime.ResonanceProfile for ANY accepted skill cost
ResonanceRefillAction(profile, runtime, amount)
  -> supplied profile reserve/active; supplied runtime events/turn
  -> no actor argument or profile/runtime identity check
```

**Source-grounded findings:**
- CombatRuntimeContext stores one caller-injected optional profile and exactly one Player/Enemy pair. CombatEntity owns Essence, not Resonance. Neither PlayerProfile nor PlayerProgressionRuntimeState owns Resonance. There is no owner ID, actor-to-resource map or exclusivity enforcement; callers can alias the same profile into multiple contexts.
- Production FoundationVerticalSlice constructs the four-argument runtime without Resonance. Accessible nonzero pool construction is in smoke fixtures. Constructors initialize Active/Reserve ResourcePools to their maximum; no application/session factory creates those resources implicitly. LoadedPlayerSession and ProgressionAwareCombatEngineFactory create validated engines, not combat entities, runtime contexts or Resonance state.
- TurnBasedCombatEngine checks optional skill validation and SkillSeal, then reads the supplied definition's ResonanceCost. A positive cost consults and spends runtime.ResonanceProfile regardless of Attacker. Missing/insufficient active resource emits ResourceNotEnough and ends the turn before Essence spending, Apply, damage or XP. Reserve is never auto-spent. Enemy classification bypasses player progression requirements, not this shared engine resource cost.
- Refill accepts any nonnull profile and runtime, with no actor argument. Consequently it cannot establish which actor is authorized to refill, and the profile need not equal runtime.ResonanceProfile. Existing V3 tests intentionally exercise refill with a separately supplied profile. Normal refill success consumes a turn; ownership enforcement would need an explicit adapter/migration decision. AUD-06 status ticking and AUD-07 ended-combat behavior remain untouched.
- The production enemy AI chooses BasicAttack only. No production enemy skill catalog or approved nonzero enemy Resonance configuration was found. Public TurnRequest.SkillAction and SkillDefinition permit enemy requests with a positive ResonanceCost; the regression exercises that supported path without proposing a new enemy skill mechanic. Default existing skill constructors use ResonanceCost 0, while Essence remains actor-owned and charged independently.
- Both represented combatants act in the same context. There is no party/roster model. With progression validation, a foreign actor is rejected by CombatActorValidationClassifier; the legacy engine alone is not a party-membership authority. Multi-ally/enemy resource isolation is not implemented or verified.
- No Resonance UI adapter/presenter or save DTO exists in the inspected source. GameEventLog exposes ResourceSpent/ResourceNotEnough/turn messages, not a typed resource-owner projection. Unity currently supplies persistence adapters, not a Resonance UI. Any future presentation must consume explicit owner facts and events rather than infer ownership from the global pool or create parallel state.

**Deterministic reproduction:** [ResonanceOwnershipAuditSmokeTests.cs](../src/Game.SmokeTests/ResonanceOwnershipAuditSmokeTests.cs) supplies a profile intended by its caller for the player (active 30, reserve 50); this is a test ownership expectation, not a claim that PlayerProfile already stores it. The actual gateway classifies the enemy as Enemy without player profiles. The unchanged engine executes its structurally valid skill with Essence cost 10 and ResonanceCost 10. Results: active 30 -> 20; reserve 50 -> 50; turn 1 -> 2; player HP 100 -> 94; enemy XP 0 -> 10; enemy Essence 20 -> 15 after spending 10 and regenerating 5; player Essence remains 20. Apply executes. Events in order: TurnStarted, ResourceSpent (enemy Essence), ResourceSpent (enemy Resonance), SkillUsed, DamageDealt, ResourceRegenerated, TurnEnded. The final isolation assertion fails with InvalidOperationException, exit `-532462766`, against unchanged production. There is no cooldown model to record.

Historical command used to execute the intentionally red probe before approval (its temporary flag has since been removed):

```powershell
dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build -- --aud-05-repro
```

At investigation time the flag ran only this probe and did not catch or convert the failure into success. The default suite still passed 1412 assertions. The approved correction now includes the isolation regression and policy-specific cases in normal coverage; no special flag or excluded failing test remains.

**Alternatives considered (all proposals, not accepted contracts):**

| Dimension | A: optional per-actor ownership | B: player-owned pool with explicit enemy policy | C: injected owner/resource resolver |
|---|---|---|---|
| Domain ownership | One optional resource reference on each participating actor; never assume all actors have it | Existing runtime profile is explicitly associated with runtime.Player, not every requester | Resolve a request actor to an existing resource owner; no second pool |
| Multiple combatants | Natural resource isolation, but still needs a future roster model | Smallest fit for current one-player/one-enemy context; does not solve party resources | Extensible routing, but current architecture has no authoritative mapping to reuse |
| Save/load | No current Resonance DTO; persistence/lifetime choices remain separate | No save change needed for per-combat binding; persistence still undecided | Owner IDs/lifetime would need later persistence design, not an implicit schema addition |
| Runtime compatibility | Actor initialization and the existing runtime property need one canonical reference, not two independently mutable owners | Preserve the optional runtime profile and player cost semantics; change only inappropriate non-player access after approval | Additional constructor dependency/adapters; larger contract surface |
| Unity boundary | Future owner-scoped events/read models; no Unity resource mutation | Future player resource projection; no new enemy gauges implied | Future presentation consumes resolver facts, not a separate UI mapping |
| Migration complexity | Higher: actor construction, legacy runtime aliases and refill API; ResonanceProfile requires both Active and Reserve | Lower for skill spending; refill owner association still needs explicit compatibility handling | Highest for this slice; adds routing without settled enemy semantics |
| Parallel ownership risk | Avoid retaining separate actor and runtime stores; detect shared references rather than silently accepting aliasing | Keep one profile reference; do not add a second PlayerProfile/session-owned pool | A new dictionary would duplicate ownership unless a single source is first approved |
| Tests/factories | Update actor setup, pool isolation and missing-resource cases; no automatic enemy profiles | Existing player/factory/V1 behavior can remain; explicit positive enemy cost rejection needs new tests | Broader injection and resolution tests; no supporting production abstraction currently exists |

**Recommendation, NOT authorization:** choose B for the present slice: make the supplied runtime ResonanceProfile explicitly player-owned; do not create enemy tanks, regenerate enemy Resonance or silently waive a positive cost. Until a distinct enemy resource policy is approved, a non-player request declaring ResonanceCost > 0 should fail closed before Essence/Resonance spending, effects or XP, using the existing blocked-turn lifecycle. A zero-Resonance enemy request would still pay its declared actor Essence cost normally. Exact diagnostic facts and refill ownership-compatible API must be agreed before implementation. A is preferable if a broader per-actor Resonance capability is an actual approved requirement; merely moving the current profile onto every actor would invent that requirement. C is not recommended now because it adds a resolver without an existing owner map and does not decide enemy mechanics.

**Approval required / remaining questions:** is the player-only contract B approved, including rejecting unsupported positive enemy costs rather than making them free, or should actors explicitly opt into A with separately approved enemy capabilities? Who supplies those capabilities, can enemies have Active without Reserve, and what lifetime/persistence is intended? How should the legacy refill API express or validate ownership while retaining its separately supplied-profile compatibility? Do not select defaults or introduce new resources to answer these questions implicitly.

**Historical investigation verification:** restore/build/default smoke/diff-check exited 0; build had 0 warnings/errors. Normal smoke was V1 2 / events 38, Shared 16, V2 371, V3 100, Progression 923, total 1412. AUD-01 68, AUD-02 139, AUD-03 107 and AUD-04 199 passed. Default smoke used expanded permission for existing temporary-file tests; the opt-in reproduction ran without it and failed with exit `-532462766`. At that checkpoint production diff was empty, changes were local/uncommitted and AUD-05 remained OPEN. The following closure records the separately authorized correction.

### AUD-05 Closure: Approved Player-Owned Resonance

**Status: RESOLVED (2026-11-03).** The user's subsequent architecture decision explicitly approved Option B for the current vertical slice: CombatRuntimeContext.ResonanceProfile is player-owned, positive enemy ResonanceCost is unsupported and must be blocked, zero-Resonance enemy skills retain normal Essence costs. This is a compatibility contract, not a permanent prohibition against optional per-actor resources. No enemy resource model is inferred.

**Production enforcement:** after the existing optional validator and SkillSeal gate, TurnBasedCombatEngine reads ResonanceCost and rejects a positive cost whenever request.Attacker is not runtime.Player. The engine emits ActionBlocked with the stable appended CombatSkillUseBlockReason.UnsupportedResonanceOwnership value and calls the existing EndTurn. This runs before GetBaseCost, mastery cost modifiers, OnBeforeSkillCostCalculated, resource availability/spending, Apply, damage and XP. It therefore applies to progression-aware, explicit legacy-bypass and two-argument unvalidated engines; even a foreign legacy actor cannot spend the player pool. The runtime property has a player-ownership comment; no new resolver, pool, actor map, constructor or method was added. Existing enum numeric values are preserved.

**Separation of responsibilities:** CombatSkillUseValidator still evaluates structural/progression access, not resource ownership. Enemy access can be allowed by that validator and then rejected by the engine's resource boundary. Earlier validation failures and SkillSeal retain their precedence. Canonical player metadata validation from AUD-04 is unchanged. Player skill costs and missing/insufficient resource behavior are unchanged: ResourceNotEnough before spending/effects/XP, reserve never implicitly spent. Enemy ResonanceCost 0 proceeds through the existing enemy Essence checks and spending, not free execution.

**Mutation boundary:** rejected enemy skills emit no ResourceSpent/ResourceNotEnough, execute no skill-cost hook or Apply, inflict no skill damage and gain no XP. They consume their turn through normal EndTurn, including permitted Essence regeneration and status ticks. Earlier turn-start/cognition/debuff behavior remains intact; this is not whole-turn rollback. In the original scenario, player active stays 30, reserve stays 50, HP stays 100, enemy XP stays 0, enemy Essence stays 20 and turn advances to 2. A depleted enemy test verifies regeneration; a Poison instance verifies existing skill end-turn ticking without changing refill lifecycle.

**Permanent regression coverage:** [ResonanceOwnershipAuditSmokeTests.cs](../src/Game.SmokeTests/ResonanceOwnershipAuditSmokeTests.cs) contributes 175 normal Player Progression / Combat Validation assertions. The original failing isolation invariant now passes without opt-in. Cases cover enemy positive/zero costs with full/empty/missing player pools in all three engine compositions; actual enemy Essence spending and insufficiency; player positive costs with sufficient/insufficient/missing Resonance and insufficient Essence; stable rejection reason and cost-hook ordering; no player resource replacement/implicit creation; basic attacks by both actors; foreign legacy actor rejection; unchanged blocked-turn regeneration/status behavior; and an integrated player skill -> rejected enemy skill -> zero-cost enemy skill -> explicit player refill flow. Existing V3 refill tests and all AUD-01 through AUD-04 regressions remain unchanged and green.

**Executed verification:** baseline 1412 assertions / 38 V1 events; intentional pre-fix isolation failure exit `-532462766`. Final `dotnet restore Fragments-of-Wisdom.sln`, `dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal`, `dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build`, and `git diff --check` exit 0. Build: 0 warnings/errors. Final counts: V1 2, Shared 16, V2 371, V3 100, Progression 1098, total 1587; V1 events 38. One intermediate integrated test failed because its deterministic random sequence covered only the first attack; the test fixture was corrected to cover both attacks without a production change. Smoke uses expanded permission for existing temporary-file tests.

**Compatibility limits and remaining concerns:** existing non-player callers that previously paid positive costs from the shared pool now receive the intentional ownership rejection, not a waived cost. The legacy refill method remains a trusted explicit player operation, including its separately supplied-profile compatibility; it has no actor parameter and is not a general enemy action authorization API. No enemy refill capability was introduced and no refill status/ended-combat behavior changed. Runtime setup must supply the correct Player reference and profile; cross-runtime aliasing, malicious trusted skill/passive code and invalid numeric definitions remain outside this correction. There is no party resource model, no implicit enemy Active/Reserve state and no new persistence schema or Unity integration. Future per-actor resources require a separately approved capability/lifetime design. AUD-06/AUD-07/AUD-08 and the other remaining findings are unchanged. No Unity Editor/Play Mode verification was performed. Stop after AUD-05 for architectural review.

### AUD-09 Closure: Overflow-Safe Mastery Progression

**Status: RESOLVED (2026-11-03).** Scope is only MasteryRecord.AddProgress arithmetic. The original observation above is preserved; AUD-10 through AUD-12 and every other open finding are unchanged. No new progression service, storage type, rank threshold or reward rule is introduced.

**Original reproduction and root cause:** [MasteryOverflowSmokeTests.cs](../src/Game.SmokeTests/MasteryOverflowSmokeTests.cs) constructs a record at progress 50 / Practiced and calls AddProgress(int.MaxValue). Before production edits, the int addition wrapped to -2147483599, Rank became Unranked, and the method returned true. The post-addition `> 100` check missed the negative result. The normal smoke command threw InvalidOperationException on the new expected-invariant assertion, exit `-532462766`. Production diff was empty at that checkpoint. After the correction the same regression reports 100 / Mastered and returns true.

**Numeric and API contract:** Progress remains an int in the inclusive range 0..100. Rank is derived: 0 -> Unranked, 1..99 -> Practiced, 100 -> Mastered. Both constructors and SetProgress retain existing clamping, including negative input -> 0 and values above 100 -> 100. AddProgress still returns false without mutation for amount <= 0 or an already-mastered record. Otherwise it returns true and increases progress up to 100. The sole arithmetic change is `Progress = (int)System.Math.Min((long)Progress + amount, MasteredProgress);`. Conversion to long occurs before addition; the largest reachable sum after guards is 99 + int.MaxValue = 2147483746, safely representable in long. Capping happens before conversion back to int. Positive accumulation therefore cannot lower either progress or rank. SetProgress/restore remain explicit replacement operations and can lower a prior value; the monotonic guarantee applies to positive AddProgress, not those APIs.

**Call paths and boundaries:** MasteryProfile.IncreaseProgress validates the stable skill ID, obtains/creates one record and returns AddProgress's bool. Its existing behavior of creating an Unranked record for a registered first zero/negative increment is preserved. MentorTeachingApplicationService delegates through that same method and consumes its bool plus IsMastered; retention consumes IsMastered. Snapshot capture reads Progress/Rank; RestoreMasteryProfile delegates to RestoreRecord/SetProgress and derives rank from restored progress rather than trusting saved rank. No caller, registry or persistence code needed changes. SkillProgress XP/level mastery is a separate model and is untouched. The mentor application's inaccurate ProgressApplied capped-delta reporting is AUD-10 and is intentionally not fixed or redefined by these tests.

**Regression evidence:** 186 assertions in the normal Player Progression / Combat Validation group cover the exact overflow; initial zero; constructors/SetProgress at int.MinValue, -1, 0, 1, 2, 98, 99, 100, 101 and int.MaxValue; a 36-case matrix crossing valid starting progress with small, exact-cap, excess-cap and extreme positive increments; unchanged bool results; rank/progress monotonicity; zero, negative and int.MinValue increments; repeated additions after mastery; registry validation; and profile no-op record compatibility. The matrix uses a remaining-capacity oracle rather than duplicating the production widened-sum expression.

**Integrated workflow and saves:** registered mentor discovery/trust -> available int.MaxValue teaching applied to mastery 50 -> progress 100 / Mastered -> repeated teaching blocked as AlreadyMastered -> snapshot -> JSON -> registry-aware validation -> fresh and populated mastery restore -> repeated oversized increment safely rejected. Snapshot schema stays 1, persisted Progress remains an integer and Rank values are unchanged. Legacy schema-1 JSON at progress 50 still restores and then safely advances; a deliberately inconsistent saved rank confirms existing derived-rank restore behavior. Older JSON without mastery data still validates/restores to an empty profile. No persistence implementation or schema changed.

**Executed verification (2026-11-03):**
- `dotnet restore Fragments-of-Wisdom.sln`: exit 0.
- `dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal`: baseline and final exit 0, 0 warnings/errors.
- `dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build`: baseline exit 0 / 1587; intentional pre-fix regression exit `-532462766`; corrected suite exit 0 / 1773 (V1 2, Shared 16, V2 371, V3 100, Progression 1284), V1 events 38. Unchanged AUD-01 68, AUD-02 139, AUD-03 107, AUD-04 199 and AUD-05 175 assertions pass.
- `git diff --check`: exit 0. Smoke used expanded execution permission for existing temporary-file persistence tests.

**Compatibility and limits:** public APIs and bool semantics are unchanged; only overflow-corrupted positive accumulation is corrected. No mentor rewards/delta reporting, ResourcePool arithmetic, combat costs, Resonance ownership, skill authorization, save schema or Unity code changed. Historical saves that already contain overflow-corrupted negative progress are not reconstructed: existing restore clamping remains unchanged and lost history cannot be inferred from the negative number. Concurrent mutation, reflection-corrupted state and unrelated numeric validation are outside this fix. No Unity Editor/Play Mode run was performed. AUD-10 remains the next proposed atomic candidate, not authorization to begin it; stop after AUD-09 for architectural review.

### AUD-10 Closure: Actual Mastery Delta and Canonical Operation Facts

**Status: RESOLVED (2026-11-03).** Baseline was latest `origin/main` at `16e6ae0`, containing resolved AUD-09 and the preserved Echoes / The Path Unwalked chapters. AUD-10 was the next unresolved priority (5b). Baseline restore/build/smoke succeeded: 1773 assertions, 38 V1 events, zero build warnings/errors. No other audit fix is included.

**Reproduction and root cause:** before production edits, a permanent assertion in [MasteryProgressContractSmokeTests.cs](../src/Game.SmokeTests/MasteryProgressContractSmokeTests.cs) ran through the existing smoke runner. A registered skill at 90 received an available +50 teaching. Console facts were `initial=90, requested=50, final=100, reported=50, actual=10`; the assertion threw InvalidOperationException and `dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build` exited **1**. Production diff was empty. The mentor service returned the requested grant on success rather than the capped record's actual change. The same assertion now passes with `reported=10`.

**Decision B, not A:** reading state before/after in the teaching service would fix the report but leave future raw-overflow accounting to another caller. A cohesive extension instead keeps the one authoritative arithmetic/mutation in `MasteryRecord.AddProgressWithResult(int)`. `AddProgress(int)` delegates and returns Applied. `MasteryProfile.IncreaseProgressWithResult(...)` validates the registry and delegates to that record; `IncreaseProgress(...)` preserves its bool signature. The mentor Apply signature and MentorTeachingApplicationResult constructor/properties remain unchanged; Apply consumes AppliedProgress from this operation. No parallel calculator/service or second mastery mutation path was added. RestoreRecord/SetProgress remain the existing explicit replacement APIs, not ordinary grants.

**Immutable contract:** `MasteryProgressResult` is sealed, has getter-only facts and an internal constructor. RequestedProgress retains the exact supplied int, including rejected negative inputs, for diagnostics. AppliedProgress is the actual delta; Applied is true only when that delta is positive. OverflowProgress is raw unused progress, with no multiplier or reward claim. ReachedMastery is true only when this accepted operation first moves a record from below 100 to 100. Rejected results have zero applied/overflow and false transition; registry failures and nonpositive inputs through the detailed profile API create no record. Already-mastered records return no delta or overflow. No low-level reason enum is added; the mentor service preserves all existing reason values and precedence.

**Arithmetic:** AUD-09's widened `(long)Progress + amount` and cap at 100 are retained. After valid positive mutation, actual delta is `Progress - previousProgress` in 1..100; raw overflow is `amount - appliedProgress` in 0..int.MaxValue-1, safely representable without addition/wraparound. The guard ensures previous progress is below 100 and amount is positive. Examples: 90 + 50 => applied 10, overflow 40, first mastery true; 99 + 2 => applied 1, overflow 1; 0 + int.MaxValue => applied 100, overflow int.MaxValue-100. Rejected 100 + 50, 40 + 0 and 40 - 10 return no overflow or transition. Rank thresholds/storage remain unchanged.

**Compatibility exception:** the legacy boolean profile API has an established side effect: a first registered zero/negative grant creates an empty Unranked record while returning false. AUD-09 permanently tests this. Only that wrapper preserves this behavior through a shared GetOrCreateRecord helper before delegating; the new detailed API and rejected mentor application are mutation-free. Existing records never lose progress on rejection. This distinction is explicit rather than silently changing the older API. Existing callers depending on the erroneous requested ProgressApplied will now receive the correct smaller delta for capped grants.

**Regression evidence:** 373 new assertions under the existing Player Progression / Combat Validation group, with a dedicated AUD-10 subsection. The acceptance matrix covers normal/exact/capped grants, 99, int.MaxValue from 0 and 50, already-mastered records, zero/negative/int.MinValue, all operation fields, unchanged ranks, final-state consistency and bool delegation. Further cases cover unknown/null/empty IDs, missing registry/profile, null/unavailable teaching, block-reason precedence, fresh/populated no-mutation rejections, compatibility-only empty records and immutable detached result facts. The integrated workflow discovers a mentor, raises trust, evaluates a teaching, applies the corrected delta without changing trust, round-trips schema-1 mastery JSON into fresh/populated profiles and rejects repeat progression after restore. Legacy inconsistent saved ranks still derive from progress, and missing mastery JSON remains safe.

**Verification:** restore, build (`--no-restore -v:minimal`), full smoke (`--no-build`) and `git diff --check` all exit 0. Build has 0 warnings/errors. Final counts: V1 2 / events 38, Shared 16, V2 371, V3 100, Progression 1657, total **2146**. AUD-01 68, AUD-02 139, AUD-03 107, AUD-04 199, AUD-05 175 and AUD-09 186 remain unchanged and green. Smoke uses expanded permission for the existing temporary-file tests. No Unity Editor or Play Mode verification is claimed.

**Echoes readiness and limits:** Part I records the approved 1:1 initial generation, configurable future multiplier default 1, strict transfer capacity, no charging for unapplied progress, and open passive/efficiency/rounding decisions. Echoes remains MUST HAVE / LOCKED DESIGN and **CONCEPT ONLY**. Raw overflow is not approved-origin eligibility, a balance or an exactly-once award. No multiplier configuration, RNG, passive, spendable resource, ledger, transfer, save schema or reward persistence was introduced. A result can be retained/replayed by callers; future ownership/claiming must enforce eligibility, atomicity and anti-replay. Repeating a still-unmastered grant can advance mastery again, as before. Loading an earlier pre-mastery save or using SetProgress can recreate a transition; no durable claim history is implied. Thread safety, broader transactions, eligibility/retention reconciliation and historical-save migration remain outside this fix. AUD-11, AUD-12 and all other open findings retain their priorities. Stop for independent review; do not start the next audit.

### AUD-11 Closure: Resource Spending and Pool Invariants

**Status: RESOLVED (2026-11-03).** Started from fetched `origin/main` at `0b5759c`, with AUD-01 through AUD-05, AUD-09 and AUD-10 integrated/resolved. Baseline restore/build/smoke exited 0: 2146 assertions, 38 V1 events and zero build warnings/errors. Priority 5c was next; AUD-12 and AUD-06/AUD-07/AUD-08 remain open. Echoes remains CONCEPT ONLY and all Part I creative commitments are unchanged.

**Spending reproduction:** the permanent negative-spend regression in [ResourcePoolSpendSmokeTests.cs](../src/Game.SmokeTests/ResourcePoolSpendSmokeTests.cs), registered in the existing Shared group, ran before production edits. `new ResourcePool(100).TrySpend(-10)` printed `accepted=True, Current=110, Max=100` and threw InvalidOperationException; the normal smoke command exited **1**. Root cause: the sole guard was `amount > Current`, so subtracting a negative amount created resource; int.MinValue also risked arithmetic wraparound. Production diff was empty during this red run. After correction the same regression prints `accepted=False, Current=100, Max=100` and passes.

**Chosen minimal correction:** `TrySpend` rejects `amount < 0 || amount > Current` before arithmetic. For valid state, remaining subtraction has `0 <= amount <= Current <= Max`, so it cannot underflow or create resource. Zero remains true with unchanged state, including an empty or zero-maximum pool. Positive exact-balance spending succeeds and depletes to zero; insufficient amounts return false without changing either property. Constructors still use `Math.Max(0, max)` and begin full, including negative/int.MinValue maximum inputs clamped to zero. No public signature, exception contract or alternate resource system was introduced.

**Regeneration investigation and explicit scope justification:** a second permanent regression was independently run first against the same unchanged production implementation. Starting from max 100, spending 50 and then calling `Regenerate(int.MaxValue)` printed `Current=-2147483599, Max=100`, threw InvalidOperationException and exited **1**. Its positive int addition wrapped before Math.Min. This is independently reproducible, not the negative-spend symptom. Deferring it would leave the mission's invariant false for another supported operation of the same canonical container, including ActiveResonance.Refill and large-maximum end-turn Essence regeneration. Under the mission's explicit invariant exception, the smallest necessary adjustment is `(int)Math.Min(Max, (long)Current + amount)` with the existing positive-only guard. The largest valid intermediate is 2 * int.MaxValue, safely inside long; narrowing occurs only after the nonnegative cap. Nonpositive regeneration remains a no-op, legitimate gains/capping and rates are unchanged. This is a local arithmetic adjustment, not a regeneration/refill redesign or resolution of any other audit finding. Its regression now yields Current=100.

**Consumer review:** direct ResourcePool spend consumers are TurnBasedCombatEngine for actor Essence, ActiveResonance.TrySpend and ResonanceReserve.TryConsume; ResonanceProfile delegates active spending and reserve transfer to these wrappers. Existing test setup callers spend only positive amounts. Engine ordering remains authorization/SkillSeal/ownership, cost hooks, Resonance availability, Essence spending, Resonance spending, effects/XP and normal end-turn. ResourcePool regeneration consumers are end-turn Essence (+5 when allowed) and ActiveResonance.Refill. ResonanceProfile already bounds reserve-to-active transfer by missing capacity. No engine, validator, SkillCost, status, ownership or refill implementation changed.

**Compatibility evidence:** registered player skills with positive Essence cost, zero Essence at an empty pool, insufficient Essence and a negative computed cost use the real gateway/engine. Valid actions apply expected damage and XP; invalid/insufficient spends emit the existing ResourceNotEnough event before effects/XP and leave Resonance unspent. Normal blocked end-turn regeneration still occurs, so whole-turn balances can increase even though the rejected spend itself is mutation-free. The integrated workflow performs paid player skill -> rejected negative Essence/active/reserve spends -> rejected positive-cost enemy Resonance request -> fully zero-cost enemy skill -> explicit reserve refill. It checks exact balances, XP, damage, ownership reason and turn progression. All 175 AUD-05 assertions and the existing V3 refill/status tests remain unchanged and green; this does not resolve AUD-06/AUD-07/AUD-08.

**Permanent coverage:** 316 new Shared assertions cover positive/zero/negative/extreme construction; full/partial/depleted negative spending including int.MinValue; the required acceptance matrix; exact int.MaxValue spending; repeated spending/depletion/recovery; unchanged Max and no mutation on rejection; successful spend monotonicity; ordinary/nonpositive/extreme regeneration at max 0, 100 and int.MaxValue; wrapper delegation, capped/insufficient refill and the integrated workflows above. The regeneration oracle uses remaining capacity instead of copying the production widened sum. Existing assertions were not weakened or removed. The only production file is ResourcePool.cs; its pre-existing stored CRLF endings were normalized to LF so plain `git diff --check` passes, without repository-wide formatting/config changes.

**Verification commands and evidence:**
- `dotnet restore Fragments-of-Wisdom.sln`: exit 0.
- `dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal`: exit 0, 0 warnings/errors in baseline, red-test builds and final build.
- `dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build`: baseline exit 0 / 2146; each independently executed red regression exit 1; final exit 0 / **2462**. Final groups: V1 2, Shared 332 (16 existing + 316 AUD-11), V2 371, V3 100, Progression 1657. V1 events remain **38**. AUD-01 68, AUD-02 139, AUD-03 107, AUD-04 199, AUD-05 175, AUD-09 186 and AUD-10 373 are unchanged and pass.
- `git diff --check`: exit 0 after local line-ending normalization. Smoke uses expanded permission for existing temporary-file persistence tests; no parallel runner or special reproduction flag remains.

**Limits:** the bounds guarantee is for sequential supported operations from construction, not repair of reflection-corrupted state or thread-safe multi-resource transactions. ActiveResonance.CanSpend still reports true for nonpositive query inputs; actual TrySpend now rejects negatives. ResonanceProfile.TryRefillFromReserve still treats nonpositive refill requests as successful no-ops. Those pre-existing query/refill semantics are not redesigned here. Negative final Essence costs fail through the existing insufficient-resource event path; invalid definition metadata, SkillCost arithmetic and multi-resource transaction failures are separate contracts, not certified by this fix. No snapshot schema, persistence, mastery, Echoes, enemy resource pool, new combat event, UI or Unity change occurred. Unity Editor/Play Mode was not exercised. AUD-12 remains the next candidate for separate approval; stop after AUD-11 for independent review.

### AUD-12 Closure: Restore Feasibility vs. Applied State

**Status: RESOLVED (2026-11-03).** Started from fetched `origin/main` at `8b36525`, with AUD-01 through AUD-05 and AUD-09 through AUD-11 integrated/resolved. Baseline restore/build/smoke exited 0: 2462 assertions, 38 V1 events, zero build warnings/errors. This closes priority 5d only; AUD-06/AUD-07/AUD-08 remain open in their existing order. Earlier audit closure statements about outstanding AUD-12 work are historical evidence.

**Red reproduction:** the permanent smoke regression first ran against unchanged production code (production diff empty). Both public coordinator overloads received an existing `audit.before` class, a snapshot requesting `audit.after`, invalid mastery and otherwise valid sections. The complete populated destination remained unchanged, but both calls printed `ClassRestored=True`, `MasteryRestored=False`, `FullyRestored=False`. The aggregated assertion `AUD-12: rejected preflight must report all applied-state flags false in both public overloads.` failed with smoke exit 1; the reproduction build exited 0 with zero warnings/errors. Both overloads were exercised before the failing assertion.

**Root cause and correction:** temporary-profile restore booleans were returned as applied-state facts, even when another subsystem prevented any real application. After feasible preflight, the original coordinator also discarded real restore return values. The existing coordinator now keeps feasibility in local variables. Rejected preflight returns all five `*Restored` flags false without touching destinations. Successful preflight runs the existing individual restore services in the unchanged order: Class, Mastery, Mentor relationships, Skill-tree progress, Class-tree bindings when requested. The result uses their actual boolean returns. No second validator, snapshot model, transaction framework or restore service was introduced.

**Diagnostics and participation:** existing enum values `None=0` and `ValidationFailed=1` remain stable; appended `NotApplied=2` distinguishes feasible requested sections skipped because another section failed preflight, and `ApplicationFailed=3` reports a real operation returning false. Messages identify subsystem and phase; application-failure messages warn that partial changes may remain. Multiple invalid sections retain their own ValidationFailed reasons. An unrequested binding section has no failure reason/message and `ClassSkillTreeBindingsRestored=false`. The additive `ClassSkillTreeBindingsRequested` fact makes that explicit, and FullyRestored evaluates only the requested subsystems. Both public Restore signatures and all four public result constructors remain compatible; existing constructors retain their historical defaults, reason mappings and null-message normalization. Consumers of the former four-profile binding-success sentinel should use Requested/FullyRestored; exhaustive enum switches should handle the appended reasons.

**Meaning of success:** a true flag means the requested restore operation completed successfully, including empty legacy data and idempotent no-ops; it does not claim a value necessarily changed. For the reproduced invalid-mastery case, the after result is all restored flags false, mastery ValidationFailed, other requested sections NotApplied, FullyRestored false, with destination class still `audit.before`. On successful four-profile restore, the first four flags and FullyRestored are true while binding requested/restored are both false. On successful binding-inclusive restore, all five flags and FullyRestored are true. Snapshot schema and JSON contracts are unchanged.

**Application-stage investigation:** class assignment validates the registry independently of the previous class. Mastery restore upserts registered records; existing records are not duplicate-registration failures. Mentor restore updates known discovered relationships; undiscovered records retain their existing supported no-op semantics. Tree restore merges valid discovered/unlocked facts, treating repeated supported operations as successful completion. Binding preflight copies registered trees and existing bindings, so stable conflicts with existing class bindings are detected before real mutation; identical bindings remain successful no-ops. Fresh and populated stable-input paths therefore showed no normal preflight/application disagreement. This is evidence for supported serial calls, not a general atomicity proof.

**Reachable failure boundary:** snapshot IReadOnlyList properties retain supplied collection references, not immutable copies. Tests use a deterministic changing-list fixture, explicitly fault injection rather than an ordinary save, to expose changes between preflight and application. In both overloads, mastery becomes a valid record followed by an unknown skill after preflight: the real service returns false after applying the valid record, and the result reports MasteryRestored false/ApplicationFailed while accurately reporting other completed sections. A binding list changes after its preflight enumerations into a conflict with an existing binding: earlier profile applications remain reported true, binding application is false/ApplicationFailed, and existing bindings are preserved. These tests prove actual return values are consumed without adding production test hooks.

**Permanent verification:** `RestoreResultIntegritySmokeTests` contributes 518 assertions through the existing Progression runner. Coverage includes both public overloads, every applicable subsystem preflight failure, existing binding conflicts, simultaneous class/mastery failures, retries, unchanged populated destinations, precise reasons/messages, fresh/populated success, legacy unscoped and scoped trees, multi-tree scopes, binding participation, old JSON missing optional fields, JSON round trips, idempotent reapplication, constructor compatibility and controlled real application failures. Six existing coordinator assertions that expected the defective preflight-success flags/reasons were corrected, not removed; their existing subsection remains 26 assertions.

**Green evidence:** `dotnet restore Fragments-of-Wisdom.sln`, `dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal`, `dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build`, `git diff --check` and `git status --short` all exited 0. Build: zero warnings/errors. Final groups: V1 2, Shared Domain Foundations 332, V2 371, V3 100, Progression 2175, total 2980; V1 events remain 38. Existing AUD-01 68, AUD-02 139, AUD-03 107, AUD-04 199, AUD-05 175, AUD-09 186, AUD-10 373 and AUD-11 316 assertions remain green and included in those totals. Smoke execution used the required environment permissions; no Unity Editor or Play Mode validation is claimed.

**Limits and preserved boundaries:** application remains sequential and continues after a false return, reporting each outcome. A false result can coexist with partial writes within that subsystem. Exceptions still propagate; there is no rollback, snapshot isolation, staging-and-commit or thread-safety guarantee. Honest result facts do not make general restoration transactional. Existing individual restore methods, AUD-01 reference guards, AUD-02 semantic validation, AUD-03 Codex persistence and all snapshot formats remain unchanged. No combat, ResourcePool, mastery arithmetic, mentor teaching, Echoes, The Path Unwalked, Village of Memories, Unity, UI or generator changes occurred. Stop after AUD-12 for independent architectural review; do not start another audit item.

### AUD-06 Closure: Resonance Refill and Canonical Turn Lifecycle

**Status: RESOLVED (2026-11-03).** Branch `codex/aud-06-refill-turn-lifecycle` starts at fetched `origin/main` `3b4f2bf`, including merged AUD-01 through AUD-05 and AUD-09 through AUD-12. AUD-06 was the next authorized priority (6a). AUD-07 and AUD-08 remain OPEN; backlog order is unchanged. Earlier closure statements about unresolved AUD-06 are historical evidence, not the current implementation status.

**Baseline and permanent red reproduction:** restore/build/full smoke exited 0 before production edits: 2980 assertions (V1 2, Shared 332, V2 371, V3 100, Progression 2175), 38 V1 events, zero build warnings/errors. The new normal-runner regression used living combatants, player HP 100, Poison potency 7/duration 3, active Resonance 4/10 and reserve 50. Requesting refill 4 returned true, moved active to 8 and reserve to 46, and advanced turn 1 to 2, but HP stayed 100 and duration stayed 3. With production diff empty, the regression failed with `AUD-06 refill must apply Poison damage and decrement duration exactly once.` and smoke exit 1. Its build exited 0 with zero warnings/errors. After correction the same probe returns true, HP 93, duration 2, turn 2, active 8, reserve 46.

**Root cause and architecture:** ResonanceRefillAction performed the transfer, emitted its own TurnEnded and called AdvanceTurn directly, bypassing both legacy debuff hooks and canonical status completion. The existing engine's start/completion code is now a small internal CombatTurnLifecycle shared by engine and refill. It owns start events, flag reset, hooks, completion regeneration, status boundary, actor defeat and clock advancement. TurnBasedCombatEngine still owns attack/skill orchestration, target damage and target-defeat detection; its public constructors and ExecuteTurn API remain unchanged. There is no second combat/status engine, public lifecycle API, new event type or independent clock. StatusEffectResolver, DebuffContainer and their potency/duration algorithms are unchanged.

**Exact refill ordering:**
1. Preserve ArgumentNullException for null resonanceProfile/runtime, then require live combat/player/enemy. This applies the engine's admission condition to the player action.
2. Query transfer feasibility without mutation through an internal ResonanceProfile method. The query and actual transfer share the existing capped amount calculation. Unaffordable reserve emits ActionBlocked with `resonance.block.empty_reserve`, returns false and runs no turn hooks/ticks.
3. Construct the player-versus-enemy CombatContext; shared Begin emits TurnStarted, resets flags, invokes legacy OnTurnStart and OnBeforeAction, then detects start-boundary defeat. Lethal legacy Poison/Burn returns false before transfer; no completion or clock advance occurs.
4. Attempt the real reserve-to-active transfer. On success emit the existing ResourceSpent message. Refill invokes no skill validator, skill cost passive, offensive resolver, cleanse, XP grant or enemy effect application.
5. Shared Complete regenerates 5 actor Essence unless ResourceRegenBlocked, invokes Debuffs.OnAfterAction (duration decrement/removal), calls StatusEffectResolver.TickTurnBoundary (all DOT, then permitted Regen, then status durations), and checks actor defeat.
6. On terminal defeat emit CombatEnded and set CombatEnded, without TurnEnded or AdvanceTurn. Otherwise emit exactly one canonical TurnEnded and advance exactly once. A transfer that completed before lethal end-boundary DOT still returns true; this is not a survival result.

**Status/debuff semantics:** Poison/Burn StatusEffect instances act at completion, after resource transfer. Their legacy IDebuff counterparts act at start, before transfer. Regen follows DOT regardless of registration order; RegenBlock suppresses healing, while legacy ResourceBlock suppresses Essence regeneration, not Resonance transfer. All supported durations tick once at their existing boundary; expiration removes the effect. Blind modifies an unused offensive context on refill and causes no attack. SkillSeal does not block refill. Legacy Silence's diagnostic ActionBlocked remains unchanged and can coexist with a successful refill; fixing its skill execution inconsistency is AUD-08, not this repair. Turn flags reset on refill and the next attack, so an expired ResourceBlock cannot continue blocking regeneration.

**Lethal outcomes:** tests cover both Poison and Burn through both status models. For end-boundary DOT, transfer and normal Essence regeneration happen before defeat; the status duration decreases once, CombatEnded is emitted and the turn number does not advance. For legacy start damage, no transfer, regeneration, after-action decrement or end-boundary tick occurs after defeat detection. Retrying refill after terminal defeat adds no events, transfers or ticks. Defeat is evaluated at the same existing canonical boundaries, not after each individual effect: this branch does not redesign the resolver's full DOT/Regen batch or add a new death/interruption/anti-revival rule. Once terminal defeat is detected no further completion/tick occurs.

**Resource and failure compatibility:** positive refill consumes only `min(requested, missing active capacity)`, never a partial amount based on insufficient reserve. Amounts zero/negative and already-full active capacity remain successful resource no-ops, even with empty reserve; they retain their existing one-turn cost and now pay normal lifecycle consequences. Oversized requests including int.MaxValue remain capped and conserve Resonance. Null exceptions retain parameter names and occur before mutation. The separately supplied nonnull player profile remains supported even when runtime.ResonanceProfile is null; there is no new enemy refill capability or resource resolver. A custom extensible hook can change reserve after admission; the real transfer is rechecked. If that fails, the already-started turn emits the existing block reason and completes once, without a transfer-success event. A test-only reserve-draining debuff verifies this reachable boundary; it is not a new production status type.

**Events:** ordinary refill now emits TurnStarted -> ResourceSpent (unchanged transfer text) -> ResourceRegenerated -> TurnEnded (canonical turn-number text). Legacy StatusTick, DebuffExpired or Silence diagnostics appear at their established hook locations. The current StatusEffectResolver does not emit individual DOT/Regen events; none were invented. A lethal end replaces TurnEnded with CombatEnded, and a lethal legacy start emits TurnStarted -> StatusTick -> CombatEnded after any prior setup events. No duplicate TurnEnded, BasicAttackUsed, SkillUsed, DamageDealt or XP is produced by refill. Existing normal combat event behavior and the 38-event V1 contract remain green.

**Regression and workflow evidence:** Refill-specific coverage resides in `RefillTurnLifecycleSmokeTests.cs` through the existing V3 runner: 119 new assertions. It covers Poison, Burn, Regen, RegenBlock, SkillSeal, simultaneous effects, order independent of registration, expiry, no double ticks, both lethal boundaries, legacy hooks/flags, resource caps/conservation, empty/insufficient reserve rejection, nonpositive/full-active no-ops, nulls, separately supplied profiles and hook-induced transfer failure. Real attack comparisons verify identical actor status/resource boundaries. The integrated refill -> player skill -> rejected positive-Resonance enemy skill workflow verifies isolated turn ticks, actual cost/XP timing and player-only resource ownership. One old expectation each in AUD-05 and AUD-11 was intentionally changed from player Essence 15 to 20 because canonical completion now regenerates 5; XP, enemy Essence, ownership and transfer assertions remain enforced, and section counts remain 175 and 316. An intermediate full run exposed the AUD-05 old expectation and exited 1; after its scoped correction the full suite passed.

**Executed green verification:** `dotnet restore Fragments-of-Wisdom.sln`, `dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal`, `dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build`, `git diff --check` and `git status --short` exited 0. Build: 0 warnings/errors. Final groups: V1 2, Shared 332, V2 371, V3 219 (100 original + 119 new), Progression 2175, total **3099**. V1 events: **38**. AUD-01 68, AUD-02 139, AUD-03 107, AUD-04 199, AUD-05 175, AUD-09 186, AUD-10 373, AUD-11 316 and AUD-12 518 remain green. Smoke used expanded execution permission for existing filesystem tests. No Unity Editor or Play Mode validation is claimed.

**Limits and scope:** live-combat admission incidentally rejects post-combat refill, but AUD-07 remains OPEN until its independent reproduction/verification and review; no general ended-combat action audit was performed here. AUD-08 Silence is untouched. Trusted custom hooks and externally mutable profiles do not gain thread-safety, rollback or reentrancy guarantees. The two existing status representations remain separate; this extraction shares lifecycle, not their data models. No save schema, new status formula, ResourcePool arithmetic, resource ownership policy, automatic refill/buff/passive, tank, mastery, Echoes, narrative, reward, UI, Unity or generator change occurred. Stop after AUD-06 for independent architectural review, without starting AUD-07/AUD-08.

### AUD-07 Closure: Combat-Ended Refill Boundary Verification

**Status: RESOLVED (2026-11-03), verification-only closure.** Branch `codex/aud-07-terminal-refill-boundary` starts from fetched `origin/main` at `f598b97`, which merges AUD-06 commit `32be0b44b669d648351ecbbcc5027a7fd52e5e36`. AUD-07 was the next unresolved priority, 6b. AUD-08 remains OPEN and the backlog order is unchanged. Earlier closure references to pending AUD-07 verification remain historical evidence.

**Historical defect versus current main:** the original audit recorded a refill accepted after EndCombat, transferring resource and advancing turn 2 to 3. That is preserved historical evidence, not a newly reproduced failure. AUD-06 added the guard `runtime.CombatEnded || !runtime.Player.IsAlive || !runtime.Enemy.IsAlive` before transfer feasibility, context construction and lifecycle hooks. The current-main public-API probe starts combat, explicitly ends it while both actors remain alive, then requests refill 4 with active 4/10 and reserve 50. It prints `accepted=False, active=4, reserve=50, turn=1, unchanged=True`. The complete captured state and event log remain equal. The first expanded suite passed with exit 0 against unchanged production. **No red phase was possible for the original defect on this main; the guard was not removed or bypassed to manufacture one. No production correction was needed.**

**Public reachability and terminal matrix:** all tested states are reachable without reflection, new setters or production APIs. EndCombat explicitly sets the flag regardless of actor health. ReceiveDamage can defeat an actor without updating the separate runtime flag. Normal engine actions establish defeat through their existing lifecycle. The permanent matrix covers:

| Terminal scenario | Public preparation | Result |
|---|---|---|
| Ended, both actors alive | StartCombat -> EndCombat | Rejected, unchanged |
| Ended, player defeated | ReceiveDamage -> EndCombat | Rejected, unchanged |
| Ended, enemy defeated | ReceiveDamage -> EndCombat | Rejected, unchanged |
| Player defeated, ended flag false | ReceiveDamage only | Rejected; flag remains false |
| Enemy defeated, ended flag false | ReceiveDamage only | Rejected; flag remains false |
| Ended after normal attack | Real basic attack defeats weakened enemy | Rejected, unchanged |
| Ended after skill | Registry-validated player skill defeats weakened enemy | Rejected; prior skill XP/costs remain unchanged |
| Ended after Poison/Burn boundary | Real action followed by lethal StatusEffect DOT | Both rejected, unchanged |
| Ended after legacy Poison/Burn start | Real action stopped by lethal legacy debuff | Both rejected, unchanged |
| Repeated attempts | Each of the eleven terminal cases, twice per amount | Rejected, unchanged |
| Active combat control | Living actors and available reserve | Accepted; one normal refill turn |

Each terminal case receives amounts `4`, `0`, `-1`, `int.MinValue`, `int.MaxValue`, twice each: 110 matrix calls plus the historical-sequence current-main probe. Nonpositive/full-pool resource no-op rules cannot bypass the earlier terminal guard. The positive active control confirms that the existing guard is not an unconditional rejection and that the hook observers see a real turn.

**Rejection invariants:** immediately before every rejected call the test materializes an immutable comparison string using System.Text.Json over anonymous test-only value records, not live domain references or a new save schema. It captures active/reserve Current/Max; both actors' HP, Essence, turn flags, status identities/potencies/durations and legacy debuff identities/durations; turn number and CombatEnded; tracked skill XP/level/application; all passive callback counts; per-actor turn-start, before-action and Tick counters; and every event type/message. Rejection returns false and the entire representation remains identical. Stale ResourceRegenBlocked is deliberately retained to detect even a silent flag reset. No damage, healing, spending, regeneration, duration decrement, XP, passive execution or lifecycle hook occurs. The existing direct damage API does not automatically synchronize CombatEnded, and refill correctly leaves these stale terminal states untouched rather than silently repairing them.

**Events and null precedence:** terminal calls append no events, including no ActionBlocked, TurnStarted, ResourceSpent, ResourceRegenerated, TurnEnded or CombatEnded. The active control retains TurnStarted -> ResourceSpent -> ResourceRegenerated -> TurnEnded. Null ResonanceProfile still throws ArgumentNullException with `resonanceProfile`; null runtime with a valid profile reports `runtime`; both-null reports `resonanceProfile` first. Active and terminal fixture captures remain unchanged on all six exception cases. Exceptions are not converted into false merely because the supplied runtime has ended.

**Verification evidence:** the unmodified baseline restored/built/ran successfully: 3099 assertions, 38 V1 events, 0 warnings/errors. `TerminalRefillBoundarySmokeTests.cs` adds 247 assertions under the existing V3 runner; no AUD-06 assertions were changed or duplicated wholesale. Final groups: V1 2, Shared 332, V2 371, V3 466 (100 original + 119 AUD-06 + 247 AUD-07), Progression 2175, total **3346**; V1 events **38**. AUD-01 through AUD-06 and AUD-09 through AUD-12 remain green with unchanged subsection counts. The exact commands `dotnet restore Fragments-of-Wisdom.sln`, `dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal`, `dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build`, `git diff --check` and `git status --short` exit 0. Build has zero warnings/errors. Smoke uses expanded execution permission for existing persistence tests. Production diff is empty; existing AUD-06 tests, status/resource algorithms, ownership and events are unchanged.

**Limits and scope:** this verifies sequential calls through the public refill API with valid constructed runtimes. It does not promise thread safety, reentrant-hook safety, rollback, automatic flag synchronization, or that direct mutable entity/resource APIs become combat-aware. It is not a general audit of all action types or malformed runtime internals; source inspection of the engine's existing attack/skill admission found the analogous live-combat guard, not a separate reproduced defect. No new production API, lifecycle, resource framework, gameplay behavior, snapshot schema, Unity integration or visual assets were added. No Unity Editor/Play Mode verification was performed. AUD-08 remains open; stop after AUD-07 for independent architectural review, without a PR, merge or another audit item.

### AUD-08 Closure: Silence / Skill Seal Execution Integrity

**Status: RESOLVED (2026-11-04).** Branch `codex/aud-08-silence-action-integrity` starts from fetched `origin/main` at `171f361`, with AUD-01 through AUD-07 and AUD-09 through AUD-12 integrated/resolved. AUD-08 was the last reproduced open finding and the only authorized repair. This closure does not authorize backlog priorities 8/9, Defend/Wait or a broader combat redesign. Earlier closure descriptions of pending AUD-08 remain historical evidence.

**Baseline and red evidence:** restore/build/full smoke exited 0 before production changes: 3346 assertions, 38 V1 events, zero warnings/errors. The permanent real-engine probe supplies a living player/enemy, active Silence duration 3, a registered class-allowed skill with valid progress, Essence 20, active Resonance 10, skill Essence cost 10/Resonance cost 2 and deterministic random input. Against unchanged production and generator code it printed `blocked=1, applied=True, XP=10, enemyHP=94, Essence=15, Resonance=8`. The assertion `AUD-08 a skill reported blocked by Silence must not execute, grant XP, damage or spend skill resources.` failed with smoke exit 1; the reproduction build exited 0 with zero warnings/errors. Essence 15 includes the original cost followed by normal +5 completion regeneration. After correction the same probe prints `blocked=1, applied=False, XP=0, enemyHP=100, Essence=20, Resonance=10`.

**Root cause and chosen representation:** SilenceDebuff.OnBeforeAction emitted an observation but supplied no decision for the engine to consume. CombatContext now carries the internal per-action bool SkillsBlockedBySilence, default false in each fresh context. Active Silence sets it true; multiple instances are idempotent, while already-expired instances do not set it. The hook emits no ActionBlocked itself. The engine consumes the fact only inside its existing Skill branch and returns through the existing EndTurn/CombatTurnLifecycle path before cost passives or spending. The bool describes a skill capability restriction, not whether the current action failed. Therefore no action-intent field, constructor change, public API, new enum/status, event-log scan, validator or global state is needed. Basic attack and refill legitimately run the same hooks but never interpret the skill-only restriction as their own rejection.

**Deterministic precedence:** existing admission, cognition and start hooks retain their order. Start-boundary defeat wins and prevents both skill execution and a spurious Silence diagnostic. For a surviving skill actor: configured ICombatSkillTurnValidator first (including null/blocked result handling), then Silence, then SkillSeal, then the existing positive non-player Resonance ownership check, cost calculation/passives and resource checks. The gateway and validation result types are unchanged. The selected blocker alone emits the engine's ActionBlocked and returns; Silence plus SkillSeal chooses Silence, Silence plus a real gateway ID mismatch chooses the validator, and Silence plus insufficient Essence/Resonance chooses Silence without reaching cost hooks. Without an optional validator, Silence still blocks through the same engine branch. This adds Silence at the existing status gate without reordering unrelated checks.

**Execution and event contract:** the Silence message is `<actor> is silenced and cannot use skills.` One rejected attempt has one meaningful ActionBlocked even with multiple Silence records. No Skill.Apply, SkillUsed, skill ResourceSpent, offensive damage/debuff request, CleanseResolved or XP occurs. The test-only executable effect probe requests a damage bonus, Poison and partial cleanse; a successful post-expiry control demonstrates those effects actually execute when allowed. Surviving blocked turns emit TurnStarted -> ActionBlocked -> ResourceRegenerated when permitted -> optional DebuffExpired -> TurnEnded, then advance once. Existing hook StatusTick and other legitimate effects retain their locations. Basic attacks and refill under Silence/SkillSeal emit their normal action events with no misleading block. Existing AUD-06's one assertion that required the old misleading refill diagnostic was corrected to require its absence; no coverage was deleted and its 119-assertion count remains unchanged.

**Lifecycle and duration guarantees:** active one-turn Silence blocks the attempted skill before completion expires it; expiration cannot retroactively execute that attempt. Repeated attempts decrement once each, and the next fresh context after expiration permits the skill normally. Legacy start Poison/Burn can kill before the skill gate; there is no extra completion, cost, XP, ActionBlocked or turn advancement afterward. Completion Poison/Burn can kill after rejection; CombatEnded replaces normal TurnEnded and advancement. ResourceBlock still suppresses Essence regeneration and other legitimate pre-action penalties remain. Zero spending is not a promise of unchanged whole-turn Essence: normal completion may regenerate it. No status potency, duration algorithm or shared lifecycle code changed.

**Enemy and resource ownership:** Silence applies to the acting enemy as well as the player. A silenced enemy pays no skill Essence or player Resonance and receives no XP. After expiration a zero-Resonance enemy skill may execute and pay enemy Essence; a positive-Resonance enemy skill remains blocked by UnsupportedResonanceOwnership without touching the player pool. Existing AUD-04 metadata and AUD-05 ownership regression sections remain green and unchanged. Standalone access validators continue returning access facts; the execution-time status gate belongs to the engine, not a second validator.

**Permanent verification:** `SilenceActionIntegritySmokeTests.cs` contributes 220 assertions through the existing V3 runner. The matrix covers legacy/no-validator, explicit legacy gateway and player-progression gateway; Silence alone, SkillSeal alone, both, independent gateway rejection, insufficient Essence/Resonance and multiple Silence instances; non-skill attack/refill compatibility; all skipped skill effect/event/resource/XP paths; exact normal event order; legitimate hook damage/resource blocking; one/two-turn expiration and successful follow-up; already-expired Silence; lethal Poison/Burn at both boundaries; enemy zero/positive Resonance ownership. Existing AUD-01 68, AUD-02 139, AUD-03 107, AUD-04 199, AUD-05 175, AUD-06 119, AUD-07 247, AUD-09 186, AUD-10 373, AUD-11 316 and AUD-12 518 assertions remain green. V1 still emits 38 events; the original V3 foundation remains 100 assertions.

**Generator consistency:** modules `01_Common_And_Combat_Core.ps1` and `02_Status_Skills_Resolvers_Battle.ps1` maintain copies of all three modified production files. Only the new context fact, Silence hook and pre-cost engine check were mirrored. PowerShell AST parsing reports zero syntax errors; extracted emitted CombatContext/SilenceDebuff text matches production after newline normalization, and the historical engine includes the same Silence gate before cost calculation. The generator was not executed and no snapshot tree was rewritten. Its historical engine predates the current validator, Resonance and shared lifecycle integrations; this narrow update prevents reintroducing the Silence observation/authorization bug in those copies, not broader historical-generator drift. It remains unsafe to regenerate over the current checkout without a separately approved migration. The canonical tooling warnings still apply.

**Executed verification:** `dotnet restore Fragments-of-Wisdom.sln`, `dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal`, `dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build`, `git diff --check` and `git status --short` exit 0. Build: zero warnings/errors. Final groups: V1 2, Shared 332, V2 371, V3 686 (100 original + 119 AUD-06 + 247 AUD-07 + 220 AUD-08), Progression 2175, total **3566**. Baseline 3346 -> 220 new assertions -> 3566; V1 events **38**. An intermediate diff-check exited 1 because newly inserted CRLF lines in two legacy files were reported as whitespace; only those added line endings were normalized and the final check passes. Smoke uses expanded permission for the existing filesystem persistence tests. No Unity Editor/Play Mode validation is claimed.

**Limits and stop boundary:** hooks still run before skill validation and may have legitimate side effects; this is not a transaction or rollback mechanism for arbitrary trusted hooks/skills. Trusted assembly code can mutate context facts, and direct calls to ICombatSkill.Apply outside the engine are not execution-authorized by this change. The two status representations remain distinct; neither generalized stacking nor action categories beyond existing public paths were introduced. No save/schema, ResourcePool arithmetic, refill lifecycle, ownership policy, mastery/Echoes, narrative, Unity/UI or new gameplay skill was changed. All twelve reproduced audit findings are now resolved within their stated limits, not a claim of full-game correctness. Stop after AUD-08 for independent review; no PR, merge or next backlog work.

### Architectural Risks and Inferences

The following are source-grounded risks, **not additional reproduced findings**:
- A folder-only assembly boundary cannot prevent every domain/application dependency leak.
- Several staged restore checks and semantic validators can diverge; duplicated validation rules need ownership, not another parallel restore implementation.
- Public mutable profiles and legacy aggregate tree queries allow consumers to ignore intended scope/ownership.
- Merge-style restoration needs an explicit policy for loading over nonempty state; thread safety and multi-writer save semantics are not established.
- Two mastery concepts and two status/event systems need explicit adapters or eventual decisions, not opportunistic rewrites.
- Legacy reward replay and incomplete state persistence may break a future economy/full-session restore.
- Large historical documentation and smoke classes raise maintenance cost; atomic test organization must preserve counts and behavior.

<a id="reconciliation"></a>
## Contradictions Resolved and Decisions Still Open

| Older claim / source | Current interpretation and evidence |
|---|---|
| Bible said tests were not executed; archive lists changing counts | Audit execution and current rerun in Part IV are authoritative; 899 is not the sum of archived historical checkpoints |
| Old backlog says active tests still use V4SmokeTests | Source uses PlayerProgressionFoundationSmokeTests plus ProgressionRuntimeBootstrapSmokeTests; completed neutral test/bootstrap/doc renames are recorded below |
| Essence replaced by Resonance | Engine still charges actor Essence and optional runtime active Resonance |
| All restore state is atomic after preflight | AUD-01/AUD-02 guards and AUD-12 honest application results are verified; absence of rollback/isolation still invalidates a general atomicity guarantee |
| Codex save preserves discovered/completed facts | AUD-03 now verifies supported facts through optional schema-1 ID lists plus enriched records; historical saves cannot regain facts never written |
| Skill mastery means one unified system | XP SkillProgress mastery and 100-point MasteryProfile retention remain separate |
| Class identity already restricts every mastered cross-class skill | Current retention admits any registered mastered skill; restricted eligibility is proposed, not implemented |
| Silence and SkillSeal are equivalent blockers | AUD-08 now makes both block skills before costs/effects; their data models and duration lifecycles remain distinct, with Silence preceding SkillSeal after the configured validator |
| Unity adapters mean a playable or fully saved Unity game | Adapter source exists outside solution with narrower state; no Editor, scene or Play Mode verification |
| Generator commands are the current workflow | Historical snapshot generation can overwrite output and recreate retired docs; not part of current solution build |
| Session naming implies automatic file loading | Loaded session/bootstrap compose prepared state; disk load remains explicit |

**OPEN DECISIONS:** future per-actor Resonance capabilities beyond the approved player-only slice contract; replacement vs merge save loading and durable migration; retained-skill class eligibility; bridging XP and retention mastery; mentor progression/slot commitments; Codex completion semantics; hybrid input/time authority; Sanctuary milestone selection and repetition; town capability mapping; final Nahual terminology; full narrative/content economy; Unity framework/platform and presentation boundary. AUD-04 settles definition/request identity as full metadata value equality; AUD-05 settles current Resonance ownership as player-only with unsupported positive non-player costs blocked; AUD-06 settles refill participation in the existing canonical turn lifecycle without redesigning its status formulas. No implicit approval follows from documenting a candidate solution.

<a id="governance"></a>
# Part VI: Development Governance

## PETTO Mode and Atomic Work

**Foundation -> Workflow Verification -> Documentation -> Audit -> Next Atomic Step.**

Current priority remains the incremental vertical-slice backend foundation, not full-game expansion unless explicitly requested. Read this Bible's current status, audit and backlog before work. Confirm repository structure, branch and worktree; identify relevant code, provide a short plan and preserve unrelated user changes. Require explicit approval for the proposed implementation direction, merge, deployment or destructive cleanup. A specific authorized task is not permission to implement adjacent features.

Use small branches and commits with one purpose. Keep gameplay fixes separate from naming, tooling and documentation-cleanup work. Documentation/verification updates necessary to explain the chosen mechanic belong in its closure, not an unrelated redesign. Before committing inspect status and exact diff; summarize changed files. Do not reset/revert user work.

Prefer domain-first, small focused classes and one important public class per file. Narrow exceptions: tightly related tiny enums, private helpers and generated DTOs. Do not add giant MonoBehaviours, parallel gameplay/save engines or hidden cross-domain mutation. Keep gameplay independently testable. No generator-wide rewrite or broad bootstrap migration without explicit authorization.

Preserve serialization compatibility, stable IDs and Unity GUID/.meta/serialized references. Historical version prefixes are provenance or compatibility identifiers, not evidence of unfinished behavior. Name new active APIs by responsibility; avoid sweeping rename churn or mixing numerical/logic repairs into naming work.

<a id="documentation-format"></a>
## Canonical Documentation Format

**Markdown (.md) is the canonical format for project specifications, decisions, contracts, audits and workflow documentation.** This Bible remains the one governing specification. README/AGENTS/tool README files are entry points, not parallel specifications. Do not create versioned Bible copies.

Binary exports are non-authoritative. Preserve original images, editable diagrams and external/mixed references where Markdown cannot faithfully represent their information. Retire a tracked specification only after a source comparison confirms all meaningful text, tables, diagrams, comments and requirements are preserved. Keep provenance, source hashes and unresolved conflicts as OPEN DECISION. Do not delete untracked personal files without explicit authorization.

No broad extension-based ignore is appropriate: DOCX/PDF can be legitimate references or editable assets. This inventory found no confirmed generated-export directory requiring a .gitignore rule, so .gitignore remains unchanged. A future export workflow may ignore only its specifically identified generated output path.

This addendum preserves commit adc751cf exactly in history and adds to its Bible without removing or rewriting its existing lines. Its earlier binary inventory and 67-link/31-anchor verification are dated evidence from that commit, not the final post-standardization inventory. See the [new source ledger](#binary-document-ledger) for current dispositions. The twelve findings and the single current backlog retain their previous content and priority.

<a id="workflow-verification"></a>
## Workflow Verification Checklist and Definition of Done

After any significant mechanic, pause expansion and create a verification iteration; suggested branch `codex/<mechanic>-workflow-verification`.

- Build passes; smoke tests pass.
- Record assertion counts by group and V1 event count 38; retain the V3 100-assertion baseline unless an explicitly scoped test addition changes it.
- Update this Bible's current status, findings and single backlog for completed, deferred or reprioritized work.
- Confirm no parallel system was introduced and domain/application boundaries remain Unity-free.
- Stateful mechanics have save/restore coverage, including nonempty destinations and explicit failure behavior.
- Validate stable IDs through the relevant registries; test unknown/duplicate IDs and malformed inputs.
- Preserve old JSON compatibility and document schema/default/legacy handling explicitly.
- No generator-wide rewrite unless requested.
- Run at least one integrated E2E workflow crossing the mechanic's real boundaries.
- Inspect diff/status and report every changed file, behavior/architecture risks and test results.
- When Unity assets/adapters are touched: open affected scenes, check Console, inspect prefabs/ScriptableObjects, enter Play Mode and confirm serialized references; otherwise report Unity not exercised.

Use the [verification commands](#verification). Green counts alone do not close an audit finding; its acceptance direction needs a targeted regression and integrated verification. Record environmental failures honestly and distinguish fresh execution from inherited evidence.

<a id="current-backlog"></a>
## Single Current Audit-Driven Backlog

**PROPOSED PRIORITY, PENDING EXPLICIT ARCHITECTURAL APPROVAL.** This replaces the old naming-first ordering as the recommended roadmap, not as authorization to implement fixes. The archived backlog is historical only. The explicitly authorized AUD-01 through AUD-12 missions are complete within their documented limits. Every implementation item must complete the PETTO loop before the next. P8A and P8B were subsequently authorized and completed as separate API naming/compatibility steps. P9 design/audit, P9.1 composition, P9.2 read-only projections, P9.3 intent/event boundary, P9.4 encounter turn flow and the subsequently authorized P9.5 external headless verification are complete within their stated limits; visual implementation remains unstarted. Remaining priorities retain their existing order and require separate approval. Stop after P9.5 for independent architectural review and merge; P9.6, Defend/Wait and all later implementation remain unauthorized.

**Explicit AUD-05 approval and closure (2026-11-03):** investigation paused at the design gate, then the user approved Option B. Player-only skill resource ownership is now enforced and verified by 175 permanent assertions. Existing priorities and AUD-06/AUD-07/AUD-08 scope are unchanged; no further implementation is authorized by this report.

| Priority | Atomic purpose or approval gate | Traceability / acceptance |
|---|---|---|
| 1 (complete) | Validate real restore destinations before mutation | AUD-01 RESOLVED; null destinations rejected before target mutation; 68 regression assertions; individual restore methods unchanged |
| 2 (complete) | Wire registry-aware semantic preflight into actual restore | AUD-02 RESOLVED; both coordinator overloads reject invalid scopes/nodes/discovery/prerequisite closure without mutation; 139 new assertions |
| 3 (complete) | Preserve legacy Codex round trips | AUD-03 RESOLVED; optional schema-1 ID lists, existing merge semantics, full supported Codex round trip; 107 new V2 assertions |
| 4 (complete) | Validate canonical skill metadata | AUD-04 RESOLVED; authoritative registry resolution and full metadata equality; 199 regression assertions; identical-value copies and engine lifecycle preserved |
| 5a (complete) | Repair bounded mastery arithmetic | AUD-09 RESOLVED; widened sum capped at 100; unchanged rank thresholds, int storage, no-op contracts and schema; 186 new assertions |
| 5b (complete) | Report actual teaching delta | AUD-10 RESOLVED; canonical immutable requested/applied/raw-overflow/first-mastery facts, actual teaching delta, bool compatibility; 373 new assertions; no Echoes economy |
| 5c (complete) | Reject invalid ResourcePool spending | AUD-11 RESOLVED; negative rejection, zero-cost compatibility, safe pool bounds including necessary capped regeneration; 316 new assertions |
| 5d (complete) | Separate restore feasibility from applied facts | AUD-12 RESOLVED; applied flags from real operations, all false on preflight rejection, phase-aware diagnostics and explicit binding participation; 518 new assertions |
| 6 (complete) | Audit/approve actor resource ownership and refill lifecycle | AUD-05 RESOLVED with approved player-only contract; AUD-06 RESOLVED with 119 shared lifecycle assertions; AUD-07 RESOLVED by 247 independent terminal-boundary assertions against unchanged production |
| 7 (AUD-08 complete) | Correct remaining action/status contracts | AUD-08 RESOLVED: authoritative Silence blocking and coherent events; 220 assertions; Defend/Wait remain outside this closure and require a separate public-contract decision |
| 8a (complete) | Responsibility-based restore API naming | P8A: canonical ProgressRestore APIs with V4 forwarding compatibility; 58 assertions and old-binary verification; no persistence rename |
| 8b (complete) | Responsibility-based combat diagnostic naming | P8B: canonical diagnostic views and typed constants; legacy constructors, enum strings and JSON intact; 138 assertions and old-binary verification |
| 9 (audit, P9.1 through P9.5 complete; visual implementation not started) | Visual boundary and playable slice | P9 audit preserved; public-only headless consumer verifies existing single-owner boundary; P9.6 remains unauthorized pending independent review and merge |
| 10 | Revisit long-term systems behind their design gates | Town after visual boundary, Sanctuary after milestone inputs, hybrid combat only after dedicated redesign audit |

Each 5a-5d is a separate repair candidate, not a combined cleanup branch. Each combat repair after priority 6 also needs one purpose. Historical proposed branch names `codex/rename-progress-restore-api`, `codex/rename-combat-validation-diagnostics` and `codex/visual-smoke-demo-design-audit` remain traceable here; no branch is created by this document.

### Completed Foundations, Not Claims of Defect-Free Systems

Historical completion records retained: legacy V1/V3 guards; classes/mastery/mentors/teaching, retention, scoped trees and bindings; restore coordinator foundation; validator/gateway/optional engine integration/composition; progression-aware application combat factory; one runtime state under LoadedPlayerSession; PlayerSessionComposition, PlayerSessionBootstrap and ProgressionRuntimeDemoBootstrap; dedicated runtime/bootstrap workflow test organization.

Completed neutral naming: V5RuntimeDemoBootstrap -> ProgressionRuntimeDemoBootstrap; V4SmokeTests -> PlayerProgressionFoundationSmokeTests; RunV4SmokeTests -> RunPlayerProgressionFoundationSmokeTests; reporting labels -> progression/session/bootstrap responsibilities; runtime helpers -> ProgressionRuntimeBootstrapSmokeTests; V4_Classes_Mastery_Foundation.md -> PlayerProgression_Foundation.md (now archived here). These historical file names are not current links. Audit defects reopen specific correctness gaps, not the fact that the foundations exist.

<a id="restore-api-naming"></a>
### P8A: Restore API Responsibility Naming

**Verified 2026-11-04, branch `codex/rename-progress-restore-api`.** Baseline `c38c69d` is the latest fetched origin/main at preparation, with AUD-01 through AUD-12 integrated and resolved. P8A was the earliest remaining atomic naming item. This is a naming/compatibility change, not a gameplay or restoration repair.

**Discovery gate inventory, completed before production edits:**
- Public coordinator: parameterless and `V2ProgressSnapshotService` constructors; two nonvirtual `Restore` overloads with nine and ten parameters, the latter including `ClassSkillTreeBindingRegistry`.
- Public result: four constructors (4 booleans; 5 booleans; 4 booleans + 4 reasons + 4 messages; 5 booleans + 5 reasons + 5 messages), 17 public readable facts, no public setters. The internal participation-aware constructor is implementation detail, not an external contract.
- Failure enum: int-backed `None=0`, `ValidationFailed=1`, `NotApplied=2`, `ApplicationFailed=3`. Caller-supplied undefined enum values are not newly rejected or normalized.
- All in-repository consumers were in PlayerProgressionFoundationSmokeTests, ProgressionRuntimeBootstrapSmokeTests, ProgressRestoreAtomicitySmokeTests, SemanticSkillTreeRestoreSmokeTests and RestoreResultIntegritySmokeTests. No separate production/Unity consumer of these three types was found.
- No reflection/string lookup dependency, result persistence mapping, package publication configuration or SnapshotGenerator copy/reference of these APIs was found. Snapshot JSON uses private V2 DTOs and domain enums, not restore results or failure reasons. The generator was inspected but not executed or modified.
- Active persistence prose and naming backlog referenced the old API; historical audit/archival references remain historical. No evidence establishes a published external contract or proves that external consumers do not exist; retain the full public entry-point surface conservatively.

| Previous public type, retained | Canonical API for new/internal callers |
|---|---|
| V4ProgressRestoreCoordinator | ProgressRestoreCoordinator |
| V4ProgressRestoreResult | ProgressRestoreResult |
| V4ProgressRestoreFailureReason | ProgressRestoreFailureReason |

**Single ownership and compatibility:** ProgressRestoreCoordinator owns the existing preflight/application implementation; ProgressRestoreResult owns completion and diagnostic facts. Their implementations match the baseline after identifier substitution. Both overload families, exception parameter names/messages, application order (class, mastery, mentors, tree, optional bindings), real applied-state reporting and existing public constructor defaults remain unchanged. V4ProgressRestoreCoordinator holds one canonical coordinator and forwards each call; V4ProgressRestoreResult holds its canonical result and forwards all facts, casting reason values without remapping ordinals. The old enum constants reference explicit canonical values. No duplicated restore algorithm, implicit type conversions, new restore framework or obsolete warnings are introduced. Legacy calls add one coordinator wrapper per coordinator and one result wrapper per returned result; canonical calls have no adapter allocation. Public result construction through the old API similarly adds one wrapper.

All five existing consumer files now use the canonical names. The focused compatibility suite deliberately uses both families. Original AUD-01/02/12 matrices and runtime/bootstrap workflows execute the canonical API without copying their matrices into a second suite. Public names, namespace, assembly name, constructor/method signatures and public getter types of the old API remain available; parameter names retain named-argument compatibility. Private fields/setters and stack traces are not preserved contracts. The two API families are distinct nominal types, not aliases or assignable replacements.

**Serialization invariants:** no V2 snapshot DTO, serializer, validation service, file repository, individual restore service or schema-version source changed. Schema remains 1; field names, stable IDs and domain-enum representations are unchanged. Tests verify byte-stable snapshot JSON round trips, numeric persisted mastery ranks, old JSON with only SchemaVersion, all 17 result facts/messages and reason numeric serialization. Restore result enums are not save fields; keeping their numeric values also protects callers that independently serialize diagnostics.

**Verification evidence:** baseline restore/build/smoke passed with 3566 assertions and 38 V1 events. The new permanent suite adds 58 assertions for public surface, four constructor families, enum ordinals/undefined-value preservation, both overloads, successful and repeated merge restore, missing legacy fields, registry and semantic-tree preflight rejection without mutation, null destinations and controlled application failure. Final suite: 3624 assertions (V1 2, Shared 332, V2 371, V3 686, Progression 2233), V1 events 38. Existing audit subsection counts remain green and unchanged. Restore/build/smoke/diff-check/status commands exit 0; build has 0 warnings and 0 errors. No artificial red phase was created.

**Binary compatibility experiment:** archived unchanged baseline source was built in an isolated temporary directory; only its `FragmentsOfWisdom.Game.dll` was replaced with the P8A library. The baseline smoke client then ran without recompilation against the replacement and passed all 3566 assertions / 38 events, exit 0, including its old V4 API calls and constructor coverage. This is concrete repository-client evidence, not certification of arbitrary downstream clients. An initial sandbox-temporary copy was unavailable on a subsequent call; the isolated baseline build replaced that incomplete experiment without changing project files.

**Remaining limits:** unknown external clients, nonpublic reflection and custom serializers require their own verification. Restore remains sequential, merge-based and nontransactional after preflight: no rollback, isolation, new exception handling or stronger concurrent-input guarantee. Unity Editor/Play Mode was not exercised; no Unity assets or adapters changed. Historical versioned DTOs and generator documents remain intact. No P8B diagnostics, P9 visual work, combat behavior or additional audit repair is included.

<a id="combat-diagnostic-naming"></a>
### P8B: Combat Validation Diagnostic Naming

**Verified 2026-11-04, branch `codex/rename-combat-validation-diagnostics`.** Latest fetched origin/main at preparation was `c92202a`; P8A commit `970b5a5f471db46bcbf4076b6bea419857cfe103` was verified as an ancestor. Worktree was clean, AUD-01 through AUD-12 remained resolved, and P8B was the earliest unresolved atomic naming item. The original baseline independently passed 3624 assertions / 38 V1 events, with zero build warnings/errors.

**Discovery and decision gate:** version labels described two different checks, not two interchangeable authorization results. The inventory below was established before editing. Use-result properties are consumed by smoke tests and the gateway; turn-result reasons are consumed by tests. Enum reasons are produced by the validator, asserted in tests and converted to externally visible event text by the unchanged engine. No current snapshot DTO mapping, string-enum converter configuration, property-name reflection dependency or affected SnapshotGenerator fragment was found. No external package publication contract was established; absence of known downstream callers is not permission to remove public members.

| Existing identifier | Actual meaning | Type | Consumers | Canonical name | Compatibility risk / decision |
|---|---|---|---|---|---|
| V3AccessChecked | Required-unlock gate reached; true even when UnlockProfile is missing, not proof of permission or completed evaluation | bool | Use result / progression smoke | UnlockAccessChecked | Preserve legacy property; new read-only view, excluded from JSON |
| V4AccessChecked | Class-or-mastery policy evaluated, regardless of allowed/blocked outcome | bool | Use result / progression smoke | ClassMasteryAccessChecked | Preserve legacy property; new read-only view, excluded from JSON |
| V3BlockReason | SkillAccessPolicy failure detail, currently skill.block.missing_unlock; empty for missing profile | string | Use/turn results, gateway, smoke | UnlockBlockReason | Keep exact string, old wire name and public constructor parameter |
| V4BlockReason | Class/mastery retention failure detail | MasteredSkillRetentionBlockReason | Use/turn results, gateway, smoke | ClassMasteryBlockReason | Same enum type/value, no rename of the retention enum |
| V3UnlockProfileMissing | Required unlock with absent profile in the compatible validator overload | CombatSkillUseBlockReason, 10 | Validator / diagnostics | CombatSkillUseBlockReasons.UnlockProfileMissing | Typed constant, not an enum alias |
| V3SkillLocked | Required unlock not granted | CombatSkillUseBlockReason, 11 | Validator, engine message, smoke | CombatSkillUseBlockReasons.UnlockSkillLocked | Typed constant preserves old ToString and wire name |
| V4ContextIncomplete | Some class/mastery dependencies exist, but not all, in the compatible overload | CombatSkillUseBlockReason, 12 | Validator / diagnostics | CombatSkillUseBlockReasons.ClassMasteryContextIncomplete | Distinct from strict PlayerContextIncomplete; typed constant |
| V4SkillBlocked | Registered skill rejected by class-or-mastery retention policy | CombatSkillUseBlockReason, 13 | Validator, engine message, smoke | CombatSkillUseBlockReasons.ClassMasterySkillBlocked | Typed constant preserves old ToString and wire name |

**One source of facts:** canonical properties forward to existing stored legacy properties, with no additional fields, recalculation, mutation or allocations. Public use-result and turn-result constructors retain all original parameter names/order/types and null-string normalization. In particular `v3AccessChecked`, `v4AccessChecked`, `v3BlockReason` and `v4BlockReason` remain valid named arguments; no optional arguments or public overloads are added. Only private validator helper parameter names and local variables were renamed. Existing private helper defaults remain identical. Gateway projection and two existing test consumers (PlayerProgressionFoundationSmokeTests and CanonicalSkillDefinitionSmokeTests) use the canonical vocabulary; the dedicated compatibility suite intentionally exercises legacy names too.

**Enum and JSON policy:** CombatSkillUseBlockReason is byte-for-byte unchanged. Its 18 members retain values 0 through 17, names/order, Enum.GetNames/GetValues/Parse, ToString, default numeric JSON and opt-in JsonStringEnumConverter representations. Four compile-time constants outside the enum provide responsibility-based source names usable in switch/constant patterns. They do not add enum members or a parallel diagnostic type; Enum.Parse intentionally does not accept those new constant names. For example UnlockSkillLocked still formats as V3SkillLocked, preserving ActionBlocked event text. This is an explicit compatibility tradeoff, not an incomplete enum migration. MasteredSkillRetentionBlockReason remains unchanged.

New result views use System.Text.Json.Serialization.JsonIgnore so the existing eight-field use-result JSON and seven-field turn-result JSON remain exact, without duplicate diagnostic fields. Existing constructor-based deserialization and null normalization are verified. No save/schema/DTO, restore API, persistence service or Unity code is changed. Other/custom serializers or reflection tools that enumerate public properties can see the additive views and need their own integration checks; no universal external-client guarantee is claimed.

**Precedence is unchanged:** explicit null validation context precedes request checks; request/action/skill/progress/definition/ID checks precede LegacyBypass and Enemy structural-only admission. Strict Player mode requires all dependencies before canonical skill metadata validation. Where a registry exists, missing/invalid/conflicting definitions fail before required-unlock policy. The unlock gate precedes compatible partial class/mastery context detection and class-or-mastery policy evaluation. Missing unlock profile and incomplete class/mastery context remain reachable in the compatible overload, while strict incomplete Player context returns PlayerContextIncomplete earlier. The orchestrator may reject incomplete actor dependencies at Classification before any validator result exists. Gateway preserves stage, mode, classification reason, top-level reason and both subordinate reasons; it does not invent checked flags that its original result never exposed.

The engine itself is untouched: configured validation precedes Silence, SkillSeal and cost checks; block events use historical enum strings, consume the established blocked turn, spend no skill resources and grant no skill XP/effects. Resource ownership, refill lifecycle, damage and end-turn behavior are unchanged. Existing AUD-04/05/06/07/08 matrices and runtime/bootstrap workflows remain the authority for those behaviors, not duplicated matrices in the naming suite.

**Permanent evidence:** CombatDiagnosticNamingCompatibilitySmokeTests adds 138 assertions under Player Progression / Combat Validation. Coverage includes all enum names/numbers/parse/string/numeric JSON round trips, switch/pattern constants, legacy named constructors, exact result JSON fields/round trips, real unlock/class/mastered-retention decisions, missing/partial contexts, player/enemy/legacy distinctions, gateway projection, precedence and invalid definitions, exact blocked-engine strings and no cost/effect/XP mutation. All previous audit/P8A/runtime sections remain green with unchanged counts. Final groups: V1 2, Shared 332, V2 371, V3 686, Progression 2371; total 3762 = 3624 baseline + 138 new; V1 events 38. Restore/build/smoke/diff-check/status exit 0; build has zero warnings/errors. No artificial red phase was needed.

**Binary experiment:** `git archive` of baseline `c92202a` (src, Directory.Build.props and solution) was expanded and built in an isolated temporary directory. Only its Game DLL was replaced by the P8B Game DLL, then the baseline smoke DLL was executed with dotnet without recompiling the client. Build and execution exited 0; the old client passed all 3624 assertions / 38 events, including legacy diagnostic consumers and P8A compatibility. This verifies the repository's compiled client, not every possible downstream serializer/reflection integration.

**Boundaries and deferred work:** generator inspected, not executed or edited; no affected copies required synchronization. No new validation engine, gateway, rule registry, combat behavior, P8A restore edits, Defend/Wait, Unity, town or mentor work. Unity Editor/Play Mode remains unverified. P8B is complete within these compatibility boundaries; stop for independent review and do not start P9.

<a id="p9-visual-boundary"></a>
### P9: Visual Boundary and Playable Slice Architecture Audit

**2026-11-04; branch `codex/p9-visual-boundary-design-audit`. DESIGN/AUDIT COMPLETE, VISUAL IMPLEMENTATION NOT STARTED.** This section is evidence and proposed architecture, not approval to implement any roadmap branch. The domain determines what happened; presentation determines how it is experienced. One authoritative player session and one existing combat execution path remain mandatory.

#### Baseline, Sources and Verification

Latest fetched origin/main was `5046047`, containing P8B `13c04ebc7e9c2cf96d76890374838e5478c050d9` and its P8A ancestor. Worktree was clean before branching. The canonical backlog records AUD-01 through AUD-12 resolved and P8A/P8B complete; P9 was the next explicitly approved design item. No new AUD defect is opened here: identified gaps are source-confirmed missing integration capabilities, design risks or platform unknowns, not reproduced gameplay regressions.

Repository inventory: `src/Game` contains Domain, Application, Infrastructure and Slice in one `FragmentsOfWisdom.Game` assembly. `Game.csproj` and `Game.SmokeTests.csproj` both target net10.0; the solution lists the smoke executable, which references Game. Directory.Build.props selects latest C#, disables implicit usings/nullable analysis and disables NuGet audit; no pinned global.json or Unity/package build pipeline was found. Installed SDK visible to this audit: 10.0.204. No package references are declared in these two projects; framework-provided System.Text.Json is still a runtime dependency.

No Assets/Packages/ProjectSettings Unity project, ProjectVersion.txt, scene, asmdef or meta files were found in the repository inventory. The two `src/Game.Unity/Persistence` source adapters are not part of either project and do not prove Editor compatibility. Unity version, rendering pipeline and UI framework remain unselected. Unity/adb/sdkmanager were not on the inspected PATH; standard Program Files Unity/Hub locations were absent and Android environment variables unset. This is limited environment discovery, not proof that no custom installation exists. No Editor, Play Mode, Android build/device, profiling or visual test was executed.

Required baseline commands all exited **0**: `dotnet restore Fragments-of-Wisdom.sln`; `dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal`; `dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build`; `git diff --check`; `git status --short`. Build: **0 warnings, 0 errors**. Counts remain **V1 2; Shared 332; V2 371; V3 686; Progression 2371; total 3762; V1 events 38**. New assertions: **0**. No artificial red phase, generator execution, production edits or test edits. Only this Bible is changed.

Primary code evidence (paths relative to repository root):
- `src/Game/Application/Bootstrap/ProgressionRuntimeDemoBootstrap.cs`, `PlayerSessionBootstrap.cs`; `Application/Runtime/PlayerSessionComposition.cs`, `LoadedPlayerSession.cs`, `PlayerProgressionRuntimeState.cs`; `Application/Integration/ProgressionAwareCombatEngineFactory.cs`.
- `src/Game/Domain/Combat/TurnBasedCombatEngine.cs`, `CombatRuntimeContext.cs`, `TurnRequest.cs`, `CombatContext.cs`, `CombatTurnLifecycle.cs`, `CombatEntity.cs`; `Common/GameEventLog.cs`, `GameEvent.cs`, `GameEventType.cs`.
- `Domain/Combat/Resources/ResonanceProfile.cs`, `ResonanceRefillAction.cs`; `StatusEffects/StatusEffectResolver.cs`, `StatusEffectCollection.cs`, `DebuffContainer.cs`, `BuffContainer.cs`; `CombatSkillTurnValidatorGateway.cs`, `CombatSkillUseValidator.cs`, `CombatActorValidationClassifier.cs`.
- `Domain/Player/PlayerProfile.cs`, `PlayerSkillBook.cs`; `Domain/Skills/SkillProgress.cs`, `SkillDefinitionRegistry.cs` and concrete skill implementations; `Domain/Classes/MasteredSkillRetentionPolicy.cs`; `Combat/AI/BasicAttackEnemyAiStrategy.cs`, `Combat/Outcomes/BattleOutcomeResolver.cs`; `Infrastructure/Random/SeededRandomSource.cs`; `Slice/FoundationVerticalSlice.cs`.
- `src/Game.SmokeTests/ProgressionRuntimeBootstrapSmokeTests.cs` (especially AssertProgressionRuntimeDemoBootstrapIntegratedWorkflow), `PlayerProgressionFoundationSmokeTests.cs`, `CanonicalSkillDefinitionSmokeTests.cs`, `CombatDiagnosticNamingCompatibilitySmokeTests.cs`, `ResonanceOwnershipAuditSmokeTests.cs`, `RefillTurnLifecycleSmokeTests.cs`, `TerminalRefillBoundarySmokeTests.cs`, `SilenceActionIntegritySmokeTests.cs` and `V3SmokeTests.cs`. Existing tests create runtimes/combatants separately from the bootstrap; they are not a production encounter factory.

#### Public Operation Inventory

All Unity-callability entries below are conditional on a separately verified compatible build. Plain C# does not mean the current net10.0 DLL is importable. Public mutable objects should be callable by a trusted application composition boundary, not by arbitrary view components.

| Operation / input -> output | Owner, mutation and lifetime | Events and error behavior | Current evidence / safe presentation use |
|---|---|---|---|
| ProgressionRuntimeDemoBootstrap constructors: 3 registries, optionally bindings -> bootstrap; SessionBootstrap/Composition/LoadedSession/RuntimeState getters | Allocates a new nested session chain; getters return the same live references. Three-argument overload allocates bindings; four-argument overload retains supplied bindings | No events; required null registries propagate ArgumentNullException from composition | Runtime/bootstrap identity and independence tests pass. Create once at app entry, never per view; it does not create PlayerProfile, combatants, runtime, RNG, skill instances or encounter |
| PlayerSessionBootstrap / PlayerSessionComposition constructors and getters | Each constructor creates a new composition/session, not a lookup of an existing saved session; registries retained by reference | Composition rejects null class/skill/mentor/binding registries; no load or events | Tested identity and guard paths. Views must receive the existing owner instead of calling constructors |
| LoadedPlayerSession constructors: registries/bindings, optional factory -> session; RuntimeState getter | Owns one new progression runtime and factory; injected factory may be shared but state is per constructed session | Null factory/registries rejected; no disk loading despite the name Loaded | Tested composition; do not infer restore or combat ownership |
| PlayerProgressionRuntimeState constructor -> state; profile/registry getters | Creates UnlockProfile, ClassProfile, MasteryProfile, MentorRelationshipProfile and SkillTreeProgressProfile; retains all supplied registries/bindings | Null registries rejected; no events; getters leak mutable profile references | Tested ownership/restore workflows. Restore targets these exact profiles; no scene-local copies |
| CreateCombatEngine(IRandomSource, IReadOnlyList<ICombatPassiveEffect>) on demo/bootstrap/composition/session; factory CreateEngine(state, random, passives) -> TurnBasedCombatEngine | Delegation to one factory; each call creates fresh engine/pipeline/gateway referencing session profiles; supplied RNG/passive instances/list are not cloned | Factory rejects null state/random/list; no events; list elements are not validated here | Tested fresh-engine/session-reference behavior. Create one engine per encounter; caller must provide fresh mutable random/passive state |
| CombatRuntimeContext constructors: player, enemy, log, PlayerProfile, optional ResonanceProfile -> runtime; public getters | Retains references; starts TurnNumber=1, CombatEnded=false; does not own/create a progression session | No null guards or events in constructor; downstream calls can throw if graph is incomplete | Tested as fixture input, not a safe public UI factory. Construct/validate graph once behind boundary |
| TurnBasedCombatEngine constructors: random, pipeline, optional validator -> engine | Stores dependencies/resolvers/lifecycle; two-argument constructor deliberately retains legacy validation bypass | No constructor guards here; no events | Factory is recommended. Do not construct a second engine or choose bypass in the visual layer |
| StartCombat(CombatRuntimeContext) -> void | Calls passives/cognition and may change flags/state through hooks; no Started guard, no idempotency guarantee | Hooks/cognition events can precede CombatStarted, then TraitRevealed if requested; invalid runtime graph can throw; no terminal admission guard here | Existing combat tests; application must call once, not on every OnEnable/rebind |
| TurnRequest.BasicAttack(attacker, defender) / SkillAction(attacker, defender, ICombatSkill, SkillProgress) -> request | Holds live references, no mutation or validation; no refill/Defend/Wait factory | No events/guards; malformed requests possible | Existing skill/request smoke. UI sends IDs/intents, trusted boundary builds requests with owned actors and progress |
| ExecuteTurn(runtime, request) -> void | Executes the full turn synchronously: HP/resources/effects/XP/log/turn/terminal state; engine is not tied to one runtime | Ended/dead guard precedes context; null runtime/request/actors unsafe. Validation rejection is a normal completed blocked turn, not an exception; arbitrary skill/passive exceptions are not rolled back | Broad V1/V3/progression/AUD coverage. Call exactly once per admitted intent, never from an animation event |
| CombatSkillTurnValidatorGateway.Validate(request, runtime) -> CombatSkillTurnValidationResult; CombatSkillUseValidator overloads -> CombatSkillUseValidationResult | Read-only access/structural checks; references session profiles; classifier identifies actor by reference, not turn ownership | Structured stage/mode/reasons, no events/costs; malformed facts generally return failure. Does not validate every cost/status/targeting/scheduling rule | AUD-04/P8B/context tests. Suitable for advisory skill access, not a guarantee of successful execution |
| ResonanceRefillAction.Execute(profile, runtime, int amount) -> bool | Mutates supplied player pool and the canonical player turn lifecycle; separate from TurnRequest and skill gateway | Null profile/runtime throw. Terminal returns false without events. Unaffordable transfer returns false + ActionBlocked before TurnStarted. Admitted action shares lifecycle; true does not guarantee survival | AUD-06/07/08 verified. Boundary must always supply runtime's same player pool, not a separate equivalent/new pool |
| GameEventLog.Events -> IReadOnlyList<GameEvent>; Add(type, message) -> void | In-memory append-only public API, live read-only wrapper; Add remains publicly callable | No IDs, timestamps, cursor, subscriptions, removal or thread-safety; GameEvent only Type/Message | Event-order/count tests. Boundary owns writes and copies a range; view must not retain live log |
| CombatRuntimeContext.AdvanceTurn()/EndCombat() -> void; TurnNumber/CombatEnded getters | Public clock increment/terminal mutation, no actor queue or winner storage | These methods emit no events; EndCombat can mark two living actors ended | Terminal AUD coverage. Never expose mutators to UI; presentation reads facts, does not advance the domain clock |
| CombatContext constructor, mutable calculation properties, RequestDebuff/RequestCleanse -> context/void | Per-action scratch state, references actors/log; mutable pending-debuff list | Constructor dereferences actor stats; requests do not apply effects immediately | Engine/skill tests. Never cache or present as a durable encounter model |
| CombatEntity constructors/ReceiveDamage/Heal and public Stats/Resource/effect access | Combat-scoped mutations; Stats has public setters; resources and effect records remain mutable | ReceiveDamage/Heal emit no events; invalid constructor dependencies unsafe | Domain smoke. Project values only; a read-only collection interface does not make records immutable |
| BasicAttackEnemyAiStrategy.ChooseAction(runtime) -> TurnRequest | Builds existing enemy basic attack; no scheduling or terminal guard | Emits AiDecisionMade before engine execution; null runtime unsafe | Legacy/combat tests. Only call after authorized enemy phase and terminal admission |
| BattleOutcomeResolver.Resolve(runtime) -> BattleOutcome | Read-only HP-based result; player-dead checked first, then enemy-dead, otherwise None | No events/null guard; ignores CombatEnded; not a general exit/cancellation result | Outcome/legacy use. Distinguish ended-with-no-winner from victory; no automatic rewards in proposed slice |
| SkillDefinitionRegistry.Register/Contains/Get; SkillProgress ctor/GainExperience/GetBaseCost/GetMasteryCostMultiplier/GetMasteryEffectBonus | Registry stores metadata, not executable skill factories; progress is separately allocated mutable XP/level state | Invalid/duplicate registry registration false, unknown Get null; progress can retain null definition and fail on later use | Skills/AUD-04 tests. No complete iterable playable-action list or owned skill-progress store is provided |
| FoundationVerticalSlice.Run() -> GameEventLog; RunToConsole() -> void | Creates its own legacy PlayerProfile, missions/rewards, combatants and engine, executes scripted actions; not interactive | Fixed historical event trace, console output optional; no reusable live session returned | V1 38-event contract. Retain as regression entry point; do not call it to start the visual slice |

#### Ownership Map and Missing Composition

| Object/fact | Current actual scope / sharing | Proposed owner for the first slice, not implemented |
|---|---|---|
| Demo/bootstrap/composition/LoadedPlayerSession | One chain per constructor call; references exposed | Persistent app composition root creates once and passes an existing session handle across scene/view lifetimes |
| Progression runtime, class/mastery/unlock/mentor/tree profiles | Session-owned; in-place mutable and restoreable | Same session instances; no clone/replacement on encounter enter/exit; no restore while action/presentation batch is pending |
| Definition registries | Caller-supplied references, mutable registration; may be shared between sessions | Shared prepared content with a convention of no registration during combat; immutable registry enforcement is not currently implemented |
| Class-tree bindings | Caller-supplied mutable registry; tests use separate registries per session; reuse is technically possible | One binding registry per session, never copied into a UI registry; definitions can be shared but mutable binding facts cannot |
| PlayerProfile | Separate legacy/cognition/inventory/codex model required by CombatRuntimeContext; absent from LoadedPlayerSession | App root retains one explicitly supplied PlayerProfile for the same player, alongside the existing progression session; do not create one per combat or synchronize legacy class/skill collections as a second access authority |
| Engine, gateway, passive pipeline | Fresh per factory call; gateway reads existing session; pipeline retains caller list/instances | Encounter-owned engine and fresh passive instances/list; no engine in each view |
| Runtime, player/enemy entities, HP/Essence/statuses | Entirely caller-composed; nothing enforces one active encounter | One encounter owner validates and retains the graph; combatants recreated only at explicit encounter start, never on HUD refresh |
| Active/reserve Resonance | Optional runtime reference, player-owned by AUD-05; mutable | Exactly one prepared player pool for the encounter, supplied to both runtime and refill. Carryover between encounters remains OPEN; first scenario has no save/carryover claim |
| SkillProgress and ICombatSkill bindings | Not owned by the bootstrap or metadata registry | Trusted prepared encounter action bindings retain one progress object per skill across actions; no fresh SkillProgress per click; cross-encounter XP retention remains OPEN |
| Turn number | Runtime-owned count, increments after completed living actions; not rounds or parity-based initiative | Domain lifecycle remains sole clock owner |
| Active/next actor | No existing owner/queue/enforced alternation; requests choose actors | A small application encounter-flow owner, subject to approval, selects player/enemy commands. No speed/initiative model invented; no separate UI turn counter |
| GameEventLog | Caller supplied and mutable; no encounter association | Fresh encounter log; adapter keeps cursor and copied batches, not another gameplay event store |
| Random source | Injected IRandomSource retained by engine/resolvers; SeededRandomSource wraps System.Random | Fresh combat-scoped source; no RNG calls for HUD/animation. Cross-runtime identical random sequences are not promised by this audit |
| Presentation state | Absent | View-scoped animations, visual HP interpolation, selected command and batch acknowledgement only; never authority for HP, ownership or outcome |

The most important composition gap is not just missing MonoBehaviours: PlayerProfile, execution SkillProgress, combat runtime and progression runtime are not currently assembled into one production-owned encounter. The existing legacy class collection/SkillBook and current ClassProfile/UnlockProfile are not interchangeable. The first boundary must retain complementary models by reference, use only the current validation chain for access, and avoid an implicit migration or duplicate synchronization service.

#### Candidate Boundary: Commands, Validation, Execution, Events, Projections

**PROPOSED, approval required:** a small application-level encounter owner/facade in the existing Game assembly, with plain C# copied read models. A separate assembly is not necessary to establish the logical boundary. Direct view access to the demo's RuntimeState, engine, runtime, resource pools or mutable definitions is not recommended.

Candidate contracts, names illustrative rather than committed APIs:
- A trusted composition operation accepts the existing LoadedPlayerSession (obtained from the demo bootstrap), the one PlayerProfile and prepared encounter dependencies, constructs the runtime/engine once, and starts once. It must validate nulls, distinct player/enemy identities, canonical definitions and skill/progress ID correspondence before starting.
- `GetSnapshot()` returns an immutable/copy-owned CombatViewSnapshot with encounter identity/revision, actor IDs/names, HP/Essence, optional active/reserve pools, turn count, proposed flow phase, terminal/outcome facts, copied effects and action descriptors. No live domain collection or record escapes.
- `RequestBasicAttack`, `RequestSkill(skillId)` and `RequestRefill(amount)` express intent. Only the owner resolves actors, targets, canonical skill implementations and retained progress. Unknown IDs, wrong encounter/revision, busy or terminal input fail at application admission without calling the engine. Validly identified but access-blocked skill attempts retain the existing engine's turn-consuming semantics; do not silently transform them into free validation rejections.
- A command receipt contains admission disposition, before/after copied snapshots and the newly appended ordered event batch. Distinguish AcceptedForExecution from Succeeded: ExecuteTurn returns void and a blocked action can advance the turn; refill false can mean admission rejection or a lethal start. Terminal facts take priority over turn-counter changes.
- Skill access previews reuse CombatSkillTurnValidatorGateway/CombatSkillUseValidator and expose P8B facts. They are advisory; do not infer cost affordability, SkillSeal/Silence, hit success or actor schedule from IsAllowed. No damage/cost/progression formulas are recalculated by UI.
- Engine execution remains authoritative and revalidates at its existing position. The engine does not return the actual structured validator result to the caller. First-slice feedback may display the emitted ActionBlocked message and label any earlier structured preview as a preview. Do not claim a pre-call result was the result actually used after turn-start hooks. If exact structured post-execution failure is required, an additive observation contract needs a separately approved task; no parser or second validation engine is justified.
- Exceptions during trusted skill/passive execution may follow mutation. Stop input, expose a diagnostic/fault state and retain the final observable snapshot; never retry the same command automatically or claim rollback. Null/foreign requests should be prevented by ownership, not patched into combat in this audit.

Commands are intent; previews are validation facts; existing engine/refill perform mutations; event batches describe emitted observations; projections describe current state. These are different responsibilities, not five new subsystems. No command bus, event bus, general service locator or second gameplay state store is proposed.

**Actor-flow decision required:** recommend a minimal player-first alternating player/enemy basic-attack scenario, with the existing enemy AI strategy, only after explicit approval. Today no rule enforces this. Select the next actor in application flow from admitted turn completion and terminal facts, not from TurnNumber parity or animation duration. A skill rejection that completes a living turn yields enemy phase; unaffordable pre-turn refill does not. A lethal start/end boundary terminates regardless of unchanged TurnNumber. Queue at most one authorized enemy response; do not call AI to populate the HUD because it emits an event. Animation acknowledgement only releases playback pacing/input; it must not call AdvanceTurn or decide winner/whose turn it is.

#### Events, Lifecycle and Animation Synchronization

**Observed ordering, not an idealized timeline:**
1. StartCombat executes passive/cognition hooks before CombatStarted; optional TraitRevealed follows. Start must therefore be captured as a batch too.
2. Skill execution can emit Atlas CognitionTriggered before TurnStarted. Begin then resets flags and runs legacy start/before-action hooks; legacy Poison/Burn emit StatusTick and can kill before validation.
3. Configured gateway rejection precedes Silence, then SkillSeal. The chosen blocker emits ActionBlocked and executes normal completion. ResourceNotEnough is a different event; positive enemy Resonance costs use ActionBlocked/UnsupportedResonanceOwnership.
4. Cost hooks can emit passive events. Essence ResourceSpent precedes optional active-Resonance ResourceSpent; skill Apply supplies SkillUsed and any skill-specific effects, then XP changes silently. BasicAttackUsed replaces the skill body for basic attacks.
5. CleanseResolved, miss or optional CriticalHit then DamageDealt, and pending debuff apply/resist occur before defender-defeat CombatEnded. Defender death returns without TurnEnded or increment.
6. Normal completion may emit ResourceRegenerated (message says 5 even when capped, so it is not an actual delta), legacy DebuffExpired, then canonical status DOT -> Regen -> duration tick. Canonical status damage/healing/expiry emit no per-effect events here. Attacker defeat emits CombatEnded and skips TurnEnded/increment; otherwise TurnEnded is emitted then AdvanceTurn increments.
7. Refill rejects terminal state silently, or unaffordable reserve with ActionBlocked before a turn. An admitted refill shares Begin/Complete, logs ResourceSpent with a refill message rather than structured transferred quantity, and may die at either boundary. Nonpositive/full-active no-op refills can still consume a turn. Refill is not a skill and ignores skill-only Silence/SkillSeal restrictions.
8. GameEventType contains BuffApplied/BuffExpired but no current production emitter was found; BuffContainer is a stub. Heal and canonical status changes are not fully represented. There is no dedicated actor-defeat payload; CombatEnded is text plus a separate runtime flag. OutcomeResolver reads HP, not that event.

**Consequence:** the log is not a complete event-sourced state model or a sufficient per-hit animation transcript. It has Type/Message only, no actor/target/skill IDs, amount, before/after value, effect identity, sequence ID or timestamp. Damage messages can describe resolved rather than clamped HP loss. Mixed DOT/heal or spend/regenerate can cancel in net deltas. Do not parse English names/messages to reconstruct rules or invent intermediate HP/effects.

**Minimal playback design:** keep an encounter-scoped integer cursor over Events.Count. Capture start index before StartCombat, AI selection or command execution; synchronously execute once; capture end count and copied final projection; copy events in [start,end) and wrap with encounter ID plus local batch/revision. Drain any previously unconsumed prefix rather than silently skipping it. Preserve original order and messages. A single adapter cursor assigns each index once; the view acknowledges batch IDs without executing commands again. Reject stale acknowledgements, double-clicks and old encounter IDs. Log append and snapshot capture occur on one serialized execution thread, with no callbacks that mutate state mid-read.

Domain execution completes before animation starts. The view may show generic action cues and ordered text, then animate each actor/resource from the copied before values to the final values and reconcile effect glyphs. Label numbers as net action/boundary changes, not individual damage/heal hits. First-slice status feedback can use final durations plus existing messages; no fabricated canonical expiry events. Input remains gated while a batch is playing; the flow owner already knows the next actor or terminal state. Rebinding a view reads the latest projection and cursor/ack state; do not replay historical gameplay or call StartCombat again. A timeout/cancel/scene-unload path should snap to the final authoritative projection and safely release or terminate playback, never undo/re-execute the turn.

A new log is required for a new encounter; do not reuse a raw cursor against a different log. Terminal batches finish their feedback once and disable gameplay input immediately; transition out once. If rich per-hit/DOT/regen choreography becomes required, add structured event facts at the existing emission sites under a separate compatibility-approved branch, preserving old messages/counts where possible. That expansion is **not required for the proposed aggregate first slice and not implemented here**.

#### HUD Data Availability

| HUD requirement | Current source | Missing contract / proposed treatment |
|---|---|---|
| Player/enemy identity and HP | CombatEntity.Id/Name; Stats.CurrentHealth/MaxHealth | Copy consistent values after each complete command; no Stats references in UI |
| Essence, active/reserve Resonance | Entity.Resource.Current/Max; runtime.ResonanceProfile.Active/Reserve | Pools exist; no HUD projection. Preserve absent-pool distinction; no enemy Resonance display/pool invention |
| Turn and turn order | Runtime.TurnNumber only | No active actor or initiative list. Show action count, plus approved application next-actor flow, not an invented speed forecast |
| Available executable skills | Registry Get(id), concrete ICombatSkill types, caller-owned SkillProgress; class/mastery policies | Registry lacks enumeration/executable mapping; SkillBook has HasSkill but no enumeration and is not current access authority. Prepared trusted action bindings are required, not a full inventory/skill unlock redesign |
| Access/lock reasons | Gateway + P8B result facts | Read-only advisory access descriptor; actual blocked feedback from execution batch. Checked != allowed, access != affordable/executable |
| Costs | SkillProgress.GetBaseCost(), Definition.ResonanceCost | Final Essence cost also uses mastery multiplier, cognition and potentially stateful passives plus ceiling. No pure exact quote API. Show explicitly base Essence and active-Resonance cost; do not invoke cost hooks/RNG or duplicate final formula for preview |
| Buffs/debuffs/status durations | Debuffs.Active IDebuff Name/Type/RemainingTurns; StatusEffects.Active Type/Potency/RemainingDuration | Two different effect models; BuffContainer has no enumerable buffs. Preserve source kind and display copies. For prepared effects use explicit presentation glyph mapping; no generic duration unification or buff system claim |
| Skill progression / retention mastery | SkillProgress.Level/Experience/IsMastered vs session.MasteryProfile records/rank | Two meanings of mastery; label separately. XP survives repeated actions only if the owner retains the same progress object |
| Victory/defeat | CombatEnded, both IsAlive flags, BattleOutcomeResolver | No structured terminal reason or cancellation outcome. Both alive + ended is not victory; both dead follows resolver's player-death-first result. No reward automatic application |
| Portraits/glyphs/animations | No verified visual asset catalog | Presentation asset mapping by stable IDs; asset absence is separate from missing data contracts |

HD-2D, Mesoamerican-inspired fantasy identity, tactical turn readability, distinct active/reserve Resonance, portraits, effect glyphs and readable feedback remain the visual direction. These are presentation concerns, not permission to attach damage or turn advancement to sprites/animation callbacks. No art, shader, UI framework, animation controller or asset production occurs in P9.

#### Unity Compatibility and Integration Options

**Verified repository fact:** the current Game artifact targets net10.0, not a Unity-compatible target already proven by this repository. Unity's 6.0 documentation lists .NET Standard 2.1/.NET Framework profiles and does not support .NET Core-targeted plug-ins in that matrix. Therefore copying the current DLL into Assets is not an accepted integration path. A compatible target must be built and tested for the chosen Editor. This conclusion is a compatibility inference from the repository target and [Unity API compatibility documentation](https://docs.unity3d.com/6000.0/Documentation/Manual/dotnet-profile-support.html), not an executed Unity import failure.

Unity supports externally built managed plug-ins, but explicitly requires suitable compilation targets and dependencies; see [managed plug-ins](https://docs.unity3d.com/6000.0/Documentation/Manual/plug-ins-managed.html). IL2CPP/AOT reflection and serialization can require preservation/testing, so Editor success alone would not prove Android readiness; see [IL2CPP restrictions](https://docs.unity3d.com/6000.0/Documentation/Manual/scripting-restrictions.html). References checked during this audit; these versioned pages are evidence, not selection of an Editor version.

| Option | Maintenance / reproducibility | Android / portability | Isolation / dependencies / tests / Editor workflow | Assessment |
|---|---|---|---|---|
| A. Compiled Game domain/application assembly | One source tree and explicit packaged artifact; requires target/dependency pinning and repeatable build/copy validation | Candidate compatible target (for example netstandard2.1) must pass actual Editor and IL2CPP tests; current net10.0 output is not acceptable | Strong existing separation, .NET smoke remains independent; ship required compatible JSON dependencies once; Editor rebuild/import step needed | **Recommended conditionally**, after compatibility branch proves a portable artifact from the same source |
| B. Canonical source via Unity asmdefs/package | No manual duplicate source permitted; package/asmdef and compiler/API configuration must be maintained alongside .NET project | Unity compiler/runtime restrictions and JSON/AOT remain; source import does not solve missing APIs | Easier Editor source debugging; two compilation environments and include/exclude policies; danger of copied or generated divergent code | Fallback only if an approved packaging spike demonstrates a concrete advantage |
| C. Separate application-facing integration assembly | Extra project/API/version/dependency boundary; useful only when it removes a real packaging/coupling obstacle | A thin wrapper over a net10.0-only Game still cannot make it portable | Can restrict presentation references; adds artifact coordination/testing, not a replacement engine/session | Defer physical split; implement logical facade in current Application first |

**Recommendation detail:** retain the net10.0 smoke target and prove a compatible Game target built from the same files, preferably multi-targeted rather than a fork or hand-copied subset. Pin Editor patch, API profile, language/compiler expectations and package versions in the later compatibility branch. System.Text.Json occurs in persistence and P8B JsonIgnore attributes, so excluding save UI does not remove the compile dependency. Choose compatible package assets and transitive dependencies deliberately; do not copy net10.0 runtime/System libraries indiscriminately into Unity. Preserve P8B JSON contracts and save schema; do not remove attributes to force a build.

No native interop or UnityEngine references were found in Game domain/application. File.Replace/Move, directory operations and reflection-based snapshot DTO serialization still require platform validation; the narrow UnityV2SaveService is not a complete session loader and is outside the .NET solution. Manual constructor injection is already the composition mechanism; a DI container is not needed. First playable scope can use an in-memory prepared session, without adding disk save/loading or pretending the existing persistence adapter covers every profile.

If the compatible build exposes unsupported APIs, stop and document the exact failures before authorizing targeted portability changes or an assembly split. No retargeting, package installation, Unity project creation or Android configuration has been attempted here. Android toolchain, device behavior, performance, graphics backend and memory footprint remain **UNVERIFIED**.

#### Proposed First Playable Acceptance Scenario

**Candidate, not implementation authorization:** one prepared player and one prepared enemy, one retained session, current basic attacks and an existing concrete skill (for example BalanceRitualSkill), canonical metadata with a small explicit active-Resonance cost, fixed scenario setup and the existing basic-attack enemy AI. Prepared values/content need approval; no new skill or balance system. Use a prepared existing effect to guarantee a status demonstration rather than relying on a random debuff roll.

| Step | Existing support | Required integration / acceptance |
|---|---|---|
| 1. Start prepared session | Demo/session chain and registries/profiles | App root prepares once, retains complementary PlayerProfile, assigns existing class and explicit skill/progress bindings; no save required |
| 2. Enter encounter | Engine factory, runtime/entity constructors, StartCombat | Encounter owner creates once and captures start events; repeated view mount does not recreate session/encounter |
| 3. Show both actors | Mutable HP/name/resource/effect facts | Copied initial HUD projection, portraits/placeholders, active/reserve separation, truthful action count and approved next actor |
| 4. Choose basic attack or skill | TurnRequest factories and canonical validation | ID-only input; prepared skill selected from bindings; no caller-supplied ICombatSkill or cost metadata |
| 5. Execute | Synchronous existing engine | Exactly one execution per admitted click; application owns actor sequence; no animation-driven rules |
| 6. Show damage/resources | Engine log and final states | Ordered log + aggregate before/after visual changes; do not claim exact intermediate hit amounts |
| 7. Show blocked feedback | Gateway reasons, ActionBlocked/ResourceNotEnough, blocked-turn lifecycle | A prepared locked skill or SkillSeal attempt demonstrates feedback and turn consequences; no UI short-circuit that changes the engine contract |
| 8. Refill active Resonance | Existing player refill action | Same pool as runtime; approved explicit refill amount; show denial/no-turn and accepted-turn outcomes; no enemy refill |
| 9. Show representative status | Existing Poison/Burn/SkillSeal/Regen models | Prepare one existing effect deterministically; display duration/change after its owning actor boundary without adding stacking/rewards |
| 10. Reach victory or defeat | HP/death checks and outcome resolver | Terminal projection and final feedback once; stop further player/AI execution, including lethal statuses; verify both outcomes |
| 11. Exit encounter | Session can outlive independent engines | Dispose/detach encounter/view state only; same session/profile references survive; no automatic rewards, profile reset or implicit persistent HP/XP policy |

No exploration, full missions, town, Sanctuary, saves, new actions, enemy skills, skill tree rewards or full-game economy is required for this acceptance scenario. Initial art may be explicitly labelled placeholder; the long-term visual identity is not replaced by placeholder aesthetics.

#### Risk Register

Severity describes integration impact, not a new AUD severity. "Block" means must be addressed/approved before the relevant playable path.

| Risk / classification | Evidence | Severity | Mitigation proposal | Required verification | First-slice blocker? |
|---|---|---|---|---|---|
| Duplicate session / confirmed gap | Each bootstrap constructor allocates a new chain; runtime needs a separate PlayerProfile | High | One persistent root, explicit retained references, no per-view constructors or legacy class syncing | Reference identity across enter/exit/rebind and multiple encounters | Yes |
| Mutable state leaking / confirmed API exposure | Public profiles, Stats setters, resources and effect records; IReadOnlyList does not freeze records | High | ID-only commands, copy-owned projections, keep runtime private to boundary | Mutating returned collections cannot change domain; UI assembly references only boundary where practical | Yes |
| Event replay/loss / design risk | Live append-only log without cursor/encounter IDs | High | Single encounter cursor, copied batch IDs, explicit ack/rebind strategy | Start/AI/action batch ordering, duplicate ack, empty batch, scene rebind, new-log reset | Yes |
| Animation/input races / design risk | Engine is synchronous; no admission or playback guard | High | Serialized command admission, revision/busy gate; visual queue never re-executes actions | Double click, delayed/cancelled animation, old callbacks, pause/unload | Yes |
| Stale HUD / design risk | Domains expose mutable facts; no atomic UI snapshot | High | One after-action copy under single-thread ownership; clear revision | Status/resource/progression changes visible together, no partial refresh | Yes |
| Validation/execution disagreement / confirmed boundary gap | Preview omits costs/statuses; engine rechecks after hooks and returns void | High | Advisory labels; do not skip execution for identified blocked attempts; preserve actual event feedback | Locked skill, insufficient resource, start death, hook changes, no fabricated structured result | Yes |
| Incorrect terminal transitions / confirmed representational gap | EndCombat can have living actors; death paths may omit TurnEnded; outcome uses HP | High | Combine terminal/HP/outcome facts, explicit no-winner state, stop AI before ChooseAction | Player/enemy/both dead, ended alive, terminal refill, repeated exit | Yes |
| Combat lifetime / confirmed missing owner | Factory creates engine only; RNG/passives/progress supplied independently | High | Encounter owner; fresh RNG/passive instances, retained per-skill progress; reject second active encounter | No cross-encounter state leak; no XP reset per click; exactly-once start | Yes |
| Actor/target authority / confirmed missing scheduling | Requests select attacker/defender; classifier checks attacker identity but not turn order; basics bypass gateway | High | Owner constructs targets from known actors; explicit approved flow, no external TurnRequest | Wrong-side/stale command rejected before mutation, blocked-turn vs refill denial routing | Yes |
| Event detail / confirmed limitation | Type/message only; canonical DOT/heal/expiry silent | Medium for aggregate slice; High for exact choreography | Net-change/final-state playback; no message parsing; defer structured payload expansion | Simultaneous DOT/regen and spend/regen, caps, expiry without log event | No if aggregate feedback approved; otherwise yes |
| Action roster/cost model / confirmed missing contract | Metadata registry is not execution map; no pure final cost quote | High | Prepared canonical bindings; base-cost label, existing access policy; no duplicate calculations | Missing/mismatched IDs, retained access, cost passive not executed during preview | Yes |
| Unity assembly compatibility / confirmed target mismatch, runtime unverified | net10.0 only; no Unity project/version or portable build | High | Pin target and prove compatible artifact before scenes | Editor import, sample boundary calls, dependency resolution, compatible-target regression host | Yes for Unity |
| Android deployment / unknown | No verified SDK/NDK/device/IL2CPP build | High | Dedicated Android build/device gate with package/AOT/stripping checks | Install/run, resume, orientation/input, representative JSON and action loop | Yes for Android claim; not pure .NET boundary |
| Legacy slice coexistence / design risk | FoundationVerticalSlice.Run allocates independent profiles and executes missions/rewards | Medium | Regression-only legacy entry; visual flow consumes demo/session root | V1 remains exactly 38; visual start never invokes legacy Run | Yes, simple exclusion |
| Untested visual assumptions / unknown | No renderer/HUD/assets/input project | Medium | Small placeholder view, distinct visual verification iteration | Readability, glyphs, HP/resource transitions, no overlaps, input gating and terminal feedback on target devices | Yes for playable claim |

No risk above authorizes repairs to resolved audit contracts. Encounter scheduling, persistence carryover and exact intermediate event payloads are explicit design choices, not facts inferred from green smoke counts.

#### Ordered Atomic Roadmap for Separate Approval

Every row is a separate candidate branch, **not created or automatically authorized**. Every implementation row ends with a verification iteration (build, all existing smoke/counts, focused E2E, diff and Bible evidence) before moving to the next; the dedicated integrated verification row exercises the combined boundary. New state remains encounter/view ephemeral unless explicitly stated; no save changes are implied.

| Order / candidate branch | One purpose and exact scope | Dependencies | Acceptance / tests | Bible update and explicit exclusions |
|---|---|---|---|---|
| 1. `codex/combat-encounter-composition-foundation` | Establish encounter ownership around the supplied LoadedPlayerSession + one retained PlayerProfile; prepare owned actors/runtime/log/RNG/passives and canonical skill/progress bindings; one engine/start path | Approve ownership/skill-progress lifetime and in-memory prepared scenario | Same session/profile references; fresh encounter dependencies; null/mismatched inputs rejected before start; repeated start/rebind safe; existing V1/AUD/P8 regressions | Record owner/lifetimes and errors. No UI, actor alternation, new gameplay rules, persistence or full content registry |
| 2. `codex/combat-read-only-projection-foundation` | Copy immutable actor/resource/effect/progression facts and prepared action descriptors with canonical access previews and explicitly base costs | 1 | Repeated reads do not mutate/RNG/emit events; copies cannot alter domain; absent Resonance, distinct mastery models, lock reasons and durations truthful | Record projection semantics. No final-cost formula duplication, animation, execution or new buffs |
| 3. `codex/combat-intent-event-batch-boundary` | Admit ID-based basic/skill/refill intent; invoke existing authority once; return before/after and cursor-delimited event batch including start | 1-2, approval for text/net-change feedback and fault policy | Unknown/stale/terminal inputs safe; access-blocked valid attempt retains turn consumption; refill denial vs completed action; exactly-once batch ranges; actual final resources/HP | Record commands/dispositions and event limitations. No new engine/bus, message parsing, typed event schema or turn formulas |
| 4. `codex/combat-encounter-turn-flow` | Single application actor-flow authority for the approved prepared scenario; choose existing enemy basic action and next phase from real completion/terminal facts | 3, explicit player-first alternation approval | No parity inference; denied pre-turn refill stays player; blocked completed skill yields enemy; lethal start/end yields no enemy; reentrant/duplicate action blocked; AI event included | Record this as newly approved orchestration, not preexisting initiative. No speed system, Defend/Wait, enemy skills or animation-owned clock |
| 5. `codex/combat-presentation-boundary-workflow-verification` | Verify complete headless prepared-session -> encounter -> attack/skill/block/refill/status -> outcome -> exit -> same session flow | 1-4 | Both winners; event ordering/cursors; copied HUD consistency; repeated enter/exit; skill XP retained across actions; all smoke/V1 38; fault/no-retry scenarios | Evidence and remaining gaps only. No UI, packaging or gameplay expansion |
| 6. `codex/unity-runtime-compatibility-spike` | Pin Editor/runtime target and reproducible compatible Game artifact/dependencies from one source tree; minimal import/call harness only | 5 plus approved Editor/API/backend/package plan and available tooling | Build portable target and net10 baseline; run compatible artifact regressions; Editor import and boundary call; IL2CPP/Android probe where available; preserve JSON/enum/save behavior; report blockers rather than assume | Record exact versions/commands/platform proof. No visual combat scene, full assembly split, schema change or broad portability rewrite |
| 7. `codex/unity-combat-presentation-shell` | Minimal one-encounter scene/HUD/input bound only to the approved facade and prepared root; placeholder assets and distinct HP/Essence/active/reserve/effect display | 6 proven, approved visual framework and assets/placeholder policy | Editor scene reload retains intended session; no duplicate runtime; command clicks reach boundary; displays match projections; view cannot mutate domain; desktop/mobile layout checks | Record scene/adapter ownership and manual Editor steps; preserve meta/GUIDs. No new gameplay, custom cost logic or full-game scene architecture |
| 8. `codex/unity-combat-feedback-sequence` | View-only ordered batch playback and input gating; aggregate state interpolation, cancellation/rebind and once-only terminal display | 7 and event-detail compromise approved | Delayed/cancelled clips, double taps, stale callbacks, zero-event batches, lethal statuses and scene exit cannot execute twice or advance turns | Record visual state machine and acknowledgement contract. No engine awaits, per-hit inference, animation-triggered damage or new events |
| 9. `codex/playable-combat-slice-workflow-verification` | End-to-end acceptance of the eleven-step scenario in Editor/desktop and an explicitly selected Android target | 8, actual platform tooling/device | Both outcomes; blocked feedback, refill/status durations; pause/resume/input races; readable HD-2D-oriented HUD; no domain changes from view lifecycle; full smoke/V1 contract | Mark only tested platforms playable; record screenshots/manual steps/device/build. No town, Sanctuary, rewards, saves or additional features |

**Earliest safe implementation branch:** `codex/combat-encounter-composition-foundation`, only after independent review approves the ownership contract. No implementation branch is created by P9. A Unity compatibility probe may be reprioritized separately to retire platform risk, but scenes should not precede a proven application boundary.

#### Decisions Requiring Approval and Audit Conclusion

1. Accept the existing progression session plus one retained complementary PlayerProfile as the prepared root, without merging or synchronizing legacy class/skill access models. Decide later cross-encounter HP/Resonance/SkillProgress carryover; this audit proposes encounter-scoped combat state, not a new persistence rule.
2. Approve prepared action bindings and scenario values using existing skills/statuses, not a general skill catalog rewrite or new mechanics.
3. Approve application-owned player-first alternation for this one encounter. No existing domain initiative/active-actor rule is being claimed.
4. Accept base-cost previews and text/net-change animation for the first slice, or separately authorize exact cost-quote/structured action-event work before visual acceptance.
5. Select and pin Unity Editor, rendering/UI approach, target API/dependency packaging and desktop/Android verification scope. Option A is conditional, not certified.
6. Approve serialized command execution, playback gating, rebind/fault/exit behavior and no automatic retries/rewards/save operations.

**Conclusion:** existing combat/progression rules and regression coverage are a sound base for a thin application boundary, not yet a ready-to-bind playable Unity SDK. Session/encounter composition, actor admission/flow, copied projections/action roster, event-batch consumption and runtime compatibility must be established before scene implementation. The smallest safe first slice can retain the current engine and use aggregate feedback; precise intermediate animations require additional contracts. Design/audit phase only is complete. Stop for architectural review: no implementation, PR, merge or subsequent branch.

<a id="combat-encounter-composition"></a>
### P9.1: Combat Encounter Composition Foundation

**COMPLETE within the authorized composition scope (2026-11-04).** Branch `codex/combat-encounter-composition-foundation` was created from freshly fetched origin/main `4fa2426`, which integrates P9 commit `636aec4afa4c4872c505285eb151b289f40f9511`. The preceding P9 section is the dated pre-implementation audit; this closure implements only its first atomic step. No P9.2, presentation facade, command API, actor alternation or Unity work is authorized or implemented.

**Discovery and design gate:** existing bootstrap/session wrappers retain one PlayerProgressionRuntimeState, but do not compose combatants/runtime/log or own executable skill progress. LoadedPlayerSession.CreateCombatEngine delegates through the existing ProgressionAwareCombatEngineFactory and validation gateway; these methods are nonvirtual and are not replaced for testing. StartCombat is synchronous, has no repeat guard and can throw after passive mutation. GameEventLog is append-only; CombatEntity and PlayerProfile have no common persistent player-identity key. Skill metadata lives in the session registry, while ICombatSkill implementations and SkillProgress are separate caller-created objects. The ownership table was established before production edits.

| Object | Scope and final owner/reference contract |
|---|---|
| LoadedPlayerSession | Existing session-scoped object, retained exactly; never allocated by encounter composition |
| PlayerProgressionRuntimeState and all its profiles/registries | Same session-owned state, accessed through the retained session; no alternative progression arguments or copies |
| Complementary PlayerProfile | Supplied by the trusted root and retained exactly alongside the session; runtime references this instance; not reconstructed per view |
| Player CombatEntity | Encounter-scoped prepared instance, explicitly assigned to runtime.Player |
| Enemy CombatEntity | Different encounter-scoped prepared instance, explicitly assigned to runtime.Enemy; distinct IDs and Stats required |
| CombatRuntimeContext | One newly constructed runtime per owner; no replacement/reinitialize API |
| TurnBasedCombatEngine | One engine created only through retained Session.CreateCombatEngine, with its existing progression-aware validation path |
| GameEventLog | Supplied fresh empty encounter log, retained exactly; no clear/replay/event-batch mechanism |
| RNG | Supplied encounter-scoped source, retained; no validation-time rolls or implicit reseeding |
| Passive effects | Encounter-scoped mutable instances retained in a copied read-only membership list; no validation-time hooks |
| ResonanceProfile | Optional supplied encounter pool, retained exactly in runtime; player-owned AUD-05 contract unchanged; no automatic allocation/refill |
| Executable skills | Trusted prepared-content bindings; existing implementations, not a generated catalog or authorization grant; mutable implementations must remain encounter-exclusive |
| SkillProgress | Supplied instance retained for the whole encounter, including repeated engine actions; no per-action reconstruction or cross-encounter persistence |
| SkillDefinition/SkillCost | Session registry remains canonical metadata authority; equivalent-value progress definitions accepted under AUD-04; no duplicate definition registry |

**Public API (Game.Application.Runtime):**
- `PreparedCombatSkillBinding(ICombatSkill skill, SkillProgress progress)` exposes only immutable `SkillId`. Executable/progress references are internal. Constructor rejects nulls, missing definitions, blank IDs and executable/progress ID disagreement.
- `CombatEncounterPreparation(CombatEntity player, CombatEntity enemy, GameEventLog eventLog, IRandomSource random, IReadOnlyList<ICombatPassiveEffect> passives, IReadOnlyList<PreparedCombatSkillBinding> skills, ResonanceProfile resonance = null)` copies collection membership and retains dependencies internally. It has no public mutable getters. Empty skill/passive collections support a basic-only encounter; null collections/elements do not.
- `CombatEncounterComposition(LoadedPlayerSession session, PlayerProfile playerProfile, CombatEncounterPreparation preparation)` validates, claims the preparation once, builds a private binding index, creates the existing engine and constructs the one runtime. Construction does not start combat. The resulting object itself is the stable encounter handle; reading it creates nothing.
- `bool Start()` returns true only on the first successful start, false for any repeat or reentry; first failure throws the original exception. `State` exposes Prepared/Starting/Started/Terminal/Faulted; `StartFailure` retains the original exception or null. Terminal is observed from existing CombatEnded/dead-actor facts after successful start, not a new winner rule or a mutation of domain flags. No disposal/resource framework is introduced for these managed in-memory objects.

**Validation and binding semantics:** null required inputs are ArgumentNullException; malformed preparation/bindings are ArgumentException; reusing an already claimed valid preparation is InvalidOperationException. Actor IDs must be nonblank, distinct and case-sensitive; actors must be different living objects and cannot share Stats. A nonempty log is rejected, never erased. Binding IDs use ordinal uniqueness, including duplicate IDs in separate binding instances. All required registries come from the existing session (whose constructors already reject null registries). Each binding is resolved against that session's SkillDefinitionRegistry; unknown IDs, changed executable IDs, null costs or conflicting metadata are rejected before engine start. AUD-04's HasCanonicalMetadata helper changes only from private to internal for reuse, preserving its comparison and all existing access behavior, including equivalent-value copies. No skill access policy, cost formula or unlock grant is added; a structurally valid locked skill can be prepared and remains blocked by the existing engine gateway on execution.

**Identity limits:** assigning the supplied `player` to runtime.Player is the only available role contract; the existing classifier then recognizes that exact reference. There is no reliable API to prove that a given PlayerProfile belongs to a given CombatEntity or LoadedPlayerSession. Names, ID prefixes and legacy class collections are not used as heuristics. The trusted root must supply the correct association. Prepared skill code is also trusted: metadata/ID validation cannot prove an arbitrary ICombatSkill implementation's behavior.

**Ownership limits:** all new public types are sealed and expose no gameplay-object getters. Internal access permits later application integration and focused smoke tests, not Unity/presentation access. Game.csproj grants InternalsVisibleTo only to the existing smoke assembly so identity and real engine behavior can be tested without public runtime/engine escape hatches. Existing domain/session APIs remain mutable and unchanged. Callers still possess the references they supply: they must transfer exclusive encounter use of actors, logs, RNG, mutable passives, pools and SkillProgress and must not wrap the same objects in a second preparation. Single-use claiming protects reuse of one preparation object; it is not a global identity registry, deep clone, thread-safe lease or security boundary. Multiple independently prepared encounters may retain one session; scheduling which is active is outside P9.1.

**Start/failure contract:** construction performs all checks before factory/runtime creation or hooks. Start transitions to Starting before revalidation and engine StartCombat, so reentrant callbacks cannot run start twice. Revalidation catches intervening dead actors, log writes and executable-ID changes. A successful engine return sets Started (or observed Terminal); repeat calls append nothing and replace nothing. Any start exception sets Faulted, retains the original exception and rethrows with its stack; later Start calls return false. Partially applied HP changes and appended events remain observable internally and through the original caller-owned references. No rollback, automatic retry, repair or save occurs. Faulted owners must not be used for later execution. Single-threaded trusted use is required; concurrency, malicious callbacks and arbitrary external mutation are not certified.

**Deterministic permanent verification:** CombatEncounterCompositionSmokeTests adds **156** assertions under Player Progression / Combat Validation. Its in-memory fixture uses one real loaded session with Tlamatini allowing the existing BalanceRitualSkill and Wanderer allowing none, canonical max-level 3 / Essence cost 7 / Resonance cost 2 metadata, one complementary profile, two distinct 100-HP actors, a deterministic RNG, one observer passive, one fresh log and active/reserve pools 20/50. Prepared scenario values are test-only content, not a new production catalog or balance commitment.

Coverage includes exact session/runtime/profile/combatant/log/pool/binding identities; nonmutating preparation; explicit player/enemy classification; copied collection membership; stable reads; null inputs and null session registries; blank/equal actor IDs, aliased actors/Stats, dead actors and reused logs; null/duplicate bindings; unknown/conflicting/null-cost metadata and equivalent copies; one-use preparation; first/repeated/reentrant start; preserved original exception and partial state; no automatic retries; stale preparation at start; optional Resonance/basic-only setup; terminal observation without flag repair; and public-boundary structural guards. The integrated workflow starts two independent encounters on one session, exercises the real existing engine with retained XP, changes class to verify blocked access/normal consumed-turn semantics, masters the skill on the original session to verify retained access, then starts the second encounter without session resets or XP carryover. Existing inventory/wallet facts and absence of automatic grants are checked. No persistence API is called by the composition; full existing save/old-JSON regressions still run unchanged. Reflection is limited to public-surface guards, not the behavioral/identity workflow.

**Verification evidence:** unchanged baseline restore/build/smoke exited 0 with **3762 assertions**, **38 V1 events**, zero warnings/errors. No artificial red phase was manufactured for the new API; no existing defect is claimed. Final restore/build/smoke/diff-check/status exit **0**, build **0 warnings / 0 errors**. New assertions **156**; final **3918**: V1 **2**, Shared **332**, V2 **371**, V3 **686**, Progression **2527**. V1 events **38**. AUD-01 through AUD-12, P8A/P8B and previous integrated sections retain their counts and pass. The focused composition workflow is the verification iteration for this atomic feature, not authorization to proceed.

**Changed scope and remaining gates:** four new focused application files, one focused smoke file, one runner registration, the internal-only metadata-helper visibility change, the test-friend project declaration and this Bible. TurnBasedCombatEngine, factory/session implementations, skill rules, costs, turn lifecycle, events, snapshots, save schema and Unity sources/assets are untouched. No generator was executed. No automatic rewards or HP/Resonance/XP carryover, save writes, action facade, scheduling, projections, event cursor, animation, scene or new gameplay skill. Unity/Android remain unverified; no Unity manual validation is required by this backend-only diff. P9.1 alone is complete; stop for independent architectural review, with no PR, merge or P9.2 implementation.

<a id="combat-readonly-projections"></a>
### P9.2: Read-Only Combat Projections

**COMPLETE within the authorized read-model scope (2026-11-04).** Branch `codex/combat-readonly-projections` starts from freshly fetched origin/main `57b725c`, which integrates P9.1 commit `e89b7104f4738b65e13bc8fbeb806ad7638f3841`. P9 and P9.1 sections above are dated discovery/closure evidence; this explicit mission authorizes only detached observations, not the advisory access previews mentioned in the older P9 candidate roadmap. P9.3 commands, validation results, event consumption and later steps remain unauthorized.

**Discovery gate:** inspected composition/preparation/bindings/session, runtime/entity/stats/resources, actual effect implementations, SkillDefinition/SkillProgress and event/validation contracts before production edits. All supported built-in source getters are nonmutating; no gameplay hook is needed to obtain the fields below. CombatStats, pools, effects, XP and lifecycle are mutable; IDs/names and canonical metadata are constructor-set, and registry registration does not replace definitions. Strings/enums/numbers are copied as values; no source object is retained by a read model. Optional/missing information is distinguished explicitly rather than inferred from display strings or GameEvent messages.

**Public API:** `CombatEncounterComposition.GetSnapshot() -> CombatEncounterSnapshot`. Seven sealed, getter-only types in Game.Application.Runtime: CombatEncounterSnapshot, CombatantSnapshot, CombatResourceSnapshot, CombatResonanceSnapshot, CombatDebuffSnapshot, CombatStatusEffectSnapshot and PreparedSkillSnapshot. Constructors are internal. The existing owner remains the only public capture entry; there is no projection manager, cache, generic cloning framework or second state store. Snapshots are transient observations, not save DTOs.

#### Actual Field Mapping

Every row is a side-effect-free read of built-in contracts. "Current" means directly observed data; "derived" means copying an existing domain/application getter, not adding a formula. Each scalar is eagerly stored; nested collections copy membership and element values. No row returns a mutable source reference.

| Projection fields | Authoritative source | Presence / source mutability / meaning |
|---|---|---|
| Encounter.State | CombatEncounterComposition.State | Always present outside rejected Starting; mutable lifecycle; derived Terminal observation preserves existing owner semantics |
| Encounter.TurnNumber, CombatEnded | Runtime.TurnNumber, Runtime.CombatEnded | Always present; mutable current action counter and raw ended flag, not active actor or winner |
| Encounter.Player, Enemy | Runtime.Player, Runtime.Enemy | Required prepared actors; copied CombatantSnapshot values |
| Combatant.Id, Name | CombatEntity.Id, Name | ID required by preparation; Name copied verbatim even if caller supplied null; constructor-set strings, not fabricated labels |
| Combatant.CurrentHealth, MaxHealth | CombatEntity.Stats.CurrentHealth, MaxHealth | Mutable current scalar values; no clamping, damage reconstruction or healing calculation |
| Combatant.IsAlive | CombatEntity.IsAlive | Existing derived alive fact captured as bool |
| Combatant.Essence.Current, Max | CombatEntity.Resource.Current, Max | Required pool; current balance mutable, capacity constructor-set; detached CombatResourceSnapshot |
| Encounter.HasPlayerResonance, PlayerResonance | Runtime.ResonanceProfile != null / that profile | Optional profile; false/null when absent; a present zero-capacity pool remains true/non-null |
| PlayerResonance.Active.Current/Max; Reserve.Current/Max | ResonanceProfile.Active and Reserve getters | Two separate resources; balances mutable, capacities constructor-set; no enemy pool or transfer calculation |
| Combatant.Debuffs[].Name, Type, RemainingTurns, IsExpired | DebuffContainer.Active entries via IDebuff | Ordered current membership; duration mutable, expiry existing derived getter; copies each entry, including expired entries still present |
| Combatant.StatusEffects[].Type, RemainingDuration, Potency, AllowsStacking, IsExpired | StatusEffectCollection.Active entries | Ordered current membership; duration mutable, other metadata constructor-set, expiry existing derived getter; independent copies, no merging |
| Encounter.PreparedSkills membership/order | Retained CombatEncounterPreparation.Skills | Captured prepared list, not an access-filtered or complete registry catalog; later registrations alone add no prepared skills |
| PreparedSkill.SkillId, Name, Category, Targeting, MaxLevel | Session.RuntimeState.SkillRegistry.Get(binding.SkillId) | Required canonical definition validated by P9.1; constructor-set metadata; executable display name is not authoritative |
| PreparedSkill.Level, Experience, IsSkillProgressMastered | Retained binding.Progress.Level, Experience, IsMastered | Mutable XP/level and existing derived mastery; **not** MasteryProfile retention mastery or rank |
| PreparedSkill.BaseEssenceCost, EssenceCostPerLevel, ResonanceCost | Canonical Cost.BaseCost, Cost.CostPerLevel, ResonanceCost | Constructor-set raw cost metadata; not SkillProgress.GetBaseCost(), a final payable quote, a mastery discount or an affordability decision |

**Unsupported data is omitted:** BuffContainer exposes only empty lifecycle hook methods, no collection or readable buff records. Therefore no Buffs property or misleading empty-buff assertion is supplied. IDebuff exposes no stable instance ID, magnitude, stack count or source actor; its name is display text, not parsed into identity. StatusEffectInstance exposes a type identifier and potency, but no unique instance ID, source actor or aggregate stack count. AllowsStacking is copied as permission metadata only. Neither model provides a generic round duration; RemainingTurns and RemainingDuration retain their distinct original boundary semantics. No active actor, initiative, winner, event cursor, event transcript, skill availability or exact final cost is invented.

**Immutability:** all seven types are sealed with get-only properties and readonly backing fields. Source objects are used only during internal construction and are not retained. Each capture constructs fresh nested values and fresh private lists wrapped with AsReadOnly; callers cannot mutate membership through generic or nongeneric list casts. Element snapshots themselves are immutable. No arrays/dictionaries, delegates, runtime resources, actors, stats, bindings, SkillProgress, SkillDefinition, SkillCost, effect instances or exception objects escape through the snapshot graph. Enum values reuse existing definitions without renaming or altering serialization contracts. There is no lazy getter back into gameplay, serialization-based cloning, implicit normalization or snapshot caching.

**Lifecycle contract:** Prepared returns current prepared values without calling Start. Starting throws InvalidOperationException before copying any values; a hook may catch that rejection and let the original start complete, or an uncaught exception follows P9.1's existing fault contract. Started returns current state. Terminal returns the observed lifecycle and raw CombatEnded independently: a dead actor with a false flag remains Terminal/false, and ended-with-both-alive remains Terminal/true without a fabricated winner. Faulted is readable and clearly marked Faulted; partial HP/resource/event mutations from the failed start are not rolled back or represented as success. The original exception remains on the existing owner, not in the value graph. Reading never retries start, clears a fault or advances a turn.

**Freshness and consistency:** the P9.1 trusted single-threaded contract remains. Capture between gameplay mutations on the owning thread; do not concurrently edit source objects or invoke observation from mutating callbacks. Starting reentry has the explicit rejection above; no broader execution lock or action-phase scheduler is added. Each successful call completes one eager read and returns new detached objects; earlier snapshots remain unchanged after later damage, resource transfers, XP/level changes or effect ticks/removals. Source collections and trusted extension getters must remain valid and nonmutating during capture. This is not a transaction, concurrent snapshot guarantee, defense against malicious IDebuff getters or repair of externally corrupted containers. Canonical definitions remain guaranteed by existing preparation plus the registry's no-replacement/no-removal API; capture performs lookup, not a second authorization check.

**Permanent verification:** CombatEncounterProjectionSmokeTests contributes **149** assertions in Player Progression / Combat Validation, reusing the P9.1 in-memory fixture rather than duplicating its ownership suite. It covers custom player/enemy names and different resource capacities; canonical skill names distinct from executable names; nonzero per-level metadata without cost evaluation; empty effects/basic-only skills; absent versus present zero-capacity Resonance; prepared ordering and no full-catalog enumeration; repeated pure reads with complete source fingerprints; stable owner/session/runtime/engine identities; fresh HP/capacity/resource/XP/mastery observations; and unchanged old nested values. All five built-in legacy debuff classes and all five status types are exercised, including repeated stackable Poison entries, expired entries awaiting domain cleanup, preserved ordering and distinct enemy values. Real Tick/OnAfterAction/TickDurations calls verify copied element values and removal freshness. Attempts to Add, Clear or replace elements through list casts throw NotSupportedException; supplementary public/private field checks verify the scalar/value-only immutable graph without replacing behavioral tests.

Lifecycle coverage includes Prepared, caught Starting rejection, Started, both terminal representations and Faulted partial-start state with unchanged original diagnostics. The integrated real-engine workflow verifies class-allowed execution, class-switch blocking and turn consumption, retention mastery access, unsupported enemy Resonance rejection and enemy basic attack; observations do not change the existing gateway, XP, resource ownership or emitted events. No production projection method executes an action or reads/consumes GameEventLog. Snapshot fingerprint serialization is test-only and is not a production cloning or persistence mechanism.

**Verification evidence:** baseline restore/build/smoke/diff-check/status exit **0**, with **3918 assertions**, **38 V1 events**, **0 warnings / 0 errors**. Final restore/build/smoke/diff-check/status exit **0**, build **0 warnings / 0 errors**. New projection assertions **149**; final **4067**: V1 **2**, Shared **332**, V2 **371**, V3 **686**, Progression **2676**; V1 events **38**. P9.1 remains **156** and all existing AUD/P8 sections and persistence/old-JSON tests remain green with unchanged counts. No artificial red phase or unrelated defect repair was needed.

**Scope and remaining gates:** seven new read-model files, one additive GetSnapshot method/comment update on the existing owner, one focused smoke file, its runner registration and this Bible. No Domain, existing engine signature, validation, cost/Resonance/turn/mastery rules, save schema, serialized enum names, event payload, target framework, package, compatibility adapter or generator changes. These transient values need no new save format. Unity/Android and HUD integration remain unverified; no Unity assets were changed and no Editor validation was performed. P9.2 alone is complete; stop for architectural review, with no PR, merge or P9.3 work.

<a id="combat-intent-event-boundary"></a>
### P9.3: Combat Intent and Event Batch Boundary

**COMPLETE within the authorized application boundary scope (2026-11-05).** Branch `codex/combat-intent-event-batch-boundary` starts from freshly fetched origin/main `84ddad3`, which integrates P9.2 commit `9126523b96ca50330aba2b9bd666ebf15b7ca13f`. Earlier P9 closures remain dated evidence, not competing current authorization. This step extends the existing owner; no parallel engine, session, validator, event bus or event store is introduced. P9.4 remains unauthorized.

**Discovery/design gate:** basic and prepared skill actions enter `TurnBasedCombatEngine.ExecuteTurn(Runtime, TurnRequest)`; it returns void. Prepared bindings retain the exact executable and SkillProgress objects. `ResonanceRefillAction.Execute(profile, runtime, amount)` is the existing refill authority and delegates turn lifecycle to CombatTurnLifecycle; its bool is not a turn-completion verdict. Engine and refill may return early after blocking or lethal effects. Terminal observation is the existing ended flag or either actor no longer alive, not a fabricated winner. Start hooks, CombatStarted and optional TraitRevealed can append events before any command. GameEventLog exposes an ordered append-only list in the supported scenario; count/index reads permit half-open ranges. Trusted hooks can throw after irreversible cost or effect mutations; neither authority offers rollback. These facts determine the API below rather than a new generic action framework.

**Public API and value boundary:** `CombatIntent.BasicAttack()`, `CombatIntent.Skill(string skillId)` and `CombatIntent.Refill(int amount)` produce sealed immutable values (Type, SkillId, RefillAmount). `CombatIntentType` has BasicAttack, PreparedSkill and ResonanceRefill. `CombatEncounterComposition.SubmitIntent(intent)` returns CombatIntentReceipt. Actor/target are always the retained player/enemy; foreign actors, targets, skill objects, progress objects and runtime references cannot be supplied through an intent. Existing `Start()` and `GetSnapshot()` remain; additive `ReadEvents(int startIndex, int? endIndex = null)` and `ReadStartEvents()` return detached CombatEventBatch values. `ExecutionFailure` retains an original command exception on the owner, separate from StartFailure; exception references never enter the immutable receipt graph.

**Admission:** reentry/Starting rejects first with CommandInFlight. Otherwise precedence is EncounterFaulted, EncounterNotStarted, EncounterTerminal, NullIntent, then type-specific validation. Prepared skill IDs must be nonblank and resolve ordinally in the retained prepared binding map: InvalidSkillId or SkillNotPrepared, including registered but unprepared skills and case mismatches. There is no new ID syntax or normalization. UnknownIntentType is defensive against invalid enum construction; typed factories cannot create it. Refill without the optional player Resonance profile rejects with ResonanceUnavailable. No gameplay hook is called during admission. Negative/zero refill amounts are not redefined as admission errors: the existing refill contract decides their behavior. Rejection invokes no gameplay, appends no domain event and returns an empty range at the current log count. Stable rejected state gets the same detached observation in Before and After; busy rejection has null observations, and an unreadable rejected state has null observations plus ObservationFailureMessage.

**Execution:** an admitted basic/skill intent constructs the existing TurnRequest with retained references and invokes the engine exactly once. A prepared skill blocked by canonical unlock or class/mastery authorization still enters that engine; ActionBlocked and established consumed-turn behavior are preserved. Refill calls the existing action exactly once. The boundary never computes damage/cost/XP, advances the turn, retries an invocation, or changes status lifecycle. Before is captured after admission and before invocation; After is captured after return. No resources are implicitly carried between encounters and no save operation occurs.

| Receipt fact | Meaning |
|---|---|
| Disposition | AdmissionRejected, Completed or Faulted; no overloaded Success flag |
| AdmissionReason | Stable CombatIntentAdmissionReason; None for accepted invocations/faults |
| AcceptedForExecution | Admission passed; does not promise the invocation started or a skill applied |
| InvocationStarted / InvocationCompleted | Whether the authoritative method was entered / returned normally, independent of later projection failure |
| Before / After | Detached P9.2 snapshots; null explicitly when busy or unavailable, never a stale substitute for post-fault state |
| EventBatch | Immutable copied events in [StartIndex, EndIndex), each carrying encounter-local Index, existing Type and verbatim Message |
| TurnStartedObserved / TurnEndedObserved | Presence of existing event types in this invocation's range; no text parsing or counter inference |
| EncounterIsTerminal | Nullable observed ended/dead-actor fact from After, independent of the owner's Faulted lifecycle; null if After is unavailable |
| RefillResult | Nullable raw refill bool; null for non-refill or a refill that did not return; not effect or turn success |
| FaultType / FaultMessage | Detached original failure diagnostics; original exception/stack remains on ExecutionFailure |
| ObservationFailureMessage | A further inability to capture state; never replaces the original invocation failure |

**Refill evidence:** ordinary transfer returns true with start/end events. A nonpositive request or full active pool can return true and consume a normal turn without transfer. Empty reserve can return false before starting a turn; reserve drained by a before-action hook can return false after a started, completed turn. Lethal start returns false with terminal state and no transfer/end event. A successful transfer followed by lethal end effects returns true and terminal state without TurnEnded or counter advance. All distinctions are copied from the real action, not inferred from its bool. A completed skill invocation likewise does not imply hit, authorization success or XP gain.

#### Event Consumption Protocol

There is no hidden owner acknowledgement cursor. The consumer owns one `next` cursor per encounter, initially zero. Reads copy existing log values and never consume gameplay or append events. Use this protocol for exactly-once processing within that consumer:

1. Read `ReadEvents(next)` after Start or a command; process ordered indices starting at next, advancing next to EndIndex only after successful processing. A subsequent read at that EndIndex returns only later events (possibly none).
2. When delivering a receipt batch directly, first recover any gap with `ReadEvents(next, receipt.EventBatch.StartIndex)` if its start is ahead of next. Ignore indices below next when a batch overlaps already processed events. Alternatively use ReadEvents(next) as the sole delivery source and use receipts only for state/diagnostics.
3. An admission-rejected empty batch at the current count is **not** acknowledgement of older pending events. Never jump next over those events merely because a rejected receipt reports that count.
4. `ReadStartEvents()` returns the fixed range captured around the first Start attempt, including hook events and partial events when Start throws. Process it through the same cursor. The first action always captures its own later start index; it cannot absorb unobserved Start events. Start-range reads after later actions remain unchanged.
5. Recover a lost receipt with `ReadEvents(next)` and a fresh GetSnapshot if readable; never resubmit the intent to recover observations. Earlier-range reads intentionally replay values, not gameplay. Previously returned event/receipt snapshots remain unchanged after later mutations.

Bounds outside the current log or reversed ranges throw ArgumentOutOfRangeException without mutation. Public snapshot/event reads throw InvalidOperationException while Starting or a command is in flight. This protocol prevents gaps/duplicate processing when followed; arbitrary replay reads are intentionally not globally exactly-once delivery. No cross-owner cursor routing, persistent acknowledgement, networking idempotency or reconstruction of a lost receipt's historical Before snapshot is provided. A later explicit submission of the same intent is a new action; only in-flight duplicate/reentrant execution is blocked.

**Fault boundary:** the in-flight guard covers admission observations, before capture, invocation and after capture, and is released in finally. Reentrant submissions return CommandInFlight without disturbing the outer operation. Start reentry stays false. A before-read failure records Faulted without invoking; an invocation exception records Faulted with InvocationStarted true / InvocationCompleted false; an after-read failure records Faulted with both true. The original exception identity and stack are preserved on ExecutionFailure, the existing owner lifecycle becomes Faulted, and later commands are rejected. Capture partial state where possible; otherwise After remains null with an observation diagnostic while fault-time events remain readable. There is no rollback, automatic retry, repair or implied successful turn finalization. Existing Start still rethrows its original exception and stores StartFailure; its finally captures the partial start event range.

**Validation preview decision:** no preview is added. The engine's existing gateway is private, and its exact post-start validation result is not returned by ExecuteTurn. Creating a second gateway or changing the domain API solely for an optional preview is outside this boundary. Receipts do not fabricate that result, parse ActionBlocked messages or quote affordability/Silence/SkillSeal/final cost. Consumers have actual emitted event values and supported post-state only.

**Permanent verification:** CombatIntentBoundarySmokeTests adds **174** assertions under Player Progression / Combat Validation, reusing the existing real-session fixture and engine. It covers all three intents, admission/no mutation, prepared authorization blocking and normal turn consumption, retained SkillProgress/one cost/one XP grant, start-hook ordering, literal messages, immutable old receipts, contiguous consumer indices, lost-receipt recovery, cursor bounds and empty reads, refill early/late denial and no-op transfer, lethal start/action/end, reentry, fault-time partial mutation/original exception/no retry, failed Start observation, and before/invocation/after projection faults. Test-only wrappers delegate the existing skill; no production skill/content is added. Reflection supplements behavioral tests for immutable surfaces and the otherwise unconstructible unknown intent enum. The existing P9.1 public-property whitelist now includes ExecutionFailure; its **156** assertions and P9.2's **149** remain intact.

**Verification evidence:** unchanged baseline restore/build/smoke/diff-check/status exit **0**, **4067 assertions**, **38 V1 events**, **0 warnings / 0 errors**. A test-authoring build initially exited **1** (two CS8123 warnings and two CS1061 errors from tuple name inference); explicitly typing the test matrix corrected it, with no domain defect claimed or artificial red regression. Final restore/build/smoke/diff-check/status exit **0**, build **0 warnings / 0 errors**. Final **4241** = baseline **4067** + new **174**: V1 **2**, Shared **332**, V2 **371**, V3 **686**, Progression **2850**; V1 events **38**. Existing AUD/P8 and save/old-JSON regressions remain green. The integrated boundary workflows are this feature's verification iteration.

**Scope and limitations:** seven new application value/enum files, one extended existing owner, one focused smoke suite, its runner registration, one prior surface assertion adjustment and this Bible. Domain engine/gateway/refill/lifecycle, factory/session implementations, save schema, event payloads, project packages, Unity and generators are unchanged. Trusted single-threaded ownership and valid nonmutating extension getters remain prerequisites, not a concurrency or malicious-extension defense. No enemy command/AI, actor alternation, initiative, availability service, presentation, Defend/Wait or new gameplay rules. Unity/Android/manual HUD behavior remains unverified and is not required for this backend-only diff. Stop after P9.3 for independent architectural review: no PR, merge or P9.4.

<a id="combat-turn-flow"></a>
### P9.4: Combat Encounter Turn Flow

**COMPLETE within the authorized scheduling scope (2026-11-05).** Branch `codex/combat-encounter-turn-flow` starts from fetched origin/main `ba1365f`, containing P9.3 commit `1e46c9d0cc9f931556abd1e995f80f1574e23044`. Unchanged baseline restore/build/smoke exit 0: **4241 assertions**, **38 V1 events**, **0 warnings / 0 errors**. The design gate below was recorded before production edits. Earlier closures are historical; this mission authorizes P9.4 only, not P9.5.

**Pre-implementation inventory and decision:** the existing engine starts basic/skill lifecycle before authorization, Silence/SkillSeal and resource checks. Normal attacks, skills and blocked skills therefore consume an opportunity when TurnStarted is observed. Refill can deny reserve feasibility before Begin (no consumption), or fail transfer after Begin (consumption); nonpositive/full-pool refill still starts and consumes. Begin emits TurnStarted before legacy hooks and possible attacker death. Defender defeat returns before Complete; Complete can kill the attacker before TurnEnded and counter advancement. Exceptions can leave partial state and events. Accordingly, opportunity consumption is admission plus an observed TurnStarted in that invocation's own range; it never requires TurnEnded, a counter increment, resource change or a true refill result. Terminal and Faulted always suppress scheduling regardless of consumption. No essential distinction requires Domain changes or message parsing.

**Ownership/compatibility decision:** evolve SubmitIntent centrally, retaining its signature/receipt and all admission/engine delegation contracts. Add explicit AdvanceEnemyTurn on that same owner, sharing its existing commandInFlight guard and execution/observation path. Only a consumed nonterminal player action enables one enemy basic attack; no automatic enemy execution. A completed consumed nonterminal enemy action returns control to the player. Extend the immutable receipt with actor side, consumption and flow observations; keep CombatEncounterSnapshot unchanged and expose a separate scalar-only flow snapshot. The owner alone stores scheduling phase; terminal/fault observations reuse CombatEncounterState, not a competing lifecycle. TurnNumber remains an untouched domain counter. Existing P9.3 repeated-player tests will insert legitimate enemy advancement while preserving their original assertions and event-recovery intent.

#### Final Flow Contract

`CombatEncounterComposition.GetTurnFlowSnapshot()` returns a fresh sealed, getter-only CombatTurnFlowSnapshot with Phase, nullable EligibleActor, IsTerminal and IsFaulted. CombatActorSide is Player or Enemy; only AwaitingPlayer/AwaitingEnemy has an eligible side. No engine/runtime/session/skill reference or new opportunity counter is exposed. This scalar-only read is permitted inside hooks to observe ResolvingPlayer/ResolvingEnemy; full combat snapshots and event reads retain P9.3's in-flight rejection. Prepared and Starting have NotStarted/no eligible actor until Start returns successfully. Terminal/Faulted derive from existing owner State and override scheduling; no competing terminal/fault flags are stored.

| Current phase / observation | Operation / evidence | Next phase |
|---|---|---|
| NotStarted | Successful Start with live encounter | AwaitingPlayer |
| AwaitingPlayer | Admission rejected | AwaitingPlayer, no gameplay/events |
| AwaitingPlayer | Admitted SubmitIntent | ResolvingPlayer during capture/invocation |
| ResolvingPlayer | Returned, no TurnStarted, still live (early refill denial) | AwaitingPlayer |
| ResolvingPlayer | Returned, TurnStarted observed, still live | AwaitingEnemy |
| AwaitingEnemy | Player SubmitIntent | Unchanged; NotPlayerOpportunity rejection |
| AwaitingEnemy | Explicit AdvanceEnemyTurn | ResolvingEnemy |
| ResolvingEnemy | Returned, TurnStarted observed, still live | AwaitingPlayer |
| AwaitingPlayer | AdvanceEnemyTurn | Unchanged; NoEnemyOpportunity rejection |
| Starting or either resolving operation | Either action entry reentered | CommandInFlight rejection; outer operation preserved |
| Any started action / pending opportunity | Existing ended flag or defeated combatant | Terminal; no next actor |
| Start or accepted command | Original exception, including before/after projection failure | Existing Faulted owner; no next actor/retry |

The same private ExecuteCommand path handles both public entries. The enemy branch constructs only `TurnRequest.BasicAttack(Runtime.Enemy, Runtime.Player)` and calls the retained engine once. No actor selection argument, enemy skill/refill, queue, automatic retaliation or public alternate execution controller exists. Invalid enemy advancement before Start, after terminal/fault or outside AwaitingEnemy returns the existing receipt type with explicit diagnostics and an empty current range. Existing lifecycle precedence remains busy, faulted, not started, terminal, then opportunity gating and player payload validation. Thus a null player intent during AwaitingEnemy reports NotPlayerOpportunity; when AwaitingPlayer, its original NullIntent diagnostic remains. Existing enum values are preserved, with the two opportunity reasons appended.

**Receipt additions:** ActorSide identifies the requested actor even on rejection; OpportunityConsumed equals accepted admission AND TurnStartedObserved in that invocation's batch; FlowBefore/FlowAfter are detached scheduling snapshots. InvocationCompleted, Disposition.Completed and RefillResult retain P9.3 meanings, not gameplay success. A fault after TurnStarted can have OpportunityConsumed true and InvocationCompleted false, but FlowAfter is Faulted with no next actor. A pre-invocation projection fault consumes nothing; a post-return projection fault preserves InvocationCompleted true and actual lifecycle evidence. Terminal lethal actions consume once even without TurnEnded/counter advancement. A rejection consumes nothing and has unchanged before/after flow. There is no extra wrapper receipt or duplicated event/snapshot graph.

**Compatibility and bypass boundary:** SubmitIntent retains its public signature and P9.3 facts, but intentionally no longer permits consecutive player actions without a consumed enemy opportunity between them. Invalid/early-denied actions may be retried while still AwaitingPlayer. Both entries share commandInFlight and central phase admission, so choosing the old method cannot bypass turn gating. Duplicate enemy advancement after completion rejects until another player opportunity is consumed. Internal engine/runtime access remains for trusted assembly composition and existing friend smoke tests, not public UI execution. Prepared dependencies remain trusted single-owner inputs; out-of-band internal/external mutations of retained objects or malicious hooks are not a supported scheduler API. This is not thread-safe, network-idempotent or adversarial isolation.

**Event/cursor integration:** Start, player and enemy operations share the single append-only log. Consecutive action ranges are disjoint and contiguous; start remains a separate fixed range. No message parsing determines ownership or consumption. Consumers follow the unchanged P9.3 protocol: recover missing prefix, process each index once, advance only after processing, and never treat an empty rejected range as acknowledgement. Lost player/enemy receipts are recovered with ReadEvents(next), not resubmission or another enemy advance. Previous combat/flow snapshots and batches remain detached. Repeating a historical read is deliberate observation replay; delayed calls after a later legitimate opportunity are not deduplicated by request IDs.

**Terminal/fault enforcement:** State's existing ended/dead-actor check gates both action entries before execution, including stale dead actors whose raw CombatEnded flag is false and ended-with-living-actors cases. No phase is advanced on a terminal return. A fault retains original exception/stack on ExecutionFailure (StartFailure for Start), available partial snapshots and indexed events; unreadable state remains null with ObservationFailureMessage. The same finally releases the in-flight guard without clearing the fault, completing gameplay, repairing resources or retrying. P9.3's fault behavior and single-thread ownership contract remain intact.

**Permanent verification:** CombatEncounterTurnFlowSmokeTests adds **336** assertions. Real-engine integrated tests cover three alternating cycles, all public player intents gated while enemy is pending, duplicate/invalid advances, consumed class/unmastered/unlock/Silence/SkillSeal/resource blockers, retained-mastery authorization, early/late refill denial, transfer and no-op refills, player and enemy lethal start/action/end, unchanged lethal counter values, enemy actor-specific poison/DOT/duration/Essence behavior and exact existing deterministic damage/RNG. They verify no scheduling XP/Resonance spend, contiguous cursor recovery and start separation, detached old results, reentry through both entries, original player/enemy hook exceptions and partial mutation, enemy before/after projection faults and the scalar-only public flow surface. P9.1 **156**, P9.2 **149** and P9.3 **174** assertions remain green. P9.3 inserts explicit enemy advancement between consumed player actions, updates the expected domain action counter and includes enemy ranges in cursor reconstruction; no coverage is removed. P9.1/P9.2 internal engine tests remain focused ownership/projection verification rather than public scheduling examples.

**Executed evidence:** baseline **4241** plus **336** = **4577** assertions: V1 **2**, Shared **332**, V2 **371**, V3 **686**, Player Progression / Combat Validation **3186**; V1 events **38**. Final restore/build/smoke/diff-check/status exit **0**, build **0 warnings / 0 errors**. An initial test-authoring build exited **1** with CS1729 (missing allowsStacking constructor argument in the new SkillSeal fixture; 0 warnings/1 error); fixing only that test yielded passing builds/smokes. This is an additive scheduling feature, not a fabricated defect reproduction. All AUD/P8 and existing persistence/old-JSON suites remain green. These focused integrated workflows are the verification iteration for this feature.

**Scope and remaining gates:** three new immutable flow/side types; existing owner, receipt, admission enum and internal event-type helper; focused smoke suite, runner registration and P9.3 test migration; this Bible only for docs. Domain, engine formulas, resource/status/authorization rules, save schemas, factory/session, Unity and generators remain unchanged. Scheduling is encounter-local transient state, not resumable/persisted combat. No AI, enemy skills, initiative, party/multiple enemies, Defend/Wait, animations, UI or automatic advancement. Unity/Android and visual pacing remain unverified. Stop after P9.4 for independent architectural review: no PR, merge or P9.5/later implementation.

<a id="combat-presentation-verification"></a>
### P9.5: External Combat Presentation Workflow Verification

**COMPLETE for public application APIs and headless interaction only (2026-11-05).** Branch `codex/combat-presentation-boundary-workflow-verification` starts from freshly fetched origin/main `72f6bca`; merge-base verifies P9.4 commit `24b941112e7e5bd074b6ff5a4e36cb8432c5fc1b` is included. Baseline restore/build/core smoke exit 0, **4577 assertions**, **38 V1 events**, zero warnings/errors. This verification does not produce a playable game or authorize P9.6.

#### Discovery and External Boundary

Public construction already suffices. PlayerSessionBootstrap constructs one PlayerSessionComposition, which constructs one LoadedPlayerSession. Its RuntimeState and registries are public trusted-composition APIs, as are ClassProfile assignment, MasteryProfile progression, SkillDefinition/SkillProgress constructors, CombatEntity/resources, PreparedCombatSkillBinding, CombatEncounterPreparation and CombatEncounterComposition constructors. The test registers one canonical skill definition in its session and binds the existing BalanceRitualSkill to matching progress; its numeric fixture metadata is not a new production content catalog or balance rule. Canonical metadata is validated by the existing owner. No new factory, visibility change or production fix is required.

**Trusted assembly versus presentation:** TrustedEncounterAssembly builds each independent scenario through those public APIs, configures status/passive hooks before handoff and returns only the encounter owner (plus an expected exception reference to the test harness for identity verification). It never constructs an engine or CombatRuntimeContext. The existing owner creates those through the existing loaded-session factory. No setup reference to entities, progress, pools, session, log or hook is passed to PresentationConsumer. One owner/session/log exists per scenario; independently constructed scenarios are separate tests, not parallel owners of the same encounter. Hooks installed on Start use the same authoritative log; preparation remains fresh-log validated.

`Game.PresentationBoundary.SmokeTests` is a separate net10.0 executable referencing Game, included in Debug/Release solution builds. Its unique assembly name is not the existing friend assembly `FragmentsOfWisdom.Game.SmokeTests`. No friend grant or project reference to that smoke suite was added. Existing P9.1/P9.2/P9.3/P9.4 tests use friend-only engine/runtime inspection for their own contracts and remain unchanged; they are not claimed as external-consumer proof. The external executable compiles using public members only. Supplemental reflection inspects public property metadata and friend-attribute names; it never reads private/internal fields, invokes hidden members or constructs inaccessible types. Public projection graph checks exclude mutable entities, SkillProgress, ResonanceProfile, engine, runtime and session references.

#### Adapter API Inventory

| API / retained value | Responsibility and access |
|---|---|
| CombatEncounterComposition | The consumer's only live gameplay handle; created by trusted composition, not by the view |
| Start() | Gameplay-affecting, explicit once; no implicit start from reads |
| SubmitIntent(BasicAttack / Skill(id) / Refill(amount)) | Gameplay-affecting, centrally gated player command; receipt is not a generic success verdict |
| AdvanceEnemyTurn() | Gameplay-affecting, explicit pending enemy basic attack; no automatic animation-driven replay |
| GetSnapshot(), GetTurnFlowSnapshot() | Read-only fresh detached combat/flow observations; full snapshot reads reject in-flight capture |
| ReadStartEvents(), ReadEvents(cursor, optionalEnd) | Read-only indexed immutable copies; no hidden acknowledgement or engine execution |
| State, StartFailure, ExecutionFailure | Public lifecycle/original exception diagnostics; exception objects stay separate from detached receipt values |
| CombatIntentReceipt and nested values | Immutable observed admission/invocation/consumption/flow/partial-state facts and event batch |
| Consumer cursor, displayed values and transcript | Test-only presentation state, never authoritative gameplay or a production event bus/store |

The public bootstrap/session APIs can still manufacture engines for trusted application composition; they are not capabilities handed to this consumer. The boundary is disciplined dependency ownership, not a security sandbox preventing arbitrary code from referencing other public Domain types. Public projection absence checks and actual non-friend compilation establish the intended adapter contract without widening it.

#### Consumer and Recovery Protocol

PresentationConsumer holds one owner, detached display snapshots, one integer cursor and a presentation transcript. It submits intentions and reads phase facts; it neither calculates damage/availability nor accesses mutable source objects. All display messages are copied verbatim. A deliberately misleading start message containing TurnEnded and damage text does not create lifecycle or damage facts.

Normal Start delivery is acknowledged before the first action. For a receipt whose start is ahead of the cursor, ReadEvents(cursor, start) supplies the missing prefix first. Previously acknowledged overlapping indices are skipped. A rejected receipt does not acknowledge any range, including older pending Start/fault events. The simulator stages each batch's display values locally and updates displayed entries/cursor only after all items process successfully. The failure test throws after one staged item: nothing from that batch is published, its cursor remains unchanged, and ReadEvents(cursor) can replay it. A previously completed prefix may already be acknowledged; each range commits independently. This is a test consumer convention, not a new domain transaction or a guarantee about irreversible real animations.

Lost enemy receipts recover events and current snapshots, never gameplay, through ReadEvents(cursor) and read-only observation. Historical reads preserve original values without executing turns. A late duplicate receipt does not redisplay already acknowledged indices. End-to-end comparison verifies every displayed Index/Type/Message against the public projection of the authoritative log, with no gaps or duplicates. A second identically prepared encounter inserts extra snapshots/start/full-log reads between actions; the complete varying-RNG-driven final state and event stream remain identical. This is external observational evidence, not access to an internal RNG counter.

**Future input/animation policy:** the owner rejects extra player input while AwaitingEnemy and duplicate enemy advancement after returning AwaitingPlayer. A real adapter must also disable input/advancement while its prior receipt is being presented, then call AdvanceEnemyTurn once at the explicit presentation-completion point. Do not schedule from TurnNumber, retry an action after display failure, or interpret a consumed blocked skill as free admission rejection. P9.5 tests headless gating and staged processing; real animation/input races, asynchronous cancellation and engine/UI threading are not verified. On Terminal/Faulted, stop submitting, display available final/partial snapshots, consume remaining events and retain fault diagnostics. Neither state permits further action, rollback or automatic retry.

#### Complete Executed Workflow Transcripts

Rows below record every successful action, not computed damage estimates. All ranges are half-open. The executable also prints every admission rejection: unknown skill immediately after Start; an extra player submission after each live player action; duplicate enemy advance after each live enemy action; both entries after termination. These reject with SkillNotPrepared, NotPlayerOpportunity, NoEnemyOpportunity and EncounterTerminal respectively, with empty ranges at the displayed cursor and unchanged state.

| Player victory operation | Event range / cursor | Phase after | Player/enemy HP | Player Essence / active Resonance / skill XP |
|---|---|---|---|---|
| Start | [0,2) / 2 | AwaitingPlayer | 100 / 30 | 40 / 20 / 0 |
| Player basic | [2,7) / 7 | AwaitingEnemy | 99 / 24 | 40 / 20 / 0 |
| Enemy basic | [7,12) / 12 | AwaitingPlayer | 93 / 24 | 40 / 20 / 0 |
| Player Balance Ritual | [12,20) / 20 | AwaitingEnemy | 92 / 18 | 38 / 18 / 10 |
| Enemy basic, existing Burn lifecycle | [20,26) / 26 | AwaitingPlayer | 87 / 13 | 38 / 18 / 10 |
| Player refill(4) | [26,30) / 30 | AwaitingEnemy | 86 / 13 | 40 / 20 / 10 |
| Enemy basic, existing Burn lifecycle | [30,37) / 37 | AwaitingPlayer | 81 / 8 | 40 / 20 / 10 |
| Player basic | [37,42) / 42 | AwaitingEnemy | 81 / 2 | 40 / 20 / 10 |
| Enemy basic | [42,47) / 47 | AwaitingPlayer | 75 / 2 | 40 / 20 / 10 |
| Player lethal basic | [47,51) / 51 | Terminal | 75 / 0 | 40 / 20 / 10 |

The player starts with an existing three-boundary Poison status (potency 1). Initial and first-receipt collections remain immutable after later status ticks, skill XP, refill and terminal actions. Final event cursor **51** accounts for every emitted event exactly once. No actor is killed by test-side mutation after handoff; the final basic attack is legitimate and no enemy retaliation follows.

| Enemy victory operation | Event range / cursor | Phase after | Player/enemy HP | Player Essence / active Resonance / skill XP |
|---|---|---|---|---|
| Start | [0,2) / 2 | AwaitingPlayer | 15 / 100 | 40 / 20 / 0 |
| Player basic | [2,7) / 7 | AwaitingEnemy | 15 / 94 | 40 / 20 / 0 |
| Enemy basic | [7,12) / 12 | AwaitingPlayer | 9 / 94 | 40 / 20 / 0 |
| Player Balance Ritual | [12,20) / 20 | AwaitingEnemy | 9 / 88 | 38 / 18 / 10 |
| Enemy basic | [20,26) / 26 | AwaitingPlayer | 4 / 83 | 38 / 18 / 10 |
| Player refill(4) | [26,30) / 30 | AwaitingEnemy | 4 / 83 | 40 / 20 / 10 |
| Enemy lethal basic | [30,35) / 35 | Terminal | 0 / 78 | 40 / 20 / 10 |

Other executed cases: class-blocked and cross-class mastery 99 skills emit ActionBlocked, no XP/Resonance spending and AwaitingEnemy; mastery 100 permits the existing skill. Early reserve denial returns false, consumes no opportunity and stays AwaitingPlayer. A trusted preconfigured hook drains reserve after Begin: false still consumes and yields AwaitingEnemy. Missing Resonance is false/null and rejects refill; a present zero-capacity active pool is true/non-null/zero, not treated as absent. Recovery scenario acknowledges a missing Start prefix, loses an enemy receipt, then recovers a failed skill-batch presentation to cursor **20** with unchanged gameplay and no duplicate indices.

**Fault evidence:** separately preconfigured player/enemy hooks apply existing public damage/resource mutations during authoritative execution, append one diagnostic event and throw the configured original exception. The consumer cannot access those hooks. Player fault ends with HP **93**, Essence **37**, fault range **[3,5)**, recovered cursor **5**; enemy fault follows a legitimate refill(0), ends with enemy HP **23**, enemy Essence **37**, range **[7,9)**, cursor **9**. Both retain the exact original exception and ExecuteTurn stack, expose partial snapshots and Faulted/no eligible actor, grant no skill XP and reject both later entries. Post-fault rejection does not acknowledge pending fault events. Recovery leaves state unchanged: no retry, rollback or normal end-turn regeneration is fabricated.

#### Verification and Remaining Limits

The standalone executable passes **230 assertions**; unchanged Game.SmokeTests passes **4577**, including P9.1 **156**, P9.2 **149**, P9.3 **174**, P9.4 **336**, every AUD/P8 regression and **38 V1 events**. Combined verification is **4807**, not a claim that the core runner contains the external checks. Restore, final solution build, independent external build, both smoke runs, diff-check and status exit **0**; final builds have **0 warnings / 0 errors**. The solution includes the new project so compilation is permanent, but both executables must be run using the commands in the current verification section.

**Investigation failures preserved:** the initial restricted solution build exited 1 without compiler errors in its summary; a diagnostic serial build exposed a denied compiler-server named pipe, and independent compilation also identified test-authoring CS0246/CS0535 from a missing public Resolvers namespace import (0 warnings/2 errors). Correcting the test import and using approved expanded execution allowed a clean build. The first external run exited 1 after **204** passing assertions because the test installed a debuff into the encounter log before preparation; production correctly rejected the non-fresh log. Moving that trusted fixture installation into its preconfigured Start hook preserved the existing contract, one log and consumer separation. No production defect or public composition gap was hidden with internal access.

**Changed scope:** solution registration, four files in the new external verification project (project, trusted assembler, consumer, runner) and this Bible. No production source, existing tests, friend configuration, gameplay formulas, save schema, Unity or generator changes. This verifies public .NET application APIs, external compilation and headless interaction only. It does **not** verify Unity assembly/API compatibility, Editor import, rendering, animation, Android packaging, device performance, real input races or a playable game. Snapshot durability, cross-encounter save/exit behavior, asynchronous presentation rollback and untrusted executable plugins are not added. No new architectural blocker was found for the scoped public workflow; platform integration requires its separate gate. Audit findings/priorities remain unchanged. Stop for independent architectural review: no PR, merge or P9.6 until this verification is reviewed and merged.

### Unresolved Naming and Compatibility Work

- P8A is complete: ProgressRestoreCoordinator, ProgressRestoreResult and ProgressRestoreFailureReason are canonical; V4-prefixed public entry points remain compatibility-only, not a second implementation.
- P8B is complete: canonical unlock/class-mastery diagnostic views and typed reason constants are available; historical enum strings, JSON names and named constructor arguments remain deliberate compatibility contracts.
- Defer IV2SavePathProvider, UnityV2SavePathProvider, UnityV2SaveService, V2ProgressSnapshot* and V2*Snapshot DTO renames: serialization/public API blast radius requires an isolated persistence plan.
- Retain version-prefixed branch names, migration notes, schema history and explicit compatibility guards as provenance.
- Keep documentation closure factual; do not reintroduce independent TODO/specification files.

### Preserved Deferred Gameplay Items

This is the unresolved inventory from the historical backlog, not additional approved work:
- Smarter enemy AI; status stacking, immunity windows and Poison/Burn synergy; Atlas prediction API; balance simulator.
- Full Tlamatini tree, additional culturally reviewed Mesoamerican-inspired classes, item-taught skills and cognition UI.
- Active/fixed/equipped teaching persistence beyond the current foundation; Lyra/Atlas hybrid skill; Gepetto/Spark after validation; teaching slot upgrades.
- Partial objective progress; event-based mission tracking; mission chains/prerequisites; repeatable bounties; story mission examples.
- Codex completion thresholds, reward bridge, discovery stages and UI models.
- Item registry, equipment registry, affixes, loot tables and equipment mastery.
- Richer alignment consequences and actual transmutation; no invented economy formulas.
- Town restoration: buildings/capabilities linked to learned teachings and later mastery upgrades, not free placement. Future shop/workshop/cooking/alchemy functions need separate approval.
- Sanctuary reflection: real selected milestones only, no score/XP/percentages; stable event/persistence contracts first.
- Visual adapter workflow verification/audit after the approved smallest demo; preserve FoundationVerticalSlice and do not construct competing sessions, bindings or engines.

### Explicit Exclusions Until Authorized

No broad generator/bootstrap rewrite, legacy slice replacement, save-format change, combat-policy redesign, rewards/economy additions, full-game expansion, Unity asset renaming or unrelated feature work inside cleanup. Hybrid combat is a design-only track until its timing and authority contract is approved. Source inspection does not settle open cultural, narrative, balance or platform choices.

<a id="historical-archive"></a>
# Part VII: Historical Decisions & Reference Archive

**HISTORICAL REFERENCE, NOT CURRENT INSTRUCTIONS.** These complete source records preserve technical detail, formulas, overloads, edge cases, compatibility requirements and unresolved decisions. Their old counts, "next branch" instructions, links and optimistic safety claims are not current authority. Read Parts II-VI for corrections and the only active backlog.

The original documents are presented as fenced Markdown source inside collapsible sections so their headings/backlogs do not compete with current navigation and their obsolete links are not live links. Source wording, examples, tables and encoding artifacts are preserved. CRLF/LF presentation is normalized; the two trailing spaces on original Archive 01 line 72 (after `Classes = how Nova fights.`) are omitted to pass whitespace checks. No other source whitespace is changed. Original Git blob IDs permit byte-level source recovery. No game/code/test file is replaced by this archive.

<a id="preservation-matrix"></a>
## Preservation Matrix and Source Inventory

Each original archived body was compared in full to `git cat-file blob <original-sha>`, normalizing line endings and the wrapper's final newline. All eleven matched before removal. Final fenced bodies are checked again with the single documented trailing-space normalization above. No truncated sections were accepted. The ten surviving files' tree blob IDs also matched the recorded originals before removal. FOUNDATION_MASTER_SPEC was already absent and was recovered from its recorded Git blob. "Retired" describes the separate Markdown original, not removal of its content.

| Original document (historical path) | Original Git blob SHA | Verified | Integrated location | Separate original retired |
|---|---|---|---|---|
| `docs/FOUNDATION_MASTER_SPEC.md` | `98cc5f90f0f62d10b0e39c443f5b5d5e229b28f6` | Yes, full text | Parts I-II; [Archive 01](#archive-01) | Already absent before this pass |
| `docs/V3_Combat_Foundation.md` | `08d2348ef926e3ad136e4ef0b62d017ee746cdaa` | Yes, full text | Combat rules; [Archive 02](#archive-02) | Yes, this pass |
| `docs/V2_Narrative_Foundation.md` | `3e20aec0ae8357fa0afdb4ea1326fcc25e35e469` | Yes, full text | Narrative rules; [Archive 03](#archive-03) | Yes, this pass |
| `docs/V2_Save_Foundation.md` | `640ff15d099b2e9e4f79fac1034fe29abd62047f` | Yes, full text | Persistence; [Archive 04](#archive-04) | Yes, this pass |
| `docs/PlayerProgression_Foundation.md` | `9b6e1a70d52cd8d5e221db33c969fc7084628b4e` | Yes, full text | Progression / runtime / persistence; [Archive 05](#archive-05) | Yes, this pass |
| `docs/Foundation_Version_Prefix_Naming_Audit.md` | `c231625cc1852edd2129cb633352a98d49f7b80e` | Yes, full text | Reconciliation / naming backlog; [Archive 06](#archive-06) | Yes, this pass |
| `docs/TODO_BACKLOG.md` | `01bf92203ef6d2a9c00ae446e77b05505126d888` | Yes, full text | Current backlog; [Archive 07](#archive-07) | Yes, this pass |
| `docs/CODEX_WORKFLOW.md` | `5277f5003f52e0f4dc65f0905ed79f13cda8b4e9` | Yes, full text | Governance; [Archive 08](#archive-08) | Yes, this pass |
| `docs/STRUCTURE_NOTES.md` | `6881774e8f133fabb60c2b4815503f4f2463c51c` | Yes, full text | Architecture / governance; [Archive 09](#archive-09) | Yes, this pass |
| `docs/GENERATOR_USAGE.md` | `c73c308cd3aa3030933f767cd740bf12dfe2f45a` | Yes, full text | Historical tooling; [Archive 10](#archive-10) | Yes, this pass |
| `docs/Workflow_Verification_Checklist.md` | `edd9df140fe118bf764b12e6e7f0ddb2d440db0c` | Yes, full text | Workflow verification; [Archive 11](#archive-11) | Yes, this pass |

Other provenance:
- The pre-consolidation Bible is recoverable at editing-baseline blob `1dbb44611a631e3f815831ec71236b984363a6a2`. Creative material was retained and contradictory active claims reconciled above.
- Binary design references remain untouched: `Fragments Of Wysdom — Codex Ui Concept V0.docx`, `Fragments Of Wysdom — Visual Foundation Bible V0.docx`, `Fragments of Wisdom — Vertical Slice Iteration 2 Spec.docx`, `LLM_Conception.pdf` and `📘 FOUNDATION MASTER SPEC — NOVA RPG.pdf`.
- The 425-page LLM conception PDF was only partially text-inspected; the 11-page foundation PDF had text inspection, not complete visual verification. Binary references are not certified as fully consolidated and must not be deleted.
- The unrelated local `Fragments_of_Wysdom_Game_Design_Architecture_Bible_v1.docx` remains untracked and untouched.
- Design conversations informed Part I; they are not proof that content or mechanics exist. Unsettled decisions remain open.
- Old tool-entry prose is preserved in [Tool entrypoint provenance](#tool-entrypoint-provenance). The live tools README files now only navigate here.

<a id="archive-01"></a>
## Archive 01: FOUNDATION_MASTER_SPEC.md

Historical source path: `docs/FOUNDATION_MASTER_SPEC.md`. Original blob: `98cc5f90f0f62d10b0e39c443f5b5d5e229b28f6`.

<details>
<summary>Show complete historical Markdown source (01)</summary>

``````markdown
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
``````

</details>

<a id="archive-02"></a>
## Archive 02: V3_Combat_Foundation.md

Historical source path: `docs/V3_Combat_Foundation.md`. Original blob: `08d2348ef926e3ad136e4ef0b62d017ee746cdaa`.

<details>
<summary>Show complete historical Markdown source (02)</summary>

``````markdown
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
``````

</details>

<a id="archive-03"></a>
## Archive 03: V2_Narrative_Foundation.md

Historical source path: `docs/V2_Narrative_Foundation.md`. Original blob: `3e20aec0ae8357fa0afdb4ea1326fcc25e35e469`.

<details>
<summary>Show complete historical Markdown source (03)</summary>

``````markdown
# V2 Narrative Foundation

This document describes the current Vertical Slice V2 narrative foundation. It is meant to help future Codex branches and contributors extend the architecture without collapsing domain boundaries or changing the V1 gameplay loop by accident.

For V2 profile persistence, snapshots, JSON, file IO, and Unity save adapters, see [V2_Save_Foundation.md](V2_Save_Foundation.md).

## Current Flow

```text
PlayerProfile
  -> Alignment state
  -> FirstNahualTeachingEvaluator
  -> TransmutationAccessPolicy
  -> NpcReactionService
  -> NarrativeHookService
  -> MissionTriggerService
  -> MissionTriggerCandidateDeduplicator
  -> CodexEntryService
  -> CodexEntryCandidateDeduplicator
```

The flow is fact-based. Each step reads the facts it is allowed to know about, returns a lightweight result, and leaves state mutation to future explicit systems.

## Layer Responsibilities

### PlayerProfile and Alignment

`PlayerProfile` is the source of current player state for this foundation. The V2 narrative path currently reads:

- `CurrentAlignment`
- `AlignmentProgression`
- `AlignmentFlags`

Alignment remains domain state. It should not know about UI, scenes, missions, codex entries, combat effects, dialogue trees, or Unity objects.

### FirstNahualTeachingEvaluator

`FirstNahualTeachingEvaluator` reads `PlayerProfile` alignment state and returns `FirstNahualTeachingResult`.

It decides only whether the first Nahual teaching path is accepted or rejected, and emits narrative tags describing those facts. It does not unlock mechanics, start missions, modify profile state, or drive presentation.

### TransmutationAccessPolicy

`TransmutationAccessPolicy` reads either a `PlayerProfile` or a `FirstNahualTeachingResult` and returns `TransmutationAccessResult`.

It decides whether transmutation access is currently allowed, which alignment or teaching path is required, and why access is blocked when it is blocked. It does not implement transmutation mechanics, item conversion, combat effects, rewards, or UI.

### NpcReactionService

`NpcReactionService` consumes narrative tags and returns `NpcReactionResult`.

It maps existing facts into NPC reaction facts such as tone, trust preview, fear preview, available narrative hooks, and reaction tags. It does not mutate reputation, spawn dialogue, trigger missions, or touch `PlayerProfile`.

### NarrativeHookService

`NarrativeHookService` consumes narrative tags from `FirstNahualTeachingResult`, `TransmutationAccessResult`, `NpcReactionResult`, or a raw tag list and returns `NarrativeHook` facts.

It translates already-known narrative facts into hook candidates for future systems. It does not create mission state, codex entries, dialogue branches, world state, or Unity scene behavior.

### MissionTriggerService

`MissionTriggerService` consumes `NarrativeHook` facts and returns `MissionTriggerCandidate` facts.

It names possible mission trigger candidates for future journal or mission systems. It does not start missions, track objectives, apply rewards, save progress, or alter the V1 slice.

### CodexEntryService

`CodexEntryService` consumes `NarrativeHook` facts and `MissionTriggerCandidate` facts and returns `CodexEntryCandidate` facts.

It names possible encyclopedia/codex entry candidates by entry id, type, priority, source hook or trigger id, concept key, and narrative tags. It does not unlock entries, write to `CodexProfile`, create an encyclopedia database, track completion, grant rewards, save progress, or drive UI.

### Candidate Deduplication

Candidate deduplication is a pure domain cleanup step applied after candidates are generated. It does not change generation rules; it only collapses repeated facts into stable candidate lists.

Current rules:

- `CodexEntryCandidate` values deduplicate by `EntryId`.
- `MissionTriggerCandidate` values deduplicate by `TriggerId`.
- When duplicates exist, the candidate with the highest `Priority` provides the retained candidate fields.
- `NarrativeTags` are merged in first-seen order without duplicates.

## Allowed Dependencies

The dependency direction should stay narrow and domain-first:

- `Game.Domain.Player` may own player state containers.
- `Game.Domain.Alignment` may define alignment values, flags, and progression state.
- `Game.Domain.Narrative` may read alignment/player facts and emit narrative facts.
- `Game.Domain.Transmutation` may depend on narrative facts when deciding access policy.
- `Game.Domain.World` may consume narrative tags to preview NPC reaction facts.
- `Game.Domain.Missions` may consume narrative hooks to preview mission trigger candidates.
- `Game.Domain.Codex` may consume narrative hooks and mission trigger candidates to preview codex entry candidates.
- Smoke tests may compose the full chain to prove the foundation still works.

Domain services may depend on other pure domain services or result objects when that keeps the flow explicit. Prefer constructor injection only when a service genuinely composes another service, as `TransmutationAccessPolicy` does with `FirstNahualTeachingEvaluator`.

## Forbidden Couplings

Do not introduce these couplings into the V2 foundation:

- Domain services depending on Unity `MonoBehaviour`, `GameObject`, scenes, prefabs, assets, or serialized references.
- Narrative evaluators starting missions, adding codex entries, opening UI, triggering combat, granting rewards, or changing inventory.
- NPC reaction services mutating reputation, world state, or `PlayerProfile`.
- Mission trigger candidates becoming active mission tracking or journal state.
- Codex entry candidates becoming active encyclopedia entries, completion state, rewards, persistence, or UI.
- Transmutation access checks implementing actual transmutation mechanics.
- Generated Unity YAML edits for narrative foundation work.
- Giant orchestration MonoBehaviours that contain gameplay rules.
- Hidden dependencies through global state, static mutable registries, scene lookups, or service locators.

## Architectural Philosophy

The V2 foundation exists to create stable facts before full systems are built.

- Keep pure domain first.
- Keep evaluators side-effect free.
- Keep domain code free of Unity dependencies.
- Avoid giant MonoBehaviours.
- Let narrative systems consume facts rather than mutate state.
- Preserve the existing V1 gameplay loop and 38-event smoke assertion.
- Prefer small classes with one clear responsibility.
- Add integration only through explicit adapters when a future Unity or gameplay system is ready.

## Naming Conventions

Use stable, centralized names for cross-system facts:

- Narrative tags live in `NarrativeTags`.
- Narrative hook ids live in `NarrativeHookIds`.
- Suggested downstream systems live in `SuggestedNarrativeSystem`.
- NPC narrative hook ids live in `NpcNarrativeHooks`.
- Transmutation block reasons live in `TransmutationAccessBlockReasons`.
- Mission trigger ids live in `MissionTriggerIds`.
- Mission family names live in `MissionFamilies`.
- Codex entry ids live in `CodexEntryIds`.
- Codex concept/title keys live in `CodexConceptKeys`.

Prefer clear domain names over generic ids. Avoid repeated string literals for tags, hook ids, trigger ids, codex entry ids, concept keys, mission families, suggested systems, or block reasons.

## Narrative Tag Philosophy

Narrative tags are lightweight facts, not commands.

A tag should say what is true, not what a future system must do. For example, a tag can describe that transmutation access is blocked or that the first Nahual teaching was accepted. It should not say that a UI panel must open, a quest must start, or an NPC must speak a specific line.

Tags should be:

- Stable enough for multiple services to consume.
- Small and readable.
- Centralized when reused.
- Safe to pass through result objects.
- Free of presentation, scene, and persistence assumptions.

When adding a tag, add or update smoke coverage for the behavior that consumes it.

## Future Extension Points

### Codex System

Future codex work should consume `CodexEntryCandidate` facts. Codex systems should decide how and when entries are discovered, persisted, displayed, or completed outside the evaluator chain.

### Mission Journal

Future mission journal work should consume `MissionTriggerCandidate` facts. The current service only suggests candidates; a future journal system can decide whether to create, ignore, defer, or merge them.

### Dialogue System

Future dialogue work should consume `NpcReactionResult`, `NarrativeHook`, or narrative tags. Dialogue selection should not be embedded in `NpcReactionService`; that service only previews tone, deltas, hooks, and tags.

### Transmutation Mechanics

Future transmutation mechanics should call `TransmutationAccessPolicy` before doing work. The policy should remain a gate that returns facts. Actual conversion, costs, combat effects, inventory changes, and animation should live in separate systems.

### Reputation and World State

Future reputation or world-state systems may consume `NpcReactionResult` or `NarrativeHook` facts. They should apply durable state changes in explicit services, not inside reaction or hook resolution.

## Validation Workflow

Run these commands after changing the V2 foundation:

```powershell
dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal
dotnet run --project src\Game.SmokeTests\Game.SmokeTests.csproj --no-build
```

Current smoke expectations:

- V1 slice emitted event count remains `38`.
- Smoke output is grouped into `V1 Legacy Slice`, `Shared Domain Foundations`, `V3 Combat / Skills / Resonance / Status / Stats`, and `V2 Narrative / Progression / Persistence`.
- V1 Legacy Slice assertions: `2`.
- Shared Domain Foundations assertions: `16`.
- V3 Combat / Skills / Resonance / Status / Stats assertions: `81`.
- Alignment assertions still run.
- Nahual evaluator assertions still run.
- Transmutation policy assertions still run.
- NPC reaction assertions still run.
- Narrative hook assertions still run.
- Mission trigger assertions still run.
- Codex entry assertions still run.
- Candidate deduplication assertions still run.
- Codex profile assertions still run.
- Mission journal assertions still run.
- Discovery orchestration assertions still run.
- V2 save snapshot assertions still run.
- V2 save JSON assertions still run.
- V2 save file repository assertions still run.
- V2 foundation flow assertions still run.
- V2 Narrative / Progression / Persistence assertions: `239`.
- Total smoke assertions remain `338` unless a branch intentionally adds or renames smoke coverage and explains why.

Documentation-only changes should not alter smoke counts, gameplay behavior, Unity assets, scenes, prefabs, or `.meta` files.
``````

</details>

<a id="archive-04"></a>
## Archive 04: V2_Save_Foundation.md

Historical source path: `docs/V2_Save_Foundation.md`. Original blob: `640ff15d099b2e9e4f79fac1034fe29abd62047f`.

<details>
<summary>Show complete historical Markdown source (04)</summary>

``````markdown
# V2 Save Foundation

This document describes the current V2 save and persistence foundation. It covers only the lightweight state profiles and save plumbing that exist today; it does not describe a final save system, save slots, UI, autosave, or Unity scene integration.

## Current Flow

```text
CodexEntryCandidate facts
  -> CodexDiscoveryService
  -> CodexProfile

MissionTriggerCandidate facts
  -> MissionDiscoveryService
  -> MissionJournalProfile

CodexProfile + MissionJournalProfile
  -> DiscoveryOrchestrationService
  -> V2ProgressSnapshotService
  -> V2ProgressSnapshot
  -> V2ProgressSnapshotJsonSerializer
  -> V2ProgressSnapshotFileRepository

UnityV2SavePathProvider
  -> UnityV2SaveService
  -> V2ProgressSnapshotFileRepository
```

## Stateful Profiles

### CodexProfile

`CodexProfile` tracks discovered codex entries. Candidate-backed discoveries are stored as `CodexEntryRecord` values, preserving:

- `EntryId`
- `EntryType`
- `ConceptKey`
- first source hook id
- first source trigger id
- narrative tags

Discovery is idempotent by `EntryId`. Re-applying the same candidate does not duplicate records.

### MissionJournalProfile

`MissionJournalProfile` tracks available mission leads. Mission leads are stored as `MissionLeadRecord` values, preserving:

- `TriggerId`
- `TriggerType`
- `Priority`
- `SuggestedMissionFamily`
- `SourceHookId`
- narrative tags

Discovery is idempotent by `TriggerId`. Re-applying the same trigger candidate does not duplicate records.

## Discovery Services

`CodexDiscoveryService` consumes `CodexEntryCandidate` facts only and applies them to `CodexProfile`.

`MissionDiscoveryService` consumes `MissionTriggerCandidate` facts only and applies them to `MissionJournalProfile`.

`DiscoveryOrchestrationService` composes both discovery services. It applies supplied codex and mission candidate collections to supplied profiles and returns a lightweight summary of newly discovered ids and already-known counts. It does not contain mapping rules and does not generate candidates.

## Snapshot Model

`V2ProgressSnapshot` is the current in-memory DTO for V2 profile persistence. It captures:

- schema version
- codex discovered-entry snapshots
- mission lead snapshots

`V2ProgressSnapshotService` creates snapshots from `CodexProfile` and `MissionJournalProfile`, and restores profiles from a snapshot. Restore mutates only those profiles and remains idempotent because the profiles enforce identity rules.

## Versioning

Current snapshot schema version is centralized in `V2ProgressSnapshotVersions.CurrentSchemaVersion`.

Versioning rules:

- new snapshots use the current schema version
- `V2ProgressSnapshot.IsCurrentVersion()` checks whether a snapshot matches the current schema
- `V2ProgressSnapshotService.CanRestore()` returns true only for current-version snapshots
- restore rejects unsupported versions before mutating target profiles
- no migration support exists yet

## JSON Serialization

`V2ProgressSnapshotJsonSerializer` serializes and deserializes `V2ProgressSnapshot` only.

Serialization rules:

- use built-in `System.Text.Json`
- preserve schema version, codex entries, mission leads, tags, ids, priorities, concept keys, source ids, and mission families
- validate schema version during deserialization using the existing restore/version rules
- malformed JSON fails through `JsonException`
- unsupported schema JSON fails through `InvalidOperationException`

The serializer does not perform file IO, choose paths, manage save slots, or invoke Unity APIs.

## File Repository

`V2ProgressSnapshotFileRepository` owns file IO for V2 snapshot JSON.

Repository rules:

- `Save(path, snapshot)` serializes and writes to the exact path supplied
- `Save` creates the parent directory when needed
- `Load(path)` remains strict for missing files
- `Load` delegates malformed JSON and unsupported schema handling to the JSON serializer

The repository does not know about Unity, save slots, PlayerPrefs, cloud saves, encryption, compression, UI, or gameplay triggers.

## Unity Boundary

Unity-facing code is isolated under `src/Game.Unity`.

`UnityV2SavePathProvider` resolves the V2 save file path using `Application.persistentDataPath`.

`UnityV2SaveService` is a thin facade that composes:

- `IV2SavePathProvider`
- `V2ProgressSnapshotFileRepository`
- `V2ProgressSnapshotService`

The facade exposes:

- `Save(CodexProfile, MissionJournalProfile)`
- `LoadInto(CodexProfile, MissionJournalProfile)`
- `Exists()`

Actual JSON and file persistence remain in the repository and serializer. Unity-specific path resolution remains behind `IV2SavePathProvider`.

## Explicit Non-Goals

The current foundation does not include:

- save slot UI
- autosave
- PlayerPrefs
- `Application.persistentDataPath` in domain code
- encryption
- compression
- cloud saves
- version migrations
- gameplay-triggered saves
- active quest objectives or quest completion
- codex completion percentages
- scene, prefab, or asset changes

## Validation Workflow

Run these commands after changing the V2 save foundation:

```powershell
dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal
dotnet run --project src\Game.SmokeTests\Game.SmokeTests.csproj --no-build
```

Current smoke expectations:

- V1 slice emitted event count remains `38`.
- Smoke output is grouped into `V1 Legacy Slice`, `Shared Domain Foundations`, `V3 Combat / Skills / Resonance / Status / Stats`, and `V2 Narrative / Progression / Persistence`.
- V1 Legacy Slice assertions: `2`.
- Shared Domain Foundations assertions: `16`.
- V3 Combat / Skills / Resonance / Status / Stats assertions: `81`.
- Codex profile assertions still run.
- Mission journal assertions still run.
- Discovery orchestration assertions still run.
- V2 save snapshot assertions still run.
- V2 save JSON assertions still run.
- V2 save file repository assertions still run.
- V2 Narrative / Progression / Persistence assertions: `239`.
- Total smoke assertions remain `338` unless a branch intentionally adds or renames smoke coverage and explains why.

Unity-facing files are outside the current .NET solution. Validate them manually in Unity by compiling the `src/Game.Unity` adapter code in a Unity assembly that references the game assembly, then exercising `UnityV2SavePathProvider` and `UnityV2SaveService` in Play Mode.
``````

</details>

<a id="archive-05"></a>
## Archive 05: PlayerProgression_Foundation.md

Historical source path: `docs/PlayerProgression_Foundation.md`. Original blob: `9b6e1a70d52cd8d5e221db33c969fc7084628b4e`.

<details>
<summary>Show complete historical Markdown source (05)</summary>

``````markdown
# Player Progression Foundation

This document describes the current player progression foundation for classes,
mentors, mastery, mentor teaching, persistence, mastered skill retention, skill
tree progression, combat validation, and runtime/bootstrap ownership.

The scope remains intentionally small and domain-first. The progression
foundation stores and evaluates facts; it does not execute combat skills, grant
rewards, drive UI, or introduce Unity behavior.

## Current Flow

```text
SkillDefinition
  -> SkillDefinitionRegistry
  -> ClassDefinition allowed skill ids
  -> ClassDefinitionRegistry
  -> ClassProfile current class id
  -> SkillTreeId stable skill tree ids
  -> SkillTreeDefinition id/node/prerequisite facts
  -> ClassSkillTreeBinding class-to-skill-tree facts
  -> ClassSkillTreeBindingRegistry
  -> SkillTreeProgressProfile discovered/unlocked facts
  -> SkillTreeUnlockPolicy
  -> MasteryProfile mastered skill facts
  -> MasteredSkillRetentionPolicy
```

```text
MentorDefinition
  -> MentorDefinitionRegistry
  -> MentorRelationshipProfile discovery/trust/availability facts
  -> MentorTeachingService teaching facts
  -> MentorTeachingApplicationService explicit MasteryProfile progress
```

`UnlockProfile`, combat execution, UI, and Unity presentation remain separate.

## Class Foundation

`ClassId` centralizes stable class ids.

Current ids:

- `ClassId.Tlamatini`
- `ClassId.Wanderer`

`ClassDefinition` describes a class:

- `Id`
- `Name`
- `FixedSkillIds`
- `AllowedSkillIds`
- `AllowsSkill(skillId)`
- `AllowsSkill(skillId, skillRegistry)`
- `ResolveAllowedSkills(skillRegistry)`

`ClassDefinitionRegistry` owns class definition lookup:

- `Register(classDefinition)`
- `Contains(classId)`
- `Get(classId)`
- `ResolveCurrentClass(profile)`

Registry rules:

- valid class definitions register by stable id
- null, empty id, and empty name are rejected safely
- duplicate class ids are rejected safely
- duplicate class ids do not replace the original definition

`ClassProfile` owns only current class id state:

- `CurrentClassId`
- `AssignClass(classDefinition)`
- `ChangeClass(classDefinition)`
- `AssignClassId(classId, registry)`
- `ChangeClassId(classId, registry)`
- `ResolveCurrentClass(registry)`
- `CurrentClassAllowsSkill(skillId, registry)`
- `CurrentClassAllowsSkill(skillId, classRegistry, skillRegistry)`
- `ResolveCurrentClassAllowedSkills(classRegistry, skillRegistry)`

Class changes do not mutate unlocks, mastery, mentors, combat state, inventory,
or skill definitions.

## Skill Definition Registry

`SkillDefinitionRegistry` is the stable source of truth for skill ids used by
class, skill tree nodes, mastery, mentor teaching, retention, and snapshot
validation.

Rules:

- valid skill definitions register by stable id
- null, empty id, and empty name are rejected safely
- duplicate skill ids are rejected safely
- unknown skill ids fail safely when a registry-aware operation is used

## Skill Tree Progression

`SkillTreeId` centralizes stable skill tree ids.

Current ids:

- `SkillTreeId.Tlamatini`
- `SkillTreeId.Wanderer`

`SkillTreeNodeDefinition` describes a skill tree node:

- `Id`
- `SkillId`
- `RequiredNodeIds`

Node rules:

- node ids are stable ids
- skill ids must resolve through `SkillDefinitionRegistry`
- missing prerequisite lists are treated as empty
- prerequisite ids are copied as stable node id facts

`SkillTreeDefinition` owns a stable skill tree id and node lookup:

- `Id`
- `RegisterNode(node, skillRegistry)`
- `Contains(nodeId)`
- `Get(nodeId)`
- `IsValidNode(node, skillRegistry)`

Registry rules:

- stable skill tree ids are optional for standalone node/progression tests
- stable skill tree ids are required when registering a tree for class binding
- valid nodes register by stable node id
- prerequisite nodes must already exist in the same `SkillTreeDefinition`
- nodes must be registered in prerequisite-first order
- forward prerequisite references are rejected safely
- self-references are rejected because the node does not exist before its own
  registration
- cycles cannot be introduced through incremental registration because every
  dependency must already exist
- missing prerequisite ids reject the node without mutating the tree
- null or empty node ids are rejected safely
- nodes with unknown skill ids are rejected safely
- duplicate node ids are rejected safely
- duplicate node ids do not replace the original node

`SkillTreeProgressProfile` owns progression facts:

- discovered node ids scoped by `SkillTreeId`
- unlocked node ids scoped by `SkillTreeId`
- `DiscoverNode(nodeId, skillTree)`
- `IsDiscovered(nodeId)`
- `IsDiscovered(nodeId, skillTree)`
- `IsUnlocked(nodeId)`
- `IsUnlocked(nodeId, skillTree)`

Progress rules:

- discovery validates node ids through `SkillTreeDefinition`
- policy and application queries use the supplied `SkillTreeDefinition.Id`
- identical node ids in different trees do not share progress
- legacy node-only queries remain aggregate compatibility views
- public unlock mutation is applied through `SkillTreeUnlockApplicationService`
- the profile's low-level unlock recorder is internal for application and
  snapshot restoration only
- unlock records node facts only
- unlock does not grant skills automatically
- unlock does not mutate mastery

`SkillTreeUnlockPolicy` evaluates whether a node can be unlocked.

It returns `SkillTreeUnlockResult` facts:

- `NodeId`
- `SkillId`
- `CanUnlock`
- `IsDiscovered`
- `IsAlreadyUnlocked`
- `MissingPrerequisiteNodeId`
- `BlockReason`

Block reasons:

- `None`
- `UnknownNode`
- `UnknownSkill`
- `NodeNotDiscovered`
- `PrerequisiteMissing`

Unlock policy rules:

- unknown nodes fail safely
- node skill ids are validated through `SkillDefinitionRegistry`
- undiscovered nodes are blocked
- prerequisite node ids must already be unlocked
- unlocked prerequisites allow dependent nodes to become unlockable
- the policy returns facts only and does not grant skills

`SkillTreeUnlockApplicationService` is the public policy-enforced mutation
path. It evaluates `SkillTreeUnlockPolicy` before recording an unlock and does
not mutate progress when the policy blocks the node.

## Class Skill Tree Binding

`ClassSkillTreeBinding` stores a class-to-skill-tree fact:

- `ClassId`
- `SkillTreeId`

`ClassSkillTreeBindingRegistry` owns skill tree registration and class binding
lookup:

- `RegisterSkillTree(skillTree)`
- `RegisterBinding(binding, classRegistry)`
- `ContainsSkillTree(skillTreeId)`
- `GetSkillTree(skillTreeId)`
- `GetBindingForClass(classId)`
- `ResolveSkillTreeForClass(classId)`
- `ResolveCurrentClassSkillTree(profile)`

Binding rules:

- class ids validate through `ClassDefinitionRegistry`
- skill tree ids validate through registered `SkillTreeDefinition` values
- null bindings, empty class ids, and empty skill tree ids are rejected safely
- skill trees with empty ids are rejected by the binding registry
- duplicate class bindings are rejected safely
- duplicate class bindings do not replace the original binding
- current class skill tree resolution reads `ClassProfile.CurrentClassId`
- binding returns facts only
- binding does not grant skills automatically
- binding does not mutate `SkillTreeProgressProfile`

## Class Skill Tree Binding Snapshot And Restore

Class skill tree binding persistence extends `V2ProgressSnapshot` without a
schema version bump.

Each binding snapshot stores:

- `ClassId`
- `SkillTreeId`

Restore rules:

- class ids validate through `ClassDefinitionRegistry`
- skill tree ids validate through trees registered in
  `ClassSkillTreeBindingRegistry`
- the complete binding collection is validated before mutation
- unknown ids, duplicate class ids, or conflicting existing bindings fail
  safely
- invalid binding collections do not partially mutate the target registry
- matching existing bindings restore idempotently
- older JSON without binding data restores safely as an empty collection

## Skill Tree Snapshot And Save Behavior

Skill tree progress persistence extends `V2ProgressSnapshot` in a
backward-compatible way.

Scoped snapshot records store:

- `SkillTreeId`
- `DiscoveredNodeIds`
- `UnlockedNodeIds`

Legacy unscoped discovered/unlocked lists remain readable for older JSON.

Compatibility policy:

- legacy-only progress is valid
- scoped-only progress is valid
- empty legacy lists may accompany scoped progress
- snapshots containing both non-empty legacy progress and scoped progress are
  rejected as ambiguous
- older JSON without skill tree progress remains valid as empty progress

Restore rules:

- scoped skill tree ids validate through registered `SkillTreeDefinition`
  values
- scoped node ids validate against their matching tree
- multi-tree restore validates all scopes before mutation
- legacy unscoped progress restores when an explicit target tree is supplied
- registry-based legacy restore requires exactly one target tree
- ambiguous legacy progress with multiple target trees fails without mutation
- unknown node ids fail safely and do not mutate the target profile
- older JSON without skill tree progress data restores safely as empty progress
  state
- schema version remains unchanged because missing skill tree progress data is
  optional

`V2ProgressSnapshotValidationService` rejects null skill tree progress sections,
null discovered/unlocked node id lists, empty node ids, empty scoped skill tree
ids, and duplicate skill tree scopes.

Registry-aware validation additionally uses registered
`SkillTreeDefinition` values to require:

- every scoped `SkillTreeId` resolves
- every discovered and unlocked node id exists in its scoped tree
- every unlocked node is also discovered
- every prerequisite of an unlocked node is also unlocked

Validation without a skill tree registry remains structural-only for backward
compatibility.

## Mastery Foundation

`MasteryRecord` stores skill mastery facts:

- `SkillId`
- `Progress`
- `Rank`
- `IsMastered`

`MasteryRank` is derived from progress:

- `Unranked`
- `Practiced`
- `Mastered`

`MasteryProfile` owns mastery records:

- `Records`
- `IncreaseProgress(skillId, amount, skillRegistry)`
- `GetRecord(skillId)`
- `IsMastered(skillId)`
- `RestoreRecord(skillId, progress, skillRegistry)`

Rules:

- progress only applies for registered skill ids
- zero or negative progress does not increase mastery
- mastered progress is capped at `MasteryRecord.MasteredProgress`
- duplicate progress after mastery is safe
- mastery remains facts only

## Mastery Snapshot And Save Behavior

Mastery persistence extends `V2ProgressSnapshot` in a backward-compatible way.

Snapshot stores:

- `SkillId`
- `Progress`
- derived `Rank`

Restore rules:

- restore validates skill ids through `SkillDefinitionRegistry`
- known skill ids restore into `MasteryProfile`
- unknown skill ids fail safely and do not create mastery records
- older JSON without mastery data restores safely as empty mastery state
- schema version remains unchanged because missing mastery data is optional

`V2ProgressSnapshotValidationService` also supports registry-aware mastery
validation. When a `SkillDefinitionRegistry` is supplied, mastery snapshot skill
ids must resolve through that registry.

## Mentor Foundation

`MentorId` centralizes stable mentor ids.

Current ids:

- `MentorId.Lyra`
- `MentorId.Atlas`

`MentorDefinition` describes a mentor:

- `Id`
- `Name`
- `IsAvailable`
- `IsValid()`

`MentorDefinitionRegistry` owns mentor definition lookup:

- `Register(mentor)`
- `Contains(mentorId)`
- `Get(mentorId)`

Registry rules:

- valid mentor definitions register by stable id
- null, empty id, and empty name are rejected safely
- duplicate mentor ids are rejected safely
- duplicate mentor ids do not replace the original definition

`MentorRelationshipProfile` owns relationship facts:

- discovered mentor ids
- trust/progress per mentor id
- availability fact per discovered mentor id

Relationship rules:

- discovery validates mentor ids through `MentorDefinitionRegistry`
- duplicate discovery is idempotent
- trust increases only for discovered, registered mentors
- unavailable mentors are reported as unavailable
- unknown mentor ids fail safely

## Mentor Relationship Snapshot And Save Behavior

Mentor relationship persistence extends `V2ProgressSnapshot` in a
backward-compatible way.

Snapshot stores:

- `MentorId`
- `IsDiscovered`
- `Trust`
- `IsAvailable`

Restore rules:

- restore validates mentor ids through `MentorDefinitionRegistry`
- known mentor ids restore discovered, trust, and availability facts
- unknown mentor ids fail safely and do not create relationship state
- older JSON without mentor relationship data restores safely as empty mentor
  relationship state
- schema version remains unchanged because missing mentor relationship data is
  optional

`V2ProgressSnapshotValidationService` rejects null mentor relationship
collections and mentor relationship records with empty mentor ids.

## Mentor Teaching

`MentorTeachingDefinition` describes a teaching fact:

- `MentorId`
- `TaughtSkillId`
- `RequiredTrust`
- `MasteryProgressGranted`

`MentorTeachingService` evaluates availability. It validates:

- mentor id through `MentorDefinitionRegistry`
- taught skill id through `SkillDefinitionRegistry`
- mentor discovery through `MentorRelationshipProfile`
- required trust through `MentorRelationshipProfile`

It returns `MentorTeachingResult` facts only:

- availability
- mentor id
- taught skill id
- required trust
- current trust
- mastery progress granted
- stable block reason
- optional block message

Teaching does not grant skills, mutate mastery, start missions, affect combat, or
drive UI.

`MentorTeachingApplicationService` explicitly consumes an available
`MentorTeachingResult` and applies its mastery progress to `MasteryProfile`.

Application rules:

- blocked teaching does not mutate mastery
- unknown skill ids fail safely
- missing mastery profile fails safely
- zero or negative mastery progress is not applied
- already-mastered skills are safe and are not over-applied
- successful application mutates only `MasteryProfile`

## Mastered Skill Retention

`MasteredSkillRetentionPolicy` evaluates whether a registered skill is usable
after class changes.

A skill is usable if:

- the current class allows it, or
- it is mastered in `MasteryProfile`

The policy returns facts only:

- `SkillId`
- `IsUsable`
- `AllowedByCurrentClass`
- `AllowedByMastery`
- stable block reason

Rules:

- skill ids must resolve through `SkillDefinitionRegistry`
- class access is resolved through `ClassProfile` and `ClassDefinitionRegistry`
- mastery access is resolved through `MasteryProfile`
- no class is required when mastery allows the skill
- the policy does not mutate `ClassProfile` or `MasteryProfile`
- the policy does not execute skills or integrate with combat

## Combat-Facing Skill Access

`CombatSkillAccessService` is a read-only adapter from combat-facing callers to
`MasteredSkillRetentionPolicy`.

It returns `CombatSkillAccessResult` facts:

- `SkillId`
- `IsUsable`
- `AllowedByCurrentClass`
- `AllowedByMastery`
- `BlockReason`

Access rules:

- current-class skills are usable
- mastered retained skills remain usable after a class switch
- non-mastered cross-class skills are blocked
- unknown skill ids fail safely
- output facts are copied directly from `MasteredSkillRetentionPolicy`
- no profile, registry, combat engine, or skill execution state is mutated
- `TurnBasedCombatEngine` does not consume the service yet

## Combat Skill Use Validation

`CombatSkillUseValidator` composes structural request validation with the
existing V3 `SkillAccessPolicy` and V4 `CombatSkillAccessService`.

`CombatSkillUseValidationContext` provides explicit validation modes:

- `LegacyBypass` validates request structure and matching skill ids while
  bypassing V3/V4 progression rules
- `Enemy` validates request structure and matching skill ids without requiring
  player progression profiles
- `Player` requires `UnlockProfile`, `ClassProfile`, `MasteryProfile`,
  `ClassDefinitionRegistry`, and `SkillDefinitionRegistry`

Static context factories create each mode. Partial player context fails with
the stable `PlayerContextIncomplete` block reason. A missing context fails with
`ValidationContextMissing`.

`CombatSkillUseValidationContextProvider` centralizes context creation:

- `CreateLegacyBypass` creates only the explicit legacy mode
- `CreateEnemy` creates enemy mode without player progression dependencies
- `TryCreatePlayer` requires all five player profiles and registries
- missing player dependencies return `false` with a null context
- failed player creation never falls back to legacy or enemy mode
- produced contexts are directly consumable by `CombatSkillUseValidator`

`CombatActorValidationClassifier` determines the validation mode for a combat
actor without creating contexts or mutating combat state.

Classification rules:

- an explicitly requested legacy path classifies as `LegacyBypass`
- the runtime enemy classifies as `Enemy` without player dependencies
- the runtime player classifies as `Player` only when all five player
  dependencies are present
- null runtime, null actor, incomplete player dependencies, and actors outside
  the runtime fail with stable classification reasons
- failed classification returns `Unknown` with no validation mode
- classification uses actor identity from `CombatRuntimeContext`
- `TurnBasedCombatEngine` does not consume the classifier directly; the
  configured gateway owns classification

`CombatSkillValidationOrchestrator` composes the production validation flow:

1. classify the actor
2. create the matching validation context
3. run `CombatSkillUseValidator`
4. return combined classification and validation facts

`CombatSkillValidationOrchestrationResult` exposes whether orchestration
reached validation, whether the skill is allowed, whether a context was
created, the selected mode, the original classification, the validator result,
and a stable failure stage.

Failure behavior:

- classification failures preserve the classifier reason and do not create a
  context
- context creation failures preserve the selected classification mode
- blocked skills complete orchestration but report the `Validation` failure
  stage and preserve V3/V4 validator reasons
- successful validation reports no failure stage
- the orchestrator is read-only and does not emit events or execute combat
- `TurnBasedCombatEngine` does not consume the orchestrator directly; the
  configured gateway owns orchestration

`ICombatSkillTurnValidator` is the engine-facing validation boundary. It
exposes only:

```csharp
CombatSkillTurnValidationResult Validate(
    TurnRequest request,
    CombatRuntimeContext runtime);
```

`CombatSkillTurnValidatorGateway` implements that boundary and owns the
configuration needed by `CombatSkillValidationOrchestrator`. The default
constructor selects explicit `LegacyBypass`. Strict runtime classification can
be configured without player dependencies for enemy actions, while the player
constructor receives the complete unlock, class, mastery, and registry state
once at composition time.

`CombatSkillTurnValidationResult` flattens the engine-relevant facts:

- whether the skill is allowed
- selected validation mode
- orchestration failure stage
- actor classification failure reason
- skill-use block reason
- underlying V3 and V4 block reasons

The gateway derives the actor from `TurnRequest.Attacker`. Engine-facing
callers do not receive or pass the orchestrator, classifier, context provider,
validator, profiles, or registries per turn. The gateway remains read-only and
does not emit events, advance turns, spend resources, apply skills, grant
experience, or modify combat state.

`TurnBasedCombatEngine` accepts `ICombatSkillTurnValidator` through an optional
three-argument constructor overload. The existing two-argument constructor
remains unchanged for callers and delegates with no validator, preserving the
legacy/pass-through behavior and event sequence.

For skill actions, the engine calls the optional validator immediately before
the existing `SkillSeal` check. Allowed validation continues through the
existing skill path without adding events. Blocked validation:

- emits exactly one `ActionBlocked` event with a stable enum-based reason
- ends and consumes the turn through the existing `EndTurn` path
- does not continue into `SkillSeal`
- does not calculate or spend Essence
- does not calculate or spend Resonance
- does not execute the skill
- does not grant skill experience
- does not emit `ResourceNotEnough`

Basic attacks do not invoke combat skill turn validation.

Custom `ICombatSkillTurnValidator` implementations must return a non-null
`CombatSkillTurnValidationResult`. If a custom validator returns null, the
engine fails closed before `SkillSeal` and resource processing. It emits one
`ActionBlocked` event with the stable `NullValidationResult` reason, consumes
the turn through `EndTurn`, and does not spend resources, grant experience,
execute skill effects, or emit `ResourceNotEnough`.

`CombatSkillTurnValidationComposition` owns the explicit progression-aware construction path
without moving dependency graphs into the combat engine. No production
bootstrap call site uses that path yet.

It exposes:

```csharp
ICombatSkillTurnValidator CreateValidator(
    UnlockProfile unlockProfile,
    ClassProfile classProfile,
    MasteryProfile masteryProfile,
    ClassDefinitionRegistry classRegistry,
    SkillDefinitionRegistry skillRegistry);

TurnBasedCombatEngine CreateProgressionAwareEngine(
    IRandomSource random,
    CombatPassivePipeline pipeline,
    UnlockProfile unlockProfile,
    ClassProfile classProfile,
    MasteryProfile masteryProfile,
    ClassDefinitionRegistry classRegistry,
    SkillDefinitionRegistry skillRegistry);
```

`CreateValidator` constructs the existing
`CombatSkillTurnValidatorGateway`. `CreateProgressionAwareEngine` passes that
validator through the existing optional engine constructor. The composition
helper does not duplicate classification, context creation, validation, or
engine behavior.

Player validation requires all five player dependencies. Missing dependencies
remain configured on the gateway and fail player actions closed through the
existing classifier. Enemy actions still classify and validate without player
progression dependencies. Legacy callers continue constructing
`TurnBasedCombatEngine` with its existing two-argument constructor and remain
pass-through.

## Combat Validation Bootstrap Ownership Audit

The current production-like entry point is `FoundationVerticalSlice`. It owns a
short-lived `PlayerProfile`, combat entities, passive pipeline,
`TurnBasedCombatEngine`, and `CombatRuntimeContext`. It intentionally creates
the engine through the legacy two-argument constructor and remains the V1
event-count compatibility path.

The slice is not yet a safe V4 composition root:

- `PlayerProfile` owns the legacy `PlayerClassCollection`, but not the V4
  `UnlockProfile`, `ClassProfile`, `MasteryProfile`,
  `MentorRelationshipProfile`, or `SkillTreeProgressProfile`
- V4 restore APIs mutate caller-supplied profile instances in place
- `ClassBootstrap` can build class definitions, but there is no complete
  production bootstrap for the skill, mentor, skill-tree, and binding
  registries
- slice combat skills are currently defined locally instead of being resolved
  from one authoritative `SkillDefinitionRegistry`
- creating fresh V4 profiles or registries per combat would disconnect
  validation from restored and mutated progression state

The recommended owner is a future application/session-scoped V4 runtime state,
not `TurnBasedCombatEngine`, `CombatRuntimeContext`, or a static domain
singleton. That owner should:

- create each mutable V4 profile exactly once per loaded player session
- create each definition registry exactly once per application content
  session
- restore snapshots into those same live profile instances before combat
- provide the same profile and registry references to progression services,
  save/snapshot services, and `CreateProgressionAwareEngine`
- keep combat entities, turn order, passive pipelines, resonance, and the
  combat engine scoped to an individual combat

`CombatSkillTurnValidationComposition` is sufficient for final engine
assembly, but it does not own or build the dependency graph. Calling it is safe
only after an application owner can supply canonical live profiles and
registries.

Accidental legacy validation remains possible anywhere callers use the
two-argument engine constructor. This is required for V1 compatibility, so the
future V4 application path should centralize engine creation and avoid direct
constructor use without changing the legacy slice.

The earliest safe integration point is the future application-level
per-combat creation method, after session state has been created or restored
and the canonical registries are available. `CreateProgressionAwareEngine` should be
called there immediately before constructing the matching
`CombatRuntimeContext`.

Unresolved risks before integration:

- legacy `PlayerClassCollection` and V4 `ClassProfile` can drift unless one
  application owner defines their roles
- independently rebuilt registries can contain different definition sets
- replacing profile instances after gateway construction would leave the
  gateway reading stale state
- locally constructed skill definitions can diverge from the canonical skill
  registry
- production/bootstrap call sites have not yet been migrated to the verified
  runtime-state and combat-validation composition path

Recommended next atomic step:

- decide the application/bootstrap integration owner and migrate one explicit
  progression-aware engine creation path without changing the legacy V1 slice

## Player Progression Runtime State Ownership Foundation

`Game.Application.Runtime.PlayerProgressionRuntimeState` is the application-layer owner for
one loaded player's live V4 progression state.

It creates and retains these session/player-scoped mutable profiles:

- `UnlockProfile`
- `ClassProfile`
- `MasteryProfile`
- `MentorRelationshipProfile`
- `SkillTreeProgressProfile`

It receives and retains these application-content-scoped canonical definition
registries:

- `ClassDefinitionRegistry`
- `SkillDefinitionRegistry`
- `MentorDefinitionRegistry`

It also retains:

- `ClassSkillTreeBindingRegistry`

All registry constructor arguments are required. The runtime state returns the
same profile and registry references on every access. It does not clone,
rebuild, or replace them.

`ClassSkillTreeBindingRegistry` is currently a mutable restore target because
class-to-tree bindings are persisted in V2 progress snapshots. Bootstrap must
therefore scope that registry to the loaded session, or otherwise guarantee
that it is not shared across independently loaded players. The three
definition registries may be shared as application-content catalogs, but the
current binding registry must not be treated as an unrestricted process-wide
singleton.

Restore coordinators and progression services should mutate the profiles held
by `PlayerProgressionRuntimeState` directly. Future validation composition should receive
those same profile and registry references, ensuring combat validation reads
the state restored and changed during the loaded session.

The runtime state intentionally does not own or create:

- `TurnBasedCombatEngine`
- `CombatRuntimeContext`
- combat entities
- passive pipelines
- turn order
- resonance state

Those objects remain per-combat. Application/bootstrap migration and engine
call-site changes remain deferred.

The runtime-state ownership smoke coverage verifies:

- all required live profiles and binding state are exposed
- repeated access preserves profile identity
- injected registries remain canonical by reference
- `V4ProgressRestoreCoordinator` mutates the owned profile instances in place
- future validator composition observes later mutations to those live profiles
- the runtime state exposes no engine or combat-runtime creation path
- the legacy `FoundationVerticalSlice` remains unchanged through its existing
  V1 event-count guard

The dedicated runtime-state/combat-validation workflow further verifies that:

- `V4ProgressRestoreCoordinator` restores into the live profile instances
  owned by `PlayerProgressionRuntimeState`
- progression and class changes remain visible through those same instances
- `CombatSkillTurnValidationComposition` can create per-combat engines from
  the runtime state's live profiles and canonical registries
- a restored current-class skill and a mastered retained skill are allowed
- a non-mastered cross-class skill is blocked, consumes the turn, emits one
  `ActionBlocked`, and spends no Essence or Resonance
- blocked validation grants no skill experience and executes no skill effects
- missing player context fails closed
- enemy validation requires no player progression context
- legacy engine construction remains pass-through

Application/bootstrap integration remains intentionally deferred. The
workflow proves the dependency and lifetime path but does not migrate any
production engine call site.

## Runtime State And Combat Validation Final Audit

The runtime-state and combat-validation workflow is ready for a narrow
application-owned integration step. No additional combat, validation, restore,
or runtime-state foundation is required before that step.

The audit confirms:

- `PlayerProgressionRuntimeState` owns one stable set of mutable profiles for a loaded player
- restore preflights into temporary profiles, then mutates the owned live
  profiles only after all restore checks pass
- the validator gateway retains references to the supplied live profiles and
  reads later progression changes without cloning
- `CombatSkillTurnValidationComposition` creates per-combat validators and
  engines without moving the V4 dependency graph into
  `TurnBasedCombatEngine`
- missing player context fails closed, while enemy and explicit legacy paths
  preserve their documented behavior
- workflow coverage verifies restored current-class access, mastered retained
  access, cross-class blocking consequences, enemy behavior, and legacy
  pass-through

Remaining ownership risks are outside the validated combat path:

- the APIs cannot prevent a caller from creating a second `PlayerProgressionRuntimeState`
  for the same loaded player
- callers can still rebuild definition registries or provide registries that
  do not match the definitions used to create combat requests
- the mutable `ClassSkillTreeBindingRegistry` can leak restored bindings
  between players if bootstrap shares it beyond one loaded session
- replacing a runtime state after constructing a validator leaves that
  validator attached to the old profiles
- any V4 application call site can accidentally bypass validation by using
  the legacy two-argument engine constructor
- the legacy `PlayerClassCollection` and V4 `ClassProfile` remain separate
  models until application ownership chooses the authoritative path

The earliest safe next atomic item is one application-owned combat creation
method that:

- receives the single loaded `PlayerProgressionRuntimeState`
- creates a fresh passive pipeline and engine per combat
- calls `CreateProgressionAwareEngine` with the runtime state's existing profile and
  registry references
- prevents direct legacy engine construction in that V4 application path
- leaves `FoundationVerticalSlice` unchanged as the V1 compatibility path

V4 should not close immediately after that integration change. The integration
must be followed by a focused bootstrap workflow-verification iteration that
proves the application owner reuses one runtime state across restore,
progression, and multiple combat creations without registry duplication,
stale validators, or accidental legacy construction.

## Application-Owned Progression-Aware Combat Creation

`Game.Application.Integration.ProgressionAwareCombatEngineFactory` provides one
explicit application-layer method:

```csharp
TurnBasedCombatEngine CreateEngine(
    PlayerProgressionRuntimeState runtimeState,
    IRandomSource random,
    IReadOnlyList<ICombatPassiveEffect> passives);
```

The caller must supply the already-loaded `PlayerProgressionRuntimeState`. The factory rejects
a null runtime state and never creates, stores, clones, or replaces one. Live
profiles, canonical definition registries, and the session-scoped mutable
binding registry remain owned outside the combat factory.

For each call, the factory creates a fresh `CombatPassivePipeline` from the
provided per-combat passive list and delegates engine construction to
`CombatSkillTurnValidationComposition.CreateProgressionAwareEngine`. The existing
composition path receives the runtime state's current:

- `UnlockProfile`
- `ClassProfile`
- `MasteryProfile`
- `ClassDefinitionRegistry`
- `SkillDefinitionRegistry`

The resulting engine remains per-combat and observes later mutations to the
same live profile instances. Gateway, orchestrator, classification, and
validation policy logic are not duplicated.

This is not a full bootstrap migration. `FoundationVerticalSlice` and the
legacy two-argument `TurnBasedCombatEngine` constructor remain unchanged and
continue to provide the V1 pass-through path.

Application combat creation smoke coverage verifies:

- null runtime state is rejected
- the factory has no owned `PlayerProgressionRuntimeState` field
- the created engine uses the existing gateway-backed composition path
- current-class and mastered retained skills read the supplied live state
- non-mastered cross-class skills block with one `ActionBlocked`
- blocked actions consume the turn, spend no Essence or Resonance, grant no
  experience, and execute no skill effects
- the legacy foundation slice still emits exactly 38 events

## Bootstrap Combat Creation Workflow Verification

The dedicated application-owned combat creation workflow verifies:

- one loaded `PlayerProgressionRuntimeState` is reused across restore, progression, and three
  separate combat engine creations
- restore mutates the same live profiles and preserves canonical registry
  references
- a first factory-created combat allows a restored current-class skill
- mastery progression and a class switch mutate the same live profiles
- a later factory-created combat allows the newly mastered retained skill
- a non-mastered cross-class skill remains blocked in another later combat
- blocking emits one `ActionBlocked`, consumes the turn, spends no Essence or
  Resonance, grants no experience, and executes no skill effects
- each factory call creates a distinct engine and passive pipeline while
  retaining its supplied per-combat random source
- the legacy `FoundationVerticalSlice` remains unchanged at 38 events

No production or bootstrap call site is migrated by this verification. Full
application/bootstrap migration remains deferred.

The runtime-state, restore, combat-validation composition, application combat
creation, and repeated-combat workflow foundations are now ready for a final
V4 closure audit. That audit should decide whether the completed foundation can
close while recording full bootstrap migration as later application work.

## Final V4 Closure Audit

The V4 foundations are behaviorally complete, and the active
application-facing production APIs have been neutralized for V5 readiness.

Closure findings:

- runtime-state ownership is explicit and session-scoped
- restore preflights before mutating live profiles
- progression and restore mutate the same live profile instances later read by
  combat validation
- combat validation remains behind `ICombatSkillTurnValidator`
- `TurnBasedCombatEngine` receives V4 validation only through an optional
  dependency
- application-owned combat creation accepts an already-loaded runtime state
  and creates only per-combat engine dependencies
- repeated-combat workflow coverage verifies restore, progression, multiple
  engine creations, blocked-action semantics, fresh per-combat dependencies,
  and V1 compatibility
- save/restore remains schema-compatible through existing V2 snapshot types
- full production/bootstrap migration remains later application work, not a
  V4 foundation blocker

Remaining risks:

- callers can still bypass validation by directly using the legacy
  two-argument `TurnBasedCombatEngine` constructor outside the explicit V4
  application path
- callers can still create duplicate runtime-state or registry instances
  unless the future bootstrap owner centralizes that lifecycle
- stale validators remain possible if an application replaces runtime-state
  profile instances after creating engines
- the mutable `ClassSkillTreeBindingRegistry` must stay scoped to the loaded
  session or be guarded from cross-player sharing

Version-prefixed name classification at V4 closure:

- later neutralized in the active smoke suite:
  `RunV4SmokeTests`, V4/V5 assertion labels, and the V4 smoke test filename
- still safe to keep for test helper history:
  helper names such as `V4ValidatorCombatSkill`
- safe to keep because they represent existing serialized schema or snapshot
  compatibility:
  all `V2*` snapshot, repository, path-provider, serializer, service, and
  version types
- safe to defer until broader persistence naming changes:
  `V4ProgressRestoreCoordinator`, `V4ProgressRestoreResult`, and
  `V4ProgressRestoreFailureReason`, because they are restore adapters for the
  V4 class/mastery/mentor/skill-tree foundation over the existing V2 snapshot
  schema
- safe to defer as internal validation facts:
  `V4AccessChecked`, `V4BlockReason`, `V4ContextIncomplete`, and
  `V4SkillBlocked`, because they distinguish V3 unlock checks from class and
  mastery access checks in diagnostic results
- neutralized active application/composition APIs:
  `PlayerProgressionRuntimeState`, `ProgressionAwareCombatEngineFactory`, and
  `CreateProgressionAwareEngine`

Completed neutral renames:

- `V4RuntimeState` -> `PlayerProgressionRuntimeState`
- `V4ApplicationCombatFactory` -> `ProgressionAwareCombatEngineFactory`
- `CreateV4EnabledEngine` -> `CreateProgressionAwareEngine`

With these names neutralized, V4 is ready for a final close decision after
this branch's smoke/doc verification pass. The first V5 objective should then
be an application/bootstrap integration design slice that wires the neutral
runtime-state and combat-creation APIs into one real loaded-session path
without changing gameplay rules.

## V4 Foundation Closure Decision

V4 can close after this audit branch merges.

The final closure audit confirms:

- no active application or composition API still exposes stale V4 naming
- `PlayerProgressionRuntimeState`,
  `ProgressionAwareCombatEngineFactory`, and
  `CreateProgressionAwareEngine` are the neutral production-facing names
- deferred V4-prefixed restore/result and validation diagnostic facts remain
  intentional and non-blocking
- workflow coverage still verifies runtime-state ownership, restore mutation,
  combat validation composition, application-owned combat creation, repeated
  combat creation, blocked-action semantics, and legacy V1 compatibility
- full application/bootstrap migration remains outside V4 foundation scope

Remaining risks carried forward:

- direct legacy engine construction can bypass progression-aware validation
  outside the explicit application factory path
- bootstrap must centralize runtime-state and registry lifetimes to avoid
  duplicate instances or stale validators
- `ClassSkillTreeBindingRegistry` remains mutable during restore and must stay
  scoped to the loaded player/session or be protected from cross-player
  sharing

Deferred items carried into V5:

- full application/bootstrap integration
- broader naming policy for versioned persistence and restore adapters
- any future decision to rename V4 diagnostic facts after V3/V4 terminology is
  no longer useful for debugging

First V5 objective:

- design and verify one application/bootstrap loaded-session path that creates
  the player progression runtime state, restores/saves it, creates
  progression-aware combat engines through the application factory, and
  preserves the legacy V1 slice unchanged.

## V5 Loaded-Session Bootstrap Design Audit

The first V5 slice should add one application-owned loaded-session boundary,
not migrate the legacy slice directly.

Design findings:

- `FoundationVerticalSlice` remains the V1 compatibility path and should keep
  constructing `TurnBasedCombatEngine` through the legacy two-argument
  constructor
- the new loaded-session boundary should own exactly one
  `PlayerProgressionRuntimeState` for the loaded player/session
- canonical `ClassDefinitionRegistry`, `SkillDefinitionRegistry`, and
  `MentorDefinitionRegistry` should be built once by that boundary and passed
  into the runtime state
- `ClassSkillTreeBindingRegistry` should be created once for the loaded
  session because restore can mutate bindings from snapshots
- restore and progression services should mutate the runtime state's live
  profiles, not replace the runtime state or profile instances
- combat creation should call `ProgressionAwareCombatEngineFactory.CreateEngine`
  with that loaded runtime state and fresh per-combat random/passive
  dependencies
- `CombatRuntimeContext`, combat entities, resonance, turn order, and the
  combat engine remain per-combat

Ownership and lifetime decision:

- application/session owner:
  `PlayerProgressionRuntimeState`, mutable binding registry, and live V4
  progression profiles
- application-content owner:
  class, skill, and mentor definition registries
- per-combat owner:
  combat entities, runtime context, passive pipeline, random source, resonance,
  and `TurnBasedCombatEngine`
- legacy owner:
  `FoundationVerticalSlice` and legacy `PlayerProfile` remain unchanged until a
  future migration explicitly chooses how V2/V4 profile roles converge

Risks to guard in the implementation branch:

- creating a second runtime state for the same loaded player
- rebuilding registries per combat
- sharing mutable class-skill-tree bindings across loaded players
- replacing live profiles after creating a progression-aware engine
- accidentally using the legacy engine constructor in the new V5 loaded-session
  path
- diverging locally constructed combat `SkillDefinition` instances from the
  canonical `SkillDefinitionRegistry`

Earliest safe V5 implementation branch:

- add a small loaded-session application object or factory that builds canonical
  registries, creates one `PlayerProgressionRuntimeState`, exposes it for
  restore/progression, and provides a method for creating progression-aware
  combat engines through `ProgressionAwareCombatEngineFactory`
- include smoke coverage proving one loaded session reuses the same runtime
  state across restore/progression and combat creation while the legacy
  foundation slice still emits 38 events

## V5 Loaded-Session Foundation

`Game.Application.Runtime.LoadedPlayerSession` is the first V5
loaded-session boundary.

It is constructed with canonical registries:

- `ClassDefinitionRegistry`
- `SkillDefinitionRegistry`
- `MentorDefinitionRegistry`
- `ClassSkillTreeBindingRegistry`

The session creates exactly one `PlayerProgressionRuntimeState` from those
registries and exposes it through `RuntimeState` for restore and progression.
Repeated access returns the same runtime-state instance. The class-skill-tree
binding registry is supplied to that runtime state once and remains scoped to
the loaded session.

Combat creation is explicit:

```csharp
TurnBasedCombatEngine CreateCombatEngine(
    IRandomSource random,
    IReadOnlyList<ICombatPassiveEffect> passives);
```

Each call delegates to `ProgressionAwareCombatEngineFactory` and creates a
fresh combat engine with fresh per-combat passive pipeline/random inputs.
Combat entities, `CombatRuntimeContext`, resonance, turn order, and engine
instances remain outside the loaded session.

This is not a full bootstrap migration. `FoundationVerticalSlice` remains the
legacy V1 pass-through path and still constructs combat through the old
two-argument `TurnBasedCombatEngine` constructor.

Loaded-session smoke coverage verifies:

- one session owns one stable `PlayerProgressionRuntimeState`
- repeated access returns the same runtime state
- restore and progression mutate that same runtime state
- canonical registries are passed by reference and not rebuilt per combat
- repeated combat engine creation uses the same runtime state
- each combat engine receives fresh per-combat dependencies
- current-class and mastered retained player skills pass
- non-mastered cross-class skills block with one `ActionBlocked`
- blocked actions consume the turn, spend no Essence or Resonance, grant no
  experience, and execute no skill effects
- `FoundationVerticalSlice` remains 38-event compatible

Dedicated loaded-session workflow verification now proves:

- one `LoadedPlayerSession` reuses the same `RuntimeState` across restore,
  progression, and repeated combat creation
- restore mutates the runtime state's live profiles
- mastery progression before a later combat is observed by later validation
- multiple `CreateCombatEngine` calls produce fresh per-combat engines,
  passive pipelines, and random sources
- canonical class and skill registries stay stable by reference across the
  workflow
- current-class skills and mastered retained skills pass
- non-mastered cross-class skills block with one `ActionBlocked`
- blocked actions consume the turn, spend no Essence or Resonance, grant no
  experience, and execute no skill effects
- `FoundationVerticalSlice` remains 38-event compatible

The loaded-session foundation is ready for an audit/design pass before any
production bootstrap migration. Full bootstrap migration remains deferred.

## V5 Loaded-Session Foundation Audit

The loaded-session foundation is complete enough to proceed to full bootstrap
migration design.

Audit findings:

- `LoadedPlayerSession` owns exactly one `PlayerProgressionRuntimeState`
  and exposes it for restore/progression
- canonical class, skill, and mentor registries are supplied once to the
  session runtime state
- mutable class-skill-tree bindings are scoped to the loaded session through
  the supplied `ClassSkillTreeBindingRegistry`
- combat engines, passive pipelines, random sources, runtime contexts,
  entities, turn order, and resonance remain per-combat
- `ProgressionAwareCombatEngineFactory` usage is clean and keeps progression
  dependency graphs out of `TurnBasedCombatEngine`
- repeated workflow coverage verifies live profile mutation, registry identity,
  fresh per-combat dependencies, blocked-action semantics, and V1 compatibility

Risks carried into bootstrap design:

- a future bootstrap owner can still create duplicate loaded sessions or
  runtime states unless ownership is centralized
- replacing runtime-state profile instances after engine creation could leave
  stale validators
- direct legacy engine construction can still bypass progression-aware
  validation outside the loaded-session path
- locally constructed combat skill definitions can diverge from the canonical
  `SkillDefinitionRegistry`

Test and documentation naming:

- progression foundation coverage lives under
  `PlayerProgressionFoundationSmokeTests`
- runtime/session/bootstrap/demo coverage now lives under
  `ProgressionRuntimeBootstrapSmokeTests`
- active smoke runner labels now use responsibility-based progression naming
- this document now uses the neutral `PlayerProgression_Foundation.md` filename
- a future split may separate combat validation and runtime/bootstrap history if
  the document becomes too broad

Earliest safe next atomic item:

- add a bootstrap migration design audit for one loaded-session path that
  creates or receives canonical registries, owns one `LoadedPlayerSession`,
  restores/progresses through its runtime state, and creates combat through
  `LoadedPlayerSession.CreateCombatEngine` while leaving
  `FoundationVerticalSlice` unchanged.

## V5 Bootstrap Migration Design Audit

The smallest safe bootstrap boundary is one loaded player/session owner that
creates or receives canonical content registries, creates exactly one
`LoadedPlayerSession`, exposes `LoadedPlayerSession.RuntimeState` to restore and
progression flows, and routes progression-aware combat creation through
`LoadedPlayerSession.CreateCombatEngine`.

Ownership and lifetime decisions:

- bootstrap should own the session lifetime and either receive already-built
  application-content registries or build them once before creating the session
- class, skill, mentor, and skill-tree definition registries are
  application-content scoped and must not be rebuilt per combat
- `ClassSkillTreeBindingRegistry` is mutable player/session state and should be
  supplied to exactly one `LoadedPlayerSession` for the loaded player
- `LoadedPlayerSession` owns exactly one `PlayerProgressionRuntimeState` for the
  loaded session
- restore and progression should mutate the live profiles exposed by
  `LoadedPlayerSession.RuntimeState`; they should not replace profile instances
- combat creation should call `LoadedPlayerSession.CreateCombatEngine`, passing
  fresh per-combat random sources and passive collections
- `FoundationVerticalSlice` remains the legacy/pass-through compatibility path
  and should not be migrated as part of the first bootstrap implementation

Design risks to address in implementation:

- accidental legacy engine construction in the V5 path can bypass progression
  validation
- creating more than one `LoadedPlayerSession` or
  `PlayerProgressionRuntimeState` for a loaded player can split restore,
  progression, and validation state
- replacing runtime-state profile instances after a validator has been created
  can leave stale validation dependencies
- sharing one mutable `ClassSkillTreeBindingRegistry` across players can leak
  binding state between sessions
- constructing local combat skill definitions outside the canonical
  `SkillDefinitionRegistry` can diverge from validation facts

Recommended first implementation branch:

- add one explicit bootstrap/session composition entry point that creates or
  receives canonical registries, creates one `LoadedPlayerSession`, restores
  through `LoadedPlayerSession.RuntimeState`, and exposes combat creation only
  through `LoadedPlayerSession.CreateCombatEngine`
- keep `FoundationVerticalSlice` and existing legacy constructors unchanged
- follow immediately with workflow verification before broad call-site migration

## V5 Bootstrap Session Composition Foundation

`Game.Application.Runtime.PlayerSessionComposition` is the explicit
application-layer composition entry point for one loaded player/session.

It receives canonical registries:

- `ClassDefinitionRegistry`
- `SkillDefinitionRegistry`
- `MentorDefinitionRegistry`

It also receives, or creates for an empty session, one
`ClassSkillTreeBindingRegistry`. This binding registry is mutable
player/session state and must not be shared across loaded players.

The composition creates exactly one `LoadedPlayerSession` during construction
and exposes:

```csharp
LoadedPlayerSession LoadedSession
PlayerProgressionRuntimeState RuntimeState
TurnBasedCombatEngine CreateCombatEngine(
    IRandomSource random,
    IReadOnlyList<ICombatPassiveEffect> passives)
```

`RuntimeState` is the same instance owned by `LoadedSession`. Restore and
progression flows should mutate that live runtime state. Combat creation is
available only through `CreateCombatEngine`, which delegates to
`LoadedPlayerSession.CreateCombatEngine` and keeps per-combat dependencies
fresh.

Lifetime boundaries:

- `PlayerSessionComposition` is one loaded-session owner
- `LoadedPlayerSession` and `PlayerProgressionRuntimeState` are created once per
  composition
- class, skill, and mentor registries are canonical references supplied to the
  composition and reused across combat creation
- `ClassSkillTreeBindingRegistry` is session-scoped mutable state
- combat engines, passive pipelines, random sources, combat entities, runtime
  contexts, and event logs remain per-combat

This is not full bootstrap migration. `FoundationVerticalSlice` and legacy
engine construction remain unchanged/pass-through. Dedicated workflow
verification should follow before any broad call-site migration.

## V5 Player Session Composition Workflow Verification

Dedicated workflow verification now proves the `PlayerSessionComposition` path
before full bootstrap migration.

The workflow uses one `PlayerSessionComposition` and verifies:

- one stable `LoadedPlayerSession`
- one stable `PlayerProgressionRuntimeState`
- restore mutates the same live runtime-state profiles exposed by the
  composition
- progression and mastery mutation affect later combat validation
- repeated `CreateCombatEngine` calls use the same session state
- combat engines, passive pipelines, random sources, combat runtimes, and event
  logs remain per-combat
- canonical class and skill registries remain stable by reference
- mutable class-skill-tree bindings are session-scoped and not global
- restored current-class skills pass validation
- mastered retained skills pass after progression mutation
- non-mastered cross-class skills block with one `ActionBlocked`
- blocked actions consume the turn, spend no Essence or Resonance, grant no
  experience, and execute no skill effects
- `FoundationVerticalSlice` remains V1-compatible at 38 events

Full bootstrap migration remains deferred. The player-session composition path
is ready for a focused audit before any broader call-site migration.

## V5 Player Session Composition Audit

The verified `PlayerSessionComposition` path is ready for a narrow bootstrap
migration design/implementation step.

Audit findings:

- `PlayerSessionComposition` is right-sized as an application/session
  composition boundary
- it creates exactly one `LoadedPlayerSession` and exposes that same session by
  reference
- `RuntimeState` is the same `PlayerProgressionRuntimeState` owned by
  `LoadedPlayerSession`
- restore and progression mutate the live runtime-state profile instances
- canonical class, skill, and mentor registries are supplied once and reused by
  validation
- `ClassSkillTreeBindingRegistry` remains session-scoped mutable state
- repeated combat creation flows through
  `PlayerSessionComposition.CreateCombatEngine` and then
  `LoadedPlayerSession.CreateCombatEngine`
- combat engines, passive pipelines, random sources, combat runtimes, entities,
  and event logs remain per-combat
- `FoundationVerticalSlice` remains unchanged as the legacy/pass-through path

Risks carried into the next migration step:

- a future bootstrap path can still accidentally call the legacy
  `TurnBasedCombatEngine` constructor and bypass progression-aware validation
- creating more than one `PlayerSessionComposition` for the same loaded player
  can split restore, progression, and combat validation state
- replacing runtime-state profile instances instead of mutating them can leave
  validators stale
- sharing a mutable `ClassSkillTreeBindingRegistry` across loaded players can
  leak binding state between sessions
- locally constructed skill definitions can diverge from the canonical
  `SkillDefinitionRegistry`

Earliest safe next atomic item:

- add one narrow bootstrap/session entry point that creates or receives
  canonical registries, creates one `PlayerSessionComposition`, restores
  through `PlayerSessionComposition.RuntimeState`, and exposes combat creation
  through `PlayerSessionComposition.CreateCombatEngine` while leaving
  `FoundationVerticalSlice` unchanged.

## V5 Narrow Bootstrap Session Entry Point

`Game.Application.Bootstrap.PlayerSessionBootstrap` is the first narrow
bootstrap/session entry point for one loaded player/session.

It receives canonical registries:

- `ClassDefinitionRegistry`
- `SkillDefinitionRegistry`
- `MentorDefinitionRegistry`

It also receives, or creates for an empty session, one session-scoped
`ClassSkillTreeBindingRegistry`.

The bootstrap creates exactly one `PlayerSessionComposition` and exposes:

```csharp
PlayerSessionComposition Composition
LoadedPlayerSession LoadedSession
PlayerProgressionRuntimeState RuntimeState
TurnBasedCombatEngine CreateCombatEngine(
    IRandomSource random,
    IReadOnlyList<ICombatPassiveEffect> passives)
```

`RuntimeState` is the same live state owned by the composition's
`LoadedPlayerSession`. Restore and progression flows should mutate that state.
Combat creation delegates to `PlayerSessionComposition.CreateCombatEngine`.

Ownership boundaries:

- `PlayerSessionBootstrap` is one loaded-session bootstrap owner
- canonical definition registries are supplied once and reused
- class-skill-tree bindings remain mutable session state
- `PlayerSessionComposition`, `LoadedPlayerSession`, and
  `PlayerProgressionRuntimeState` are created once per bootstrap
- combat engines, passive pipelines, random sources, combat entities, combat
  runtimes, and event logs remain per-combat

This is controlled migration, not full bootstrap replacement.
`FoundationVerticalSlice` and existing legacy engine construction remain
unchanged/pass-through. Dedicated workflow verification should follow before
broader call-site migration.

## V5 Player Session Bootstrap Workflow Verification

Dedicated workflow verification now proves the `PlayerSessionBootstrap` path
before broader bootstrap migration.

The workflow uses one `PlayerSessionBootstrap` and verifies:

- one stable `PlayerSessionComposition`
- one stable `LoadedPlayerSession`
- one stable `PlayerProgressionRuntimeState`
- restore mutates the same live runtime-state profiles exposed by the bootstrap
- progression and mastery mutation affect later combat validation
- repeated `CreateCombatEngine` calls use the same session state
- combat engines, passive pipelines, random sources, combat runtimes, and event
  logs remain per-combat
- canonical class and skill registries remain stable by reference
- mutable class-skill-tree bindings are session-scoped and not global
- separate bootstrap instances do not share mutable binding registries
- restored current-class skills pass validation
- mastered retained skills pass after progression mutation
- non-mastered cross-class skills block with one `ActionBlocked`
- blocked actions consume the turn, spend no Essence or Resonance, grant no
  experience, and execute no skill effects
- `FoundationVerticalSlice` remains V1-compatible at 38 events

Full bootstrap migration remains deferred. The player-session bootstrap path is
ready for a focused audit before any broader call-site migration.

## V5 Player Session Bootstrap Audit

The verified `PlayerSessionBootstrap` path is ready for the first narrow real
bootstrap migration step.

Audit findings:

- `PlayerSessionBootstrap` is right-sized as a thin loaded-session bootstrap
  owner
- it owns exactly one `PlayerSessionComposition`
- `Composition`, `LoadedSession`, and `RuntimeState` remain stable by reference
- canonical class, skill, and mentor registries are supplied once through the
  composition path
- `ClassSkillTreeBindingRegistry` remains session-scoped mutable state
- separate bootstrap instances do not share mutable binding registries
- restore and progression mutate the live runtime-state profile instances
- combat creation flows through `PlayerSessionBootstrap.CreateCombatEngine`,
  then `PlayerSessionComposition.CreateCombatEngine`
- combat engines, passive pipelines, random sources, combat runtimes, entities,
  and event logs remain per-combat
- `FoundationVerticalSlice` remains unchanged as the legacy/pass-through path

Risks carried into the next migration step:

- a future runtime path can still bypass progression-aware validation by
  directly constructing `TurnBasedCombatEngine`
- creating more than one `PlayerSessionBootstrap` for the same loaded player can
  split restore, progression, and combat validation state
- replacing runtime-state profile instances instead of mutating them can stale
  validators created before replacement
- sharing mutable binding registries across loaded players can leak class/tree
  binding state
- local combat skill definitions can diverge from the canonical
  `SkillDefinitionRegistry`

Earliest safe next atomic item:

- add one explicit V5 runtime/demo bootstrap flow that creates or receives
  canonical registries, creates one `PlayerSessionBootstrap`, restores through
  `PlayerSessionBootstrap.RuntimeState`, and creates combat only through
  `PlayerSessionBootstrap.CreateCombatEngine`, while leaving
  `FoundationVerticalSlice` unchanged.

## Progression Runtime Demo Bootstrap Flow

`Game.Application.Bootstrap.ProgressionRuntimeDemoBootstrap` is the first explicit
runtime/demo bootstrap flow. It is a narrow path, not a full bootstrap
migration.

It receives canonical registries:

- `ClassDefinitionRegistry`
- `SkillDefinitionRegistry`
- `MentorDefinitionRegistry`

It also receives, or creates for an empty demo session, one session-scoped
`ClassSkillTreeBindingRegistry`.

The flow creates exactly one `PlayerSessionBootstrap` and exposes:

```csharp
PlayerSessionBootstrap SessionBootstrap
PlayerSessionComposition Composition
LoadedPlayerSession LoadedSession
PlayerProgressionRuntimeState RuntimeState
TurnBasedCombatEngine CreateCombatEngine(
    IRandomSource random,
    IReadOnlyList<ICombatPassiveEffect> passives)
```

Restore and progression should use `RuntimeState`, which is the same live state
owned by `SessionBootstrap`. Combat creation delegates to
`PlayerSessionBootstrap.CreateCombatEngine`; the demo flow does not own or
directly construct `TurnBasedCombatEngine`.

Ownership boundaries:

- one `ProgressionRuntimeDemoBootstrap` owns one `PlayerSessionBootstrap`
- canonical definition registries are supplied once and reused
- class-skill-tree bindings remain mutable session state
- `PlayerSessionBootstrap`, `PlayerSessionComposition`,
  `LoadedPlayerSession`, and `PlayerProgressionRuntimeState` are created once
  per demo flow
- combat engines, passive pipelines, random sources, combat entities, combat
  runtimes, and event logs remain per-combat

`FoundationVerticalSlice` remains the V1 legacy/pass-through path. Dedicated
workflow verification should follow before broader bootstrap migration.

## Progression Runtime Demo Bootstrap Workflow Verification

Dedicated workflow verification now proves the `ProgressionRuntimeDemoBootstrap` path
before broader bootstrap migration.

The workflow uses one `ProgressionRuntimeDemoBootstrap` and verifies:

- one stable `PlayerSessionBootstrap`
- one stable `PlayerSessionComposition`
- one stable `LoadedPlayerSession`
- one stable `PlayerProgressionRuntimeState`
- restore mutates the same live runtime-state profiles exposed by the demo
  bootstrap
- progression and mastery mutation affect later combat validation
- repeated `CreateCombatEngine` calls use the same session state
- combat engines, passive pipelines, random sources, combat runtimes, and event
  logs remain per-combat
- canonical class and skill registries remain stable by reference
- mutable class-skill-tree bindings are session-scoped and not global
- separate demo bootstrap instances do not share mutable binding registries
- the demo path does not own or expose direct `TurnBasedCombatEngine`
  construction
- restored current-class skills pass validation
- mastered retained skills pass after progression mutation
- non-mastered cross-class skills block with one `ActionBlocked`
- blocked actions consume the turn, spend no Essence or Resonance, grant no
  experience, and execute no skill effects
- `FoundationVerticalSlice` remains V1-compatible at 38 events

Broader bootstrap migration remains deferred. The progression runtime demo
bootstrap path is ready for a focused audit before any broader call-site
migration.

## Progression Runtime Demo Bootstrap Audit

The verified `ProgressionRuntimeDemoBootstrap` path is ready for visual smoke
demo design. It is also complete enough to close the bootstrap foundation after
this audit, with broader bootstrap migration carried forward as later
application work.

Audit findings:

- `ProgressionRuntimeDemoBootstrap` is thin enough to remain a demo/runtime path
- it owns exactly one `PlayerSessionBootstrap` and does not duplicate
  `PlayerSessionBootstrap` internals
- `SessionBootstrap`, `Composition`, `LoadedSession`, and `RuntimeState` remain
  stable by reference
- canonical class, skill, and mentor registries are supplied once and reused
- `ClassSkillTreeBindingRegistry` remains session-scoped mutable state
- separate demo bootstrap instances do not share mutable binding registries
- restore and progression mutate the live runtime-state profile instances
- combat creation flows through `ProgressionRuntimeDemoBootstrap.CreateCombatEngine`,
  then `PlayerSessionBootstrap.CreateCombatEngine`
- the demo path does not own or expose direct `TurnBasedCombatEngine`
  construction
- combat engines, passive pipelines, random sources, combat runtimes, entities,
  and event logs remain per-combat
- `FoundationVerticalSlice` remains unchanged as the legacy/pass-through path

Risks carried forward:

- future visual or runtime demo code can still bypass progression-aware combat
  by constructing `TurnBasedCombatEngine` directly
- creating multiple demo bootstraps for one loaded player can split restore,
  progression, and combat validation state
- replacing runtime-state profile instances instead of mutating them can stale
  validators created before replacement
- sharing mutable binding registries across loaded players can leak class/tree
  binding state
- locally constructed combat skill definitions can diverge from the canonical
  `SkillDefinitionRegistry`

Earliest safe next atomic item:

- add a visual smoke demo design audit that uses `ProgressionRuntimeDemoBootstrap` as the
  backend entry point, keeps `FoundationVerticalSlice` legacy, and defines the
  minimum non-Unity or Unity-adapter-facing presentation contract without
  implementing visual demo behavior yet.

Validation rules:

- the request, skill, progress, and progress definition must exist
- `ICombatSkill.Id` must match `SkillProgress.Definition.Id`
- player skills with a required unlock are checked through
  `SkillAccessPolicy`
- V4 access is checked only when a complete class/mastery registry context is
  supplied
- partial V4 context is rejected explicitly
- enemy mode skips player unlock and class/mastery checks
- enemy requests still require valid skill/progress facts and matching ids

`CombatSkillUseValidationResult` returns stable facts including:

- `IsAllowed`
- `SkillId`
- `IsPlayerAction`
- `V3AccessChecked`
- `V4AccessChecked`
- `CombatSkillUseBlockReason`
- underlying V3 and V4 block reasons when applicable

The validator is read-only. It does not emit events, spend resources, advance
turns, apply skills, grant experience, or modify combat state.
`TurnBasedCombatEngine` consumes validation only through
`ICombatSkillTurnValidator`.

The previous parameter-based validator overload remains compatible. New engine
integration should use the context-based overload so legacy, player, and enemy
behavior cannot be inferred from null parameters.

## Unified Progress Restore Coordinator

`V4ProgressRestoreCoordinator` restores V4 state in this defined order:

1. `ClassProfile`
2. `MasteryProfile`
3. `MentorRelationshipProfile`
4. `SkillTreeProgressProfile`
5. class skill tree bindings

Before mutating target state, the coordinator runs the same individual restore
operations against temporary `ClassProfile`, `MasteryProfile`,
`MentorRelationshipProfile`, and `SkillTreeProgressProfile` instances. Binding
restore feasibility is checked against a temporary
`ClassSkillTreeBindingRegistry`.

Preflight rules:

- class, mastery, mentor, skill tree progress, and binding feasibility are all
  evaluated
- any failed subsystem returns before target mutation begins
- successful preflight applies the existing individual restore operations in
  the documented order
- per-subsystem success facts, failure reasons, and messages remain available
- individual restore methods remain unchanged and independently callable
- snapshot DTOs, JSON shape, and schema version remain unchanged

The binding-aware coordinator overload receives:

- `ClassDefinitionRegistry`
- `SkillDefinitionRegistry`
- `MentorDefinitionRegistry`
- `SkillTreeDefinition`
- `ClassSkillTreeBindingRegistry` containing registered skill tree definitions

Existing coordinator calls remain compatible. The original overload restores
the original four subsystems and treats binding restore as not required. The
binding-aware overload restores bindings through the existing
`V2ProgressSnapshotService` behavior.

`V4ProgressRestoreResult` exposes per-subsystem facts, including:

- `ClassRestored`
- `MasteryRestored`
- `MentorRelationshipsRestored`
- `SkillTreeProgressRestored`
- `ClassSkillTreeBindingsRestored`
- `FullyRestored`

Binding restore also exposes structured failure details:

- `ClassSkillTreeBindingsFailureReason`
- `ClassSkillTreeBindingsFailureMessage`

All subsystems expose stable `V4ProgressRestoreFailureReason` values and
human-readable messages. Any failed subsystem, including binding restore,
causes `FullyRestored` to be false.

## Integrated Workflow Verification

Player progression smoke tests include integrated workflow coverage for:

- class + mentor + teaching + mastery
- class/mastery snapshot and restore
- mastered skill retention after class switch
- combat-facing skill access after mentor teaching and class switch
- combat skill use validation across teaching and class switching
- combat skill use validation context modes across class switching
- combat skill use validation context provider across class switching
- combat actor classification through context creation and validation
- combat skill validation orchestration across class switching and failures
- runtime-state restore/progression through combat validation composition
- application-owned repeated combat creation from one loaded runtime state
- loaded-session restore/progression through repeated session-created combats
- skill tree progression
- class-specific skill tree binding
- class skill tree binding snapshot, JSON round-trip, and restore
- scoped skill tree progress across class switching and JSON restore

The mastered skill retention workflow verifies:

- two skills are registered
- two classes with different allowed skills are registered
- the first class is assigned
- a mentor teaches a first-class skill
- teaching is applied until the skill is mastered
- the profile switches to a second class
- the mastered first-class skill remains usable through
  `MasteredSkillRetentionPolicy`
- a non-mastered first-class skill is not usable after the class switch

The combat-facing skill access workflow verifies:

- two skills and two classes with different allowed skills are registered
- the first class is assigned and its skill is combat-usable
- a mentor teaches that skill to mastery
- the profile switches to the second class
- the mastered first-class skill remains combat-usable
- the non-mastered first-class skill and an unknown skill are blocked
- `CombatSkillAccessService` facts match `MasteredSkillRetentionPolicy`
- no combat execution behavior is changed

The combat skill use validator workflow verifies:

- two skills and two classes are registered
- the first class is assigned
- a first-class skill passes composed request validation
- mentor teaching masters that skill
- the profile switches to the second class
- the mastered retained skill still passes validation
- a non-mastered cross-class skill is blocked
- enemy mode accepts a structurally valid skill without V4 profiles
- mismatched skill and progress ids are blocked
- `TurnBasedCombatEngine` is not invoked or changed

The combat skill use validation context workflow verifies:

- `LegacyBypass` accepts a structurally valid skill without V4 profiles
- `Enemy` accepts a structurally valid skill without player profiles
- `Player` rejects partial context
- `Player` accepts a current-class skill
- `Player` accepts a mastered retained skill after a class switch
- `Player` rejects a non-mastered cross-class skill
- `Player` rejects mismatched skill and progress ids
- `TurnBasedCombatEngine` is not invoked or changed

The combat skill use validation context provider workflow verifies:

- the provider creates explicit legacy bypass and enemy contexts
- both non-player-progression contexts validate structurally valid skills
- incomplete player dependencies are refused without fallback
- complete player dependencies produce a valid player context
- the player context validates a current-class skill
- the same player context observes mastered retention after a class switch
- the same player context blocks a non-mastered cross-class skill
- `TurnBasedCombatEngine` is not invoked or changed

The combat actor classification workflow verifies:

- explicit legacy classification maps to a legacy context accepted by the
  validator
- the runtime enemy maps to an enemy context without player profiles
- a complete runtime player classifies as `Player`
- incomplete player dependencies fail classification safely
- an actor outside the runtime fails classification safely
- the classified player mode requests the matching context from the provider
- the produced player context is accepted by `CombatSkillUseValidator`
- `TurnBasedCombatEngine` is not invoked or changed

The combat skill validation orchestrator workflow verifies:

- legacy and enemy actors complete orchestration successfully
- a player current-class skill completes orchestration successfully
- a mastered retained skill remains valid after a class switch
- a non-mastered cross-class skill fails at the `Validation` stage
- incomplete player dependencies fail at the `Classification` stage
- a foreign actor fails safely at the `Classification` stage
- mismatched skill and progress ids fail at the `Validation` stage
- successful orchestration reports no failure stage
- `TurnBasedCombatEngine` is not invoked or changed

The combat skill turn validator gateway workflow verifies:

- legacy mode allows a structurally valid skill
- enemy mode allows a structurally valid skill without player dependencies
- player mode allows a current-class skill
- player mode allows a mastered retained skill
- player mode blocks a non-mastered cross-class skill
- missing player dependencies return clear classification failure facts
- the engine-facing interface accepts only the turn request and runtime

The combat skill turn validator engine integration verifies:

- the existing two-argument constructor preserves legacy execution
- an explicit null validator produces the same event types
- allowed validation preserves skill execution, costs, and experience
- blocked validation emits exactly one `ActionBlocked`
- blocked validation consumes the turn
- blocked validation spends neither Essence nor Resonance
- blocked validation grants no experience and executes no skill effects
- blocked validation does not emit `ResourceNotEnough`
- validation runs before and preempts `SkillSeal`
- a null custom validator result fails closed with `NullValidationResult`
- null-result blocking preserves turn, resource, experience, and effect rules
- null-result blocking preempts `SkillSeal`

The combat skill turn validation composition verifies:

- the factory creates a non-null `ICombatSkillTurnValidator`
- the explicit V4 engine path allows current-class player skills
- legacy engine construction remains pass-through
- missing player dependencies fail closed
- enemy actions require no player progression dependencies
- mastered retained skills remain allowed
- non-mastered cross-class skills are blocked
- blocked composed validation emits one `ActionBlocked` and consumes the turn
- blocked composed validation spends no Essence or Resonance
- blocked composed validation grants no experience or skill effects

The skill tree workflow verifies:

- skills are registered
- a three-node skill tree is created
- the first node is discovered
- the first node is unlocked
- a dependent node is blocked before its prerequisite unlock
- the prerequisite node unlocks
- the dependent node becomes unlockable
- the dependent node unlocks
- no skills are automatically granted
- `MasteryProfile` remains unchanged

The class skill tree binding workflow verifies:

- two classes are registered
- two stable skill trees are registered
- each class binds to a different skill tree
- the first class is assigned
- the first class skill tree resolves
- the profile switches to the second class
- the second class skill tree resolves
- bindings do not mutate `SkillTreeProgressProfile`
- bindings do not grant skills automatically

The binding save workflow additionally verifies:

- both bindings are captured in the progress snapshot
- bindings survive JSON serialization and deserialization

The scoped skill tree progress workflow verifies:

- two classes bind to different trees containing the same node id
- unlocking the shared node in the first tree does not unlock it in the second
- the shared node can be discovered and unlocked independently after switching
  classes
- both scopes survive snapshot creation and JSON round-trip
- restore preserves shared unlocks and tree-specific discovery facts
- bindings restore into a fresh registry
- restored bindings resolve after assigning and switching classes
- restored bindings do not mutate skill tree progress
- restored bindings do not grant skills automatically

## Boundaries

Allowed:

- pure C# domain models
- registry-backed validation
- smoke-testable facts and services
- snapshot DTOs and restore helpers
- explicit service calls for state mutation

Not included:

- application/bootstrap integration
- changes to skill effect behavior
- automatic skill unlock grants
- mentor rewards
- mentor missions
- dialogue
- UI
- Unity
- generator-wide rewrites

## Smoke Expectations

Current smoke groups:

- V1 Legacy Slice assertions: `2`
- V1 emitted event count: `38`
- Shared Domain Foundations assertions: `16`
- V2 Narrative / Progression / Persistence assertions: `264`
- V3 Combat / Skills / Resonance / Status / Stats assertions: `100`
- Player Progression / Combat Validation assertions: `517`
- Total smoke assertions executed: `899`

## Validation

Run:

```powershell
dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal
dotnet run --project src\Game.SmokeTests\Game.SmokeTests.csproj --no-build
git diff --check
```
``````

</details>

<a id="archive-06"></a>
## Archive 06: Foundation_Version_Prefix_Naming_Audit.md

Historical source path: `docs/Foundation_Version_Prefix_Naming_Audit.md`. Original blob: `c231625cc1852edd2129cb633352a98d49f7b80e`.

<details>
<summary>Show complete historical Markdown source (06)</summary>

``````markdown
# Foundation Version Prefix Naming Audit

Branch: `codex/foundation-version-prefix-naming-audit`

## Purpose

This audit classifies active `V1` / `V2` / `V3` / `V4` / `V5`-prefixed names before visual work begins. The target rule is:

- active production APIs, test suites, assertion groups, and living architecture docs should use responsibility-based names
- version prefixes may remain in branch names, migration notes, audit history, schema compatibility notes, and explicitly historical references
- this branch records the rename plan only; it does not perform renames

## Must Rename

These names are active production or production-facing API names. They should be renamed through small dedicated branches.

| Current name | Location | Recommended name | Notes |
| --- | --- | --- | --- |
| `V5RuntimeDemoBootstrap` | `src/Game/Application/Bootstrap/V5RuntimeDemoBootstrap.cs` | `ProgressionRuntimeDemoBootstrap` | Active demo/runtime path and the earliest visual-work blocker. |
| `V4ProgressRestoreCoordinator` | `src/Game/Domain/Progress/V4ProgressRestoreCoordinator.cs` | `ProgressRestoreCoordinator` or `PlayerProgressionRestoreCoordinator` | Active restore/progression coordinator. |
| `V4ProgressRestoreResult` | `src/Game/Domain/Progress/V4ProgressRestoreResult.cs` | `ProgressRestoreResult` or `PlayerProgressionRestoreResult` | Active result type. |
| `V4ProgressRestoreFailureReason` | `src/Game/Domain/Progress/V4ProgressRestoreFailureReason.cs` | `ProgressRestoreFailureReason` or `PlayerProgressionRestoreFailureReason` | Active failure type. |
| `IV2SavePathProvider` | `src/Game/Application/Persistence/IV2SavePathProvider.cs` | `ISavePathProvider` or `IProgressSavePathProvider` | Active application persistence API. |
| `UnityV2SavePathProvider` | `src/Game.Unity/Persistence/UnityV2SavePathProvider.cs` | `UnitySavePathProvider` or `UnityProgressSavePathProvider` | Active Unity adapter. |
| `UnityV2SaveService` | `src/Game.Unity/Persistence/UnityV2SaveService.cs` | `UnitySaveService` or `UnityProgressSaveService` | Active Unity adapter. |
| `V2ProgressSnapshotService` | `src/Game/Domain/Progress/V2ProgressSnapshotService.cs` | `ProgressSnapshotService` | Active save/restore service. |
| `V2ProgressSnapshotJsonSerializer` | `src/Game/Domain/Progress/V2ProgressSnapshotJsonSerializer.cs` | `ProgressSnapshotJsonSerializer` | Active serializer. |
| `V2ProgressSnapshotFileRepository` | `src/Game/Domain/Progress/V2ProgressSnapshotFileRepository.cs` | `ProgressSnapshotFileRepository` | Active repository. |
| `V2ProgressSnapshotValidationService` | `src/Game/Domain/Progress/V2ProgressSnapshotValidationService.cs` | `ProgressSnapshotValidationService` | Active validation service. |
| `V2ProgressSnapshotVersions` | `src/Game/Domain/Progress/V2ProgressSnapshotVersions.cs` | `ProgressSnapshotSchemaVersions` | Keep the internal schema version value, but neutralize the type name. |
| `V2*Snapshot` DTO types | `src/Game/Domain/Progress/V2*Snapshot.cs` | remove `V2` prefix, for example `ProgressSnapshot`, `ClassProfileSnapshot`, `MasterySnapshot` | High blast radius because these are persistence contracts and test fixtures. Rename in a dedicated persistence branch. |

## Should Rename

These names are active test organization, assertion grouping, or living documentation names. They do not change runtime behavior, but they keep versioned framing alive in the current workflow.

| Current name | Location | Recommended name | Notes |
| --- | --- | --- | --- |
| `V4SmokeTests` | `src/Game.SmokeTests/V4SmokeTests.cs` | `ProgressionFoundationSmokeTests` or `FoundationProgressionSmokeTests` | Current V4/V5 progression foundation suite. |
| `RunV4SmokeTests` | `src/Game.SmokeTests/V4SmokeTests.cs`, `SmokeTestRunner.cs` | `RunProgressionFoundationSmokeTests` | Preserve assertion count reporting while renaming the group. |
| `V1SmokeTests` | `src/Game.SmokeTests/V1SmokeTests.cs` | `LegacySliceSmokeTests` | Active legacy guardrail, but the file/class can be neutral. |
| `V2SmokeTests` | `src/Game.SmokeTests/V2SmokeTests.cs` | `NarrativePersistenceSmokeTests` | Active save/narrative/progression guardrail. |
| `V3SmokeTests` | `src/Game.SmokeTests/V3SmokeTests.cs` | `CombatFoundationSmokeTests` | Active combat foundation guardrail. |
| Smoke runner labels such as `V1 Legacy Slice`, `V2 Narrative / Progression / Persistence`, `V3 Combat / Skills / Resonance / Status / Stats`, `V4 Classes / Mentors / Mastery` | `src/Game.SmokeTests/SmokeTestRunner.cs` | `Legacy Slice`, `Narrative / Progression / Persistence`, `Combat / Skills / Resonance / Status / Stats`, `Player Progression / Combat Validation` | Keep count semantics while neutralizing active output. |
| Assertion group labels containing `V4` or `V5` | `src/Game.SmokeTests/V4SmokeTests.cs` | progression/session/bootstrap responsibility names | Rename with the smoke suite to avoid count-report churn in multiple branches. |
| `docs/V4_Classes_Mastery_Foundation.md` | docs | `PlayerProgression_CombatValidation_Foundation.md` | Smallest safe doc rename; split later only if the document becomes too broad. |
| `docs/V3_Combat_Foundation.md` | docs | `Combat_Foundation.md` | Active living combat doc. |
| `docs/V2_Save_Foundation.md` | docs | `Save_Persistence_Foundation.md` | Active persistence doc. |
| `docs/V2_Narrative_Foundation.md` | docs | `Narrative_Progression_Foundation.md` | Active narrative/progression doc. |

## Deferred Low-Risk Internal Diagnostics

These are active internal facts or enum values, but they are lower risk to defer because they are diagnostic policy labels rather than public composition entry points. They should still be renamed before the naming cleanup is fully closed.

| Current name | Location | Recommended name |
| --- | --- | --- |
| `V3AccessChecked` | `CombatSkillUseValidationResult` | `UnlockAccessChecked` or `LegacyUnlockAccessChecked` |
| `V4AccessChecked` | `CombatSkillUseValidationResult` | `ProgressionAccessChecked` |
| `V3BlockReason` | `CombatSkillUseValidationResult`, `CombatSkillTurnValidationResult` | `UnlockBlockReason` |
| `V4BlockReason` | `CombatSkillUseValidationResult`, `CombatSkillTurnValidationResult` | `ProgressionBlockReason` |
| `V3UnlockProfileMissing` | `CombatSkillUseBlockReason` | `UnlockProfileMissing` |
| `V3SkillLocked` | `CombatSkillUseBlockReason` | `SkillLocked` |
| `V4ContextIncomplete` | `CombatSkillUseBlockReason` | `ProgressionContextIncomplete` |
| `V4SkillBlocked` | `CombatSkillUseBlockReason` | `ProgressionSkillBlocked` |

## Historical References To Keep

The following version-prefixed references are acceptable when clearly historical or compatibility-scoped:

- branch names in audit notes and changelog-style history
- migration notes that explain how a feature moved from one foundation slice to another
- explicit count guardrails such as "V1 remains 38 events" when documenting historical compatibility
- schema compatibility notes that mention an old save schema by version
- snapshot generator profile names such as `VerticalSliceV1` / `VerticalSliceV2` when they intentionally reproduce historical slice output
- script names that invoke historical snapshot generation, unless they become part of the new active demo path

## Recommended Rename Sequence

1. `codex/rename-runtime-demo-bootstrap-api`
   - Rename `V5RuntimeDemoBootstrap` to `ProgressionRuntimeDemoBootstrap`.
   - Update usages, smoke assertions, and docs.
   - This is the smallest branch that unblocks visual smoke demo design.

2. `codex/rename-progression-foundation-smoke-suite`
   - Rename `V4SmokeTests` and `RunV4SmokeTests`.
   - Neutralize active V4/V5 assertion group labels.
   - Preserve V1 event count, V3 assertion count, grouped progression count, and total assertion count.

3. `codex/rename-progression-foundation-docs`
   - Rename `V4_Classes_Mastery_Foundation.md` to `PlayerProgression_CombatValidation_Foundation.md`.
   - Update active headings and links.
   - Keep audit-history version names clearly marked as history.

4. `codex/rename-progress-restore-api`
   - Rename `V4ProgressRestoreCoordinator`, `V4ProgressRestoreResult`, and `V4ProgressRestoreFailureReason`.
   - Update tests/docs without changing restore behavior.

5. `codex/rename-combat-validation-diagnostics`
   - Rename `V3*` / `V4*` combat validation diagnostic properties and enum values to unlock/progression terms.
   - Keep event payload semantics stable.

6. `codex/rename-legacy-combat-and-narrative-smoke-suites`
   - Rename `V1SmokeTests`, `V2SmokeTests`, `V3SmokeTests`, their runner methods, and active group labels.
   - Keep historical count references in docs and final reports.

7. `codex/rename-progress-persistence-api`
   - Rename `V2` persistence services, repository, serializer, validation service, path providers, Unity adapters, and snapshot DTO types.
   - This has the largest blast radius and should be isolated from the demo/bootstrap naming cleanup.

## Blast Radius Summary

- `V5RuntimeDemoBootstrap`: low. One production class plus focused tests/docs.
- `V4SmokeTests`: medium. Test file/class/method names, runner labels, assertion output strings, docs.
- `V4_Classes_Mastery_Foundation.md`: medium. File rename plus links and active headings.
- `V4ProgressRestore*`: medium. Production files, type references, tests, docs.
- Combat validation diagnostic facts: medium. Production result models, validators, smoke assertions, docs.
- `V1` / `V2` / `V3` smoke suite names: medium. Test organization and count reporting only.
- `V2` persistence APIs and DTOs: high. Production persistence surface, Unity adapters, tests, docs, and schema compatibility language.

## Audit Decision

Naming consistency is a real blocker for visual smoke demo work because the active demo entry point still exposes `V5` in its production-facing API. The earliest safe atomic branch is:

`codex/rename-runtime-demo-bootstrap-api`

That branch should rename only `V5RuntimeDemoBootstrap` to `ProgressionRuntimeDemoBootstrap`, update direct usages and documentation, and rerun the full smoke suite before any visual design begins.
``````

</details>

<a id="archive-07"></a>
## Archive 07: TODO_BACKLOG.md

Historical source path: `docs/TODO_BACKLOG.md`. Original blob: `01bf92203ef6d2a9c00ae446e77b05505126d888`.

<details>
<summary>Show complete historical Markdown source (07)</summary>

``````markdown
# TODO Backlog

This is the living backlog for the current foundation track. It should reflect what is true now, not an early wishlist.

## Guardrails

- Preserve the V1 legacy slice guardrail: `38` emitted events.
- Preserve the V3 combat foundation guardrail: `100` assertions.
- Keep foundation work atomic: one branch, one purpose.
- Prefer audit/design slices before broad integration.
- Keep gameplay rules in domain/application code, not Unity presentation code.
- Do not migrate or replace `FoundationVerticalSlice` unless a dedicated branch explicitly approves that scope.
- Do not start visual work until active version-prefixed API names that block the visual boundary are neutralized.
- Visual layers may consume backend state, results, and events; visual layers must not own gameplay rules.
- Every feature branch must update this backlog when it changes the project state.

## Completed / Closed Foundations

- V1 legacy slice remains the compatibility baseline with exactly `38` emitted events.
- V3 combat foundation remains stable with exactly `100` assertions.
- Classes, mastery, mentors, and teaching foundations are implemented and covered by smoke assertions.
- Skill retention is implemented for mastered retained skills.
- Skill trees and scoped skill tree progress are implemented.
- Class-skill tree binding is implemented.
- Progression restore foundation exists through the current restore coordinator/result/failure types.
- Combat validation foundation exists through the skill validator, gateway, optional engine integration, and composition path.
- Progression-aware combat creation exists through the neutral application combat factory path.
- `LoadedPlayerSession` owns one runtime state and creates progression-aware combat engines.
- `PlayerSessionComposition` owns one loaded session and exposes stable runtime state and combat creation.
- `PlayerSessionBootstrap` provides the narrow session bootstrap boundary.
- `ProgressionRuntimeDemoBootstrap` provides the current runtime demo bootstrap path.
- The active runtime/demo bootstrap API rename from `V5RuntimeDemoBootstrap` to `ProgressionRuntimeDemoBootstrap` is complete.
- Runtime/session/bootstrap/demo smoke coverage now lives in `ProgressionRuntimeBootstrapSmokeTests`.
- Active progression foundation documentation now lives in `PlayerProgression_Foundation.md`.

## Current Atomic Track

The current track is naming cleanup before visual smoke demo design.

- Rename progress restore APIs currently prefixed with `V4`.
- Rename combat validation diagnostics currently prefixed with `V3` / `V4`.
- Defer V2 persistence DTO/API renames because the blast radius is high and should be isolated from visual-demo boundary cleanup.

## Immediate Next Branches

1. `codex/rename-progress-restore-api`
   - Rename `V4ProgressRestoreCoordinator`, `V4ProgressRestoreResult`, and `V4ProgressRestoreFailureReason`.
   - Preserve restore semantics.

2. `codex/rename-combat-validation-diagnostics`
   - Rename active `V3` / `V4` combat validation diagnostic facts to unlock/progression terms.
   - Preserve event and validation behavior.

3. `codex/visual-smoke-demo-design-audit`
   - Start only after the visual boundary no longer depends on active V5-named bootstrap APIs.

## Naming Cleanup Backlog

- Completed: `V5RuntimeDemoBootstrap` -> `ProgressionRuntimeDemoBootstrap`.
- Completed: `V4SmokeTests` -> `PlayerProgressionFoundationSmokeTests`.
- Completed: `RunV4SmokeTests` -> `RunPlayerProgressionFoundationSmokeTests`.
- Completed: active V4/V5 smoke runner and assertion group labels -> responsibility-based progression/session/bootstrap labels.
- Completed: runtime/session/bootstrap/demo smoke helpers split into `ProgressionRuntimeBootstrapSmokeTests`.
- Completed: `docs/V4_Classes_Mastery_Foundation.md` -> `docs/PlayerProgression_Foundation.md`.
- `V4ProgressRestoreCoordinator` -> `ProgressRestoreCoordinator` or `PlayerProgressionRestoreCoordinator`.
- `V4ProgressRestoreResult` -> `ProgressRestoreResult` or `PlayerProgressionRestoreResult`.
- `V4ProgressRestoreFailureReason` -> `ProgressRestoreFailureReason` or `PlayerProgressionRestoreFailureReason`.
- `V3AccessChecked` / `V4AccessChecked` -> unlock/progression access terms.
- `V3BlockReason` / `V4BlockReason` -> unlock/progression block reason terms.
- `V3UnlockProfileMissing`, `V3SkillLocked`, `V4ContextIncomplete`, `V4SkillBlocked` -> responsibility-based validation facts.
- Defer broad V2 persistence renames:
  - `IV2SavePathProvider`
  - `UnityV2SavePathProvider`
  - `UnityV2SaveService`
  - `V2ProgressSnapshot*`
  - `V2*Snapshot` DTOs
- Historical references may keep version prefixes when they are branch names, migration notes, audit history, schema compatibility notes, or explicit compatibility guardrails.

## Visual Smoke Demo Backlog

- Run a visual smoke demo design audit before implementation.
- Define the smallest visual boundary that consumes existing backend state, results, and events.
- Use `ProgressionRuntimeDemoBootstrap` as the backend entry point.
- Keep combat, progression, validation, restore, and session ownership rules in backend code.
- Visual layer must not own game rules.
- Visual layer must not construct `TurnBasedCombatEngine` directly.
- Visual layer must not create duplicate runtime state, loaded sessions, or mutable binding registries.
- Add a visual smoke demo foundation only after the design audit approves the boundary.
- Add workflow verification after the visual smoke demo foundation.

## Town Restoration Backlog

Town Restoration is deferred until after the visual smoke demo boundary is designed.

- Town Restoration is not a free placement builder.
- It represents restoring the protagonist's village, roots, culture, and knowledge.
- Buildings appear or are restored when skills and teachings are learned.
- Buildings level up through skill mastery.
- Building functions unlock later, after the restoration foundation is safe.
- No Town Restoration implementation should begin before the visual smoke demo boundary is designed.
- Future Town Restoration work should start with an audit/design branch, not implementation.

## Deferred Gameplay Systems

- Smarter enemy AI.
- Status stacking rules beyond the current foundation.
- Status immunity windows.
- Poison and Burn synergy.
- Atlas prediction API.
- Balance simulator.
- Full Tlamatini tree.
- Additional Mesoamerican-inspired classes.
- Item-taught skills.
- Cognition UI model.
- Active/fixed/equipped teaching persistence beyond the current foundation.
- Lyra and Atlas hybrid skill.
- Gepetto and Spark expansion after validation.
- Teaching slot upgrades.
- Partial objective progress.
- Event-based mission tracking.
- Mission chains and prerequisites.
- Repeatable bounties.
- Story mission examples.
- Codex completion thresholds.
- Codex reward bridge.
- Codex discovery stages.
- Codex UI models.
- Item registry.
- Equipment registry.
- Affixes.
- Loot tables.
- Equipment mastery.

## Technical Debt

- Active production/test/doc names still carry V1/V2/V3/V4/V5 prefixes.
- V2 persistence DTO/API names have high rename blast radius and need a dedicated persistence cleanup branch.
- Current V4/V5 progression assertions are still grouped under the old V4 smoke suite.
- Living progression docs still include active V4 framing.
- Restore API naming still exposes V4 despite being part of the active progression foundation.
- Combat validation diagnostics still expose V3/V4 terminology.
- Snapshot generator profiles and scripts should remain historical unless they become part of the active demo path.
- Backlog updates must become part of normal branch closure to avoid stale planning docs.

## Explicitly Not Now

- Do not perform visual implementation in naming cleanup branches.
- Do not implement Town Restoration before the visual smoke demo boundary is designed.
- Do not perform broad bootstrap migration.
- Do not replace `FoundationVerticalSlice`.
- Do not change save format.
- Do not change combat policy.
- Do not add rewards, unlock bonuses, progression bonuses, or economy changes.
- Do not start new gameplay mechanics while the active naming cleanup track is open.
- Do not do broad refactors under the cover of naming cleanup.
- Do not rename V2 persistence DTO/API names in the first visual-boundary cleanup branch.

## Definition Of Done Update

Every future feature branch must update `TODO_BACKLOG.md` when it:

- completes an item
- defers an item
- creates a new backlog item
- changes priority
- renames an active system
- closes an audit milestone

Backlog updates should stay factual and scoped to the branch. If a branch changes architecture direction, the backlog must say what changed, what moved next, and what is explicitly not being done yet.
``````

</details>

<a id="archive-08"></a>
## Archive 08: CODEX_WORKFLOW.md

Historical source path: `docs/CODEX_WORKFLOW.md`. Original blob: `5277f5003f52e0f4dc65f0905ed79f13cda8b4e9`.

<details>
<summary>Show complete historical Markdown source (08)</summary>

``````markdown
# Codex Workflow

Use Codex as an incremental engineering partner for Fragments of Wysdom, not as a bulk generator.

## Safe Request Pattern

1. Ask Codex to inspect the repo first.
2. Ask for a short plan before code changes.
3. Keep each task narrow: one system, one bug, one refactor, or one document.
4. Prefer small branches and small commits.
5. Ask Codex to summarize every changed file before you commit.

## Good Tasks

- Add project scaffolding or tests.
- Implement one small Vertical Slice V2 foundation piece.
- Add a focused domain service.
- Extract a small class from an oversized file.
- Review Unity asset impact before touching scenes or prefabs.

## Risky Tasks

- Regenerate the whole project over existing source.
- Rewrite combat, progression, or mission systems in one pass.
- Rename Unity assets without preserving `.meta` files.
- Edit scenes, prefabs, or ScriptableObjects without manual Unity validation.
- Mix gameplay behavior changes with tooling or documentation changes.

## After Codex Changes

Review:

- `git status`
- The diff for every changed file
- Any new or modified Unity `.meta` files
- Any scene, prefab, or ScriptableObject reference changes
- The test/build output

Then manually validate in Unity when assets are involved:

- Open the changed scene.
- Check the Console for errors.
- Inspect changed prefabs and ScriptableObjects.
- Enter Play Mode for the affected flow.
- Confirm serialized references are intact.
``````

</details>

<a id="archive-09"></a>
## Archive 09: STRUCTURE_NOTES.md

Historical source path: `docs/STRUCTURE_NOTES.md`. Original blob: `6881774e8f133fabb60c2b4815503f4f2463c51c`.

<details>
<summary>Show complete historical Markdown source (09)</summary>

``````markdown
# Structure Notes

This foundation uses the rule:

> One important public class per file.

Acceptable exceptions:

- tiny enums may be grouped only when very tightly related
- private helper classes
- generated DTOs

Current organization is intended for:

- easier IDE navigation
- cleaner Git diffs
- less painful Unity project growth
- easier refactor and tests
``````

</details>

<a id="archive-10"></a>
## Archive 10: GENERATOR_USAGE.md

Historical source path: `docs/GENERATOR_USAGE.md`. Original blob: `c73c308cd3aa3030933f767cd740bf12dfe2f45a`.

<details>
<summary>Show complete historical Markdown source (10)</summary>

``````markdown
# Multi-Part Generator Usage

Run each script in order from the folder containing the scripts:

```powershell
Unblock-File .\*.ps1

.\00_Create_Structure_And_Docs.ps1
.\01_Common_And_Combat_Core.ps1
.\02_Status_Skills_Resolvers_Battle.ps1
.\03_Player_Cognition_Classes_Equipment_Codex.ps1
.\04_Missions_AI_Persistence.ps1
.\05_Slice_And_Validation.ps1
```

To generate somewhere else:

```powershell
.\00_Create_Structure_And_Docs.ps1 -Root "C:\Dev\NovaFoundation"
```

Use the same `-Root` for all scripts.
``````

</details>

<a id="archive-11"></a>
## Archive 11: Workflow_Verification_Checklist.md

Historical source path: `docs/Workflow_Verification_Checklist.md`. Original blob: `edd9df140fe118bf764b12e6e7f0ddb2d440db0c`.

<details>
<summary>Show complete historical Markdown source (11)</summary>

``````markdown
# Workflow Verification Checklist

Use this checklist after completing any significant mechanic and before continuing
with the next feature branch.

Suggested branch format:

`codex/<mechanic>-workflow-verification`

## Required Checks

- Build passes.
- Smoke tests pass.
- Assertion counts by group are updated in the final report.
- Documentation is updated for the mechanic or workflow being verified.
- No parallel systems were introduced for existing concepts.
- Stateful mechanics have save/restore coverage.
- Registries validate stable ids for definitions and state restoration.
- Old JSON compatibility is preserved when snapshots exist.
- Domain code remains Unity-free.
- No generator-wide rewrites were made unless explicitly requested.
- At least one integrated E2E flow covers the mechanic across its expected boundaries.

## Verification Goal

Detect technical debt and integration problems early before continuing to expand
the backend.
``````

</details>

<a id="tool-entrypoint-provenance"></a>
## Tool Entrypoint Provenance

Historical entrypoint text from the editing baseline, preserved before replacing those README files with navigation-only pointers. These are not current run or Unity integration instructions.

### Historical tools/README_TO_RUN.md

<details>
<summary>Show former tool README source</summary>

``````markdown
# How to Run

This is source-only C#.

## Console quick test

Create a console app and call:

```csharp
Game.Slice.FoundationVerticalSlice.RunToConsole();
```

## Unity

Copy `src/Game` into:

```text
Assets/Scripts/Game
```

Then create a MonoBehaviour adapter that calls domain/application services.

Do not move gameplay rules into MonoBehaviours.
``````

</details>

### Historical tools/SnapshotGenerator/README.md

<details>
<summary>Show former tool README source</summary>

``````markdown
# NovaFoundation Consolidated Generator V2

This package replaces the previous "run scripts 00-06 manually" flow with a single consolidated entry point:

```powershell
.\Generate-NovaFoundation.ps1 -Profile VerticalSliceV2 -Force
```

## Why this exists

The Iteration 2 Nahual Path work should not become the beginning of an endless chain of patch scripts. This generator keeps the project generation modular, but exposes one stable command.

## Profiles

### VerticalSliceV1
Generates the original NovaFoundation vertical slice foundation.

```powershell
.\Generate-NovaFoundation.ps1 -Profile VerticalSliceV1 -Force
```

### VerticalSliceV2
Generates NovaFoundation with the Iteration 2 Nahual Path systems included:

- Knowledge alignment system
- Nahual discovery/unlock hook
- Wisdom Fragments resource placeholder
- forbidden transmutation prototype
- lightweight NPC/vendor reaction system
- minimal branching mission
- codex expansion
- documentation and TODO updates

```powershell
.\Generate-NovaFoundation.ps1 -Profile VerticalSliceV2 -Force
```

## Windows shortcut

If PowerShell script execution policy blocks you, run:

```bat
Run_VerticalSliceV2_Bypass.bat
```

## Recommended future workflow

Do not keep adding permanent incremental scripts like `07`, `08`, `09` forever.

Instead:

1. Add or update a module under `Modules/`.
2. Add the module to the selected profile.
3. Keep `Generate-NovaFoundation.ps1` as the only public entry point.
4. Once an iteration is accepted, treat it as part of the profile, not as a temporary patch.

This keeps the generator scalable while preserving a clean architectural history.
``````

</details>


<a id="binary-document-ledger"></a>
## Documentation Format Standardization Ledger

**Publication addendum, 2026-10-31; baseline adc751cf.** The existing 15-file consolidation commit is reused, not recreated. The additions above and this appendix are a separate documentation-only standardization. Earlier statements that all binaries remain untouched describe the prior pass; the dispositions below supersede that inventory without rewriting its record.

Inventory covered tracked and accessible untracked project files, including hidden paths, for DOC/DOCX, PDF, RTF, ODT, presentation/spreadsheet formats, TXT, HTML, SVG, editable diagrams, EPUB, TeX and RST. Git internals and generated bin/obj/.dotnet_home output were excluded. Exactly six document binaries were found; no RTF/ODT, editable diagram source or standalone project image was found. The canonical active project specification is this Markdown Bible; no binary is an active competing specification.

| Source in docs (historical filename) | Classification | Comparison / migration | Disposition |
|---|---|---|---|
| Fragments of Wisdom — Vertical Slice Iteration 2 Spec.docx | Historical project specification | 254 body paragraphs, 226 nonempty; no tables; 22 decorative horizontal rules only; full text in binary-iteration2; missing constraints in Part II | Retired after preservation verification |
| Fragments Of Wysdom — Codex Ui Concept V0.docx | Historical design document | 729 paragraphs, 700 nonempty; no tables; 29 decorative rules; full concept in binary-codex; state/field/navigation details in Part I | Retired after preservation verification |
| Fragments Of Wysdom — Visual Foundation Bible V0.docx | Historical design document | 714 paragraphs, 692 nonempty; no tables; 22 decorative rules; full concept in binary-visual; art acceptance details in Part I | Retired after preservation verification |
| FOUNDATION MASTER SPEC — NOVA RPG.pdf (book-symbol filename) | Generated/exported historical project specification | All 11 pages text-extracted and visually inspected; both tables transcribed; complete page text in binary-foundation-pdf | Retired after preservation verification |
| LLM_Conception.pdf | Historical design conversation / external-reference and graphical capture | All 425 pages text-extracted; decision-bearing passages compared; examples/diagrams/reward proposal indexed below; 3 embedded images on page 2; only representative pages visually inspected | Preserved unchanged as non-authoritative source, not a parallel Bible |
| Fragments_of_Wysdom_Game_Design_Architecture_Bible_v1.docx | Generated/exported historical consolidation; formerly untracked local file | All 346 body paragraphs and 3 tables compared; unique constraints migrated; historical tables preserved below; headers/footer inspected | Removed locally after explicit user authorization and repeat content verification; never tracked or published |

The three retired DOCX files contain no image/media/embedded objects, diagrams, tracked revisions or substantive comments/notes. OOXML graphics are only decorative horizontal rules. Extraction retains all nonempty paragraphs in order, heading hierarchy, list nesting and numbering semantics; empty layout paragraphs and decorative rules are omitted. DOCX rendering was unavailable because no bundled LibreOffice renderer was installed; retirement relies on complete OOXML structural/content verification, not a claim of visual page-layout fidelity. The retired PDF is text plus two tables and separators, with no embedded images. Its original page text is retained alongside readable tables. Typography and pagination are not canonical requirements.

The conversation PDF remains because a text-only migration is not a faithful reconstruction of its source attribution, graphical capture and conversational context; it includes clipped historical code and superseded designs. We do not claim every one of its 425 pages was visually inspected or that its prototype code is authoritative. No external references or original graphical assets were deleted. The local Bible export was never staged and was subsequently removed with explicit user authorization after repeat content verification.

### Binary Provenance

Original Git blobs and SHA-256 identify the exact compared files. The untracked export's Git blob value is computed for identification only; it is not added to the object database by this task.

| Source | Original Git blob | SHA-256 |
|---|---|---|
| `Fragments of Wisdom — Vertical Slice Iteration 2 Spec.docx` | `dc7d23143a6ca5986331c40398a0b0f918e2a9dc` | `95c4ae46cc00311dc2e5a5741e55ff1cf0bfaf85927869f4997bb1ffcae83441` |
| `Fragments Of Wysdom — Codex Ui Concept V0.docx` | `189ca77cddd833e0926c77213b33e16a6c292cb9` | `e41e3836652ae19d7fbf26d24fa365697def5bd482725a4d71668aa6f40e2b11` |
| `Fragments Of Wysdom — Visual Foundation Bible V0.docx` | `c0f4cfbe55c2d6514ef4aba70ce707be381107ad` | `0c7640d5b709ad00287df584f0c99f220a19d1702bed4c77b4aa86a798169a9d` |
| `Fragments_of_Wysdom_Game_Design_Architecture_Bible_v1.docx` | `79332da083a15a7711e05d55c3b004f9eab5afe5` | `46f450e280e9dd8e43047c780c3393710e28574a7a8faa36f95b1c2e214407e2` |
| `LLM_Conception.pdf` | `9552ef0ea52f0e021375f81f8c0133f9257f5298` | `92ac963c20829fa9853dd812056a1b44ca4229e0903a94e25e240a8d1ad3316f` |
| `📘 FOUNDATION MASTER SPEC — NOVA RPG.pdf` | `eba2ea2b2fa97f5104562130aa32dfadb6d4004b` | `bcba75c01960645b10eb606e05b2ee6f0b32ed2ddddd10129fe2a8b6b3dd8e97` |

### Source Comparison and Conflict Resolution

Standardization verification: build exit 0 with 0 warnings/errors; complete smoke exit 0 with V1 2, Shared 16, V2 264, V3 100, Progression 517, total 899 and V1 events 38. All 80 live local Markdown links resolve and all 41 explicit anchors are unique. Source checks preserve all 1,618 nonempty paragraphs from the three retired DOCX files and every extracted page of the 11-page PDF. Every pre-existing Bible line remains unchanged and in order. The original eleven technical archives, twelve audit findings and audit-driven backlog remain intact. No gameplay, test, tooling script, configuration or .gitignore changes are part of this follow-up.

- Iteration 2's theme and architecture were already present. Missing transmutation failure/immunity/economy limits, discovery-setting choices and future hooks were migrated into Part II. Its old imperative "Implement" text is historical, not current authorization.
- The visual/Codex sources supplied missing field inventories, layout trade-offs, progression-state cues, motion/audio/logo/icon rules, acceptance questions and example entries. Complete detail is preserved below; no art assets or completed UI are claimed.
- The local Bible's vision, architecture, governance, deferred scope and Sanctuary rules were already represented. Added mentor mission examples, optional reflection, rewards/economy safeguards and detailed concept maps preserve remaining intent. Its Essence replacement, implementation status and naming-first sequence are superseded by the current audited chapters.
- The foundation PDF's stat pairs and temporary/fixed/equipped table are transcribed below. Its claims about Silence, shared mastery, complete save coverage and Codex completion rewards are historical; Part V findings and Part II boundaries remain authoritative.
- The raw conversation exposes unresolved prerequisites (mastery vs node unlock), recoverable death loss, map/economy choices, class/AI candidates and synthesis. These are recorded as historical preferences/proposals and OPEN DECISION, not silently promoted to current rules.
- No new implementation backlog replaces the existing audit-driven one. Recovered proposals must pass its design and verification gates.

<a id="binary-iteration2"></a>
## Migrated Source: Vertical Slice Iteration 2

Historical project specification, not current implementation instructions. Source blob `dc7d23143a6ca5986331c40398a0b0f918e2a9dc`. Full nonempty paragraph content follows, converted from OOXML to Markdown.

<details>
<summary>Historical Iteration 2 specification</summary>

``````markdown
### Fragments of Wisdom — Vertical Slice Iteration 2 Spec

#### Context

Current project:

Fragments of Wisdom

Stylized logo option:

Fragments of Wysdom

The game is a turn-based HD-2D RPG with:

- cognition systems

- mastery systems

- codex progression

- class systems

- mentor/teaching systems

- tactical combat

- mesoamerican-inspired atmosphere

- philosophical progression themes

The project is NOT intended to be:

- edgy grimdark fantasy

- generic JRPG clone

- pure visual novel

The target tone is:

- intelligent

- mysterious

- contemplative

- adventurous

- systemic

- emotionally restrained but meaningful

Inspirational tonal reference:

Octopath Traveler

### Core Theme

The central thematic question of the game is now:

“Is wisdom something that is shared… or extracted?”

This creates two progression philosophies:

#### Shared Wisdom Path (mainline philosophy)

Knowledge is:

- taught

- inherited

- mastered through community

- refined through mentorship

This path includes:

- teachers

- towns

- commerce

- codex collaboration

- social integration

#### Nahual Path (working name)

Knowledge is:

- extracted directly from reality

- transmuted

- isolated from tradition

- forcibly reinterpreted

This path:

- rejects traditional teachers

- sacrifices social support

- unlocks forbidden cognition/transmutation systems

IMPORTANT:
This is NOT an “evil route”.

It must feel:

- tempting

- philosophically dangerous

- lonely

- powerful

- misunderstood

NOT:

- demonic

- cartoonishly evil

- necromantic cliché

### Goal of Iteration 2

DO NOT create two full campaigns.

The purpose of this iteration is ONLY to:

1. Introduce the existence of the Nahual Path

2. Demonstrate systemic consequences

3. Prove architectural scalability

4. Add thematic depth to the vertical slice

5. Show meaningful player choice

### Iteration 2 — Required Systems

### 1. Alignment System

Introduce a lightweight alignment/progression branch system.

#### Required enum

KnowledgeAlignment

{

    SharedWisdom,

    NahualPath

}

#### PlayerProfile additions

Add:

- currentAlignment

- alignmentFlags

- alignmentProgression

The system must support:

- future expansion

- NPC reactions

- vendor restrictions

- skill restrictions

- mission branching

WITHOUT rewriting the combat engine.

### 2. Nahual Unlock Event

Add an early-game discovery moment.

Suggested structure:

- hidden ruin

- forbidden codex fragment

- isolated NPC

- ancient device

- cognition anomaly

The player discovers:

there exists another method of understanding reality.

This must feel:

- mysterious

- intellectually seductive

- dangerous

NOT:

- overtly evil

### 3. First Forbidden Skill

Implement the first Nahual-exclusive skill.

#### Working Skill Name

Transmute Lesser Being

#### Initial functionality

During combat:

- attempt to transmute defeated enemy

- chance-based success

- converts enemy into resource/item/material

#### Important balancing rules

The skill CANNOT:

- trivialize economy

- produce infinite value

- invalidate loot systems

Required risks:

- failure chance

- unstable output

- corrupted materials

- some enemies immune

- bosses highly resistant

### 4. NPC Reaction System (light implementation)

DO NOT create a full reputation system yet.

Only lightweight reactions.

Examples:

- vendors increase prices

- some NPC dialogue changes

- one mentor becomes distant

- one sidequest becomes unavailable

Purpose:

prove the world reacts to philosophical choices.

### 5. New Resource Type

Introduce:

Wisdom Fragments

These are NOT crystals.

They represent:

- incomplete understanding

- fractured teachings

- extracted cognition

- conceptual remnants

#### Uses

Future systems:

- fixing teachings

- upgrading cognition

- unlocking forbidden nodes

- codex progression

Current iteration:

- minimal placeholder implementation is enough

### 6. Minimal Branching Mission

Add one optional mission where the player chooses:

#### Option A

Continue with traditional teachers

#### Option B

Accept first Nahual teaching

The mission outcome should:

- change alignment

- unlock one future flag

- slightly alter world behavior

### 7. Codex Expansion

Add codex entries for:

- Nahual Path

- Wisdom Fragments

- Transmutation

- Forbidden Teachings

### 8. Architectural Constraints

VERY IMPORTANT:

Do NOT:

- rewrite combat engine

- fork entire gameplay systems

- duplicate progression systems

- create separate save structures

Instead:

- extend existing architecture

- use flags/hooks/interfaces

- preserve scalability

### 9. Tone Requirements

The Nahual Path should feel:

seductive

isolating

intelligent

dangerous

forbidden

elegant

ancient

NOT:

edgy

bloody

satanic

generic evil

### 10. Future Hooks (placeholder only)

Prepare extension points for future systems:

- advanced transmutation

- corrupted cognition

- forbidden teachers

- alternative endings

- alignment-based cities

- transmutation crafting

- enemy harvesting

- cognition instability

DO NOT fully implement them yet.

Only prepare architecture/hooks.

### Deliverables for Iteration 2

Expected output:

- updated vertical slice

- updated domain model

- updated mission flow

- alignment system

- first Nahual mechanics

- transmutation prototype

- codex updates

- architecture hooks

- updated documentation/TODOs

### Final Direction Reminder

This project should increasingly feel like:

“a philosophical RPG with systemic combat”

NOT:

“a generic RPG with philosophy text pasted on top.”
``````

</details>

<a id="binary-codex"></a>
## Migrated Source: Codex UI Concept

Historical concept with proposed mechanics and sample content, not a completed UI or grant policy. Source blob `189ca77cddd833e0926c77213b33e16a6c292cb9`.

<details>
<summary>Complete historical Codex concept</summary>

``````markdown
### Fragments of Wysdom — Codex UI Concept v0.1

#### 1. Core Concept

The Codex in Fragments of Wysdom is not a simple lore journal.

It is the central interface where the player reconstructs the forgotten wisdom of the world.

The Codex should feel like:

A ceremonial memory instrument that unfolds the lost knowledge of a world that forgot its wisdom.

It should combine three identities:

1. Ceremonial Codex — a sacred record of recovered teachings, myths, enemies, places, skills, classes, and artifacts.

2. Living Mental Map — a visual network where knowledge connects, grows, and reveals relationships.

3. Altar of Knowledge — a place where discoveries feel meaningful, almost ritualistic.

The Codex must not feel like a database, wiki, or generic RPG journal. It should feel like a sacred object that the player slowly restores.

#### 2. Design Goal

The Codex must support four major game fantasies:

##### 2.1 Recovering Lost Wisdom

Every new entry should feel like a fragment returning to its place.

The player should not feel that they are merely collecting text. They should feel that they are repairing memory.

##### 2.2 Understanding the World

The Codex should help the player understand:

- creatures

- cultures

- regions

- teachers

- classes

- skills

- items

- myths

- factions

- corrupted knowledge

- philosophical branches

Knowledge should become useful, not decorative.

##### 2.3 Progression Through Learning

The Codex should visually support the idea that learning changes the character.

It may connect to:

- skill unlocks

- class mastery

- bestiary completion

- world knowledge

- teacher relationships

- hidden paths

- advanced abilities

- lore-based discoveries

##### 2.4 Emotional Discovery

Opening the Codex should feel quiet, reflective, and important.

The player should feel:

- curiosity

- reverence

- adventure

- personal growth

- contact with an ancient world

#### 3. Visual Metaphor

The Codex should not be represented as only a European-style book.

Recommended metaphor:

A folded ceremonial knowledge map made of restored fragments, carved memory plates, living glyphs, and star-like connections.

It may visually combine:

- folded codex panels

- bark-paper or ancient manuscript surfaces

- carved stone plates

- jade/gold markers

- inked glyphs

- petroglyph-inspired faces/symbols

- constellation-like knowledge connections

- fragment slots

- altar-like framing

The Codex can unfold horizontally, vertically, or radially depending on the screen.

#### 4. Main UX Structure

The Codex should be organized into major knowledge domains.

Recommended first-level sections:

1. World Memory

2. Bestiary

3. Classes & Masters

4. Skills & Teachings

5. Artifacts & Items

6. Regions & Sites

7. Factions & Philosophies

8. Corrupted Knowledge

9. Myths & Echoes

10. Completion / Wisdom Reconstruction

Not all sections need to be unlocked from the start.

Early in the game, the Codex can appear incomplete, with some categories hidden as sealed fragments.

#### 5. Navigation Concept

The Codex should avoid a plain list-only structure.

A good structure would combine:

- category wheel or carved tab ring

- folded-page panels

- node maps

- searchable/indexable entries later if needed

- visual completion marks

- relationship lines between entries

##### Recommended Navigation Layout

###### Left / Outer Layer

Category selector.

Could appear as:

- carved glyph tabs

- radial knowledge ring

- vertical stone markers

- folded codex edge tabs

###### Center Layer

Knowledge map or entry list depending on category.

For example:

- Bestiary may show creature glyph cards.

- Skills may show prerequisite nodes.

- Regions may show a fragment map.

- Myths may show restored panels.

###### Right / Inner Layer

Selected entry details.

Contains:

- title

- visual emblem

- discovered information

- hidden/unknown fields

- gameplay notes

- lore fragment

- relationships

- completion progress

#### 6. Entry States

Every Codex entry should have clear visual states.

##### 6.1 Unknown

The player has not encountered this knowledge yet.

Visual language:

- dark empty fragment slot

- erased glyph

- broken outline

- unreadable marks

- muted stone/paper texture

Text examples:

- “Unknown”

- “Fragment missing”

- “Memory not yet recovered”

##### 6.2 Glimpsed

The player has seen hints but not enough to fully identify the entry.

Visual language:

- faint silhouette

- partial glyph

- incomplete border

- dim jade pulse

- one or two visible clue marks

Useful for:

- enemies seen but not studied

- rumors

- teacher mentions

- incomplete myths

- regions heard about but not visited

##### 6.3 Discovered

The player has unlocked the basic entry.

Visual language:

- readable title

- basic illustration/glyph

- restored panel segment

- soft ink reveal

##### 6.4 Studied

The player has gained deeper information.

This may require:

- fighting an enemy several times

- completing teacher tasks

- scanning/observing

- finding lore fragments

- using a skill repeatedly

- solving a regional puzzle

Visual language:

- more complete diagram

- relationship lines appear

- gameplay data fills in

- stronger jade/gold highlights

##### 6.5 Mastered / Completed

The entry has been fully reconstructed.

Visual language:

- complete glyph

- closed circle

- gold/jade seal

- ceremonial completion animation

- restored codex panel

This should feel satisfying but not like a mobile reward burst.

#### 7. Core Categories

#### 7.1 World Memory

This is the broad historical and philosophical section.

Contains:

- major historical eras

- lost civilizations

- cosmology

- great catastrophes

- foundational teachings

- world-level mysteries

Visual presentation:

- mural fragments

- timeline as broken ceremonial panels

- star paths

- recovered memory stones

Purpose:

Give the player a sense that the world’s past is being reconstructed piece by piece.

#### 7.2 Bestiary

The Bestiary should be more than enemy stats.

It should represent the player learning the nature, habits, symbolism, weaknesses, and ecological/spiritual role of creatures.

Each creature entry may include:

- name

- glyph/icon

- silhouette/illustration

- region

- behavior

- weaknesses/resistances

- drops

- related myths

- observed attacks

- lore note

- mastery progress

##### Bestiary Progression Idea

Creature knowledge could advance through levels:

1. Encountered

2. Defeated

3. Studied

4. Pattern understood

5. Fully mastered

Each level reveals more information.

This supports the game’s theme: knowledge gives power.

##### Visual Direction

Bestiary entries could appear as carved creature plates or codex field sketches.

Creature illustrations should not look like clean encyclopedia photos. They should look like reconstructed knowledge: part drawing, part glyph, part memory.

#### 7.3 Classes & Masters

This section tracks the player’s relationship with teachers, classes, disciplines, and paths of wisdom.

Contains:

- available classes

- locked classes

- mastered classes

- teacher relationships

- class philosophies

- class skill trees

- prerequisites

- related Codex knowledge

##### Visual Direction

Classes should appear as schools of thought, not merely jobs.

Each class should have:

- a sigil

- a teacher mark

- a philosophy phrase

- a visual motif

- a mastery seal

##### Example Entry Fields

- Class Name

- School / Discipline

- Master / Teacher

- Philosophy

- Known Skills

- Mastery Progress

- Required Teachings

- Related Codex Entries

- Hidden Advanced Path

#### 7.4 Skills & Teachings

Skills should not be listed like disposable abilities.

Each skill should feel like a learned principle.

Contains:

- skill name

- school

- type

- cost

- effect

- mastery condition

- required knowledge

- related teacher

- related enemies or field uses

##### Visual Direction

Skill entries should use sacred geometry and living writing.

Each skill may have:

- a glyph

- a diagram of motion/effect

- a prerequisite path

- a mastery ring

- a small lore explanation

##### Important Rule

The Codex should reinforce that skills come from learning, observation, and mastery, not random level-up magic.

#### 7.5 Artifacts & Items

Items should be documented as cultural and practical objects.

Contains:

- weapons

- armor

- accessories

- relics

- materials

- quest items

- rare knowledge fragments

##### Entry Fields

- Name

- Type

- Origin

- Effect

- Crafting/upgrade info

- Cultural or historical note

- Related class/teacher/faction

- Hidden property if discovered

##### Visual Direction

Item entries should look like catalogued relics, not shop inventory tiles.

Rare artifacts should appear as reconstructed object diagrams with annotations.

#### 7.6 Regions & Sites

This section tracks places the player discovers.

Contains:

- villages

- ruins

- temples

- caves

- observatories

- coastal sanctuaries

- corrupted sites

- hidden schools

##### Entry Fields

- Name

- Region type

- Map fragment

- Known history

- Notable teachers

- Creatures

- Artifacts

- unresolved mysteries

- completion status

##### Visual Direction

Regions should appear as map fragments stitched into the Codex.

Some entries can include:

- topographic markings

- water lines

- star alignments

- route symbols

- ancient names partially restored

#### 7.7 Factions & Philosophies

This section tracks ideological groups, schools of thought, and social structures.

Contains:

- wisdom orders

- teacher lineages

- corrupted knowledge branches

- regional councils

- hidden sects

- philosophical paths

##### Entry Fields

- Faction name

- Symbol

- Core belief

- Known members

- Related regions

- Relationship with player

- Conflicts

- Hidden doctrine

##### Visual Direction

Each faction should have a distinct symbolic language.

Not every faction needs a flag. Some may be represented by:

- teaching seals

- carved marks

- textile patterns

- mask shapes

- ceremonial tools

- astronomical symbols

#### 7.8 Corrupted Knowledge

This section should not be available immediately, or should appear partially sealed.

It tracks forbidden teachings, distorted doctrines, corrupted masters, and dangerous insights.

##### Visual Direction

This category should visibly break the Codex rules.

Use:

- obsidian panels

- fractured circles

- misaligned glyphs

- violet-black accents

- incomplete seals

- elegant but unsettling patterns

##### UX Idea

When entering this section, the Codex should feel slightly wrong.

Not horror. Not gore. More like the instrument itself is being forced to display knowledge it was not meant to hold.

Possible effects:

- delayed glyph alignment

- faint distortion

- missing sound cues

- fractured panel reveal

- headings that appear partially overwritten

##### Important Rule

Corrupted Knowledge should be tempting.

It should not look obviously stupid or purely evil. It should look powerful, beautiful, efficient, and dangerous.

#### 7.9 Myths & Echoes

This section contains stories, memories, oral traditions, visions, songs, and symbolic fragments.

It is where the world feels most spiritual and poetic.

Contains:

- myths

- ancestral echoes

- dreams

- recovered oral traditions

- symbolic stories

- moral lessons

- prophecy-like fragments

##### Visual Direction

This section can be more abstract:

- animated mural panels

- waves of ink

- shadowed silhouettes

- constellation trails

- voices represented as glyph ripples

- cave-wall imagery

##### Purpose

This section allows cultural and spiritual depth without turning everything into dry exposition.

#### 7.10 Completion / Wisdom Reconstruction

This section tracks overall knowledge recovery.

It should not feel like a checklist alone.

It should feel like the world-memory becoming whole again.

##### Possible Metrics

- World Memory restored

- Bestiary understood

- Classes mastered

- Skills mastered

- Regions documented

- Myths recovered

- Artifacts catalogued

- Corruption understood

##### Visual Direction

Represent completion as a large broken central diagram gradually restoring itself.

This could be:

- a wisdom mandala

- a ceremonial map

- a constellation mural

- a fragmented sun/circle

- a sacred knowledge tree

As the player progresses, missing fragments return.

#### 8. Main Screen Concept

#### 8.1 First Impression

When the player opens the Codex, the screen should not instantly show a flat list.

Instead:

1. A soft ritual sound plays.

2. A folded panel or memory surface unfolds.

3. Glyphs briefly align.

4. The current knowledge map appears.

5. The selected section glows subtly.

This animation should be elegant and fast enough for repeated use.

#### 8.2 Layout Option A: The Knowledge Wheel

A central circular diagram represents the whole Codex.

Around it are major categories:

- World Memory

- Bestiary

- Classes

- Skills

- Artifacts

- Regions

- Factions

- Corruption

- Myths

Selecting a category rotates or unfolds that region of the diagram.

Pros:

- Strong identity

- Feels ceremonial

- Supports reconstruction metaphor

Cons:

- More complex to implement

- Must be carefully designed for usability

#### 8.3 Layout Option B: Folded Codex Panels

The Codex is presented as horizontal or vertical folding panels.

Each major section is a panel or tab.

Pros:

- Strong connection to codex inspiration

- Easier to understand

- Good for HD-2D aesthetic

Cons:

- Risk of becoming parchment-menu generic if not designed well

#### 8.4 Layout Option C: Altar Map

The Codex appears like an altar or stone table where fragments are placed.

Categories are carved around the edges; selected knowledge appears in the center.

Pros:

- Unique

- Strong ritual feeling

- Good for controller navigation

Cons:

- Needs strong visual design to avoid looking static

#### 8.5 Recommended Direction

Use a hybrid:

Folded Codex Panels + Central Knowledge Map + Altar-like framing.

This gives the Codex a distinctive identity while preserving usability.

#### 9. Entry Detail Screen

Each entry detail screen should feel like a restored knowledge fragment.

##### Suggested Layout

###### Header

- Entry title

- category sigil

- completion state

- small mastery marker

###### Left Area

- illustration/glyph/object diagram

- silhouette or restored image

- visual state based on completion

###### Center Area

- primary description

- gameplay information

- discovered facts

- hidden fields

###### Right Area

- relationships

- related skills/classes/items/regions

- mastery requirements

- discovered sources

###### Bottom Area

- fragment progress

- teacher notes

- myth echo

- player annotations in future versions if desired

#### 10. Completion Language

Avoid generic completion phrases like:

- “100% complete”

- “Collected all entries”

- “New database record”

Use world-specific language instead.

Possible terms:

- Fragment Restored

- Memory Recovered

- Teaching Understood

- Pattern Recognized

- Wisdom Complete

- Seal Restored

- Doctrine Clarified

- Echo Remembered

- Glyph Completed

Completion percentage can still exist, but it should be visually framed as reconstruction.

#### 11. Interaction Feel

The Codex should feel tactile and ceremonial.

##### Input Feel

When navigating:

- tabs slide like folded panels

- glyphs light subtly

- selection markers move like carved beads or jade inlays

- relationship lines draw themselves

- hidden fields pulse faintly

##### Sound Direction

Possible UI sounds:

- soft stone movement

- dry paper fold

- ink stroke

- jade click

- distant wooden percussion

- low ceremonial breath

- water echo for Taíno-inspired regions

Avoid:

- sci-fi beeps

- mobile reward sounds

- casino chimes

- overly cheerful clicks

#### 12. Visual States for Missing Knowledge

Missing information should be visually beautiful, not just blank.

Examples:

- erased glyph marks

- darkened panels

- broken decorative borders

- incomplete constellation lines

- title hidden behind worn pigment

- “unknown” represented as a missing stone fragment

This keeps the Codex visually interesting even when incomplete.

#### 13. How Codex Unlocks Should Work

The Codex should update through multiple discovery sources.

Possible sources:

- encountering enemies

- defeating enemies

- observing enemy skills

- speaking with teachers

- completing class tasks

- discovering ruins

- finding inscriptions

- solving puzzles

- using skills in combat

- mastering weapons/classes

- finding artifacts

- experiencing myths/visions

- making moral or philosophical choices

This helps the Codex feel integrated with gameplay.

#### 14. Gameplay Integration Ideas

The Codex can support gameplay directly.

##### 14.1 Bestiary Advantage

Studying enemies reveals:

- weaknesses

- resistances

- behavior patterns

- drop hints

- counter-skill suggestions

Potential unlock:

- After reaching studied/mastered state, certain skills gain bonuses against that enemy family.

##### 14.2 Class Unlocks

Some classes may require Codex knowledge.

Example:

- Learn three water-related myths.

- Study two coastal spirits.

- Complete a teacher’s doctrine.

- Recover a lost diagram.

##### 14.3 Skill Mastery

Skills may gain mastery when the player understands related principles.

Example:

- A wind skill improves after studying flying creatures or storm ruins.

- A seal skill improves after recovering glyph knowledge.

##### 14.4 Hidden Path Discovery

The Codex can hint at hidden branches without spelling everything out.

Example:

- “This doctrine appears incomplete.”

- “A related teaching was erased.”

- “The glyph connects to an unknown school.”

#### 15. Controller and Usability Rules

The Codex must be beautiful, but it must remain usable.

##### Rules

- Player must reach major categories quickly.

- Text must be readable.

- Completion status must be clear.

- Hidden information must not confuse the player.

- Repeated opening should not feel slow.

- Animations must be skippable or short.

- Controller navigation must be natural.

- Search/filter can exist later, but should not define the visual identity.

##### Danger

The Codex cannot become a beautiful inconvenience.

If the player avoids opening it because it is slow or confusing, the design has failed.

#### 16. UI Anti-Patterns

Avoid these:

- Flat wiki page

- Generic quest journal

- Blue RPG Maker menu

- Mobile collection album

- Gacha character archive

- Overly ornate unreadable parchment

- Purely decorative fake glyphs everywhere

- Giant walls of text with no visual hierarchy

- Modern clean app layout disconnected from the world

#### 17. First Prototype Recommendation

The first Codex prototype should not try to build everything.

Build one vertical slice of the Codex:

##### Prototype Scope

1. Main Codex opening animation.

2. Category selector with 4 categories:

  - World Memory

  - Bestiary

  - Classes & Masters

  - Skills & Teachings

3. One sample Bestiary entry with multiple completion states.

4. One sample Class entry.

5. One sample Skill entry.

6. One locked/unknown entry.

7. One relationship line between entries.

8. One completion/reconstruction marker.

##### Why This Prototype

This tests:

- visual identity

- readability

- navigation

- progression language

- ornament balance

- relationship mapping

- HD-2D UI compatibility

#### 18. Sample Entry: Bestiary

##### Entry Name

Stone-Shell Burrower

##### State: Studied

Fields:

- Region: Root-buried ruins

- Nature: Defensive earth creature

- Observed Behavior: curls when threatened, counterattacks after impact

- Weakness: resonance / precision strikes

- Resistance: blunt force

- Related Teaching: Patience Before Impact

- Related Skill: Echo Tap

- Lore Fragment: “The old teachers watched creatures like this to understand how stillness can become defense.”

Visual:

- creature shown as partial field sketch

- shell glyph restored

- behavior diagram half-complete

- jade line connecting it to a defensive skill

#### 19. Sample Entry: Class

##### Entry Name

Keeper of Echoes

##### State: Discovered

Fields:

- Discipline: Memory and resonance

- Master: Unknown / hidden

- Philosophy: “Nothing truly vanishes if the world still carries its shape.”

- Known Skills: Echo Tap, Recall Mark

- Mastery Condition: recover three echo fragments and complete the first resonance trial

- Related Codex Entries: Myths & Echoes, Forgotten Halls, Stone-Shell Burrower

Visual:

- class sigil partially restored

- teacher mark hidden

- resonance circle incomplete

- soft blue/jade highlights

#### 20. Sample Entry: Corrupted Knowledge

##### Entry Name

The Perfected Shortcut

##### State: Glimpsed

Fields:

- Doctrine: Unknown

- Promise: mastery without patience

- Cost: erased from visible record

- Related Path: sealed

- Warning: “A circle completed too quickly may not close around the truth.”

Visual:

- black-violet panel

- beautiful but cracked circle

- glyphs slightly misaligned

- no blood, no gore

- elegant danger

#### 21. Strongest Concept Summary

The Codex should be a gameplay and visual pillar.

It is where:

- lore becomes progression

- enemies become lessons

- teachers become systems

- places become memory

- skills become understood principles

- completion becomes reconstruction

- corruption becomes temptation

Final guiding phrase:

The Codex is not where the player stores information. It is where the world remembers itself through the player.
``````

</details>

<a id="binary-visual"></a>
## Migrated Source: Visual Foundation

Historical visual constitution and proposals, not approved asset files or a replacement roadmap. Source blob `c0f4cfbe55c2d6514ef4aba70ce707be381107ad`.

<details>
<summary>Complete historical visual concept</summary>

``````markdown
### Fragments of Wysdom — Visual Foundation Bible v0.1

#### 1. Purpose of This Document

This document defines the foundational visual direction for Fragments of Wysdom. It exists to prevent the project from becoming visually inconsistent, generic, or disconnected from its core themes.

The goal is not to lock every artistic decision forever. The goal is to establish a clear visual constitution: what the game must feel like, what it must avoid, and how visual decisions should support the identity of the world.

This document should guide:

- Art direction

- UI and UX design

- Logo and branding

- Character design

- Environment design

- Iconography

- VFX and magic language

- Codex presentation

- Menus and HUD

- Future concept art prompts

- Vertical slice visual decisions

#### 2. Core Visual Identity

Fragments of Wysdom should feel like:

Memories of a world that forgot its wisdom.

The visual identity should communicate that the world was once profound, sacred, advanced in its own spiritual and intellectual way, but has suffered erosion, fragmentation, loss, and misinterpretation.

The player should not feel like they are entering a generic fantasy world. They should feel like they are uncovering the remaining pieces of a civilization whose knowledge was powerful, beautiful, dangerous, and partially forgotten.

The world should feel ancient, but not dead. Damaged, but not empty. Mysterious, but still inviting.

#### 3. Primary Emotional Targets

The game should visually evoke three primary emotions:

##### Curiosity

The player should constantly feel that there is something hidden beneath the surface: forgotten symbols, ruined mechanisms, half-understood rituals, lost teachings, buried meanings, and incomplete truths.

Visual design should encourage questions:

- What was this place used for?

- Who built this?

- What does this symbol mean?

- Why was this knowledge abandoned?

- What did this civilization understand that others forgot?

##### Reverence

Knowledge in this world is not casual information. It is sacred, earned, dangerous, and transformative.

The visual language should make learning feel meaningful. Important locations, teachers, codex discoveries, and major abilities should feel ceremonial rather than disposable.

Reverence should be expressed through:

- Ancient geometry

- Sacred spaces

- Ritual framing

- Celestial alignments

- Symbolic architecture

- Carefully paced visual effects

- UI that feels crafted rather than mass-produced

##### Adventure

Despite the ancient and melancholic tone, the game must still feel like an adventure.

The world should invite exploration. It should not feel so oppressive that discovery becomes exhausting. There should be a sense of movement, wonder, danger, reward, and personal growth.

The visual tone should support the fantasy of becoming wiser through exploration.

#### 4. Darkness Level

The preferred tonal range is:

Melancholic and ancient, but still hopeful, with selected moments of heavier decay and tragedy.

The baseline should be emotional and mysterious, not grimdark.

##### Default Tone

- Ancient

- Spiritual

- Mysterious

- Reflective

- Hopeful beneath the sadness

- Beautiful despite decay

##### Occasional Darker Moments

Some areas, factions, memories, bosses, or narrative turns may become heavier:

- Tragic ruins

- Corrupted knowledge

- Failed masters

- Broken temples

- Forgotten victims of misused wisdom

- Places where learning became obsession

##### Avoid as Default Tone

The game should not primarily feel:

- Nihilistic

- Horror-driven

- Edgy

- Gore-focused

- Oppressively dark

- Hopeless

Darkness should be used as contrast, not as the visual foundation.

#### 5. Cultural Inspiration Rule

The game should be:

Strongly inspired by real Taíno and Mesoamerican elements, transformed into a respectful fictional world.

This means the game should not simply copy real cultures or use sacred symbols as decoration. It should study real sources, extract respectful principles, and transform them into an original fantasy language.

##### Desired Approach

- Inspired by real cultural patterns, materials, cosmology, architecture, mythology, and symbolism.

- Transformed into fictional nations, orders, schools, ruins, rituals, and knowledge systems.

- Cultural references should feel integrated into the world logic, not pasted on top.

- Taíno influence should be treated with special care because of its connection to Cuban/Caribbean identity.

##### Dangerous Approach to Avoid

- Random pyramids with no meaning.

- Generic “tribal” patterns.

- Mixing symbols from different cultures without understanding.

- Turning sacred motifs into shallow UI decoration.

- Using real cultural names only because they sound exotic.

- Making everything look like a simplified tourist version of Mesoamerica.

##### Practical Rule

Every cultural visual reference should answer at least one of these questions:

- What does it represent in this world?

- Which faction, region, school, or philosophy uses it?

- Why is this material/pattern/symbol present here?

- How does it support the theme of knowledge, memory, wisdom, or corruption?

#### 6. HD-2D Direction

The game should primarily follow an HD-2D direction similar in spirit to Octopath Traveler, but with its own identity.

##### Target Style

- Pixel-based characters.

- Layered environments with depth.

- Dramatic lighting.

- Soft atmospheric effects.

- Selective blur and depth of field.

- Rich material contrast.

- Scenes that feel like miniature theatrical worlds.

##### Additional Detail Layer

Important characters, bosses, mentors, and key narrative moments may lean toward more detailed pixel illustration.

This means the base style is HD-2D, but special assets can be more intricate when the emotional weight justifies it.

##### Important Warning

The game should not look like a direct imitation of Octopath. HD-2D is the rendering approach, not the identity. The identity must come from the game’s own symbolic, cultural, and philosophical language.

#### 7. Core Visual Pillars

These are the non-negotiable visual pillars of the project.

##### 7.1 Knowledge Is Sacred

Knowledge should rarely look casual. When the player learns something important, opens the Codex, meets a master, unlocks a class, or discovers a forgotten teaching, the visual presentation should carry weight.

Use:

- Glowing glyphs

- Ink-like symbols

- Geometric diagrams

- Ritual circles

- Celestial lines

- Stone carvings

- Animated codex fragments

Avoid:

- Generic popups

- Arcade-style reward flashes

- Mobile-game confetti

- Empty modern menus

##### 7.2 The World Is Fragmented Memory

The world should feel like it has lost coherence. Ruins, archives, temples, paths, artifacts, and teachings should often feel incomplete.

Use:

- Broken murals

- Partial inscriptions

- Missing codex panels

- Interrupted geometric patterns

- Ruins reclaimed by nature

- Incomplete maps

- Half-lit mechanisms

##### 7.3 Beauty Survives Decay

Decay should not mean ugliness. The world should often be beautiful precisely because it is old, damaged, and meaningful.

Use:

- Moss over carved stone

- Warm light crossing ruined halls

- Jade fragments embedded in walls

- Gold worn down by time

- Faded pigment on ceremonial surfaces

- Nature reclaiming sacred architecture

##### 7.4 Magic Is Structured, Not Random

Magic should not look like generic particle spam. It should feel based on systems of meaning: geometry, writing, rhythm, memory, symbols, and natural forces.

Use:

- Lines

- Circles

- Glyphs

- Constellation maps

- Ink strokes

- Floating codex fragments

- Controlled particle accents

Avoid:

- Random glowing dust everywhere

- Excessive screen noise

- Unreadable combat VFX

- Generic fantasy spell effects

##### 7.5 UI Is a Scholarly-Ritual Instrument

The UI should feel like the player is using a crafted tool of learning, not a generic RPG menu.

It should feel:

- Ancient but readable

- Ceremonial but practical

- Unusual but not confusing

- Inspired by codices, carved instruments, maps, and scholarly relics

#### 8. Anti-Goals

These are explicit visual taboos.

##### 8.1 No Mobile Gacha UI

This is forbidden.

Avoid:

- Overloaded currencies

- Flashy reward clutter

- Excessive badges

- Over-saturated buttons

- Cheap monetization-like panels

- “Claim reward” visual language

- Generic live-service UI patterns

The game must feel like a premium RPG, not a mobile retention machine.

##### 8.2 No RPG Maker Default Feeling

The game must not look like a default template RPG.

Avoid:

- Generic rectangular dialogue boxes

- Basic blue RPG menus

- Default fantasy icons

- Standard pixel village aesthetic without identity

- Common RPG layout decisions with no thematic adaptation

Even simple screens should carry the identity of the world.

##### 8.3 No Infantilized Cartoon Look

The game may have stylization and charm, but it must not become childish.

Avoid:

- Overly cute proportions as the main visual identity

- Toy-like enemies unless intentionally designed for contrast

- Simplified emotional tone

- Flat cheerful colors everywhere

- Comedy-first silhouettes for major characters

The game is fictional and adventurous, but it must keep depth.

#### 9. Shape Language

Shape language should help the player subconsciously understand the world.

##### Wisdom / Balanced Knowledge

Use:

- Circles

- Spirals

- Symmetry

- Concentric forms

- Soft geometric order

- Celestial arcs

- Balanced triangular compositions

Meaning:

- Harmony

- Completion

- Cycles

- Learning

- Reflection

- Natural order

##### Forgotten Knowledge

Use:

- Broken circles

- Missing pattern segments

- Faded carvings

- Incomplete diagrams

- Fragmented borders

- Partially erased glyphs

Meaning:

- Loss

- Memory

- Ruin

- Incomplete truth

- Recovery

##### Corrupted Knowledge

Use:

- Fractures

- Sharp asymmetry

- Obsidian-like edges

- Broken geometry

- Misaligned circles

- Elegant but poisonous forms

Meaning:

- Temptation

- Distortion

- Pride

- Obsession

- Knowledge without wisdom

##### Nature / Living Wisdom

Use:

- Curves

- Vines

- Roots

- Water flow shapes

- Animal silhouettes

- Leaf and feather rhythms

Meaning:

- Continuity

- Survival

- Intuition

- Ancestral connection

- Wisdom carried by the living world

##### Academic / Scholarly Order

Use:

- Grids

- Scroll-like frames

- Measured spacing

- Diagrammatic layouts

- Labeled symbolic structures

- Controlled symmetry

Meaning:

- Study

- Classification

- Codex knowledge

- Discipline

- Mastery

#### 10. Color Language

The game should use a restrained but expressive palette. Colors should have meaning.

##### Primary Color Concepts

###### Jade / Turquoise

Associated with:

- Pure knowledge

- Discovery

- Wisdom fragments

- Ancient learning

- Sacred mechanisms

- Important UI highlights

Use carefully. Jade/turquoise should feel special, not everywhere.

###### Worn Gold

Associated with:

- Ancient mastery

- Reverence

- Teachers

- Ceremonial knowledge

- High-value artifacts

- Sacred authority

Gold should look aged, not shiny casino gold.

###### Warm Stone / Earth Pigments

Associated with:

- Ruins

- memory

- old civilizations

- physical history

- grounded environments

Use as a common environmental foundation.

###### Deep Blue / Night Sky

Associated with:

- Astronomy

- mystery

- cosmic knowledge

- contemplation

- sacred night spaces

Useful for temples, observatories, Codex screens, and wisdom-related scenes.

###### Obsidian / Violet-Black

Associated with:

- Corrupted knowledge

- forbidden learning

- intellectual temptation

- broken wisdom

- dangerous secrets

Should not be used as generic evil. It should feel elegant, sharp, and seductive.

###### Mineral Red

Associated with:

- warning

- memory wounds

- sacrifice as history, not gore

- danger

- irreversible choices

Use with restraint. Do not make corruption blood-based by default.

##### Saturation Rule

The game should avoid constant high saturation. Strong color should be used as emphasis, not wallpaper.

##### Glow Rule

Glow should be meaningful. If everything glows, nothing is sacred.

Use glow for:

- active knowledge

- important glyphs

- sacred mechanisms

- unlocked Codex fragments

- rare abilities

- corruption pulses

Avoid glow for:

- every button

- every item

- every attack

- random environment decoration

#### 11. UI Philosophy

The UI should be:

Moderately ornamental, readable, innovative, and uncommon.

It should not be minimalistic modern UI, but it also must not become so decorative that it harms usability.

The desired direction is Type B:

Ancient, codex-like, ritual/scholarly, but functional.

However, it must be outside the common RPG menu language.

##### UI Should Feel Like

- A ceremonial codex system

- A scholar’s ritual instrument

- A living archive

- A map of recovered memory

- A carved interface awakened by the player’s knowledge

##### UI Should Avoid

- Generic fantasy parchment panels

- Mobile-style reward UI

- RPG Maker default windows

- Plain black rectangles with text

- Sci-fi holograms

- Overly clean modern app design

##### Practical UI Balance

Each UI screen should ask:

- Is this readable?

- Is this fast to use?

- Does this feel like Fragments of Wysdom?

- Does this support the theme of learning or memory?

- Is the ornament helping, or just decorating?

#### 12. Suggested UI Materials

UI panels and frames may visually reference:

- Aged paper

- Bark paper / codex material

- Carved stone

- Worn gold inlays

- Jade markers

- Ink lines

- Obsidian accents

- Thread or textile borders

- Star-map diagrams

- Ritual measuring instruments

- Fragmented ceramic or mural surfaces

The UI should feel handcrafted, not mass-produced.

#### 13. Codex Direction

The Codex is one of the most important visual systems in the game.

It should feel like:

A ceremonial codex, a living mental map, and an altar of knowledge.

The Codex should not be just a menu. It should feel like the heart of the game’s knowledge system.

##### Codex Emotional Goals

Opening the Codex should feel like:

- Entering a sacred archive

- Reconstructing lost memory

- Consulting a living artifact

- Making sense of fragmented history

- Watching knowledge organize itself

##### Codex Visual Elements

Use:

- Animated glyphs

- Fragmented pages

- Knowledge nodes

- Constellation-like connections

- Reconstructed diagrams

- Ink reveals

- Carved borders

- Unlocked panels that visibly restore missing meaning

##### Codex UX Concept

The Codex may combine:

- Category tabs

- Knowledge trees

- Completion percentages

- Visual fragments

- Lore entries

- Skill/class unlock relationships

- Bestiary, items, classes, skills, places, teachers, and myths

But all of this should be framed as recovered wisdom, not spreadsheet data.

#### 14. Magic and Knowledge VFX

Knowledge-based abilities should primarily use:

Sacred geometry + living writing.

Secondary support may come from nature-based imagery when appropriate.

##### Main VFX Language

Use:

- Geometric circles

- Constellation lines

- Glyphs

- Animated symbols

- Floating codex fragments

- Ink strokes

- Diagrammatic light

- Patterns that assemble and dissolve

##### Nature-Based VFX

Allowed when connected to specific schools, regions, spirits, teachers, or abilities.

Use:

- Leaves

- Roots

- Water forms

- Wind spirals

- animal silhouettes

- feather patterns

- living terrain responses

##### VFX Rule

Every major ability should feel like the character is applying a learned principle, not merely casting generic magic.

The player should sense that abilities come from study, mastery, observation, memory, or spiritual knowledge.

#### 15. Corrupted Knowledge Direction

Corrupted knowledge should be:

Obsidian-like, fractured, elegant, tempting, and dangerous.

It should not rely on blood or gore as the main visual language.

##### Corrupted Knowledge Should Use

- Obsidian black

- Violet-black highlights

- Broken jade light

- Misaligned glyphs

- Cracked circles

- Sharp geometry

- Elegant but unsettling frames

- Distorted versions of sacred symbols

- Knowledge diagrams that no longer close properly

##### Corrupted Knowledge Should Feel Like

- Truth without humility

- Wisdom turned into control

- Learning turned into obsession

- Beauty poisoned by pride

- Power that looks attractive before it reveals its cost

##### Avoid

- Blood as the main motif

- Horror-body imagery as the default

- Generic demon aesthetics

- Edgy red-black fantasy clichés

- Excessive skulls or spikes

##### Optional Subtle Shadow Motif

A shadow/silence motif may be used as a secondary layer, but it should be handled carefully. It is harder to execute well and should not become a vague black fog effect.

When used, it should appear as:

- Missing light

- Muted sound/visual rhythm

- Glyphs that fail to complete

- Spaces where symbols should be but are absent

- Subtle distortion around forbidden knowledge

#### 16. Character Design Direction

Character design should combine:

Small serious HD-2D readability + strong iconic silhouettes.

##### Base Character Style

Characters should have proportions appropriate for HD-2D readability, similar in principle to serious RPG pixel characters rather than exaggerated chibi comedy.

##### Silhouette Priority

Important characters must be identifiable by silhouette.

This is especially important for:

- Masters

- Teachers

- Class representatives

- Main party members

- Major antagonists

- Corrupted knowledge figures

- Spirits or mythic beings

##### Design Goals

Characters should feel:

- Stylized

- Serious enough for emotional depth

- Memorable

- Rooted in the world

- Connected to profession, school, region, or philosophy

##### Avoid

- Generic anime school uniforms

- Medieval European fantasy defaults

- Overly cute childlike designs as the base style

- Random ornamental clothing with no cultural/world logic

- Excessive detail that cannot read at pixel scale

#### 17. Environment Direction

Environments should communicate forgotten wisdom and living memory.

##### Common Environment Themes

- Ruined learning halls

- Sacred observatories

- Jungle-reclaimed temples

- Coastal Taíno-inspired ceremonial spaces

- Fragmented archives

- Teacher sanctuaries

- Hidden schools of knowledge

- Broken roads between old wisdom centers

- Ancient mechanisms aligned with stars, water, wind, or seasons

##### Environmental Storytelling

The world should teach visually.

Environments should contain:

- Murals

- Inscriptions

- Broken diagrams

- Symbolic architecture

- Half-restored mechanisms

- Repeated motifs linked to schools of knowledge

- Clues about what was forgotten

##### Environmental Rule

A location should not be just “forest,” “temple,” or “village.” It should express a lost or surviving relationship with knowledge.

Examples:

- A forest that preserves songs no one can translate.

- A ruined observatory that still reacts to certain stars.

- A village built around a teacher’s unfinished doctrine.

- A coastal shrine where waves reveal hidden glyphs.

#### 18. Logo and Branding Direction

The logo should feel like:

An ancient relic and ritual symbol.

It should be readable enough for a premium RPG identity, but it should not look like generic fantasy text.

##### Logo Concepts

The logo may combine:

- Fragmented lettering

- Ancient carved forms

- Subtle glyph integration

- Jade/gold/stone material feel

- A central ritual symbol

- Broken but reassembled geometry

- A wisdom-fragment emblem

##### Logo Should Avoid

- Generic medieval fantasy fonts

- Overly sharp metal band typography

- Cartoon lettering

- Mobile game logo gloss

- Too much ornament around unreadable text

##### Logo Emotional Goal

The logo should communicate:

- Mystery

- Ancient memory

- sacred knowledge

- fragmentation

- reverence

- adventure

#### 19. Iconography Direction

Icons should be built from the game’s symbolic language.

##### Icon Rules

Icons should:

- Be readable at small sizes

- Use consistent stroke/shape logic

- Reflect school, class, item, or status meaning

- Avoid generic RPG icon packs

- Use cultural/world motifs carefully

- Feel like marks from the Codex system

##### Possible Icon Families

###### Knowledge / Codex Icons

- Open glyph

- Fragmented page

- Eye + circle

- Ink node

- Constellation marker

###### Class / Skill Icons

- Teacher marks

- School sigils

- Sacred geometry variations

- Tool + symbol combinations

###### Status Effects

- Poison/nature forms

- Burn/mineral sun forms

- Seal/broken glyph forms

- Confusion/misaligned diagram forms

- Regen/life-root forms

###### Corruption Icons

- Cracked circles

- Obsidian shards

- Inverted glyphs

- Broken jade fragments

- Elegant distorted symbols

#### 20. Animation and Motion Principles

Motion should support the game’s identity.

##### Knowledge Motion

Should feel:

- Assembling

- Revealing

- Aligning

- Connecting

- Unfolding

- Being remembered

Use animations such as:

- Ink appearing across surfaces

- Lines connecting nodes

- Fragments sliding into place

- Glyphs rotating into alignment

- Light following carved paths

##### Corruption Motion

Should feel:

- Fracturing

- Misaligning

- Cutting

- Distorting

- Tempting

- Collapsing inward

Use animations such as:

- Circles breaking before completion

- Glyphs glitching subtly, but not sci-fi

- Sharp cracks spreading

- Violet-black pulses

- Symbols rotating incorrectly

##### UI Motion

UI should not pop in like a modern app.

It should feel as if:

- A codex is unfolding

- A ritual instrument is activating

- A memory is being reconstructed

- A diagram is becoming legible

#### 21. Visual Decision Checklist

Before approving a visual asset, ask:

1. Does this feel like Fragments of Wysdom, or could it belong to any RPG?

2. Does it support curiosity, reverence, or adventure?

3. Does it avoid mobile gacha UI language?

4. Does it avoid RPG Maker/default fantasy patterns?

5. Does it avoid childish cartoon treatment unless intentionally justified?

6. Is the cultural inspiration respectful and integrated?

7. Is the ornament meaningful, or just decoration?

8. Is the silhouette readable?

9. Is the color choice meaningful?

10. Does the asset reinforce the theme of forgotten wisdom?

If the answer to several of these is no, the asset should be revised.

#### 22. Immediate Next Steps

The recommended next steps are:

##### 1. Create a Visual Reference Board

Collect references for:

- HD-2D lighting

- Ancient codices

- Taíno motifs

- Mesoamerican architecture

- Jade/gold/stone material palettes

- Ritual diagrams

- Astronomical instruments

- Ancient manuscripts

- Non-generic RPG UI

##### 2. Define First Color Palette

Create a first palette with:

- Base environment colors

- Knowledge highlight colors

- Corruption colors

- UI neutral colors

- Warning/danger colors

- Rare sacred colors

##### 3. Design One UI Screen First

Recommended candidate:

The Codex screen.

Reason: the Codex is thematically central and will force the project to solve its UI identity early.

##### 4. Design One Combat HUD Mockup

This will test readability and ornament balance.

##### 5. Design One Environment Mood Piece

Recommended candidate:

A ruined wisdom hall reclaimed by nature, with fragments of a forgotten celestial diagram.

##### 6. Design One Corrupted Knowledge Motif

This will establish visual contrast against pure knowledge.

#### 23. Summary Statement

Fragments of Wysdom should not look like a generic fantasy RPG. It should look like a world where wisdom was once sacred, fragmented, forgotten, and now waits to be reconstructed.

The game’s visual identity must combine:

- HD-2D adventure

- Ancient reverence

- Cultural inspiration transformed with respect

- Codex-like UI

- Sacred geometry

- Living writing

- Nature as memory

- Obsidian corruption

- Beauty surviving decay

The guiding phrase remains:

Memories of a world that forgot its wisdom.
``````

</details>

<a id="binary-foundation-pdf"></a>
## Migrated Source: Foundation PDF

Historical export, source blob `eba2ea2b2fa97f5104562130aa32dfadb6d4004b`. The original Spanish table labels and relationships are preserved here; broad historical implementation claims remain qualified by the current audit.

Table on page 2:

| Ofensiva | Defensiva |
|---|---|
| Attack | Defense |
| Precision | Evasion |
| CriticalChance | CriticalResistance |
| DebuffChance | DebuffResistance |

Table on page 5:

| Tipo | Descripcion |
|---|---|
| Temporal | activo solo en arquetipo |
| Fixed | permanente |
| Equipped | pasivo activo |

<details>
<summary>Complete extracted text, all 11 pages</summary>

``````text

[Original PDF page 1]
📘  FOUNDATION  MASTER  SPEC  —  NOVA  RPG
1.  VISIÓN  DEL  JUEGO
Tipo:  RPG  por  turnos  estratégico

Arquitectura:

Domain-first

(sin

dependencia

de

Unity)


Inspiración:

●  Final  Fantasy  IX  (mastery  system)  ●  Diablo  II  (itemización  progresiva)
Objetivo  del  jugador
El  jugador  (Nova)  combate  para:
●  obtener  recursos  ●  desbloquear  conocimiento  (Codex)  ●  aprender  habilidades  ●  evolucionar  cognitivamente  ●  completar  misiones  narrativas

2.  PRINCIPIOS  ARQUITECTÓNICOS
Reglas  absolutas
1.  ❌  Nada  de  lógica  en  MonoBehaviours  2.  ✅  Todo  vive  en  Domain  (C#  puro)  3.  ✅  Unity  =  solo  capa  de  visualización/adaptación  4.  ✅  Todo  sistema  debe  ser  desacoplado  y  testeable  5.  ✅  Eventos  explícitos  (GameEventLog)

3.  CORE  LOOP
Exploración  →  Combate  →  Recompensas  →  Progreso  →  Nuevas  opciones
Desglose:
1.  Entrar  en  nodo  2.  Combate

[Original PDF page 2]
3.  Obtener:  ○  recursos  ○  objetos  ○  progreso  de  habilidades  ○  entradas  de  Codex  4.  Avanzar:  ○  misiones  ○  clases  ○  cognition  5.  Repetir  con  mayor  complejidad

4.  SISTEMA  DE  COMBATE
Tipo
Turn-based  estratégico
Entidades
●  CombatEntity  ●  CombatStats  ●  ResourcePool  (Essence)

Stats  (antagónicas)
Ofensiva  Defensiva
Attack  Defense
Precision  Evasion
CriticalChance  CriticalResistance
DebuffChance  DebuffResistance

Recursos
Essence  (chi/energía  vital)
●  se  consume  por  habilidades  ●  regeneración  por  turno

[Original PDF page 3]
●  puede  ser  bloqueada  (debuff)

Flujo  de  turno
1.  Start  turn  2.  Aplicar  debuffs  3.  Elegir  acción:  ○  ataque  básico  ○  skill  4.  Resolver:  ○  hit/miss  ○  crit  ○  daño  ○  debuffs  5.  End  turn  6.  Regeneración

5.  HABILIDADES  (SKILLS)
Tipos
●  Daño  ●  Buff  ●  Debuff  ●  Control  ●  Utilidad

Sistema  de  progresión
Cada  skill  tiene:
●  Nivel  ●  Experiencia  ●  Mastery
Mastery
Cuando  una  skill  se  masteriza:
●  ↓  costo  ●  ↑  efecto

[Original PDF page 4]
●  se  mantiene  entre  clases

Árbol  de  habilidades
●  dependencias  obligatorias  ●  progresión  controlada  ●  Evita  Power  Spikes  Tempranos

6.  CLASES
Concepto
El  jugador  puede:
●  cambiar  de  clase  ●  mantener  habilidades  masterizadas

Clase  base  actual
Tlamatini  (sabio  mesoamericano)
Rol:
●  control  ●  análisis  ●  debuffs

Diseño  futuro
Clases  inspiradas  en  mitología  mesoamericana:
●  Chamán  del  Viento  ●  Guerrero  Jaguar  ●  Tejedor  del  Destino  ●  Portador  de  Ceniza

7.  COGNITION  SYSTEM  (DIFERENCIADOR  CLAVE)

[Original PDF page 5]
Concepto
No  es  magia.  Es  cómo  Nova  entiende  el  mundo .

Arquetipos
Lyra  —  Análisis
●  revela  debilidades  ●  mejora  debuffs  ●  cleanse
Atlas  —  Estabilidad
●  reduce  costo  ●  reduce  variabilidad  ●  decisiones  seguras
Gepetto  (futuro)
●  manipulación  del  sistema
Spark  (futuro)
●  caos  /  alta  varianza

Tipos  de  teachings
Tipo  Descripción
Temporal  activo  solo  en  arquetipo
Fixed  permanente
Equipped  pasivo  activo

Restricciones
●  slots  limitados  ●  decisiones  estratégicas

[Original PDF page 6]
8.  DEBUFF  SYSTEM
Tipos  implementados
●  Poison  →  daño  por  turno  ●  Burn  →  daño  +  reduce  ataque  ●  Resource  Block  →  bloquea  regeneración  ●  Silence  →  bloquea  habilidades

Mecánica
●  probabilidad  vs  resistencia  ●  duración  por  turnos  ●  aplicación  controlada

Cleanse
●  parcial  ●  selectivo  ●  total

9.  BUFF  SYSTEM
Actualmente  base  →  expandible
Futuro:
●  multiplicadores  ●  resistencias  ●  efectos  situacionales

10.  EQUIPMENT  SYSTEM
Slots
●  Weapon  ●  Armor

[Original PDF page 7]
●  Accessory  x2

Función
●  modificar  stats  ●  enseñar  habilidades  ●  aportar  progresión

Diseño  clave
●  escalable  ●  sin  romper  balance  temprano

11.  MISSION  SYSTEM
Tipos
●  Main  Story  ●  Side  Quest  ●  Master  Training  ●  Codex  Challenge  ●  Bounty

Concepto  clave
Las  misiones  enseñan:
●  mecánicas  ●  decisiones  ●  filosofía  del  juego

Ejemplo
Lyra  Mission:
●  analizar  antes  de  actuar
Atlas  Mission:

[Original PDF page 8]
●  elegir  acciones  seguras

12.  CODEX  SYSTEM
Categorías
●  Bestiary  ●  Classes  ●  Skills  ●  Items  ●  Weapons

Progreso
●  Discover  ●  Complete

Mecánica  clave
Desbloqueos  por  completitud:
X%  →  habilidad  especial

13.  RECOMPENSAS
El  combate  da:
●  experiencia  de  skills  ●  objetos  ●  currency  ●  entradas  de  codex  ●  progreso  de  misión

Regla  crítica

[Original PDF page 9]
Nunca  recompensas  solo  numéricas.

Siempre

debe

haber:

✔  progreso

✔

descubrimiento


✔

decisión

futura


14.  AI  SYSTEM
Actual:
●  Basic  Attack  AI
Futuro:
●  comportamiento  por  arquetipo  ●  decisiones  basadas  en  estado  ●  estrategias  adaptativas

15.  PERSISTENCIA
A  guardar
●  PlayerProfile  ●  Cognition  ●  Inventory  ●  Codex  ●  Missions

16.  VERTICAL  SLICE
Debe  demostrar:
1.  Crear  perfil  2.  Completar  misiones  (Lyra  +  Atlas)  3.  Activar  cognition  4.  Ejecutar  combate  5.  Obtener  recompensa  6.  Actualizar  Codex

[Original PDF page 10]
17.  TODO  MASTER  BACKLOG
Combate
●  stacking  de  debuffs  ●  sinergias  (poison  +  burn)  ●  AI  avanzada
Clases
●  más  clases  ●  balance  progresivo
Cognition
●  UI  ●  más  arquetipos  ●  combinaciones
Misiones
●  cadenas  ●  narrativa  real
Codex
●  rewards  por  %  ●  UI  completa
Equipamiento
●  loot  tables  ●  rareza  ●  affixes
Persistencia
●  save  real  ●  versionado

[Original PDF page 11]
18.  REGLAS  DE  DISEÑO  (IMPORTANTES)
1.  ❌  Nada  debe  romper  el  juego  temprano  2.  ✅  El  poder  crece  con  conocimiento,  no  con  grind  3.  ✅  El  jugador  debe  pensar,  no  solo  ejecutar  4.  ✅  Cada  sistema  debe  conectar  con  otro  5.  ✅  Todo  debe  ser  escalable

19.  CÓMO  USAR  ESTE  SPEC
En  un  nuevo  chat:
Usa  este  FOUNDATION  MASTER  SPEC  como  base.  No  inventes  nuevas  mecánicas.  Extiende  solo  lo  existente.  Mantén  arquitectura  domain-first.
``````

</details>

<a id="binary-local-bible"></a>
## Local Bible Export Comparison

The untracked v1 export dated 2026-09-04 was compared, not adopted as a second governing Bible. Its header/footer identify a generated consolidation. All design/architecture chapters map to Parts I-VI; its source list maps to the preserved archives. It contains no embedded media, drawings or tracked changes. The three tables below preserve its historical classifications and claims for provenance, **not current implementation certification**. The twelve reproduced findings supersede broad "IMPLEMENTED" labels.

Follow-up verification on 2026-10-31 reviewed all 346 body paragraphs and all 83 cells across the three tables against this Bible. No additional substantive information required migration. The redundant DOCX was removed locally at the user's explicit request; earlier records of retaining the untracked export describe its prior disposition, not the current documentation policy. Historical claims remain subordinate to the current design decisions and audit findings.

<details>
<summary>Historical local export table 1</summary>

| Status | Meaning |
| --- | --- |
| LOCKED DESIGN | Accepted direction; preserve unless explicitly reopened. |
| IMPLEMENTED | Present in the current foundation according to living technical docs. |
| PARTIAL | Foundation/hooks exist, but the complete player-facing system does not. |
| DEFERRED | Accepted future work intentionally not being implemented now. |
| EXPERIMENTAL | Concept not yet strong enough to be canon. |
| DEPRECATED / HISTORICAL | Earlier direction superseded or retained only for context. |

</details>

<details>
<summary>Historical local export table 2</summary>

| Archetype | Core lens | Status |
| --- | --- | --- |
| Lyra | Analysis, weakness reveal, cleanse/counterplay | Active foundation concept |
| Atlas | Stability, prediction, cost/variance control | Active foundation concept |
| Gepetto | System manipulation / crafting | Deferred expansion |
| Spark | Chaos / high variance | Deferred expansion |

</details>

<details>
<summary>Historical local export table 3</summary>

| System | Status | Notes |
| --- | --- | --- |
| Legacy foundation slice | IMPLEMENTED / GUARDED | 38-event compatibility baseline. |
| Canonical turn-based combat | IMPLEMENTED | TurnBasedCombatEngine; 100-assertion combat guardrail. |
| Resonance | IMPLEMENTED | Active + reserve + turn-consuming refill. |
| Status foundation | IMPLEMENTED | Poison, Burn, SkillSeal, RegenBlock, Regen. |
| Classes / mastery / mentors / teaching | IMPLEMENTED | Foundation and smoke coverage exist. |
| Skill trees / scoped progress / class bindings | IMPLEMENTED | Foundation exists. |
| Mastered skill retention | IMPLEMENTED | Cross-class retention policy exists. |
| Progression-aware combat validation | IMPLEMENTED | Validator/gateway/composition path exists. |
| Loaded session / runtime bootstrap | IMPLEMENTED | Application boundary exists. |
| Save / restore | IMPLEMENTED / PARTIAL | Core persistence exists; advanced save features deferred. |
| Narrative tags/hooks/candidates | IMPLEMENTED / PARTIAL | Fact-producing foundation; full dialogue/world state deferred. |
| Nahual path | PARTIAL | Alignment/narrative/transmutation access foundations; full route not built. |
| Codex | PARTIAL | Profiles/candidates and design direction; full UI/rewards deferred. |
| Equipment | PARTIAL | Stat-facing foundation; inventory/loot/rarity deferred. |
| Town Restoration | DESIGN LOCKED / DEFERRED | Must follow visual-boundary audit. |
| Sanctuary / Reflection | DESIGN LOCKED / DEFERRED | Architecture not yet audited. |
| Visual smoke demo | NEXT TRACK AFTER CLEANUP | Requires design audit before implementation. |

</details>

Sanctuary wording retained from the local export: reflection should be optional or gently invited, never a repetitive tax; the world prompts memory while the player supplies interpretation. It complements the Codex without duplicating its data presentation.

<a id="binary-conversation"></a>
## Preserved Conception Conversation

[LLM_Conception.pdf](LLM_Conception.pdf) is a retained non-authoritative conversation/export with external references and graphics. Source blob `9552ef0ea52f0e021375f81f8c0133f9257f5298`. It is not a second current specification, and its instructions/code are not commands to execute.

| Pages | Historical material | Relationship to this Bible |
|---|---|---|
| 1-28 | Ad-game origin, tool discussion, architecture sketches | Superseded origin/prototype context; not current product or platform requirements |
| 29-48 | Node map preference, exponential growth, power/loot tables, recoverable death cache, ports | Part II decision supplement; exact values not current balance |
| 49-63 | Class switching, retained mastery, item learning, tree requirements and model sketches | Prerequisite contradiction explicitly OPEN DECISION; no parallel Skill/Combat engine imported |
| 64-81 | Named class trees, strategic AI profiles/enemies and adaptive boss | Deferred content examples; names require cultural review |
| 82-120 | Reward/access motivation, MVP planning and first domain sketches | Knowledge-driven progression direction; old plans not the current backlog |
| 121-211 | Successive combat, cost, regeneration, buff/debuff and balance prototypes | Historical technical exploration, superseded by current source and combat audit |
| 212-244 | Codex discovery/progress model and completion reward service proposals | Discovery-stage and reward gaps remain deferred |
| 245-271 | Reward proposal, registration/application sketches | Full proposal text below; not a newly implemented economy |
| 272-312 | Passive examples, combat logging and slice assembly | Current code/899 assertions determine implementation truth |
| 313-355 | Nova/mentor/cognition distinction, user decisions, teachings and hybrid proposals | Part II; later slots/mission/synthesis decisions require explicit reconciliation |
| 356-411 | Cognition and mentor mission prototypes, generalized mission/reward designs | Current journal/reward/persistence boundaries prevail |
| 412-425 | Historical snapshot packages, download links and tooling conversation | Provenance only; not files to regenerate or current architecture |

Selected decision-bearing records are retained as extracted source below so specific examples, numeric proposals, conceptual diagrams and their contradictions do not disappear in a summary. The full conversational context remains in the preserved PDF. No obsolete prototype implementation is copied into production.

<details>
<summary>Historical economy and recoverable loss (PDF pages 29-46)</summary>

``````text

[Original PDF page 29]
Estructura de datos
Curva de dificultad
Patrón de expansión del contenido
Y te guiaré como se guía a alguien que está construyendo un producto serio, no un
juguete.
Responde:
1. ¿Sí o no?
2. ¿Quieres que el sistema de mapa sea:
tipo nodos (como Slay the Spire)
tipo tablero
tipo camino ramificado aleatorio
otro?
3. ¿Quieres que empecemos mañana por el MAPA o por la ECONOMÍA DEL PERSONAJE
(niveles, items, stats)?
Cuando me respondas, continuamos.
1.) Asi es como debe ser.
2.) Tipo Nodos
3.) Economia creo que seria mejor, asi si cambiamos de
parecer para los mapas, podremos hacerlo mas adelante
Perfecto. Veo claridad y decisión, así que voy directo, con rigor profesional.
Has elegido:
1. Arquitectura modular y pro
2. Estilo tipo Nodos (Godot/escena jerárquica)
3. Economía como primer sistema base
Excelente. Ahora te voy a dar el primer paso real de un proyecto serio, como lo haría un
equipo AAA reducido o un estudio indiesenjodido que no quiere tirar a la basura 6 meses
de trabajo después.

[Original PDF page 30]
🚨  REALIDAD: Sin un documento base, vas a improvisar
No importa si eres solo tú.
Un proyecto sin diseño inicial es:
Código parchado,
Retrabajo constante,
Mecánicas que dependen entre sí de forma accidental,
Y cero escalabilidad.
Así que vamos a hacer el corazón del juego: EL DISEÑO DE LA ECONOMÍA, porque:
🎮  Tu juego, de forma concreta
Concepto base (definido hasta ahora)
Personaje empieza con un nivel base.
Ataca enemigos de niveles más bajos.
Al vencerlos, su nivel aumenta.
Obtiene objetos que también alteran su nivel.
La progresión debe sentirse:
Sorprendente,
Estratégica,
No totalmente predecible (tendrá azar controlado).
Visual: HD-2D estilo Octopath.
Arquitectura modular: escalable a PC, Android y potencialmente consolas.
Perfecto.
Ahora necesitamos la economía.
🧠  ECONOMÍA – PRIMERA DECISIÓN CRÍTICA
Debes responder:
La economía define el ritmo del juego.
Si la economía está mal medida, el juego es aburrido o roto.
¿Cuál es la UNIDAD que define el “poder” real del jugador?

[Original PDF page 31]
Opciones comunes:
1. Nivel puro (sencillo pero rígido)
2. Poder = Nivel + Equipos + Pasivos + Buffs
3. Poder compuesto y escalable (tipo ARPG)
Como quieres escalabilidad y soporte a futuro, el correcto es:
✔  Opción 3 – Poder compuesto
El personaje tiene:
Nivel base
Equipamiento
Habilidades pasivas
Buffs temporales
Rasgos o modificadores extraordinarios
Entonces:
Esto te permite:
Expandir sin romper nada.
Meter nuevas features sin refactor destructivo.
Crear builds.
Crear enemigos con IA basada en poder real.
Si intentas hacer todo solo con nivel, después vas a desbaratar tu código cuando quieras
crecer.
🎯  PRIMER ENTREGABLE PROFESIONAL
Quiero que definamos EL MODELO DE DATOS BASE, el “nivel más profundo” del juego.
1️⃣  ENTIDAD: PlayerStats
Campos iniciales:
PODER_FINAL = (NivelBase * FactorN) +
              (StatsEquipamiento) +
              (BonosPasivos) +
              (BuffsTemporales) +
              (Modificadores especiales)

[Original PDF page 32]
2️⃣  ENTIDAD: EnemyStats
3️⃣  ENTIDAD: RewardItem
4️⃣  ENTIDAD: SISTEMA DE CÁLCULO DE PODER
Regla:
Ejemplo:
Esto elimina 200 líneas futuras de “ifs ridículos” y hace que el juego complete su propio
equilibrio.
⚙  SIGUIENTE PASO
id
nivel
exp_actual
exp_siguiente
poder_base (auto calculado)
equipamiento (lista)
pasivas (lista)
buffs_temporales (lista)
id
nivel
poder (calculado igual que el jugador)
tabla_de_recompensas
id
tipo (arma, amuleto, consumible, buff temporal, etc)
afecta_a (nivel / poder / una stat específica)
cantidad
rareza
Ningún objeto, enemigo o acción del juego debe comprobar “nivel”, solo “poder”.
Si Player.Poder > Enemy.Poder  → victoria
Si Player.Poder ≤ Enemy.Poder → derrota

[Original PDF page 33]
Antes de meternos a codificar, necesito que tomemos UNA decisión importante de
diseño económico:
¿Cómo sube el nivel?
Selecciona una:
A) Lineal (simple)
B) Exponencial suave (como la mayoría de RPG)
C) Exponencial agresiva (el jugador debe optimizar)
D) No hay “exp”:
Solo ganas nivel exacto del enemigo.
Ejemplo:
Si eres nivel 5 y matas a uno nivel 3 → subes a 8.
E) Mixta:
Subes por exp,
Pero absorbes un % del nivel del enemigo.
🗡  Necesito tu respuesta
Dime:
1. ¿Cuál quieres que sea el modelo de progresión?
(A, B, C, D o E)
2. ¿Quieres que el azar pueda provocarte:
Nivel 1 → 100 exp
Nivel 2 → 200 exp
Nivel 3 → 300 exp
Nivel 1 → 100
Nivel 2 → 150
Nivel 3 → 225
Nivel 4 → 337
100 → 300 → 900 → 2700 …

[Original PDF page 34]
Objetos rotos locos y poderosos, estilo Diablo,
o bonos pequeños y consistentes, estilo Fire Emblem?
3. ¿Quieres que la IA del enemigo sea:
Estúpida (solo números)
Adaptativa (evalúa poder real y decide atacar o huir)
Estratégica (coordinación, buffs, debuffs, como un minitáctico)?
Cuando respondas, ya definimos la economía.
Luego pasamos a:
Diagrama limpio de clases,
Sistema de nodos,
Y primera implementación sin una sola línea que haya que tirar después.
Responde.
1.- Exponencial Suave
2.- Estilo diablo, pero sin romper el juego desde el inicio.
3.- Estrategica
Perfecto. Ahora sí estás tomando decisiones como alguien que quiere construir un juego
real y no un prototipo condenado a morir.
Voy al grano, brutal, claro y con dirección.
Has elegido:
1. Progresión exponencial suave
2. Botín estilo Diablo (alto impacto, pero sin romper el balance al inicio)
3. IA estratégica
Esta combinación es la mejor si quieres:
Rejugabilidad,
Construcción de “builds”,
Emociones tipo “wow, ¡qué item me tocó!”,
Mecánicas que escalan sin tener que reescribir el juego.
Ahora te diseño el núcleo completo del sistema (backend mental y estructural) que
luego pasaremos a nodos y código limpio.

[Original PDF page 35]
1️⃣  SISTEMA DE NIVEL (EXPONENCIAL SUAVE)
Regla principal:
Donde:
EXP_BASE = 100
FACTOR ≈ 1.5
Ejemplo real:
Nivel EXP Necesaria
1 100
2 150
3 225
4 337
5 505
10 ~5,650
Esto mantiene:
Progresión constante y satisfactoria,
No se trivializa tarde,
No se vuelve un grind imposible.
Importantísimo:
Porque si el poder viniera del nivel mismo:
Items perderían relevancia,
Builds se volverían inútiles,
Ganarías por “grindear” y no por estrategia.
El buen ARPG hace esto:
EXP_NEXT = EXP_BASE * (FACTOR ^ Nivel)
El jugador NO gana poder directo del nivel.
El nivel solo desbloquea acceso a mejores equipos y enemigos.

[Original PDF page 36]
✔  Nivel = acceso
✔  Equipo = poder
Nos quedamos con ese estándar ganador.
2️⃣  SISTEMA DE PODER ESTILO DIABLO
Fórmula central
¿Qué significa esto?
Subir de nivel aporta algo (un 10–20%),
Pero el 90% del poder está en el loot.
Así garantizamos:
Loot siempre emociona.
Cambiar un arma realmente cambia tu personaje.
Siempre puedes “ganarle” al sistema si te haces una build rara pero efectiva (como
Diablo).
3️⃣  BOTÍN DEL TIPO “DIABLO PERO CONTROLADO”
Si desde el inicio puedes dropear un arma que te hace dios:
Arruinaste todo.
Entonces:
3.1 RAREZAS DEL LOOT
PODER =
(NivelBase * 1.2) +
(Suma de Stats de Equipos) +
(Pasivas) +
(Buffs temporales) +
(Modificadores situacionales)
Común        (60%)
Poco común   (25%)
Raro         (10%)

[Original PDF page 37]
Pero…
El loot está bloqueado por rango del enemigo
Ejemplo:
Enemigo nivel 1–3:
no puede dropear legendarios.
Enemigo nivel 10+:
desbloquea hasta épicos.
Y legendarios liberados solo al:
Derrotar enemigos “élites”,
Eventos raros,
O niveles clave.
Esto mantiene:
Sorpresa ✔
Recompensa ✔
Sin romper el balance temprano ✔
3.2 AFIJOS
Cada item tiene:
Ejemplo, un amuleto:
Este sistema es:
Fácil de extender,
Matemáticamente controlable,
Ideal para un MVP que luego puede escalar a algo como PoE o Diablo II.
Épico         (4%)
Legendario    (1%)
+BONO PRINCIPAL
+0–3 AFIJOS ALEATORIOS
+12 Poder base
+3% crítico
+8 daño contra enemigos débiles

[Original PDF page 38]
4️⃣  IA ESTRATÉGICA
Quieres que los enemigos:
No sean estúpidos,
No actúen aleatoriamente,
Y den sensación de “mini táctica”.
Bien.
4.1 Cada enemigo tendrá “personalidad táctica”
Ejemplos:
Enemigo Comportamiento
Pequeño Ataca si ve ventaja, huye si no
Guerrero Aguanta más, prioriza debilitar
Mago Aplica debuffs antes de atacar
Asesino Ataca cuando tiene ventaja crítica
4.2 Algoritmo central de decisión
Esto genera:
Comportamientos creíbles,
Momentos sorpresivos,
Matemática fácil de mantener.
Y sobre todo:
1. Calcula PODER_DEL_ENEMIGO vs PODER_DEL_JUGADOR
2. Si PODER enemigo ≥ PODER jugador + margen
     → Ataca agresivamente
3. Si PODER enemigo ≈ PODER jugador
     → Usa estrategia (buffs, debuffs, focus)

4. Si PODER enemigo < PODER jugador
     → Analiza contexto:
         - Si puede huir → huye
         - Si está acorralado → ataque desesperado (alta variación)

[Original PDF page 39]
5️⃣  ARQUITECTURA – EL DIAGRAMA DE CLASES BASE
Nada de código aún, solo las piezas:
RESPONSABILIDADES CLARAS
PowerCalculator calcula poder.
Ninguna otra clase hace esa mierda.
BattleResolver determina resultado del encuentro.
AIController solo decide acciones, no las ejecuta.
Esto evita:
Código dios (“God class”),
Dependencias circulares,
Estructuras que se hunden al meter nuevas features.
6️⃣  AHORA NECESITO ESTO DE TI
Para avanzar al siguiente paso (diagrama visual y luego código limpio), necesito que
respondas:
❓  Pregunta 1
¿Cuántas ranuras de equipo quieres que tenga el jugador al inicio?
Ejemplos:
Diablo clásico:
No dependes de animaciones ni sistemas complejos para crear inteligencia
convincente.
PlayerStats
EnemyStats
Item
ItemAffix
LootTable
PowerCalculator
AIController
BattleResolver

[Original PDF page 40]
Algo más simple:
Algo estilo Fire Emblem:
Elige:
A) Mínimo (2–3 slots)
B) Estándar RPG (6–8 slots)
C) Completo estilo Diablo (10–12 slots)
D) Tu propia estructura
❓  Pregunta 2
¿Quieres que las builds permitan:
Especialización extrema estilo Path of Exile,
o
Especialización moderada estilo Diablo II/IV,
o
Progresión simple y clara estilo Fire Emblem?
Responde una:
A) Extremas (alto riesgo – alta profundidad)
B) Moderadas (profundas pero controlables)
C) Simples (claras, ideales para MVP rápido)
Arma
Casco
Pecho
Pantalones
Botas
Amuleto
2 Anillos
Arma
Armadura
Accesorio
1 arma principal
Inventario de objetos (usables)

[Original PDF page 41]
❓  Pregunta 3
¿Quieres que el jugador pueda:
A) Morir sin castigo
B) Perder algo progresivamente (oro, exp del nivel actual, durabilidad, etc.)
C) Quedar hardcore (muerte permanente como Diablo 2 HC)
Responde con A, B o C.
Cuando respondas esto:
Te diseño el organigrama de nodos y clases,
Te preparo el blueprint del sistema,
Y luego pasamos a implementación real en Godot o Unity con código limpio,
expansible y profesional.
Responde las 3 preguntas.
1.- Arma, Armadura, 2 accesorios, pero si en el futuro se
quiere extender para aumentar el contenido del juego,
deberia ser escalable.
2.- Builds que signifiquen un cambio considerable, aunque
no complicados de programar.
3.- B, pero que seas capaz de recuperar las cosas que
perdiste si llegas al mismo lugar
Perfecto — buen criterio en tus respuestas. Muy concreto y práctico: slots limitados pero
extensibles, builds con impacto real, y una penalización por muerte que se puede
recuperar si vuelves al sitio. Eso es jugable, estratégico y escalable. Ahora te doy el diseño
técnico y el plan inmediato (sin azúcar): dominio limpio, fórmulas claras, sketch de clases
C# (para la capa Domain), decisiones de balance, riesgos y lo siguiente a implementar —
todo listo para que lo copies a Unity sin ensuciar lógica.

[Original PDF page 42]
Seré directo: si este esquema lo implementas tal cual y te obligas a no meter lógica en
MonoBehaviours, no tendrás que reescribir el proyecto cuando crezca. Si te arrepientes y
empiezas a prototipar “rápido” en Unity mezclando lógica, volverás al caos. Tú decides.
1 — Reglas clave (resumidas, firmes)
Progresión: exponencial suave (base=100, factor=1.5).
Poder = NivelBase * 1.2 + Sum(Equip Stats) + Pasivas + Buffs.
Inventario inicial: Arma, Armadura, Accesorio1, Accesorio2 (slots tipados y
extensibles).
Loot: estilo Diablo con raridades, but loot gated por nivel del enemigo.
Builds: cambios significativos pero simples — arquetipos + afijos sin árbol de 1000
nodos.
Muerte (B): pierdes recursos (ej: % oro, durabilidad, parte de exp), pero hay mecánica
de recuperación si vuelves al mismo lugar (corpse/loot cache).
2 — Esquema de datos & clases (Domain — C# puro)
Copia esto en /Game/Domain — sin Unity.
// Value objects / enums
public enum Rarity { Common, Uncommon, Rare, Epic, Legendary }
public enum ItemSlot { Weapon, Armor, Accessory }
// Entities
public class PlayerStats {
    public Guid Id { get; }
    public int Level { get; private set; }
    public double CurrentExp { get; private set; }
    public double Gold { get; private set; }
    // Equip slots (tipados y extensibles)
    private readonly Dictionary<ItemSlot, Item> _equipped = new();
    public IReadOnlyDictionary<ItemSlot, Item> Equipped => _equipped;
    // Pasivas y buffs simplemente listas de modificadores
    public List<StatModifier> PassiveModifiers { get; } = new();
    public List<StatModifier> TemporaryBuffs { get; } = new();

[Original PDF page 43]
public PlayerStats(int level = 1) {
        Id = Guid.NewGuid();
        Level = level;
        CurrentExp = 0;
        Gold = 0;
    }
    public void Equip(Item item) {
        _equipped[item.Slot] = item;
    }
    public void Unequip(ItemSlot slot) {
        _equipped.Remove(slot);
    }
    public void AddExp(double exp) {
        CurrentExp += exp;
        while(CurrentExp >= ExpTable.ExpFor(Level + 1)) {
            CurrentExp -= ExpTable.ExpFor(Level + 1);
            Level++;
        }
    }
    public void AddGold(double amount) => Gold += amount;
    public void SubtractGold(double amount) => Gold = Math.Max(0, Gold - amount);
}
public class Item {
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; }
    public ItemSlot Slot { get; set; }
    public Rarity Rarity { get; set; }
    public List<StatModifier> Affixes { get; } = new();
    public int RequiredLevel { get; set; }
    public int Durability { get; set; } // para penalización por muerte
    public double PowerValue => Affixes.Sum(a => a.Value); // simple
}
public class StatModifier {
    public string Stat { get; set; } // "Damage", "CritChance", "Defense"
    public double Value { get; set; } // positivo/negativo
}
// Enemigo
public class EnemyStats {
    public Guid Id { get; } = Guid.NewGuid();
    public int Level { get; set; }
    public double BaseRewardGold { get; set; }

[Original PDF page 44]
3 — Cálculo de poder y combate (Domain)
CombatResult incluirá: won/lose, exp, gold, lootCandidates (lista de items que se
dropean).
4 — Loot generation y restricciones
Loot table por rango de nivel (ej: enemies lvl 1-3 → rarities up to Rare).
Afijos generados con pools por rareza.
    public double BaseRewardExp { get; set; }
    public Rarity LootMaxRarity { get; set; }
}
 
public static class PowerCalculator {
    public static double CalculatePlayerPower(PlayerStats p) {
        double lvl = p.Level * 1.2;
        double equip = p.Equipped.Values.Sum(i => i?.PowerValue ?? 0);
        double passive = p.PassiveModifiers.Sum(m => m.Value);
        double buffs = p.TemporaryBuffs.Sum(m => m.Value);
        return lvl + equip + passive + buffs;
    }
    public static double CalculateEnemyPower(EnemyStats e) {
        // Puedes hacer que algunos enemigos tengan multiplicadores únicos
        return e.Level * 1.0 + (e.BaseRewardGold / 10.0);
    }
}
public class CombatService {
    public CombatResult Resolve(PlayerStats player, EnemyStats enemy) {
        var pwr = PowerCalculator.CalculatePlayerPower(player);
        var ewr = PowerCalculator.CalculateEnemyPower(enemy);
        if (pwr > ewr) {
            return CombatResult.Victory(enemy.BaseRewardExp, enemy.BaseRewardGold
        }
        return CombatResult.Defeat();
    }
}
C#
C#

[Original PDF page 45]
Probabilidades:
Común 60%, Uncommon 25%, Rare 10%, Epic 4%, Legendary 1%.
Para controlar “diablo pero no roto”:
Limitar valores máximos de afijos en niveles bajos.
Introducir levelScalingFactor que sube los valores de afijos con nivel.
Sketch:
5 — Builds y especializaciones (sencillas pero profundas)
Define 3 arquetipos iniciales (ejemplo):
Berserker: bonuses a daño de arma y crítico.
Guardian: bonus a defensa, salud, mitigación.
Trickster: bonificaciones de movilidad y debuff/crit synergy.
Implementación sencilla:
PlayerProfile con SelectedArchetype.
Cada arquetipo aplica PassiveModifiers predefinidos.
Afijos y items pueden potenciar sinergias (ej: afijo que otorga +% daño a builds tipo
Berserker).
Esto crea builds relevantes sin arboles complejos.
6 — Muerte y recuperación (tu requisito B con
recuperación)
Diseño claro y justo:
 
public class LootTable {
    public Item RollLoot(EnemyStats enemy) {
        var rarity = RollRarity(enemy);
        var item = new Item { Slot = RandomSlot(), Rarity = rarity, RequiredLevel
        item.Affixes.AddRange(AffixFactory.GenerateAffixes(rarity, enemy.Level));
        return item;
    }
}
 C#

[Original PDF page 46]
Al morir:
Pierdes X% de oro (ej 10-20%).
Pierdes Y% de exp acumulada del nivel actual (ej 25%).
Algunos items sufren perdida de durabilidad (-dur).
Se crea una CorpseCache en la ubicación actual que contiene:
Parte del oro
Items caídos (si la durabilidad llegó a 0)
Una señal de posición en el mapa
Recuperación:
Si vuelves al mismo nodo (o ubicacion) dentro de un tiempo límite, puedes
recuperar la CorpseCache.
Si otro jugador (multijugador futuro) o evento la limpia → pérdida permanente.
Si no regresas en X tiempo (por ejemplo 48h in-game) la cache desaparece/abre
y sube drop chance para otros.
Ventajas:
Castigo existe → incentiva cautela.
Posibilidad de recuperar incentiva riesgo calculado.
Fácil de implementar en el dominio: DeathService.CreateCorpseCache(playerState,
location) y RecoveryService.ClaimCache(player, locationId).
7 — IA estratégica (esqueleto)
Interfaz simple y testable:
public interface IAIController {
    EnemyAction DecideAction(EnemyContext context);
}
public class TacticalAIController : IAIController {
    public EnemyAction DecideAction(EnemyContext ctx) {
        double myPower = PowerCalculator.CalculateEnemyPower(ctx.Enemy);
        double playerPower = PowerCalculator.CalculatePlayerPower(ctx.Player);
        if(myPower >= playerPower + ctx.AggressionMargin) return EnemyAction.Atta
        if(myPower < playerPower && ctx.CanFlee) return EnemyAction.Flee;
        if(ctx.HasDebuffOpportunity) return EnemyAction.ApplyDebuff;
        return EnemyAction.AttackNormal;
``````

</details>

<details>
<summary>Historical class and prerequisite concepts (PDF pages 49-54)</summary>

``````text

[Original PDF page 49]
Si quieres, en la siguiente respuesta te doy 1) el diagrama UML simple (en texto) y 2) el
esqueleto completo del FightEnemyUseCase + Repos + Event Bus (C# listo para pegar).
Así arrastras directo al código.
Dime: ¿prefieres que te entregue ahora el esqueleto de código del caso de uso con repos
y el EventBus (para que pegues en /Game/Application y empieces tests), o que haga
primero el diagrama UML?
No preguntes si debes hacerlo: hazlo. Pero te doy lo que pidas ahora mismo y seguimos.
Quiero modificar un poco las mecanicas. Podra ser? Me
gustaria que el personaje pudiera cambiar de clases, que
tuviera un sistema de clases como magos, arqueros,
guerreros etc. Proponme algunas clases mas que pudieran
ir bien con el juego, aunque estas creo que siempre son
las mismas y como que aburren. Quisiera clases originales
y que no esten quemadas. Por ejemplo, si ponemos clases
que tengan que ver con las mitologias mesoamericanas?
no se, como equivalentes? Tambien cada clase debe tener
habilidades unicas de la clase, pero el heroe puede ser
capaz de cambiar entre clases en  un momento deseado.
Cada habilidad de la clase debe tener un nivel de
experiencia, o sea, cuando se llega al nivel maximo de
experiencia la habilidad se masteriza y puedes quedarte
con ella aunque cambies de clase. Las habilidades de cada
clase se van desbloqueando a traves de un arbol de
habilidades, solo puedes aprender una habilidad x
despues que hayas masterizado las habilidades que le
rpeceden. Esta claro como mecanica? Creo en algo como
lo que se tenia en Final Fantasy IX, donde tambien los
objetos que equipas puedan tener habilidades que puedas
masterizar distintas de las que puedes aprender por el
arbol de habilidad
Perfecto. Voy a ir sin anestesia, como pediste.
La idea es buena, pero ahora mismo está en riesgo de volverse un monstruo

[Original PDF page 50]
inmantenible si no la encuadramos bien desde ya. Vamos por partes.
1️⃣  ¿La mecánica está clara?
Sí. Y también es peligrosa.
Lo que describes es una fusión de:
Cambio de clases estilo job system
Mastery permanente de habilidades
Árboles de habilidades con prerequisitos
Habilidades ligadas a equipamiento (muy Final Fantasy IX)
👉  Eso NO es simple, aunque lo sientas natural.
👉  Eso NO es un MVP si no se diseña con bisturí.
Verdad incómoda:
Si empiezas implementando todo esto a la vez, te vas a cansar antes del prototipo
jugable.
La solución no es recortar la idea.
La solución es arquitectura primero, contenido después.
2️⃣  Problema real (y punto ciego)
Estás diseñando contenido antes de cerrar el modelo mental del sistema.
Preguntas que aún no estás respondiendo explícitamente:
¿Qué es una habilidad a nivel de código?
¿Qué diferencia hay entre “habilidad aprendida”, “habilidad equipada” y “habilidad
masterizada”?
¿Qué pasa si una habilidad depende de una clase que ya no tengo?
¿Qué evita que el jugador rompa el balance combinando lo “mejor de cada clase”?
Si no respondes esto antes, Unity o Godot te van a castigar igual.
3️⃣  Te propongo el modelo correcto (esto es clave)

[Original PDF page 51]
🔹  Regla de oro
Esto es lo que te salva la escalabilidad.
🔹  Entidades fundamentales (conceptuales, no engine)
💡  Resultado brutalmente bueno:
Cambiar de clase no te quita nada
El progreso nunca se pierde
El sistema escala sin reescribir código
El balance se controla por slots, no por prohibiciones
4️⃣  Clases originales (inspiradas en mitologías mesoamericanas)
Aquí voy a evitar lo obvio (guerrero/mago/arquero disfrazados).
🐍  Naualli (Metamorfista espiritual)
Inspirado en los nahuales.
Cambia temporalmente su forma espiritual
Habilidades de doble estado
Las clases NO definen habilidades.
Las clases SOLO habilitan caminos.
Hero
 ├── KnownSkills (habilidades aprendidas)
 ├── MasteredSkills (habilidades permanentes)
 ├── ActiveClass
 └── EquippedSkills (limitadas por slots)
Skill
 ├── Effect (daño, buff, invocación, etc.)
 ├── XP / Mastery
 ├── Requirements (otras skills masterizadas)
 └── Source (Class | Item | Event)
Class
 ├── SkillTree (referencias a skills)
 └── PassiveModifiers (no habilidades activas)
Item
 └── GrantsSkills (temporal hasta masterizar)

[Original PDF page 52]
Mastery permite conservar rasgos pasivos en otras clases
Ejemplos:
Piel de Jaguar: evasión + contraataque
Ojos del Búho: visión de secretos / críticos
🌽  Tlamatini (Sabio del equilibrio)
No es mago. Es gestor del mundo.
Manipula probabilidades, economía, eventos
Skills afectan sistemas, no solo combate
Ejemplos:
Destino Reescrito: reroll de loot
Equilibrio Ancestral: enemigos más fuertes → mejores recompensas
🔥  Cuāuhtli (Guerrero del sacrificio)
Poder a cambio de pérdida controlada.
Gasta vida, objetos o progreso temporal
Mastery reduce el costo
Ejemplos:
Corazón Ardiente: daño escala con HP faltante
Ofrenda Solar: sacrifica oro para buffs largos
🌬  Ehecatzin (Tejedor del viento)
Movilidad, turnos, manipulación temporal.
No “pega fuerte”
Controla el ritmo del combate
Ejemplos:
Paso Invisible: actúas dos veces
Eco del Viento: repites la última skill usada

[Original PDF page 53]
5️⃣  Árboles de habilidades (corrección importante)
Tu idea actual:
Eso es demasiado rígido y mata la experimentación.
Ajuste profesional:
Aprender ≠ Masterizar
El árbol desbloquea acceso
El mastery desbloquea permanencia
👉  Así el jugador prueba antes de comprometerse
👉  Tú reduces frustración y soporte futuro
6️⃣  Equipamiento con habilidades (control de daño)
Esto es potentísimo y fácil de romper.
Regla obligatoria:
Deben ser:
Utilitarias
Situacionales
O versiones “incompletas”
Ejemplo:
Espada da Golpe Ígneo I
Clase da Golpe Ígneo III
Masterizar con ítem desbloquea solo el I permanente
7️⃣  Plan priorizado (sin fantasía)
“Solo puedes aprender X después de masterizar las anteriores”
Skill A → Skill B → Skill C
     ↘
      Skill D (requiere A masterizada)
Las habilidades de ítems NUNCA escalan igual que las de clase

[Original PDF page 54]
Fase 1 – Núcleo (NO negociable)
Sistema de Skill genérico
Mastery independiente de clase
Cambio de clase sin penalización
👉  Sin UI bonita. Sin arte. Sin HD-2D.
Fase 2 – 2 clases + 12 skills
Una clase ofensiva
Una sistémica (economía / control)
👉  Si aquí ya se siente divertido, vas bien.
Fase 3 – Equipamiento con skills
Solo 1 tipo de ítem con habilidades
Medir cuánto rompe el juego
Fase 4 – Recién aquí:
Más clases
Arte HD-2D
Plataformas adicionales
8️⃣  Verdad final (de amigo)
Estás pensando como diseñador senior,
pero todavía reaccionas como desarrollador indie impulsivo.
La diferencia entre ambos:
El junior dice: “qué cool sería si…”
El senior dice: “qué mínimo necesito para probar esto sin arruinarme”
Si quieres, en el próximo mensaje:
Te diseño el diagrama exacto de clases/interfaces
O bajamos esto a Godot + arquitectura limpia
``````

</details>

<details>
<summary>Historical class trees and strategic AI (PDF pages 64-86)</summary>

``````text

[Original PDF page 64]
Todas encajan con tu sistema de mastería persistente entre clases.
🧩  PRINCIPIOS DE BALANCE (importantes)
Antes de las clases, reglas no negociables:
1. Ninguna habilidad temprana multiplica poder
2. Las habilidades finales:
alteran economía
alteran flujo de combate
o alteran reglas de progresión
3. Las sinergias requieren planificación, no suerte pura
4. El jugador no puede equipar todas las skills masterizadas
→ el loadout sigue limitando builds
🟥  CLASE 1: Tlamatini
🜂  El Sabio del Equilibrio
Rol: Control / Alteración del campo
Fantasía: Filósofo-sacerdote que manipula el orden natural
Mecánica central
Manipula reglas, no daño directo.
🟢  Rama I – Observación
Mirada Profunda
Revela poder real de enemigos (no nivel).
Early game, informativo.
Análisis Ritual
+% XP de habilidades usadas en combate.
Lectura del Flujo (Mastería)
Permite ver modificadores ocultos de enemigos.

[Original PDF page 65]
🟡  Rama II – Alteración
Ritual de Balance
Reduce buffs enemigos si superan cierto umbral.
Ancla del Orden
Impide que un enemigo escale durante el combate.
Corrección del Destino (Mastería)
Reescribe un resultado negativo 1 vez por combate.
🔴  Habilidad Final – Reordenar Realidad
Durante X turnos:
Todos los buffs > cierto valor se normalizan
No aumenta poder, reduce extremos
Extremadamente difícil de adquirir
🟥  CLASE 2: Cuāuhxicalli
🜄  Guardián del Sacrificio
Rol: Riesgo / Recompensa
Fantasía: Guerrero ritual que crece con el peligro
🟢  Rama I – Sangre
Corte Ritual
Autodaño leve → buff menor temporal.
Marca del Sacrificio
Si bajas de cierto HP, ganas poder.
Sangre Sabia (Mastería)
Los buffs por riesgo duran más.
🟡  Rama II – Ofrenda
Tributo de Vida
Convierte HP en recursos.

[Original PDF page 66]
Eco del Dolor
Parte del daño recibido se devuelve.
Pacto Carmesí (Mastería)
El autodaño no puede matarte.
🔴  Habilidad Final – Corazón al Sol
Mientras estés por debajo de X% HP:
Todas las skills cuestan menos
Ganas resistencia extrema
Si sobrevives, quedas exhausto
No usable sin preparación
🟥  CLASE 3: Ah Kin
🜁  Astrónomo del Tiempo
Rol: Timing / Manipulación de turnos
Fantasía: Sabio que lee ciclos solares
🟢  Rama I – Ciclos
Pulso Solar
Buff leve si actúas en el momento correcto.
Marca del Alba
Bonus al iniciar combate.
Calendario Vivo (Mastería)
Extiende buffs si se usan en orden correcto.
🟡  Rama II – Retrasos
Eclipse Parcial
Retrasa acciones enemigas.
Fase Oscura
Penaliza enemigos fuera de ciclo.

[Original PDF page 67]
Reloj Sagrado (Mastería)
Reduce cooldowns al ejecutar combos temporales.
🔴  Habilidad Final – Gran Cuenta
El combate entra en fases:
Amanecer / Cenit / Ocaso
Cada fase cambia reglas
Exige ejecución perfecta
🟥  CLASE 4: Teōtlcalli
🜃  Canalizador de Espíritus
Rol: Invocación / Apoyo indirecto
Fantasía: Portador de deidades menores
🟢  Rama I – Espíritus
Invocar Nahual
Espíritu menor con efecto pasivo.
Eco Espiritual
Reaplica buffs usados recientemente.
Vínculo Ancestral (Mastería)
Espíritus persisten más.
🟡  Rama II – Pactos
Contrato Efímero
Invocaciones sacrifican algo a cambio de poder.
Canal Profundo
Potencia efectos espirituales.
Pacto Mayor (Mastería)
Permite 2 espíritus activos.

[Original PDF page 68]
🔴  Habilidad Final – Llamado del Panteón
Invoca un espíritu mayor:
Cambia reglas del combate
Consume recursos raros
Cooldown extremo
🟥  CLASE 5: Tlacatecolotl
🜂  Embaucador del Umbral
Rol: Trampa / Manipulación del riesgo enemigo
Fantasía: Trickster nocturno
🟢  Rama I – Engaño
Paso Falso
Penaliza acciones enemigas.
Sombra Cambiante
Evade si el enemigo falla.
Juego Mental (Mastería)
Castiga decisiones erróneas del enemigo.
🟡  Rama II – Umbral
Límite Roto
Enemigos no pueden retirarse.
Espejo Oscuro
Refleja debuffs.
Risa del Búho (Mastería)
Aumenta fallos enemigos.
🔴  Habilidad Final – Contrato del Umbral
Enemigos más fuertes = más errores
Combates caóticos, no seguros

[Original PDF page 69]
Requiere IA estratégica activa
🟥  CLASE 6 (opcional, late-game): Huehuetl
🜄  El Resonador
Rol: Sinergia / Build breaker
Fantasía: El ritmo del mundo
🔴  Solo habilidades avanzadas
Amplifica efectos masterizados
Escala con diversidad de clases
No usable sin varias masterías previas
🧠  VERDAD INCÓMODA
Esto no es un juego casual.
Es un juego que:
Premia entender sistemas
Castiga jugar en automático
Escala en complejidad, no solo números
Si no estás dispuesto a balancear esto con datos, el juego morirá.
Próximo paso (elige uno, sin rodeos):
1️⃣  Te diseño los primeros 2 niveles jugables
2️⃣  Te bajo 2 clases completas a datos JSON
3️⃣  Diseñamos IA estratégica que responda a estas skills
4️⃣  Definimos cómo se introducen estas clases sin abrumar al jugador
Di cuál.
Seguimos igual de firmes.

[Original PDF page 70]
3 por favor
Bien. Esta es una decisión adulta.
Diseñar IA estratégica antes de implementar combate es lo que separa un sistema vivo de
un “sumador de números con animaciones”.
Voy a ser directo y estructurado.
🧠  OBJETIVO DE LA IA (claro y medible)
La IA NO debe:
Ganar siempre
Hacer trampa
Leer inputs del jugador
La IA SÍ debe:
Evaluar poder real, no nivel
Reconocer amenazas
Cambiar comportamiento según el estado del combate
Castigar builds mal pensadas
Fallarte si tú fallas
🧩  PRINCIPIO BASE
Ejecutar es responsabilidad del sistema de combate.
La IA solo elige qué intentar.
🧱  ARQUITECTURA DE IA (engine-agnostic)
La IA no ejecuta habilidades.
La IA toma decisiones.

[Original PDF page 71]
1️⃣  EnemyAIProfile
👉  Cada enemigo tiene perfil, no scripts únicos.
2️⃣  Threat Evaluation (la clave)
Variables que sí evalúa
Poder efectivo actual
Buffs activos
Skills equipadas
Ritmo de ejecución (cooldowns)
HP relativo
Variables que NO evalúa
Nivel
Rareza de ítems
Clases directamente
3️⃣  ThreatLevel (simple pero poderoso)
class EnemyAIProfile {
    ThreatEvaluationStrategy threatStrategy;
    ActionSelectionStrategy actionStrategy;
    RiskTolerance riskTolerance;
    PhaseBehavior phaseBehavior;
}
interface ThreatEvaluationStrategy {
    ThreatLevel evaluate(Hero hero, CombatState state);
}
La IA reacciona a consecuencias, no a etiquetas.
Java
Java

[Original PDF page 72]
Esto evita:
árboles de if infinitos
comportamientos erráticos
🧠  DECISIÓN POR FASES (combate vivo)
4️⃣  PhaseBehavior
Ejemplo real:
OPENING: analiza, prueba
MID: optimiza daño / control
CRISIS: defensivo, huida, sacrificios
FINISHER: apuestas agresivas
⚙  SELECCIÓN DE ACCIONES
5️⃣  ActionSelectionStrategy
enum ThreatLevel {
    LOW,
    MEDIUM,
    HIGH,
    EXTREME
}
interface PhaseBehavior {
    CombatPhase determinePhase(CombatState state);
}
enum CombatPhase {
    OPENING,
    MID,
    CRISIS,
    FINISHER
}
Java
Java
Java

[Original PDF page 73]
Tipos de acción
Attack
Debuff
Buff
Delay
Escape
Sacrifice
Summon
👉  No son habilidades, son intenciones.
🟥  EJEMPLOS CON TUS CLASES
🧠  Contra Tlamatini
Detección
Buffs normalizados
Reescritura de eventos
Respuesta IA
Prioriza daño constante, no burst
Evita buffs grandes
Cambia a debuffs progresivos
👉  Castiga jugar a “controlar todo”.
🩸  Contra Cuāuhxicalli
Detección
interface ActionSelectionStrategy {
    EnemyAction chooseAction(ThreatLevel threat,
                             CombatPhase phase,
                             CombatState state);
}
 Java

[Original PDF page 74]
HP bajo
Buffs por riesgo
Respuesta IA
Cambia a daño distribuido
Evita ataques grandes
Intenta sangrar lentamente
👉  Si el jugador no mide riesgo, muere.
⏳  Contra Ah Kin
Detección
Uso rítmico de skills
Buffs cíclicos
Respuesta IA
Rompe timing
Retrasa turnos
Fuerza decisiones fuera de ciclo
👉  No puedes ejecutar combos perfectos gratis.
👻  Contra Teōtlcalli
Detección
Espíritus activos
Reaplicación de buffs
Respuesta IA
Mata invocaciones primero
Aplica silencios
Cambia objetivos
👉  El jugador debe proteger su setup.
🦉  Contra Tlacatecolotl
Detección

[Original PDF page 75]
Fallos inducidos
Penalizaciones indirectas
Respuesta IA
Reduce acciones
Juega conservador
Usa acciones seguras
👉  La IA aprende a no caer en trampas.
🧮  RIESGO CONTROLADO
6️⃣  RiskTolerance
Esto define:
Si huye
Si sacrifica aliados
Si apuesta a habilidades finales
👉  Aquí nace la personalidad enemiga.
🧪  EJEMPLO COMPLETO (resumido)
enum RiskTolerance {
    COWARD,
    BALANCED,
    AGGRESSIVE,
    FANATIC
}
Threat: HIGH
Phase: MID
Risk: BALANCED
→ Acción elegida:
   Debuff persistente
   + ataque moderado
Java

[Original PDF page 76]
Nada mágico.
Nada “inteligencia artificial falsa”.
🧠  VERDAD INCÓMODA
Si no implementas esta IA:
Tus clases profundas se vuelven inútiles
El jugador “rompe” el juego
El late-game se siente injusto o trivial
Esta IA no es opcional para el diseño que elegiste.
🔜  PRÓXIMO PASO (elige)
1️⃣  Diseñamos 3 tipos de enemigos base con estos perfiles
2️⃣  Bajamos esta IA a código Java limpio
3️⃣  Diseñamos un jefe que obligue a cambiar de build
4️⃣  Diseñamos cómo se enseña esta IA al jugador sin tutoriales largos
Di cuál.
Seguimos con la misma disciplina.
hagamos los 4, me parecen muy buenas sugerencias todas
Excelente. Esto ya es diseño de juego de alto nivel, no “ideas bonitas”.
Voy a ejecutar los 4 puntos, en orden lógico, con foco en jugabilidad real, enseñanza
implícita y castigo justo.
Voy a ser exigente: si algo no sirve en producción, no lo incluyo.
1️⃣  TRES TIPOS DE ENEMIGOS BASE (arquetipos de IA)

[Original PDF page 77]
Estos no son “slimes con distinto color”.
Son pilares de aprendizaje del jugador.
🟥  ENEMIGO A: Vigilante del Umbral
Rol: Castigador de builds caóticas
RiskTolerance: BALANCED
ThreatStrategy: Evalúa diversidad de skills
Comportamiento
Si detecta muchas skills distintas:
Aplica debuffs de interferencia
Si el jugador cambia de ritmo:
Castiga el cambio
👉  Enseña:
🟨  ENEMIGO B: Custodio Ritual
Rol: Control defensivo
RiskTolerance: COWARD
ThreatStrategy: Evalúa burst vs daño sostenido
Comportamiento
Si detecta burst:
Se protege
Si detecta daño lento:
Se fortalece con el tiempo
👉  Enseña:
🟦  ENEMIGO C: Depredador Cíclico
“Tener muchas habilidades masterizadas no significa que debas usarlas todas.”
“El ritmo importa tanto como el poder.”

[Original PDF page 78]
Rol: Timing killer
RiskTolerance: AGGRESSIVE
ThreatStrategy: Evalúa patrones temporales
Comportamiento
Observa repeticiones
Ataca justo cuando el jugador entra en “rutina”
👉  Enseña:
2️⃣  IA EN CÓDIGO (Java limpio, real)
Nada de pseudo-vapor.
📌  Importante:
No hay ifs por clase
No hay referencias al engine
Todo es datos + estrategias
Si mañana agregas una clase nueva →
NO TOCAS LA IA
“Los combos perfectos también pueden ser una debilidad.”
class EnemyDecisionEngine {
    EnemyAction decide(EnemyAIProfile profile, CombatState state) {
        ThreatLevel threat =
            profile.threatStrategy.evaluate(state.getHero(), state);
        CombatPhase phase =
            profile.phaseBehavior.determinePhase(state);
        return profile.actionStrategy.chooseAction(
            threat,
            phase,
            state
        );
    }
}
 Java

[Original PDF page 79]
3️⃣  JEFE DISEÑADO PARA ROMPER BUILDS (no números)
👑  JEFE: Xolotl, Guardián del Retorno
Concepto:
Un jefe que aprende de tus habilidades masterizadas.
FASE 1 – Observación
No ataca fuerte
Registra:
Skills usadas
Orden
Frecuencia
👉  El jugador cree que es fácil
👉  Error
FASE 2 – Adaptación
Bloquea temporalmente:
Skills repetidas
Refleja pasivas dominantes
👉  El jugador entra en pánico
👉  Cambiar build es obligatorio
FASE 3 – Confrontación
Usa versiones corruptas de tus propias habilidades
No escala en poder
Escala en castigo inteligente
FASE FINAL – Juicio
Si usas solo habilidades masterizadas:
El jefe se fortalece

[Original PDF page 80]
Si mezclas habilidades aprendidas y de ítems:
Se debilita
👉  Enseña:
4️⃣  CÓMO SE ENSEÑA TODO ESTO (sin tutoriales)
Este punto es crítico. Si fallas aquí, el juego muere.
🧠  PRINCIPIO
🔹  Nivel 1–2
Enemigos simples
Reglas invisibles
No castigan builds
👉  Libertad total
🔹  Nivel 3–5
Introduces Vigilante del Umbral
Castiga exceso de skills
👉  El jugador siente fricción
👉  Ajusta sin saber por qué (bien)
🔹  Nivel 6–8
Depredador Cíclico aparece
Rompe combos repetidos
“La mastería es poder… pero también una atadura.”
El jugador aprende perdiendo…
pero entiende por qué perdió.

[Original PDF page 81]
👉  El jugador empieza a pensar
🔹  Primer jefe
Mini-versión de Xolotl
No mata rápido
Pero bloquea estrategias
👉  El jugador entiende:
🧠  VERDAD INCÓMODA (pero necesaria)
Este diseño:
No es para todos
No es “idle-friendly”
No funciona si el jugador quiere apretar un botón
Pero…
✔  Es memorable
✔  Es escalable
✔  Soporta contenido durante años
✔  No se rompe con más clases
SIGUIENTE DECISIÓN (importante)
Ahora toca bajar esto a realidad jugable. Elige:
1️⃣  Diseñamos el primer mapa completo (con curva de dificultad)
2️⃣  Bajamos una clase + enemigos + jefe a datos reales
3️⃣  Diseñamos economía de progresión (XP, drop, rarezas)
4️⃣  Diseñamos cómo portar esto a Unity/Godot sin corromper el core
Di el número.
Seguimos sin bajar el estándar.
“El juego me está leyendo.”

[Original PDF page 82]
El juego tiene que tener un objetivo por el cual el jugador
quiera vencer los monstruos. cual seria la recompensa de
combatir y como hacemos que el jugador quiera seguir
buscando mas peleas/items/habilidades?
Buenísima pregunta. Y ojo: aquí se mueren el 80 % de los juegos.
Voy a ser brutalmente honesto y estructural.
Si el jugador pelea solo para subir números, tu sistema profundo se desperdicia.
Con la complejidad que diseñamos, el objetivo NO puede ser “hacerse más fuerte”. Eso
es infantil para este juego.
🧠  VERDAD CENTRAL (incómoda)
Acceso a:
Nuevas reglas
Nuevas decisiones
Nuevas combinaciones
Nuevas verdades del mundo
El poder es solo el peaje.
🎯  OBJETIVO MACRO DEL JUEGO
El mundo está fracturado en dominios
Cada dominio:
Tiene leyes distintas
Favorece ciertas builds
Castiga otras
La recompensa principal NO es poder.
Es acceso.

[Original PDF page 83]
👉  El objetivo del jugador es:
Pero mecánicamente:
🔁  EL BUCLE DE JUEGO REAL (loop)
Nada nuevo…
La diferencia está en qué es la recompensa.
🏆  ¿QUÉ GANA EL JUGADOR AL COMBATIR?
Voy por capas, de menor a mayor impacto.
1️⃣  Recompensa básica (necesaria, no suficiente)
✔  XP de habilidades
✔  Recursos
✔  Ítems
Esto no motiva por sí solo, pero sostiene el sistema.
2️⃣  Recompensa REAL: Fragmentos de Sistema
Los enemigos no sueltan solo loot, sueltan:
Restaurar / dominar / comprender esos dominios
(elige el tono narrativo después)
Cada dominio desbloquea nuevas posibilidades del sistema
Exploras →
Encuentras enemigos →
Combates →
Obtienes recompensas →
Desbloqueas acceso →
Cambias build →
Exploras zonas más complejas

[Original PDF page 84]
🔹  Fragmentos de:
Árboles de habilidades (nodos sellados)
Clases (habilitan ramas nuevas)
Reglas de combate
Pasivas globales
Ejemplo:
Ese fragmento:
Desbloquea skills temporales
O altera cómo funcionan los cooldowns
O habilita nuevas ramas de Ah Kin
👉  El jugador quiere pelear para ver qué desbloquea, no para subir daño.
3️⃣  Recompensa psicológica: Comprensión
Este es el nivel alto.
El jugador aprende:
Cómo piensa la IA
Qué castiga el mundo
Qué sinergias funcionan
Ese es el santo grial.
🧩  ¿POR QUÉ EL JUGADOR QUIERE SEGUIR PELEANDO?
Te doy 5 motores de motivación, todos compatibles.
🔥  MOTOR 1: “¿Y si…?”
¿Y si masterizo esta skill?
Matas a un Depredador Cíclico →
obtienes “Fragmento de Ritmo”
Empieza a buscar peleas para probar hipótesis.

[Original PDF page 85]
¿Y si mezclo esta clase con otra?
¿Y si cambio todo el loadout?
👉  El sistema responde, no ignora.
🧠  MOTOR 2: Enemigos que revelan contenido
Algunos enemigos:
Solo aparecen si usas ciertas skills
Solo sueltan loot si los derrotas de cierta forma
Cambian drops según tu build
👉  El jugador caza enemigos, no XP.
🔓  MOTOR 3: Zonas cerradas por comprensión, no nivel
No:
Sí:
👉  El jugador vuelve voluntariamente cuando cambia.
🧱  MOTOR 4: Mastería persistente
Cuando masterizas una habilidad:
Se queda contigo
Se convierte en parte de tu identidad
👉  El jugador siente:
👑  MOTOR 5: Jefes que desafían tu ego
Jefes como Xolotl:
“Nivel 20 requerido”
“Aquí tu build actual no funciona”
“Este personaje es mío”

[Original PDF page 86]
No se derrotan con fuerza bruta
Exponen tus malas decisiones
👉  El jugador quiere revancha.
👉  No por loot. Por orgullo.
🎁  ¿QUÉ NO DEBES HACER? (errores comunes)
❌  Loot infinito sin identidad
❌  Subir números sin cambiar decisiones
❌  Habilidades “mejor versión de lo mismo”
❌  Enemigos que solo escalan stats
Si caes en esto, todo el diseño previo muere.
🧠  RESUMEN CLARO
El jugador pelea para:
1. Desbloquear acceso, no solo poder
2. Expandir su caja de herramientas
3. Entender el mundo
4. Construir identidad
5. Vencer sistemas, no números
SIGUIENTE PASO (clave)
Ahora toca decidir qué es el mundo.
Elige uno:
1️⃣  Dominios con reglas físicas distintas
2️⃣  Mundo fragmentado por dioses / entidades
3️⃣  Simulación abstracta (tipo “realidad rota”)
4️⃣  Narrativa mínima, foco 100 % en sistemas
Dime el número.
Aquí se define el alma del juego.
``````

</details>

<details>
<summary>Historical Codex reward proposal (PDF pages 245-253)</summary>

``````text

[Original PDF page 245]
estratégicamente valioso
pero no imprescindible para sobrevivir
El codex debe premiar curiosidad, no castigar a quien no completa todo.
Mi recomendación brutalmente honesta
Sí, métele esta mecánica.
Le queda perfecta al juego.
Porque tu juego ya está construido alrededor de:
descubrimiento
conocimiento
mastery
builds
adaptación
La recompensa por completitud no es un injerto. Es coherente con el alma del sistema.
Próximo paso correcto
Lo siguiente es uno de estos dos:
1. Diseñar la tabla completa de rewards por categoría y porcentaje
2. Codificar el sistema interno de rewards encima del Codex actual
Lo mejor ahora es 1 primero, porque si codificas antes de cerrar el diseño, luego
remiendas.
Si quieres, en el próximo mensaje te dejo la tabla completa de recompensas del Codex,
bien balanceada, por categoría y umbrales.
hazlo

[Original PDF page 246]
Perfecto. Vamos a hacer esto como debe hacerse: una tabla completa, coherente,
balanceada y que no rompa el juego a medio plazo.
Voy a ser directo contigo:
🧠  PRINCIPIOS QUE VOY A RESPETAR
Antes de darte la tabla, te dejo las reglas que estoy aplicando (para que puedas juzgar si
algo no te cuadra):
1. Nada de power creep bruto
2. Recompensas = nuevas decisiones, no solo números
3. Cada categoría refuerza su fantasía
4. El 100% NO debe romper el juego
5. Las recompensas escalan en sofisticación, no en fuerza
🧾  TABLA COMPLETA DE RECOMPENSAS DEL CODEX
Formato:
% → Nombre
Tipo
Efecto
Intención de diseño
🐉  BESTIARIO (Conocimiento del enemigo)
25% — Instinto del Rastreador
Tipo: Pasiva global
Efecto: Revela 1 resistencia o debilidad del enemigo al iniciar combate
Intención: Reduce incertidumbre sin trivializar combate
Esto define la identidad del metajuego.
Si aquí te equivocas, todo lo demás se siente plano o injusto.

[Original PDF page 247]
50% — Taxonomía Bélica
Tipo: Pasiva global
Efecto: +5% probabilidad al primer debuff aplicado
Intención: Premia abrir combates estratégicamente
75% — Memoria del Cazador
Tipo: Activa (1 vez por combate)
Efecto: Ignora parcialmente la resistencia a debuffs del enemigo en ese turno
Intención: Botón táctico, no spam
100% — Nombre Verdadero
Tipo: Habilidad neutral desbloqueable
Efecto:
Revela completamente el enemigo
Reduce ligeramente su resistencia a debuffs por 1 turno
Intención: Poder informativo + ventana táctica
🧙  CLASES (Dominio de estilos)
25% — Memoria de Oficios
Tipo: Pasiva
Efecto: Cambio de clase fuera de combate sin penalización
Intención: Fomenta experimentar
50% — Eco de Disciplina
Tipo: Sistema
Efecto: +1 slot de pasiva híbrida
Intención: Buildcrafting real
75% — Camino Cruzado

[Original PDF page 248]
Tipo: Sistema
Efecto: Permite acceder a nodos de otra rama/clase en el árbol
Intención: Builds híbridas avanzadas
100% — Quinta Esencia
Tipo: Desbloqueo mayor
Efecto: Desbloquea metaclase o stance especial
Intención: Endgame significativo sin romper early/mid
📜  HABILIDADES (Maestría técnica)
25% — Disciplina Menor
Tipo: Pasiva
Efecto: +5% XP para habilidades usadas
Intención: Acelera mastery sin romper ritmo
50% — Resonancia Técnica
Tipo: Pasiva
Efecto: Primera habilidad usada en combate cuesta -10% recurso
Intención: Mejora apertura táctica
75% — Variación Controlada
Tipo: Sistema
Efecto: Las habilidades masterizadas pueden elegir 1 mutación menor
Intención: Profundidad sin complejidad absurda
100% — Archivo Vivo
Tipo: Habilidad neutral
Efecto: Duplica efectos secundarios de una skill por 1 turno
Intención: Momento “combo”, no spam permanente

[Original PDF page 249]
🗡  ARMAS (Dominio ofensivo)
25% — Afinidad Bélica
Tipo: Pasiva
Efecto: Identifica propiedades ocultas de armas más rápido
Intención: Mejor lectura de loot
50% — Golpe Afinado
Tipo: Pasiva
Efecto: +pequeña probabilidad de crítico situacional
Intención: No rompe daño, pero añade tensión
75% — Transferencia Parcial
Tipo: Sistema
Efecto: Puedes extraer una propiedad menor de un arma equipada
Intención: Customización sin caos
100% — Arma Reliquia
Tipo: Desbloqueo
Efecto: Acceso a arma especial única
Intención: Recompensa tangible, no solo números
🛡  ARMADURAS (Supervivencia y control)
25% — Piel Adaptativa
Tipo: Pasiva
Efecto: Reduce ligeramente duración de debuffs
Intención: Defensa elegante

[Original PDF page 250]
50% — Reflejo Controlado
Tipo: Pasiva
Efecto: Pequeña probabilidad de reflejar debuff
Intención: Contra builds de control
75% — Resistencia Dinámica
Tipo: Sistema
Efecto: Resistencias se ajustan ligeramente según daño recibido
Intención: Combate más reactivo
100% — Forma Inquebrantable
Tipo: Activa (cooldown interno)
Efecto: Ignoras efectos negativos por 1 turno
Intención: Botón clutch, no permanente
💍  ACCESORIOS (Especialización)
25% — Afinidad Arcana
Tipo: Pasiva
Efecto: +ligero bonus a buffs
Intención: Mejora builds de soporte
50% — Resonancia Dual
Tipo: Sistema
Efecto: Puedes equipar accesorios con sinergia especial
Intención: Builds interesantes
75% — Amplificación Selectiva
Tipo: Pasiva
Efecto: Aumenta efectos de un tipo específico de habilidad

[Original PDF page 251]
Intención: Especialización
100% — Relicario Vivo
Tipo: Desbloqueo
Efecto: Acceso a reliquias con efectos únicos
Intención: Endgame build-defining
🧪  OBJETOS (Economía y control)
25% — Identificación Instintiva
Tipo: Pasiva
Efecto: Identifica objetos básicos automáticamente
Intención: QoL útil
50% — Recuperación Eficiente
Tipo: Pasiva
Efecto: Objetos consumibles son ligeramente más efectivos
Intención: Mejora economía sin romperla
75% — Síntesis Menor
Tipo: Sistema
Efecto: Permite combinar objetos básicos
Intención: Crafting simple
100% — Alquimia Avanzada
Tipo: Desbloqueo
Efecto: Acceso a crafting especial
Intención: Meta progresión

[Original PDF page 252]
⚖  COMPLETITUD GLOBAL
100% GLOBAL — Archivo Absoluto
Tipo: Meta recompensa
Efecto:
+ligero bonus global a aprendizaje
acceso a contenido oculto (boss / zona / lore)
Intención: premio simbólico + contenido, no poder roto
🔥  VERDAD INCÓMODA FINAL
Esto que acabamos de definir:
No es solo “recompensas”
Es una segunda capa de progresión
Es lo que hará que el jugador quiera completar el juego
Pero cuidado:
Deben ser:
útiles
interesantes
deseables
pero nunca obligatorias.
⏭  SIGUIENTE PASO (te recomiendo esto)
Ahora que esto está definido, lo correcto es:
1. Integrar estas rewards en el CodexRewardRegistry
2. Conectar con:
sistema de habilidades
sistema de pasivas
sistema de clases
3. Añadir eventos de desbloqueo con feedback (log/UI)
Si haces que estas recompensas sean necesarias para ganar, matas la libertad del
jugador.

[Original PDF page 253]
Si quieres, el siguiente paso fuerte es:
👉  Convertir TODA esta tabla en código listo para enchufar al engine
(con factories, ids, wiring limpio)
Y ahí ya entras en fase “esto es un juego de verdad”.
bien, hagamoslo
Thought for 12s
Voy a bajarlo a código con estructura limpia y dejando claros los puntos de integración
con habilidades, pasivas y clases. Primero cierro el modelo de recompensas y luego te doy
el wiring mínimo para que el engine lo consuma sin hacks.
Perfecto. Vamos a convertirlo en código listo para enchufar al engine, con separación
clara entre:
definición de rewards
registro
desbloqueo
aplicación
consulta para UI/log
Y voy a ser estricto con el diseño:
Eso te salva de acoplar el Codex a todo el juego.
Estructura propuesta
El sistema de rewards del Codex no debe conocer los detalles internos de combate,
clases o skills.
Solo debe emitir recompensas desbloqueadas.
Luego otro servicio las aplica al perfil del jugador.
``````

</details>

<details>
<summary>Historical Nova and mentor decisions (PDF pages 314-328)</summary>

``````text

[Original PDF page 314]
1. Lo más valioso del documento
Lo más potente del documento no son los nombres. Es esta idea base:
Eso sí vale oro para tu juego, porque encaja con lo que ya definimos:
clases intercambiables
habilidades masterizables
sinergias
codex
recompensas por conocimiento
progresión no lineal
Lo que el documento hace bien es cambiar el foco de:
“más fuerza”
a
“más comprensión”
Y eso combina perfecto con tu frase final de diseño: aprender a pensar de diferentes
maneras y combinarlas.
2. Qué incorporaría sí o sí
2.1 Nova como eje del juego
Esto lo incorporaría casi intacto.
Nova no debería ser “otra clase más”.
Debe ser el contenedor del progreso transversal.
Eso significa:
los otros arquetipos no son personajes jugables completos al estilo tradicional
son fuentes de doctrina
Nova es quien:
aprende
sintetiza
equipa
el progreso no consiste en “subir de rango”, sino en aprender formas distintas de
pensar e integrarlas.  Diseño conceptual Diseño conceptual
Diseño conceptual

[Original PDF page 315]
mezcla
evoluciona
Eso respeta la distinción especialista vs generalista integrador del documento.
Traducción mecánica
Gepetto, Lyra, Atlas, Spark → escuelas/doctrinas/arquetipos
Nova → avatar principal del jugador
el jugador no “abandona a Nova” para jugar a otro
el jugador incorpora capas de esos arquetipos sobre Nova
Eso es mucho mejor que convertir a todos en clases completas separadas y perder el
centro del diseño.
2.2 Los arquetipos como doctrinas, no solo personajes
Esto también lo incorporaría.
Cada arquetipo puede existir en 3 niveles a la vez:
Narrativo: personaje/mentor/perspectiva
Sistémico: árbol de habilidades y pasivas
Metajuego: fuente de progreso y desbloqueos
Eso sale directamente del documento y es correcto.
Mi traducción concreta
Gepetto → crafting, optimización, tuning de builds
Lyra → análisis, cleanse, lectura de debilidades, contra-juego
Atlas → precisión, estabilidad, reducción de azar, consistencia
Spark → mutaciones, combos raros, sinergias no lineales
Nova → síntesis entre sistemas incompatibles
Eso sí tiene identidad.
2.3 Evoluciones de Nova
Las tres evoluciones propuestas me parecen buenas, pero no como “formas finales fijas”.
Me parecen mejores como especializaciones de metaprogresión:
Diseño conceptual
Diseño conceptual

[Original PDF page 316]
Sintetizador → mezcla sistemas distintos
Catalizador → potencia sinergias y efectos cruzados
Conductor → coordina varios subsistemas y turnos
Esto es útil porque le da a Nova una identidad propia sin caer en “aprendiz → maestro”.
3. Qué NO incorporaría tal cual
Aquí viene la parte honesta.
3.1 No convertiría a los arquetipos en “personajes presentes en
combate” de entrada
El documento deja abierta la idea de que estén “presentes” y que Catalizador potencie
habilidades “cuando están presentes”.
Eso me genera una alarma.
Si lo interpretas como:
party system
invocaciones permanentes
5 personajes coordinados en combate
vas a disparar brutalmente la complejidad del juego.
Mi recomendación
No los hagas compañeros físicos al principio.
Hazlos primero como:
árbol
doctrina
stance
mentor activo
afinidad equipada
Más adelante, si quieres, pueden manifestarse como:
ecos
avatares
Diseño conceptual
Diseño conceptual

[Original PDF page 317]
proyecciones
ultimates
Pero no abriría party combat todavía.
3.2 No haría “misiones secundarias” completas de cada arquetipo
desde el inicio
Sí, el documento propone rutas de aprendizaje con desafíos y misiones únicas.
Eso está bien conceptualmente. Pero en producción, si arrancas con 4 líneas narrativas
completas más el juego base, te ahogas.
Lo correcto
Primero haz:
2 arquetipos
1 reto de afinidad por arquetipo
1 nodo de progresión claro
1 habilidad híbrida desbloqueable
Luego escalar.
El documento mismo recomienda introducir el sistema de forma incremental y empezar
con 1–2 arquetipos. Ahí tiene razón.
4. Cómo lo integraría con lo que ya construimos
Aquí está la parte importante. No quiero que esto se quede como “ideas bonitas”.
4.1 Estructura conceptual nueva
Lo metería así:
Nova sigue siendo el jugador
Arquetipos Cognitivos pasan a ser una nueva capa de progreso
cada arquetipo aporta:
un árbol
una pasiva doctrinal
Diseño conceptual
Diseño conceptual

[Original PDF page 318]
una prueba
una recompensa de integración
las habilidades híbridas se desbloquean cuando Nova combina fragmentos de varios
arquetipos
4.2 Cómo convive con clases
Aquí hay una duda grande de diseño y no la voy a adivinar.
Hasta ahora veníamos construyendo un juego donde:
hay clases
cambias de clase
masterizas habilidades
conservas habilidades
El documento propone otra capa:
Nova aprende de Gepetto, Lyra, Atlas y Spark
no como clases clásicas, sino como formas de pensar
Mi lectura correcta
Las clases de combate y los arquetipos cognitivos no deberían ser la misma cosa.
Mejor modelo
Clase = cómo peleas ahora
Arquetipo cognitivo = cómo entiendes, modificas y combinas sistemas
Eso evita duplicidad.
Ejemplo
puedes estar en clase Tlamatini
pero con afinidad cognitiva principal Lyra
y fragmentos de Atlas
Eso sí produce identidad.
Pregunta 1
¿Quieres que los arquetipos cognitivos sean:
A) una capa separada de las clases,
o B) que reemplacen completamente el sistema de clases que ya diseñamos?

[Original PDF page 319]
Yo te recomiendo A. Si eliges B, hay que desmontar bastante de lo anterior.
5. Traducción de cada arquetipo a mecánicas reales
Gepetto — El Constructor
Esto encaja perfecto con tu arquitectura y con el codex.
Lo valioso
crafting avanzado
optimización
mejora de builds
resolución estructural
Lo traduciría en:
reconfiguración de equipamiento
extracción de afijos
reducción de coste de mutación
mejoras de eficiencia
“planos” o “ensambles” híbridos
Recompensa de integración
Nova aprende de Gepetto a reconciliar piezas incompatibles.
Habilidad híbrida ejemplo
Ensamble Improbable
Permite combinar dos propiedades que normalmente no conviven.
Lyra — La Crítica
Este me parece uno de los más fuertes del documento.
Lo valioso
refutación
análisis
cuestionamiento
counterplay
Diseño conceptual

[Original PDF page 320]
eliminación de efectos negativos
identificación de debilidades
Lo traduciría en:
reveal de patrones enemigos
cleanse selectivo
ruptura de buffs enemigos
“insight” para exponer incoherencias del rival
skills que castigan builds lineales
Recompensa de integración
Nova aprende a detectar contradicciones sistémicas.
Habilidad híbrida ejemplo
Refutación Dirigida
Si un enemigo depende demasiado de una condición, la invalida por 1 turno.
Atlas — El Lógico
Muy útil para estabilizar el caos del sistema.
Lo valioso
precisión
estabilidad
cálculo
predicción
control de estados estables
Lo traduciría en:
reducción de varianza
mejora de precisión
predicción de resultado
ordenamiento de turnos
minimización de RNG
Recompensa de integración
Nova aprende a fijar patrones y volverlos confiables.

[Original PDF page 321]
Habilidad híbrida ejemplo
Axioma de Repetición
La próxima habilidad usada repite su comportamiento con menor varianza.
Spark — El Creativo
Muy peligroso y muy valioso.
Lo valioso
improvisación
caos controlado
efectos aleatorios controlados
variaciones
sinergias no lineales
Riesgo
Si lo diseñas mal, se vuelve “random gracioso” y nada más.
Lo traduciría en:
rerolls tácticos
bifurcación de efectos
combinaciones emergentes
mutaciones de habilidad
efectos sorpresa con límites
Recompensa de integración
Nova aprende a forzar la aparición de posibilidades nuevas.
Habilidad híbrida ejemplo
Chispa Divergente
Una skill puede resolverse con una variante secundaria inesperada.
Nova — El Integrador
Aquí está el corazón.

[Original PDF page 322]
Nova no debería tener árbol propio grande al inicio.
Nova debería tener un sistema de síntesis.
Lo traduciría en:
slots de fragmentos doctrinales
recetas de integración
perks híbridos
evoluciones por umbral
6. Cómo quedaría la progresión real
Fase 1 — Afinidad
Nova conoce a un arquetipo y desbloquea:
visión
doctrina base
primer nodo
Fase 2 — Fragmento
Supera una prueba y obtiene:
fragmento cognitivo del arquetipo
Fase 3 — Integración cruzada
Al combinar 2 o más fragmentos:
desbloquea habilidad híbrida
o pasiva de síntesis
Fase 4 — Evolución de Nova
Cuando alcanza cierto patrón:
puede evolucionar hacia
Sintetizador
Catalizador
Conductor
Eso es fiel al documento y jugablemente sólido. Diseño conceptual

[Original PDF page 323]
7. Compatibilidad con nuestras pautas técnicas
Sí, sí se puede desarrollar con las pautas del documento, y de hecho son buenas pautas:
separar narrativa, mecánicas y arquitectura
no hardcodear lógica en UI
evitar dependencia rígida
introducir incrementalmente
datos extensibles
Eso encaja muy bien con lo que ya armamos:
core engine desacoplado
codex interno
rewards
unlocks
passives/features
combat log
slice
Mi veredicto
El documento no contradice el enfoque técnico. Lo fortalece.
8. Dónde todavía hay ambigüedad real
Aquí van las preguntas que necesito para no inventar decisiones por ti.
Pregunta 2
¿Qué peso narrativo quieres que tengan Gepetto, Lyra, Atlas y Spark?
A) solo mentores/sistemas
B) personajes con diálogo y presencia fuerte
C) personajes reclutables/manifestables
D) mezcla de A y B
Mi recomendación: D, pero sin volverlos party members de combate aún.
Pregunta 3
Diseño conceptual

[Original PDF page 324]
¿Cómo quieres que Nova “aprenda” de ellos?
A) completando misiones secundarias
B) ganando fragmentos en combate
C) usando sistemas vinculados a cada arquetipo
D) mezcla de A, B y C
Mi recomendación: D, porque solo una vía se queda corta.
Pregunta 4
Las evoluciones de Nova — Sintetizador, Catalizador, Conductor — ¿quieres que
sean:
A) elecciones permanentes
B) stances intercambiables
C) ramas largas pero reconfigurables
D) títulos narrativos con perks
Mi recomendación: C o B.
A es demasiado rígido para el tipo de juego que estás imaginando.
Pregunta 5
¿Quieres que el sistema de arquetipos entre ya en el combate del slice actual, o prefieres
primero integrarlo como metaprogresión/codex/progreso fuera de combate?
Mi recomendación brutalmente honesta:
primero fuera de combate y como capa de progreso, luego lo llevamos al combate.
Si lo metes ya en combate sin cerrar el modelo, ensucias el engine.
9. Qué haría yo, paso a paso
Paso 1
Congelar esta decisión:
clases de combate y arquetipos cognitivos son capas distintas

[Original PDF page 325]
Paso 2
Crear nuevo sistema interno:
CognitiveArchetypeId
NovaIntegrationProfile
ArchetypeFragment
ArchetypeAffinityProgress
NovaEvolutionTrack
Paso 3
Empezar solo con:
Lyra
Atlas
Porque son los más fáciles de aterrizar sin ruido:
análisis / counterplay
precisión / estabilidad
Paso 4
Conectar al Codex:
completar entradas de bestiario/habilidades/clases da progreso de afinidad
Paso 5
Desbloquear primera habilidad híbrida de Nova
Paso 6
Recién después:
Gepetto
Spark
evoluciones de Nova
Mi valoración final
Tus ideas del documento sí son valiosas. No son humo.
Lo más valioso no es el lore de los nombres, sino la estructura:
especialistas
Nova como integrador

[Original PDF page 326]
progresión por conocimiento
evitar linealidad
aprendizaje como mecánica central
Lo peligroso sería intentar meterlo todo a la vez.
Responde estas 5 preguntas y con eso ya puedo aterrizarte el sistema exacto sin seguir
adivinando.
Sources
Pregunta 1 - A
Pregunta 2 - D
Pregunta 3 - D, en efecto, estaba pensando en hacer
misiones especificas con cada maestro para "fijar" las
ensenanzas en nova. Si no haces las misiones, no te
quedas con la ensenanza y solo estara activa cuando estes
en esa clase o plano de pensamiento.
Pregunta 4 - Quisiera una combinacion de habilidades fijas
por clase + habilidades equipables, estas equipables son
las que podemos aprender de los maestros, de las armas
que masterizmos, etc etc
Pregunta 5 - Si, llevemoslo solo como capa de progreso,
despues lo mejoramos.
Incorporemos lo que sea posible
Bien. Tus respuestas aclaran lo importante y, sinceramente, evitan varios errores típicos.
Voy a integrarlo sin romper lo que ya construimos y sin inflar el sistema antes de tiempo.
🧠  Decisiones ya fijadas (y por qué son buenas)
Voy directo:

[Original PDF page 327]
✔  Arquetipos = capa separada de clases (Pregunta 1: A)
Perfecto.
Esto evita duplicidad y mantiene el sistema limpio.
✔  Arquetipos como mentores + presencia narrativa (Pregunta 2: D)
Bien.
Eso te permite:
sistema mecánico
narrativa rica
sin obligarte a party combat
✔  Aprendizaje híbrido (Pregunta 3: D)
Clave.
Esto introduce una mecánica MUY interesante:
conocimiento prestado vs integrado
✔  Habilidades fijas + equipables (Pregunta 4)
Esto ya encaja perfecto con:
nuestro sistema de skills
mastery
equipables
No hay que reinventar nada aquí.
✔  Arquetipos como capa de progreso (Pregunta 5)
Correcto.
Esto evita ensuciar el combate ahora.
Misiones = fijación permanente
Uso temporal = solo mientras estás en ese “plano”

[Original PDF page 328]
🎯  Qué vamos a construir ahora
Vamos a integrar solo lo necesario para que esto viva sin romper nada:
Nueva capa: Sistema Cognitivo
🔥  Concepto clave (el más importante)
Esto es lo mejor de todo tu documento, y lo vamos a respetar:
Eso se traduce a código así:
Enseñanza temporal → activa mientras estás alineado con el arquetipo
Enseñanza fijada → permanente (tras misión)
🧱  Nuevas clases (core del sistema)
CognitiveArchetypeId.cs
Nova
 ├── Clase (combate)
 ├── SkillBook (habilidades)
 ├── Codex (progreso)
 └── Cognición (nuevo sistema)
       ├── Afinidad por arquetipo
       ├── Fragmentos aprendidos
       ├── Enseñanzas fijadas
       ├── Enseñanzas temporales
No todo lo que Nova aprende se queda.
Solo lo que integra.
namespace Game.Core.Cognition
{
    public enum CognitiveArchetypeId
    {
        Gepetto,
        Lyra,
``````

</details>

<details>
<summary>Historical cognition decisions and conceptual layout (PDF pages 335-341)</summary>

``````text

[Original PDF page 335]
🧠  Preguntas clave (las que sí importan ahora)
No te voy a hacer 20 preguntas. Solo las que desbloquean el siguiente paso.
Pregunta 1 (CRÍTICA)
¿Cómo quieres que el jugador cambie de arquetipo activo?
A) libremente fuera de combate
B) en combate con coste
C) automático según acciones
D) limitado por cooldown o recurso
👉  Esto define toda la estrategia del sistema
Pregunta 2
¿Quieres limitar cuántos fragmentos activos puede tener Nova al mismo tiempo?
A) no (todo activo)
B) sí, por slots
C) sí, por afinidad
D) híbrido
👉  Si no lo limitas, el sistema se rompe
Pregunta 3
¿Quieres que las misiones sean:
A) narrativas
B) desafíos mecánicos
C) puzzles cognitivos
D) mezcla
👉  Esto define cómo se “fijan” enseñanzas

[Original PDF page 336]
🧨  Veredicto final
Esto que acabamos de integrar:
respeta tu documento
no rompe el engine
es escalable
introduce una mecánica muy original
diferencia tu juego de clones tipo Hero Wars
Y lo más importante:
Respóndeme esas 3 preguntas y el siguiente paso te lo bajo a:
👉  misiones reales de Lyra y Atlas + integración directa en el slice actual
Ya no estás haciendo un RPG más.
Estás haciendo un juego sobre aprender a pensar.
Pregunta 1: A,
Pregunta 2: B
Pregunta 3: D, si lo hacemos todo igual, tenemos que
garantizar un desarrollo demasiado bueno de la historia
para que no se vuelva pedante, sin embargo, si lo hacemos
mezclado, puede ser medianero la historia que sigue
siendo sorprendente e innovador, quizas lo suficiente para
lograr un equilibrio.
Thought for 20s
Perfecto. Tus respuestas cierran la ambigüedad importante y, honestamente, van por el
camino correcto.
La combinación que elegiste evita tres errores muy comunes:
convertir los arquetipos en otra capa de clases redundante,

[Original PDF page 337]
obligarte a escribir una historia “demasiado brillante” para sostener sola el sistema,
y romper el balance dejando todas las enseñanzas activas al mismo tiempo.
Además, encaja muy bien con el corazón del documento: Nova como generalista
integrador, los demás como especialistas, progreso por rutas de aprendizaje, habilidades
híbridas y crecimiento basado en integrar conocimiento, no solo experiencia.
Lo que queda fijado desde ya
1. Los arquetipos cognitivos no reemplazan a las clases
Quedan como capa separada de progreso.
Eso respeta el documento y evita duplicidad sistémica. Las clases siguen definiendo el
combate base; los arquetipos definen cómo Nova aprende, analiza, combina y fija
conocimiento.
2. El aprendizaje tiene dos estados
Esto es de lo mejor que salió de tu respuesta:
enseñanza temporal: activa mientras Nova está alineado con ese arquetipo/plano de
pensamiento
enseñanza fijada: se vuelve permanente al completar la misión/desafío del maestro
Eso aterriza muy bien la idea del documento de que cada arquetipo ofrece misiones,
desafíos y mecánicas específicas, y que el progreso ocurre al adquirir fragmentos de
disciplina que luego se integran.
3. El loadout final del jugador será híbrido
Queda así:
habilidades fijas por clase
habilidades equipables aprendidas de maestros, armas masterizadas y otras fuentes
Nova decide qué lleva activamente
Eso es compatible con lo que ya construimos y además refuerza los árboles no lineales y
las combinaciones imposibles para especialistas que el documento buscaba.
4. Los arquetipos entran primero como metaprogresión
No los vamos a meter aún como entidades de combate ni party system.
Primero serán:

[Original PDF page 338]
afinidad,
fragmentos,
enseñanza activa,
fijación por misión,
y desbloqueo de sinergias híbridas.
Eso también está alineado con el lineamiento de introducir el sistema de forma
incremental, empezar con 1–2 arquetipos y validar mecánicas antes de expandir.
Qué incorporamos ahora mismo
Voy a aterrizar solo lo valioso y viable.
Capa nueva: Cognición de Nova
Esto captura exactamente la lógica del documento sin sobrecargar todavía el combate.
Qué arquetipos incorporamos primero
Voy a ser firme aquí: solo Lyra y Atlas.
No porque Gepetto y Spark no sirvan, sino porque Lyra y Atlas son los que mejor permiten
validar el sistema sin meter ruido:
Lyra: análisis, insight, counterplay, debilidad, limpieza.
Atlas: precisión, estabilidad, predicción, reducción de variabilidad.
Diseño conceptual
Nova
 ├── Clase actual
 ├── Habilidades fijas de clase
 ├── Habilidades equipables
 ├── Codex / Rewards
 └── Cognición
      ├── Arquetipo activo
      ├── Afinidad por arquetipo
      ├── Fragmentos desbloqueados
      ├── Enseñanzas temporales
      ├── Enseñanzas fijadas
      └── Potencial híbrido

[Original PDF page 339]
Spark y Gepetto los dejaría como TODO de siguiente ola.
Diseño concreto que incorporamos
Lyra
Rol
Arquetipo de refutación, insight y contra-juego.
Fragmentos iniciales
lyra_detect_weakness
lyra_minor_cleanse
Efectos
lyra_detect_weakness
temporal: al estar en plano Lyra, Nova puede revelar una debilidad o resistencia
del enemigo al inicio de combate
fijada: ese efecto ya no depende del plano Lyra, queda disponible siempre
lyra_minor_cleanse
temporal: habilita una limpieza menor
fijada: esa habilidad equipable queda permanente
Misión de fijación propuesta
La Grieta del Juicio
mezcla de narrativa + desafío mecánico + pequeño puzzle
Nova debe identificar una contradicción en el comportamiento de un enemigo o
sistema y explotarla correctamente
si lo logra, “fija” lyra_detect_weakness
Atlas
Rol
Arquetipo de estabilidad, precisión y previsibilidad.
Fragmentos iniciales

[Original PDF page 340]
atlas_stable_cast
atlas_predict_outcome
Efectos
atlas_stable_cast
temporal: reduce variación de daño o coste en ciertas skills
fijada: se vuelve una pasiva o modificador permanente de una familia de
habilidades
atlas_predict_outcome
temporal: muestra un preview simple del resultado probable de una acción
fijada: mejora ese preview o lo hace más preciso
Misión de fijación propuesta
El Problema de las Tres Certezas
mezcla de desafío de combate controlado + puzzle de decisión
Nova debe demostrar consistencia, no creatividad
al completarla, fija atlas_stable_cast
Cómo se comporta Nova con este sistema
Regla de oro
Nova no “domina” a Lyra ni a Atlas.
Nova adopta temporalmente su plano y luego, si completa la prueba, internaliza parte de
su forma de pensar.
Eso respeta la idea del documento de que el jugador aprende las formas de pensamiento
de estos personajes y que la progresión surge al integrar múltiples disciplinas.
Limitación por slots
Elegiste bien al limitar por slots. Si no, el sistema se rompería.
Modelo
Nova tiene un arquetipo activo
tiene acceso temporal a los fragmentos de ese arquetipo

[Original PDF page 341]
además puede llevar un número limitado de enseñanzas fijadas equipadas
Propuesta inicial
1 arquetipo activo
2 enseñanzas fijadas equipables
Eso es suficiente para:
sentir progresión,
notar elecciones,
y no convertir el personaje en “todo a la vez”.
Cómo se conecta con el slice actual
Podemos incorporar ya una parte al slice, sin romper nada.
En el slice actual
Ya tenemos:
combate,
skill usage,
debuffs,
pasivas del codex,
log.
Lo que añadimos
1. Estado cognitivo de Nova
arquetipo activo = Lyra
fragmento temporal = lyra_detect_weakness
2. Hook al combate
Cuando arranca el combate:
si Nova tiene lyra_detect_weakness, activa reveal de una debilidad
si tiene atlas_stable_cast, reduce variación o coste de la siguiente skill
3. Hook a misiones
Fuera del combate:
si completa misión de Lyra o Atlas, el fragmento pasa de temporal a fijado
``````

</details>

