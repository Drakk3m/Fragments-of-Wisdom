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
