<#
.SYNOPSIS
    Powershell script to create release in GitHub for RiakClient.
.DESCRIPTION
    Powershell script to create release in GitHub for RiakClient.
.PARAMETER VersionString
    Version of the release. Must be in vX.Y.Z-PreRelease format.
.PARAMETER IsPreRelease
    Indicates that this should be published as a pre-release Release.
.EXAMPLE
    C:\PS>cd path\to\riak-dotnet-client
    >.\build\create-github-release.ps1 v2.0.0-beta1
.NOTES
    Author: Luke Bakken
    Date:   January 27, 2015
#>
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=0)]
    [ValidatePattern("^[1-9]\.[0-9]\.[0-9](-[a-z0-9]+)?$")]
    [string]$VersionString,
    [Parameter(Mandatory=$False)]
    [switch]$IsPreRelease
)

Set-StrictMode -Version Latest

$IsDebug = $DebugPreference -ne 'SilentlyContinue'
$IsVerbose = $VerbosePreference -ne 'SilentlyContinue'

# Note:
# Set to Continue to see DEBUG messages
if ($IsVerbose) {
    $DebugPreference = 'Continue'
}

trap
{
    Write-Error -ErrorRecord $_
    exit 1
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

$release_zip_name = "RiakClient-$VersionString.zip"
$release_zip_path = Resolve-Path ($(Get-ScriptPath) + '\..\src\RiakClient\bin\Release\' + $release_zip_name)
if (!(Test-Path $release_zip_path)) {
    throw "Could not find file $release_zip_path"
}
else {
    Write-Debug -Message "RiakClient release zip file: $release_zip_path"
}

$github_api_key_file = Resolve-Path '~/.ghapi'
$github_api_key = Get-Content $github_api_key_file
Write-Debug "GitHub API Key '$github_api_key'"

$release_info = New-Object PSObject -Property @{
        tag_name = $VersionString
        name = $VersionString
        body ="riak-dotnet-client $VersionString`nhttps://github.com/basho/riak-dotnet-client/blob/master/RELNOTES.md"
        draft = $false
        prerelease = $IsPreRelease.IsPresent
    }

$headers = @{ Authorization = "token $github_api_key" }

$release_json = ConvertTo-Json -InputObject $release_info -Compress

Write-Debug "Release JSON: $release_json"

try {
    $response = Invoke-WebRequest -Headers $headers -ContentType 'application/json' -Method Post -Body $release_json -Uri 'https://api.github.com/repos/basho/riak-dotnet-client/releases'
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
# https://uploads.github.com/repos/basho/riak-dotnet-client/releases/890350/assets{?name,label}
$upload_url_with_name = $response_json.upload_url -Replace '{\?name,label}', "?name=$release_zip_name"
$body = Get-Content -Raw $release_zip_path
Write-Debug "Asset: $release_zip_name Upload url: $upload_url_with_name"
$response =  Invoke-WebRequest -Headers $headers -ContentType 'application/zip' -Method Post -Body $body -Uri $upload_url_with_name
if ([int]$response.StatusCode -ne 201) {
    throw "Creating release asset failed: $response.StatusCode"
}

