using System;

namespace Cake.Kudu.Client.VFS
{
    /// <summary>
    /// Represents a remote path for <see cref="IKuduVFS"/>.
    /// </summary>
    public interface IKuduPath
    {
        /// <summary>
        /// Gets the name of the <see cref="IKuduVFS"/> <see cref="IKuduFilePath"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the size of the <see cref="IKuduVFS"/> <see cref="IKuduFilePath"/>.
        /// </summary>
        long Size { get; }

        /// <summary>
        /// Gets the modified date of the <see cref="IKuduVFS"/> <see cref="IKuduFilePath"/>.
        /// </summary>
        DateTimeOffset Modified { get; }

        /// <summary>
        /// Gets the creation date of the <see cref="IKuduVFS"/> <see cref="IKuduFilePath"/>.
        /// </summary>
        DateTimeOffset Created { get; }

        /// <summary>
        /// Gets the mime type of the <see cref="IKuduVFS"/> <see cref="IKuduFilePath"/>.
        /// </summary>
        string Mime { get; }
    }
}