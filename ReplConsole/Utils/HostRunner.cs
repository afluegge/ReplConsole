namespace ReplConsole.Utils;

internal class HostRunner(IHost host) : IHostRunner
{
    public Task RunAsync(CancellationToken cancellationToken)
    {
        return host.RunAsync(cancellationToken);
    }
}
