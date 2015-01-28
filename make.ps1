<#
.SYNOPSIS
    Powershell script to build Riak .NET Client
.DESCRIPTION
    This script ensures that your build environment is sane and will run msbuild.exe correctly depending on parameters passed to this script.
.PARAMETER Target
    Target to build. Can be one of the following:
        * Debug           - debug build that is not versioned (default)
        * Release         - release build that versioned
        * Test            - Run all tests
        * UnitTest        - Run unit tests
        * IntegrationTest - Run live integration tests
        * CleanAll        - parallel clean build tree
.EXAMPLE
    C:\>.\make.ps1 Debug
.EXAMPLE
    C:\>.\make.ps1 -Target Release
.NOTES
    Author: Luke Bakken
    Date:   January 28, 2015
#>
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$False, Position=0)]
    [string]$Target = 'Debug'
)

Set-StrictMode -Version Latest

if ([System.Environment]::Version.Major -ne 4) {
    throw "The build depends on CLR version 4"
}

function Get-PathToMSBuildExe {

    $msbuild_exe_path = ''
    $msbuild_exe_name = 'MSBuild.exe'

    $clr_runtime_version = 'v' + [String]::Join('.', @(
                [System.Environment]::Version.Major,
                [System.Environment]::Version.Minor,
                [System.Environment]::Version.Build)
            )


    $install_root = (Get-ItemProperty -Name InstallRoot HKLM:\SOFTWARE\Microsoft\.NETFramework).InstallRoot
    $msbuild_exe_path = Join-Path -Path $install_root -ChildPath (Join-Path -Path $clr_runtime_version -ChildPath $msbuild_exe_name)
    if (!(Test-Path $msbuild_exe_path)) {
        $install_root = (Get-ItemProperty -Name InstallRoot HKLM:\SOFTWARE\Wow6432Node\Microsoft\.NETFramework).InstallRoot
        $msbuild_exe_path = Join-Path -Path $install_root -ChildPath (Join-Path -Path $clr_runtime_version -ChildPath $msbuild_exe_name)
        if (!(Test-Path $msbuild_exe_path)) {
            throw "Could not find $msbuild_exe_name on this system"
        }
    }

    return $msbuild_exe_path
}
# Note:
# Set to Continue to see DEBUG messages
# $DebugPreference = 'Continue'

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

Get-PathToMSBuildExe

exit 0

$release_zip_name = 'RiakClient.zip'
$release_zip_path = Resolve-Path ($(Get-ScriptPath) + '\..\src\RiakClient\bin\Release\' + $release_zip_name)
Write-Debug -Message "RiakClient release zip file: $release_zip_path"

$github_api_key_file = Resolve-Path '~/.ghapi'
$github_api_key = Get-Content $github_api_key_file
Write-Debug "GitHub API Key '$github_api_key'"

$release_info = New-Object PSObject -Property @{
        tag_name = $VersionString
        name = $VersionString
        body ="riak-dotnet-client $VersionString"
        draft = $false
        prerelease = $true
    }

$headers = @{ Authorization = "token $github_api_key" }

$release_json = ConvertTo-Json -InputObject $release_info -Compress

try {
    $response = Invoke-WebRequest -Headers $headers -ContentType 'application/json' -Method Post -Body $release_json -Uri 'https://api.github.com/repos/basho-labs/riak-dotnet-client/releases'
    if ([int]$response.StatusCode -ne 201) {
        throw "Creating release failed: $response.StatusCode"
    }
}
catch {
    if ([int]$_.Exception.Response.StatusCode -eq 422) {
        Write-Error -Category InvalidOperation "Release already exists in GitHub!"
        exit 1
    }
    else {
        throw
    }
}

$response_json = ConvertFrom-Json -InputObject $response.Content
# https://uploads.github.com/repos/basho-labs/riak-dotnet-client/releases/890350/assets{?name}
$upload_url_with_name = $response_json.upload_url -Replace '{\?name}', ('?name=' + $release_zip_name)

$response = Get-Content $release_zip_path | Invoke-WebRequest -Headers $headers -ContentType 'application/zip' -Method Post -Uri $upload_url_with_name
if ([int]$response.StatusCode -ne 201) {
    throw "Creating release asset failed: $response.StatusCode"
}

