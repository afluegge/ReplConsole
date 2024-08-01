namespace ReplConsole;

/// <summary>
/// Represents errors that occur during application execution.
/// </summary>
/// <remarks>
/// The <see cref="ReplConsoleException"/> class is the base class for exceptions that are thrown for application-specific errors.
/// </remarks>
[PublicAPI]
public class ReplConsoleException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReplConsoleException"/> class.
    /// </summary>
    public ReplConsoleException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReplConsoleException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ReplConsoleException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReplConsoleException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException"/> parameter is not a <see langword="null"/> reference, the current exception is raised in a catch block that handles the inner exception.</param>
    public ReplConsoleException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
