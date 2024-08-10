using ReplConsole.Configuration;
using ReplConsole.Utils;

namespace ReplCustomCommands.UnitTests;

public class ChangePromptCommandHandlerTests
{
    private readonly Mock<IReplConsole>                 _consoleMock;
    private readonly Mock<IReplConsoleConfiguration>    _configMock;
    private readonly TestableChangePromptCommandHandler _handler;
    

    public ChangePromptCommandHandlerTests()
    {
        var loggerMock = new Mock<ILogger<ChangePromptCommandHandler>>();
        
        _consoleMock = new Mock<IReplConsole>();
        _configMock  = new Mock<IReplConsoleConfiguration>();
        _handler     = new TestableChangePromptCommandHandler(loggerMock.Object, _configMock.Object, _consoleMock.Object);
    }
    

    [Fact]
    public void Name_ShouldBe_Prompt()
    {
        // Act
        var name = _handler.Name;

        // Assert
        name.Should().Be("prompt");
    }

    [Fact]
    public void Description_ShouldBe_ChangesThePromptDisplayedInTheConsole()
    {
        // Act
        var description = _handler.Description;

        // Assert
        description.Should().Contain("Changes the prompt displayed in the console.");
    }

    [Fact]
    public async Task HandleCommand_WithNoArguments_ShouldPrintHelp()
    {
        // Arrange
        var args = Array.Empty<string>();
        var cancellationToken = CancellationToken.None;

        // Act
        await _handler.TestHandleCommand(args, cancellationToken);

        // Assert
        _consoleMock.Verify(console => console.WriteError("\nInvalid number of arguments.  Please provide a prompt.\n"), Times.Once);
    }

    [Fact]
    public async Task HandleCommand_WithMultipleArguments_ShouldPrintHelp()
    {
        // Arrange
        var args = new[] { "arg1", "arg2" };
        var cancellationToken = CancellationToken.None;

        // Act
        await _handler.TestHandleCommand(args, cancellationToken);

        // Assert
        _consoleMock.Verify(console => console.WriteError("\nInvalid number of arguments.  Please provide a prompt.\n"), Times.Once);
    }

    [Fact]
    public async Task HandleCommand_WithOneArgument_ShouldChangePrompt()
    {
        // Arrange
        var args = new[] { "newPrompt" };
        var cancellationToken = CancellationToken.None;

        // Act
        await _handler.TestHandleCommand(args, cancellationToken);

        // Assert
        _configMock.VerifySet(config => config.Prompt = "newPrompt ", Times.Once);
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

    private class TestableChangePromptCommandHandler(ILogger<ChangePromptCommandHandler> logger, IReplConsoleConfiguration config, IReplConsole console) : ChangePromptCommandHandler(logger, config, console)
    {
        public ValueTask TestHandleCommand(string[] args, CancellationToken cancellationToken)
        {
            return HandleCommand(args, cancellationToken);
        }
    }
}