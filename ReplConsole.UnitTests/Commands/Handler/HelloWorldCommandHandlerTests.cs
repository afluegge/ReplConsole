using ReplConsole.Commands.Handler;
using ReplConsole.Utils;

namespace ReplConsole.UnitTests.Commands.Handler;

public class HelloWorldCommandHandlerTests
{
    private readonly Mock<ILogger<HelloWorldCommandHandler>> _loggerMock;
    private readonly Mock<IReplConsole>                      _consoleMock;
    private readonly TestableHelloWorldCommandHandler        _handler;

    
    public HelloWorldCommandHandlerTests()
    {
        _loggerMock  = new Mock<ILogger<HelloWorldCommandHandler>>();
        _consoleMock = new Mock<IReplConsole>();
        _handler     = new TestableHelloWorldCommandHandler(_loggerMock.Object, _consoleMock.Object);
    }
    

    [Fact]
    public void Name_ShouldBe_Hello()
    {
        // Act
        var name = _handler.Name;

        // Assert
        name.Should().Be("hello");
    }

    [Fact]
    public void Description_ShouldBe_PrintsANiceGreeting()
    {
        // Act
        var description = _handler.Description;

        // Assert
        description.Should().Be("Prints a nice greeting.");
    }

    [Fact]
    public async Task HandleCommand_ShouldPrintGreeting()
    {
        // Arrange
        var args = Array.Empty<string>();
        var cancellationToken = CancellationToken.None;

        // Act
        await _handler.TestHandleCommand(args, cancellationToken);

        // Assert
        _consoleMock.Verify(console => console.WriteLine("Hello: "), Times.Once);
        _loggerMock.Verify(logger => logger.Log(
            LogLevel.Debug,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? string.Empty).Contains("Hello responded with greeting message.")),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
    }

    [Fact]
    public async Task HandleCommand_WithArguments_ShouldPrintGreetingWithArguments()
    {
        // Arrange
        var args = new[] { "arg1", "arg2" };
        var cancellationToken = CancellationToken.None;

        // Act
        await _handler.TestHandleCommand(args, cancellationToken);

        // Assert
        _consoleMock.Verify(console => console.WriteLine("Hello: arg1, arg2"), Times.Once);
        _loggerMock.Verify(logger => logger.Log(
            LogLevel.Debug,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? string.Empty).Contains("Hello responded with greeting message.")),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
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
    
    

    private class TestableHelloWorldCommandHandler(ILogger<HelloWorldCommandHandler> logger, IReplConsole console) : HelloWorldCommandHandler(logger, console)
    {
        public ValueTask TestHandleCommand(string[] args, CancellationToken cancellationToken)
        {
            return HandleCommand(args, cancellationToken);
        }
    }
}
