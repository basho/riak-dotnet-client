# 
# Copyright (c) 2011-2012, Toji Project Contributors
# 
# Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
# See the file LICENSE.txt for details.
# 

properties {
  Write-Output "Loading assembly info properties"
  # The assemblyinfo filename and location can be overridden in the settings.ps1
  # You can also override the version pattern to "\d*\.\d*\.\d*\.\d*" if you want 4 digit
  $assemblyinfo = @{}
  $assemblyinfo.dir = "$($source.dir)"
  $assemblyinfo.version_pattern = "\d*\.\d*\.\d*.\d*"   # 3 digit for semver
  $assemblyinfo.file = "GlobalAssemblyInfo.cs"
  $assemblyinfo.contents = @"
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyCompany(`"`")]
[assembly: AssemblyProduct(`"`")]
[assembly: AssemblyCopyright(`"Copyright © 2012`")]
[assembly: AssemblyTrademark(`"`")]
[assembly: AssemblyCulture(`"`")]
[assembly: AssemblyVersion(`"1.0.0`")]
[assembly: AssemblyFileVersion(`"1.0.0`")]
"@
}

Task Version-AssemblyInfo {
  $file = "$($assemblyinfo.dir)\$($assemblyinfo.file)"
  if(!(Test-Path($file))) { 
    Set-Content -Value $assemblyinfo.contents -Path $file
    Write-Host -ForegroundColor Red "GlobalAssemblyInfo was not detected has has been created: $($assemblyinfo.file)"
  }
  $content = Get-Content $file | % { [Regex]::Replace($_, $assemblyinfo.version_pattern, $build.version) } 
  Set-Content -Value $content -Path $file
}