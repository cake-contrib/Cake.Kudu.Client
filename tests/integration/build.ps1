$CakeVersion = "0.35.0"
$DotNetChannel = "LTS";
$DotNetVersion = "2.1.4";
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
$FoundDotNetCliVersion = $null
[string] $DotNetCli = "dotnet"


if (Get-Command dotnet -ErrorAction SilentlyContinue) {
    $FoundDotNetCliVersion = dotnet --version
}

if($FoundDotNetCliVersion -ne $DotNetVersion)
{
    $InstallPath = Join-Path $PSScriptRoot ".dotnet"
    Remove-PathVariable "$InstallPath"
    $env:PATH = "$InstallPath;$env:PATH"

    if (!(Test-Path $InstallPath)) {
        New-Item -ItemType Directory -Force $InstallPath | Out-Null;
    }

    [string] $InstalledDotNetVersion = Get-ChildItem -Path ./.dotnet -File `
                                        | Where-Object { $_.Name -eq 'dotnet' -or $_.Name -eq 'dotnet.exe' } `
                                        | ForEach-Object { &$_.FullName --version }

    if ($InstalledDotNetVersion -eq $DotNetVersion)
    {
    }
    elseif ($IsRunningOnUnix)
    {
        $DotNetInstallerUri = "https://raw.githubusercontent.com/dotnet/cli/v2.1.4/scripts/obtain/dotnet-install.sh";
        (New-Object System.Net.WebClient).DownloadFile($DotNetInstallerUri, "$InstallPath/dotnet-install.sh");
        sudo bash "$InstallPath/dotnet-install.sh" --version $DotNetVersion --install-dir "$InstallPath" --no-path
    }
    else
    {
        $DotNetInstallerUri = "https://raw.githubusercontent.com/dotnet/cli/v2.1.4/scripts/obtain/dotnet-install.ps1";
        (New-Object System.Net.WebClient).DownloadFile($DotNetInstallerUri, "$InstallPath\dotnet-install.ps1");
        & $InstallPath\dotnet-install.ps1 -Channel $DotNetChannel -Version $DotNetVersion -InstallDir $InstallPath;
    }

    $DotNetCli = Get-ChildItem -Path ./.dotnet -File `
                                        | Where-Object { $_.Name -eq 'dotnet' -or $_.Name -eq 'dotnet.exe' } `
                                        | ForEach-Object { $_.FullName }
}

###########################################################################
# INSTALL CAKE
###########################################################################

Add-Type -AssemblyName System.IO.Compression.FileSystem
Function Unzip
{
    param([string]$zipfile, [string]$outpath)

    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}


# Make sure Cake has been installed.
$CakePath = Join-Path $ToolPath "Cake.$CakeVersion"
$CakeExePath = Join-Path $CakePath "Cake.exe"
$CakeZipPath = Join-Path $ToolPath "Cake.zip"
if (!(Test-Path $CakeExePath)) {
    Write-Host "Installing Cake $CakeVersion..."
     (New-Object System.Net.WebClient).DownloadFile("https://www.nuget.org/api/v2/package/Cake/$CakeVersion", $CakeZipPath)
     Unzip $CakeZipPath $CakePath
     Remove-Item $CakeZipPath
}

# Make sure Cake CoreCLR has been installed.
$CakeCoreCLRPath = Join-Path $ToolPath "Cake.CoreCLR.$CakeVersion"
$CakeCoreCLRDLLPath = Join-Path $CakeCoreCLRPath "Cake.dll"
$CakeCoreCLRZipPath = Join-Path $ToolPath "CakeCoreCLR.zip"
if (!(Test-Path $CakeCoreCLRDLLPath)) {
    Write-Host "Installing Cake.CoreCLR $CakeVersion..."
     (New-Object System.Net.WebClient).DownloadFile("https://www.nuget.org/api/v2/package/Cake.CoreCLR/$CakeVersion", $CakeCoreCLRZipPath)
     Unzip $CakeCoreCLRZipPath $CakeCoreCLRPath
     Remove-Item $CakeCoreCLRZipPath
}


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
if ($IsRunningOnUnix)
{
    & mono "$CakeExePath" ./setup.cake $args
}
else
{
    & "$CakeExePath" ./setup.cake $args
}

if ($LASTEXITCODE -ne 0)
{
    exit $LASTEXITCODE
}


if ($IsRunningOnUnix)
{
    Write-Host "Testing Cake.Kudu.Client on Mono..."
    & mono "$CakeExePath" ./test_net461.cake --bootstrap
    if ($LASTEXITCODE -eq 0)
    {
        & mono "$CakeExePath" ./test_net461.cake $args
    }
}
else
{
    Write-Host "Testing Cake.Kudu.Client on .NET Framework..."
    & "$CakeExePath"  ./test_net461.cake --bootstrap
    if ($LASTEXITCODE -eq 0)
    {
        & "$CakeExePath" ./test_net461.cake $args
    }
}

[int] $result = $LASTEXITCODE


Write-Host "Testing Cake.Kudu.Client on .NET Core..."
& $DotNetCli "$CakeCoreCLRDLLPath" ./test_netstandard2.0.cake $args

$result += $LASTEXITCODE;

exit $result
