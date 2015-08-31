param([String]$solutionDirectory="")

# ===== Load settings
. $($(Split-Path $script:MyInvocation.MyCommand.Path) + "\Settings.ps1") -solutionDirectory $solutionDirectory

# ===== Restore packages
foreach ($prj in $netProjects) {
	cd $($solutionDirectory + $prj)

	&$nugetExecutable restore -PackagesDirectory $packagesDirectory
}