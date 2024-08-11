namespace ReplConsole.Utils;

/// <summary>
/// This class is used to abstract the <see cref="Environment"/> static class for unit testing.
/// </summary>
/// <remarks>
/// This is a straight forward wrapper to the corresponding calls of the .NET 'Environment' class.
/// There is no need for explicitly testing this code.
/// </remarks>
[ExcludeFromCodeCoverage]
internal class WindowsEnvironmentImpl : IEnvironment
{
    /// <summary>
    /// Exit the current process with the given exit code.
    /// </summary>
    /// <param name="exitCode">The exit code to return to the operating system.</param>
    public void Exit(int exitCode) => Environment.Exit(exitCode);
}
