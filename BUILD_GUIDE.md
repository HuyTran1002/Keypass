# Keypass - Build & Deployment Guide

## Project Overview

Keypass is a password manager application for Windows that:
- Runs in system tray
- Automatically detects login forms
- Suggests saved credentials
- Auto-fills username and password
- Stores credentials securely in SQLite database

## Project Structure

```
Keypass/
├── src/
│   ├── Program.cs                     # Application entry point
│   ├── Models/
│   │   └── Credential.cs             # Credential data model
│   ├── Services/
│   │   ├── DatabaseService.cs        # SQLite CRUD operations
│   │   ├── UIHookService.cs          # Windows hook for monitoring
│   │   └── SettingsService.cs        # Settings management
│   └── UI/
│       ├── TrayApplicationContext.cs # System tray integration
│       ├── PasswordManagerForm.cs    # Main UI window
│       ├── AddEditPasswordForm.cs    # Add/Edit credential dialog
│       └── SettingsForm.cs           # Settings dialog
├── .vscode/
│   ├── launch.json                   # Debug configuration
│   └── tasks.json                    # Build/Run tasks
├── .github/
│   └── copilot-instructions.md       # Development notes
├── Keypass.csproj                    # Project file
├── README.md                         # English documentation
├── HUONG_DAN_VIETNAMESE.md           # Vietnamese guide
└── .gitignore                        # Git ignore patterns
```

## Prerequisites

### System Requirements
- Windows 7 or later
- .NET 6.0 Runtime or SDK

### Development Requirements
- Visual Studio 2022 (recommended) OR
- .NET 6.0 SDK
- Git (optional)

### Install .NET 6.0 SDK

**Windows**:
```powershell
# Using winget
winget install Microsoft.DotNet.SDK.6

# Or download from https://dotnet.microsoft.com/download/dotnet/6.0
```

## Building the Project

### Method 1: Using Command Line

```bash
# Navigate to project directory
cd path/to/Keypass

# Restore dependencies
dotnet restore

# Build Debug version
dotnet build

# Build Release version
dotnet build -c Release

# Run the application
dotnet run

# Publish as executable
dotnet publish -c Release -o ./publish
```

### Method 2: Using Visual Studio 2022

1. Open Visual Studio 2022
2. File → Open → Folder
3. Select the Keypass folder
4. Right-click on Keypass.csproj → Build
5. Run project (F5)

### Method 3: Using VS Code

1. Open folder in VS Code
2. Terminal → Run Build Task (Ctrl+Shift+B)
3. Select "build" task
4. Press F5 to debug or Ctrl+Shift+D to run

## Building Output

### Debug Build
- Location: `bin/Debug/net6.0-windows/`
- Files: `Keypass.exe`, `Keypass.dll`, and dependencies
- Size: ~30-40 MB (with .NET runtime included)

### Release Build
- Location: `bin/Release/net6.0-windows/`
- Files: Optimized executable with dependencies
- Size: ~25-30 MB

## Publishing the Application

### Self-Contained Release

```bash
dotnet publish -c Release -o ./publish --self-contained
```

This creates a folder with:
- `Keypass.exe` - Main executable
- All required .NET runtime files
- Can run on Windows without .NET installed

### Framework-Dependent Release

```bash
dotnet publish -c Release -o ./publish
```

Requires .NET 6.0 Runtime on target machine but is smaller.

## Running the Application

### From Command Line
```bash
# Navigate to output directory
cd ./publish

# Run the application
./Keypass.exe
```

### From GUI
1. Navigate to the publish folder
2. Double-click `Keypass.exe`
3. Application will appear in system tray

## Configuration Files

### Database Location
```
%APPDATA%\Keypass\credentials.db
```

### Settings Location
```
%APPDATA%\Keypass\settings.json
```

## Creating an Installer

### Using NSIS (free, open-source)

1. Install NSIS from https://nsis.sourceforge.io/

2. Create installer script `installer.nsi`:

