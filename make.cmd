@echo off
setlocal
powershell -NoLogo -NoProfile -NonInteractive -ExecutionPolicy Bypass -File %~dp0\make.ps1 %*
