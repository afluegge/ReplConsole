namespace ReplConsole.Utils;

/// <summary>
/// Provides a set of methods for interacting with the console.
/// </summary>
[PublicAPI]
public interface IReplConsole
{
    /// <summary>
    /// Gets or sets the title of the console window.
    /// </summary>
    string Title { get; set; }
    
    /// <summary>
    /// Reads the next character from the standard input stream.
    /// </summary>
    /// <returns>The next character from the input stream, or -1 if no more characters are available.</returns>
    int Read();
    
    ConsoleKeyInfo ReadKey();
    
    /// <summary>
    /// Obtains the next character or function key pressed by the user. The pressed key is optionally displayed in the console window.
    /// </summary>
    /// <param name="intercept">Determines whether the pressed key should be displayed in the console window.</param>
    /// <returns>An object that describes the <see cref="ConsoleKeyInfo"/> of a key press.</returns>
    ConsoleKeyInfo ReadKey(bool intercept);
    
    /// <summary>
    /// Reads the next line of characters from the standard input stream.
    /// </summary>
    /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
    string? ReadLine();
    
    /// <summary>
    /// Writes the specified string value to the standard output stream.
    /// </summary>
    /// <param name="text">The value to write.</param>
    void Write(string text);
    
    /// <summary>
    /// Writes the specified string value to the standard output stream using the specified console color.
    /// </summary>
    /// <param name="text">The value to write.</param>
    /// <param name="color">The color of the text.</param>
    void Write(string text, ConsoleColor color);
    
    /// <summary>
    /// Writes the specified string value to the standard output stream using the specified color.
    /// </summary>
    /// <param name="text">The value to write.</param>
    /// <param name="color">The color of the text.</param>
    void Write(string text, Color color);
    
    /// <summary>
    /// Writes the current line terminator to the standard output stream.
    /// </summary>
    void WriteLine();
    
    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="line">The value to write.</param>
    void WriteLine(string line);
    
    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream using the specified console color.
    /// </summary>
    /// <param name="line">The value to write.</param>
    /// <param name="color">The color of the text.</param>
    void WriteLine(string line, ConsoleColor color);
    
    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the standard output stream using the specified color.
    /// </summary>
    /// <param name="line">The value to write.</param>
    /// <param name="color">The color of the text.</param>
    void WriteLine(string line, Color color);
    
    /// <summary>
    /// Writes the specified warning message to the standard output stream.
    /// </summary>
    /// <param name="error">The warning message to write.</param>
    void WriteWarning(string error);
    
    /// <summary>
    /// Writes the specified error message to the standard output stream.
    /// </summary>
    /// <param name="error">The error message to write.</param>
    void WriteError(string error);
    
    /// <summary>
    /// Writes the specified exception to the standard output stream.
    /// </summary>
    /// <param name="ex">The exception to write.</param>
    void WriteError(Exception ex);
    
    /// <summary>
    /// Clears the console buffer and corresponding console window of display information.
    /// </summary>
    void Clear();
}
