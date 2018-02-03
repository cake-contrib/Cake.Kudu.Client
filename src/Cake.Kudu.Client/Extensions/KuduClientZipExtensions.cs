using System;
using System.IO;
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
        // ReSharper disable once InconsistentNaming
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
        // ReSharper disable once InconsistentNaming
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
        /// Deploys zip file to Kudu wesite.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="localPath">The local source file path.</param>
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
