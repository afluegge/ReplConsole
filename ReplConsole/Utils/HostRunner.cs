namespace ReplConsole.Utils;

// This is a straight forward wrapper to the corresponding calls of the passed in .NET 'IHostRunner' implementation.
// This wrapper exists only for making code more testable.
// I see no need for explicitly testing this code.
[ExcludeFromCodeCoverage]
internal class HostRunner(IHost host) : IHostRunner
{
    public Task RunAsync(CancellationToken cancellationToken)
    {
        return host.RunAsync(cancellationToken);
    }
}
