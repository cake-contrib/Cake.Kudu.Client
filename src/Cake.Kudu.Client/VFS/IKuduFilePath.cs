using Cake.Core.IO;

namespace Cake.Kudu.Client.VFS
{
    /// <summary>
    /// Represents a remote file path for <see cref="IKuduVFS"/>.
    /// </summary>
    public interface IKuduFilePath : IKuduPath
    {
        /// <summary>
        /// Gets the remote file path.
        /// </summary>
        FilePath Path { get; }
    }
}