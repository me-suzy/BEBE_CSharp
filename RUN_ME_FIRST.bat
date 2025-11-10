@echo off
title BEBE Task Recorder v3.0 - Setup & Build
color 0A

echo.
echo  ========================================================
echo            BEBE TASK RECORDER v3.0 - C# VERSION
echo  ========================================================
echo.
echo  This will check requirements and build the application.
echo.
pause

REM Check .NET SDK
echo [1/2] Checking .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    color 0C
    echo.
    echo  [X] .NET SDK NOT FOUND!
    echo.
    echo  Please download and install .NET SDK 6.0 or later:
    echo  https://dotnet.microsoft.com/download
    echo.
    echo  After installing, restart this script.
    echo.
    pause
    exit /b 1
)

color 0A
echo  [OK] .NET SDK found: 
dotnet --version
echo.

REM Build
echo [2/2] Building application...
call build.bat

echo.
echo  ========================================================
echo  Setup complete! You can now run:
echo    dist\BebeTaskRecorder.exe
echo  ========================================================
echo.
pause

