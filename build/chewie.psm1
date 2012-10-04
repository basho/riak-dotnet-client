# 
# Copyright (c) 2011-2012, Chewie Project Contributors
# 

$default_source = ""
$version_packages = $false

function version_packages
{
  $script:version_packages = $true
}

function source
{
  param(
    [Parameter(Position=0,Mandatory=$true)]
    [string] $source = $null
  )
  
  $script:default_source = $source
}

function script:FileExistsInPath
{
  param (
    [Parameter(Position=0,Mandatory=$true)]
    [string] $fileName = $null
  )

  $path = Get-Childitem Env:Path
  $found = $false
  foreach ($folder in $path.Value.Split(";")) { if (Test-Path "$folder\$fileName") { $found = $true; break } }
  Write-Output $found
}

function install_to
{
  param(
    [Parameter(Position=0,Mandatory=$true)]
    [string] $path = $null
  )
    
  if(!(test-path $path)) 
  {
    if([System.IO.Path]::IsPathRooted($path))
    {
      $drive_letter = [System.IO.Path]::GetPathRoot($path)
      $directory_name = $path.Replace($drive_letter, "")
      new-item -path $drive_letter -name $directory_name -itemtype directory 
    }
    else
    {
      new-item -path . -name $path -itemtype directory 
    }  
  }

  push-location $path -stackname 'chewie_nuget'
}

function chew 
{
  [CmdletBinding()]
  param (
    [Parameter(Position=0,Mandatory=$true)]
    [string] $name = $null,
    
    [Parameter(Position=1,Mandatory=$false)]
    [alias("v")]
    [string] $version = "",
    
    [Parameter(Position=2,Mandatory=$false)]
    [alias("s")]
    [string] $source = ""
  )

  $nuGetIsInPath = (FileExistsInPath "NuGet.exe") -or (FileExistsInPath "NuGet.bat")
  $command = ""
  if($nuGetIsInPath) 
  {
    $command += "NuGet install" 
    if($script:version_packages -ne $true){$command += " -x"}
    
  } else { $command += "install-package"  }
  $command += " $name"
  
  if($version -ne "") { $command += " -v $version" }
  if($source -eq "" -and $script:default_source -ne "") { $source = $script:default_source }
  if($source -ne "") { $command += " -s $source" }
    
  invoke-expression $command
}

function Invoke-Chewie 
{
  gc $pwd\.NugetFile | Foreach-Object { $block = [scriptblock]::Create($_.ToString()); % $block; }
  if((get-location -stackname 'chewie_nuget').count -gt 0) { pop-location -stackname 'chewie_nuget' }
}

function Initialize-Chewie
{
  if(!(test-path $pwd\.NugetFile))
  {
    new-item -path $pwd -name .NugetFile -itemtype file
    add-content $pwd\.NugetFile "install_to 'lib'"
    add-content $pwd\.NugetFile "chew 'machine.specifications'"
  }
}
