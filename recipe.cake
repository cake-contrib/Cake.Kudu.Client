#load nuget:?package=Cake.Recipe&version=1.0.0
#addin nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Kudu.Client&prerelease

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./src",
                            solutionFilePath: "./src/Cake.Kudu.Client.sln",
                            title: "Cake.Kudu.Client",
                            repositoryOwner: "cake-contrib",
                            repositoryName: "Cake.Kudu.Client",
                            appVeyorAccountName: "cakecontrib",
                            shouldRunDupFinder: false,
                            shouldRunInspectCode: false,
                            shouldRunGitVersion: !Bitrise.IsRunningOnBitrise);

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context,
                            dupFinderExcludePattern: new string[] {
                                BuildParameters.RootDirectoryPath + "/src/Cake.Kudu.Client/**/*.AssemblyInfo.cs",
                                BuildParameters.RootDirectoryPath + "/src/Cake.Kudu.Client/LitJson/**/*.cs" });


((CakeTask)BuildParameters.Tasks.DotNetCoreBuildTask.Task).Actions.Clear();

BuildParameters.Tasks.DotNetCoreBuildTask.Does(() => {
        Information("Building {0}", BuildParameters.SolutionFilePath);

        var msBuildSettings = new DotNetCoreMSBuildSettings()
                                .WithProperty("Version", BuildParameters.Version.SemVersion)
                                .WithProperty("AssemblyVersion", BuildParameters.Version.Version)
                                .WithProperty("FileVersion",  BuildParameters.Version.Version)
                                .WithProperty("AssemblyInformationalVersion", BuildParameters.Version.InformationalVersion);

        if(!IsRunningOnWindows())
        {
            var frameworkPathOverride = new FilePath(typeof(object).Assembly.Location).GetDirectory().FullPath + "/";

            // Use FrameworkPathOverride when not running on Windows.
            Information("Build will use FrameworkPathOverride={0} since not building on Windows.", frameworkPathOverride);
            msBuildSettings.WithProperty("FrameworkPathOverride", frameworkPathOverride);
        }

        DotNetCoreBuild(BuildParameters.SolutionFilePath.FullPath, new DotNetCoreBuildSettings
        {
            Configuration = BuildParameters.Configuration,
            MSBuildSettings = msBuildSettings
        });

        if(BuildParameters.ShouldExecuteGitLink)
        {
            ExecuteGitLink();
        }

        CopyBuildOutput();
    });


FilePath publishedDocumentationDeploymentZipFilePath = $"{BuildParameters.Paths.Directories.PublishedDocumentation.FullPath}.zip";

Task("Docs-Generate")
    .IsDependentOn("Clean-Documentation")
    .Does(() => RequireTool(WyamTool, () =>
{
    Wyam(new WyamSettings
    {
        Recipe = BuildParameters.WyamRecipe,
        Theme = BuildParameters.WyamTheme,
        OutputPath = MakeAbsolute(BuildParameters.Paths.Directories.PublishedDocumentation),
        RootPath = BuildParameters.WyamRootDirectoryPath,
        ConfigurationFile = BuildParameters.WyamConfigurationFile,
        Settings = new Dictionary<string, object>
        {
            { "Host",  BuildParameters.WebHost },
            { "BaseEditUrl", BuildParameters.WebBaseEditUrl },
            { "SourceFiles", BuildParameters.WyamSourceFiles },
            { "Title", BuildParameters.Title },
            { "IncludeGlobalNamespace", false }
        }
    });
}));

string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
        userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
        password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");

Task("Docs-Kudu-Publish")
    .IsDependentOn("Docs-Generate")
    .WithCriteria(!string.IsNullOrEmpty(baseUri)
        && !string.IsNullOrEmpty(userName)
        && !string.IsNullOrEmpty(password)
    )
    .Does(()=>
{
    var verbosity = Context.Log.Verbosity;
    try
    {
        Context.Log.Verbosity = Verbosity.Diagnostic;

        IKuduClient kuduClient = KuduClient(
            baseUri,
            userName,
            password);

        kuduClient.ZipDeployDirectory(
            BuildParameters.Paths.Directories.PublishedDocumentation);
    }
    finally
    {
        Context.Log.Verbosity = verbosity;
    }
});

// hook in and disable Git deploy
BuildParameters.Tasks.DeployGraphDocumentation.IsDependentOn("Docs-Kudu-Publish");
BuildParameters.Tasks.DeployGraphDocumentation.WithCriteria(() => false);
BuildParameters.Tasks.PublishDocumentationTask.WithCriteria(() => false);

// disable unused tasks
BuildParameters.Tasks.InstallReportGeneratorTask.WithCriteria(() => false);
BuildParameters.Tasks.InstallOpenCoverTask.WithCriteria(() => false);
BuildParameters.Tasks.DotNetCoreTestTask.WithCriteria(() => false);

Build.RunDotNetCore();
