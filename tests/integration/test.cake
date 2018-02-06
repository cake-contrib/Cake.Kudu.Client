#addin nuget:?package=Cake.Testing&version=0.25.0
#addin nuget:?package=xunit.assert&version=2.3.1

using Cake.Testing;
using Xunit;

IKuduClient kuduClient = null;
FakeFileSystem fakeFileSystem = new FakeFileSystem(Context.Environment);
byte[] expectZipFile;

Setup(ctx =>
{
    ctx.Information("Setting up test server...");
    var server = ctx.GetKuduTestServer();

    ctx.Information("Setting up Kudu Client...");
    kuduClient = ctx.KuduClient(
        new KuduClientSettings(
           "http://cake",
           "cake",
           "build"
        )
        {
            HttpClientCustomization = (settings, client) => server.CreateClient(),
            FileSystem = fakeFileSystem
        }
    );

    ctx.Information("Setting up test data...");
    fakeFileSystem.GetFile("/testfile.txt").SetContent("dummy");
    expectZipFile = System.IO.File.ReadAllBytes(ctx.MakeAbsolute(ctx.File("./TestApiResources/ZipDownload/ZipDownload.Response")).FullPath);
    fakeFileSystem.CreateFile("/testfile.zip", expectZipFile);
    fakeFileSystem.CreateDirectory("/testfolder/");
    fakeFileSystem.GetFile("/testfolder/index.htm").SetContent("<html><body><h1>Hello!</h1></body></html>");
    fakeFileSystem.GetFile("/testfolder/about.htm").SetContent("<html><body><h1>About us!</h1></body></html>");
});

RunTestTarget("Cake.Kudu.Client");
