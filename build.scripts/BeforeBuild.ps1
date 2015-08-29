param([String]$solutionDirectory="")
$scriptDirectory = Split-Path $script:MyInvocation.MyCommand.Path

# ===== Load settings
$settingsScript = $scriptDirectory+"\CommonSettings.ps1";
. $settingsScript -solutionDirectory $solutionDirectory

# ===== Restore packages for FileWarehouse.Azure.Blob
$azureBlobProjectRoot = $solutionDirectory + "\src\FileWarehouse.Azure.Blob"
cd $azureBlobProjectRoot

&$nugetExecutable restore -PackagesDirectory $packagesDirectory