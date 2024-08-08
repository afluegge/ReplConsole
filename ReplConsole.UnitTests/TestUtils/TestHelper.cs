using System.Reflection;
using ReplConsole.Commands;
using ReplConsole.Commands.Handler;
using ReplConsole.Configuration;
using ReplConsole.Utils;

namespace ReplConsole.UnitTests.TestUtils;

internal static class TestHelper
{
    public static ServiceProvider ConfigureMockServiceProvider(Action<ServiceCollection>? addServices = null)
    {
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
        services.AddSingleton(new Mock<ILogger<TestCommandHandlerImpl>>().Object);

        
        // Give the caller a chance to register additional services
        addServices?.Invoke(services);


        // Mock the assembly to return a type that implements IReplCommandHandler and is not CommandHandlerBase
        var mockAssembly = new Mock<Assembly>();
        mockAssembly.Setup(a => a.GetTypes()).Returns([typeof(TestMockClass), typeof(TestCommandHandlerImpl)]);

        // Setup AssemblyLoader to return our mock assembly
        mockAssemblyLoader.Setup(a => a.LoadFrom(It.IsAny<string>())).Returns(mockAssembly.Object);

        services.RegisterCliCommandHandlerTypes(mockAssemblyLoader.Object);

        var serviceProvider = services.BuildServiceProvider();

        return serviceProvider;
    }
}
