# Fragments of Wysdom — Game Design, Creative Vision & Architecture Bible

> **v4.0 — DRAFT for review.** Consolidated from the v2 Bible on `main`, repository technical documents, current source-code inspection and documented design conversations. No gameplay changes.  
> **Interpretation:** the Bible describes *desired experience*, *accepted direction*, *proposed ideas* and *current implementation* separately. Do not mistake a domain class for a finished game feature.  
> **Scope of evidence:** source inspected on `main` (2026-10-30); build, smoke suite, CI and Unity Play Mode were **not run** in this pass.

## Reading guide: two independent status dimensions

Every mechanic has **Design status** (`ACCEPTED`, `PROMISING / TO REFINE`, `OPEN`, `REJECTED`) and **Implementation status** (`CODE FOUNDATION`, `PARTIAL`, `NOT FOUND / NOT VERIFIED`, `CONCEPT ONLY`, `VERIFIED`). `VERIFIED` requires actual execution evidence. A promising conversation idea is **not** a mandate to implement. When a conversation did not settle a detail, record it as `OPEN`. Old version names in code are compatibility identifiers, not proof of active design version.

# Part I — Game Design & Creative Vision

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

### 4. Combat fantasy, rhythm and tactical economy

**IMPLEMENTED — DOCUMENTED:** `TurnBasedCombatEngine` is the **only canonical turn-sequencing authority**. Do not introduce a second `CombatResolver` or parallel action engine. Existing flow:

```text
TurnRequest
  -> TurnBasedCombatEngine
  -> CombatRuntimeContext
  -> CombatContext
  -> DamageResolver
  -> CombatEntity / ResourcePool / ResonanceProfile / GameEventLog
```

The foundation covers basic attacks, skills, hit/evasion, criticals, HP, defeat, resource checks, passive hooks, regeneration and status resolution. Opposed stats: Attack/Defense; Precision/Evasion; Critical Chance/Critical Resistance; Debuff Chance/Debuff Resistance.

**Resonance:** `ActiveResonance` pays for skills; `ResonanceReserve` is stored capacity. A successful refill transfers reserve into active and consumes a turn. Skills never silently consume reserve. Empty reserve and insufficient active resonance must be blocked consistently. Older `Essence`-only combat descriptions are **HISTORICAL**, not permission to create a second resource system.

**Status foundation:** Poison, Burn, SkillSeal, RegenBlock and Regen; advanced stacking, immunity and interactions are **DEFERRED**. Current technical rules and ordering live in [V3 Combat Foundation](V3_Combat_Foundation.md) pending contract-by-contract migration.

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

**IMPLEMENTED — DOCUMENTED:** stable class IDs include `Tlamatini` and `Wanderer`; class definitions and registries, skill definitions, skill trees, scoped unlock progress, class-to-tree bindings, mastery profiles, mentor definitions/relationships, teaching evaluation and application, mastered-skill retention across class changes, progression-aware combat validation and runtime/session ownership. See the existing [Player Progression Foundation](PlayerProgression_Foundation.md) for detailed contracts during transition.

Mentor archetypes: **Lyra** (analysis), **Atlas** (stability), **Gepetto** (crafting/system manipulation) and **Spark** (variance/chaos). The latter two are design directions rather than evidence of complete systems.

Teaching concepts: temporary, fixed and equipped; the entire final persistence/equipment-slot UX is not complete. Mentors may require missions to fix teachings permanently. **DEFERRED:** full class roster, Tlamatini tree content, equipment-taught skills, hybrid mentor teachings, cognition UI and expanded mentor systems.

### 5.1 Learning through equipment — PROMISING / TO REFINE

A Final Fantasy IX-like idea: equipment exposes learnable skills; practice/mastery can retain eligible abilities after unequipping and changing classes. Class identity must still matter: not every mastered skill should automatically become usable in every class. The eligibility/retention policies must be explicit. The existing mastery foundation does **not** establish a complete equipment-learning loop.

### 5.2 Mentor commitments and branching

