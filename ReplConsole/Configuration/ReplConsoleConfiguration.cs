namespace ReplConsole.Configuration;

/// <summary>
/// Initializes a new instance of the <see cref="ReplConsoleConfiguration"/> class.
/// </summary>
/// <remarks>
/// This constructor initializes a new instance of the <see cref="ReplConsoleConfiguration"/> class with the specified environment. 
/// </remarks>
/// <param name="environment">The environment for the REPL console application.</param>
[PublicAPI]
public class ReplConsoleConfiguration(string environment) : IReplConsoleConfiguration
{
    private const string DefaultAppName = "ReplConsole";
    
    
    public string Environment         { get; }       = environment;

    public string AppVersion          { get; set; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "<unknown>";

    public string AppName             { get; set; } = DefaultAppName;

    public string ConsoleTitle        { get; set; }  = DefaultAppName;

    public string Prompt              { get; set; }  = ">>";

    public string[] CommandAssemblies { get; set; } = [];
}
