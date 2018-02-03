using System;
using System.IO;
using Cake.Core.IO;
using Cake.Kudu.Client.Helpers;
using Path = Cake.Core.IO.Path;

namespace Cake.Kudu.Client.Extensions
{
#pragma warning disable SA1600 // Elements should be documented
    internal static class KuduClientDownloadExtenstions
    {
        internal static void DownloadFile<T>(
            this IKuduClient client,
            T remotePath,
            FilePath localPath,
            Func<T, string> encodeRemotePathFunc)
            where T : Path
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (remotePath == null)
            {
                throw new ArgumentNullException(nameof(remotePath));
            }

            if (localPath == null)
            {
                throw new ArgumentNullException(nameof(localPath));
            }

            if (encodeRemotePathFunc == null)
            {
                throw new ArgumentNullException(nameof(encodeRemotePathFunc));
            }

            using (var localStream = client.FileSystem.GetFile(localPath).OpenWrite())
            {
                client.HttpGetToStream(
                    encodeRemotePathFunc(remotePath),
                    localStream);
            }
        }

        internal static Stream DownloadStream<T>(
            this IKuduClient client,
            T remotePath,
            Func<T, string> encodeRemotePathFunc)
            where T : Path
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (remotePath == null)
            {
                throw new ArgumentNullException(nameof(remotePath));
            }

            if (encodeRemotePathFunc == null)
            {
                throw new ArgumentNullException(nameof(encodeRemotePathFunc));
            }

            return client.HttpGetStream(
                encodeRemotePathFunc(remotePath));
        }
    }
#pragma warning restore SA1600 // Elements should be documented
}