Mentor teachings may begin temporary; a mentor mission or meaningful commitment can “fix” a teaching in Nova. Paths should alter access to skills and world capabilities, not just grant passive numbers. Exact mentors, missions, slot limits and branching consequences are content/design work, not verified completed quests.

### 6. Philosophy, routes, choice and transmutation

**LOCKED DESIGN:** Shared Wisdom emphasizes mentorship, relationships, knowledge transmitted and community. The alternative **Nahual Path** (working name) explores knowledge extracted or forced from reality; it should be intellectually tempting, dangerous and isolating, **not** a generic evil route.

**PARTIAL — DOCUMENTED:** alignment/progression flags and fact-producing evaluators for teaching acceptance, NPC reactions, vendor restrictions, narrative hooks, mission triggers, Codex candidates and transmutation access. These must not mutate unrelated systems implicitly.

**DEFERRED:** full route content, alternate endings, faction/world consequences and economy balancing. `Transmute Lesser Being` is an early concept: defeated enemies may convert into materials/items with failure, resistance and costs. Wisdom Fragments represent **conceptual knowledge remnants**, not generic collectible crystals. Exact economy is unresolved.

### 6.1 Dark route is a design trade-off

The darker path may reduce or remove access to mentor-based permanent skills while opening transmutation/knowledge extraction. This is a **PROMISING DESIGN DIRECTION** that needs explicit economy, reversibility and narrative-cost decisions; do not assert all such restrictions are implemented. The player should understand the allure before facing consequences.

### 7. Codex, discovery and missions

**IMPLEMENTED — DOCUMENTED:** the V2 narrative foundation favors small evaluators that return facts, tags, hooks and candidate records. Explicit downstream services apply Codex and mission-journal discovery idempotently. No narrative evaluator should directly execute combat, UI, dialogue or reward logic.

**LOCKED DESIGN:** the Codex is where the world reconstructs its memory through Nova. It is more than a wiki: a scholarly, ceremonial, fragmented knowledge instrument. Proposed knowledge domains include world memory, bestiary, classes/masters, skills/teachings, artifacts, regions, factions/philosophies, corrupted knowledge and myths/echoes. Entry stages may progress from unknown to glimpsed, discovered, studied and mastered/reconstructed.

**DEFERRED:** full UI, thresholds, reward bridge, discovery-stage implementation, mission chains, partial objectives and story content. See [V2 Narrative Foundation](V2_Narrative_Foundation.md).

### 7.1 Codex visual experience — PROMISING / TO REFINE

The Codex is a **ceremonial map of reconstructed knowledge** with discovery stages, interlinked topics and subtle animated reveals. Its content may later contribute to Resonance reserve growth and town restoration, but those bridges are not verified in code. Quote from the earlier concept: *“The Codex is not where the player stores information. It is where the world remembers itself through the player.”*

### 8. Villa de las Memorias and Sanctuary

**LOCKED DESIGN / DEFERRED IMPLEMENTATION:** the **Villa de las Memorias** is a restoration of culture, roots, knowledge and function, **not** a free-placement city builder. Learning/mastering teachings can restore or deepen buildings that eventually host shops, mentors, workshops, cooking, alchemy or transmutation. No town implementation until the visual smoke-demo boundary is designed and audited.

**LOCKED DESIGN / DEFERRED IMPLEMENTATION:** a **Sanctuary / Refuge of Reflection** within the restoration fantasy offers quiet, emotionally meaningful remembrance. An NPC may naturally recall a *small selection of actual significant milestones* from the player's journey. Never fabricate remembered events, turn reflection into XP/achievements, show percentages, or force exhaustive recaps. The Codex remembers the world through Nova; the Sanctuary helps Nova remember the journey. Event inputs, persistence and UI ownership remain **UNRESOLVED**.

### 8.1 Restoration, not city construction

Buildings are manifestations of recovered cultural capabilities, not free-placement structures. A mastered skill may restore a workshop, mentor house, kitchen or alchemical/transmutation function. **OPEN:** whether one skill maps to one building, a building upgrade or a shared capability family; how quest gates and visual stages work.

