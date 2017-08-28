﻿function ZipFiles( $zipfilename, $sourcedir )
{
   Add-Type -Assembly System.IO.Compression.FileSystem
   $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
   [System.IO.Compression.ZipFile]::CreateFromDirectory($sourcedir,$zipfilename)
}
$BarDir = 'C:\G\BarCheck'
cd 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin'
.\msbuild "$BarDir\BarCheck\BarCheck\BarCheck.csproj" /p:Configuration=Release /p:DefineConstants="MFE1" /p:DefineConstants="A" /t:Rebuild

$dt = [DateTime]::Now.ToString("yyyyMMddHHmm") 
$zipdir ="$BarDir\Releases"
$debugdir = "$BarDir\BarCheck\BarCheck\bin\Release"
$exclude = @('*.pdb','*.xml','*vshost*','*.log')
Copy-Item "$debugdir\*" $zipdir -Recurse -Exclude $exclude -Force
Copy-Item "$BarDir\说明.txt" $zipdir -Force

cd 'C:\Program Files (x86)\NSIS'
.\makensis.exe "$BarDir\BarCheck.nsi"
$installout = "$BarDir\BarCheck_install.exe"
$installzipdir = "$BarDir\BarCheck_install"
Remove-Item "$installzipdir\*" -Force
Copy-Item $installout $installzipdir  -Force

cd $BarDir

$zipfile = "$BarDir\\BarCheck_install_$dt.zip"
If (Test-Path $zipfile){
	Remove-Item $zipfile
}
ZipFiles  $zipfile $installzipdir 
If (Test-Path $installout){
	Remove-Item $installout -Force
}
Copy-Item $zipfile $installzipdir  -Force

If (Test-Path $zipfile){
	Remove-Item $zipfile -Force
}
$zipfile
Read-Host

