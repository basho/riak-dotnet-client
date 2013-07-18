CorrugatedIron Build Process
============================

* Merge all required feature branches into develop.
* Verify that all tests succeed.
* Make sure that the TravisCI build succeeds.
* run `git flow release start vA.B.C`
* Edit CorrugatedIron.nuspec and VersionInfo.cs so that the version numbers are up to date.
* To produce a new `release` build open a command prompt, change to the CI folder and run: `make`
* Verify that two nupkg files were created, one for the library and one for the symbols. The version number should match that which you are releasing.
* Finish the release by running: `git flow release finish vA.B.C`
* Push all the branches/tags up: `git push origin master:master && git push origin develop:develop && git push --tags`
* Push to Nuget by running (from the same command prompt): `.nuget\Nuget.exe push CorrugatedIron.VERSION.nupkg` (it should also push up the symbols).
* Make any changes to samples that might be required.
* Make any changes to documentation that might be required.

Give yourself a pat on the back and have some tea.
