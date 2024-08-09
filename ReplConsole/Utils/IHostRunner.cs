namespace ReplConsole.Utils;

public interface IHostRunner
{
    Task RunAsync(CancellationToken cancellationToken);
}
