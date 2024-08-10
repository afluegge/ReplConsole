namespace ReplConsole.Utils;

// This is a straight forward wrapper to the corresponding calls of the .NET 'Environment' class.
// This wrapper exists only for making code more testable.
// I see no need for explicitly testing this code.
[ExcludeFromCodeCoverage]
internal class WindowsEnvironmentImpl : IEnvironment
{
    public void Exit(int exitCode) => Environment.Exit(exitCode);
}
