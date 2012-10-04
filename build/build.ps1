# 
# Copyright (c) 2011-2012, Toji Project Contributors
# 
# Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
# See the file LICENSE.txt for details.
# 

# The global settings are provided in the settings.ps1 file. If you
# want to override anything in it, you can use the overrides.ps1 to 
# replace any property value used in any indcluded file. Remember that 
# values provided on the command line will override all of these files below.
Include settings.ps1
#Include xunit.ps1
Include nunit.ps1
Include nuget.ps1
Include msbuild.ps1
Include assemblyinfo.ps1
Include overrides.ps1
#Include git.ps1

properties {
  Write-Output "Loading build properties"
  # Do not put any code in here. This method is invoked before all others
  # and will not have access to any of your shared properties.
}

Task Default -depends Initialize, Compile, Test
Task Release -depends Default
Task Deploy -depends Publish

Task Test { 
  $test_dlls = gci "$($build.dir)\*.Tests.dll"
  Invoke-TestRunner $test_dlls
}

Task IntegrationTest -Depends Test { 
  $test_dlls = gci "$($build.dir)\*.IntegrationTests.dll"
  Invoke-TestRunner $test_dlls
}

Task Initialize -Depends Clean, Bootstrap-NuGetPackages {
  New-Item $release.dir -ItemType Directory | Out-Null
  New-Item $build.dir -ItemType Directory | Out-Null
}

Task Compile -Depends Version-AssemblyInfo, Initialize, Invoke-MsBuild

Task Clean { 
  Remove-Item -Force -Recurse $build.dir -ErrorAction SilentlyContinue | Out-Null
  Remove-Item -Force -Recurse $release.dir -ErrorAction SilentlyContinue | Out-Null
}

Task Publish -Depends Package {
  Publish-NuGetPackage
}

Task Package -Depends Create-NuGetPackage {
}

Task ? -Description "Helper to display task info" {
  Write-Documentation
}
