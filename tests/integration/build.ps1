$IsRunningOnUnix = [System.Environment]::OSVersion.Platform -eq [System.PlatformID]::Unix

# temp addin fix
$env:CAKE_SETTINGS_SKIPVERIFICATION='true'
$env:CAKE_SETTINGS_SKIPPACKAGEVERSIONCHECK='true'

# Make sure tools folder exists
$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
$ToolPath = Join-Path $PSScriptRoot "tools"
if (!(Test-Path $ToolPath)) {
    Write-Verbose "Creating tools directory..."
    New-Item -Path $ToolPath -Type directory | out-null
}

$GlobalJsonFile = Resolve-Path (Join-Path $PSScriptRoot '../../global.json')

###########################################################################
# INSTALL .NET CORE CLI
###########################################################################

Function Remove-PathVariable([string]$VariableToRemove)
{
    $path = [Environment]::GetEnvironmentVariable("PATH", "User")
    if ($path -ne $null)
    {
        $newItems = $path.Split(';', [StringSplitOptions]::RemoveEmptyEntries) | Where-Object { "$($_)" -inotlike $VariableToRemove }
        [Environment]::SetEnvironmentVariable("PATH", [System.String]::Join(';', $newItems), "User")
    }

    $path = [Environment]::GetEnvironmentVariable("PATH", "Process")
    if ($path -ne $null)
    {
        $newItems = $path.Split(';', [StringSplitOptions]::RemoveEmptyEntries) | Where-Object { "$($_)" -inotlike $VariableToRemove }
        [Environment]::SetEnvironmentVariable("PATH", [System.String]::Join(';', $newItems), "Process")
    }
}

# Get .NET Core CLI path if installed.
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
$env:DOTNET_CLI_TELEMETRY_OPTOUT=1

[string] $DotNetCli = "dotnet"


if (Get-Command dotnet -ErrorAction SilentlyContinue) {
    $FoundDotNetCliVersion = dotnet --version
}

$InstallPath = Join-Path $PSScriptRoot ".dotnet"
Remove-PathVariable "$InstallPath"
$env:PATH = "$InstallPath;$env:PATH"

if (!(Test-Path $InstallPath))
{
    New-Item -ItemType Directory $InstallPath
}

if ($IsRunningOnUnix)
{
    $DotNetInstallerUri = "https://dot.net/v1/dotnet-install.sh";
    $DotNetInstallerScript = Join-Path $InstallPath dotnet-install.sh
    Invoke-RestMethod -Uri $DotNetInstallerUri -OutFile $DotNetInstallerScript
    sudo bash $DotNetInstallerScript --jsonfile $GlobalJsonFile --install-dir "$InstallPath" --no-path
}
else
{
    $DotNetInstallerUri = "https://dot.net/v1/dotnet-install.ps1";
    $DotNetInstallerScript = Join-Path $InstallPath dotnet-install.ps1
    Invoke-RestMethod -Uri $DotNetInstallerUri -OutFile $DotNetInstallerScript
    & $DotNetInstallerScript -JSonFile $GlobalJsonFile -InstallDir $InstallPath -NoPath;
}

$DotNetCli = Get-ChildItem -Path ./.dotnet -File `
                                    | Where-Object { $_.Name -eq 'dotnet' -or $_.Name -eq 'dotnet.exe' } `
                                    | ForEach-Object { $_.FullName }

###########################################################################
# INSTALL CAKE
###########################################################################

dotnet tool restore

###########################################################################
# RUN BUILD SCRIPT
###########################################################################
Push-Location
Write-Host "Building Cake.Kudu.Client..."
Resolve-Path "$PSScriptRoot/../../" | Set-Location
./build.ps1 $args
Pop-Location

if ($LASTEXITCODE -ne 0)
{
    exit $LASTEXITCODE;
}

###########################################################################
# RUN TEST SCRIPT
###########################################################################
Write-Host "Codegen test dependencies..."

dotnet cake setup.cake $args

if ($LASTEXITCODE -ne 0)
{
    exit $LASTEXITCODE
}

[int] $result = 0

@(
    'netcoreapp3.1',
    'net5.0',
    'net6.0'
) | ForEach-Object {
    Write-Host "Testing Cake.Kudu.Client on $($_)..."
    dotnet cake "test_$($_).cake" $args

    $result += $LASTEXITCODE;
}
exit $result
