using ReplConsole.Utils;

namespace ReplConsole.Commands.Handler;

/// <summary>
/// Initializes a new instance of the <see cref="HelloWorldCommandHandler"/> class.
/// </summary>
/// <remarks>
/// This constructor initializes the base <see cref="CommandHandlerBase"/> class with the provided logger and console instances.
/// </remarks>
/// <param name="logger">The <see langword="ILogger"/> instance used for logging.</param>
/// <param name="console">The <see cref="IReplConsole"/> instance used for interacting with the console.</param>
[UsedImplicitly]
public class HelloWorldCommandHandler(ILogger<HelloWorldCommandHandler> logger, IReplConsole console) : CommandHandlerBase(logger, console)
{
    public override string Name => "hello";
    public override string Description => "Prints a nice greeting.";


    /// <summary>
    /// Handles the 'hello' command.
    /// </summary>
    /// <remarks>
    /// This method takes an array of strings as arguments and a cancellation token. It writes a greeting message to the console, including the provided arguments.
    /// The greeting message is prefixed with 'Hello: '. The arguments are joined together with a comma and a space.
    /// </remarks>
    /// <param name="args">The arguments for the 'hello' command. These are included in the greeting message.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to request cancellation of the operation. This method does not actually check the cancellation token, so cancellation requests are ignored.</param>
    /// <returns>A completed <see cref="ValueTask"/>. This method always completes successfully and does not return a result.</returns>
    protected override ValueTask HandleCommand(string[] args, CancellationToken cancellationToken)
    {
        ReplConsole.WriteLine($"Hello: {string.Join(", ", args)}");
        
        Logger.LogDebug("Hello responded with greeting message.");

        return ValueTask.CompletedTask;
    }
}
