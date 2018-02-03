using System;

namespace Cake.Kudu.Client.VFS
{
    /// <inheritdoc />
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class KuduPath : IKuduPath
    {
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable SA1516 // Elements should be separated by blank line
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable InconsistentNaming
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string name { get; set; }
        public long size { get; set; }
        public DateTimeOffset mtime { get; set; }
        public DateTimeOffset crtime { get; set; }
        public string mime { get; set; }
        public string href { get; set; }
        public string path { get; set; }

        // ReSharper enable UnusedAutoPropertyAccessor.Global
        // ReSharper enable InconsistentNaming
        // ReSharper enable MemberCanBePrivate.Global
#pragma warning restore SA1516 // Elements should be separated by blank line
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore SA1300 // Element should begin with upper-case letter
#pragma warning restore SA1600 // Elements should be documented

        /// <inheritdoc />
        public string Name => name;

        /// <inheritdoc />
        public long Size => size;

        /// <inheritdoc />
        public DateTimeOffset Modified => mtime;

        /// <inheritdoc />
        public DateTimeOffset Created => crtime;

        /// <inheritdoc />
        public string Mime => mime;
    }
}