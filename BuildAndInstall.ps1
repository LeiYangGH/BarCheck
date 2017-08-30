function ZipFiles( $zipfilename, $sourcedir )
{
   Add-Type -Assembly System.IO.Compression.FileSystem
   $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
   [System.IO.Compression.ZipFile]::CreateFromDirectory($sourcedir,$zipfilename)
}
$BarDir = 'C:\G\BarCheck'
$MSBuildDir = 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin'
If (-not (Test-Path $MSBuildDir)){
	$MSBuildDir = 'E:\VS17\MSBuild\15.0\Bin'
}
cd $MSBuildDir
.\msbuild "$BarDir\BarCheck\BarCheck\BarCheck.csproj" /p:Configuration=Release /p:DefineConstants="A" /t:Rebuild

$dt = [DateTime]::Now.ToString("yyyyMMddHHmm") 
$makefromdir ="$BarDir\Releases"
$debugdir = "$BarDir\BarCheck\BarCheck\bin\Release"
$exclude = @('*.pdb','*.xml','*vshost*','*.log')
Remove-Item "$makefromdir\*" -Recurse -Force
Copy-Item "$debugdir\*" $makefromdir -Recurse -Exclude $exclude -Force
Copy-Item "$BarDir\说明.txt" $makefromdir -Force

cd 'C:\Program Files (x86)\NSIS'
.\makensis.exe "$BarDir\BarCheck.nsi"
$installout = "$BarDir\BarCheck_install.exe"
$installzipdir = "$BarDir\BarCheck_install"
Remove-Item "$installzipdir\*" -Force
Copy-Item "$BarDir\说明.txt" $installzipdir -Force
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

if ($error.Count > 0){
Read-Host
}

