; Keypass Password Manager - Inno Setup Script
; Requires: Inno Setup 6+
; Build app first: dotnet publish -c Release -o publish

[Setup]
AppId={{C7C2135D-8D4C-4E0B-A3B6-KEYPASS-2026}}
AppName=Keypass Password Manager
AppVersion=1.0.0
AppPublisher=Keypass
DefaultDirName={pf}\Keypass
DefaultGroupName=Keypass
OutputDir=installer\output
OutputBaseFilename=KeypassSetup
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64
DisableDirPage=no
DisableProgramGroupPage=no
UninstallDisplayIcon={app}\Keypass.exe
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop shortcut"; GroupDescription: "Additional icons:";
Name: "autostart"; Description: "Start Keypass on Windows startup"; GroupDescription: "Startup:";

[Files]
; Include all published files
Source: "publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Keypass"; Filename: "{app}\Keypass.exe"
Name: "{commondesktop}\Keypass"; Filename: "{app}\Keypass.exe"; Tasks: desktopicon

[Registry]
; Autostart (optional task)
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "Keypass"; ValueData: "\"{app}\Keypass.exe\""; Flags: uninsdeletevalue; Tasks: autostart

[Run]
; Launch app after install
Filename: "{app}\Keypass.exe"; Description: "Launch Keypass"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
; Remove installer output folder if needed
Type: filesandordirs; Name: "{app}\logs"
Type: filesandordirs; Name: "{app}\temp"
