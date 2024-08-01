using ReplConsole.Configuration;
using ReplConsole.Utils;

namespace ReplConsole.Commands;

/// <summary>
/// Manages the dispatching of commands in a REPL (Read-Evaluate-Print Loop) console application.
/// </summary>
internal class ReplCommandDispatcher
{
    private readonly ILogger<ReplCommandDispatcher>          _logger;
    private readonly IReplConsole                            _console;
    private readonly IServiceProvider                        _serviceProvider;
    private readonly Dictionary<string, IReplCommandHandler> _commands;
    private readonly string                                  _helpHeader;


    /// <summary>
    /// Initializes a new instance of the <see cref="ReplCommandDispatcher"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor initializes the logger, console, and service provider fields with the provided arguments. 
    /// It also initializes the commands dictionary and sets the help header based on the application name from the configuration.
    /// After the initialization, it registers command handlers from the assembly.
    /// </remarks>
    /// <param name="logger">The logger used by the dispatcher.</param>
    /// <param name="config">The configuration settings for the REPL console application.</param>
    /// <param name="console">The console interface for interacting with the console.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    public ReplCommandDispatcher(ILogger<ReplCommandDispatcher> logger, IReplConsoleConfiguration config, IReplConsole console, IServiceProvider serviceProvider)
    {
        _logger          = logger;
        _console         = console;
        _serviceProvider = serviceProvider;
        _commands        = [];

        _helpHeader = $"\n{config.AppName} - Command Line Help\n{new string('-', (config.AppName + " - Command Line Help").Length)}\n";

        RegisterCommandHandler();
    }


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
    public async ValueTask InvokeCommand(string command, string[] args, CancellationToken cancellationToken)
    {
        if (command is "help" or "?")
        {
            PrintHelp();
            return;
        }

        if (!_commands.TryGetValue(command, out var commandHandler))
        {
            PrintUnknownCommand(command);
            return;
        }

        await commandHandler.Handle(args, cancellationToken);
    }


    /// <summary>
    /// Registers command handlers in the REPL console application.
    /// </summary>
    /// <remarks>
    /// This method retrieves all implementations of the <see cref="IReplCommandHandler"/> interface from the service provider. 
    /// For each handler, it adds an entry to the '_commands' dictionary with the handler's name as the key and the handler instance as the value. 
    /// It also logs a debug message indicating the type of the handler and the command name for which it has been registered.
    /// </remarks>
    private void RegisterCommandHandler()
    {
        var handlers = _serviceProvider.GetServices<IReplCommandHandler>();

        foreach (var handler in handlers)
        {
            _commands[handler.Name] = handler;
            _logger.LogDebug("Handler '{CommandHandlerType}' for Command '{CommandName}' registered", handler.GetType().FullName, handler.Name);
        }
    }

    /// <summary>
    /// Prints the help information for all registered commands.
    /// </summary>
    /// <remarks>
    /// This method prints the help information for all registered commands in the REPL console application. 
    /// The help information includes the name and description of each command. 
    /// The method iterates over the '_commands' dictionary and writes the name and description of each command to the console.
    /// </remarks>
    private void PrintHelp()
    {
        _console.WriteLine(_helpHeader);

        foreach (var command in _commands.Values)
        {
            _console.WriteLine(command.Name);
            _console.WriteLine($"  {command.Description}\n");
        }
    }

    /// <summary>
    /// Prints an error message for an unknown command.
    /// </summary>
    /// <remarks>
    /// This method is used to inform the user when they have entered a command that the system does not recognize. 
    /// It does this by writing an error message to the console using the <see langword="IReplConsole.Write"/> and <see langword="IReplConsole.WriteLine"/> methods of the <see cref="IReplConsole"/> interface.
    /// The error message is colored for better visibility, with the command itself highlighted in a different color.
    /// </remarks>
    /// <param name="command">The command that was not recognized.</param>
    private void PrintUnknownCommand(string command)
    {
        _console.Write("\nError: Unknown Command '", Color.Firebrick);
        _console.Write(command, Color.MediumSlateBlue);
        _console.WriteLine("'\n", Color.Firebrick);
    }
}
