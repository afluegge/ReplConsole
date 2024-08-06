namespace ReplConsole.Commands;

public interface IReplCommandDispatcher
{
    /// <summary>
    /// Invokes the specified command with the provided arguments.
    /// </summary>
    /// <remarks>
    /// This method invokes the specified command with the provided arguments. If the command is 'help' or '?', it prints the help information.
    /// If the command is not recognized, it prints an unknown command message. Otherwise, it invokes the command handler associated with the command.
    /// </remarks>
    /// <param name="command">The command to be invoked.</param>
    /// <param name="args">The arguments for the command.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to propagate notification that operations should be canceled.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    ValueTask InvokeCommand(string command, string[] args, CancellationToken cancellationToken);
}
