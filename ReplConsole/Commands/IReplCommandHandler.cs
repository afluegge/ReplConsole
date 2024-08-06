namespace ReplConsole.Commands;

/// <summary>
/// Defines the contract for REPL command handlers.
/// </summary>
/// <remarks>
/// Implementations of this interface are responsible for handling specific commands in the REPL console application. 
/// Each command handler has a unique name and a description, which are used for command identification and help documentation respectively.
/// The <see cref="Handle"/> method is invoked when the corresponding command is executed in the console.
/// </remarks>
public interface IReplCommandHandler
{
    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Gets the description of the command.
    /// </summary>
    string Description { get; }
    
    /// <summary>
    /// Handles the command with the provided arguments.
    /// </summary>
    /// <param name="args">The arguments for the command.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    ValueTask Handle(string[] args, CancellationToken cancellationToken);
}
