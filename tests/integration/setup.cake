var targets = new [] {
    "netcoreapp3.1",
    "net5.0",
    "net6.0"
};

DirectoryPath rootPath = MakeAbsolute(Directory("./"));
var postScripts = new FilePath[]{ File("test.cake") };
var excludeFiles = GetFiles("./*.cake");
var scripts = (GetFiles("./**/*.cake") - excludeFiles)
                .Cast<FilePath>()
                .Select(file=>rootPath.GetRelativePath(file))
                .ToList();


foreach(var target in targets)
{
    string targetFile = $"test_{target}.cake";
    string dependencies = string.Join(
        "\r\n",
        new [] {
                 $"#r \"../../BuildArtifacts/temp/_PublishedLibraries/Cake.Kudu.Client/{target}/Cake.Kudu.Client.dll\""
        }.Concat(
            scripts
                .Concat(postScripts)
                .Select(file => $"#load \"{file}\"")
            )
    );

    System.IO.File.WriteAllText(
        targetFile,
        dependencies
        );
}