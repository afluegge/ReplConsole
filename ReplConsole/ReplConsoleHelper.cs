using ReplConsole.Commands;
using ReplConsole.Configuration;

namespace ReplConsole;

/// <summary>
/// The <c langword="ReplConsoleHelper"/> class.
/// </summary>
/// <remarks>
/// This is an internal static class that provides helper methods for the REPL console application.
/// </remarks>
internal static class ReplConsoleHelper
{
    private const string HandlerTypeAddedMessage = "Add '{HandlerType}' to services.";


    /// <summary>
    /// Registers CLI command handler types to the provided service collection.
    /// </summary>
    /// <remarks>
    /// This method performs the following:
    /// - Retrieves the current service provider and configuration.
    /// - Scans the executing assembly for types implementing the <see cref="IReplCommandHandler"/> interface and adds them to the service collection.
    /// - For each assembly name listed in the configuration's <see cref="IReplConsoleConfiguration.CommandAssemblies"/>, it attempts to load the assembly and scan it for types implementing the <see cref="IReplCommandHandler"/> interface, adding them to the service collection.
    /// - If an assembly fails to load, an error is logged.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the CLI command handler types are registered.</param>
    public static void RegisterCliCommandHandlerTypes(this IServiceCollection services)
    {
        // ToDo: The logger retrieved here does use a different output template than all other logger in the solution.
        
        
        var serviceProvider = services.BuildServiceProvider();
        var logger          = serviceProvider.GetRequiredService<ILogger<ReplCommandDispatcher>>();
        var config          = serviceProvider.GetRequiredService<IReplConsoleConfiguration>();
        var assembly        = Assembly.GetExecutingAssembly();

        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            var cliCommandType = type.GetInterface("IReplCommandHandler");

            if (cliCommandType == null || type == typeof(CommandHandlerBase))
                continue;

            logger.LogDebug(HandlerTypeAddedMessage, type.FullName);

            services.AddScoped(cliCommandType, type); // Register by interface
        }

        // Load and scan assemblies listed in "CommandAssemblies"
        var executingAssemblyPath = Path.GetDirectoryName(assembly.Location) ?? string.Empty;
        
        foreach (var assemblyName in config.CommandAssemblies)
        {
            var assemblyPath = Path.Combine(executingAssemblyPath, $"{assemblyName}.dll");

            try
            {
                var commandAssembly = Assembly.LoadFrom(assemblyPath);
                var commandTypes = commandAssembly.GetTypes();

                foreach (var type in commandTypes)
                {
                    var cliCommandType = type.GetInterface("IReplCommandHandler");

                    if (cliCommandType == null || type == typeof(CommandHandlerBase))
                        continue;

                    logger.LogDebug(HandlerTypeAddedMessage, type.FullName);

                    services.AddScoped(cliCommandType, type); // Register by interface
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load assembly '{AssemblyName}' from path '{AssemblyPath}'", assemblyName, assemblyPath);
            }
        }
    }
}
