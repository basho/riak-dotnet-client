new-item -itemtype directory release\lib\net40

Copy-Item ..\CorrugatedIron\bin\Release\CorrugatedIron.dll  .\release\lib\net40

Copy-Item .\CorrugatedIron.nuspec .\release
Copy-Item -recurse .\Content .\release

NuGet.exe pack .\release\CorrugatedIron.nuspec

remove-item -recurse -force .\release
