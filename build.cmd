@echo off

powershell -NonInteractive -NoProfile -ExecutionPolicy Unrestricted -Command "& { .\build.ps1 %* }"