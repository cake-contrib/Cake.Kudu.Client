#load nuget:https://www.myget.org/F/cake-contrib/api/v2?package=Cake.Recipe&prerelease

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./src",
                            solutionFilePath: "./src/Cake.Kudu.Client.sln",
                            title: "Cake.Kudu.Client",
                            repositoryOwner: "cake-contrib",
                            repositoryName: "Cake.Kudu.Client",
                            appVeyorAccountName: "cakecontrib",
                            shouldRunDupFinder: true,
                            shouldRunInspectCode: true);

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context,
                            dupFinderExcludePattern: new string[] {
                                BuildParameters.RootDirectoryPath + "/src/Cake.Kudu.Client/**/*.AssemblyInfo.cs",
                                BuildParameters.RootDirectoryPath + "/src/Cake.Kudu.Client/LitJson/**/*.cs" });

Build.RunDotNetCore();