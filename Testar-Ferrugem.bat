@echo off
setlocal
set "FERRUGEM_DEFAULT_MODE=-Manual"
set "FERRUGEM_PAUSE=1"
for %%A in (%*) do (
    if /I "%%~A"=="-Manual" set "FERRUGEM_DEFAULT_MODE="
    if /I "%%~A"=="-Smoke" (
        set "FERRUGEM_DEFAULT_MODE="
        set "FERRUGEM_PAUSE=0"
    )
)
powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0Tools\Start-LinuxTest.ps1" %FERRUGEM_DEFAULT_MODE% %*
set "FERRUGEM_EXIT=%ERRORLEVEL%"
if "%FERRUGEM_PAUSE%"=="1" pause
exit /b %FERRUGEM_EXIT%
