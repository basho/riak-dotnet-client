# 
# Copyright (c) 2011-2012, Toji Project Contributors
# 
# Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
# See the file LICENSE.txt for details.
# 

properties {
  Write-Output "Loading xunit properties"
  $xunit = @{}
  $xunit.logfile = "$($build.dir)\xunit.log.xml"
  $xunit.runner = (Get-ChildItem "$($base.dir)\*" -recurse -include xunit.console.clr4.x86.exe).FullName
}

function Invoke-TestRunner {
  param(
    [Parameter(Position=0,Mandatory=$true)]
    [string[]]$dlls = @()
  )

  Assert ((Test-Path($xunit.runner)) -and (![string]::IsNullOrEmpty($xunit.runner))) "xunit runner could not be found"

  if ($dlls.Length -le 0) { 
     Write-Output "No tests defined"
     return 
  }

  # TODO: This only keeps the last log file. Need to output many and merge.
  $dlls | % { exec { & $xunit.runner "$_" /noshadow }}
}