CorrugatedIron Build Process
============================

1. Verify tests all succeed.
2. Merge branches into develop
3. Create release build
4. git flow release start vA.B.C
5. bump version in .nuspec and VersionInfo.cs
6. run the magical nugget .ps1 file
7. nugget push PACKAGE.nupkg
8. tag the release in git
9. make any changes to samples
10. push to the master repo
