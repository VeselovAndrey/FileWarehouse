param([String]$solutionDirectory="")

# ===== Load settings
. $($(Split-Path $script:MyInvocation.MyCommand.Path) + "\Settings.ps1") -solutionDirectory $solutionDirectory

# ===== Restore packages
foreach ($prj in $netProjects) {
	$projectDirectory = $solutionDirectory + $prj
	$("=====> Restoring packages for " + $projectDirectory)

	$projectPackageConfig = $projectDirectory + "\packages.config"
	if (Test-Path $projectPackageConfig)
	{
		cd $projectDirectory
		&$nugetExecutable restore -PackagesDirectory $packagesDirectory
	}
}