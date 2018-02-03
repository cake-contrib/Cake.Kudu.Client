#addin nuget:?package=LitJson&version=0.12.0
#addin nuget:?package=Microsoft.AspNetCore.TestHost&version=1.1.3&loaddependencies=true
#addin nuget:?package=Microsoft.AspNetCore.Http.Extensions&version=1.1.2
#addin nuget:?package=Cake.Testing&version=0.25.0
using System.Net.Http;
using System.Threading.Tasks;
using Cake.Testing;
using LitJson;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static KuduTestServer GetKuduTestServer(this ICakeContext context) => new KuduTestServer(context);

public class KuduTestServer : IStartup
{
    public KuduTestServer(ICakeContext context)
    {
        Context = context;
        Configuration = new ConfigurationBuilder().Build();
        FakeFileSystem = new FakeFileSystem(Context.Environment);
        TestServer = new TestServer(new WebHostBuilder()
                                    .ConfigureServices(services =>
                                        services.AddSingleton<IStartup>(this)));
    }

    private FakeFileSystem FakeFileSystem { get; }

    public TestServer TestServer { get; }

    public ICakeContext Context { get; }

    public IConfigurationRoot Configuration { get; set; }

    public IFileSystem FileSystem => FakeFileSystem;

    public HttpClient CreateClient() => TestServer.CreateClient();

    public IServiceProvider ConfigureServices(IServiceCollection services) => services.BuildServiceProvider();

    public void Configure(IApplicationBuilder app)
    {
        foreach(FilePath request in Context.GetFiles("./TestApiResources/**/*.Request.json"))
        {
            byte[] kuduResponse;
            KuduRequest kuduRequest;

            var directory = request.GetDirectory();
            var response = directory.CombineWithFilePath($"{directory.GetDirectoryName()}.Response");
            using(Stream
                requestStream = Context.FileSystem.GetFile(request).OpenRead(),
                responseStream = Context.FileExists(response) ? Context.FileSystem.GetFile(response).OpenRead() : new MemoryStream())
            {
                using(StreamReader
                    requestReader = new StreamReader(requestStream, Encoding.UTF8))
                {
                    kuduRequest = LitJson.JsonMapper.ToObject<KuduRequest>(requestReader.ReadToEnd());
                }
                using(var memoryStream = new MemoryStream())
                {
                  responseStream.CopyTo(memoryStream);
                  kuduResponse = memoryStream.ToArray();
                }

                Context.Verbose("KuduTestServer Adding route: {0} ({1})",
                    kuduRequest.Uri,
                    kuduRequest.Method
                    );


                app.MapWhen(
                    context =>
                    {
                        var url = string.Concat(context.Request.GetEncodedUrl().Skip(context.Request.Scheme.Length + context.Request.Host.Value.Length + 3));
                        Context.Verbose("KuduTestServer Incoming: {0} ({1})",
                            url,
                            context.Request.Method);

                        if (!StringComparer.OrdinalIgnoreCase.Equals(url, kuduRequest.Uri)
                            || !StringComparer.OrdinalIgnoreCase.Equals(context.Request.Method, kuduRequest.Method))
                        {
                            Context.Verbose("KuduTestServer no match: {0}:{1} ({2}:{3})",
                                kuduRequest.Uri,
                                StringComparer.OrdinalIgnoreCase.Equals(url, kuduRequest.Uri),
                                kuduRequest.Method,
                                StringComparer.OrdinalIgnoreCase.Equals(context.Request.Method, kuduRequest.Method)
                            );
                            return false;
                        }
                        return true;
                    },
                    _ => _.Run(async (context) =>
                    {
                         Context.Verbose("KuduTestServer processing: {0} ({1})",
                                kuduRequest.Uri,
                                kuduRequest.Method
                            );
                        context.Response.ContentLength = kuduResponse.Length;
                        context.Response.ContentType = kuduRequest.ResponseContentType;
                        context.Response.StatusCode = kuduRequest.ResponseStatusCode ?? 200;
                        using(var ms = new MemoryStream(kuduResponse))
                        {
                            await ms.CopyToAsync(context.Response.Body);
                        }
                    }));
            }
        }
    }
}

public class KuduRequest
{
    public string Method { get; set; }
    public string Uri { get; set; }
    public string ResponseContentType { get; set; }
    public int? ResponseStatusCode { get; set; }
}