### 8.2 Sanctuary as emotional counterweight

**ACCEPTED DIRECTION / DEFERRED:** it recalls a few actual, meaningful player events in natural dialogue. No XP, score, trophy wall, percentages or moral evaluation. **Design maxim:** *“The Sanctuary never evaluates the player. It remembers them.”* It needs a stable milestone/event model before any UI.

### 8.3 Audio, pacing and exploration — OPEN

Music, environmental audio, encounter pacing, exploration layout, travel systems, encounter presentation and interaction affordances require their own design pass. Do not invent decisions from an absence of records.

# Part II — Technical Architecture & Implementation

### 9. Domain-first architecture and ownership

**LOCKED DESIGN:**
- Pure C# domain and application rules; Unity is an adapter/presentation layer.
- Visual code consumes backend state, results and events; it does not own gameplay rules.
- Combat, progression/cognition, missions, Codex, inventory/equipment, narrative and Unity presentation have explicit boundaries.
- One important public class per file, with narrow exceptions.
- Small, focused, testable services; explicit mutation; no hidden dependencies or parallel engines.
- Preserve Unity `.meta`, GUIDs, scenes, prefabs and serialized references; avoid manual YAML edits.
- Do not regenerate or overwrite the active repository from historical PowerShell snapshots.
- Keep `FoundationVerticalSlice` as a compatibility boundary unless a dedicated branch approves migration.

**IMPLEMENTED — DOCUMENTED:** `LoadedPlayerSession`, session composition/bootstrap and `ProgressionRuntimeDemoBootstrap` establish runtime ownership; registries have appropriate content/session scope; combat objects are per-combat. Full contract details are in [Player Progression Foundation](PlayerProgression_Foundation.md).

### 10. Persistence, save compatibility and adapters

**IMPLEMENTED — DOCUMENTED:** V2 snapshot DTOs, JSON serializer, file repository, save/restore profiles, Unity save-path adapter, and extensions for class/mentor/mastery/tree facts. Compatibility with existing JSON and stable IDs is mandatory. **DEFERRED:** slots, autosave, cloud and broad persistence migrations.

Version-prefixed persistence APIs have a high rename blast radius. Do not rename them as incidental cleanup. See [V2 Save Foundation](V2_Save_Foundation.md).

## 11. Implementation ledger — source-inspected, not runtime-verified

**Status semantics:** `FOUNDATION` = relevant source and contracts found; `PARTIAL` = limited domain support, not end-to-end gameplay; `CONCEPTUAL` = creative direction without verified gameplay implementation; `UNVERIFIED` = insufficient evidence; `VERIFIED` = executed and observed (none newly certified here).

