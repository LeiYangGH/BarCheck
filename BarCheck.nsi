!define PRODUCT_NAME "BarCheck"
!define PRODUCT_VERSION "1.0"
#!define PRODUCT_WEB_SITE "http://www.leiyang.com"

SetCompressor /SOLID lzma
SetCompressorDictSize 32


!define MUI_ICON "seim.ico";


InstallDir "$PROGRAMFILES\BarCheck"

OutFile "BarCheck_install.exe"

Section "x" SEC01
SectionIn RO 
  SetOutPath "$INSTDIR"
  SetOverwrite on
File "หตร๗.txt"
File "seim.ico"
File "Releases\BarCheck.exe"
CreateShortCut "$DESKTOP\BarCheck.lnk" "$INSTDIR\BarCheck.exe" "" "$INSTDIR\seim.ico"
SectionEnd
