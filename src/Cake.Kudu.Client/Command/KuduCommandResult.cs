// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Cake.Kudu.Client.Command
{
    /// <summary>
    /// Result of Kudu command execution.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class KuduCommandResult : IKuduCommandResult
    {
        /// <summary>
        /// Gets or sets the console output of the command.
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        /// Gets or sets the error output of the command.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the console exit code of the command.
        /// </summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// ToString override for <see ref="KuduCommandResult"/>.
        /// </summary>
        /// <returns><see ref="KuduCommandResult"/> as string.</returns>
        public override string ToString() => $"{Output}\r\n{Error}\r\nExitCode: {ExitCode}";
    }
}