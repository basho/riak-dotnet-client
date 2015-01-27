Set-StrictMode -Version Latest

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

$release_zip_path = Resolve-Path ($(Get-ScriptPath) + '\..\src\RiakClient\bin\Release\RiakClient.zip')
Write-Host -ForegroundColor Yellow $release_zip_path
exit 0

$github_api_key_file = '~/.ghapi'

if (Test-Path $github_api_key_file) {
    $github_api_key = Get-Content ~/.ghapi
    Write-Host -ForegroundColor Green "GitHub API Key '$github_api_key'"
}
else {
    throw "GitHub API Key must be in file '$github_api_key_file'"
}

$headers = @{ Authorization = "token $github_api_key" }

$release_info = New-Object PSObject -Property @{
        tag_name = "v2.0.0-beta1"
        target_commitish = "master"
        name = "v2.0.0-beta1"
        body ="riak-dotnet-client 2.0.0-beta1"
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
