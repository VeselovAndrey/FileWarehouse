param([String]$solutionDirectory="")

# ===== Load settings
. $($(Split-Path $script:MyInvocation.MyCommand.Path) + "\CommonSettings.ps1") -solutionDirectory $solutionDirectory

# ====== Run Azure Storage Emulator
&"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe" start
&"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe" status


# ====== FileWarehouse.Azure.Blob.Dnx.Tests

cd $solutionDirectory
cd tests\FileWarehouse.Azure.Blob.Dnx.Tests
dnx . test -appveyor -xml FileWarehouse.Azure.Blob.Dnx.Tests.xml

$wc2 = New-Object 'System.Net.WebClient'
$wc2.UploadFile("https://ci.appveyor.com/api/testresults/xunit/$($env:APPVEYOR_JOB_ID)", (Resolve-Path .\FileWarehouse.Azure.Blob.Dnx.Tests.xml))
cd ..\..


# ====== FileWarehouse.FileSystem.Dnx.Tests 

cd tests\FileWarehouse.FileSystem.Dnx.Tests 
dnx . test -appveyor -xml FileWarehouse.FileSystem.Dnx.Tests.xml

$wc1 = New-Object 'System.Net.WebClient'
$wc1.UploadFile("https://ci.appveyor.com/api/testresults/xunit/$($env:APPVEYOR_JOB_ID)", (Resolve-Path .\FileWarehouse.FileSystem.Dnx.Tests.xml))
cd ..\..

# ====== Stop Azure Storage Emulator

&"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe" stop
&"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe" status