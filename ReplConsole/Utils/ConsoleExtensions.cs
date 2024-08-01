// Ported and modified from: https://github.com/silkfire/Pastel
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

#pragma warning disable S3963
#pragma warning disable S3263

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
    private const string Kernel32DllName = "kernel32";

    private const int  STD_OUTPUT_HANDLE                  = -11;
    private const uint ENABLE_PROCESSED_OUTPUT            = 0x0001;
    private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;


    /// <summary>
    /// Gets the current input mode of a console's input buffer or the current output mode of a console screen buffer.
    /// </summary>
    /// <remarks>
    /// The GetConsoleMode function retrieves the current input mode of a console's input buffer or the current output mode of a console screen buffer.
    /// </remarks>
    /// <param name="hConsoleHandle">
    /// A handle to the console input buffer or the console screen buffer. The handle must have the <see langword="GENERIC_READ"/> access right.
    /// </param>
    /// <param name="lpMode">
    /// A pointer to a variable that receives the current mode of the specified buffer.
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is <see langword="true"/>.
    /// If the function fails, the return value is <see langword="false"/>.
    /// </returns>
    [LibraryImport(Kernel32DllName, EntryPoint = "GetConsoleMode")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetConsoleMode(nint hConsoleHandle, out uint lpMode);

    /// <summary>
    /// Sets the input mode of a console's input buffer or the output mode of a console screen buffer.
    /// </summary>
    /// <remarks>
    /// The <paramref name="hConsoleHandle"/> parameter can be any of the following values: STD_INPUT_HANDLE, STD_OUTPUT_HANDLE, or STD_ERROR_HANDLE. 
    /// The <paramref name="dwMode"/> parameter can be one or more of the following values: ENABLE_ECHO_INPUT, ENABLE_INSERT_MODE, ENABLE_LINE_INPUT, ENABLE_MOUSE_INPUT, ENABLE_PROCESSED_INPUT, ENABLE_QUICK_EDIT_MODE, ENABLE_WINDOW_INPUT, ENABLE_VIRTUAL_TERMINAL_INPUT, ENABLE_PROCESSED_OUTPUT, ENABLE_WRAP_AT_EOL_OUTPUT.
    /// </remarks>
    /// <param name="hConsoleHandle">A handle to the console input buffer or the console screen buffer.</param>
    /// <param name="dwMode">The input or output mode to be set. The mode can be one or more of the following values.</param>
    /// <returns>
    /// If the function succeeds, the return value is true. If the function fails, the return value is false. 
    /// To get extended error information, call GetLastError.
    /// </returns>
    [LibraryImport(Kernel32DllName, EntryPoint = "SetConsoleMode")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleMode(nint hConsoleHandle, uint dwMode);

    /// <summary>
    /// Retrieves a handle to the specified standard device.
    /// </summary>
    /// <remarks>
    /// This function is a P/Invoke declaration for the 'GetStdHandle' function from the 'kernel32.dll' library. 
    /// It is used to retrieve a handle to the specified standard device (standard input, standard output, or standard error).
    /// </remarks>
    /// <param name="nStdHandle">The standard device for which to get a handle.</param>
    /// <returns>A handle to the specified device, or INVALID_HANDLE_VALUE if an error occurs.</returns>
    [LibraryImport(Kernel32DllName, EntryPoint = "GetStdHandle", SetLastError = true)]
    private static partial nint GetStdHandle(int nStdHandle);


    private static bool _enabled;

    private static readonly ReadOnlyDictionary<ConsoleColor, Color> _consoleColorMapper = new(new Dictionary<ConsoleColor, Color>
    {
        [ConsoleColor.Black]       = Color.FromArgb(0x000000),
        [ConsoleColor.DarkBlue]    = Color.FromArgb(0x00008B),
        [ConsoleColor.DarkGreen]   = Color.FromArgb(0x006400),
        [ConsoleColor.DarkCyan]    = Color.FromArgb(0x008B8B),
        [ConsoleColor.DarkRed]     = Color.FromArgb(0x8B0000),
        [ConsoleColor.DarkMagenta] = Color.FromArgb(0x8B008B),
        [ConsoleColor.DarkYellow]  = Color.FromArgb(0x808000),
        [ConsoleColor.Gray]        = Color.FromArgb(0x808080),
        [ConsoleColor.DarkGray]    = Color.FromArgb(0xA9A9A9),
        [ConsoleColor.Blue]        = Color.FromArgb(0x0000FF),
        [ConsoleColor.Green]       = Color.FromArgb(0x008000),
        [ConsoleColor.Cyan]        = Color.FromArgb(0x00FFFF),
        [ConsoleColor.Red]         = Color.FromArgb(0xFF0000),
        [ConsoleColor.Magenta]     = Color.FromArgb(0xFF00FF),
        [ConsoleColor.Yellow]      = Color.FromArgb(0xFFFF00),
        [ConsoleColor.White]       = Color.FromArgb(0xFFFFFF)
    });

    private delegate string ColorFormat(string input, Color color);
    private delegate string HexColorFormat(string input, string hexColor);

    private enum ColorPlane : byte
    {
        Foreground,
        Background
    }

    private const string _formatStringStart   = "\u001b[{0};2;";
    private const string _formatStringColor   = "{1};{2};{3}m";
    private const string _formatStringContent = "{4}";
    private const string _formatStringEnd     = "\u001b[0m";
    private const string _formatStringPartial = $"{_formatStringStart}{_formatStringColor}";
    private const string _formatStringFull    = $"{_formatStringStart}{_formatStringColor}{_formatStringContent}{_formatStringEnd}";

    private static readonly ReadOnlyDictionary<ColorPlane, string> _planeFormatModifiers = new(new Dictionary<ColorPlane, string>
    {
        [ColorPlane.Foreground] = "38",
        [ColorPlane.Background] = "48"
    });

    /// <summary>
    /// Generates a regular expression pattern for closing nested pastel strings.
    /// </summary>
    /// <remarks>
    /// This method uses the <c>GeneratedRegex</c> attribute to generate a regular expression pattern that matches one or more occurrences of
    /// the ANSI escape sequence for resetting console color. The generated pattern is used to close any nested pastel strings in the console output.
    /// </remarks>
    /// <returns>
    /// A <see cref="Regex"/> object containing the generated regular expression pattern.
    /// </returns>
    [GeneratedRegex("(?:\u001b\\[0m)+")]
    private static partial Regex CloseNestedPastelStringRegex1();

    /// <summary>
    /// Generates a regular expression pattern for closing nested Pastel strings.
    /// </summary>
    /// <remarks>
    /// This method uses the <c langword="GeneratedRegex"/> attribute to generate a regular expression pattern. The pattern is designed to match sequences
    /// that should be closed in nested Pastel strings. The pattern specifically targets sequences that are not at the start of the string, are not preceded
    /// by an ANSI reset sequence, and are not preceded by an ANSI color sequence.
    /// </remarks>
    /// <returns>
    /// A <see cref="Regex"/> object encapsulating the generated pattern.
    /// </returns>
    [GeneratedRegex("(?<!^)(?<!\u001b\\[0m)(?<!\u001b\\[(?:38|48);2;\\d{1,3};\\d{1,3};\\d{1,3}m)(?:\u001b\\[(?:38|48);2;)")]
    private static partial Regex CloseNestedPastelStringRegex2();

    /// <summary>
    /// Generates a regular expression for closing nested pastel strings in the foreground.
    /// </summary>
    /// <remarks>
    /// The regular expression pattern matches the escape sequence for resetting the console color,
    /// but not followed by the start of another color sequence or the end of the string.
    /// </remarks>
    /// <returns>
    /// A <see cref="Regex"/> object that matches the closing sequence of a nested pastel string in the foreground.
    /// </returns>
    [GeneratedRegex("(?:\u001b\\[0m)(?!\u001b\\[38;2;)(?!$)")]
    private static partial Regex CloseNestedPastelStringRegex3Foreground();

    /// <summary>
    /// Generates a regular expression for closing nested pastel strings for the background color.
    /// </summary>
    /// <remarks>
    /// This method is part of the <see cref="ConsoleExtensions"/> class and is used for handling console color output. 
    /// It generates a regular expression that matches the ANSI escape sequence for resetting the background color, 
    /// but does not match the sequence for setting a 24-bit background color or the end of a string.
    /// </remarks>
    /// <returns>
    /// A <see cref="System.Text.RegularExpressions.Regex"/> object.
    /// </returns>
    [GeneratedRegex("(?:\u001b\\[0m)(?!\u001b\\[48;2;)(?!$)")]
    private static partial Regex CloseNestedPastelStringRegex3Background();


    /// <summary>
    /// Parses a hexadecimal color string to an integer.
    /// </summary>
    /// <remarks>
    /// This function is used to convert a hexadecimal color string (e.g., "#FFFFFF" for white) into an integer. The '#' character is optional.
    /// </remarks>
    /// <value>
    /// A function that takes a hexadecimal color string and returns its integer representation.
    /// </value>
    private static readonly Func<string, int> _parseHexColor = hc => int.Parse(hc.Replace("#", ""), NumberStyles.HexNumber);


    /// <summary>
    /// Formats a string with color codes.
    /// </summary>
    /// <remarks>
    /// This function formats the input string with color codes for console output. The color is specified by the <paramref name="c"/> parameter and
    /// can be applied to either the foreground or the background, as specified by the <paramref name="p"/> parameter.
    /// </remarks>
    /// <param name="i">The input string to be formatted.</param>
    /// <param name="c">The <see cref="Color"/> to be applied.</param>
    /// <param name="p">The <see cref="ColorPlane"/> (foreground or background) to which the color should be applied.</param>
    /// <returns>A string formatted with color codes.</returns>
    private static readonly Func<string, Color, ColorPlane, string> _colorFormat = (i, c, p) => string.Format(_formatStringFull, _planeFormatModifiers[p], c.R, c.G, c.B, CloseNestedPastelStrings(i, c, p));

    /// <summary>
    /// Formats a string with color codes derived from a hexadecimal color string.
    /// </summary>
    /// <remarks>
    /// This function formats the input string with color codes for console output. The color is specified by the <paramref name="c"/> parameter as a hexadecimal string and
    /// can be applied to either the foreground or the background, as specified by the <paramref name="p"/> parameter.
    /// </remarks>
    /// <param name="i">The input string to be formatted.</param>
    /// <param name="c">The hexadecimal color string to be applied.</param>
    /// <param name="p">The <see cref="ColorPlane"/> (foreground or background) to which the color should be applied.</param>
    /// <returns>A string formatted with color codes.</returns>
    private static readonly Func<string, string, ColorPlane, string> _colorHexFormat = (i, c, p) => _colorFormat(i, Color.FromArgb(_parseHexColor(c)), p);


    /// <summary>
    /// Defines a format for colorless output.
    /// </summary>
    /// <remarks>
    /// This <see langword="ColorFormat"/> delegate is used when no color output is required. It takes two parameters: a string and a <see cref="Color"/> instance. The color parameter is ignored, and the original string is returned unchanged.
    /// </remarks>
    private static readonly ColorFormat _noColorOutputFormat = (i, _) => i;

    /// <summary>
    /// Defines a format for handling hexadecimal color output.
    /// </summary>
    /// <remarks>
    /// This format ignores the hexadecimal color value and returns the input string as is. It is used when no color output is required.
    /// </remarks>
    /// <param name="i">The input string to be formatted.</param>
    /// <param name="_">The hexadecimal color value. This parameter is ignored in this format.</param>
    /// <returns>The original input string without any color formatting.</returns>
    private static readonly HexColorFormat _noHexColorOutputFormat = (i, _) => i;


    /// <summary>
    /// Applies a specified color to the foreground of an input string.
    /// </summary>
    /// <remarks>
    /// This delegate instance uses the <see cref="Func{T1,T2,T3,TResult}"/> `_colorFormat` to apply the specified color to the foreground of the input string.
    /// The color is applied using console color codes.
    /// </remarks>
    /// <param name="i">The input string to be formatted.</param>
    /// <param name="c">The <see cref="System.Drawing.Color"/> to be applied to the foreground of the input string.</param>
    /// <returns>A string with the specified color applied to its foreground.</returns>
    private static readonly ColorFormat _foregroundColorFormat = (i, c) => _colorFormat(i, c, ColorPlane.Foreground);

    /// <summary>
    /// Formats a string with foreground color codes derived from a hexadecimal color string.
    /// </summary>
    /// <remarks>
    /// This function formats the input string with color codes for console output. The color is specified by the <paramref name="c"/> parameter as a hexadecimal string and
    /// is applied to the foreground.
    /// </remarks>
    /// <param name="i">The input string to be formatted.</param>
    /// <param name="c">The hexadecimal color string to be applied to the foreground.</param>
    /// <returns>A string formatted with foreground color codes.</returns>
    private static readonly HexColorFormat _foregroundHexColorFormat = (i, c) => _colorHexFormat(i, c, ColorPlane.Foreground);


    /// <summary>
    /// Defines a method that formats a string with background color codes.
    /// </summary>
    /// <remarks>
    /// This method uses the <see cref="ColorFormat"/> delegate to format the input string with color codes for console output. The color is specified by the <paramref name="c"/> parameter and is applied to the background.
    /// </remarks>
    /// <param name="i">The input string to be formatted.</param>
    /// <param name="c">The <see cref="Color"/> to be applied to the background.</param>
    /// <returns>A string formatted with background color codes.</returns>
    private static readonly ColorFormat _backgroundColorFormat = (i, c) => _colorFormat(i, c, ColorPlane.Background);

    /// <summary>
    /// Formats a string with background color codes derived from a hexadecimal color string.
    /// </summary>
    /// <remarks>
    /// This function formats the input string with background color codes for console output. The color is specified by the <paramref name="c"/> parameter as a hexadecimal string.
    /// </remarks>
    /// <param name="i">The input string to be formatted.</param>
    /// <param name="c">The hexadecimal color string to be applied as the background color.</param>
    /// <returns>A string formatted with background color codes.</returns>
    private static readonly HexColorFormat _backgroundHexColorFormat = (i, c) => _colorHexFormat(i, c, ColorPlane.Background);

    
    /// <summary>
    /// Initializes a read-only dictionary of regular expressions for closing nested pastel strings.
    /// </summary>
    /// <remarks>
    /// This dictionary contains two entries, one for the foreground and one for the background. 
    /// Each entry is a regular expression generated by the <see cref="CloseNestedPastelStringRegex3Foreground"/> and <see cref="CloseNestedPastelStringRegex3Background"/> methods respectively.
    /// These regular expressions are used to close nested pastel strings in the console output.
    /// </remarks>
    private static readonly ReadOnlyDictionary<ColorPlane, Regex> _closeNestedPastelStringRegex3 = new(new Dictionary<ColorPlane, Regex>
    {
        [ColorPlane.Foreground] = CloseNestedPastelStringRegex3Foreground(),
        [ColorPlane.Background] = CloseNestedPastelStringRegex3Background()
    });

    /// <summary>
    /// Defines a mapping of color formatting functions based on the color output state and the color plane.
    /// </summary>
    /// <remarks>
    /// This is a <see cref="ReadOnlyDictionary{TKey, TValue}"/> where the key is a <see langword="bool"/> representing the color output state (enabled or disabled),
    /// and the value is another <see cref="ReadOnlyDictionary{TKey, TValue}"/>. The inner dictionary's key is a <see cref="ColorPlane"/> (Foreground or Background),
    /// and the value is a <see cref="ColorFormat"/> delegate that formats the color output.
    /// When color output is disabled (<see langword="false"/>), the <see cref="ColorFormat"/> delegates return the original string without any color formatting.
    /// When color output is enabled (<see langword="true"/>), the delegates return the string formatted with the specified foreground or background color.
    /// </remarks>
    private static readonly ReadOnlyDictionary<bool, ReadOnlyDictionary<ColorPlane, ColorFormat>> _colorFormatFunctions = new(new Dictionary<bool, ReadOnlyDictionary<ColorPlane, ColorFormat>>
    {
        [false] = new(new Dictionary<ColorPlane, ColorFormat>
        {
            [ColorPlane.Foreground] = _noColorOutputFormat,
            [ColorPlane.Background] = _noColorOutputFormat
        }),
        [true] = new(new Dictionary<ColorPlane, ColorFormat>
        {
            [ColorPlane.Foreground] = _foregroundColorFormat,
            [ColorPlane.Background] = _backgroundColorFormat
        })
    });

    private static readonly ReadOnlyDictionary<bool, ReadOnlyDictionary<ColorPlane, HexColorFormat>> _hexColorFormatFunctions = new(new Dictionary<bool, ReadOnlyDictionary<ColorPlane, HexColorFormat>>
    {
        [false] = new(new Dictionary<ColorPlane, HexColorFormat>
        {
            [ColorPlane.Foreground] = _noHexColorOutputFormat,
            [ColorPlane.Background] = _noHexColorOutputFormat
        }),
        [true] = new(new Dictionary<ColorPlane, HexColorFormat>
        {
            [ColorPlane.Foreground] = _foregroundHexColorFormat,
            [ColorPlane.Background] = _backgroundHexColorFormat
        })
    });


    /// <summary>
    /// Initializes the <see cref="ConsoleExtensions"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor checks if the operating system is Windows. If it is, it gets the standard output handle and sets the console mode to enable processed output and virtual terminal processing.
    /// It also checks if the "NO_COLOR" environment variable is set. If it is not set, it enables console color output. Otherwise, it disables console color output.
    /// </remarks>
    static ConsoleExtensions()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);

            _ = GetConsoleMode(iStdOut, out var outConsoleMode) && SetConsoleMode(iStdOut, outConsoleMode | ENABLE_PROCESSED_OUTPUT | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
        }

        if (Environment.GetEnvironmentVariable("NO_COLOR") == null)
            Enable();
        else
            Disable();
    }


    /// <summary>
    /// Enables any future console color output produced by Pastel.
    /// </summary>
    public static void Enable()
    {
        _enabled = true;
    }

    /// <summary>
    /// Disables any future console color output produced by Pastel.
    /// </summary>
    public static void Disable()
    {
        _enabled = false;
    }


    /// <summary>
    /// Returns a string wrapped in an ANSI foreground color code using the specified color.
    /// </summary>
    /// <param name="input">The string to color.</param>
    /// <param name="color">The color to use on the specified string.</param>
    public static string Pastel(this string input, Color color)
    {
        return _colorFormatFunctions[_enabled][ColorPlane.Foreground](input, color);
    }

    /// <summary>
    /// Returns a string wrapped in an ANSI foreground color code using the specified color.
    /// </summary>
    /// <param name="input">The string to color.</param>
    /// <param name="color">The color to use on the specified string.</param>
    public static string Pastel(this string input, ConsoleColor color)
    {
        return Pastel(input, _consoleColorMapper[color]);
    }

    /// <summary>
    /// Returns a string wrapped in an ANSI foreground color code using the specified color.
    /// </summary>
    /// <param name="input">The string to color.</param>
    /// <param name="hexColor">The color to use on the specified string.<para>Supported format: [#]RRGGBB.</para></param>
    public static string Pastel(this string input, string hexColor)
    {
        return _hexColorFormatFunctions[_enabled][ColorPlane.Foreground](input, hexColor);
    }



    /// <summary>
    /// Returns a string wrapped in an ANSI background color code using the specified color.
    /// </summary>
    /// <param name="input">The string to color.</param>
    /// <param name="color">The color to use on the specified string.</param>
    public static string PastelBg(this string input, Color color)
    {
        return _colorFormatFunctions[_enabled][ColorPlane.Background](input, color);
    }

    /// <summary>
    /// Returns a string wrapped in an ANSI background color code using the specified color.
    /// </summary>
    /// <param name="input">The string to color.</param>
    /// <param name="color">The color to use on the specified string.</param>
    public static string PastelBg(this string input, ConsoleColor color)
    {
        return PastelBg(input, _consoleColorMapper[color]);
    }

    /// <summary>
    /// Returns a string wrapped in an ANSI background color code using the specified color.
    /// </summary>
    /// <param name="input">The string to color.</param>
    /// <param name="hexColor">The color to use on the specified string.<para>Supported format: [#]RRGGBB.</para></param>
    public static string PastelBg(this string input, string hexColor)
    {
        return _hexColorFormatFunctions[_enabled][ColorPlane.Background](input, hexColor);
    }



    /// <summary>
    /// Closes nested pastel strings in the console output.
    /// </summary>
    /// <param name="input">The input string containing potential nested pastel strings.</param>
    /// <param name="color">The color used in the pastel strings.</param>
    /// <param name="colorPlane">The color plane (foreground or background) of the pastel strings.</param>
    /// <returns>A string with closed nested pastel strings.</returns>
    /// <remarks>
    /// This method is used internally to ensure that nested pastel strings are properly closed. It uses regular expressions to identify and close any open pastel strings in the input.
    /// </remarks>
    private static string CloseNestedPastelStrings(string input, Color color, ColorPlane colorPlane)
    {
        var closedString = CloseNestedPastelStringRegex1().Replace(input, _formatStringEnd);

        closedString = CloseNestedPastelStringRegex2().Replace(closedString, $"{_formatStringEnd}$0");
        closedString = _closeNestedPastelStringRegex3[colorPlane].Replace(closedString, $"$0{string.Format(_formatStringPartial, _planeFormatModifiers[colorPlane], color.R, color.G, color.B)}");

        return closedString;
    }
}
