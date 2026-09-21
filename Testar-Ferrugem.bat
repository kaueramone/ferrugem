@echo off
setlocal
powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0Tools\Start-LocalTest.ps1" %*
set "FERRUGEM_EXIT=%ERRORLEVEL%"
set "FERRUGEM_PAUSE=1"
for %%A in (%*) do if /I "%%~A"=="-Smoke" set "FERRUGEM_PAUSE=0"
if "%FERRUGEM_PAUSE%"=="1" pause
exit /b %FERRUGEM_EXIT%
