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
