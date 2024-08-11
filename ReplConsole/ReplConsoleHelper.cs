using ReplConsole.Commands;
using ReplConsole.Configuration;
using ReplConsole.Utils;

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
    /// <param name="assemblyLoader">
    /// The <see cref="IAssemblyLoader"/> to use for loading assemblies. If <c langword="null"/>, the default implementation is used.
    /// Can be used for unit testing.
    /// </param>
    public static void RegisterCliCommandHandlerTypes(this IServiceCollection services, IAssemblyLoader? assemblyLoader = null)
    {
        // If no IAssemblyLoader is provided, use the default implementation
        assemblyLoader ??= new AssemblyLoader();

        var serviceProvider             = services.BuildServiceProvider();
        var logger                      = serviceProvider.GetRequiredService<ILogger<ReplCommandDispatcher>>();
        var assembly                    = Assembly.GetExecutingAssembly();
        var executingAssemblyPath       = Path.GetDirectoryName(assembly.Location) ?? string.Empty;
        var commandHandlerInterfaceType = typeof(IReplCommandHandler);
        var commandHandlerTypes         = new List<Type>();

        var assemblyFileNames = Directory.GetFiles(executingAssemblyPath, "*.dll");

        foreach (var assemblyFileName in assemblyFileNames)
        {
            try
            {
                assembly = assemblyLoader.LoadFrom(assemblyFileName);

                // Check if the assembly has the CommandAssemblyAttribute
                if (!assembly.GetCustomAttributes<ReplCommandsAssemblyAttribute>().Any())
                    continue;

                var assemblyTypes = assembly.GetTypes()
                    .Where(t => commandHandlerInterfaceType.IsAssignableFrom(t) && t is { IsClass: true, IsAbstract: false });

                commandHandlerTypes.AddRange(assemblyTypes);
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Handle the exception if needed
                logger.LogError("Error loading types from assembly {AssemblyFileName}: {ExceptionMessage}", assemblyFileName, ex.Message);
            }
            catch (Exception ex)
            {
                // Handle other exceptions if needed
                logger.LogError("Error loading assembly {AssemblyFileName}: {ExceptionMessage}", assemblyFileName, ex.Message);
            }
        }

        foreach (var commandHandlerType in commandHandlerTypes)
        {
            logger.LogDebug(HandlerTypeAddedMessage, commandHandlerType.FullName);
            services.AddScoped(commandHandlerInterfaceType, commandHandlerType);
        }
    }
}
