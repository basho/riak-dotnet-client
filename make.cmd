@echo off
setlocal EnableExtensions

if (%FrameworkDir%)==() set FrameworkDir=%WINDIR%\Microsoft.NET\Framework\
if (%FrameworkVersion%)==() set FrameworkVersion=v4.0.30319

rem NB: %~dp0 ends in a backslash
set CURDIR=%~dp0
set NUGETEXE=%CURDIR%\.nuget\NuGet.exe
if not exist %NUGETEXE% goto ERR_NUGETEXE

set MSBUILDEXE=%FrameworkDir%%FrameworkVersion%\msbuild.exe
if not exist %MSBUILDEXE% goto ERR_MSBUILD

rem Targets:
rem Build - Normal build of a Configuration
rem Clean - Clean everything
rem PBuild - Parallel build of all Configurations
rem ProtoGen - Rebuild cs files from proto files
set TARGET=%1
if (%TARGET%)==() set TARGET=Build

set CONFIGURATION=%2
if (%CONFIGURATION%)==() set CONFIGURATION=Release

set VERBOSITY=%3
if (%VERBOSITY%)==() set VERBOSITY=Normal

%NUGETEXE% restore -PackagesDirectory %CURDIR%\packages .nuget\packages.config
if errorlevel 1 goto ERR_FAILED

rem NB: this will always do build for Release *and* Debug configuration
%MSBUILDEXE% /verbosity:%VERBOSITY% /nologo /m /property:SolutionDir=%CURDIR% /property:Configuration=%CONFIGURATION% /t:%TARGET% %CURDIR%\build\build.proj
if errorlevel 1 goto ERR_FAILED

echo.
echo ************************
echo *** BUILD SUCCESSFUL ***
echo ************************
echo.
goto :EOF

:ERR_FAILED
echo.
echo ********************
echo *** BUILD FAILED ***
echo ********************
echo.
exit /b 1

:ERR_NUGETEXE
echo.
echo. ERROR: Can't find NuGet.exe (expected: %NUGETEXE%)
echo.
exit /b 1

:ERR_MSBUILD
echo.
echo. ERROR: Can't find MSBuild.exe (expected: %MSBUILDEXE%)
echo.
exit /b 1

