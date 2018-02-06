using System;
using Cake.Core;
using Cake.Core.Annotations;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
namespace Cake.Kudu.Client
{
    /// <summary>
    /// Contains functionality related to remotely communicating with Azure App Services Kudu engine.
    /// </summary>
    [CakeAliasCategory("Kudu")]
    [CakeNamespaceImport("Cake.Kudu.Client.Command")]
    [CakeNamespaceImport("Cake.Kudu.Client.Extensions")]
    [CakeNamespaceImport("Cake.Kudu.Client.VFS")]
    public static class KuduClientAliases
    {
        /// <summary>
        ///  Get a Kudu client using supplied
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="baseUri">The base uri for site Kudu environment.</param>
        /// <param name="userName">The user name used to authenticate agains Kudu api.</param>
        /// <param name="password">The password used to authenticate agains Kudu api.</param>
        /// <returns>A new instance of a <see ref="IKuduClient"/>.</returns>
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
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Client")]
        public static IKuduClient KuduClient(
            this ICakeContext context,
            string baseUri,
            string userName,
            string password)
        {
           return context.KuduClient(
               new KuduClientSettings(
                   !string.IsNullOrWhiteSpace(baseUri) ? baseUri : throw new ArgumentNullException(nameof(baseUri)),
                   !string.IsNullOrWhiteSpace(userName) ? userName : throw new ArgumentNullException(nameof(userName)),
                   !string.IsNullOrWhiteSpace(password) ? password : throw new ArgumentNullException(nameof(password))));
        }

        /// <summary>
        /// Get a Kudu client using supplied <see ref="KuduClientSettings" />.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="settings">The settings.</param>
        /// <returns>A new instance of a <see ref="IKuduClient"/>.</returns>
        /// <example>
        /// <code>
        /// #addin nuget:?package=Cake.Kudu.Client
        ///
        /// string  baseUri     = EnvironmentVariable("KUDU_CLIENT_BASEURI"),
        ///         userName    = EnvironmentVariable("KUDU_CLIENT_USERNAME"),
        ///         password    = EnvironmentVariable("KUDU_CLIENT_PASSWORD");
        ///
        /// IKuduClient kuduClient = KuduClient(
        ///     new KuduClientSettings(
        ///         baseUri,
        ///        userName,
        ///        password
        ///    ){
        ///        Timeout = TimeSpan.FromSeconds(120)
        ///        });
        /// </code>
        /// </example>
        [CakeMethodAlias]
        [CakeAliasCategory("Client")]
        public static IKuduClient KuduClient(
            this ICakeContext context,
            KuduClientSettings settings)
        {
           return new KuduClient(
               context ?? throw new ArgumentNullException(nameof(context)),
               settings ?? throw new ArgumentNullException(nameof(settings)));
        }
    }
}