// Ported, modified and fixed from: https://github.com/silkfire/Pastel
// by Andreas Flügge
//
// MIT License
//
// Copyright (c) 2018 Gabriel Bider
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

[assembly: System.Runtime.CompilerServices.DisableRuntimeMarshalling]

namespace ReplConsole.Utils;

/// <summary>
/// Provides a set of static methods for console color manipulation.
/// </summary>
/// <remarks>
/// This class contains methods for enabling and disabling console color output, as well as methods for applying colors to strings that are output to the console.
/// The colors can be specified using <see cref="Color"/>, <see cref="ConsoleColor"/>, or hexadecimal color codes.
/// </remarks>
[PublicAPI]
[SupportedOSPlatform("windows")]
[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
[UnsupportedOSPlatform("linux")]
[UnsupportedOSPlatform("macos")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "InvalidXmlDocComment")]
public static partial class ConsoleExtensions
{
    private enum ColorPlane : byte
    {
        Foreground,
        Background
    }
    

    [GeneratedRegex("(?:\u001b\\[0m)+")]
    private static partial Regex CloseNestedPastelStringRegex1();

    [GeneratedRegex("(?<!^)(?<!\u001b\\[0m)(?<!\u001b\\[(?:38|48);2;\\d{1,3};\\d{1,3};\\d{1,3}m)(?:\u001b\\[(?:38|48);2;)")]
    private static partial Regex CloseNestedPastelStringRegex2();

    [GeneratedRegex("(?:\u001b\\[0m)(?!\u001b\\[38;2;)(?!$)")]
    private static partial Regex CloseNestedPastelStringRegex3Foreground();

    [GeneratedRegex("(?:\u001b\\[0m)(?!\u001b\\[48;2;)(?!$)")]
    private static partial Regex CloseNestedPastelStringRegex3Background();
    

    private const string _formatStringStart = "\u001b[{0};2;";
    private const string _formatStringColor = "{1};{2};{3}m";
    private const string _formatStringContent = "{4}";
    private const string _formatStringEnd = "\u001b[0m";
    private const string _formatStringPartial = $"{_formatStringStart}{_formatStringColor}";
    private const string _formatStringFull = $"{_formatStringStart}{_formatStringColor}{_formatStringContent}{_formatStringEnd}";


    private static readonly ReadOnlyDictionary<ConsoleColor, Color> _consoleColorMapper = new(new Dictionary<ConsoleColor, Color>
    {
        [ConsoleColor.Black] = Color.FromArgb(0x000000),
        [ConsoleColor.DarkBlue] = Color.FromArgb(0x00008B),
        [ConsoleColor.DarkGreen] = Color.FromArgb(0x006400),
        [ConsoleColor.DarkCyan] = Color.FromArgb(0x008B8B),
        [ConsoleColor.DarkRed] = Color.FromArgb(0x8B0000),
        [ConsoleColor.DarkMagenta] = Color.FromArgb(0x8B008B),
        [ConsoleColor.DarkYellow] = Color.FromArgb(0x808000),
        [ConsoleColor.Gray] = Color.FromArgb(0x808080),
        [ConsoleColor.DarkGray] = Color.FromArgb(0xA9A9A9),
        [ConsoleColor.Blue] = Color.FromArgb(0x0000FF),
        [ConsoleColor.Green] = Color.FromArgb(0x008000),
        [ConsoleColor.Cyan] = Color.FromArgb(0x00FFFF),
        [ConsoleColor.Red] = Color.FromArgb(0xFF0000),
        [ConsoleColor.Magenta] = Color.FromArgb(0xFF00FF),
        [ConsoleColor.Yellow] = Color.FromArgb(0xFFFF00),
        [ConsoleColor.White] = Color.FromArgb(0xFFFFFF)
    });

    private static readonly ReadOnlyDictionary<ColorPlane, string> _planeFormatModifiers = new(new Dictionary<ColorPlane, string>
    {
        [ColorPlane.Foreground] = "38",
        [ColorPlane.Background] = "48"
    });

    private static readonly ReadOnlyDictionary<ColorPlane, Regex> _closeNestedPastelStringRegex3 = new(new Dictionary<ColorPlane, Regex>
    {
        [ColorPlane.Foreground] = CloseNestedPastelStringRegex3Foreground(),
        [ColorPlane.Background] = CloseNestedPastelStringRegex3Background()
    });
    
    

    public static string Pastel(this string input, Color color)          => ColorFormat(input, color, ColorPlane.Foreground);
    public static string Pastel(this string input, ConsoleColor color)   => Pastel(input, _consoleColorMapper[color]);
    public static string Pastel(this string input, string hexColor)      => ColorHexFormat(input, hexColor, ColorPlane.Foreground);

    public static string PastelBg(this string input, Color color)        => ColorFormat(input, color, ColorPlane.Background);
    public static string PastelBg(this string input, ConsoleColor color) => PastelBg(input, _consoleColorMapper[color]);
    public static string PastelBg(this string input, string hexColor)    => ColorHexFormat(input, hexColor, ColorPlane.Background);


    private static string ColorFormat(string input, Color color, ColorPlane plane)        => string.Format(_formatStringFull, _planeFormatModifiers[plane], color.R, color.G, color.B, CloseNestedPastelStrings(input, color, plane));
    private static string ColorHexFormat(string input, string hexColor, ColorPlane plane) => ColorFormat(input, Color.FromArgb(ParseHexColor(hexColor)), plane);
    private static int    ParseHexColor(string hexColor)                                  => int.Parse(hexColor.Replace("#", ""), NumberStyles.HexNumber);
    

    private static string CloseNestedPastelStrings(string input, Color color, ColorPlane colorPlane)
    {
        var closedString = CloseNestedPastelStringRegex1().Replace(input, _formatStringEnd);

        closedString = CloseNestedPastelStringRegex2().Replace(closedString, $"{_formatStringEnd}$0");
        closedString = _closeNestedPastelStringRegex3[colorPlane].Replace(closedString, $"$0{string.Format(_formatStringPartial, _planeFormatModifiers[colorPlane], color.R, color.G, color.B)}");

        return closedString;
    }
}
