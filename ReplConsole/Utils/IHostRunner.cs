namespace ReplConsole.Utils;

/// <summary>
/// This interface is used to abstract the running of a host.
/// </summary>
public interface IHostRunner
{
    /// <summary>
    /// Run the host asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use for cancelling the operation.</param>
    /// <returns></returns>
    Task RunAsync(CancellationToken cancellationToken);
}