| Area | Design maturity | Code status as of this inspection | Concrete source evidence | Missing or unverified |
|---|---|---|---|---|
| Turn-based combat | Accepted current foundation | **FOUNDATION** | `src/Game/Domain/Combat/TurnBasedCombatEngine.cs`, `src/Game/Slice/FoundationVerticalSlice.cs` | Interactive Unity battle, fresh build/test results |
| Resonance active/reserve | Accepted | **FOUNDATION** | `src/Game/Domain/Combat/Resources/ResonanceProfile.cs`, `ResonanceRefillAction.cs` | Balance, UX, auto-refill, buff effects |
| Refill buffs / auto-refill | Promising | **CONCEPTUAL** | No specific implementation verified | Rules, probability, mentorship integration |
| Hybrid basic attacks | Proposed | **CONCEPTUAL** | Existing engine remains turn-based | Time/turn authority, input, enemies, prototype approval |
| Skill validation | Accepted | **FOUNDATION** | `src/Game/Domain/Combat/CombatSkillUseValidator.cs` | Full content and end-to-end player feedback |
| Classes, skill trees, mastery | Accepted | **FOUNDATION / PARTIAL CONTENT** | `src/Game/Domain/Classes/SkillTreeProgressProfile.cs`, unlock policy/application services; progression smoke tests | Complete roster, UI, balancing |
| Equipment-based skill learning | Promising | **PARTIAL / NOT VERIFIED** | `src/Game/Domain/Equipment/EquipmentDefinition.cs` exists; mastery/retention contracts documented | No verified full equip→learn→master→retain gameplay |
| Mentors and teaching | Accepted foundation | **FOUNDATION / PARTIAL CONTENT** | `src/Game/Domain/Classes/MentorTeachingService.cs` | Quests, full narrative dialogue, final teaching UX |
| Session/bootstrap ownership | Accepted | **FOUNDATION** | `src/Game/Application/Bootstrap/PlayerSessionBootstrap.cs`, `ProgressionRuntimeDemoBootstrap.cs` | Production Unity game-loop integration |
| Narrative and Nahual gate | Accepted foundation | **PARTIAL** | `src/Game/Domain/Transmutation/TransmutationAccessPolicy.cs`, V2 narrative source | Full route consequences and transmutation effects |
| Codex and mission discovery | Accepted foundation | **FOUNDATION / PARTIAL** | `src/Game/Domain/Codex/CodexDiscoveryService.cs`, `DiscoveryOrchestrationService.cs` | Codex UI, full quest system, completion stages |
| Snapshot JSON/save | Accepted foundation | **FOUNDATION** | `src/Game/Domain/Progress/V2ProgressSnapshotJsonSerializer.cs`, file repository, `src/Game.Unity/Persistence/UnityV2SaveService.cs` | Unity Play Mode, save slots, autosave, compatibility run |
| Legacy vertical slice | Historical compatibility | **FOUNDATION** | `FoundationVerticalSlice.Run()`, `src/Game.SmokeTests/SmokeTestRunner.cs` | 38-event count not freshly rerun |
| Villa de las Memorias | Accepted direction | **CONCEPTUAL / DEFERRED** | Backlog references, no dedicated restoration engine verified | Building state, unlocks, persistence, visuals |
| Sanctuary | Accepted direction | **CONCEPTUAL / DEFERRED** | No dedicated Sanctuary class located in code search | Milestone facts, reflection service, UI |
| Full playable game | Vision | **UNVERIFIED / NOT COMPLETE** | Domain foundations and console smoke runner | Playable production loop, content, art/audio, release readiness |

**Audit limitations:** This ledger uses current file fetches and code search, not a recursive repository tree, executed tests, or Unity. A missing search hit is not proof of absence. All `FOUNDATION` claims must be upgraded through workflow verification with file/test evidence. Documented smoke guardrails: **38 legacy events, 100 combat assertions, 899 total assertions at the latest progression documentation checkpoint**; not fresh test results.

## 12. Architecture acceptance and validation gates

- Pure C# domain/application owns gameplay facts and decisions; Unity displays results/events and adapts input/storage.
- Do not add parallel combat, progression, mission or save engines.
- For every implementation claim, record **source path + test path + executed result + known limits**.
- Run `dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal`, `dotnet run --project src/Game.SmokeTests/Game.SmokeTests.csproj --no-build`, `git diff --check`; check Unity adapters manually in Unity.
- Preserve serialization IDs, `.meta` files and V1 slice compatibility; never treat an old generator as current authority.

# Part III — Decisions, Roadmap & Development Governance

### 13. PETTO Mode and audit-driven development

**LOCKED PROCESS:** **Foundation → Workflow Verification → Documentation → Audit → Next Atomic Step.** The audit backlog is the roadmap.

1. Read latest audit/backlog and identify the **earliest unresolved atomic item**.
2. Confirm prior workflow verification and current documentation.
3. Check whether the change would create a parallel system or violate ownership.
4. Plan one purpose per branch; never mix gameplay changes with documentation/tooling cleanup.
5. Implement narrowly; run build, smoke tests, save/restore and compatibility checks where relevant.
6. Record results, update this Bible and audit, then propose the next atomic step.

**Documented guardrails (not freshly rerun here):** legacy slice emits 38 events; combat smoke group has 100 assertions; progression foundation documents 899 total smoke assertions at its latest recorded checkpoint. These are historical documented counts, not proof of the current CI result.

