using ReplConsole.Commands;
using ReplConsole.Utils;

namespace ReplConsole.UnitTests.Commands.Handler;

public class MockCommandHandler(ILogger<MockCommandHandler> logger, IReplConsole console) : CommandHandlerBase(logger, console)
{
    public bool HandleCommandCalled { get; private set; }
    public string[]? ReceivedArgs { get; private set; }
    public CancellationToken ReceivedCancellationToken { get; private set; }


    public override string Name        => "MockCommand";
    public override string Description => "Mock command for testing";


    protected override ValueTask HandleCommand(string[] args, CancellationToken cancellationToken)
    {
        HandleCommandCalled       = true;
        ReceivedArgs              = args;
        ReceivedCancellationToken = cancellationToken;

        return ValueTask.CompletedTask;
    }
}