```nsis
; Keypass Installer Script

!include "MUI2.nsh"

Name "Keypass Password Manager"
OutFile "KeypassInstaller.exe"
InstallDir "$PROGRAMFILES\Keypass"

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_LANGUAGE "English"

Section "Install"
  SetOutPath "$INSTDIR"
  File /r "publish\*.*"
  
  ; Create Start Menu shortcuts
  CreateDirectory "$SMPROGRAMS\Keypass"
  CreateShortCut "$SMPROGRAMS\Keypass\Keypass.lnk" "$INSTDIR\Keypass.exe"
  CreateShortCut "$SMPROGRAMS\Keypass\Uninstall.lnk" "$INSTDIR\uninstall.exe"
  
  ; Create Desktop shortcut
  CreateShortCut "$DESKTOP\Keypass.lnk" "$INSTDIR\Keypass.exe"
  
  ; Write uninstaller
  WriteUninstaller "$INSTDIR\uninstall.exe"
SectionEnd

Section "Uninstall"
  Delete "$INSTDIR\*.*"
  RMDir /r "$INSTDIR"
  RMDir /r "$SMPROGRAMS\Keypass"
  Delete "$DESKTOP\Keypass.lnk"
SectionEnd
```

3. Build installer:
```bash
makensis installer.nsi
```

### Using WiX Toolset (Advanced)

WiX provides more advanced installer options but requires more setup.

## Running Tests

Currently no unit tests are included. To add tests:

```bash
# Create test project
dotnet new xunit -n Keypass.Tests

# Add reference to main project
dotnet add Keypass.Tests reference Keypass.csproj

# Run tests
dotnet test
```

## Debugging

### VS Code Debugging

1. Press F5 or go to Debug → Start Debugging
2. Breakpoints can be set by clicking line numbers
3. Use the Debug Console to inspect variables

### Visual Studio Debugging

1. Press F5 to start debugging
2. Set breakpoints by clicking margin
3. Use Debug → Windows to view various debug information

### Troubleshooting Builds

**Error: "SDK not found"**
```bash
dotnet --version  # Check if .NET is installed
dotnet --list-sdks  # List available SDKs
```

**Error: "Package not found"**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages again
dotnet restore
```

**Error: "File already in use"**
- Close any running instances of the application
- Delete `bin` and `obj` folders
- Run clean build: `dotnet clean && dotnet build`

## Performance Optimization

### Reduce Executable Size

```bash
# Trim unused code
dotnet publish -c Release -o ./publish --self-contained --no-restore -p:PublishTrimmed=true

# Enable AOT compilation (Advanced)
dotnet publish -c Release -o ./publish -p:PublishAot=true
```

### Improve Startup Time

- The application should start within 1-2 seconds
- First run may take longer due to JIT compilation
- Consider using ReadyToRun compilation:

```bash
dotnet publish -c Release -o ./publish -p:PublishReadyToRun=true
```

## Continuous Integration / Deployment

### GitHub Actions Example

Create `.github/workflows/build.yml`:

```yaml
name: Build Keypass

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: windows-latest
    steps:
    - uses: actions/checkout@v2
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '6.0.x'
    - name: Restore
      run: dotnet restore
    - name: Build
      run: dotnet build --configuration Release --no-restore
    - name: Publish
      run: dotnet publish -c Release -o ./publish
    - name: Upload artifacts
      uses: actions/upload-artifact@v2
      with:
        name: keypass-release
        path: publish/
```

## Versioning

Update version in `Keypass.csproj`:

```xml
<PropertyGroup>
  <Version>1.1.0</Version>
  <FileVersion>1.1.0.0</FileVersion>
  <ProductVersion>1.1.0.0</ProductVersion>
</PropertyGroup>
```

## Creating Release Packages

### Step-by-step Release Process

1. Update version number in .csproj
2. Build Release: `dotnet publish -c Release -o ./publish --self-contained`
3. Create installer (if using NSIS)
4. Create release notes
5. Package files:
```bash
# Create zip archive
Compress-Archive -Path ./publish -DestinationPath ./Keypass-v1.0.0.zip
```

6. Upload to GitHub Releases or distribution platform

## Troubleshooting Common Issues

### Application won't start
- Check event viewer for errors
- Run in admin mode
- Verify .NET 6.0 is installed

### Database errors
- Delete `%APPDATA%\Keypass\credentials.db` and restart
- Check write permissions on %APPDATA%

### Hook not detecting forms
- Ensure running as Administrator
- Check Windows Defender isn't blocking
- Verify AutoFill is enabled in Settings

## Next Steps

1. Create MSI installer using WiX
2. Add Unit Tests
3. Implement code signing for release builds
4. Set up automated testing with GitHub Actions
5. Create Windows Store package
6. Implement auto-update functionality

---

For more information, see README.md and HUONG_DAN_VIETNAMESE.md
