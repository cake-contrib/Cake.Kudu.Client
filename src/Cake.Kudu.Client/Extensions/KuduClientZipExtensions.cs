using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Kudu.Client.Helpers;
using Path = Cake.Core.IO.Path;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Cake.Kudu.Client.Extensions
{
    /// <summary>
    /// Extends <see cref="IKuduClient"/> with remote zip methods.
    /// </summary>
    public static class KuduClientZipExtensions
    {
        /// <summary>
        /// Downloads remote directory to local zip file.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="remotePath">The remote source path.</param>
        /// <param name="localPath">The local target path.</param>
        /// <example>
        /// <code>
        /// #addin nuget:?package=Cake.Kudu.Client
        ///
        /// string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
        ///         userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
        ///         password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");
        ///
        /// IKuduClient kuduClient = KuduClient(
        ///     baseUri,
        ///     userName,
        ///     password);
        ///
        /// DirectoryPath remoteDirectoryPath = "/site/wwwroot/";
        /// FilePath localFilePath = "./wwwroot.zip";
        ///
        /// kuduClient.ZipDownloadFile(remoteDirectoryPath, localFilePath);
        /// </code>
        /// </example>
        public static void ZipDownloadFile(
            this IKuduClient client,
            DirectoryPath remotePath,
            FilePath localPath)
        {
            client.DownloadFile(remotePath, localPath, EncodeZipPath);
        }

        /// <summary>
        /// Downloads remote direactory as zip to stream.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="remotePath">The remote source path.</param>
        /// <returns>Content as <see cref="Stream"/></returns>
        /// <example>
        /// <code>
        /// #addin nuget:?package=Cake.Kudu.Client
        ///
        /// string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
        ///         userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
        ///         password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");
        ///
        /// IKuduClient kuduClient = KuduClient(
        ///     baseUri,
        ///     userName,
        ///     password);
        ///
        /// DirectoryPath remoteDirectoryPath = "/site/wwwroot/assets/";
        ///
        /// Stream resultStream = kuduClient.ZipDownloadStream(remoteDirectoryPath);
        ///
        /// Information("Result length: {0}", resultStream.Length);
        /// </code>
        /// </example>
        public static Stream ZipDownloadStream(
            this IKuduClient client,
            DirectoryPath remotePath)
        {
            return client.DownloadStream(remotePath, EncodeZipPath);
        }

        /// <summary>
        /// Uploads zip file to expand into remote directory path.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="localPath">The local source file path.</param>
        /// <param name="remotePath">The remote target directory path.</param>
        /// <example>
        /// <code>
        /// #addin nuget:?package=Cake.Kudu.Client
        ///
        /// string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
        ///         userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
        ///         password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");
        ///
        /// IKuduClient kuduClient = KuduClient(
        ///     baseUri,
        ///     userName,
        ///     password);
        ///
        /// DirectoryPath sourceDirectoryPath = "./Documentation/";
        /// DirectoryPath remoteDirectoryPath = "/site/wwwroot/docs/";
        /// FilePath zipFilePath = "./Documentation.zip";
        ///
        /// Zip(sourceDirectoryPath, zipFilePath);
        ///
        /// kuduClient.ZipUploadFile(
        ///    zipFilePath,
        ///    remoteDirectoryPath);
        /// </code>
        /// </example>
        public static void ZipUploadFile(
            this IKuduClient client,
            FilePath localPath,
            DirectoryPath remotePath)
        {
            client.UploadFile(localPath, remotePath, EncodeZipPath);
        }

        /// <summary>
        /// Uploads zip stream and extracts to remote directory path.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="sourceStream">The source stream.</param>
        /// <param name="remotePath">The remote directory path.</param>
        /// <example>
        /// <code>
        /// #addin nuget:?package=Cake.Kudu.Client
        ///
        /// string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
        ///         userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
        ///         password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");
        ///
        /// IKuduClient kuduClient = KuduClient(
        ///     baseUri,
        ///     userName,
        ///     password);
        ///
        /// DirectoryPath sourceDirectoryPath = "./Documentation/";
        /// DirectoryPath remoteDirectoryPath = "/site/wwwroot/docs/";
        /// FilePath zipFilePath = "./Documentation.zip";
        ///
        /// Zip(sourceDirectoryPath, zipFilePath);
        ///
        /// using(Stream sourceStream = kuduClient.FileSystem.GetFile(zipFilePath).OpenRead())
        /// {
        ///     kuduClient.ZipUploadStream(
        ///        sourceStream,
        ///        remoteDirectoryPath);
        /// }
        /// </code>
        /// </example>
        public static void ZipUploadStream(
            this IKuduClient client,
            Stream sourceStream,
            DirectoryPath remotePath)
        {
            client.UploadStream(
                sourceStream,
                remotePath,
                EncodeZipPath);
        }

        /// <summary>
        /// Uploads zip stream and extracts to remote directory path.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="localPath">The local directory path.</param>
        /// <param name="remotePath">The remote directory path.</param>
        /// <remarks>This will zip the folder in-memory.</remarks>
        /// <example>
        /// <code>
        /// #addin nuget:?package=Cake.Kudu.Client
        ///
        /// string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
        ///         userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
        ///         password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");
        ///
        /// IKuduClient kuduClient = KuduClient(
        ///     baseUri,
        ///     userName,
        ///     password);
        ///
        /// DirectoryPath sourceDirectoryPath = "./Documentation/";
        /// DirectoryPath remoteDirectoryPath = "/site/wwwroot/docs/";
        ///
        /// kuduClient.ZipUploadDirectory(
        ///     sourceDirectoryPath,
        ///     remoteDirectoryPath);
        /// </code>
        /// </example>
        public static void ZipUploadDirectory(
            this IKuduClient client,
            DirectoryPath localPath,
            DirectoryPath remotePath)
        {
            client.ZipDirectoryToMemoryStream(
                localPath,
                sourceStream =>
                    client.UploadStream(
                        sourceStream,
                        remotePath,
                        EncodeZipPath));
        }

        /// <summary>
        /// Deploys zip file to Kudu wesite.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="localPath">The local source file path.</param>
        /// <example>
        /// <code>
        /// #addin nuget:?package=Cake.Kudu.Client
        ///
        /// string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
        ///         userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
        ///         password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");
        ///
        /// IKuduClient kuduClient = KuduClient(
        ///     baseUri,
        ///     userName,
        ///     password);
        ///
        /// DirectoryPath sourceDirectoryPath = "./Documentation/";
        /// FilePath zipFilePath = "./Documentation.zip";
        ///
        /// Zip(sourceDirectoryPath, zipFilePath);
        ///
        /// kuduClient.ZipDeployFile(
        ///    zipFilePath);
        /// </code>
        /// </example>
        public static void ZipDeployFile(
            this IKuduClient client,
            FilePath localPath)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (localPath == null)
            {
                throw new ArgumentNullException(nameof(localPath));
            }

            if (!client.FileSystem.Exist(localPath))
            {
                throw new FileNotFoundException("Could not find local file to deploy", localPath.FullPath);
            }

            using (var localStream = client.FileSystem.GetFile(localPath).OpenRead())
            {
                client.ZipDeployStream(localStream);
            }
        }

        /// <summary>
        /// Deploys zip stream Kudu website.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="sourceStream">The source stream.</param>
        /// <example>
        /// <code>
        /// #addin nuget:?package=Cake.Kudu.Client
        ///
        /// string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
        ///         userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
        ///         password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");
        ///
        /// IKuduClient kuduClient = KuduClient(
        ///     baseUri,
        ///     userName,
        ///     password);
        ///
        /// DirectoryPath sourceDirectoryPath = "./Documentation/";
        /// FilePath zipFilePath = "./Documentation.zip";
        ///
        /// Zip(sourceDirectoryPath, zipFilePath);
        ///
        /// using(Stream sourceStream = kuduClient.FileSystem.GetFile(zipFilePath).OpenRead())
        /// {
        ///     kuduClient.ZipDeployStream(
        ///        sourceStream);
        /// }
        /// </code>
        /// </example>
        public static void ZipDeployStream(
            this IKuduClient client,
            Stream sourceStream)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (sourceStream == null)
            {
                throw new ArgumentNullException(nameof(sourceStream));
            }

            client.HttpPostStream(
                "/api/zipdeploy",
                sourceStream);
        }

        /// <summary>
        /// Deploy local directory to KuduWebsite
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="localPath">The local directory path.</param>
        /// <remarks>This will zip the folder in-memory.</remarks>
        /// <example>
        /// <code>
        /// #addin nuget:?package=Cake.Kudu.Client
        ///
        /// string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
        ///         userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
        ///         password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");
        ///
        /// IKuduClient kuduClient = KuduClient(
        ///     baseUri,
        ///     userName,
        ///     password);
        ///
        /// DirectoryPath sourceDirectoryPath = "./Documentation/";
        ///
        /// kuduClient.ZipDeployDirectory(
        ///     sourceDirectoryPath);
        /// </code>
        /// </example>
        public static void ZipDeployDirectory(
            this IKuduClient client,
            DirectoryPath localPath)
        {
            client.ZipDirectoryToMemoryStream(
                localPath,
                sourceStream =>
                    client.HttpPostStream(
                        "/api/zipdeploy",
                        sourceStream));
        }

        /// <summary>
        /// Deploy local directory to KuduWebsite as read only Zip file system
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="localPath">The local directory path.</param>
        /// <returns>The path of deployed Zip.</returns>
        /// <example>
        /// <code>
        /// #addin nuget:?package=Cake.Kudu.Client
        ///
        /// string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
        ///         userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
        ///         password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");
        ///
        /// IKuduClient kuduClient = KuduClient(
        ///     baseUri,
        ///     userName,
        ///     password);
        ///
        /// DirectoryPath sourceDirectoryPath = "./Documentation/";
        ///
        /// FilePath deployFilePath = kuduClient.ZipRunFromDirectory(sourceDirectoryPath);
        ///
        /// Information("Deployed to {0}", deployFilePath);
        /// </code>
        /// </example>
        public static FilePath ZipRunFromDirectory(
            this IKuduClient client,
            DirectoryPath localPath)
        {
            return client.ZipRunFromDirectory(
                skipPostDeploymentValidation: false,
                localPath: localPath);
        }

        /// <summary>
        /// Deploy local directory to KuduWebsite as read only Zip file system
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="localPath">The local directory path.</param>
        /// <param name="skipPostDeploymentValidation">Flag for if post deployment validation should be done.</param>
        /// <param name="relativeValidateUrl">The relative url used for validation (default: "KuduClientZipRunFromDirectoryVersion.txt").</param>
        /// <param name="expectedValidateValue">The expected value returned from validation url (default zip file name).</param>
        /// <returns>The path of deployed Zip.</returns>
        /// <example>
        /// <code>
        /// #addin nuget:?package=Cake.Kudu.Client
        ///
        /// string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
        ///         userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
        ///         password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");
        ///
        /// IKuduClient kuduClient = KuduClient(
        ///     baseUri,
        ///     userName,
        ///     password);
        ///
        /// DirectoryPath sourceDirectoryPath       = "./Documentation/";
        /// bool    skipPostDeploymentValidation    = false;
        /// string  expectedValidateValue           = "1.0.0.0";
        /// string  relativeValidateUrl             = $"/api/GetVersion?version={expectedValidateValue}";
        ///
        /// FilePath deployFilePath = kuduClient.ZipRunFromDirectory(
        ///                                         sourceDirectoryPath,
        ///                                         skipPostDeploymentValidation,
        ///                                         relativeValidateUrl,
        ///                                         expectedValidateValue);
        ///
        /// Information("Deployed to {0}", deployFilePath);
        /// </code>
        /// </example>
        public static FilePath ZipRunFromDirectory(
            this IKuduClient client,
            DirectoryPath localPath,
            bool skipPostDeploymentValidation,
            string relativeValidateUrl = null,
            string expectedValidateValue = null)
        {
            DirectoryPath sitePackagesPath = "d:/home/data/SitePackages";
            FilePath
                siteVersionPath = sitePackagesPath.CombineWithFilePath("siteversion.txt"),
                deployFilePath = sitePackagesPath
                                    .CombineWithFilePath(FormattableString.Invariant($"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.zip"));
            var relativeDeployFilePath = deployFilePath.GetFilename().FullPath;

            relativeValidateUrl = relativeValidateUrl ?? "KuduClientZipRunFromDirectoryVersion.txt";
            expectedValidateValue = expectedValidateValue ?? relativeDeployFilePath;

            client.ZipDirectoryToMemoryStream(
                localPath,
                sourceStream => client.VFSUploadStream(
                    sourceStream,
                    deployFilePath),
                archive =>
                {
                    var entry = archive.CreateEntry(relativeValidateUrl, CompressionLevel.Optimal);
                    using (var entryStream = entry.Open())
                    {
                        using (var sw = new StreamWriter(entryStream, Encoding.ASCII))
                        {
                            sw.Write(relativeDeployFilePath);
                        }
                    }
                });

            client.VFSUploadString(
                relativeDeployFilePath,
                siteVersionPath);

            if (skipPostDeploymentValidation)
            {
                return deployFilePath;
            }

            var commandResult = client.ExecuteCommand(
                "powershell",
                "site",
                $"-Command \"$ProgressPreference = 'SilentlyContinue';Invoke-RestMethod https://%WEBSITE_HOSTNAME%/{relativeValidateUrl};exit $LastExitCode\"");

            client.Log.Debug(
                "Output:\r\n{0}\r\nError:\r\n{1}\r\nExitCode: {2}",
                commandResult.Output,
                commandResult.Error,
                commandResult.ExitCode);

            var commandOutput = commandResult.Output?.TrimEnd();

            if (expectedValidateValue != commandOutput)
            {
                throw new Exception($"Deployment failed expected \"{expectedValidateValue}\" got \"{commandOutput}\"");
            }

            return deployFilePath;
        }

        private static void ZipDirectoryToMemoryStream(
            this IKuduClient client,
            DirectoryPath localPath,
            Action<Stream> streamAction,
            Action<ZipArchive> postZipAction = null)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (localPath == null)
            {
                throw new ArgumentNullException(nameof(localPath));
            }

            if (streamAction == null)
            {
                throw new ArgumentNullException(nameof(streamAction));
            }

            var root = client.FileSystem.GetDirectory(localPath);
            var rootPath = root.Path.MakeAbsolute(client.Environment);
            if (!root.Exists)
            {
                throw new DirectoryNotFoundException($"Specified directory not found: {rootPath}.");
            }

            var outputStream = new MemoryStream();
            var fileCount = 0;
            long inSize = 0;
            var rootPathLength = rootPath.FullPath.Length + 1;

            using (var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, true, Encoding.UTF8))
            {
                client.Log.Verbose(
                    "KuduClient: Zipping directory {0}...",
                    localPath);
                foreach (var file in root.GetFiles("*", SearchScope.Recursive))
                {
                    using (var inputStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        // Create the zip archive entry.
                        var entryName = file.Path.FullPath.Substring(rootPathLength);
                        client.Log.Debug("KuduClient: Zipping file {0} to {1}", file.Path, entryName);
                        var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
                        using (var entryStream = entry.Open())
                        {
                            // Copy the content of the input stream to the entry stream.
                            inputStream.CopyTo(entryStream);
                            fileCount++;
                            inSize += file.Length;
                        }
                    }
                }

                postZipAction?.Invoke(archive);
            }

            client.Log.Verbose(
                "KuduClient:  Done zipping directory {0} (Files: {1},  InBytes: {2}, OutBytes: {3})",
                localPath,
                fileCount,
                inSize,
                outputStream.Length);

            outputStream.Position = 0;
            streamAction(outputStream);
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static string EncodeZipPath(Path remotePath)
        {
            if (remotePath is FilePath)
            {
                throw new NotSupportedException("FilePath's not supported by Zip");
            }

            return $"/api/zip/{remotePath.FullPath.Replace(" ", " % 20").TrimStart('/')}/";
        }
    }
}
