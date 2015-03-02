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

$release_info = New-Object PSObject -Property @{
        tag_name = $VersionString
        name = $VersionString
        body ="riak-dotnet-client $VersionString`nhttps://github.com/basho-labs/riak-dotnet-client/blob/master/RELNOTES.md"
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

