@echo off
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Generate-NovaFoundation.ps1" -Profile VerticalSliceV1 -Force
pause
