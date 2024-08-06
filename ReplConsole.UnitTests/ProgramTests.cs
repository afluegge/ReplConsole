using ReplConsole.Commands;
using System.Reflection;
using ReplConsole.Configuration;
using ReplConsole.Utils;
using ReplConsole.Commands.Handler;

namespace ReplConsole.UnitTests;

[SupportedOSPlatform("windows")]
[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
[UnsupportedOSPlatform("linux")]
[UnsupportedOSPlatform("macos")]
public class ProgramTests
{
    private readonly IReplConsoleConfiguration _appConfig;

    public ProgramTests()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"ReplConsole:AppVersion",          "1.0.0"                 },
            {"ReplConsole:AppName",             "ReplConsoleApp"        },
            {"ReplConsole:ConsoleTitle",        "REPL Console"          },
            {"ReplConsole:Prompt",              "> "                    },
            {"ReplConsole:CommandAssemblies:0", "ReplConsole.Commands"  },
            {"ReplConsole:CommandAssemblies:1", "ReplConsole.Extensions"}
        };

        var builder = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings);

        var configuration = builder.Build();

        _appConfig = new ReplConsoleConfiguration("UnitTest");
        configuration.GetSection("ReplConsole").Bind(_appConfig);
    }


    [Fact]
    public void Configuration_Should_Be_Setup_Correctly()
    {
        // Assert
        _appConfig.Should().NotBeNull();
        
        _appConfig.Environment         .Should().Be("UnitTest");
        _appConfig.AppVersion          .Should().Be("1.0.0");
        _appConfig.AppName             .Should().Be("ReplConsoleApp");
        _appConfig.ConsoleTitle        .Should().Be("REPL Console");
        _appConfig.Prompt              .Should().Be("> ");
        _appConfig.CommandAssemblies[0].Should().Be("ReplConsole.Commands");
        _appConfig.CommandAssemblies[1].Should().Be("ReplConsole.Extensions");
    }

    [Fact]
    public void BootstrapLogger_Should_Be_Created_Correctly()
    {
        // Act
        var logger = Program.CreateBootstrapLogger();

        // Assert
        logger.Should().NotBeNull();
    }

    [Fact]
    public async Task Main_Should_Run_Host_And_Set_Console_Title()
    {
        // Arrange
        var mockHost = new Mock<IHost>();
        var mockReplConsole = new Mock<IReplConsole>();
        mockReplConsole.SetupProperty(c => c.Title, "OriginalTitle");

        var services = new ServiceCollection();
        services.AddSingleton(mockReplConsole.Object);
        services.AddSingleton(_appConfig);
        var serviceProvider = services.BuildServiceProvider();

        mockHost.Setup(h => h.Services).Returns(serviceProvider);

        var mockHostRunner = new Mock<IHostRunner>();
        mockHostRunner.Setup(r => r.RunAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await Program.InternalMain([], mockHostRunner.Object, serviceProvider);

        // Assert
        mockReplConsole.VerifySet(c => c.Title = It.Is<string>(s => s.StartsWith("ReplConsole")), Times.Once);
        mockReplConsole.VerifySet(c => c.Title = "OriginalTitle", Times.Once);
    }

    [Fact]
    public void ConfigureHost_Should_Setup_Host_Correctly()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var host = Program.ConfigureHost(args);

        // Assert
        host.Should().NotBeNull();
        host.Services.GetService<IReplConsole>().Should().NotBeNull();
        host.Services.GetService<IReplConsoleConfiguration>().Should().NotBeNull();
        host.Services.GetService<IConfiguration>().Should().NotBeNull();
    }
    
    [Fact]
    public void RegisterCliCommandHandlerTypes_RegistersExpectedTypes()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockLogger = new Mock<ILogger<ReplCommandDispatcher>>();
        var mockConfig = new Mock<IReplConsoleConfiguration>();
        var mockConsole = new Mock<IReplConsole>();
        var mockAssemblyLoader = new Mock<IAssemblyLoader>();
        
        mockConfig.Setup(c => c.CommandAssemblies).Returns(new[] { "TestAssembly" });
        
        services.AddSingleton(mockLogger.Object);
        services.AddSingleton(mockConfig.Object);
        services.AddSingleton(mockConsole.Object);
        services.AddSingleton(mockAssemblyLoader.Object);
        
        // Add loggers for each command handler
        services.AddSingleton(new Mock<ILogger<HelloWorldCommandHandler>>().Object);
        services.AddSingleton(new Mock<ILogger<ExitCommandHandler>>().Object);
        services.AddSingleton(new Mock<ILogger<ClearConsoleCommandHandler>>().Object);
        services.AddSingleton(new Mock<ILogger<ReplCommandHandlerImpl>>().Object);
        
        // Mock the assembly to return a type that implements IReplCommandHandler and is not CommandHandlerBase
        var mockAssembly = new Mock<Assembly>();
        mockAssembly.Setup(a => a.GetTypes()).Returns([typeof(TestMockClass), typeof(ReplCommandHandlerImpl)]);
        
        // Setup AssemblyLoader to return our mock assembly
        mockAssemblyLoader.Setup(a => a.LoadFrom(It.IsAny<string>())).Returns(mockAssembly.Object);
        
        // Act
        services.RegisterCliCommandHandlerTypes(mockAssemblyLoader.Object);
        
        // Assert
        var serviceProvider = services.BuildServiceProvider();
        
        var registeredHandler = serviceProvider.GetServices<IReplCommandHandler>().ToList();

        registeredHandler.Should().Contain(h => h.GetType() == typeof(HelloWorldCommandHandler));
        registeredHandler.Should().Contain(h => h.GetType() == typeof(ExitCommandHandler));
        registeredHandler.Should().Contain(h => h.GetType() == typeof(ClearConsoleCommandHandler));
        registeredHandler.Should().Contain(h => h.GetType() == typeof(ReplCommandHandlerImpl));
    }
}


// Dummy implementation of IReplCommandHandler for testing
public class ReplCommandHandlerImpl : IReplCommandHandler
{
    public string Name => "TestCommand";
    public string Description => "Test command for unit testing";
    
    public ValueTask Handle(string[] args, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}

public class TestMockClass
{
    public int FooVal { get; set; }
}
