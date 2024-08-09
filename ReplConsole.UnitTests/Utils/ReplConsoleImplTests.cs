using System.Drawing;
using ReplConsole.Utils;
using ReplConsole.Utils.WindowsConsole;

namespace ReplConsole.UnitTests.Utils;

[SupportedOSPlatform("windows")]
[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
[UnsupportedOSPlatform("linux")]
[UnsupportedOSPlatform("macos")]
public class ReplConsoleImplTests
{
    private readonly Mock<IConsole>  _mockConsole;
    private readonly ReplConsoleImpl _replConsole;
    

    public ReplConsoleImplTests()
    {
        _mockConsole = new Mock<IConsole>();
        _replConsole = new ReplConsoleImpl(_mockConsole.Object);
    }
    

    [Fact]
    public void Title_ShouldGetAndSetTitle()
    {
        // Arrange
        const string title = "Test Title";
        _mockConsole.SetupProperty(c => c.Title, title);

        // Act
        _replConsole.Title = title;

        // Assert
        _replConsole.Title.Should().Be(title);
        _mockConsole.VerifySet(c => c.Title = title, Times.Once);
        _mockConsole.VerifyGet(c => c.Title, Times.Once);
    }

    [Fact]
    public void Read_ShouldReturnConsoleRead()
    {
        // Arrange
        const int expectedValue = 42;
        _mockConsole.Setup(c => c.Read()).Returns(expectedValue);

        // Act
        var result = _replConsole.Read();

        // Assert
        result.Should().Be(expectedValue);
        _mockConsole.Verify(c => c.Read(), Times.Once);
    }

    [Fact]
    public void ReadKey_ShouldReturnConsoleReadKey()
    {
        // Arrange
        var expectedKeyInfo = new ConsoleKeyInfo('A', ConsoleKey.A, false, false, false);
        _mockConsole.Setup(c => c.ReadKey(It.IsAny<bool>())).Returns(expectedKeyInfo);

        // Act
        var result = _replConsole.ReadKey();

        // Assert
        result.Should().Be(expectedKeyInfo);
        _mockConsole.Verify(c => c.ReadKey(false), Times.Once);
    }

    [Fact]
    public void ReadKey_WithIntercept_ShouldReturnConsoleReadKey()
    {
        // Arrange
        var expectedKeyInfo = new ConsoleKeyInfo('A', ConsoleKey.A, false, false, false);
        _mockConsole.Setup(c => c.ReadKey(It.IsAny<bool>())).Returns(expectedKeyInfo);

        // Act
        var result = _replConsole.ReadKey(true);

        // Assert
        result.Should().Be(expectedKeyInfo);
        _mockConsole.Verify(c => c.ReadKey(true), Times.Once);
    }

    [Fact]
    public void ReadLine_ShouldReturnConsoleReadLine()
    {
        // Arrange
        const string expectedLine = "Test Line";
        _mockConsole.Setup(c => c.ReadLine()).Returns(expectedLine);

        // Act
        var result = _replConsole.ReadLine();

        // Assert
        result.Should().Be(expectedLine);
        _mockConsole.Verify(c => c.ReadLine(), Times.Once);
    }

    [Fact]
    public void Clear_ShouldCallConsoleClear()
    {
        // Act
        _replConsole.Clear();

        // Assert
        _mockConsole.Verify(c => c.Clear(), Times.Once);
    }

    [Fact]
    public void Write_ShouldCallConsoleWrite()
    {
        // Arrange
        const string text = "Test Text";

        // Act
        _replConsole.Write(text);

        // Assert
        _mockConsole.Verify(c => c.Write(text), Times.Once);
    }

    [Fact]
    public void Write_WithConsoleColor_ShouldCallConsoleWrite()
    {
        // Arrange
        const string text = "Test Text";
        var color = ConsoleColor.Green;

        // Act
        _replConsole.Write(text, color);

        // Assert
        _mockConsole.Verify(c => c.Write(It.Is<string>(s => s.Contains(text))), Times.Once);
    }

    [Fact]
    public void WriteLine_WithText_ShouldCallConsoleWriteLine_WithText()
    {
        // Arrange
        const string line = "Test Line";

        // Act
        _replConsole.WriteLine(line);

        // Assert
        _mockConsole.Verify(c => c.WriteLine(line), Times.Once);
    }

    [Fact]
    public void WriteLine_WithoutText_ShouldCallConsoleWriteLine_WithoutText()
    {
        // Act
        _replConsole.WriteLine();

        // Assert
        _mockConsole.Verify(c => c.WriteLine(), Times.Once);
    }

    [Fact]
    public void WriteLine_WithConsoleColor_ShouldCallConsoleWriteLine()
    {
        // Arrange
        const string line = "Test Line";
        var color = ConsoleColor.Green;

        // Act
        _replConsole.WriteLine(line, color);

        // Assert
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains(line))), Times.Once);
    }

    [Fact]
    public void WriteWarning_ShouldCallConsoleWriteLineWithYellowColor()
    {
        // Arrange
        const string warning = "Test Warning";

        // Act
        _replConsole.WriteWarning(warning);

        // Assert
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains(warning))), Times.Once);
    }

    [Fact]
    public void WriteError_ShouldCallConsoleWriteLineWithRedColor()
    {
        // Arrange
        const string error = "Test Error";

        // Act
        _replConsole.WriteError(error);

        // Assert
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains(error))), Times.Once);
    }

    [Fact]
    public void WriteError_WithException_ShouldCallConsoleWriteLineWithRedColor()
    {
        // Arrange
        var exception = GenerateExceptionWithStackTrace();
        var stackTrace = exception.StackTrace ?? string.Empty;

        // Act
        _replConsole.WriteError(exception);

        // Assert
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains($"[{exception.GetType().Name}] {exception.Message}"))), Times.Once);
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s.Contains(stackTrace))), Times.Once);
    }

    [Fact]
    public void WriteLine_WithColor_ShouldCallConsoleWriteLineWithColoredText()
    {
        // Arrange
        const string line = "Test Line";
        var color = Color.FromArgb(255, 0, 0); // Red color

        // Act
        _replConsole.WriteLine(line, color);

        // Assert
        _mockConsole.Verify(c => c.WriteLine(It.Is<string>(s => s == line.Pastel(color))), Times.Once);
    }

    [Fact]
    public void Write_WithColor_ShouldCallConsoleWriteWithColoredText()
    {
        // Arrange
        const string text = "Test Text";
        var color = Color.FromArgb(0, 255, 0); // Green color

        // Act
        _replConsole.Write(text, color);

        // Assert
        _mockConsole.Verify(c => c.Write(It.Is<string>(s => s == text.Pastel(color))), Times.Once);
    }


    private static Exception GenerateExceptionWithStackTrace()
    {
        try
        {
            throw new Exception("Test Exception");
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}
