namespace ReplConsole.UnitTests;

public class ReplConsoleExceptionTests
{
    [Fact]
    public void DefaultConstructor_ShouldCreateException()
    {
        // Arrange & Act
        var exception = new ReplConsoleException();

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be("Exception of type 'ReplConsole.ReplConsoleException' was thrown.");
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        // Arrange
        const string message = "Test message";

        // Act
        var exception = new ReplConsoleException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldSetProperties()
    {
        // Arrange
        const string message = "Test message";
        var innerException = new Exception("Inner exception");

        // Act
        var exception = new ReplConsoleException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }
}
