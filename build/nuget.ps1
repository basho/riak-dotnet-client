# 
# Copyright (c) 2011-2012, Toji Project Contributors
# 
# Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
# See the file LICENSE.txt for details.
# 
properties {
  Write-Output "Loading nuget properties"
  # see if we should be using chewie, load if needed
  $usingChewie = (Test-Path "$($base.dir)\.NugetFile")
  if($usingChewie) { Import-Module "$pwd\chewie.psm1" }
  
  $nuget = @{}
  $nuget.pub_dir = "$($release.dir)"
  $nuget.file = (Resolve-NuGet)
  
  # add either the project_name or nuspec file to use when packaging.
  $nuget.options = ""
  $nuget.targets = @((Get-ChildItem -path "$($base.dir)\*" -recurse -include *.nuspec) | Select $_.FullName)

  if ($nuget.targets.length -lt 1 ){
    if(!(Test-Path("$($source.dir)\$($solution.name)\$($solution.name).csproj"))) {  
		Write-Output "Could not find $($source.dir)\$($solution.name)\$($solution.name).csproj" 
		$nuget.targets = @()
	} else {
		$nuget.targets = @("$($source.dir)\$($solution.name)\$($solution.name).csproj")
		$nuget.options = "-Build -Sym -Properties Configuration=$($build.configuration)"
	}
  }
}

Task Bootstrap-NuGetPackages {
  Write-Output "Installing Nuget Dependencies"
  Push-Location "$($base.dir)"
  try {
    if($usingChewie) {
      Write-Output "Running chewie"
      Invoke-Chewie
    } else {
      Write-Output "Loading NuGet package files"
      . { Get-ChildItem -recurse -include packages.config | % { & $nuget.file i $_ -o Packages } }
    }
  } finally { Pop-Location }
}

Task Create-NuGetPackage -depends Set-NuSpecVersion {
  Assert (![string]::IsNullOrEmpty($nuget.file) -and (Test-Path($nuget.file))) "The location of the nuget exe must be specified."
  Assert (Test-Path($nuget.file)) "Could not find nuget exe"

  foreach ($nuget_target in $nuget.targets)
  {
	  $nuget.command = "& $($nuget.file) pack $nuget_target $($nuget.options) -Version $($build.version) -OutputDirectory $($nuget.pub_dir)"
  
	  if(!(Test-Path($nuget.pub_dir))) { new-item $nuget.pub_dir -itemType directory | Out-Null }
	  $nuget_targetPath = (Split-Path $nuget_target)
	  Write-Output "Moving into $nuget_targetPath"
	  Push-Location $nuget_targetPath
	  try {
		$message = "Error executing command: {0}"
		$command = "Invoke-Expression $($nuget.command)"
		$errorMessage = $message -f $command
		exec { Invoke-Expression $nuget.command } $errorMessage
	  } finally { Pop-Location }
  }
}

Task Publish-NuGetPackage {
  Push-Location "$($nuget.pub_dir)"
  try {
    ls "*$($build.version).nupkg" | % { & $nuget.file push $_ }
  } finally { Pop-Location }
}

Task Set-NuSpecVersion {
  Assert ($nuget.targets.length -gt 0) "The location of the nuspec file must be specified."

  $version_pattern = "<version>\d*\.\d*\.\d*.\d*</version>"   # 3 digit for semver

  foreach ($nuget_target in $nuget.targets)
  {
	$content = Get-Content "$nuget_target" | % { [Regex]::Replace($_, $version_pattern, "<version>$($build.version)</version>") } 
	Set-Content -Value $content -Path $nuget_target
  }
}