<#
This example demonstrates a software build process using the 'ZeroFailed.Build.DotNet' extension
to provide the features needed when building a .NET solutions.
#>

$zerofailedExtensions = @(
    @{
        # References the extension from its GitHub repository. If not already installed, use latest version from 'main' will be downloaded.
        Name = "ZeroFailed.Build.DotNet"
        GitRepository = "https://github.com/zerofailed/ZeroFailed.Build.DotNet"
        GitRef = "main"
    }
)

# Load the tasks and process
. ZeroFailed.tasks -ZfPath $here/.zf


#
# Build process control options
#
$SkipInit = $false
$SkipVersion = $false
$SkipBuild = $false
$CleanBuild = $Clean
$SkipTest = $false
$SkipTestReport = $false
$SkipAnalysis = $false
$SkipPackage = $false

#
# Build process configuration
#
$SolutionToBuild = (Resolve-Path (Join-Path $here "Solutions/Marain.TenantManagement.sln")).Path
$ProjectsToPublish = @(
    "Solutions/Marain.TenantManagement.Cli/Marain.TenantManagement.Cli.csproj"
)
$NuSpecFilesToPackage = @()
$NugetPublishSource = property ZF_NUGET_PUBLISH_SOURCE "$here/_local-nuget-feed"
$IncludeAssembliesInCodeCoverage = "Marain*"
$ExcludeAssembliesInCodeCoverage = ""


# Customise the build process

task . FullBuild


#
# Build Process Extensibility Points - uncomment and implement as required
#

# task RunFirst {}
# task PreInit {}
# task PostInit {}
# task PreVersion {}
# task PostVersion {}
# task PreBuild {}
# task PostBuild {}
# task PreTest {}
# task PostTest {}
# task PreTestReport {}
# task PostTestReport {}
# task PreAnalysis {}
# task PostAnalysis {}
# task PrePackage {}
# task PostPackage {}
# task PrePublish {}
# task PostPublish {}
# task RunLast {}

$DotNetTestFileLoggerProps = "`"/flp:verbosity=$DotNetFileLoggerVerbosity;logfile=$DotNetTestLogFile`""
task RunTestsWithDotNetCoverage -If {$SolutionToBuild} {
    # Setup the appropriate CI/CD platform test logger, unless explicitly disabled
    if (!$DisableCicdServerLogger) {
        if ($script:IsAzureDevOps) {
            Write-Build Green "Configuring Azure Pipelines test logger"
            $script:DotNetTestLoggers += "AzurePipelines"
        }
        elseif ($script:IsGitHubActions) {
            Write-Build Green "Configuring GitHub Actions test logger"
            $script:DotNetTestLoggers += "GitHubActions"
        }    
    }

    # Evaluate the file logger properties so we can pass them to 'dotnet test'
    $_fileLoggerProps = Resolve-Value $DotNetTestFileLoggerProps

    # Use InvokeBuild's built-in $Task variable to know where this file is installed and use it to 
    # derive where the root of the module must be.  This method will work when this module has
    # been directly imported as well as when it is used as a ZeroFailed extension.
    $moduleDir = Split-Path -Parent (Split-Path -Parent $Task.InvocationInfo.ScriptName)
    Write-Verbose "ModuleDir: $moduleDir"

    # Setup the arguments we need to pass to 'dotnet test'
    $dotnetTestArgs = @(
        "--configuration", $Configuration
        "--no-build"
        "--no-restore"
        "--verbosity", $LogLevel
        "--test-adapter-path", (Join-Path $moduleDir "bin")
        ($_fileLoggerProps ? $_fileLoggerProps : "/fl")
    )

    # Configure any test loggers that have been specified
    $DotNetTestLoggers | ForEach-Object {
        $dotnetTestArgs += @("--logger", "`"$_`"")
    }

    $coverageOutput = "coverage{0}.cobertura.xml" -f ($TargetFrameworkMoniker ? ".$TargetFrameworkMoniker" : "")
    if ($TargetFrameworkMoniker) {
        $dotnetTestArgs += @("--framework", $TargetFrameworkMoniker)
    }
    Remove-Item $coverageOutput -ErrorAction Ignore -Force
    $dotnetCoverageArgs = @(
        "collect"
        "-o", $coverageOutput
        "-f", "cobertura"
    )

    # Ensure the dotnet-coverage global tool is installed, as we need it to collect the code coverage data
    Install-DotNetTool -Name "dotnet-coverage" -Global

    # Add any custom test arguments that have been specified
    if ($AdditionalTestArgs) {
        $dotnetTestArgs += $AdditionalTestArgs
    }

    $dotnetCoverageArgs += @(
        "dotnet"
        "test"
    )
    Write-Verbose "CmdLine: $dotnetCoverageArgs $SolutionToBuild $dotnetTestArgs" -Verbose
    try {
        exec { 
            & dotnet-coverage @dotnetCoverageArgs $SolutionToBuild @dotnetTestArgs
        }

        # Only generate code coverage reports if the tests passed
        if (!$SkipTestReport -and (Test-Path $coverageOutput)) {
            if ($GenerateTestReport) {
                Write-Build White "Generating additional test reports: $TestReportTypes"
                _GenerateTestReport `
                    -ReportTypes $TestReportTypes `
                    -OutputPath $CoverageDir `
                    -IncludeAssemblyFilter $IncludeAssembliesInCodeCoverage `
                    -ExcludeAssemblyFilter $ExcludeAssembliesInCodeCoverage
            }
            if ($GenerateMarkdownCodeCoverageSummary) {
                Write-Build White "Generating Markdown code coverage summary"
                _GenerateCodeCoverageMarkdownReport `
                    -UseGitHubFlavour $IsGitHubActions `
                    -TargetFrameworkMoniker $TargetFrameworkMoniker `
                    -OutputPath $CoverageDir `
                    -IncludeAssemblyFilter $IncludeAssembliesInCodeCoverage `
                    -ExcludeAssemblyFilter $ExcludeAssembliesInCodeCoverage
            }
        }
    }
    finally {
        if ((Test-Path $DotNetTestLogFile) -and $IsAzureDevOps) {
            Write-Host "##vso[artifact.upload artifactname=logs]$((Resolve-Path $DotNetTestLogFile).Path)"
        }
    }
}
