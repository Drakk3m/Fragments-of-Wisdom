#requires -Version 7.2
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string] $UnityEditor,
    [switch] $PrepareOnly
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
Import-Module (Join-Path $PSScriptRoot 'AcceptanceRunner.psm1') -Force
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$project = Join-Path $repo 'unity/FragmentsOfWysdom.Acceptance'
$run = Join-Path $project ('Evidence/run-' + [DateTime]::UtcNow.ToString('yyyyMMddTHHmmssfffZ') + '-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $run -Force | Out-Null
$results = Join-Path $run 'results.xml'
$state = [ordered]@{ StartedUtc = [DateTime]::UtcNow.ToString('o'); FinishedUtc = $null; Stage = 'Preflight';
    Status = 'Running'; FailureClassification = 'None'; AllSevenTestsPassed = $false; TestOutcome = 'NotVerified';
    ResultsStatus = 'Missing'; Processes = @(); LogSignals = @(); Exception = $null; Recovery = $null;
    Acceptance = 'Requires independent review; never automatically authorizes P9.7' }
$previousEvidence = $env:FW_ACCEPTANCE_EVIDENCE

function Confirm-EditorProcess($Process, [string] $EditorLog = '') {
    $state.Processes += $Process
    $failure = Get-AcceptanceProcessFailure $Process @($Process.Stdout, $Process.Stderr, $EditorLog)
    $state.LogSignals += $failure.Signals
    if ($failure.Classification -ne 'None') {
        $state.FailureClassification = $failure.Classification
        throw "Unity $($state.Stage): $($failure.Classification), exit=$($Process.ExitCode), timeout=$($Process.TimedOut). Inspect $run including stdout/stderr and Editor logs."
    }
}

try {
    Write-AcceptanceJson $state (Join-Path $run 'execution.json')
    if (-not $IsWindows) { throw 'This runner currently supports Windows Unity Editor only.' }
    $UnityEditor = Assert-AcceptanceEditorPath $UnityEditor
    if (Test-Path -LiteralPath (Join-Path $project 'Temp/UnityLockfile')) { throw 'Close the acceptance project before preparation/batch execution.' }
    $state.Stage = 'EditorVersion'
    $versionProcess = Invoke-AcceptanceProcess $UnityEditor @('-version') 'editor-version' $run 60
    Confirm-EditorProcess $versionProcess
    $version = Get-AcceptanceEditorVersion (Get-Content -LiteralPath $versionProcess.Stdout -Raw)

    $state.Stage = 'PackageDiscovery'
    $package = Resolve-AcceptanceTestFramework $UnityEditor $version $run
    $state.Stage = 'BackendStaging'
    & (Join-Path $PSScriptRoot 'Stage-Backend.ps1') *>&1 | Tee-Object -FilePath (Join-Path $run 'staging.log') | Out-Host
    $state.Stage = 'ProjectPreparation'
    $manifest = Get-Content -LiteralPath (Join-Path $project 'Packages/manifest.template.json') -Raw | ConvertFrom-Json -AsHashtable
    # Use the selected installation's actual local package, not a registry replacement or version override.
    $manifest.dependencies['com.unity.test-framework'] = $package.Dependency
    Write-AcceptanceJson $manifest (Join-Path $project 'Packages/manifest.json')
    "m_EditorVersion: $version" | Set-Content -LiteralPath (Join-Path $project 'ProjectSettings/ProjectVersion.txt') -Encoding utf8NoBOM
    Copy-Item -LiteralPath (Join-Path $project 'Evidence/staged-artifacts.json') -Destination $run
    Copy-Item -LiteralPath (Join-Path $project 'Packages/manifest.json') -Destination $run
    Write-AcceptanceJson ([ordered]@{ PreparedUtc = [DateTime]::UtcNow.ToString('o'); UnityVersion = $version;
        TestFramework = $package.Version; PackageSource = $package.Path; PackageSHA256 = $package.SHA256;
        ApiProfileRequired = 'NET_Standard (.NET Standard 2.1)'; EditorExecution = 'UNVERIFIED until real test XML and environment evidence pass' }) `
        (Join-Path $run 'preparation.json')
    if ($PrepareOnly) {
        $state.Status = 'PreparedOnly'
        Write-Host "Prepared $version; open $project in Hub. No tests executed. Evidence: $run"
        return
    }

    $env:FW_ACCEPTANCE_EVIDENCE = $run
    $state.Stage = 'ConfigureAndImport'
    $configureLog = Join-Path $run 'configure.log'
    $configure = Invoke-AcceptanceProcess $UnityEditor @('-batchmode', '-nographics', '-projectPath', "`"$project`"", '-buildTarget', 'Win64',
        '-executeMethod', 'FragmentsOfWysdom.Acceptance.AcceptanceEnvironment.Configure', '-quit', '-logFile', "`"$configureLog`"") 'configure' $run
    Confirm-EditorProcess $configure $configureLog
    $state.Stage = 'EditorTests'
    $editorLog = Join-Path $run 'Editor.log'
    # -quit must not accompany -runTests. The test runner owns process completion.
    $tests = Invoke-AcceptanceProcess $UnityEditor @('-batchmode', '-nographics', '-projectPath', "`"$project`"", '-buildTarget', 'Win64',
        '-runTests', '-testPlatform', 'EditMode', '-assemblyNames', 'FragmentsOfWysdom.Acceptance.Editor',
        '-testResults', "`"$results`"", '-logFile', "`"$editorLog`"") 'tests' $run
    $state.ResultsStatus = if (Test-Path -LiteralPath $results) { 'PresentUnverified' } else { 'Missing' }
    Confirm-EditorProcess $tests $editorLog
    $state.Stage = 'ResultsValidation'
    $assessment = Test-AcceptanceResults $results (Join-Path $run 'environment.txt') $version $package.Version
    $state.ResultsStatus = $assessment.ResultsStatus
    $state.TestOutcome = $assessment.TestOutcome
    $state['ResultsAssessment'] = $assessment
    if (-not $assessment.Passed) {
        $state.FailureClassification = $assessment.Classification
        throw "Editor acceptance is not verified: $($assessment.Classification). Missing/invalid XML is not a successful or partial test run. Preserve $run."
    }
    $state.AllSevenTestsPassed = $true
    $state.Status = 'PassedPendingReview'
    Write-Host "Seven real Editor cases passed with matching version/package evidence. Submit $run for independent review; P9.6 remains open until review."
} catch {
    $state.Status = 'Failed'
    $state.AllSevenTestsPassed = $false
    if ($state.FailureClassification -eq 'None') { $state.FailureClassification = $state.Stage + 'Failed' }
    $state.Exception = [ordered]@{ Message = $_.Exception.Message; Detail = $_.Exception.ToString();
        ErrorRecord = ($_ | Out-String); ScriptStackTrace = $_.ScriptStackTrace }
    $state.Recovery = 'Preserve this entire evidence directory. Inspect configure.log/Editor.log and per-process stdout/stderr for the first compiler, assembly, package, import or license error. Verify the selected Editor/package metadata and staged DLLs; fix the environment or request architectural review, not production changes or disabled validation. Missing XML requires a fresh complete run after diagnosis.'
    Write-AcceptanceJson $state (Join-Path $run 'execution.json')
    $state.Exception.ErrorRecord | Set-Content -LiteralPath (Join-Path $run 'failure.txt') -Encoding utf8NoBOM
    Write-Warning "Acceptance failed at $($state.Stage): $($state.FailureClassification). No acceptance claimed. Evidence: $run"
    throw
} finally {
    $env:FW_ACCEPTANCE_EVIDENCE = $previousEvidence
    $state.FinishedUtc = [DateTime]::UtcNow.ToString('o')
    Write-AcceptanceJson $state (Join-Path $run 'execution.json')
    if (Test-Path -LiteralPath (Join-Path $project 'Packages/packages-lock.json')) {
        try { Copy-Item -LiteralPath (Join-Path $project 'Packages/packages-lock.json') -Destination $run }
        catch { Write-Warning "Could not capture existing package lock: $($_.Exception.Message)" }
    }
}
