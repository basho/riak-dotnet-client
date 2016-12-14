<#
.SYNOPSIS
    Powershell script to build Riak .NET Client
.DESCRIPTION
    This script ensures that your build environment is sane and will run MSBuild.exe correctly depending on parameters passed to this script.
.PARAMETER Target
    Target to build. Can be one of the following:
        * Debug              - debug build that is not versioned (default)
        * Release            - release build that versioned
        * All                - debug and release build (parallel)
        * Clean              - clean build artifacts
        * Test               - Run all tests
        * UnitTest           - Run unit tests
        * IntegrationTest    - Run live integration tests
        * IntegrationHllTest - Run live hyperloglog integration tests
.PARAMETER Verbosity
    Parameter to set MSBuild verbosity
.EXAMPLE
    C:\Users\Bashoman> cd Projects\basho\riak-dotnet-client
    C:\Users\Bashoman\Projects\basho\riak-dotnet-client>.\make.ps1 Debug
.EXAMPLE
    C:\Users\Bashoman> cd Projects\basho\riak-dotnet-client
    C:\Users\Bashoman\Projects\basho\riak-dotnet-client>.\make.ps1 -Target Release -Verbosity Detailed
.NOTES
    Author: Luke Bakken
    Date:   January 28, 2015
#>
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$False, Position=0)]
    [ValidateSet('Debug','Release', 'All', 'Publish', 'Clean', 'CleanAll',
        'Test','TestAll','UnitTest','IntegrationTest','DeprecatedTest','TimeseriesTest','IntegrationHllTest',
        'CodeAnalysis','Documentation','ProtoGen',
        IgnoreCase = $True)]
    [string]$Target = 'Debug',
    [Parameter(Mandatory=$False)]
    [ValidateSet('Quiet','q','Minimal','m','Normal','n',
        'Detailed','d','Diagnostic','diag', IgnoreCase = $True)]
    [string]$Verbosity = 'Normal',
    [Parameter(Mandatory=$False)]
    [ValidatePattern("^[1-9]\.[0-9]\.[0-9](-[a-z0-9]+)?")]
    [string]$VersionString,
    [Parameter(Mandatory=$False)]
    [string]$ProtoGenExe,
    [Parameter(Mandatory=$False)]
    [switch]$DryRun
)

Set-StrictMode -Version Latest

# Note:
# Set to Continue to see DEBUG messages
# $DebugPreference = 'Continue'
$IsVerbose = $VerbosePreference -ne 'SilentlyContinue'
if ($IsVerbose) {
    $DebugPreference = 'Continue'
}

trap
{
    Write-Error -ErrorRecord $_
    exit 1
}

if ([System.Environment]::Version.Major -ne 4) {
    throw "The build depends on CLR version 4"
}

function Get-ScriptPath {
    $scriptDir = Get-Variable PSScriptRoot -ErrorAction SilentlyContinue | ForEach-Object { $_.Value }
    if (!$scriptDir) {
        if ($MyInvocation.MyCommand.Path) {
            $scriptDir = Split-Path $MyInvocation.MyCommand.Path -Parent
        }
    }
    if (!$scriptDir) {
        if ($ExecutionContext.SessionState.Module.Path) {
            $scriptDir = Split-Path (Split-Path $ExecutionContext.SessionState.Module.Path)
        }
    }
    if (!$scriptDir) {
        $scriptDir = $PWD
    }
    return $scriptDir
}

function Get-PathToMSBuildExe {
    $msbuild_exe_path = ''
    $msbuild_exe_name = 'MSBuild.exe'

    $clr_version = @([System.Environment]::Version.Major, [System.Environment]::Version.Minor, [System.Environment]::Version.Build)
    $clr_version_str = 'v' + [String]::Join('.', $clr_version)

    $install_root = (Get-ItemProperty -Name InstallRoot HKLM:\SOFTWARE\Microsoft\.NETFramework).InstallRoot
    $msbuild_exe_path = Join-Path -Path $install_root -ChildPath (Join-Path -Path $clr_version_str -ChildPath $msbuild_exe_name)
    if (!(Test-Path $msbuild_exe_path)) {
        $install_root = (Get-ItemProperty -Name InstallRoot HKLM:\SOFTWARE\Wow6432Node\Microsoft\.NETFramework).InstallRoot
        $msbuild_exe_path = Join-Path -Path $install_root -ChildPath (Join-Path -Path $clr_version_str -ChildPath $msbuild_exe_name)
        if (!(Test-Path $msbuild_exe_path)) {
            throw "Could not find $msbuild_exe_name on this system"
        }
    }

    Write-Debug "Using $msbuild_exe_name at $msbuild_exe_path"

    return $msbuild_exe_path
}