Validation commands documented by the project:

```powershell
dotnet build Fragments-of-Wisdom.sln --no-restore -v:minimal
dotnet run --project src\Game.SmokeTests\Game.SmokeTests.csproj --no-build
git diff --check
```

### 14. Current audited track

The [living backlog](TODO_BACKLOG.md) documents:
1. Finish responsibility-based naming cleanup for progress restore and combat-validation diagnostics.
2. Design/audit the visual smoke-demo boundary.
3. Implement only the smallest approved visual adapter consuming existing state/events.
4. Run workflow verification and audit.
5. Revisit Town Restoration only after that boundary is stable.
6. Explore Sanctuary after event/milestone input contracts are designed.
7. Explore hybrid combat **in a separate design-only audit**; do not smuggle it into the current naming/visual track.

**Explicit exclusions from the cleanup branch:** combat policy changes, new rewards/economy, save-format changes, generator-wide rewrite, broad bootstrap migration, Unity asset renames and gameplay refactors.

### 15. Deferred and open systems

**Deferred:** enemy AI expansion; advanced statuses; Atlas prediction; balance simulator; full class/skill content; item-taught skills; mentor hybrids and slots; mission chains and event tracking; Codex UI/completion/rewards; item/equipment registries, affixes, loot, mastery; richer transmutation and alignment consequences; town buildings and Sanctuary implementation.

**Open decisions:** hybrid combat timing/input architecture; final Nahual terminology; full narrative/world bible; equipment/loot economy; Codex completion semantics; Sanctuary event selection and repetition rules.

## 16. Decision register — conversations vs code

| Decision / idea | Design classification | Implementation classification | Required next evidence |
|---|---|---|---|
| HD-2D with Taíno/Caribbean motifs and unique visual language | **ACCEPTED DIRECTION** | **CONCEPTUAL VISUAL IDENTITY** | Approved art references, in-engine prototype |
| Strategic Resonance refill consumes a turn | **ACCEPTED** | **FOUNDATION** | Smoke execution and UI proof |
| Refill buff “next skill cheaper” | **PROMISING / TO REFINE** | **CONCEPTUAL** | Written balance rules and tests |
| Auto-refill via mentor/class mastery | **PROMISING / TO REFINE** | **CONCEPTUAL** | Unlock, cost and timing contract |
| Hybrid combat | **PROPOSED** | **CONCEPTUAL** | Dedicated design audit |
| Equipment mastery like FFIX | **PROMISING / TO REFINE** | **PARTIAL FOUNDATION** | Equip/learn/master/retain E2E verification |
| Codex ceremonial knowledge map | **ACCEPTED DIRECTION** | **CONCEPTUAL UI** | UI spec, state model and prototype |
| Villa restoration from knowledge | **ACCEPTED DIRECTION** | **DEFERRED** | Visual smoke boundary, restoration model |
| Sanctuary reflects real milestones | **ACCEPTED DIRECTION** | **DEFERRED** | Milestone fact model and narrative audit |
| Nahual trade-off: lost mentors vs transmutation | **PROMISING / TO REFINE** | **PARTIAL POLICY ONLY** | Consequence design and execution |

### 17. Documentation consolidation ledger

| Existing file | Proposed destination | Action before acceptance |
|---|---|---|
| `docs/FOUNDATION_MASTER_SPEC.md` | Chapters 1–6, 9 | Historical ideas migrated; inspect for missed details |
| `docs/V3_Combat_Foundation.md` | Chapter 4 | Retain until all technical contracts migrated |
| `docs/V2_Narrative_Foundation.md` | Chapters 5–6 | Retain until all service boundaries migrated |
| `docs/V2_Save_Foundation.md` | Chapter 10 | Retain until all DTO/compatibility details migrated |
| `docs/PlayerProgression_Foundation.md` | Chapters 3, 9–10 | **High-density:** requires contract-by-contract audit |
| `docs/Foundation_Version_Prefix_Naming_Audit.md` | Chapters 11–12 and historical ledger | Verify completed vs pending rename items |
| `docs/TODO_BACKLOG.md` | Chapters 11–13 | Retain until audit/roadmap migration verified |
| `docs/CODEX_WORKFLOW.md`, `docs/STRUCTURE_NOTES.md` | Chapters 9, 11 | Verify no agent instructions lost |
| `docs/GENERATOR_USAGE.md` | Historical tooling | Check live references before retiring |
| `README.md` | Short project entry point | Rewrite after Bible acceptance |
| `AGENTS.md` | Minimal agent entry point to Bible | **Keep:** agents need discoverable instructions |

