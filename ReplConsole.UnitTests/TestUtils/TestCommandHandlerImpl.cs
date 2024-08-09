using System.Diagnostics.CodeAnalysis;
using ReplConsole.Commands;

namespace ReplConsole.UnitTests.TestUtils;

[ExcludeFromCodeCoverage]
public class TestCommandHandlerImpl : IReplCommandHandler
{
    public string Name        => "TestCommand";
    public string Description => "Test command for unit testing";
    

    public ValueTask Handle(string[] args, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}
