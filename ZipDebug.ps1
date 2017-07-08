function ZipFiles( $zipfilename, $sourcedir )
{
   Add-Type -Assembly System.IO.Compression.FileSystem
   $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
   [System.IO.Compression.ZipFile]::CreateFromDirectory($sourcedir,$zipfilename)
}
$zipdir ='C:\G\BarCheck\Releases\BarCheck' + [DateTime]::Now.ToString("yyyyMMddHHmm") 
$zipdir 
$debugdir = 'C:\G\BarCheck\BarCheck\BarCheck\bin\Release'
New-Item -ItemType Directory -Force -Path $zipdir
$exclude = @('*.pdb','*.xml','*vshost*','*.log')
Copy-Item "$debugdir\*" $zipdir -Recurse -Exclude $exclude -Force
Copy-Item 'C:\G\BarCheck\说明.txt' $zipdir -Force
$zipfile = "$zipdir.zip"
$zipfile
$zipdir
ZipFiles  $zipfile  $zipdir
#Compress-Archive -Path $zipdir -DestinationPath $zipfile
#Get-ChildItem $source -Recurse -Exclude $exclude | Copy-Item -Destination {Join-Path $dest $_.FullName.Substring($source.length)}
