#Requires -Version 2.0

# 
# Copyright (c) 2011, Toji Project Contributors
# 
# Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
# See the file LICENSE.txt for details.
# 

param(
  [Parameter( Position = 0, Mandatory = 0 )]
  [string] $path = ((Resolve-Path "$(Split-Path -parent $MyInvocation.MyCommand.Definition)\..\").Path),
  [Parameter( Position = 1, Mandatory = 0 )]
  [string] $install_to = 'Packages'
)

function script:BootStrap-Chewie {
  if(!(test-path $pwd\.NugetFile)) {
    new-item -path $pwd -name .NugetFile -itemtype file
    add-content $pwd\.NugetFile "install_to '.'"
    add-content $pwd\.NugetFile "chew 'psake' '4.1.0'"
  }
}

function script:FileExistsInPath {
  param (
    [Parameter(Position=0,Mandatory=$true)]
    [string] $fileName = $null
  )

  $path = Get-Childitem Env:Path
  $found = $false
  foreach ($folder in $path.Value.Split(";")) { if (Test-Path "$folder\$fileName") { $found = $true; break } }
  return $found
}

function global:Resolve-NuGet {
  $nuGetIsInPath = (FileExistsInPath "NuGet.exe") -or (FileExistsInPath "NuGet.bat")
  $nuget = "NuGet"
  if($nuGetIsInPath) {
    $nuget = (@(get-command nuget) | % {$_.Definition} | ? { (Test-Path $_) } | Select-Object -First 1)
  } else {  
    $nugets = @(Get-ChildItem "..\*" -recurse -include NuGet.exe)
    if ($nugets.Length -le 0) { 
      Write-Output "No NuGet executables found."
      return
    }
    $nuget = (Resolve-Path $nugets[0]).Path
    $env:Path = $env:Path + ";" + (Split-Path $nuget)
  }
  return $nuget
}

Push-Location $path
try {
  Write-Output "Loading Nuget Dependencies"
  $nuget = Resolve-NuGet
  BootStrap-Chewie
  Write-Output "Nuget = $nuget"
  if(Test-Path ".NugetFile") {
    Write-Output "Loading chewie"
    Import-Module "$pwd\chewie.psm1"
    Write-Output "Running chewie"
    Invoke-Chewie
  } else {
    $package_files = Get-ChildItem . -recurse -include packages.config
    $package_files | % { & $nuget i $_ -OutputDirectory $install_to }
  }
} finally { Pop-Location }