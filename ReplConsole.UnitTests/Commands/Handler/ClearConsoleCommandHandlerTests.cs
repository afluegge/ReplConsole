using ReplConsole.Commands.Handler;
using ReplConsole.Utils;

namespace ReplConsole.UnitTests.Commands.Handler;

public class ClearConsoleCommandHandlerTests
{
    private readonly Mock<IReplConsole>                 _consoleMock;
    private readonly TestableClearConsoleCommandHandler _handler;

    
    public ClearConsoleCommandHandlerTests()
    {
        var loggerMock = new Mock<ILogger<ClearConsoleCommandHandler>>();
        
        _consoleMock = new Mock<IReplConsole>();
        _handler     = new TestableClearConsoleCommandHandler(loggerMock.Object, _consoleMock.Object);
    }
    

    [Fact]
    public void Name_ShouldBe_Cls()
    {
        // Act
        var name = _handler.Name;

        // Assert
        name.Should().Be("cls");
    }

    [Fact]
    public void Description_ShouldBe_ClearsTheConsoleScreen()
    {
        // Act
        var description = _handler.Description;

        // Assert
        description.Should().Be("Clears the Console Screen.");
    }

    [Fact]
    public async Task HandleCommand_ShouldClearConsole()
    {
        // Arrange
        var args = Array.Empty<string>();
        var cancellationToken = CancellationToken.None;

        // Act
        await _handler.TestHandleCommand(args, cancellationToken);

        // Assert
        _consoleMock.Verify(console => console.Clear(), Times.Once);
    }

    [Fact]
    public async Task HandleCommand_WithArguments_ShouldStillClearConsole()
    {
        // Arrange
        var args = new[] { "arg1", "arg2" };
        var cancellationToken = CancellationToken.None;

        // Act
        await _handler.TestHandleCommand(args, cancellationToken);

        // Assert
        _consoleMock.Verify(console => console.Clear(), Times.Once);
    }

    [Fact]
    public async Task HandleCommand_WithCancellation_ShouldNotThrow()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var args = Array.Empty<string>();

        // Act
        var act = async () => await _handler.TestHandleCommand(args, cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }
    
    

    private class TestableClearConsoleCommandHandler(ILogger<ClearConsoleCommandHandler> logger, IReplConsole console) : ClearConsoleCommandHandler(logger, console)
    {
        public ValueTask TestHandleCommand(string[] args, CancellationToken cancellationToken)
        {
            return HandleCommand(args, cancellationToken);
        }
    }
}
