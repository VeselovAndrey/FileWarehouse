param([String]$solutionDirectory="")
$scriptDirectory = Split-Path $script:MyInvocation.MyCommand.Path

# ===== Load settings
$settingsScript = $scriptDirectory+"\CommonSettings.ps1";
. $settingsScript -solutionDirectory $solutionDirectory

# ===== Clean release folder
if (test-path $outputPackagesDirectory) {	
    Remove-Item $outputPackagesDirectory -Recurse
}
New-Item $outputPackagesDirectory -Type Directory *>$null

# ====== Build NuGet packages
$allNugetFiles = Get-ChildItem $nugetDirectory -Recurse
$nuspecFiles = $allNugetFiles | where {$_.Name.EndsWith(".nuspec") }

foreach ($nuspecFile in $nuspecFiles) {
    &$nugetExecutable pack $nuspecFile.FullName -Symbols -OutputDirectory $outputPackagesDirectory
}