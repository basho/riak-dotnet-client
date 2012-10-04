param(
  [Parameter(Position=0,Mandatory=0)]
  [string]$buildFile = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)\build\build.ps1",
  [Parameter(Position=1,Mandatory=0)]
  [string[]]$taskList = @(),
  [Parameter(Position=2,Mandatory=0)]
  [string]$framework = '4.0',
  [Parameter(Position=3,Mandatory=0)]
  [switch]$docs = $false,
  [Parameter(Position=4,Mandatory=0)]
  [System.Collections.Hashtable]$parameters = @{},
  [Parameter(Position=5, Mandatory=0)]
  [System.Collections.Hashtable]$properties = @{}
)

$scriptPath = (Split-Path -parent $MyInvocation.MyCommand.Definition)
$buildPath = (Resolve-Path $scriptPath\build)

. $buildPath\bootstrap.ps1 $buildPath
$psakeModule = @(Get-ChildItem $scriptPath\* -recurse -include psake.ps1)[0].FullName
. $psakeModule $buildFile $taskList '4.0' $docs $parameters $properties
#. $psakeModule $buildFile $taskList '3.5' $docs $parameters $properties
#. $psakeModule $buildFile Package $framework $docs $parameters $properties

if($env:BUILD_NUMBER) {
  [Environment]::Exit($lastexitcode)
} else {
  exit $lastexitcode
}