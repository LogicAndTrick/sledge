; Sledge NSIS Installer
; ---------------------

; Installer Info
Name "Sledge"
OutFile "Sledge.Editor.{version}.exe"
InstallDir "$PROGRAMFILES\Sledge Editor"
InstallDirRegKey HKLM "Software\Sledge\Editor" "InstallDir"
RequestExecutionLevel admin

; Version Info
VIProductVersion "{version}"
VIAddVersionKey "FileVersion" "{version}"
VIAddVersionKey "ProductName" "Sledge Editor"
VIAddVersionKey "FileDescription" "Installer for Sledge Editor"
VIAddVersionKey "LegalCopyright" "http://logic-and-trick.com 2018"

; Ensure Admin Rights
!include LogicLib.nsh

Function .onInit
    UserInfo::GetAccountType
    pop $0
    ${If} $0 != "admin" ;Require admin rights on NT4+
        MessageBox mb_iconstop "Administrator rights required!" /SD IDOK
        SetErrorLevel 740 ;ERROR_ELEVATION_REQUIRED
        Quit
    ${EndIf}
FunctionEnd

; Installer Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

; Installer Sections

Section "Sledge Editor"
    IfSilent 0 +2 ; Silent mode: Sledge has executed the installer for an update
        Sleep 2000 ; Make sure the program has shut down...
    
    SectionIn RO
    SetOutPath $INSTDIR

    ; Purge junk from old installs
    Delete "$INSTDIR\*.dll"
    Delete "$INSTDIR\*.pdb"
    Delete "$INSTDIR\Sledge.Editor.Elevate.exe"
    Delete "$INSTDIR\Sledge.Editor.Updater.exe"
    Delete "$INSTDIR\UpdateSources.txt"

    File /r "Build\*"
    
    WriteRegStr HKLM "Software\Sledge\Editor" "InstallDir" "$INSTDIR"
    WriteRegStr HKLM "Software\Sledge\Editor" "Version" "{version}"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SledgeEditor" "DisplayName" "Sledge Editor"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SledgeEditor" "UninstallString" '"$INSTDIR\Uninstall.exe"'
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SledgeEditor" "NoModify" 1
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SledgeEditor" "NoRepair" 1
    WriteUninstaller "Uninstall.exe"
SectionEnd

Section "Start Menu Shortcuts"
    IfSilent 0 +2
        Goto end ; Silent update: Don't redo shortcuts
        
    SetShellVarContext all
    CreateDirectory "$SMPROGRAMS\Sledge Editor"
    CreateShortCut "$SMPROGRAMS\Sledge Editor\Uninstall.lnk" "$INSTDIR\Uninstall.exe" "" "$INSTDIR\Uninstall.exe" 0
    CreateShortCut "$SMPROGRAMS\Sledge Editor\Sledge Editor.lnk" "$INSTDIR\Sledge.Editor.exe" "" "$INSTDIR\Sledge.Editor.exe" 0

    end:
SectionEnd

Section "Desktop Shortcut"
    IfSilent 0 +2
        Goto end ; Silent update: Don't redo shortcuts
    
    SetShellVarContext all
    CreateShortCut "$DESKTOP\Sledge Editor.lnk" "$INSTDIR\Sledge.Editor.exe" "" "$INSTDIR\Sledge.Editor.exe" 0
    
    end:
SectionEnd

Section "Run Sledge After Installation"
    SetAutoClose true
    Exec "$INSTDIR\Sledge.Editor.exe"
SectionEnd

; Uninstall

Section "Uninstall"

  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SledgeEditor"
  DeleteRegKey HKLM "Software\Sledge\Editor"

  SetShellVarContext all
  Delete "$SMPROGRAMS\Sledge Editor\*.*"
  Delete "$DESKTOP\Sledge Editor.lnk"

  RMDir /r "$SMPROGRAMS\Sledge Editor"
  RMDir /r "$INSTDIR"

SectionEnd