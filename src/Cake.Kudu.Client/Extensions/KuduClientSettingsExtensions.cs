using System;
using System.Collections.Generic;
using Cake.Kudu.Client.Helpers;

// ReSharper disable UnusedMember.Global
namespace Cake.Kudu.Client.Extensions
{
    /// <summary>
    /// Extends <see cref="IKuduClient"/> with remote app service settings methods.
    /// </summary>
    public static class KuduClientSettingsExtensions
    {
        /// <summary>
        /// Get settings from appservice.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <returns>The appservice settings.</returns>
        /// <example>
        /// <code>
        /// #addin nuget:?package=Cake.Kudu.Client
        ///
        /// string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
        ///         userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
        ///         password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");
        ///
        /// IKuduClient kuduClient = KuduClient(
        ///     baseUri,
        ///     userName,
        ///     password);
        ///
        /// ReadOnlyDictionary&lt;string, string&gt; settings = kuduClient.SettingsGet();
        ///
        /// foreach(var setting in settings)
        /// {
        ///     Information(
        ///         "{0}={1}",
        ///         setting.Key,
        ///         setting.Value);
        /// }
        /// </code>
        /// </example>
        public static IReadOnlyDictionary<string, string> SettingsGet(
            this IKuduClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return client.HttpGetJsonObject<Dictionary<string, string>>(
                "/api/settings");
        }

        /// <summary>
        /// Set settings to appservice.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="settings">The settings to set.</param>
        /// <example>
        /// <code>
        /// #addin nuget:?package=Cake.Kudu.Client
        ///
        /// string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
        ///         userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
        ///         password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");
        ///
        /// IKuduClient kuduClient = KuduClient(
        ///     baseUri,
        ///     userName,
        ///     password);
        ///
        /// var newSettings = new Dictionary&lt;string, string&gt;
        ///     {
        ///         { "FOO", "Bar" },
        ///         { "JOHN", "Doe" }
        ///     };
        ///
        /// kuduClient.SettingsSet(
        ///     newSettings);
        /// </code>
        /// </example>
        public static void SettingsSet(
            this IKuduClient client,
            IReadOnlyDictionary<string, string> settings)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            client.HttpPostJsonObject<object, IReadOnlyDictionary<string, string>>(
                "/api/settings",
                settings);
        }

        /// <summary>
        /// Delete setting from appservice.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="key">The key of settings to delete.</param>
        /// <example>
        /// <code>
        /// #addin nuget:?package=Cake.Kudu.Client
        ///
        /// string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
        ///         userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
        ///         password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");
        ///
        /// IKuduClient kuduClient = KuduClient(
        ///     baseUri,
        ///     userName,
        ///     password);
        ///
        /// kuduClient.SettingsDelete("FOO");
        /// </code>
        /// </example>
        public static void SettingsDelete(
            this IKuduClient client,
            string key)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            client.HttpDelete(
                $"/api/settings/{Uri.EscapeDataString(key)}");
        }
    }
}
