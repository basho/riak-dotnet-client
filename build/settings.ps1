# 
# Copyright (c) 2011-2012, Toji Project Contributors
# 
# Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
# See the file LICENSE.txt for details.
# 

# This file contains the build-wide settings used by all scripts.
# The overrides.ps1 can be used for CI settings where conventions don't work.

properties {
  Write-Output "Loading settings properties"
  
  $base = @{}
  $base.dir = Resolve-Path .\..\
  
  $source = @{}
  $source.dir = "$($base.dir)\src"
  if(!(Test-Path($source.dir))) { $source.dir = "$($base.dir)\source" }
  if(!(Test-Path($source.dir))) { $source.dir = "$($base.dir)" }
  
  $build = @{}
  $build.dir = "$($base.dir)\bin\$framework"
  $build.configuration = "Release"

  # BUILD_NUMBER is defined during CI builds. Make sure that this value
  # is changed if the CI system in use does not set this variable.
  # Make sure Semver versioning is used for the build number.
  $build.version = if($env:BUILD_NUMBER) { $env:BUILD_NUMBER } else { "2.5.0.0" }
  
  $tools = @{}
  $tools.dir = "$($base.dir)\tools"
  
  $solution = @{}
  $solution.name = "$(Split-Path $($base.dir) -leaf)"
  $solution.file = "$($base.dir)\$($solution.name).sln"
  
  $release = @{}
  $release.dir = "$($base.dir)\release"
  
  $packages = @{}
  $packages.name = "lib"
  $packages.dir = "$($base.dir)\$($packages.name)"
}