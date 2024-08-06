using ReplConsole.Commands;
using ReplConsole.Configuration;
using ReplConsole.Utils;
using ILogger = Serilog.ILogger;

namespace ReplConsole;

/// <summary>
/// The <c>Program</c> class.
/// </summary>
/// <remarks>
/// This is an internal class that serves as the entry point for the application.
/// </remarks>
[SupportedOSPlatform("windows")]
[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
[UnsupportedOSPlatform("linux")]
[UnsupportedOSPlatform("macos")]
[UsedImplicitly]
internal class Program
{
    private const string EnvPrefix = "REPLCLI_";

    private static readonly IConfiguration _configuration = new ConfigurationBuilder()
                                                                .SetBasePath(Directory.GetCurrentDirectory())
                                                                .AddJsonFile("appsettings.json", optional: false)
                                                                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                                                                .Build();

    private static readonly ILogger _bootstrapLogger = CreateBootstrapLogger();


    /// <summary>
    /// Initializes a new instance of the <see cref="Program"/> class.
    /// </summary>
    /// <remarks>
    /// This is a protected constructor that is used to prevent the creation of the <see cref="Program"/> class.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    protected Program()
    {
    }


    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    /// <remarks>
    /// This method is responsible for starting the ReplConsole application. It configures the host, sets up the console, and runs the host asynchronously.
    /// </remarks>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [ExcludeFromCodeCoverage]
    internal static async Task Main(string[] args)
    {
        await InternalMain(args, null);
    }


    internal static async Task InternalMain(string[] args, IHostRunner? hostRunner, IServiceProvider? serviceProvider = null)
    {
        _bootstrapLogger.Debug("Starting ReplConsole");

        var host = ConfigureHost(args);
        serviceProvider ??= host.Services;

        var console   = serviceProvider.GetRequiredService<IReplConsole>();
        var appConfig = serviceProvider.GetRequiredService<IReplConsoleConfiguration>();

        var originalTitle = console.Title;
        console.Title = $"{appConfig.AppName} {appConfig.AppVersion}";

        hostRunner ??= new HostRunner(host);
        await hostRunner.RunAsync(CancellationToken.None);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            console.Title = originalTitle;
    }


    /// <summary>
    /// Configures and builds the host for the application.
    /// </summary>
    /// <remarks>
    /// This method configures the application configuration, services, and logging. It sets up the app configuration with various sources including JSON files, user secrets, environment variables, and command-line arguments. It also configures the services for the application, including the <see cref="IReplConsole"/>, command handler types, and the <see cref="ReplConsoleRunner"/>. Finally, it sets up Serilog for logging, reading configuration from both the app configuration and the services.
    /// </remarks>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>The configured <see cref="IHost"/>.</returns>
    internal static IHost ConfigureHost(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, configApp) =>
            {
                var env = hostContext.HostingEnvironment.EnvironmentName.ToLower();

                configApp.SetBasePath(Directory.GetCurrentDirectory());
                configApp.AddJsonFile("appsettings.json", optional: false);
                configApp.AddJsonFile($"appsettings.{env}.json", optional: true);
                configApp.AddUserSecrets<Program>();
                configApp.AddEnvironmentVariables(prefix: EnvPrefix);
                configApp.AddCommandLine(args);
            })
            .ConfigureServices((hostContext, services) =>
            {
                IReplConsoleConfiguration appConfig = new ReplConsoleConfiguration(hostContext.HostingEnvironment.EnvironmentName);
                hostContext.Configuration.GetSection("ReplConsole").Bind(appConfig);

                services.AddSingleton(_ => hostContext.Configuration);
                services.AddSingleton(appConfig);

                services.AddSingleton<IReplCommandDispatcher, ReplCommandDispatcher>();
                
                services.AddSingleton<IReplConsole, ReplConsoleImpl>();
                services.RegisterCliCommandHandlerTypes();

                services.AddHostedService<ReplConsoleRunner>();
            })
            .UseConsoleLifetime()
            .UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            })
            .Build();

        return host;
    }

    /// <summary>
    /// Creates a bootstrap logger for the <see cref="Program"/> class.
    /// </summary>
    /// <remarks>
    /// This method configures a new <see cref="ILogger"/> instance with the following settings:
    /// - Minimum log level set to Debug.
    /// - Overrides the minimum log level for "Microsoft" to Information.
    /// - Enriches the log entries with context information.
    /// - Writes the log entries to the Debug output with a specific output template.
    /// - Associates the logger with the <see cref="Program"/> class context.
    /// </remarks>
    /// <returns>
    /// A <see cref="ILogger"/> instance configured for the <see cref="Program"/> class.
    /// </returns>
    internal static ILogger CreateBootstrapLogger()
    {
        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(_configuration);

        return loggerConfig.CreateBootstrapLogger().ForContext<Program>();
    }
}