**No source documents are deleted in this draft.** The final cleanup requires a traceability checklist, broken-link scan, code/test validation and explicit approval before removal.

### 18. Source provenance and change policy

The source set is the repository's living docs listed above, earlier visual/Codex concepts and the project's design conversations. The prior consolidated DOCX was a starting point, **not evidence that every old technical contract has been migrated**.

Each new approved design decision should update the relevant chapter and its status; each implementation branch should update the verified state and audit. A mismatch between code and Bible is an **audit finding**, never an excuse for silent code changes.

**Acceptance gate:** confirm complete inventory; review each original file against this Bible; audit code/tests and reference links; approve unresolved design choices; only then designate this file the sole governing document.

### 18.1 Provenance classification

- **CODE INSPECTED:** concrete source paths in Chapter 11, from `main` at the time of drafting.
- **REPOSITORY DOCUMENTED:** `docs/TODO_BACKLOG.md`, `docs/PlayerProgression_Foundation.md`, V2 narrative/save and V3 combat foundations.
- **CONVERSATION DESIGN:** visual language, Codex concept, Resonance buff/auto-refill, equipment-learning fantasy, town, Sanctuary, philosophical routes and hybrid-combat exploration. Their status reflects what was discussed, **not an invented implementation claim**.
- **OPEN / NOT VERIFIED:** design decisions or technical facts without enough evidence.

### 18.2 Next documentation audit

1. Compare every section of the dense progression foundation against Chapters 5, 9–12.
2. Cross-check live tests and Unity adapter integration, record exact results.
3. Recover any additional approved concepts from older visual, Codex and vertical-slice documents; mark unverified details `OPEN`.
4. Update `README.md` and `AGENTS.md` only after the new chapter structure is accepted.
5. Retire old documents **only after** a reference scan and user review; never as part of gameplay changes.


# Part IV — Executed Code Audit and Integrity Backlog (2026-10-30)

> **Evidence source:** Codex read-only audit of `main`, reported against implementation baseline `45fb4548` and Bible v3 `c11c74d`. The follow-up executed twelve in-memory probes against a compiled assembly. This chapter is the current **executed evidence**, superseding any older optimistic implementation labels. It does not claim Unity verification.

## 19. Executed verification baseline

- `dotnet restore` and `dotnet build`: **0 warnings, 0 errors**.
- Smoke assertions **899**: V1 **2**, Shared **16**, V2 **264**, V3 **100**, Progression **517**; legacy V1 emits **38 events**.
- Twelve additional in-memory behavioral checks **were not added to the smoke assertion count**.
- `git diff --check` and `git diff --exit-code HEAD` succeeded. Codex did not change tracked files.
- Unity project assets, scenes, prefabs, editor compilation, Play Mode, presentation and a playable vertical slice **were not verified**. A clean .NET build is not a playable-game certificate.

## 20. Reproduced integrity findings — actionable audit register

