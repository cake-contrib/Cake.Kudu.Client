using Cake.Core.IO;

namespace Cake.Kudu.Client.VFS
{
    /// <summary>
    /// Represents a remote directory path for <see cref="IKuduVFS"/>.
    /// </summary>
    public interface IKuduDirectoryPath : IKuduPath
    {
        /// <summary>
        /// Gets the remote directory path.
        /// </summary>
        DirectoryPath Path { get; }
    }
}