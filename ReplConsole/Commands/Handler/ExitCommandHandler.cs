using ReplConsole.Configuration;
using ReplConsole.Utils;

namespace ReplConsole.Commands.Handler;

/// <summary>
/// Initializes a new instance of the <see cref="ExitCommandHandler"/> class.
/// </summary>
/// <remarks>
/// The <see cref="ExitCommandHandler"/> class is responsible for handling the 'exit' command in the REPL console application. 
/// It inherits from the <see cref="CommandHandlerBase"/> class and overrides the <see cref="CommandHandlerBase.HandleCommand"/> method to implement the command execution logic. 
/// The 'exit' command causes the application to exit.
/// </remarks>
/// <param name="logger">The <see langword="ILogger"/> instance used for logging.</param>
/// <param name="console">The <see cref="IReplConsole"/> instance used for interacting with the console.</param>
/// <param name="config">The <see cref="IReplConsoleConfiguration"/> instance used for accessing configuration settings.</param>
[UsedImplicitly]
public class ExitCommandHandler(ILogger<ExitCommandHandler> logger, IReplConsole console, IReplConsoleConfiguration config) : CommandHandlerBase(logger, console)
{
    public override string Name => "exit";
    public override string Description => $"Exits {config.AppName}.";


    /// <summary>
    /// Handles the 'exit' command.
    /// </summary>
    /// <remarks>
    /// This method is responsible for handling the 'exit' command. When invoked, it writes a farewell message to the console using the <see langword="ReplConsole.WriteLine"/> method,
    /// then terminates the application using the <see cref="Environment.Exit"/> method. Finally, it returns a completed task.
    /// </remarks>
    /// <param name="args">The arguments for the 'exit' command. This parameter is not used in this method.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete. This parameter is not used in this method.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation of handling the 'exit' command.</returns>
    protected override ValueTask HandleCommand(string[] args, CancellationToken cancellationToken)
    {
        ReplConsole.WriteLine("Bye...");
        Environment.Exit(0);
        return ValueTask.CompletedTask;
    }
}
