using ReplConsole.Commands.Handler;
using ReplConsole.Configuration;
using ReplConsole.Utils;

namespace ReplConsole.UnitTests.Commands.Handler;

public class ExitCommandHandlerTests
{
    private readonly Mock<IReplConsole> _consoleMock;
    private readonly Mock<IEnvironment> _environmentMock;
    private readonly TestableExitCommandHandler _handler;

    
    public ExitCommandHandlerTests()
    {
        Mock<ILogger<ExitCommandHandler>> loggerMock = new();
        Mock<IReplConsoleConfiguration> configMock   = new();
        configMock.SetupGet(c => c.AppName).Returns("TestApp");
        
        _consoleMock     = new Mock<IReplConsole>();
        _environmentMock = new Mock<IEnvironment>();
        _handler         = new TestableExitCommandHandler(loggerMock.Object, _consoleMock.Object, configMock.Object, _environmentMock.Object);
    }
    

    [Fact]
    public void Name_ShouldBe_Exit()
    {
        // Act
        var name = _handler.Name;

        // Assert
        name.Should().Be("exit");
    }

    [Fact]
    public void Description_ShouldBe_ExitsTestApp()
    {
        // Act
        var description = _handler.Description;

        // Assert
        description.Should().Be("Exits TestApp.");
    }

    [Fact]
    public async Task HandleCommand_ShouldPrintByeAndExit()
    {
        // Arrange
        var args = Array.Empty<string>();
        var cancellationToken = CancellationToken.None;

        // Act
        await _handler.TestHandleCommand(args, cancellationToken);

        // Assert
        _consoleMock.Verify(console => console.WriteLine("Bye..."), Times.Once);
        _environmentMock.Verify(env => env.Exit(0), Times.Once);
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
    
    

    private class TestableExitCommandHandler(ILogger<ExitCommandHandler> logger, IReplConsole console, IReplConsoleConfiguration config, IEnvironment environment) : ExitCommandHandler(logger, console, config, environment)
    {
        public ValueTask TestHandleCommand(string[] args, CancellationToken cancellationToken)
        {
            return HandleCommand(args, cancellationToken);
        }
    }
}
