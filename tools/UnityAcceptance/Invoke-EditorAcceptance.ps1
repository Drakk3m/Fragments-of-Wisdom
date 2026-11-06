#requires -Version 7.2
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string] $UnityEditor,
    [switch] $PrepareOnly
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$project = Join-Path $repo 'unity/FragmentsOfWysdom.Acceptance'
if (-not $IsWindows) { throw 'This runner currently supports Windows Unity Editor only.' }
if (-not (Test-Path -LiteralPath $UnityEditor -PathType Leaf)) { throw 'UnityEditor must point to an installed Unity.exe; no installation is performed.' }
$UnityEditor = (Resolve-Path -LiteralPath $UnityEditor).Path
if ([IO.Path]::GetFileName($UnityEditor) -ne 'Unity.exe') { throw 'Expected an explicit Unity.exe path.' }
if (Test-Path -LiteralPath (Join-Path $project 'Temp/UnityLockfile')) { throw 'Close the acceptance project before preparation/batch execution.' }
$run = Join-Path $project ('Evidence/run-' + [DateTime]::UtcNow.ToString('yyyyMMddTHHmmssfffZ'))
New-Item -ItemType Directory -Path $run -Force | Out-Null

function Invoke-Editor([string[]] $Arguments) {
    $process = Start-Process -FilePath $UnityEditor -ArgumentList $Arguments -WindowStyle Hidden -PassThru
    if (-not $process.WaitForExit(900000)) {
        $process.Kill()
        $process.WaitForExit()
        throw 'Unity timed out after 15 minutes; retain the log and resolve licensing/import errors before retrying.'
    }
    return $process.ExitCode
}

$versionOut = Join-Path $run 'editor-version.txt'
$versionErr = Join-Path $run 'editor-version-errors.txt'
$versionProcess = Start-Process -FilePath $UnityEditor -ArgumentList '-version' -WindowStyle Hidden -PassThru `
    -RedirectStandardOutput $versionOut -RedirectStandardError $versionErr
if (-not $versionProcess.WaitForExit(60000)) { $versionProcess.Kill(); $versionProcess.WaitForExit(); throw 'Unity -version timed out.' }
$versionText = Get-Content -LiteralPath $versionOut -Raw
if ($versionProcess.ExitCode -ne 0 -or [string]::IsNullOrWhiteSpace($versionText) -or $versionText.Trim() -notmatch '^6000\.3\.\d+f\d+$') {
    throw "Unsupported/unresolved Editor version '$versionText'. Select an installed final Unity 6.3 LTS patch in Hub."
}
$version = $versionText.Trim()
# Unity 6.3 documents Test Framework as an Editor-bundled core package. No guessed version/fallback download.
$packageJson = Join-Path (Split-Path $UnityEditor) 'Data/Resources/PackageManager/BuiltInPackages/com.unity.test-framework/package.json'
if (-not (Test-Path -LiteralPath $packageJson)) {
    throw "Bundled Test Framework metadata not found: $packageJson. Inspect this Editor installation and review its package layout; no version is fabricated."
}
$package = Get-Content -LiteralPath $packageJson -Raw | ConvertFrom-Json
if ($package.name -ne 'com.unity.test-framework' -or $package.version -notmatch '^\d+\.\d+\.\d+([-.][A-Za-z0-9.-]+)?$') {
    throw 'Invalid bundled Unity Test Framework metadata.'
}
& (Join-Path $PSScriptRoot 'Stage-Backend.ps1')
$manifest = Get-Content -LiteralPath (Join-Path $project 'Packages/manifest.template.json') -Raw | ConvertFrom-Json -AsHashtable
$manifest.dependencies['com.unity.test-framework'] = $package.version
$manifest | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $project 'Packages/manifest.json') -Encoding utf8NoBOM
"m_EditorVersion: $version" | Set-Content -LiteralPath (Join-Path $project 'ProjectSettings/ProjectVersion.txt') -Encoding utf8NoBOM
Copy-Item -LiteralPath (Join-Path $project 'Evidence/staged-artifacts.json') -Destination $run
Copy-Item -LiteralPath (Join-Path $project 'Packages/manifest.json') -Destination $run
Copy-Item -LiteralPath $packageJson -Destination (Join-Path $run 'test-framework-package.json')
[ordered]@{ PreparedUtc = [DateTime]::UtcNow.ToString('o'); UnityVersion = $version; TestFramework = $package.version;
    ApiProfileRequired = 'NET_Standard (.NET Standard 2.1)'; EditorExecution = 'UNVERIFIED until test XML and environment evidence pass' } |
    ConvertTo-Json | Set-Content -LiteralPath (Join-Path $run 'preparation.json') -Encoding utf8NoBOM
if ($PrepareOnly) { Write-Host "Prepared $version; open $project in Hub. No tests executed. Evidence: $run"; return }

$previousEvidence = $env:FW_ACCEPTANCE_EVIDENCE
$env:FW_ACCEPTANCE_EVIDENCE = $run
try {
    $configureExit = Invoke-Editor @('-batchmode', '-nographics', '-projectPath', "`"$project`"", '-buildTarget', 'Win64',
        '-executeMethod', 'FragmentsOfWysdom.Acceptance.AcceptanceEnvironment.Configure', '-quit',
        '-logFile', "`"$(Join-Path $run 'configure.log')`"")
    if ($configureExit -ne 0) { throw "Editor configuration failed ($configureExit). Inspect configure.log." }
    $results = Join-Path $run 'results.xml'
    # -quit must not accompany -runTests. The test runner owns process completion.
    $testExit = Invoke-Editor @('-batchmode', '-nographics', '-projectPath', "`"$project`"", '-buildTarget', 'Win64',
        '-runTests', '-testPlatform', 'EditMode', '-assemblyNames', 'FragmentsOfWysdom.Acceptance.Editor',
        '-testResults', "`"$results`"", '-logFile', "`"$(Join-Path $run 'Editor.log')`"")
    $passed = $false
    if (Test-Path -LiteralPath $results) {
        [xml]$xml = Get-Content -LiteralPath $results -Raw
        $cases = @($xml.SelectNodes('//test-case'))
        $passed = $testExit -eq 0 -and $xml.'test-run'.result -eq 'Passed' -and $cases.Count -eq 7 -and
            @($cases | Where-Object result -ne 'Passed').Count -eq 0 -and (Test-Path -LiteralPath (Join-Path $run 'environment.txt'))
    }
    [ordered]@{ FinishedUtc = [DateTime]::UtcNow.ToString('o'); UnityExitCode = $testExit; AllSevenTestsPassed = $passed;
        Acceptance = 'Requires independent review; never automatically authorizes P9.7' } |
        ConvertTo-Json | Set-Content -LiteralPath (Join-Path $run 'execution.json') -Encoding utf8NoBOM
    if (-not $passed) { throw "Editor acceptance failed/incomplete; preserve all evidence in $run. Missing/zero/skipped tests are not PASS." }
    Write-Host "Seven real Editor test cases passed. Submit $run for independent review; P9.6 is not closed automatically."
} finally {
    $env:FW_ACCEPTANCE_EVIDENCE = $previousEvidence
    if (Test-Path -LiteralPath (Join-Path $project 'Packages/packages-lock.json')) {
        Copy-Item -LiteralPath (Join-Path $project 'Packages/packages-lock.json') -Destination $run
    }
}
