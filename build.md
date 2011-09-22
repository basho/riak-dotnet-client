CorrugatedIron Build Process
============================

* Verify tests all succeed.
* Merge branches into develop
* Create release build
* git flow release start vA.B.C
* bump version in .nuspec and VersionInfo.cs
* run the magical nugget .ps1 file
* git flow release finish vA.B.C
* nugget push PACKAGE.nupkg
* tag the release in git
* git push --tags
* make any changes to samples
* push to the master repo
