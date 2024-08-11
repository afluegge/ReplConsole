namespace ReplConsole.Utils;

/// <summary>
/// This interface is used to abstract the <see cref="Environment"/> static class for unit testing.
/// </summary>
public interface IEnvironment
{
    /// <summary>
    /// Exit the current process with the given exit code.
    /// </summary>
    /// <param name="exitCode">The exit code to return to the operating system.</param>
    void Exit(int exitCode);
}
