using ReplConsole.Utils.WindowsConsole;

namespace ReplConsole.Utils;

[UsedImplicitly]
[SupportedOSPlatform("windows")]
[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
[UnsupportedOSPlatform("linux")]
[UnsupportedOSPlatform("macos")]
internal class ReplConsoleImpl(IConsole console) : IReplConsole
{
    public string Title
    {
        get => console.Title;
        set => console.Title = value;
    }


    public int            Read()                           => console.Read();
    public ConsoleKeyInfo ReadKey()                        => console.ReadKey(false);
    public ConsoleKeyInfo ReadKey(bool intercept)          => console.ReadKey(intercept);
    public string?        ReadLine()                       => console.ReadLine();

    public void           Clear()                          => console.Clear();
    
    public void Write(string text)                         => console.Write(text);
    public void Write(string text, ConsoleColor color)     => console.Write(text.Pastel(color));
    public void Write(string text, Color color)            => console.Write(text.Pastel(color));

    public void WriteLine(string line)                     => console.WriteLine(line);
    public void WriteLine(string line, ConsoleColor color) => console.WriteLine(line.Pastel(color));
    public void WriteLine(string line, Color color)        => console.WriteLine(line.Pastel(color));
    public void WriteLine()                                => console.WriteLine();

    public void WriteWarning(string error)                 => console.WriteLine(error.Pastel(ConsoleColor.Yellow));

    public void WriteError(string error)                   => console.WriteLine(error.Pastel(ConsoleColor.Red));

    public void WriteError(Exception ex)
    {
        console.WriteLine($"[{ex.GetType().Name}] {ex.Message}".Pastel(ConsoleColor.Red));

        if (!string.IsNullOrWhiteSpace(ex.StackTrace))
            console.WriteLine($"{ex.StackTrace}\n".Pastel(ConsoleColor.Red));
    }
}
