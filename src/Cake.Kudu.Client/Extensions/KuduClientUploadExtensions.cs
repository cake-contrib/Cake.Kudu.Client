using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cake.Core.IO;
using Cake.Kudu.Client.Helpers;

namespace Cake.Kudu.Client.Extensions
{
#pragma warning disable SA1600 // Elements should be documented
    internal static class KuduClientUploadExtensions
    {
        internal static void UploadStream<T>(
            this IKuduClient client,
            Stream sourceStream,
            T remotePath,
            Func<T, string> encodeRemotePathFunc, // ReSharper disable once UnusedParameter.Global
            bool allowNullStream = false)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (!allowNullStream && sourceStream == null)
            {
                throw new ArgumentNullException(nameof(sourceStream));
            }

            if (remotePath == null)
            {
                throw new ArgumentNullException(nameof(remotePath));
            }

            client.HttpPutStream(
                encodeRemotePathFunc(remotePath),
                sourceStream);
        }

        internal static void UploadFile<T>(
            this IKuduClient client,
            FilePath localPath,
            T remotePath,
            Func<T, string> encodeRemotePathFunc)
        {
            if (localPath == null)
            {
                throw new ArgumentNullException(nameof(localPath));
            }

            using (var localStream = client.FileSystem.GetFile(localPath).OpenRead())
            {
                client.UploadStream(localStream, remotePath, encodeRemotePathFunc);
            }
        }
    }
#pragma warning restore SA1600 // Elements should be documented
}