| ID | Severity from executed probe | Reproduced behavior | Affected code | Required acceptance direction |
|---|---|---|---|---|
| AUD-01 | HIGH | Restore with null mastery destination changes class, then throws `ArgumentNullException` | `src/Game/Domain/Progress/V4ProgressRestoreCoordinator.cs` | Prevalidate all destinations; zero mutations on rejected restore |
| AUD-02 | HIGH | Unlocked tree node without prerequisite: semantic validator returns false, coordinator returns `FullyRestored=true` | `V2ProgressSnapshotService.cs`, validation service, coordinator | Registry-aware semantic preflight in actual restore path |
| AUD-03 | HIGH | Codex snapshot→JSON→restore loses legacy discovery and completion; enriched record survives | `V2ProgressSnapshotService.cs`, Codex profile | Explicit backward-compatible Codex round trip; preserve all supported facts |
| AUD-04 | HIGH | Alternate definition with same skill ID bypasses canonical unlock requirement | `src/Game/Domain/Combat/CombatSkillUseValidator.cs` | Resolve and validate canonical registered definition; reject conflicting request metadata |
| AUD-05 | MEDIUM | Enemy skill spends 10 from the runtime's single Resonance pool | `TurnBasedCombatEngine.cs` | Decide per-actor resource ownership before implementation |
| AUD-06 | MEDIUM | Refill with Poison advances turn but neither damages nor decrements duration | `ResonanceRefillAction.cs` | Define and integrate canonical turn/status lifecycle |
| AUD-07 | MEDIUM | Refill after combat end succeeds, restores resource and advances turn | `ResonanceRefillAction.cs` | Enforce combat lifecycle boundary |
| AUD-08 | MEDIUM | `SilenceDebuff` emits `ActionBlocked`, yet `SkillUsed`, XP and damage still occur | `SilenceDebuff.cs`, combat engine | Make blocked action truly non-executable; consistent events |
| AUD-09 | MEDIUM | Mastery 50 + `int.MaxValue` wraps to `-2147483599` / `Unranked` | `MasteryRecord.cs` | Overflow-safe bounded arithmetic |
| AUD-10 | MEDIUM | Teaching +50 at mastery 90 caps at 100 but reports `ProgressApplied=50` rather than 10 | `MentorTeachingApplicationService.cs` | Report actual applied delta |
| AUD-11 | MEDIUM | `ResourcePool.TrySpend(-10)` succeeds, `Current=110` when `Max=100` | `ResourcePool.cs` | Reject invalid spend amounts; preserve pool invariants |
| AUD-12 | MEDIUM | Failed preflight reports `ClassRestored=true` despite unchanged destination | `V4ProgressRestoreCoordinator.cs` | Separate feasibility/preflight result from actual mutation facts |

**Correction:** `Defend` and `Wait` exist in an enum, but `TurnRequest` only publicly constructs basic attacks and skills. Classify these as **incomplete contracts**, not reproduced failures through the current public API.

## 21. Code-grounded implementation clarifications

- **Combat:** backend attack/skill execution is tested; complete dispatch, target/lifecycle semantics, refill and legacy status blocking are **PARTIAL**.
- **Resonance:** active/reserve and spending transfer are tested; refill is a **separate mutation path**, not fully integrated into turn lifecycle. Random buffs, discount, auto-refill and Codex/mastery-driven reserve growth remain **NOT IMPLEMENTED**.
- **Skills/mastery:** distinguish XP-based `SkillProgress.IsMastered` from retention `MasteryProfile`. Existing retention rules are not equivalent to a future class eligibility policy. Equipment stores `TaughtSkillId`, but the complete equip→learn→master→retain loop is **NOT VERIFIED**.
- **Codex:** discovery and enriched records work for tested flows, but legacy completion/discovery data are lost in the reproduced round trip.
- **Persistence:** supported snapshot flows work; restore merges into existing profiles, lacks fully atomic semantics, and does **not** cover complete player cognition/inventory/wallet/alignment. Unity adapter coverage is narrower and unverified in Editor.
- **Narrative:** Nahual transmutation access policy exists; enemy-to-item transmutation and its economy do not.
- **Town/Sanctuary/hybrid combat:** **DESIGN ONLY / DEFERRED**, no production gameplay implementation verified.
- **Architecture:** Domain/Application have no Unity dependency found, but currently share one `net10.0` assembly rather than separately enforced assembly boundaries.

## 22. Risk-first, audit-driven atomic backlog

