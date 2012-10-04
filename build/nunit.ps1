# 
# Copyright (c) 2011-2012, Toji Project Contributors
# 
# Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
# See the file LICENSE.txt for details.
# 

properties {
  Write-Output "Loading nunit properties"
  $nunit = @{}
  $nunit.runner = (Get-ChildItem "$($base.dir)\*" -recurse -include nunit-console-x86.exe).FullName
}

function Invoke-TestRunner {
  param(
    [Parameter(Position=0,Mandatory=$true)]
    [string[]]$dlls = @()
  )

  Assert ((Test-Path($nunit.runner)) -and (![string]::IsNullOrEmpty($nunit.runner))) "NUnit runner could not be found"
  
  if ($dlls.Length -le 0) { 
     Write-Output "No tests defined"
     return 
  }

  $testOutput = $build.dir + '\test-results.xml'
  
  $targetNunitFramework = 'net-4.0'
  if ($framework -ne '4.0')
  {
	$targetNunitFramework = 'net-3.5'
  }

  exec { & $nunit.runner $dlls /noshadow /framework=$targetNunitFramework /xml=$testOutput}
}