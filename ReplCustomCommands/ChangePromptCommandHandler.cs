using ReplConsole.Commands;
using ReplConsole.Configuration;
using ReplConsole.Utils;

namespace ReplCustomCommands;


/// <summary>
/// Initializes a new instance of the <see cref="ChangePromptCommandHandler"/> class.
/// </summary>
/// <remarks>
/// This constructor initializes a new instance of the <see cref="ChangePromptCommandHandler"/> class with a specified logger, configuration, and console.
/// </remarks>
/// <param name="logger">The <see cref="Microsoft.Extensions.Logging.ILogger"/> instance used for logging.</param>
/// <param name="config">The <see cref="IReplConsoleConfiguration"/> instance used for console configuration.</param>
/// <param name="console">The <see cref="IReplConsole"/> instance used for interacting with the console.</param>
[UsedImplicitly]
public class ChangePromptCommandHandler(ILogger<ChangePromptCommandHandler> logger, IReplConsoleConfiguration config, IReplConsole console) : CommandHandlerBase(logger, console)
{
    public override string Name        => "prompt";
    public override string Description => """
                                          Changes the prompt displayed in the console.
                                          
                                              prompt <newPrompt>
                                          
                                              newPrompt: The new prompt to be displayed.
                                          """;


    /// <summary>
    /// Handles the 'change prompt' command in the REPL console application.
    /// </summary>
    /// <remarks>
    /// This method changes the prompt of the console when the 'change prompt' command is executed. It inherits the <see cref="CommandHandlerBase.HandleCommand"/> method
    /// from the <see cref="CommandHandlerBase"/> class. The new prompt is specified as the first argument in the 'args' array.
    /// If the number of arguments is not exactly one, the method will return a Task representing a failed operation and display an error message in the console.
    /// </remarks>
    /// <param name="args">The arguments for the 'change prompt' command. This command requires exactly one argument which is the new prompt.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation of handling the 'change prompt' command. The Task's result is 'true' if the operation was successful and 'false' otherwise.</returns>
    protected override ValueTask HandleCommand(string[] args, CancellationToken cancellationToken)
    {
        if (args.Length != 1)
        {
            PrintHelp();
            return ValueTask.CompletedTask;
        }
        
        var newPrompt = args[0];
        
        config.Prompt = $"{newPrompt.Trim()} ";

        return ValueTask.CompletedTask;
    }
    
    
    private void PrintHelp()
    {
        ReplConsole.WriteError("\nInvalid number of arguments.  Please provide a prompt.\n");
    }
}
