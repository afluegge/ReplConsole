using System.Text;

namespace ReplConsole.Utils.WindowsConsole;

// This is a straight forward wrapper to the corresponding calls of the .NET 'Console' class.
// This wrapper exists only for making code more testable.
// I see no need for explicitly testing this code.
[ExcludeFromCodeCoverage]

[SupportedOSPlatform("windows")]
[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
[UnsupportedOSPlatform("linux")]
[UnsupportedOSPlatform("macos")]
internal class WindowsConsoleImpl : IConsole
{
    // Properties
    public ConsoleColor BackgroundColor
    {
        get => Console.BackgroundColor;
        set => Console.BackgroundColor = value;
    }
    public int BufferHeight
    {
        get => Console.BufferHeight;
        set => Console.BufferHeight = value;
    }
    public int BufferWidth
    {
        get => Console.BufferWidth;
        set => Console.BufferWidth = value;
    }
    public bool CapsLock => Console.CapsLock;
    public int CursorLeft
    {
        get => Console.CursorLeft;
        set => Console.CursorLeft = value;
    }
    public int CursorSize
    {
        get => Console.CursorSize;
        set => Console.CursorSize = value;
    }
    public int CursorTop
    {
        get => Console.CursorTop;
        set => Console.CursorTop = value;
    }
    public bool CursorVisible
    {
        get => Console.CursorVisible;
        set => Console.CursorVisible = value;
    }
    public TextWriter Error => Console.Error;
    public ConsoleColor ForegroundColor
    {
        get => Console.ForegroundColor;
        set => Console.ForegroundColor = value;
    }
    public TextReader In => Console.In;
    public Encoding InputEncoding
    {
        get => Console.InputEncoding;
        set => Console.InputEncoding = value;
    }
    public bool KeyAvailable => Console.KeyAvailable;
    public int LargestWindowHeight => Console.LargestWindowHeight;
    public int LargestWindowWidth => Console.LargestWindowWidth;
    public bool NumberLock => Console.NumberLock;
    public TextWriter Out => Console.Out;
    public Encoding OutputEncoding
    {
        get => Console.OutputEncoding;
        set => Console.OutputEncoding = value;
    }
    public string Title
    {
        get => Console.Title;
        set => Console.Title = value;
    }
    public bool TreatControlCAsInput
    {
        get => Console.TreatControlCAsInput;
        set => Console.TreatControlCAsInput = value;
    }
    public int WindowHeight
    {
        get => Console.WindowHeight;
        set => Console.WindowHeight = value;
    }
    public int WindowLeft
    {
        get => Console.WindowLeft;
        set => Console.WindowLeft = value;
    }
    public int WindowTop
    {
        get => Console.WindowTop;
        set => Console.WindowTop = value;
    }
    public int WindowWidth
    {
        get => Console.WindowWidth;
        set => Console.WindowWidth = value;
    }

    // Methods
    public void Beep() => Console.Beep();
    public void Beep(int frequency, int duration) => Console.Beep(frequency, duration);
    public void Clear() => Console.Clear();
    public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop) =>
        Console.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop);
    public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop, char sourceChar, ConsoleColor sourceForeColor, ConsoleColor sourceBackColor) =>
        Console.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop, sourceChar, sourceForeColor, sourceBackColor);
    public Stream OpenStandardError() => Console.OpenStandardError();
    public Stream OpenStandardError(int bufferSize) => Console.OpenStandardError(bufferSize);
    public Stream OpenStandardInput() => Console.OpenStandardInput();
    public Stream OpenStandardInput(int bufferSize) => Console.OpenStandardInput(bufferSize);
    public Stream OpenStandardOutput() => Console.OpenStandardOutput();
    public Stream OpenStandardOutput(int bufferSize) => Console.OpenStandardOutput(bufferSize);
    public int Read() => Console.Read();
    public ConsoleKeyInfo ReadKey() => Console.ReadKey();
    public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);
    public string? ReadLine() => Console.ReadLine();
    public void ResetColor() => Console.ResetColor();
    public void SetBufferSize(int width, int height) => Console.SetBufferSize(width, height);
    public void SetCursorPosition(int left, int top) => Console.SetCursorPosition(left, top);
    public void SetError(TextWriter newError) => Console.SetError(newError);
    public void SetIn(TextReader newIn) => Console.SetIn(newIn);
    public void SetOut(TextWriter newOut) => Console.SetOut(newOut);
    public void SetWindowPosition(int left, int top) => Console.SetWindowPosition(left, top);
    public void SetWindowSize(int width, int height) => Console.SetWindowSize(width, height);
    public void Write(bool value) => Console.Write(value);
    public void Write(char value) => Console.Write(value);
    public void Write(char[] buffer) => Console.Write(buffer);
    public void Write(char[] buffer, int index, int count) => Console.Write(buffer, index, count);
    public void Write(decimal value) => Console.Write(value);
    public void Write(double value) => Console.Write(value);
    public void Write(int value) => Console.Write(value);
    public void Write(long value) => Console.Write(value);
    public void Write(object value) => Console.Write(value);
    public void Write(float value) => Console.Write(value);
    public void Write(string value) => Console.Write(value);
    public void Write(string format, object arg0) => Console.Write(format, arg0);
    public void Write(string format, object arg0, object arg1) => Console.Write(format, arg0, arg1);
    public void Write(string format, object arg0, object arg1, object arg2) => Console.Write(format, arg0, arg1, arg2);
    public void Write(string format, params object[] arg) => Console.Write(format, arg);
    public void Write(uint value) => Console.Write(value);
    public void Write(ulong value) => Console.Write(value);
    public void WriteLine() => Console.WriteLine();
    public void WriteLine(bool value) => Console.WriteLine(value);
    public void WriteLine(char value) => Console.WriteLine(value);
    public void WriteLine(char[] buffer) => Console.WriteLine(buffer);
    public void WriteLine(char[] buffer, int index, int count) => Console.WriteLine(buffer, index, count);
    public void WriteLine(decimal value) => Console.WriteLine(value);
    public void WriteLine(double value) => Console.WriteLine(value);
    public void WriteLine(int value) => Console.WriteLine(value);
    public void WriteLine(long value) => Console.WriteLine(value);
    public void WriteLine(object value) => Console.WriteLine(value);
    public void WriteLine(float value) => Console.WriteLine(value);
    public void WriteLine(string value) => Console.WriteLine(value);
    public void WriteLine(string format, object arg0) => Console.WriteLine(format, arg0);
    public void WriteLine(string format, object arg0, object arg1) => Console.WriteLine(format, arg0, arg1);
    public void WriteLine(string format, object arg0, object arg1, object arg2) => Console.WriteLine(format, arg0, arg1, arg2);
    public void WriteLine(string format, params object[] arg) => Console.WriteLine(format, arg);
    public void WriteLine(uint value) => Console.WriteLine(value);
    public void WriteLine(ulong value) => Console.WriteLine(value);
}
