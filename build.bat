@echo off
echo ========================================
echo BEBE Task Recorder v3.0 - C# Build
echo ========================================
echo.

REM Check if .NET SDK is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [ERROR] .NET SDK not found!
    echo.
    echo Please install .NET SDK 6.0 or later from:
    echo https://dotnet.microsoft.com/download
    echo.
    pause
    exit /b 1
)

echo [OK] .NET SDK version: 
dotnet --version
echo.

REM Restore NuGet packages
echo [INFO] Restoring NuGet packages...
dotnet restore
if errorlevel 1 (
    echo [ERROR] Failed to restore packages!
    pause
    exit /b 1
)

echo [OK] Packages restored
echo.

REM Build the project
echo [INFO] Building project...
dotnet build -c Release
if errorlevel 1 (
    echo [ERROR] Build failed!
    pause
    exit /b 1
)

echo [OK] Build successful
echo.

REM Publish as single-file executable
echo [INFO] Publishing as standalone executable...
echo This may take a minute...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false -o dist
if errorlevel 1 (
    echo [ERROR] Publish failed!
    pause
    exit /b 1
)

echo.
echo ========================================
echo BUILD COMPLETE!
echo ========================================
echo.
echo Executable location:
echo   dist\BebeTaskRecorder.exe
echo.
echo Size: 
for %%A in ("dist\BebeTaskRecorder.exe") do echo   %%~zA bytes (%%~zAk KB)
echo.
echo This executable:
echo  - Requires NO Python
echo  - Requires NO PyInstaller
echo  - Includes .NET runtime
echo  - Can generate other EXEs (if .NET SDK is on target system)
echo.
echo To test: Run dist\BebeTaskRecorder.exe as Administrator
echo.
pause

