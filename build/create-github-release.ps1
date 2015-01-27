<#
.SYNOPSIS
    Powershell script to create release in GitHub for RiakClient.
.DESCRIPTION
    Powershell script to create release in GitHub for RiakClient.
.PARAMETER VersionString
    Version of the release. Must be in vX.Y.Z-PreRelease format.
.EXAMPLE
    C:\PS>cd path\to\riak-dotnet-client
    >.\build\create-github-release.ps1 v2.0.0-beta1
.NOTES
    Author: Luke Bakken
    Date:   January 27, 2015
#>
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True, Position=1)]
    [ValidatePattern("^v[1-9]\.[0-9]\.[0-9](-[a-z0-9]+)?$")]
    [string]$VersionString
)

Set-StrictMode -Version Latest

# Note:
# Set to Continue to see DEBUG messages
# $DebugPreference = 'SilentlyContinue'

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

$release_zip_name = 'RiakClient.zip'
$release_zip_path = Resolve-Path ($(Get-ScriptPath) + '\..\src\RiakClient\bin\Release\' + $release_zip_name)
Write-Debug -Message "RiakClient release zip file: $release_zip_path"

$github_api_key_file = Resolve-Path '~/.ghapi'
$github_api_key = Get-Content $github_api_key_file
Write-Debug "GitHub API Key '$github_api_key'"

$headers = @{ Authorization = "token $github_api_key" }

$release_info = New-Object PSObject -Property @{
        tag_name = $VersionString
        target_commitish = "master"
        name = $VersionString
        body ="riak-dotnet-client $VersionString"
        draft = $false
        prerelease = $true
    }

$release_json = ConvertTo-Json -InputObject $release_info -Compress

$response = Invoke-WebRequest -Headers $headers -ContentType 'application/json' -Method Post -Body $release_json -Uri 'https://api.github.com/repos/basho-labs/riak-dotnet-client/releases'
if (!($response.StatusCode == 201)) {
    throw "Creating release failed: $response.StatusCode"
}

$response_json = ConvertFrom-Json -InputObject $response.Content
$assets_url_with_name = $response_json.assets_url + '?name=' + $release_zip_name

$response = Get-Content $release_zip_path | Invoke-WebRequest -Headers $headers -ContentType 'application/zip' -Method Post -Uri $assets_url_with_name
if (!($response.StatusCode == 201)) {
    throw "Creating release failed: $response.StatusCode"
}

