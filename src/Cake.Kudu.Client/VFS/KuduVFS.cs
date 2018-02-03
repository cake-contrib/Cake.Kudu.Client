using System.Collections.Generic;
using System.Linq;
using Cake.Core.IO;

namespace Cake.Kudu.Client.VFS
{
    /// <inheritdoc />
    // ReSharper disable once InconsistentNaming
    internal class KuduVFS : IKuduVFS
    {
        private KuduVFS(
            DirectoryPath path,
            ICollection<IKuduPath> entries,
            ICollection<IKuduDirectoryPath> directories,
            ICollection<IKuduFilePath> files)
        {
            Path = path;
            Entries = entries;
            Directories = directories;
            Files = files;
        }

        /// <inheritdoc />
        public DirectoryPath Path { get; }

        /// <inheritdoc />
        public ICollection<IKuduPath> Entries { get; }

        /// <inheritdoc />
        public ICollection<IKuduDirectoryPath> Directories { get; }

        /// <inheritdoc />
        public ICollection<IKuduFilePath> Files { get; }

        /// <summary>
        /// Converts a collection of <see cref="KuduPath"/> paths to <see cref="IKuduVFS"/>.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="paths">The paths.</param>
        /// <returns>A <see cref="IKuduVFS"/>.</returns>
        internal static IKuduVFS ToKuduVfs(DirectoryPath path, ICollection<KuduPath> paths)
        {
            return new KuduVFS(
                path,
                paths.Cast<IKuduPath>().ToArray(),
                paths.Where(IsDirectory).Select(ToDirectory).ToArray(),
                paths.Where(IsFile).Select(ToFile).ToArray());
        }

        private static IKuduDirectoryPath ToDirectory(KuduPath path)
        {
            return new KuduDirectoryPath(path);
        }

        private static IKuduFilePath ToFile(KuduPath path)
        {
            return new KuduFilePath(path);
        }

        private static bool IsFile(KuduPath path)
        {
            return path?.href.EndsWith("/") == false;
        }

        private static bool IsDirectory(KuduPath path)
        {
            return path?.href.EndsWith("/") == true;
        }
    }
}