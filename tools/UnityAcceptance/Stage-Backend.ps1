#requires -Version 7.2
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$project = Join-Path $repo 'src/Game.UnityCompatibility/Game.UnityCompatibility.csproj'
$unity = Join-Path $repo 'unity/FragmentsOfWysdom.Acceptance'
$plugins = Join-Path $unity 'Assets/Plugins/Backend'
$generated = Join-Path $unity 'Assets/Acceptance/Editor/Generated'
$evidence = Join-Path $unity 'Evidence'

function Invoke-DotNet([string[]] $Arguments) {
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) { throw "dotnet failed ($LASTEXITCODE): $Arguments" }
}

if (Test-Path -LiteralPath (Join-Path $unity 'Temp/UnityLockfile')) {
    throw 'Close this acceptance project in Unity before staging.'
}
Push-Location $repo
try {
    Invoke-DotNet @('restore', $project, '--locked-mode')
    Invoke-DotNet @('build', $project, '--no-restore', '-c', 'Release', '-v:minimal')
    $inputs = (& dotnet msbuild $project '-getProperty:TargetFramework,LangVersion' '-getItem:Compile') | ConvertFrom-Json
    if ($LASTEXITCODE -ne 0) { throw 'Could not evaluate compatibility inputs.' }
    $original = (& dotnet msbuild 'src/Game/Game.csproj' '-getItem:Compile') | ConvertFrom-Json
    if ($LASTEXITCODE -ne 0) { throw 'Could not evaluate original inputs.' }
    if ($inputs.Properties.TargetFramework -ne 'netstandard2.1' -or $inputs.Properties.LangVersion -ne '9.0' -or
        $inputs.Items.Compile.Count -ne 266 -or
        (Compare-Object ($inputs.Items.Compile.FullPath | Sort-Object) ($original.Items.Compile.FullPath | Sort-Object))) {
        throw 'Unsupported build target/source inventory: expected the same 266 sources, Standard 2.1, C# 9.'
    }

    # Reviewed package/runtime map, not a glob over NuGet or host output directories.
    $mapping = [ordered]@{
        'Microsoft.Bcl.AsyncInterfaces/10.0.8' = 'lib/netstandard2.1/Microsoft.Bcl.AsyncInterfaces.dll'
        'System.Buffers/4.6.1' = 'lib/netstandard2.1/_._'
        'System.IO.Pipelines/10.0.8' = 'lib/netstandard2.0/System.IO.Pipelines.dll'
        'System.Memory/4.6.3' = 'lib/netstandard2.1/_._'
        'System.Runtime.CompilerServices.Unsafe/6.1.2' = 'lib/netstandard2.0/System.Runtime.CompilerServices.Unsafe.dll'
        'System.Text.Encodings.Web/10.0.8' = 'lib/netstandard2.0/System.Text.Encodings.Web.dll'
        'System.Text.Json/10.0.8' = 'lib/netstandard2.0/System.Text.Json.dll'
        'System.Threading.Tasks.Extensions/4.6.3' = 'lib/netstandard2.1/_._'
    }
    $assets = Get-Content (Join-Path $repo 'src/Game.UnityCompatibility/obj/project.assets.json') -Raw | ConvertFrom-Json
    $target = $assets.targets.PSObject.Properties | Where-Object Name -eq '.NETStandard,Version=v2.1'
    if (-not $target -or (Compare-Object @($mapping.Keys) @($target.Value.PSObject.Properties.Name))) {
        throw 'Dependency graph changed; review the mapping before staging.'
    }
    $output = Join-Path $repo 'src/Game.UnityCompatibility/bin/Release/netstandard2.1'
    $artifacts = @([ordered]@{ File = 'FragmentsOfWisdom.Game.dll'; Origin = 'same-source netstandard2.1 Release build' })
    foreach ($key in $mapping.Keys) {
        $runtime = @($target.Value.$key.runtime.PSObject.Properties.Name)
        if ($runtime.Count -ne 1 -or $runtime[0] -ne $mapping[$key]) { throw "Unsupported runtime asset for $key" }
        if ($runtime[0].EndsWith('/_._')) { continue }
        $name = [IO.Path]::GetFileName($runtime[0])
        $library = $assets.libraries.$key.path
        $packageFile = @($assets.packageFolders.PSObject.Properties.Name | ForEach-Object {
            Join-Path $_ "$library/$($runtime[0])"
        } | Where-Object { Test-Path -LiteralPath $_ -PathType Leaf })
        if ($packageFile.Count -eq 0 -or -not (Test-Path -LiteralPath (Join-Path $output $name))) {
            throw "Missing managed dependency: $key / $name"
        }
        if ((Get-FileHash -LiteralPath $packageFile[0]).Hash -ne (Get-FileHash -LiteralPath (Join-Path $output $name)).Hash) {
            throw "Output differs from the selected Standard asset: $name"
        }
        $artifacts += [ordered]@{ File = $name; Origin = "$key/$($runtime[0])" }
    }
    if (Compare-Object @($artifacts.File) @(Get-ChildItem -LiteralPath $output -Filter '*.dll' | Select-Object -ExpandProperty Name)) {
        throw 'Unexpected DLLs in compatibility build output; nothing staged.'
    }
    foreach ($item in $artifacts) {
        $source = Join-Path $output $item.File
        if (-not (Test-Path -LiteralPath $source -PathType Leaf)) { throw "Missing artifact: $source" }
        if (-not (Test-Path -LiteralPath (Join-Path $plugins ($item.File + '.meta')))) { throw "Missing importer metadata: $($item.File)" }
        $item['AssemblyIdentity'] = [Reflection.AssemblyName]::GetAssemblyName($source).FullName
        $item['SHA256'] = (Get-FileHash -LiteralPath $source).Hash
    }
    $extras = @(Get-ChildItem -LiteralPath $plugins -Filter '*.dll' | Where-Object Name -notin $artifacts.File)
    if ($extras.Count -gt 0) { throw 'Unexpected DLL already in plugin directory. Review it; the script will not delete it.' }
    $helpers = @('TrustedEncounterAssembly.cs', 'PresentationConsumer.cs')
    $unexpected = @(Get-ChildItem -LiteralPath $generated -Filter '*.cs' | Where-Object Name -notin $helpers)
    if ($unexpected.Count -gt 0) { throw 'Unexpected generated test source; refusing to overwrite unrelated work.' }
    $helperRecords = foreach ($name in $helpers) {
        $source = Join-Path $repo "src/Game.PresentationBoundary.SmokeTests/$name"
        if (-not (Test-Path -LiteralPath $source)) { throw "Missing authoritative test helper: $name" }
        [ordered]@{ File = $name; SHA256 = (Get-FileHash -LiteralPath $source).Hash }
    }
    foreach ($item in $artifacts) { Copy-Item -LiteralPath (Join-Path $output $item.File) -Destination (Join-Path $plugins $item.File) }
    foreach ($name in $helpers) {
        Copy-Item -LiteralPath (Join-Path $repo "src/Game.PresentationBoundary.SmokeTests/$name") -Destination (Join-Path $generated $name)
    }
    New-Item -ItemType Directory -Path $evidence -Force | Out-Null
    $commit = & git rev-parse HEAD
    if ($LASTEXITCODE -ne 0) { throw 'Cannot record repository commit.' }
    $dirty = @(& git status --porcelain --untracked-files=normal)
    $manifest = [ordered]@{
        StagedUtc = [DateTime]::UtcNow.ToString('o'); GitCommit = $commit; WorkingTreeDirty = ($dirty.Count -gt 0)
        SDK = (& dotnet --version); Framework = 'netstandard2.1'; Language = '9.0'; SourceCount = 266
        Artifacts = $artifacts; TestHelpers = @($helperRecords)
        LockSHA256 = (Get-FileHash -LiteralPath (Join-Path $repo 'src/Game.UnityCompatibility/packages.lock.json')).Hash
        UnityAcceptance = 'UNVERIFIED: staging is not Editor execution'
    }
    $manifest | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $evidence 'staged-artifacts.json') -Encoding utf8NoBOM
    Write-Host "Staged $($artifacts.Count) reviewed DLLs and two unchanged test helpers. Unity execution UNVERIFIED."
} finally { Pop-Location }
