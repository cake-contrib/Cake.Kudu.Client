using System.Collections.Generic;
using Cake.Core.IO;

namespace Cake.Kudu.Client.VFS
{
    /// <summary>
    /// Repreesents a remote Kudu directory.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public interface IKuduVFS
    {
        /// <summary>
        /// Gets the current path.
        /// </summary>
        DirectoryPath Path { get; }

        /// <summary>
        /// Gets the files and directories for given remote <see cref="DirectoryPath"/>.
        /// </summary>
        ICollection<IKuduPath> Entries { get; }

        /// <summary>
        /// Gets the directories for given remote <see cref="DirectoryPath"/>.
        /// </summary>
        ICollection<IKuduDirectoryPath> Directories { get; }

        /// <summary>
        /// Gets the files for given remote <see cref="DirectoryPath"/>.
        /// </summary>
        ICollection<IKuduFilePath> Files { get; }
    }
}