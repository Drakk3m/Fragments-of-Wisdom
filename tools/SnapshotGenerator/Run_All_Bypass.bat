@echo off
powershell.exe -ExecutionPolicy Bypass -NoProfile -File "%~dp0Run_All.ps1" %*
pause