**Approved proposed reordering requires explicit review before implementation.** The older naming-first backlog remains preserved below as historical provenance. Current recommended sequence:

1. `AUD-01` Restore destination validation and no partial mutation on failure. Separate branch, regression probe, workflow verification, documentation, audit.
2. `AUD-02` Registry-aware tree semantic preflight in actual restore path.
3. `AUD-03` Legacy Codex compatibility round-trip and schema decision.
4. `AUD-04` Canonical skill-definition validation.
5. `AUD-09`/`AUD-10`/`AUD-11`/`AUD-12` each individually scoped as needed; do not hide correctness repairs in rename work.
6. `AUD-05`/`AUD-06`/`AUD-07`/`AUD-08` require explicit combat lifecycle and actor-resource ownership decisions before atomic fixes.
7. Resume existing `codex/rename-progress-restore-api` and subsequent naming/visual boundary track after risk-based approval.

**Gate for every branch:** Foundation → Workflow Verification → Documentation → Audit → Next Atomic Step. Do not implement unrelated gameplay during repairs.

## 23. Consolidation policy and historical record

**This file is the single authoritative specification.** Parts I–IV are the active interpretation; the verbatim source records below are **historical evidence and detailed contracts**, not separate competing instructions. Where source records conflict with executed findings, prefer Chapter 20–22 and verify in code. Where a detail is absent from the active chapters, retain its technical contract from the historical record pending an explicit reconciliation; never silently discard it. Historical counts and branch plans are not current test results. The source records are preserved verbatim to prevent losing overload signatures, edge cases, naming audits, workflow instructions and design rationale during cleanup.

# Part V — Consolidated Historical Technical Source Records (verbatim archival)

## Archive 01 — docs/FOUNDATION_MASTER_SPEC.md

> Original SHA: `98cc5f90f0f62d10b0e39c443f5b5d5e229b28f6`. **Archived source text; not a separate governing document.**

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

---

## Archive 02 — docs/V3_Combat_Foundation.md

> Original SHA: `08d2348ef926e3ad136e4ef0b62d017ee746cdaa`. **Archived source text; not a separate governing document.**

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

---

## Archive 03 — docs/V2_Narrative_Foundation.md

> Original SHA: `3e20aec0ae8357fa0afdb4ea1326fcc25e35e469`. **Archived source text; not a separate governing document.**

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

---

## Archive 04 — docs/V2_Save_Foundation.md

> Original SHA: `640ff15d099b2e9e4f79fac1034fe29abd62047f`. **Archived source text; not a separate governing document.**

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

---

## Archive 05 — docs/PlayerProgression_Foundation.md

> Original SHA: `9b6e1a70d52cd8d5e221db33c969fc7084628b4e`. **Archived source text; not a separate governing document.**

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

---

## Archive 06 — docs/Foundation_Version_Prefix_Naming_Audit.md

> Original SHA: `c231625cc1852edd2129cb633352a98d49f7b80e`. **Archived source text; not a separate governing document.**

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

---

## Archive 07 — docs/TODO_BACKLOG.md

> Original SHA: `01bf92203ef6d2a9c00ae446e77b05505126d888`. **Archived source text; not a separate governing document.**

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

---

## Archive 08 — docs/CODEX_WORKFLOW.md

> Original SHA: `5277f5003f52e0f4dc65f0905ed79f13cda8b4e9`. **Archived source text; not a separate governing document.**

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

---

## Archive 09 — docs/STRUCTURE_NOTES.md

> Original SHA: `6881774e8f133fabb60c2b4815503f4f2463c51c`. **Archived source text; not a separate governing document.**

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

---

## Archive 10 — docs/GENERATOR_USAGE.md

> Original SHA: `c73c308cd3aa3030933f767cd740bf12dfe2f45a`. **Archived source text; not a separate governing document.**

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

---

## Archive 11 — docs/Workflow_Verification_Checklist.md

> Original SHA: `edd9df140fe118bf764b12e6e7f0ddb2d440db0c`. **Archived source text; not a separate governing document.**

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

---

