using ReplConsole.Utils;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ReplConsole.Commands;

/// <summary>
/// Provides a base class for command handlers in the REPL console application.
/// </summary>
/// <remarks>
/// This class provides a common constructor for all command handlers, which accepts an <see cref="ILogger"/> and an <see cref="IReplConsole"/> as parameters. 
/// It implements the <see cref="IReplCommandHandler"/> interface, which defines the contract for all command handlers in the application.
/// </remarks>
/// <param name="logger">The <see cref="ILogger"/> instance used for logging.</param>
/// <param name="console">The <see cref="IReplConsole"/> instance used for interacting with the console.</param>
public abstract class CommandHandlerBase(ILogger logger, IReplConsole console) : IReplCommandHandler
{
    protected readonly ILogger      Logger     = logger;
    protected readonly IReplConsole ReplConsole = console;

    public abstract string Name { get; }

    public abstract string Description { get; }


    /// <summary>
    /// Handles the command.
    /// </summary>
    /// <remarks>
    /// This is an abstract method that must be implemented by derived classes to handle specific commands. 
    /// The <paramref name="args"/> parameter represents the arguments for the command, and the <paramref name="cancellationToken"/> parameter can be used to request cancellation of the operation.
    /// </remarks>
    /// <param name="args">The arguments for the command.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    protected abstract ValueTask HandleCommand(string[] args, CancellationToken cancellationToken);


    /// <summary>
    /// Handles the command with the provided arguments.
    /// </summary>
    /// <remarks>
    /// This method logs the handling of the command with the command's name and then calls the abstract <see cref="HandleCommand"/> method with the provided arguments and cancellation token.
    /// </remarks>
    /// <param name="args">The arguments for the command.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation of handling the command.</returns>
    public ValueTask Handle(string[] args, CancellationToken cancellationToken)
    {
        Logger.LogDebug("Handle {CommandName} command", Name);

        return HandleCommand(args, cancellationToken);
    }
}
