#requires -version 5.1
<#
NovaFoundation consolidated generator.
PowerShell 5.1 compatible.

This generator creates NovaFoundation from scratch using a selected vertical slice profile.
It avoids accumulating endless incremental patch scripts by keeping the feature writers
inside Modules and selecting the intended slice through -Profile.

Examples:
  .\Generate-NovaFoundation.ps1 -Profile VerticalSliceV2 -Force
  .\Generate-NovaFoundation.ps1 -Root "C:\Dev\NovaFoundation" -Profile VerticalSliceV2 -Force
  .\Generate-NovaFoundation.ps1 -Profile VerticalSliceV1 -Force
#>

param(
    [string]$Root = (Join-Path (Get-Location) "NovaFoundation"),

    [ValidateSet("VerticalSliceV1", "VerticalSliceV2")]
    [string]$Profile = "VerticalSliceV2",

    [switch]$Force
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = "Stop"

function Invoke-GeneratorModule
{
    param(
        [string]$ModuleName,
        [string]$TargetRoot
    )

    $modulePath = Join-Path $script:ModulesRoot $ModuleName

    if (-not (Test-Path -LiteralPath $modulePath))
    {
        throw "Generator module not found: $modulePath"
    }

    Write-Host ""
    Write-Host "[MODULE] $ModuleName" -ForegroundColor Cyan
    & $modulePath -Root $TargetRoot
}

$ScriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$ModulesRoot = Join-Path $ScriptRoot "Modules"

Write-Host ""
Write-Host "NovaFoundation Consolidated Generator" -ForegroundColor Green
Write-Host "Profile: $Profile"
Write-Host "Target : $Root"
Write-Host ""

if ((Test-Path -LiteralPath $Root) -and -not $Force)
{
    throw "Target folder already exists: $Root. Re-run with -Force to recreate it."
}

if ((Test-Path -LiteralPath $Root) -and $Force)
{
    Write-Host "[CLEAN] Removing existing target folder." -ForegroundColor Yellow
    Remove-Item -LiteralPath $Root -Recurse -Force
}

$baseModules = @(
    "00_Create_Structure_And_Docs.ps1",
    "01_Common_And_Combat_Core.ps1",
    "02_Status_Skills_Resolvers_Battle.ps1",
    "03_Player_Cognition_Classes_Equipment_Codex.ps1",
    "04_Missions_AI_Persistence.ps1",
    "05_Slice_And_Validation.ps1"
)

foreach ($module in $baseModules)
{
    Invoke-GeneratorModule -ModuleName $module -TargetRoot $Root
}

if ($Profile -eq "VerticalSliceV2")
{
    Invoke-GeneratorModule -ModuleName "06_Iteration2_NahualPath.ps1" -TargetRoot $Root
}

Write-Host ""
Write-Host "NovaFoundation generated successfully." -ForegroundColor Green
Write-Host "Location: $Root"
Write-Host "Profile : $Profile"
Write-Host ""
Write-Host "Suggested next step:"
Write-Host "  cd `"$Root`""
Write-Host "  git init"
