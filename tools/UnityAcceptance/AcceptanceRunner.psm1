#requires -Version 7.2
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Write-AcceptanceJson($Value, [string] $Path) {
    $Value | ConvertTo-Json -Depth 30 | Set-Content -LiteralPath $Path -Encoding utf8NoBOM
}

function Assert-AcceptanceEditorPath([string] $Path) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw 'UnityEditor must point to an installed Unity.exe. Select the exact Unity 6.3 LTS installation in Hub; no installation is performed.'
    }
    $resolved = (Resolve-Path -LiteralPath $Path).Path
    if ([IO.Path]::GetFileName($resolved) -ne 'Unity.exe') { throw 'Expected an explicit Unity.exe path, not Hub or another executable.' }
    return $resolved
}

function Get-AcceptanceEditorVersion([string] $Text) {
    if ([string]::IsNullOrWhiteSpace($Text) -or $Text.Trim() -notmatch '^6000\.3\.\d+f\d+$') {
        throw "Unsupported/unresolved Editor version '$Text'. Select an installed final Unity 6.3 LTS patch; retain editor-version stdout/stderr."
    }
    return $Text.Trim()
}

function Resolve-AcceptanceTestFramework([string] $EditorPath, [string] $EditorVersion, [string] $EvidenceDirectory) {
    $null = Get-AcceptanceEditorVersion $EditorVersion
    $root = Join-Path (Split-Path $EditorPath) 'Data/Resources/PackageManager'
    $report = [ordered]@{ EditorVersion = $EditorVersion; SearchRoot = $root; MaxDepth = 4; Candidates = @(); Selected = $null; Error = $null }
    $recovery = 'In Hub verify/repair this exact Editor installation. Inspect its PackageManager contents and the Test Framework manifest through Package Manager > Locate/Open Manifest. Share package-discovery.json and local metadata for review. Do not supply a guessed version, use another Editor/cache, or disable compatibility/signature checks.'
    try {
        if (-not (Test-Path -LiteralPath $root -PathType Container)) { throw "Selected Editor has no PackageManager directory: $root" }
        # Search only this installation, never project/global caches or the network. Do not follow links.
        $files = @(Get-ChildItem -LiteralPath $root -File -Recurse -Depth 4 | Sort-Object FullName)
        foreach ($file in $files) {
            $isArchive = $file.Name -match '^com\.unity\.test-framework(?:-|@).+\.tgz$'
            if ($file.Name -ne 'package.json' -and -not $isArchive) { continue }
            $parent = $file.Directory
            while ($parent -and $parent.FullName.Length -ge $root.Length) {
                if ($parent.Attributes -band [IO.FileAttributes]::ReparsePoint) { throw "Linked package directory is not trusted installation evidence: $($parent.FullName)" }
                $parent = $parent.Parent
            }
            if ($file.Attributes -band [IO.FileAttributes]::ReparsePoint) { throw "Linked package metadata is unsupported: $($file.FullName)" }
            $namedCandidate = $isArchive -or $file.Directory.Name -match '^com\.unity\.test-framework(?:$|@|-)'
            if ($isArchive) {
                if (-not (Get-Command tar -ErrorAction SilentlyContinue)) { throw 'tar is required to inspect installed .tgz metadata without extracting files.' }
                $raw = (& tar -xOf $file.FullName 'package/package.json' 2>&1) -join "`n"
                if ($LASTEXITCODE -ne 0) { throw "Cannot read package/package.json from $($file.FullName): $raw" }
            } else { $raw = Get-Content -LiteralPath $file.FullName -Raw }
            try { $metadata = ConvertFrom-Json -InputObject $raw -AsHashtable }
            catch { throw "Invalid package JSON at $($file.FullName): $($_.Exception.Message)" }
            if ($metadata -isnot [System.Collections.IDictionary]) { throw "Expected package object at $($file.FullName)" }
            if ($metadata['name'] -ne 'com.unity.test-framework') {
                if ($namedCandidate) { throw "Test Framework candidate has wrong/missing name at $($file.FullName)" }
                continue
            }
            $version = $metadata['version']
            if ($version -isnot [string] -or $version -notmatch '^\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?(?:\+[0-9A-Za-z.-]+)?$') {
                throw "Missing/invalid exact package version at $($file.FullName)"
            }
            if ($metadata.Contains('unityRelease') -and -not $metadata.Contains('unity')) { throw "unityRelease lacks unity constraint: $($file.FullName)" }
            if ($metadata.Contains('unity')) {
                if ($metadata['unity'] -notmatch '^\d+\.\d+$') { throw "Unsupported minimum unity metadata at $($file.FullName)" }
                $minimum = [version]($metadata['unity'] + '.0')
                $minimumSuffix = 0
                $minimumChannel = 'a'
                if ($metadata.Contains('unityRelease')) {
                    if ($metadata['unityRelease'] -notmatch '^(\d+)([abfp])(\d+)$') { throw "Unsupported unityRelease at $($file.FullName)" }
                    $minimum = [version]($metadata['unity'] + '.' + $Matches[1])
                    $minimumChannel = $Matches[2]
                    $minimumSuffix = [int]$Matches[3]
                }
                $null = $EditorVersion -match '^(\d+\.\d+\.\d+)f(\d+)$'
                $current = [version]$Matches[1]
                $suffix = [int]$Matches[2]
                if ($current -lt $minimum -or ($current -eq $minimum -and
                    ($minimumChannel -eq 'p' -or ($minimumChannel -eq 'f' -and $suffix -lt $minimumSuffix)))) {
                    throw "Package $version requires Unity $($metadata['unity']) $($metadata['unityRelease']); selected $EditorVersion is too old."
                }
            }
            $kind = if ($isArchive) { 'Tarball' } else { 'Folder' }
            $localPath = if ($isArchive) { $file.FullName } else { $file.Directory.FullName }
            $report.Candidates += [ordered]@{ Version = $version; Kind = $kind; Path = $file.FullName;
                SHA256 = (Get-FileHash -LiteralPath $file.FullName).Hash; Metadata = $metadata;
                Dependency = 'file:' + $localPath.Replace('\', '/') }
        }
        if ($report.Candidates.Count -eq 0) { throw "No local com.unity.test-framework package metadata found under $root (depth 4)." }
        $versions = @($report.Candidates.Version | Sort-Object -Unique)
        if ($versions.Count -ne 1) { throw "Ambiguous installed Test Framework versions: $($versions -join ', '). No highest-version or first-match fallback is allowed." }
        # Duplicate representations must agree on all package metadata, not only the version string.
        $reference = $report.Candidates[0].Metadata
        foreach ($candidate in $report.Candidates) {
            if ((ConvertTo-AcceptanceCanonicalJson $candidate.Metadata) -cne (ConvertTo-AcceptanceCanonicalJson $reference)) {
                throw 'Conflicting installed Test Framework metadata for the same version; review all candidates.'
            }
        }
        $selected = $report.Candidates | Sort-Object Kind, Path | Select-Object -First 1
        $report.Selected = $selected
        Write-AcceptanceJson $selected.Metadata (Join-Path $EvidenceDirectory 'test-framework-package.json')
        return [pscustomobject]$selected
    } catch {
        $report.Error = $_.Exception.ToString()
        throw "Test Framework discovery failed: $($_.Exception.Message) Recovery: $recovery"
    } finally {
        Write-AcceptanceJson $report (Join-Path $EvidenceDirectory 'package-discovery.json')
    }
}

function ConvertTo-AcceptanceCanonicalJson($Value) {
    if ($Value -is [System.Collections.IDictionary]) {
        $sorted = [ordered]@{}
        foreach ($key in ($Value.Keys | Sort-Object)) { $sorted[$key] = ConvertTo-AcceptanceCanonicalJson $Value[$key] }
        return ConvertTo-Json -InputObject $sorted -Compress -Depth 30
    }
    if ($Value -is [array]) { return ConvertTo-Json -InputObject @($Value | ForEach-Object { ConvertTo-AcceptanceCanonicalJson $_ }) -Compress -Depth 30 }
    return ConvertTo-Json -InputObject $Value -Compress -Depth 30
}

function Invoke-AcceptanceProcess([string] $Executable, [string[]] $Arguments, [string] $Stage,
    [string] $EvidenceDirectory, [int] $TimeoutSeconds = 900) {
    $result = [ordered]@{ Stage = $Stage; StartedUtc = [DateTime]::UtcNow.ToString('o'); FinishedUtc = $null;
        ExitCode = $null; TimedOut = $false; LaunchException = $null; Executable = $Executable; Arguments = $Arguments;
        Stdout = (Join-Path $EvidenceDirectory "$Stage.stdout.log"); Stderr = (Join-Path $EvidenceDirectory "$Stage.stderr.log") }
    try {
        $process = Start-Process -FilePath $Executable -ArgumentList $Arguments -WindowStyle Hidden -PassThru `
            -RedirectStandardOutput $result.Stdout -RedirectStandardError $result.Stderr
        if (-not $process.WaitForExit($TimeoutSeconds * 1000)) {
            $result.TimedOut = $true
            $process.Kill()
            $process.WaitForExit()
        }
        $result.ExitCode = $process.ExitCode
    } catch { $result.LaunchException = $_.Exception.ToString() }
    finally {
        $result.FinishedUtc = [DateTime]::UtcNow.ToString('o')
        Write-AcceptanceJson $result (Join-Path $EvidenceDirectory "$Stage-process.json")
    }
    return [pscustomobject]$result
}

function Get-AcceptanceProcessFailure($Process, [string[]] $LogPaths) {
    $patterns = [ordered]@{
        CompilationFailed = '\berror CS\d{4}\b|Scripts have compiler errors|Compilation failed|script compilation (?:failed|errors)'
        AssemblyResolutionFailed = 'Unable to resolve reference|Could not (?:load|resolve) (?:file or assembly|assembly)|AssemblyResolutionException|Assembly .+ (?:will not be loaded|could not be loaded)'
        PackageResolutionFailed = 'An error occurred while resolving packages|Package resolution failed|Failed to resolve packages|Invalid package signature'
        AssetImportFailed = 'Asset import failed|Failed to import (?:asset|package)|Error importing'
    }
    $signals = @()
    foreach ($path in $LogPaths) {
        if ([string]::IsNullOrWhiteSpace($path)) { continue }
        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { continue }
        foreach ($kind in $patterns.Keys) {
            $matches = @(Select-String -LiteralPath $path -Pattern $patterns[$kind])
            foreach ($match in $matches) { $signals += [ordered]@{ Kind = $kind; Log = $path; Line = $match.LineNumber; Text = $match.Line } }
        }
    }
    $classification = if ($Process.LaunchException) { 'ProcessLaunchFailed' }
        elseif ($Process.TimedOut) { 'ProcessTimedOut' }
        elseif ($signals.Count) { $signals[0].Kind }
        elseif ($Process.ExitCode -ne 0) { 'EditorProcessFailed' }
        else { 'None' }
    return [pscustomobject]@{ Classification = $classification; Signals = $signals }
}

function Test-AcceptanceResults([string] $ResultsPath, [string] $EnvironmentPath,
    [string] $EditorVersion, [string] $PackageVersion) {
    $result = [ordered]@{ Passed = $false; Classification = 'MissingTestResults'; ResultsStatus = 'Missing';
        TestOutcome = 'NotVerified'; CaseCount = 0; Exception = $null }
    if (-not (Test-Path -LiteralPath $ResultsPath -PathType Leaf)) { return [pscustomobject]$result }
    try {
        $settings = [Xml.XmlReaderSettings]::new()
        $settings.DtdProcessing = [Xml.DtdProcessing]::Prohibit
        $settings.XmlResolver = $null
        $reader = [Xml.XmlReader]::Create($ResultsPath, $settings)
        try { $xml = [Xml.XmlDocument]::new(); $xml.Load($reader) } finally { $reader.Dispose() }
        $root = $xml.DocumentElement
        if ($root.Name -ne 'test-run') { throw 'Expected NUnit test-run root.' }
        $cases = @($xml.SelectNodes('//test-case'))
        $result.ResultsStatus = 'Present'
        $result.CaseCount = $cases.Count
        $result.Classification = 'IncompleteOrFailedTestRun'
        if ($root.GetAttribute('result') -ne 'Passed' -or $cases.Count -ne 7 -or
            @($cases | Where-Object { $_.GetAttribute('result') -ne 'Passed' }).Count -ne 0) {
            $result.TestOutcome = 'FailedOrIncomplete'
            return [pscustomobject]$result
        }
        $result.Classification = 'EnvironmentEvidenceMismatch'
        if (-not (Test-Path -LiteralPath $EnvironmentPath -PathType Leaf)) { return [pscustomobject]$result }
        $environment = @(Get-Content -LiteralPath $EnvironmentPath)
        foreach ($required in @("UnityVersion=$EditorVersion", "TestFramework=com.unity.test-framework@$PackageVersion",
            'ApiCompatibilityLevel=NET_Standard', 'Mono=True')) {
            if ($environment -cnotcontains $required) { return [pscustomobject]$result }
        }
        if ($environment -match 'ENVIRONMENT GATE FAILED') { return [pscustomobject]$result }
        $result.Passed = $true
        $result.Classification = 'None'
        $result.TestOutcome = 'Passed'
        return [pscustomobject]$result
    } catch {
        $result.Classification = 'InvalidTestResults'
        $result.ResultsStatus = 'Invalid'
        $result.Exception = $_.Exception.ToString()
        return [pscustomobject]$result
    }
}

Export-ModuleMember -Function Write-AcceptanceJson, Assert-AcceptanceEditorPath, Get-AcceptanceEditorVersion,
    Resolve-AcceptanceTestFramework, Invoke-AcceptanceProcess, Get-AcceptanceProcessFailure, Test-AcceptanceResults
