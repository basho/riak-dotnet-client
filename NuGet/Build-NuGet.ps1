# create the release folder
new-item -itemtype directory release\lib\net40

# prepare for the release
Copy-Item .\CorrugatedIron.nuspec .\release
Copy-Item -recurse .\Content .\release
Copy-Item ..\CorrugatedIron\bin\Release\CorrugatedIron.dll  .\release\lib\net40
Copy-Item ..\CorrugatedIron\bin\Release\CorrugatedIron.pdb  .\release\lib\net40
Copy-Item -rec -filter *.cs ..\CorrugatedIron .\release
Rename-Item .\release\CorrugatedIron src

# create the symbol source release first, as this will have everything in it
NuGet.exe pack .\release\CorrugatedIron.nuspec -Symbols -NoDefaultExcludes -Exclude *.nu*

# remove stuff we don't want
remove-item -recurse -force .\release\src
remove-item .\release\lib\net40\*.pdb

# create the binary release which doesn't have the PDBs or source in it
NuGet.exe pack .\release\CorrugatedIron.nuspec

# clean up all the stuff we don't need/want any more
remove-item -recurse -force .\release
