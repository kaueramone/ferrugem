@echo off
setlocal
powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0Tools\Start-LinuxTest.ps1" %*
set "FERRUGEM_EXIT=%ERRORLEVEL%"
for %%A in (%*) do if /I "%%~A"=="-Manual" pause
exit /b %FERRUGEM_EXIT%
