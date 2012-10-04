# 
# Copyright (c) 2012, Toji Project Contributors
# 
# Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
# See the file LICENSE.txt for details.
# 

properties {
  Write-Output "Loading git properties"
  $git = @{}
  $git.file = @(Get-Command git)[0]
}

function Invoke-Git {
  param(
    [Parameter(Position=0,Mandatory=0)]
    [string[]]$args = @()
  )
  if ($args.Length -le 0) { 
     Write-Output "No git arguments defined"
     return 
  }
  & $git.file $args
}

function Get-GitRevision {
  ([regex]"-\d{1,5}-").Match((Invoke-Git describe)).Value.Trim('-')
}