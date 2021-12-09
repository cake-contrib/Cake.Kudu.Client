using System;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

namespace Cake.Kudu.Client
{
    /// <summary>
    /// Represents client settings used by Kudu client.
    /// </summary>
    public class KuduClientSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KuduClientSettings"/> class.
        /// </summary>
        /// <param name="baseUri">The base uri for site Kudu environment.</param>
        /// <param name="userName">The user name used to authenticate agains Kudu api.</param>
        /// <param name="password">The password used to authenticate agains Kudu api.</param>
        public KuduClientSettings(
            string baseUri,
            string userName,
            string password)
        {
            BaseUri = !string.IsNullOrWhiteSpace(baseUri) ? baseUri : throw new ArgumentNullException(nameof(baseUri));
            UserName = !string.IsNullOrWhiteSpace(userName) ? userName : throw new ArgumentNullException(nameof(userName));
            Password = !string.IsNullOrWhiteSpace(password) ? password : throw new ArgumentNullException(nameof(password));
        }

#pragma warning disable SA1629
        /// <summary>
        /// Gets the base uri for site Kudu environment.
        /// </summary>
        /// <example>
        /// https://yoursite.scm.azurewebsites.net
        /// </example>
#pragma warning restore SA1629
        public string BaseUri { get; }

        /// <summary>
        /// Gets the user name used to authenticate agains Kudu api.
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// Gets the password used to authenticate agains Kudu api.
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Gets or sets the timeout for API operations.
        /// </summary>
        /// <remarks>Default 300 seconds.</remarks>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(300);

        /// <summary>
        /// Gets or sets the delegate for customizing the HttpClient used for API options.
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Func<KuduClientSettings, System.Net.Http.HttpClient, System.Net.Http.HttpClient> HttpClientCustomization { get; set; }

        /// <summary>
        /// Gets or sets the local file system used by client.
        /// </summary>
        /// <remarks>Defaults to <see cref="ICakeContext.FileSystem"/>.</remarks>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public IFileSystem FileSystem { get; set; }

        /// <summary>
        /// Gets or sets the log used by kudu client.
        /// </summary>
        /// <remarks>Defaults to <see cref="ICakeContext.Log"/>.</remarks>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ICakeLog Log { get; set; }

        /// <summary>
        /// Gets or sets the environment used by kudu client.
        /// </summary>
        /// <remarks>Defaults to <see cref="ICakeContext.Environment"/>.</remarks>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ICakeEnvironment Environment { get; set; }
    }
}