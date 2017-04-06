param(
  [string]$publishPath,
  [string]$publishOutputPath,
  [string]$buildConfiguration
)

# bootstrap DNVM into this session.
&{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}

# load up the global.json so we can find the DNX version
$globalJson = Get-Content -Path $PSScriptRoot\global.json -Raw -ErrorAction Ignore | ConvertFrom-Json -ErrorAction Ignore

if($globalJson)
{
    $dnxVersion = $globalJson.sdk.version
    $dnxRuntime = $globalJson.sdk.runtime
    $architecture = $globalJson.sdk.architecture
}
else
{
    Write-Warning "Unable to locate global.json to determine using 'latest' & 'clr' & 'default' architecture"
    $dnxVersion = "latest"
    $dnxRuntime = "clr"
    $architecture = "default"
}

# testing
$dnxVersion = "latest"
$dnxRuntime = "clr"
$architecture = "x86"
# end of testing

Write-Host "DNX Ver: $dnxVersion"
Write-Host "DNX Runtime: $dnxRuntime"
Write-Host "Architecture: $architecture"

# Install the runtime that's referenced
& $env:USERPROFILE\.dnx\bin\dnvm install 1.0.0-beta8
# & $env:USERPROFILE\.dnx\bin\dnvm install $dnxVersion -Persistent -r $dnxRuntime -arch $architecture
& $env:USERPROFILE\.dnx\bin\dnvm install latest -Persistent

# Ensure runtime & architecture is the active one (required if other versions exist)
# & $env:USERPROFILE\.dnx\bin\dnvm use $dnxVersion -r $dnxRuntime -arch $architecture
# & $env:USERPROFILE\.dnx\bin\dnvm use latest

# Run DNU restore on all project.json files in the src folder including 2>1 to redirect stderr to stdout for badly behaved tools
Get-ChildItem -Path $PSScriptRoot\src -Filter project.json -Recurse | ForEach-Object { & dnu restore $_.FullName 2>1 }

# Use dnu to publish application ready for deployment
dnu publish $publishPath --out $publishOutputPath --configuration $buildConfiguration --runtime "active" --wwwroot-out "wwwroot" --quiet

