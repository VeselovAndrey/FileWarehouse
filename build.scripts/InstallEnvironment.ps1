# Prebuild script for AppVeyor
param([String]$solutionDirectory="")

# bootstrap DNVM into this session.
&{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}

# load up the global.json so we can find the DNX version
$globalJson = Get-Content -Path $solutionDirectory\global.json -Raw -ErrorAction Ignore | ConvertFrom-Json -ErrorAction Ignore

if($globalJson) {
    $dnxVersion = $globalJson.sdk.version
}
else {
    Write-Warning "Unable to locate global.json to determine using 'latest'"
    $dnxVersion = "latest"
}

# install DNX
# only installs the default (x86, clr) runtime of the framework.
# If you need additional architectures or runtimes you should add additional calls
# ex: & $env:USERPROFILE\.dnx\bin\dnvm install $dnxVersion -r coreclr
& $env:USERPROFILE\.dnx\bin\dnvm install $dnxVersion -Persistent -r clr -arch x86
& $env:USERPROFILE\.dnx\bin\dnvm install $dnxVersion -Persistent -r clr -arch x64
& $env:USERPROFILE\.dnx\bin\dnvm install $dnxVersion -Persistent -r coreclr -arch x86
& $env:USERPROFILE\.dnx\bin\dnvm install $dnxVersion -Persistent -r coreclr -arch x64

 # run DNU restore on all project.json files in the src and tests folders including 2>1 to redirect stderr to stdout for badly behaved tools
Get-ChildItem -Path $solutionDirectory\src -Filter project.json -Recurse | ForEach-Object { & dnu restore $_.FullName 2>1 }
Get-ChildItem -Path $solutionDirectory\tests -Filter project.json -Recurse | ForEach-Object { & dnu restore $_.FullName 2>1 }