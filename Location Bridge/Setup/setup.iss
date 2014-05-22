#define _AppName "Location Bridge"
#define _AppVersion "1.0"
#define _AppPublisher "Al Brown"
#define _AppURL "http://eartoearoak.com/location-bridge"
#define _AppExeName "LocationBridge.exe"
#define _DotNet "NDP451-KB2859818-Web.exe"

[Setup]
AppId={{11305330-A050-4C36-BBC4-0AB1C94D29FB}
AppName={#_AppName}
AppVersion={#_AppVersion}
AppPublisher={#_AppPublisher}
AppCopyright={#_AppPublisher}
AppPublisherURL={#_AppURL}
AppSupportURL={#_AppURL}
AppUpdatesURL={#_AppURL}
DefaultDirName={pf}\{#_AppName}
DefaultGroupName={#_AppName}
OutputBaseFilename=setup
SetupIconFile=..\icon.ico
UninstallDisplayIcon={app}\{#_AppExeName}, 1
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64
LicenseFile=gplv3.txt
MinVersion=0,6.1

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\bin\x86\Release\{#_AppExeName}"; DestDir: "{app}"; Flags: ignoreversion; Check: not Is64BitInstallMode
Source: "..\bin\x64\Release\{#_AppExeName}"; DestDir: "{app}"; Flags: ignoreversion; Check: Is64BitInstallMode
Source: "{#_DotNet}"; DestDir: "{tmp}"; Flags: deleteafterinstall; Check: not DotNetCheck; AfterInstall: DotNetInstall

[Icons]
Name: "{group}\{#_AppName}"; Filename: "{app}\{#_AppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#_AppName}}"; Filename: "{#_AppURL}"
Name: "{group}\{cm:UninstallProgram,{#_AppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#_AppName}"; Filename: "{app}\{#_AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#_AppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(_AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[ThirdParty]
UseRelativePaths=True

[Code]
function DotNetCheck(): boolean;
var
  reg: string;
  release : cardinal;
begin
  reg := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full';

  result := RegQueryDWordValue(HKLM, reg, 'Release', release);
  result := result and (release >= 378389);
end;

procedure DotNetInstall();
var
  status: string;
  code: integer;
begin
  status := WizardForm.StatusLabel.Caption;
  WizardForm.StatusLabel.Caption := 'Installing .Net 4.5.1';
  WizardForm.ProgressGauge.Style := npbstMarquee;
  Exec(ExpandConstant('{tmp}\{#_DotNet}'), '/q /noreboot', '', SW_SHOW, ewWaitUntilTerminated, code)
  if (code <> 0) then
  begin
    MsgBox('.NET 4.5.1 installation failed.', mbError, MB_OK);
  end;
  WizardForm.StatusLabel.Caption := status;
  WizardForm.ProgressGauge.Style := npbstNormal;
end;

