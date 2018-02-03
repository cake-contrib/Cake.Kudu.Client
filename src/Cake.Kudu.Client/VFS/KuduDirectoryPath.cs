using System;
using Cake.Core.IO;

namespace Cake.Kudu.Client.VFS
{
    /// <inheritdoc />
    internal class KuduDirectoryPath : Path, IKuduDirectoryPath
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KuduDirectoryPath"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        internal KuduDirectoryPath(KuduPath path)
            : this(
                  path.Name,
                  path.Size,
                  path.Modified,
                  path.Created,
                  path.Mime,
                  path.path)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KuduDirectoryPath"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="size">The size.</param>
        /// <param name="modified">The modified date.</param>
        /// <param name="created">The creation date.</param>
        /// <param name="mime">The mime type.</param>
        /// <param name="path">The path.</param>
        internal KuduDirectoryPath(
            string name,
            long size,
            DateTimeOffset modified,
            DateTimeOffset created,
            string mime,
            string path)
            : base(path)
        {
            Name = name;
            Size = size;
            Modified = modified;
            Created = created;
            Mime = mime;
            Path = path;
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public long Size { get; }

        /// <inheritdoc />
        public DateTimeOffset Modified { get; }

        /// <inheritdoc />
        public DateTimeOffset Created { get; }

        /// <inheritdoc />
        public string Mime { get; }

        /// <inheritdoc />
        public DirectoryPath Path { get; }
    }
}