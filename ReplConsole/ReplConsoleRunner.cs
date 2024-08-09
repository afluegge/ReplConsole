using ReplConsole.Commands;
using ReplConsole.Configuration;
using ReplConsole.Utils;

namespace ReplConsole;

/// <summary>
/// Represents a console runner for REPL (Read-Eval-Print Loop) operations.
/// </summary>
/// <remarks>
/// The <see cref="ReplConsoleRunner"/> class is a background service that continuously reads commands from the console, 
/// evaluates them, and prints the result. It uses a <see cref="ReplCommandDispatcher"/> to dispatch commands to their 
/// respective handlers. The runner supports cancellation and will stop reading commands when a cancellation request is received.
/// </remarks>
[UsedImplicitly]
[SupportedOSPlatform("windows")]
[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
[UnsupportedOSPlatform("linux")]
[UnsupportedOSPlatform("macos")]
internal partial class ReplConsoleRunner : BackgroundService
{
    [GeneratedRegex("(?<=\")[^\"]*(?=\")|[^\" ]+")]
    private static partial Regex MyRegex();
    
    private static readonly Regex _regex = MyRegex();
    
    private readonly ILogger<ReplConsoleRunner> _logger;
    private readonly IReplConsole               _console;
    private readonly IReplCommandDispatcher     _commandDispatcher;
    private readonly IReplConsoleConfiguration  _appConfig;


    /// <summary>
    /// Initializes a new instance of the <see cref="ReplConsoleRunner"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor initializes the <see cref="ReplConsoleRunner"/> with the provided parameters. It also creates a new instance of the <see cref="ReplCommandDispatcher"/> class
    /// using the provided parameters and registers the <see cref="OnStarted"/>, <see cref="OnStopping"/>, and <see cref="OnStopped"/> methods to the respective application lifetime events.
    /// </remarks>
    /// <param name="logger">The logger used for logging events in this class.</param>
    /// <param name="commandDispatcher">The command dispatcher used for dispatching commands to their respective handlers.</param>
    /// <param name="console">The console used for input and output operations.</param>
    /// <param name="appConfig">The configuration settings for the REPL console.</param>
    /// <param name="appLifetime">The application lifetime used for managing application start and stop events.</param>
    /// <param name="loggerFactory">The logger factory used for creating loggers.</param>
    /// <param name="serviceProvider">The service provider used for dependency injection.</param>
    public ReplConsoleRunner(ILogger<ReplConsoleRunner> logger, IReplCommandDispatcher commandDispatcher, IReplConsole console, IReplConsoleConfiguration appConfig, IHostApplicationLifetime appLifetime, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
    {
        _logger            = logger;
        _console           = console;
        _appConfig         = appConfig;
        _commandDispatcher = commandDispatcher;

        appLifetime.ApplicationStarted.Register(OnStarted);
        appLifetime.ApplicationStopping.Register(OnStopping);
        appLifetime.ApplicationStopped.Register(OnStopped);
    }

    /// <summary>
    /// Executes the REPL (Read-Eval-Print Loop) operations.
    /// </summary>
    /// <remarks>
    /// This method starts by printing the application name and version to the console. It then enters a loop where it continuously reads commands from the console, evaluates them,
    /// and prints the result. The loop is interrupted if the <paramref name="stoppingToken"/> signals a cancellation request.
    /// If the read command line is empty, the method continues to the next iteration. If the command line is null, the method invokes the 'exit' command and breaks the loop.
    /// For each read command line, the method tokenizes it using the <see langword="GetCommandLineTokens"/> method, extracts the command and arguments, and invokes the command
    /// using the <see cref="ReplCommandDispatcher.InvokeCommand"/> method.
    /// </remarks>
    /// <param name="stoppingToken">A <see cref="CancellationToken"/> used to propagate notification that operations should be canceled.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _console.WriteLine($"{_appConfig.AppName} Version {_appConfig.AppVersion}\n");


        while (!stoppingToken.IsCancellationRequested)
        {
            _console.Write(_appConfig.Prompt);

            var commandLine = _console.ReadLine();

            if (commandLine == string.Empty)
                continue;

            if (commandLine == null)
            {
                _console.WriteLine();
                await _commandDispatcher.InvokeCommand("exit", [], stoppingToken);
                break;
            }

            var tokens = GetCommandLineTokens(commandLine, _regex);
            var command = tokens.Length > 0 ? tokens[0] : string.Empty;
            var args = tokens.Skip(1).ToArray();

            await _commandDispatcher.InvokeCommand(command, args, stoppingToken);
        }

        return;


        static string[] GetCommandLineTokens(string commandLineString, Regex regex)
        {
            var tokens = regex.Matches(commandLineString).Select(m => m.Value).ToArray();

            return tokens;
        }
    }

    /// <summary>
    /// Handles the event that is triggered when the <see cref="ReplConsoleRunner"/> service starts.
    /// </summary>
    /// <remarks>
    /// This method is called when the <see cref="ReplConsoleRunner"/> service starts. It logs a debug message using the <see langword="ILogger"/> instance, indicating that the service has started.
    /// The name of the service, obtained using the <see langword="nameof"/> operator, is included in the log message.
    /// </remarks>
    private void OnStarted()
    {
        _logger.LogDebug("{ServiceName} started", nameof(ReplConsoleRunner));
    }

    /// <summary>
    /// Handles the stopping event of the <see cref="ReplConsoleRunner"/>.
    /// </summary>
    /// <remarks>
    /// This method is called when the <see cref="ReplConsoleRunner"/> is stopping. It logs a debug message indicating that the <see cref="ReplConsoleRunner"/> is attempting to stop.
    /// </remarks>
    private void OnStopping()
    {
        _logger.LogDebug("Attempting to stop {ServiceName}", nameof(ReplConsoleRunner));
    }

    /// <summary>
    /// Handles the event when the REPL console runner service is stopped.
    /// </summary>
    /// <remarks>
    /// This method is called when the <see cref="ReplConsoleRunner"/> service is stopped. It logs a debug message indicating that the service has stopped.
    /// </remarks>
    private void OnStopped()
    {
        _logger.LogDebug("{ServiceName} stopped", nameof(ReplConsoleRunner));
    }
}
