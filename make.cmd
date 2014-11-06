.nuget\NuGet.exe restore -PackagesDirectory .\packages .nuget\packages.config

%windir%\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe make.msbuild /target:Release
