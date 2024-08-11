namespace ReplConsole.Utils;

/// <summary>
/// This class is used to abstract the running of a host.
/// </summary>
/// <remarks>
/// This is a straight forward wrapper to the corresponding calls of the passed in .NET 'IHostRunner' implementation.
/// This wrapper exists only for making code more testable.
/// There is no need for explicitly testing this code.
/// </remarks>
/// <param name="host"></param>
[ExcludeFromCodeCoverage]
internal class HostRunner(IHost host) : IHostRunner
{
    /// <summary>
    /// Run the host asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use for cancelling the operation.</param>
    /// <returns></returns>
    public Task RunAsync(CancellationToken cancellationToken) => host.RunAsync(cancellationToken);
}
