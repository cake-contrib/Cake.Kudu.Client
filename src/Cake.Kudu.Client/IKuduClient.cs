using System.Net.Http;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

// ReSharper disable UnusedMemberInSuper.Global
namespace Cake.Kudu.Client
{
    /// <summary>
    /// Represents a client that talks to Kudu rest API.
    /// </summary>
    public interface IKuduClient
    {
        /// <summary>
        /// Gets the Cake context.
        /// </summary>
        IFileSystem FileSystem { get; }

        /// <summary>
        /// Gets the Cake log.
        /// </summary>
        ICakeLog Log { get; }

        /// <summary>
        /// Gets the Cake environment.
        /// </summary>
        ICakeEnvironment Environment { get; }

        /// <summary>
        /// Gets the Kudu client settings.
        /// </summary>
        KuduClientSettings Settings { get; }

        /// <summary>
        /// Gets the underlying HttpClient used for remote API calls.
        /// </summary>
        HttpClient HttpClient { get; }
    }
}
