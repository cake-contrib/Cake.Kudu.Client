using System;
using System.Net;
using System.Net.Http;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;

namespace Cake.Kudu.Client
{
    /// <inheritdoc />
    internal sealed class KuduClient : IKuduClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KuduClient"/> class.
        /// </summary>
        /// <param name="context">The Cake context.</param>
        /// <param name="settings">The provider settings.</param>
        internal KuduClient(
            ICakeContext context,
            KuduClientSettings settings)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrWhiteSpace(settings.BaseUri))
            {
                throw new ArgumentNullException(nameof(settings), "Invalid base uri specified.");
            }

            if (string.IsNullOrWhiteSpace(settings.UserName))
            {
                throw new ArgumentNullException(nameof(settings), "Invalid user name specified.");
            }

            if (string.IsNullOrWhiteSpace(settings.Password))
            {
                throw new ArgumentNullException(nameof(settings), "Invalid password specified.");
            }

            HttpClient GetDefaultHttpClient()
            {
                var credentials = new NetworkCredential(settings.UserName, settings.Password);
                var handler = new HttpClientHandler { Credentials = credentials };
                return new HttpClient(handler)
                {
                    Timeout = settings.Timeout,
                    DefaultRequestHeaders =
                    {
                        { "If-Match", "*" },
                    },
                };
            }

            HttpClient = settings.HttpClientCustomization == null
                ? GetDefaultHttpClient()
                : settings.HttpClientCustomization(settings, GetDefaultHttpClient())
                  ?? throw new NullReferenceException("KuduClientSettings HttpClientCustomization returned null.");
        }

        /// <inheritdoc />
        public IFileSystem FileSystem => Settings.FileSystem ?? Context.FileSystem;

        /// <inheritdoc />
        public ICakeLog Log => Settings.Log ?? Context.Log;

        /// <inheritdoc />
        public ICakeEnvironment Environment => Settings.Environment ?? Context.Environment;

        /// <inheritdoc />
        public KuduClientSettings Settings { get; }

        /// <inheritdoc />
        public HttpClient HttpClient { get; }

        private ICakeContext Context { get; }
    }
}