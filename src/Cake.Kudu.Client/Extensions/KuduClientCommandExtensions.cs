using System;
using Cake.Core;
using Cake.Core.IO;
using Cake.Kudu.Client.Command;
using Cake.Kudu.Client.Helpers;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Cake.Kudu.Client.Extensions
{
    /// <summary>
    /// Extends <see cref="IKuduClient"/> with remote CLI command execution methods.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class KuduClientCommandExtensions
    {
        /// <summary>
        /// Executes an arbitrary command line and return its output.
        /// </summary>
        /// <param name="client">The Kudu client.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="directory">The remote directory to execute command in.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns><see ref="KuduCommandResult"/></returns>
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
        /// IKuduCommandResult commandResult = kuduClient.ExecuteCommand(
        ///     "echo",
        ///     "D:/home/site/wwwroot",
        ///     "hello");
        ///
        /// Information(
        ///     "Output:\r\n{0}\r\nError:\r\n{1}\r\nExitCode: {2}",
        ///     commandResult.Output,
        ///     commandResult.Error,
        ///     commandResult.ExitCode);
        /// </code>
        /// </example>
        public static IKuduCommandResult ExecuteCommand(
            this IKuduClient client,
            string command,
            string directory,
            ProcessArgumentBuilder arguments = null)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentNullException(nameof(directory));
            }

            var commandArgs = new ProcessArgumentBuilder().AppendQuoted(command);
            arguments?.CopyTo(commandArgs);

            var param = new
            {
                command = commandArgs.Render(),
                dir = directory,
            };

            return client.HttpPostJsonObject<object, KuduCommandResult>(
                "/api/command",
                param);
        }
    }
}
