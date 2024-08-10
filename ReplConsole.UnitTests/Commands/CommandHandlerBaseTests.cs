using ReplConsole.UnitTests.TestUtils;
using ReplConsole.Utils;

namespace ReplConsole.UnitTests.Commands;

public class CommandHandlerBaseTests
{
    private readonly Mock<ILogger<MockCommandHandler>> _mockLogger;
    private readonly MockCommandHandler                _commandHandler;
    

    public CommandHandlerBaseTests()
    {
        var mockReplConsole = new Mock<IReplConsole>();
        _mockLogger         = new Mock<ILogger<MockCommandHandler>>();
        _commandHandler     = new MockCommandHandler(_mockLogger.Object, mockReplConsole.Object);
    }
    

    [Fact]
    public async Task Handle_ShouldLogCommandNameAndCallHandleCommand()
    {
        // Arrange
        var args              = new[] { "arg1", "arg2" };
        var cancellationToken = CancellationToken.None;

        // Act
        await _commandHandler.Handle(args, cancellationToken);

        // Assert
        _mockLogger.Verify(
            logger => logger.Log(
                It.Is<LogLevel>(level => level == LogLevel.Debug),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? string.Empty).Contains("Handle MockCommand command")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);

        _commandHandler.HandleCommandCalled.Should().BeTrue();
    }

    [Fact]
    public async Task HandleCommand_ShouldBeCalledWithCorrectArguments()
    {
        // Arrange
        var args              = new[] { "arg1", "arg2" };
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _commandHandler.Handle(args, cancellationToken);

        // Assert
        _commandHandler.ReceivedArgs.Should().BeEquivalentTo(args);
        _commandHandler.ReceivedCancellationToken.Should().Be(cancellationToken);
    }

    [Fact]
    public async Task HandleCommand_ShouldHandleEmptyArguments()
    {
        // Arrange
        var args = Array.Empty<string>();
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _commandHandler.Handle(args, cancellationToken);

        // Assert
        _commandHandler.ReceivedArgs.Should().BeEquivalentTo(args);
        _commandHandler.ReceivedCancellationToken.Should().Be(cancellationToken);
    }

    [Fact]
    public async Task HandleCommand_ShouldHandleNullArguments()
    {
        // Arrange
        string[] args = null!;
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _commandHandler.Handle(args, cancellationToken);

        // Assert
        _commandHandler.ReceivedArgs.Should().BeNull();
        _commandHandler.ReceivedCancellationToken.Should().Be(cancellationToken);
    }
}