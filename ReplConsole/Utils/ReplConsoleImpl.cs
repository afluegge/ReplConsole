namespace ReplConsole.Utils;

[UsedImplicitly]
[SupportedOSPlatform("windows")]
[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
[UnsupportedOSPlatform("linux")]
[UnsupportedOSPlatform("macos")]
internal class ReplConsoleImpl : IReplConsole
{
    public string Title
    {
        get => Console.Title;
        set => Console.Title = value;
    }


    public int            Read()                           => Console.Read();
    public ConsoleKeyInfo ReadKey()                        => Console.ReadKey(false);
    public ConsoleKeyInfo ReadKey(bool intercept)          => Console.ReadKey(intercept);
    public string?        ReadLine()                       => Console.ReadLine();

    public void           Clear()                          => Console.Clear();
    
    public void Write(string text)                         => Console.Write(text);
    public void Write(string text, ConsoleColor color)     => Console.Write(text.Pastel(color));
    public void Write(string text, Color color)            => Console.Write(text.Pastel(color));

    public void WriteLine(string line)                     => Console.WriteLine(line);
    public void WriteLine(string line, ConsoleColor color) => Console.WriteLine(line.Pastel(color));
    public void WriteLine(string line, Color color)        => Console.WriteLine(line.Pastel(color));
    public void WriteLine()                                => Console.WriteLine();

    public void WriteWarning(string error)                 => Console.WriteLine(error.Pastel(ConsoleColor.Yellow));

    public void WriteError(string error)                   => Console.WriteLine(error.Pastel(ConsoleColor.Red));

    public void WriteError(Exception ex)
    {
        Console.WriteLine($"[{ex.GetType().Name}] {ex.Message}".Pastel(ConsoleColor.Red));

        if (!string.IsNullOrWhiteSpace(ex.StackTrace))
            Console.WriteLine($"{ex.StackTrace}\n".Pastel(ConsoleColor.Red));
    }
}