function Get-BuildTargetsFile {
    Param([Parameter(Mandatory=$True, Position=0)]
          [string]$ScriptPath)

    $build_targets_file_name = 'build.targets'

    $build_dir = Join-Path -Path $ScriptPath -ChildPath 'build'
    $build_targets_file = Join-Path -Path $build_dir -ChildPath 'build.targets'
    if (!(Test-Path $build_targets_file)) {
        throw "Could not find $build_targets_file_name on this system"
    }

    return $build_targets_file
}

function Get-PathToNuGetExe {
    Param([Parameter(Mandatory=$True, Position=0)]
          [string]$NuGetDir)

    $nuget_exe_name = 'NuGet.exe'

    $nuget_exe_path = Join-Path -Path $NuGetDir -ChildPath $nuget_exe_name

    if (!(Test-Path $nuget_exe_path)) {
        throw "Could not find $nuget_exe_name on this system"
    }

    Write-Debug "Using $nuget_exe_name at $nuget_exe_path"

    return $nuget_exe_path
}

function Get-NuGetData {
    $script_path = Get-ScriptPath

    $nuget_dir = Join-Path -Path $script_path -ChildPath '.nuget'
    $nuget_exe = Get-PathToNuGetExe -NuGetDir $nuget_dir
    $nuget_packages_dir = Join-Path -Path $script_path -ChildPath 'packages'
    $solution_file = Join-Path -Path $script_path -ChildPath 'RiakClient.sln'

    $props = @{
        ScriptPath = $script_path
        Dir = $nuget_dir
        Exe = $nuget_exe
        PackagesDir = $nuget_packages_dir
        SolutionFile = $solution_file
    }
    return New-Object PSObject -Property $props
}

function Restore-Dependencies {
    $nuget_data = Get-NuGetData
    & $nuget_data.Exe restore -PackagesDirectory $nuget_data.PackagesDir -NonInteractive $nuget_data.SolutionFile
    if ($? -ne $True) {
        throw "$nuget_data restore failed: $LastExitCode"
    }
    Write-Debug "$nuget_data restore exit code: $LastExitCode"
}

Write-Debug "Target: $Target"

$version_property = ''
if (! ([String]::IsNullOrEmpty($VersionString))) {
    if ($Target -ne 'Publish') {
        throw 'Only use the -VersionString parameter for the "Publish" target'
    }
    $version_property = "/property:VersionString=$VersionString"
}

$vs_version_property = ''
if (Test-Path -Path HKLM:\SOFTWARE\Microsoft\VisualStudio\14.0) {
    $vs_version_property = '/property:VisualStudioVersion=14.0'
}

$script_path = Get-ScriptPath

$build_targets_file = Get-BuildTargetsFile -ScriptPath $script_path

$msbuild_exe = Get-PathToMSBuildExe

if ($Target -eq 'Publish') {
    Stop-Process -Name msbuild -ErrorAction SilentlyContinue
    & git clean -fxd
}

Restore-Dependencies

$dryrun_property = '/property:IsDryRun=false'
if ($DryRun) {
    $dryrun_property = '/property:IsDryRun=true'
}

$verbose_property = ''
if ($IsVerbose -or $Verbosity -eq 'detailed' -or $Verbosity -eq 'd' -or
    $Verbosity -eq 'diagnostic' -or $Verbosity -eq 'diag') {
    $verbose_property = '/property:Verbose=true'
}

$protogen_property = ''
if ($Target -eq 'ProtoGen') {
    if ([String]::IsNullOrEmpty($ProtoGenExe)) {
        throw 'The -ProtoGenExe parameter pointing to protogen.exe is required by the "ProtoGen" target'
    }
    else {
        $protogen_exe_path = $(Resolve-Path $ProtoGenExe).Path
        $protogen_property = "/property:ProtoGenExe=$protogen_exe_path"
    }
}

# Fix up Target to use CleanAll in build.targets file
if ($Target -eq 'Clean') {
    $Target = 'CleanAll'
}

Write-Debug "MSBuild command: $msbuild_exe ""/verbosity:$Verbosity"" /nologo /m ""/property:SolutionDir=$script_path\"" ""$dryrun_property"" ""$version_property"" ""$vs_version_property"" ""$verbose_property"" ""$protogen_property"" ""/target:$Target"" ""$build_targets_file"""
& $msbuild_exe "/verbosity:$Verbosity" /nologo /m "/property:SolutionDir=$script_path\" "$dryrun_property" "$version_property" "$vs_version_property" "$verbose_property" "$protogen_property" "/target:$Target" "$build_targets_file"
if ($? -ne $True) {
    throw "$msbuild_exe failed: $LastExitCode"
}
Write-Debug "$msbuild_exe exit code: $LastExitCode"
exit 0

