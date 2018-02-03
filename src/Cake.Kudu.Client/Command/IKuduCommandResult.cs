// ReSharper disable UnusedMemberInSuper.Global
namespace Cake.Kudu.Client.Command
{
    /// <summary>
    /// Result of Kudu command execution.
    /// </summary>
    public interface IKuduCommandResult
    {
         /// <summary>
        /// Gets the console output of the command.
        /// </summary>
        string Output { get; }

        /// <summary>
        /// Gets the error output of the command.
        /// </summary>
        string Error { get; }

        /// <summary>
        /// Gets the console exit code of the command.
        /// </summary>
        int ExitCode { get; }
    }
}