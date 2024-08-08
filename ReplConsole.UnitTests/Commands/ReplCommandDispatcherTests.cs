using System.Drawing;
using ReplConsole.Commands;
using ReplConsole.Commands.Handler;
using ReplConsole.Configuration;
using ReplConsole.UnitTests.Commands.Handler;
using ReplConsole.UnitTests.TestUtils;
using ReplConsole.Utils;

namespace ReplConsole.UnitTests.Commands;

public class ReplCommandDispatcherTests
{
    private readonly Mock<IReplConsole>    _mockConsole;
    private readonly ReplCommandDispatcher _dispatcher;
    private readonly MockCommandHandler    _mockCommand;

    public ReplCommandDispatcherTests()
    {
        // Following commands will be registered in the command dispatcher:
        // ----------------------------------------------------------------
        //
        // - MockCommand
        // - TestCommand
        // - cls
        // - exit
        // - hello
        //
        
        var mockConfig = new Mock<IReplConsoleConfiguration>();
        var mockLogger = new Mock<ILogger<ReplCommandDispatcher>>();

        _mockConsole = new Mock<IReplConsole>();

        mockConfig.Setup(c => c.AppName).Returns("TestApp");
        
        var serviceProvider = TestHelper.ConfigureMockServiceProvider(services =>
        {
            services.AddSingleton(new Mock<ILogger<MockCommandHandler>>().Object);
            services.AddSingleton<IReplCommandHandler, MockCommandHandler>();
        });

        var registeredCommands = serviceProvider.GetServices<IReplCommandHandler>();

        _mockCommand = (registeredCommands.FirstOrDefault(cmd => cmd.Name == "MockCommand") as MockCommandHandler) ?? throw new InvalidOperationException("Could not get \"MockCommandHandler\".");

        _dispatcher = new ReplCommandDispatcher(
            mockLogger.Object,
            mockConfig.Object,
            _mockConsole.Object,
            serviceProvider
        );
    }

    [Fact]
    public async Task InvokeCommand_ShouldPrintHelp_WhenCommandIsHelp()
    {
        // Act
        await _dispatcher.InvokeCommand("help", [], CancellationToken.None);

        // Assert
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("TestApp - Command Line Help"))), Times.Once);
    }

    [Fact]
    public async Task InvokeCommand_ShouldPrintHelp_WhenCommandIsQuestionMark()
    {
        // Act
        await _dispatcher.InvokeCommand("?", [], CancellationToken.None);

        // Assert
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("TestApp - Command Line Help"))), Times.Once);
    }

    [Fact]
    public async Task InvokeCommand_ShouldPrintUnknownCommand_WhenCommandIsNotRecognized()
    {
        // Act
        await _dispatcher.InvokeCommand("unknown", [], CancellationToken.None);

        // Assert
        _mockConsole.Verify(c => c.Write(It.Is<string>(s => s == "\nError: Unknown Command '"), It.IsAny<Color>()), Times.Once);
        _mockConsole.Verify(c => c.Write(It.Is<string>(s => s == "unknown"), It.IsAny<Color>()), Times.Once);
    }

    [Fact]
    public async Task InvokeCommand_ShouldInvokeCommandHandler_WhenCommandIsRecognized()
    {
        // Arrange
        var commandParameter = new[] { "Foo", "Baa" };

        // Act
        await _dispatcher.InvokeCommand("MockCommand", commandParameter, CancellationToken.None);

        // Assert
        _mockCommand.HandleCommandCalled.Should().BeTrue();
        _mockCommand.ReceivedArgs.Should().BeEquivalentTo(commandParameter);
    }

    [Fact]
    public async Task PrintHelp_ShouldPrintAllRegisteredCommands()
    {
        // Act
        await _dispatcher.InvokeCommand("help", [], CancellationToken.None);
        
        // Assert
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("MockCommand"))), Times.Once);
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("cls"))), Times.Once);
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("exit"))), Times.Once);
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("hello"))), Times.Once);
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains("TestCommand"))), Times.Once);
    }
}
