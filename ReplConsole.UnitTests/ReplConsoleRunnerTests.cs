using System.Diagnostics.CodeAnalysis;
using ReplConsole.Commands;
using ReplConsole.Configuration;
using ReplConsole.Utils;

namespace ReplConsole.UnitTests;

[SupportedOSPlatform("windows")]
[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
[UnsupportedOSPlatform("linux")]
[UnsupportedOSPlatform("macos")]
public class ReplConsoleRunnerTests
{
    private static readonly string[] _args = [ "arg1", "arg2" ];

    private readonly Mock<ILogger<ReplConsoleRunner>> _mockLogger;
    private readonly Mock<IReplConsole>               _mockConsole;
    private readonly Mock<IServiceProvider>           _mockServiceProvider;
    private readonly ReplConsoleRunner                _runner;
    private readonly Mock<IReplCommandDispatcher>     _mockDispatcher;

    public ReplConsoleRunnerTests()
    {
        var mockAppLifetime   = new Mock<IHostApplicationLifetime>();
        var mockLoggerFactory = new Mock<ILoggerFactory>();
        var mockConfig        = new Mock<IReplConsoleConfiguration>();

        _mockLogger          = new Mock<ILogger<ReplConsoleRunner>>();
        _mockConsole         = new Mock<IReplConsole>();
        _mockServiceProvider = new Mock<IServiceProvider>();

        mockConfig.SetupGet(c => c.AppName).Returns("TestApp");
        mockConfig.SetupGet(c => c.AppVersion).Returns("1.0");
        mockConfig.SetupGet(c => c.Prompt).Returns("> ");

        // Create a list to hold your command handler mocks
        var commandHandlerMocks = new List<Mock<IReplCommandHandler>> {
            // Add a mock for each command handler you want to test
            CreateMockHandler("Foo", "Foo-Handler"),
            CreateMockHandler("Baa", "Baa-Handler")
        };
        // Convert the list of mocks to a list of objects
        var commandHandlers = commandHandlerMocks.Select(m => m.Object).ToList();
        
        // Set up the service provider mock to return the list of command handlers
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(IEnumerable<IReplCommandHandler>)))
            .Returns(commandHandlers);

        _mockDispatcher = new Mock<IReplCommandDispatcher>();
        
        _runner = new ReplConsoleRunner(
            _mockLogger.Object,
            _mockDispatcher.Object,
            _mockConsole.Object,
            mockConfig.Object,
            mockAppLifetime.Object,
            mockLoggerFactory.Object,
            _mockServiceProvider.Object
        );

        return;
        

        static Mock<IReplCommandHandler> CreateMockHandler(string name, string description)
        {
            // Create a mock for each command handler you want to test
            var commandHandlerMock = new Mock<IReplCommandHandler>();
            
            // Set up the Name and Description properties
            commandHandlerMock.Setup(m => m.Name).Returns(name);
            commandHandlerMock.Setup(m => m.Description).Returns(description);
            
            return commandHandlerMock;
        }
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPrintAppNameAndVersion()
    {
        CancellationTokenSource cts = null!;

        try
        {
            // Arrange
            cts = new CancellationTokenSource();
            var token = cts.Token;

            await cts.CancelAsync(); // Cancel immediately to exit the loop

            // Act
            await _runner.StartAsync(token);

            // Assert
            _mockConsole.Verify(c => c.WriteLine("TestApp Version 1.0\n"), Times.Once);
        }
        finally
        {
            cts.Dispose();
        }
    }

    [Fact]
    public async Task ExecuteAsync_ShouldContinueOnEmptyCommand()
    {
        CancellationTokenSource cts = null!;

        try
        {
            // Arrange
            cts = new CancellationTokenSource();

            _mockConsole.SetupSequence(c => c.ReadLine())
                .Returns(string.Empty)
                .Returns((string?)null); // Exit loop

            // Act
            await _runner.StartAsync(cts.Token);

            // Assert
            _mockConsole.Verify(c => c.Write(It.IsAny<string>()), Times.Exactly(2));
        }
        finally
        {
            cts.Dispose();
        }
    }

    [Fact]
    public async Task ExecuteAsync_ShouldInvokeExitCommandOnNullCommand()
    {
        CancellationTokenSource cts = null!;

        try
        {
            // Arrange
            cts = new CancellationTokenSource();

            _mockConsole.Setup(c => c.ReadLine()).Returns((string?)null);

            // Act
            await _runner.StartAsync(cts.Token);

            // Assert
            _mockDispatcher.Verify(d => d.InvokeCommand(It.Is<string>(s => s == "exit"), It.IsAny<string[]>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            cts.Dispose();
        }
    }

    [Fact]
    public async Task ExecuteAsync_ShouldInvokeCommandWithArguments()
    {
        CancellationTokenSource cts = null!;

        try
        {
            // Arrange
            cts = new CancellationTokenSource();

            _mockConsole.SetupSequence(c => c.ReadLine())
                .Returns("test arg1 arg2")
                .Returns((string?)null); // Exit loop

            // Act
            await _runner.StartAsync(cts.Token);

            // Assert
            var comparer = new ArrayValueComparer<string>();
            
            _mockDispatcher.Verify(d => d.InvokeCommand(
                It.Is<string>(s => s == "test"),
                It.Is<string[]>(args => comparer.Equals(args, _args)),
                It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            cts.Dispose();
        }
    }

    [Fact]
    public void OnStarted_ShouldLogServiceStarted()
    {
        // Act
        var method = _runner.GetType().GetMethod("OnStarted", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                     ?? throw new InvalidOperationException("Can't get MethodInfo for \"OnStarted\".");

        method.Invoke(_runner, null);

        // Assert
        _mockLogger.Verify(l => l.Log(
            LogLevel.Debug,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? string.Empty).Contains("ReplConsoleRunner started")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public void OnStopping_ShouldLogServiceStopping()
    {
        // Act
        var method = _runner.GetType().GetMethod("OnStopping", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                     ?? throw new InvalidOperationException("Can't get MethodInfo for \"OnStopping\".");

        method.Invoke(_runner, null);

        // Assert
        _mockLogger.Verify(l => l.Log(
            LogLevel.Debug,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? string.Empty).Contains("Attempting to stop ReplConsoleRunner")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }

    [Fact]
    public void OnStopped_ShouldLogServiceStopped()
    {
        // Act
        var method = _runner.GetType().GetMethod("OnStopped", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                     ?? throw new InvalidOperationException("Can't get MethodInfo for \"OnStopped\".");

        method.Invoke(_runner, null);


        // Assert
        _mockLogger.Verify(l => l.Log(
            LogLevel.Debug,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? string.Empty).Contains("ReplConsoleRunner stopped")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()
        ), Times.Once);
    }
}


[ExcludeFromCodeCoverage]
public class ArrayValueComparer<T> : IEqualityComparer<T[]>
{
    public bool Equals(T[]? x, T[]? y)
    {
        if (x == null && y == null)
            return true;
        
        if (x == null || y == null)
            return false;

        return x.SequenceEqual(y);
    }

    public int GetHashCode(T[] obj)
    {
        return obj.Aggregate(0, (current, element) => current ^ (element?.GetHashCode() ?? 0));
    }
}
