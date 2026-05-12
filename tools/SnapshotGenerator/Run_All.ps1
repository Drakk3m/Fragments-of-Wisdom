#requires -version 5.1

param(
    [string]$Root = (Join-Path (Get-Location) "NovaFoundation"),
    [switch]$Force
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = "Stop"

if ((Test-Path -LiteralPath $Root) -and $Force)
{
    Remove-Item -LiteralPath $Root -Recurse -Force
}

$here = Split-Path -Parent $MyInvocation.MyCommand.Path

& (Join-Path $here "00_Create_Structure_And_Docs.ps1") -Root $Root
& (Join-Path $here "01_Common_And_Combat_Core.ps1") -Root $Root
& (Join-Path $here "02_Status_Skills_Resolvers_Battle.ps1") -Root $Root
& (Join-Path $here "03_Player_Cognition_Classes_Equipment_Codex.ps1") -Root $Root
& (Join-Path $here "04_Missions_AI_Persistence.ps1") -Root $Root
& (Join-Path $here "05_Slice_And_Validation.ps1") -Root $Root
& (Join-Path $here "06_Iteration2_NahualPath.ps1") -Root $Root

Write-Host ""
Write-Host "NovaFoundation V2 generated successfully at:" -ForegroundColor Green
Write-Host "  $Root"
Write-Host ""
Write-Host "Suggested next step: git init"
