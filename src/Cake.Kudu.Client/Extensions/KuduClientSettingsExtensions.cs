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
