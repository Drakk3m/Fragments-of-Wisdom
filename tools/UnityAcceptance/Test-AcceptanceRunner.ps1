#requires -Version 7.2
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
Import-Module (Join-Path $PSScriptRoot 'AcceptanceRunner.psm1') -Force
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$evidenceRoot = Join-Path $repo 'unity/FragmentsOfWysdom.Acceptance/Evidence'
$root = Join-Path $evidenceRoot ('runner-tests-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $root -Force | Out-Null
$script:count = 0

function Check([bool] $Condition, [string] $Message) {
    if (-not $Condition) { throw "Runner regression failed: $Message" }
    $script:count++
}
function Reject([scriptblock] $Action, [string] $Pattern) {
    try { & $Action | Out-Null } catch {
        Check ($_.Exception.Message -match $Pattern) "Expected rejection '$Pattern', got: $($_.Exception.Message)"
        return
    }
    throw "Expected rejection: $Pattern"
}
function Fixture([string] $Name) {
    $path = Join-Path $root $Name
    New-Item -ItemType Directory -Path $path -Force | Out-Null
    # Synthetic filesystem metadata only. Never executed or reported as an installed Unity Editor.
    [IO.File]::WriteAllText((Join-Path $path 'Unity.exe'), 'synthetic path fixture, not executable')
    return $path
}
function Package([string] $Fixture, [string] $Relative, [string] $Json) {
    $path = Join-Path $Fixture ('Data/Resources/PackageManager/' + $Relative + '/package.json')
    New-Item -ItemType Directory -Path (Split-Path $path) -Force | Out-Null
    [IO.File]::WriteAllText($path, $Json)
    return $path
}
function Discover([string] $Fixture) {
    return Resolve-AcceptanceTestFramework (Join-Path $Fixture 'Unity.exe') '6000.3.123f1' $Fixture
}

try {
    Reject { Assert-AcceptanceEditorPath (Join-Path $root 'missing/Unity.exe') } 'installed Unity.exe'
    $wrong = Join-Path $root 'Hub.exe'
    [IO.File]::WriteAllText($wrong, 'not an Editor')
    Reject { Assert-AcceptanceEditorPath $wrong } 'not Hub'
    Check ((Get-AcceptanceEditorVersion "6000.3.123f1`r`n") -eq '6000.3.123f1') 'Exact final release parser'
    foreach ($invalid in @('', '6000.2.123f1', '6000.3.123b1', '6000.3.123f1 extra')) {
        Reject { Get-AcceptanceEditorVersion $invalid } 'Unsupported/unresolved'
    }
    $missing = Fixture 'missing-package-root'
    Reject { Discover $missing } 'no PackageManager directory'
    Check (Test-Path -LiteralPath (Join-Path $missing 'package-discovery.json')) 'Missing-root discovery report retained'
    $validJson = '{"name":"com.unity.test-framework","version":"1.2.3","unity":"6000.3","unityRelease":"1f1"}'
    $folder = Fixture 'unpacked'
    $null = Package $folder 'BuiltInPackages/com.unity.test-framework' $validJson
    $found = Discover $folder
    Check ($found.Version -eq '1.2.3' -and $found.Kind -eq 'Folder' -and $found.Dependency.StartsWith('file:')) 'Unpacked installed package, local dependency'
    $alternate = Fixture 'versioned-folder'
    $null = Package $alternate 'Editor/com.unity.test-framework@1.2.3' $validJson
    Check ((Discover $alternate).Version -eq '1.2.3') 'Versioned folder does not depend on BuiltInPackages spelling'
    $unrelated = Fixture 'unrelated-package'
    $null = Package $unrelated 'BuiltInPackages/com.unity.other' '{"name":"com.unity.other","version":"9.9.9"}'
    Reject { Discover $unrelated } 'No local com.unity.test-framework'
    foreach ($case in @(
        @{ Name='corrupt'; Json='{invalid'; Pattern='Invalid package JSON' },
        @{ Name='wrong-name'; Json='{"name":"com.unity.other","version":"1.2.3"}'; Pattern='wrong/missing name' },
        @{ Name='no-version'; Json='{"name":"com.unity.test-framework"}'; Pattern='Missing/invalid exact' },
        @{ Name='range-version'; Json='{"name":"com.unity.test-framework","version":"^1.2.3"}'; Pattern='Missing/invalid exact' },
        @{ Name='too-new'; Json='{"name":"com.unity.test-framework","version":"1.2.3","unity":"6000.4"}'; Pattern='too old' },
        @{ Name='patch-too-new'; Json='{"name":"com.unity.test-framework","version":"1.2.3","unity":"6000.3","unityRelease":"123f2"}'; Pattern='too old' },
        @{ Name='bad-minimum'; Json='{"name":"com.unity.test-framework","version":"1.2.3","unity":"latest"}'; Pattern='Unsupported minimum' }
    )) {
        $fixture = Fixture $case.Name
        $null = Package $fixture 'BuiltInPackages/com.unity.test-framework' $case.Json
        Reject { Discover $fixture } $case.Pattern
    }
    $ambiguous = Fixture 'ambiguous'
    $null = Package $ambiguous 'BuiltInPackages/com.unity.test-framework' $validJson
    $null = Package $ambiguous 'Editor/com.unity.test-framework@2.0.0' '{"name":"com.unity.test-framework","version":"2.0.0"}'
    Reject { Discover $ambiguous } 'Ambiguous installed'
    $conflict = Fixture 'conflict'
    $null = Package $conflict 'BuiltInPackages/com.unity.test-framework' $validJson
    $null = Package $conflict 'Editor/com.unity.test-framework@1.2.3' '{"name":"com.unity.test-framework","version":"1.2.3","unity":"6000.2"}'
    Reject { Discover $conflict } 'Conflicting installed'
    $null = Package $folder 'Editor/com.unity.test-framework@1.2.3' '{"unityRelease":"1f1","unity":"6000.3","version":"1.2.3","name":"com.unity.test-framework"}'
    Check ((Discover $folder).Path -eq $found.Path) 'Equivalent duplicate metadata has deterministic selection'

    $tarFixture = Fixture 'tarball'
    $tarSource = Join-Path $root 'tar-source/package'
    New-Item -ItemType Directory -Path $tarSource -Force | Out-Null
    [IO.File]::WriteAllText((Join-Path $tarSource 'package.json'), $validJson)
    $tarDir = Join-Path $tarFixture 'Data/Resources/PackageManager/Editor'
    New-Item -ItemType Directory -Path $tarDir -Force | Out-Null
    & tar -czf (Join-Path $tarDir 'com.unity.test-framework-1.2.3.tgz') -C (Split-Path $tarSource) package
    if ($LASTEXITCODE -ne 0) { throw 'Synthetic tarball creation failed.' }
    $packed = Discover $tarFixture
    Check ($packed.Kind -eq 'Tarball' -and $packed.Version -eq '1.2.3' -and $packed.Dependency.EndsWith('.tgz')) 'Read local tarball metadata without extracting into project'
    Check (-not (Test-Path -LiteralPath (Join-Path $tarDir 'package'))) 'Tarball discovery did not extract files'

    $log = Join-Path $root 'synthetic-Editor.log'
    $process = [pscustomobject]@{ ExitCode = 0; TimedOut = $false; LaunchException = $null }
    foreach ($sample in @(
        @{ Line='Assets/Test.cs(1,1): error CS0246: type not found'; Kind='CompilationFailed' },
        @{ Line="Unable to resolve reference 'System.Text.Json'"; Kind='AssemblyResolutionFailed' },
        @{ Line='An error occurred while resolving packages'; Kind='PackageResolutionFailed' },
        @{ Line='Failed to import asset Assets/test.dll'; Kind='AssetImportFailed' }
    )) {
        [IO.File]::WriteAllText($log, $sample.Line)
        $failure = Get-AcceptanceProcessFailure $process @('', $log)
        Check ($failure.Classification -eq $sample.Kind -and $failure.Signals.Count -eq 1) 'Import/compile diagnostic even with exit zero and no XML'
    }
    $process.ExitCode = 17
    Check ((Get-AcceptanceProcessFailure $process @()).Classification -eq 'EditorProcessFailed') 'Unknown nonzero exit is still failure'
    $process.TimedOut = $true
    Check ((Get-AcceptanceProcessFailure $process @()).Classification -eq 'ProcessTimedOut') 'Timeout never passes'

    $xml = Join-Path $root 'synthetic-results.xml'
    $envFile = Join-Path $root 'synthetic-environment.txt'
    $assessment = Test-AcceptanceResults $xml $envFile '6000.3.123f1' '1.2.3'
    Check (-not $assessment.Passed -and $assessment.TestOutcome -eq 'NotVerified' -and $assessment.ResultsStatus -eq 'Missing') 'No XML means not verified, not partial success'
    [IO.File]::WriteAllText($xml, '<broken')
    $assessment = Test-AcceptanceResults $xml $envFile '6000.3.123f1' '1.2.3'
    Check ($assessment.Classification -eq 'InvalidTestResults' -and $assessment.Exception.Length -gt 0) 'Malformed XML retains exception'
    [IO.File]::WriteAllText($xml, '<test-run result="Passed"/>')
    Check (-not (Test-AcceptanceResults $xml $envFile '6000.3.123f1' '1.2.3').Passed) 'Zero tests rejected'
    [IO.File]::WriteAllText($xml, '<test-run result="Passed">' + ('<test-case result="Passed"/>' * 7) + '</test-run>')
    Check ((Test-AcceptanceResults $xml $envFile '6000.3.123f1' '1.2.3').Classification -eq 'EnvironmentEvidenceMismatch') 'Missing environment rejected'
    [IO.File]::WriteAllLines($envFile, @('UnityVersion=6000.3.123f1', 'TestFramework=com.unity.test-framework@1.2.3', 'ApiCompatibilityLevel=NET_Standard', 'Mono=True'))
    Check ((Test-AcceptanceResults $xml $envFile '6000.3.123f1' '1.2.3').Passed) 'Synthetic complete result validates policy only, NOT actual Unity execution'
    Check (-not (Test-AcceptanceResults $xml $envFile '6000.3.123f1' '9.9.9').Passed) 'Resolved package replacement rejected'
    Check (-not (Test-AcceptanceResults $xml $envFile '6000.3.124f1' '1.2.3').Passed) 'Different Editor version rejected'
    [IO.File]::WriteAllText($xml, '<test-run result="Passed">' + ('<test-case result="Passed"/>' * 6) + '<test-case result="Skipped"/></test-run>')
    Check (-not (Test-AcceptanceResults $xml $envFile '6000.3.123f1' '1.2.3').Passed) 'Skipped test rejected'

    # Exercise real OS process capture with PowerShell, explicitly not a mock Unity executable.
    $pwsh = (Get-Process -Id $PID).Path
    $code = "[Console]::Out.WriteLine('synthetic diagnostic'); [Console]::Error.WriteLine('Could not load file or assembly synthetic'); exit 23"
    $encoded = [Convert]::ToBase64String([Text.Encoding]::Unicode.GetBytes($code))
    $captured = Invoke-AcceptanceProcess $pwsh @('-NoProfile', '-EncodedCommand', $encoded) 'synthetic-process' $root 30
    Check ($captured.ExitCode -eq 23 -and -not $captured.TimedOut) 'Exact nonzero child exit retained'
    Check ((Get-Content -LiteralPath $captured.Stderr -Raw) -match 'Could not load file or assembly') 'Child stderr retained'
    Check ((Get-AcceptanceProcessFailure $captured @($captured.Stdout, $captured.Stderr)).Classification -eq 'AssemblyResolutionFailed') 'Actual captured stderr classified'
    $launchFailure = Invoke-AcceptanceProcess (Join-Path $root 'does-not-exist.exe') @() 'synthetic-launch-failure' $root 1
    Check ($launchFailure.LaunchException.Length -gt 0 -and (Test-Path -LiteralPath (Join-Path $root 'synthetic-launch-failure-process.json'))) 'Launch exception persists without XML'

    $beforeRuns = @(Get-ChildItem -LiteralPath $evidenceRoot -Directory -Filter 'run-*' | Select-Object -ExpandProperty Name)
    & $pwsh -NoProfile -File (Join-Path $PSScriptRoot 'Invoke-EditorAcceptance.ps1') -UnityEditor (Join-Path $root 'not-installed/Unity.exe') -PrepareOnly *>&1 | Out-Host
    Check ($LASTEXITCODE -ne 0) 'Actual runner missing-Editor path exits nonzero'
    $newRun = @(Get-ChildItem -LiteralPath $evidenceRoot -Directory -Filter 'run-*' | Where-Object Name -notin $beforeRuns)
    Check ($newRun.Count -eq 1) 'Actual runner creates unique evidence for preflight failure'
    $summary = Get-Content -LiteralPath (Join-Path $newRun[0].FullName 'execution.json') -Raw | ConvertFrom-Json
    Check ($summary.Status -eq 'Failed' -and $summary.TestOutcome -eq 'NotVerified' -and -not $summary.AllSevenTestsPassed -and $summary.Exception.Detail.Length -gt 0) 'Failure summary preserves details and no acceptance'
    Check (Test-Path -LiteralPath (Join-Path $newRun[0].FullName 'failure.txt')) 'Actionable failure report retained'

    Write-AcceptanceJson @{ Assertions = $script:count; Result = 'Passed'; EvidenceKind = 'Synthetic runner/OS process regressions; Unity UNVERIFIED' } (Join-Path $root 'runner-test-summary.json')
    Write-Host "Runner regression assertions: $script:count PASS. Synthetic data only, not Unity acceptance. Evidence: $root"
} catch {
    Write-AcceptanceJson @{ AssertionsPassed = $script:count; Result = 'Failed'; Error = $_.Exception.ToString() } (Join-Path $root 'runner-test-summary.json')
    throw
}
