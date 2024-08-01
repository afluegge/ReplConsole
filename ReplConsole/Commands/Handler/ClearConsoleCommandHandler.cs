using ReplConsole.Utils;

namespace ReplConsole.Commands.Handler;

/// <summary>
/// Initializes a new instance of the <see cref="ClearConsoleCommandHandler"/> class.
/// </summary>
/// <remarks>
/// The <see cref="ClearConsoleCommandHandler"/> class is responsible for handling the 'cls' command in the REPL console application. 
/// This command clears the console screen. The class inherits from the <see cref="CommandHandlerBase"/> class.
/// </remarks>
/// <param name="logger">The <see langword="ILogger"/> instance used for logging.</param>
/// <param name="console">The <see cref="IReplConsole"/> instance used for interacting with the console.</param>
[UsedImplicitly]
public class ClearConsoleCommandHandler(ILogger<ClearConsoleCommandHandler> logger, IReplConsole console) : CommandHandlerBase(logger, console)
{
    public override string Name => "cls";
    public override string Description => "Clears the Console Screen.";


    /// <summary>
    /// Handles the 'cls' command in the REPL console application.
    /// </summary>
    /// <remarks>
    /// This method clears the console screen when the 'cls' command is executed. It inherits the <see cref="CommandHandlerBase.HandleCommand"/> method from the <see cref="CommandHandlerBase"/> class.
    /// </remarks>
    /// <param name="args">The arguments for the 'cls' command. This command does not require any arguments.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation of handling the 'cls' command.</returns>
    protected override ValueTask HandleCommand(string[] args, CancellationToken cancellationToken)
    {
        ReplConsole.Clear();

        return ValueTask.CompletedTask;
    }
}
