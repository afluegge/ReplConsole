namespace ReplConsole.Configuration;

/// <summary>
/// Provides configuration settings for the REPL console application.
/// </summary>
/// <remarks>
/// This interface defines the properties that are used to configure the REPL console application. 
/// These include the application version, name, console title, prompt, and command assemblies.
/// </remarks>
[PublicAPI]
public interface IReplConsoleConfiguration
{
    /// <summary>
    /// Gets the environment for the REPL console application.
    /// </summary>
    string Environment { get; }
    
    /// <summary>
    /// Gets the version of the application.
    /// </summary>
    /// <value>The version of the application.</value>
    string AppVersion { get; }
    
    /// <summary>
    /// Gets the name of the application.
    /// </summary>
    /// <value>The name of the application.</value>
    string AppName { get; }
    
    /// <summary>
    /// Gets or sets the title of the console.
    /// </summary>
    /// <value>The title of the console.</value>
    string ConsoleTitle { get; set; }
    
    /// <summary>
    /// Gets or sets the prompt of the console.
    /// </summary>
    /// <value>The prompt of the console.</value>
    string Prompt { get; set; }
    
    /// <summary>
    /// Gets the command assemblies of the application.
    /// </summary>
    /// <value>The command assemblies of the application.</value>
    /// <remarks>
    /// The <see langword="string"/>[] array contains the names of the assemblies that contain the command handlers for the application.
    /// </remarks>
    string[] CommandAssemblies { get; }
}
