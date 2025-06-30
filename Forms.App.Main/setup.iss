[Setup]
AppName=商友联盟
AppVersion=1.0.0
DefaultDirName={pf}\PvFriend
DefaultGroupName=PvFriend
OutputDir=..\package
OutputBaseFilename=商友联盟
Compression=lzma2
SolidCompression=yes
DisableDirPage=no

[Languages]
Name: "chinesesimp"; MessagesFile: "compiler:Languages\ChineseSimplified.isl"

[Files]
Source: ".\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "./setup.iss"


[Icons]
Name: "{group}\商友联盟"; Filename: "{app}\Forms.App.Main.exe"
Name: "{userdesktop}\商友联盟"; Filename: "{app}\Forms.App.Main.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "在桌面创建快捷方式"; GroupDescription: "附加任务："

[Run]
Filename: "{app}\Forms.App.Main.exe"; Description: "启动 商友联盟"; Flags: nowait postinstall skipifsilent
