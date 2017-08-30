!define PRODUCT_NAME "BarCheck"
!define PRODUCT_VERSION "1.0"
!define APPNAME "BarCheck"
!define COMPANYNAME "CDSEIM"
!define INSTALLSIZE 600
SetCompressor /SOLID lzma
SetCompressorDictSize 32


InstallDir "$PROGRAMFILES64\BarCheck"

OutFile "BarCheck_install.exe"

!include LogicLib.nsh
page directory
Page instfiles

Section "x" SEC01

SectionIn RO
  SetOutPath "$INSTDIR\Wav"
  SetOverwrite on
  File "Releases\Wav\*"
 
  SetOutPath "$INSTDIR"
  SetOverwrite on
File "seim.ico"
File "Releases\*"
writeUninstaller "$INSTDIR\uninstall.exe"
CreateShortCut "$DESKTOP\BarCheck.lnk" "$INSTDIR\BarCheck.exe" "" "$INSTDIR\seim.ico"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "DisplayName" "BarCheck"
WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "Publisher" "$\"${COMPANYNAME}$\""
WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "EstimatedSize" ${INSTALLSIZE}
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "QuietUninstallString" "$\"$INSTDIR\uninstall.exe$\" /S"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "InstallLocation" "$\"$INSTDIR$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "DisplayIcon" "$\"$INSTDIR\seim.ico$\""
	

SectionEnd

section "uninstall"
 
	# Remove Start Menu launcher
	delete "$DESKTOP\BarCheck.lnk"
 
 
	# Remove files
	delete $INSTDIR\*
	delete $INSTDIR\Wav\*
	rmDir $INSTDIR\Wav
 
	# Always delete uninstaller as the last action
	delete $INSTDIR\uninstall.exe
 
	# Try to remove the install directory - this will only happen if it is empty
	rmDir $INSTDIR
 
	# Remove uninstaller information from the registry
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}"
sectionEnd
