param([String]$solutionDirectory="")

# ===== Load settings
. $($(Split-Path $script:MyInvocation.MyCommand.Path) + "\Settings.ps1") -solutionDirectory $solutionDirectory

# ====== Run Azure Storage Emulator
&"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe" start
&"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe" status

$failCode = 0

# ====== Run Dnx test
foreach ($prj in $dnxTests) {
	$dnxTestReportXml = $prj + ".xml"

	cd $($solutionDirectory + "\tests\" + $prj)
	dnx test -appveyor -xml $dnxTestReportXml

	$errCode = $LASTEXITCODE
	if (($failCode -eq 0) -and ($errCode -ne 0)) { $failCode = $errCode }
	
	$wc = New-Object 'System.Net.WebClient'
	$wc.UploadFile("https://ci.appveyor.com/api/testresults/xunit/$($env:APPVEYOR_JOB_ID)", (Resolve-Path .\$dnxTestReportXml))
}

# ===== Install xUnit console runner and get path to it
&$nugetExecutable install xunit.runner.console -Prerelease -OutputDirectory $packagesDirectory
$xUnitRunner = Get-ChildItem -Path $packagesDirectory -Filter xunit.console.exe -Recurse

# ====== Run console test
foreach ($prj in $consoleTests) {	
	cd $($solutionDirectory + "\tests\" + $prj +"\bin\Release")

	$testAssembly =  ".\" + $prj + ".dll"
	$xUnitParameters = @($testAssembly, "-appveyor")

	&$xUnitRunner.FullName $xUnitParameters

	$errCode = $LASTEXITCODE
	if (($failCode -eq 0) -and ($errCode -ne 0)) { $failCode = $errCode }	
}

# ====== Stop Azure Storage Emulator

&"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe" stop
&"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe" status

exit $failCode