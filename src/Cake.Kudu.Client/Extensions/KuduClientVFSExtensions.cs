using System;
using System.IO;
using System.Text;
using Cake.Core.IO;
using Cake.Kudu.Client.Helpers;
using Cake.Kudu.Client.VFS;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Cake.Kudu.Client.Extensions
{
    /// <summary>
    /// Extends <see cref="IKuduClient"/> with virtual file system methods.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class KuduClientVFSExtensions
    {
        /// <summary>
        /// Lists remote resources for a given path.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="remotePath">The remote directory path.</param>
        /// <returns><see cref="IKuduVFS"/> instance containing remote directory and file paths.</returns>
        // ReSharper disable once InconsistentNaming
        public static IKuduVFS VFSList(
            this IKuduClient client,
            DirectoryPath remotePath)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (remotePath == null)
            {
                throw new ArgumentNullException(nameof(remotePath));
            }

            var paths = client.HttpGetJsonObject<KuduPath[]>(
                EncodeVFSPath(remotePath));

            return KuduVFS.ToKuduVfs(remotePath, paths);
        }

        /// <summary>
        /// Downloads remote file locally.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="remotePath">The remote source path.</param>
        /// <param name="localPath">The local target path.</param>
        // ReSharper disable once InconsistentNaming
        public static void VFSDownloadFile(
            this IKuduClient client,
            FilePath remotePath,
            FilePath localPath)
        {
            client.DownloadFile(remotePath, localPath, EncodeVFSPath);
        }

        /// <summary>
        /// Downloads remote file to stream
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="remotePath">The remote source path.</param>
        /// <returns>Content as <see cref="Stream"/></returns>
        // ReSharper disable once InconsistentNaming
        public static Stream VFSDownloadStream(
            this IKuduClient client,
            FilePath remotePath)
        {
            return client.DownloadStream(remotePath, EncodeVFSPath);
        }

        /// <summary>
        /// Downloads remote file as string.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="remotePath">The remote source path.</param>
        /// <param name="encoding">The text encoding.</param>
        /// <returns>Content as string.</returns>
        // ReSharper disable once InconsistentNaming
        public static string VFSDownloadString(
            this IKuduClient client,
            FilePath remotePath,
            Encoding encoding = null)
        {
            using (var reader = new StreamReader(client.VFSDownloadStream(remotePath), encoding ?? Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Uploads file to remote path.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="localPath">The local source file path.</param>
        /// <param name="remotePath">The remote target file path.</param>
        // ReSharper disable once InconsistentNaming
        public static void VFSUploadFile(
            this IKuduClient client,
            FilePath localPath,
            FilePath remotePath)
        {
            client.UploadFile(localPath, remotePath, EncodeVFSPath);
        }

        /// <summary>
        /// Uploads stream to remote path.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="sourceStream">The source stream.</param>
        /// <param name="remotePath">The remote target file path.</param>
        // ReSharper disable once InconsistentNaming
        public static void VFSUploadStream(
            this IKuduClient client,
            Stream sourceStream,
            FilePath remotePath)
        {
            client.UploadStream(sourceStream, remotePath, EncodeVFSPath);
        }

        /// <summary>
        /// Uploads stream to remote path.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="sourceString">The source string.</param>
        /// <param name="remotePath">The remote target file path.</param>
        /// <param name="encoding">The text encoding.</param>
        // ReSharper disable once InconsistentNaming
        public static void VFSUploadString(
            this IKuduClient client,
            string sourceString,
            FilePath remotePath,
            Encoding encoding = null)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (sourceString == null)
            {
                throw new ArgumentNullException(nameof(sourceString));
            }

            if (remotePath == null)
            {
                throw new ArgumentNullException(nameof(remotePath));
            }

            client.HttpPutString(
                EncodeVFSPath(remotePath),
                sourceString,
                encoding);
        }

        /// <summary>
        /// Creates directory on remote path.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="remotePath">The remote target path.</param>
        // ReSharper disable once InconsistentNaming
        public static void VFSCreateDirectory(
            this IKuduClient client,
            DirectoryPath remotePath)
        {
            client.UploadStream(null, remotePath, EncodeVFSPath, true);
        }

        /// <summary>
        /// Deletes remote file.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="remotePath">The remote source path.</param>
        // ReSharper disable once InconsistentNaming
        public static void VFSDelete(
            this IKuduClient client,
            FilePath remotePath)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (remotePath == null)
            {
                throw new ArgumentNullException(nameof(remotePath));
            }

            client.HttpDelete(EncodeVFSPath(remotePath));
        }

        // ReSharper disable once InconsistentNaming
        private static string EncodeVFSPath(Core.IO.Path remotePath)
        {
            return string.Concat(
                "/api/vfs/",
                remotePath.FullPath.Replace(" ", "%20").TrimStart('/'),
                remotePath is DirectoryPath ? "/" : string.Empty);
        }
    }
}