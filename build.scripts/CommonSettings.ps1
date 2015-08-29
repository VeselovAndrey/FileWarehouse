param([String]$solutionDirectory="")

if ([string]::IsNullOrEmpty($solutionDirectory)) {
	$base = Split-Path $script:MyInvocation.MyCommand.Path
	$solutionDirectory = $base + "\.." # assume that solution folder at 1 level up
}

# ===== Directories
$artifactsBinDirectory = $solutionDirectory + "\artifacts\bin"
$packagesDirectory = $solutionDirectory + "\packages"
$nuspecDirectory = $solutionDirectory + "\build.scripts\nuget"
$nugetExecutable = $nuspecDirectory + "\nuget"
$outputPackagesDirectory = $nuspecDirectory + "\release"