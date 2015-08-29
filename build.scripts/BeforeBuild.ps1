param([String]$solutionDirectory="")

# ===== Load settings
. $($(Split-Path $script:MyInvocation.MyCommand.Path) + "\CommonSettings.ps1") -solutionDirectory $solutionDirectory


# ===== Restore packages for FileWarehouse.Azure.Blob
$azureBlobProjectRoot = $solutionDirectory + "\src\FileWarehouse.Azure.Blob"
cd $azureBlobProjectRoot

&$nugetExecutable restore -PackagesDirectory $packagesDirectory