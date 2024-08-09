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

using System.Drawing;
using ReplConsole.Utils;
using Xunit.Abstractions;

namespace ReplConsole.UnitTests.Utils;

[SupportedOSPlatform("windows")]
[UnsupportedOSPlatform("android")]
[UnsupportedOSPlatform("browser")]
[UnsupportedOSPlatform("ios")]
[UnsupportedOSPlatform("tvos")]
[UnsupportedOSPlatform("linux")]
[UnsupportedOSPlatform("macos")]
public class ConsoleExtensionsTests
{
    public class ForegroundColor(ITestOutputHelper output)
    {
        [Theory]
        [InlineData(3, 40, 255, "", "\x1b[38;2;3;40;255m\x1b[0m")]
        [InlineData(1, 1, 1, "input", "\x1b[38;2;1;1;1minput\x1b[0m")]
        [InlineData(44, 70, 125, "input", "\x1b[38;2;44;70;125minput\x1b[0m")]
        public void Given_Specified_RGB_Color_And_Input_String_Should_Return_Specified_String(int red, int green, int blue, string inputString, string expectedAnsiColorString)
        {
            var outputAnsiColorString = inputString.Pastel(Color.FromArgb(red, green, blue));

            Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
        }

        [Theory]
        [InlineData("#0328ff", "", "\x1b[38;2;3;40;255m\x1b[0m")]
        [InlineData("#010101", "input", "\x1b[38;2;1;1;1minput\x1b[0m")]
        [InlineData("#DDDDDD", "input", "\x1b[38;2;221;221;221minput\x1b[0m")]
        [InlineData("#dDdDdD", "input", "\x1b[38;2;221;221;221minput\x1b[0m")]
        [InlineData("#C2985D", "input", "\x1b[38;2;194;152;93minput\x1b[0m")]
        [InlineData("#aaaaaa", "input", "\x1b[38;2;170;170;170minput\x1b[0m")]
        public void Given_Specified_Hex_Color_String_And_Input_String_Should_Return_Specified_String(string hexColor, string inputString, string expectedAnsiColorString)
        {
            var outputAnsiColorString = inputString.Pastel(hexColor);

            Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
        }


        [Theory]
        [InlineData("0328ff", "", "\x1b[38;2;3;40;255m\x1b[0m")]
        [InlineData("010101", "input", "\x1b[38;2;1;1;1minput\x1b[0m")]
        [InlineData("DDDDDD", "input", "\x1b[38;2;221;221;221minput\x1b[0m")]
        [InlineData("dDdDdD", "input", "\x1b[38;2;221;221;221minput\x1b[0m")]
        [InlineData("C2985D", "input", "\x1b[38;2;194;152;93minput\x1b[0m")]
        [InlineData("aaaaaa", "input", "\x1b[38;2;170;170;170minput\x1b[0m")]
        public void Given_Specified_Hex_Color_String_Without_A_Leading_Number_Sign_And_Input_String_Should_Return_Specified_String(string hexColor, string inputString, string expectedAnsiColorString)
        {
            var actualAnsiColorString = inputString.Pastel(hexColor);

            output.WriteLine($"hexColor: [{hexColor}]  --  inputString: [{inputString}]  --  expectedAnsiColorString: [{expectedAnsiColorString}]  --  actualAnsiColorString: [{actualAnsiColorString}]");

            actualAnsiColorString.Should().Be(expectedAnsiColorString);
        }

        [Fact]
        public void A_Given_Hex_Color_String_Should_Return_Same_Ansi_Output_String_Irrespective_Of_Being_Preceded_By_A_Number_Sign()
        {
            const string inputString = "input";
            const string hexColor = "010101";
            var hexColorWithLeadingNumberSign = $"#{hexColor}";

            Assert.NotEqual(hexColor, hexColorWithLeadingNumberSign);
            Assert.Equal(hexColor, hexColorWithLeadingNumberSign.Substring(1));

            var outputAnsiColorString1 = inputString.Pastel(hexColor);
            var outputAnsiColorString2 = inputString.Pastel(hexColorWithLeadingNumberSign);

            Assert.Equal(outputAnsiColorString1, outputAnsiColorString2);
        }

        [Theory]
        [InlineData("ababab")]
        [InlineData("ABaBaB")]
        [InlineData("aBaBaB")]
        [InlineData("aBaBAB")]
        [InlineData("ABAbab")]
        [InlineData("abaBAB")]
        [InlineData("ABABAB")]
        public void A_Given_Hex_Color_String_Should_Return_Same_Ansi_Output_String_Irrespective_Of_Case(string hexColor)
        {
            const string inputString = "input";
            const string expectedAnsiColorString = "\x1b[38;2;171;171;171minput\x1b[0m";


            var outputAnsiColorString = inputString.Pastel(hexColor);


            Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
        }

        [Theory]
        [InlineData(ConsoleColor.Black, "", "\x1b[38;2;0;0;0m\x1b[0m")]
        [InlineData(ConsoleColor.Red, "input", "\x1b[38;2;255;0;0minput\x1b[0m")]
        [InlineData(ConsoleColor.Green, "input", "\x1b[38;2;0;128;0minput\x1b[0m")]
        [InlineData(ConsoleColor.Blue, "input", "\x1b[38;2;0;0;255minput\x1b[0m")]
        public void Given_Specified_ConsoleColor_And_Input_String_Should_Return_Specified_String(ConsoleColor consoleColor, string inputString, string expectedAnsiColorString)
        {
            // Act
            var result = inputString.Pastel(consoleColor);

            // Assert
            result.Should().Be(expectedAnsiColorString);
        }
    }


    public class BackgroundColor(ITestOutputHelper output)
    {
        [Theory]
        [InlineData(3, 40, 255, "", "\x1b[48;2;3;40;255m\x1b[0m")]
        [InlineData(1, 1, 1, "input", "\x1b[48;2;1;1;1minput\x1b[0m")]
        [InlineData(44, 70, 125, "input", "\x1b[48;2;44;70;125minput\x1b[0m")]
        public void Given_Specified_RGB_Color_And_Input_String_Should_Return_Specified_String(int red, int green, int blue, string inputString, string expectedAnsiColorString)
        {
            var outputAnsiColorString = inputString.PastelBg(Color.FromArgb(red, green, blue));

            Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
        }

        [Theory]
        [InlineData("#0328ff", "", "\x1b[48;2;3;40;255m\x1b[0m")]
        [InlineData("#010101", "input", "\x1b[48;2;1;1;1minput\x1b[0m")]
        [InlineData("#DDDDDD", "input", "\x1b[48;2;221;221;221minput\x1b[0m")]
        [InlineData("#dDdDdD", "input", "\x1b[48;2;221;221;221minput\x1b[0m")]
        [InlineData("#C2985D", "input", "\x1b[48;2;194;152;93minput\x1b[0m")]
        [InlineData("#aaaaaa", "input", "\x1b[48;2;170;170;170minput\x1b[0m")]
        public void Given_Specified_Hex_Color_String_And_Input_String_Should_Return_Specified_String(string hexColor, string inputString, string expectedAnsiColorString)
        {
            var outputAnsiColorString = inputString.PastelBg(hexColor);

            Assert.Equal(expectedAnsiColorString, outputAnsiColorString);
        }

        [Theory]
        [InlineData("0328ff", "", "\x1b[38;2;3;40;255m\x1b[0m")]
        [InlineData("010101", "input", "\u001b[38;2;1;1;1minput\u001b[0m")]
        [InlineData("DDDDDD", "input", "\x1b[38;2;221;221;221minput\x1b[0m")]
        [InlineData("dDdDdD", "input", "\x1b[38;2;221;221;221minput\x1b[0m")]
        [InlineData("C2985D", "input", "\x1b[38;2;194;152;93minput\x1b[0m")]
        [InlineData("aaaaaa", "input", "\x1b[38;2;170;170;170minput\x1b[0m")]
        public void Given_Specified_Hex_Color_String_Without_A_Leading_Number_Sign_And_Input_String_Should_Return_Specified_String(string hexColor, string inputString, string expectedAnsiColorString)
        {
            var actualAnsiColorString = inputString.Pastel(hexColor);

            output.WriteLine($"hexColor: [{hexColor}]  --  inputString: [{inputString}]  --  expectedAnsiColorString: [{expectedAnsiColorString}]  --  actualAnsiColorString: [{actualAnsiColorString}]");

            actualAnsiColorString.Should().Be(expectedAnsiColorString);
        }

        [Theory]
        [InlineData(ConsoleColor.Black, "", "\x1b[48;2;0;0;0m\x1b[0m")]
        [InlineData(ConsoleColor.Red, "input", "\x1b[48;2;255;0;0minput\x1b[0m")]
        [InlineData(ConsoleColor.Green, "input", "\x1b[48;2;0;128;0minput\x1b[0m")]
        [InlineData(ConsoleColor.Blue, "input", "\x1b[48;2;0;0;255minput\x1b[0m")]
        public void Given_Specified_ConsoleColor_And_Input_String_Should_Return_Specified_String(ConsoleColor consoleColor, string inputString, string expectedAnsiColorString)
        {
            // Act
            var result = inputString.PastelBg(consoleColor);

            // Assert
            result.Should().Be(expectedAnsiColorString);
        }
        
        [Fact]
        public void A_Given_Hex_Color_String_Should_Return_Same_Ansi_Output_String_Irrespective_Of_Being_Preceded_By_A_Number_Sign()
        {
            const string inputString = "input";
            const string hexColor = "010101";

            var outputAnsiColorString1 = inputString.PastelBg(hexColor);
            var outputAnsiColorString2 = inputString.PastelBg($"#{hexColor}");

            Assert.Equal(outputAnsiColorString1, outputAnsiColorString2);
        }
    }


    public class NestedColor
    {
        [Fact]
        public void A_Nested_Color_String_That_Ends_With_Reset_Should_Not_Add_An_Extra_Reset_Escape_Sequence_1()
        {
            const int red1 = 1, green1 = 1, blue1 = 1;
            const int red2 = 2, green2 = 2, blue2 = 2;

            const string expected = "\u001b[38;2;1;1;1mSTART\u001b[0m\u001b[38;2;2;2;2mb\u001b[0m\u001b[0m";
            
            var output = $"START{"b".Pastel(Color.FromArgb(red2, green2, blue2))}".Pastel(Color.FromArgb(red1, green1, blue1));

            output.Should().Be(expected);
        }

        [Fact]
        public void A_Nested_Color_String_That_Ends_With_Reset_Should_Not_Add_An_Extra_Reset_Escape_Sequence_2()
        {
            const int red1 = 1, green1 = 1, blue1 = 1;
            const int red2 = 2, green2 = 2, blue2 = 2;

            const string expected = "\u001b[48;2;1;1;1mSTART\u001b[0m\u001b[48;2;2;2;2mb\u001b[0m\u001b[0m";

            var output = $"START{"b".PastelBg(Color.FromArgb(red2, green2, blue2))}".PastelBg(Color.FromArgb(red1, green1, blue1));

            output.Should().Be(expected);
        }

        [Fact]
        public void A_Nested_Color_String_That_Ends_With_Reset_Should_Not_Add_An_Extra_Reset_Escape_Sequence_3()
        {
            const int red1 = 1, green1 = 1, blue1 = 1;
            const int red2 = 2, green2 = 2, blue2 = 2;

            const string expected = "\u001b[48;2;1;1;1mSTART\u001b[0m\u001b[48;2;1;1;1m\u001b[38;2;2;2;2mb\u001b[0m\u001b[0m";

            var output = $"START{"b".Pastel(Color.FromArgb(red2, green2, blue2))}".PastelBg(Color.FromArgb(red1, green1, blue1));

            output.Should().Be(expected);
        }

        [Fact]
        public void A_Nested_Color_String_That_Ends_With_Reset_Should_Not_Add_An_Extra_Reset_Escape_Sequence_4()
        {
            const int red1 = 1, green1 = 1, blue1 = 1;
            const int red2 = 2, green2 = 2, blue2 = 2;

            const string expected = "\u001b[38;2;1;1;1mSTART\u001b[0m\u001b[38;2;1;1;1m\u001b[48;2;2;2;2mb\u001b[0m\u001b[0m";

            var output = $"START{"b".PastelBg(Color.FromArgb(red2, green2, blue2))}".Pastel(Color.FromArgb(red1, green1, blue1));

            output.Should().Be(expected);
        }

        [Fact]
        public void A_Nested_Color_String_Must_Be_Correctly_Closed_1()
        {
            const int red1 = 1, green1 = 1, blue1 = 1;
            const int red2 = 2, green2 = 2, blue2 = 2;

            const string expected = "\u001b[38;2;1;1;1ma\u001b[0m\u001b[38;2;2;2;2mb\u001b[0m\u001b[38;2;1;1;1mc\u001b[0m";

            var output = $"a{"b".Pastel(Color.FromArgb(red2, green2, blue2))}c".Pastel(Color.FromArgb(red1, green1, blue1));

            output.Should().Be(expected);
        }

        [Fact]
        public void A_Nested_Color_String_Must_Be_Correctly_Closed_2()
        {
            const int red1 = 1, green1 = 1, blue1 = 1;
            const int red2 = 2, green2 = 2, blue2 = 2;

            const string expected = "\u001b[48;2;1;1;1ma\u001b[0m\u001b[48;2;2;2;2mb\u001b[0m\u001b[48;2;1;1;1mc\u001b[0m";

            var output = $"a{"b".PastelBg(Color.FromArgb(red2, green2, blue2))}c".PastelBg(Color.FromArgb(red1, green1, blue1));

            output.Should().Be(expected);
        }

        [Fact]
        public void A_Nested_Color_String_Must_Be_Correctly_Closed_3()
        {
            const int red1 = 1, green1 = 1, blue1 = 1;
            const int red2 = 2, green2 = 2, blue2 = 2;

            const string expected = "\u001b[48;2;1;1;1ma\u001b[0m\u001b[48;2;1;1;1m\u001b[38;2;2;2;2mb\u001b[0m\u001b[48;2;1;1;1mc\u001b[0m";

            var output = $"a{"b".Pastel(Color.FromArgb(red2, green2, blue2))}c".PastelBg(Color.FromArgb(red1, green1, blue1));

            output.Should().Be(expected);
        }

        [Fact]
        public void A_Nested_Color_String_Must_Be_Correctly_Closed_4()
        {
            const int red1 = 1, green1 = 1, blue1 = 1;
            const int red2 = 2, green2 = 2, blue2 = 2;

            const string expected = "\u001b[38;2;1;1;1ma\u001b[0m\u001b[38;2;1;1;1m\u001b[48;2;2;2;2mb\u001b[0m\u001b[38;2;1;1;1mc\u001b[0m";

            var output = $"a{"b".PastelBg(Color.FromArgb(red2, green2, blue2))}c".Pastel(Color.FromArgb(red1, green1, blue1));

            output.Should().Be(expected);
        }

        [Fact]
        public void A_Foreground_And_Background_Nested_Color_String_Must_Be_Correctly_Closed()
        {
            const string expected = "\u001b[38;2;255;20;147mSTART_\u001b[0m\u001b[38;2;255;20;147m\u001b[48;2;220;20;60m\u001b[38;2;255;255;0m[TEST1]\u001b[0m\u001b[38;2;255;20;147m____\u001b[0m\u001b[38;2;255;20;147m\u001b[48;2;220;20;60m\u001b[38;2;255;255;0m[TEST2]\u001b[0m\u001b[38;2;255;20;147m_END\u001b[0m";
            var outputAnsiColorString = $"{$"START_{"[TEST1]".Pastel(Color.Yellow).PastelBg(Color.Crimson)}____{"[TEST2]".Pastel(Color.Yellow).PastelBg(Color.Crimson)}_END".Pastel(Color.DeepPink)}";

            outputAnsiColorString.Should().Be(expected);
        }

        [InlineData("\x1b[0m", "\u001b[38;2;0;128;0mSTART_\u001b[0m\u001b[38;2;255;0;0m[TEST1]\u001b[0m\u001b[38;2;0;128;0m____\u001b[0m\u001b[38;2;0;0;255m[TEST2]\u001b[0m\u001b[38;2;0;128;0m____\u001b[0m\u001b[38;2;0;128;0m____END\u001b[0m")]
        [InlineData("\x1b[0m\x1b[0m", "\u001b[38;2;0;128;0mSTART_\u001b[0m\u001b[38;2;255;0;0m[TEST1]\u001b[0m\u001b[38;2;0;128;0m____\u001b[0m\u001b[38;2;0;0;255m[TEST2]\u001b[0m\u001b[38;2;0;128;0m____\u001b[0m\u001b[38;2;0;128;0m____END\u001b[0m")]
        [InlineData("\x1b[0m\x1b[0m\x1b[0m", "\u001b[38;2;0;128;0mSTART_\u001b[0m\u001b[38;2;255;0;0m[TEST1]\u001b[0m\u001b[38;2;0;128;0m____\u001b[0m\u001b[38;2;0;0;255m[TEST2]\u001b[0m\u001b[38;2;0;128;0m____\u001b[0m\u001b[38;2;0;128;0m____END\u001b[0m")]
        [Theory]
        public void A_Foreground_And_Background_Nested_Color_String_Containing_Valid_Reset_Escape_Sequences_Must_Be_Correctly_Closed(string validEscapeSequence, string expected)
        {
            var outputAnsiColorString = $"{$"START_{"[TEST1]".Pastel(Color.Red)}____{"[TEST2]".Pastel(Color.Blue)}____{validEscapeSequence}____END".Pastel(Color.Green)}";

            outputAnsiColorString.Should().Be(expected);
        }

        [InlineData("\x1b[38")]
        [InlineData("\x1b[48")]
        [Theory]
        public void A_Foreground_And_Background_Nested_Color_String_Containing_Valid_Color_Escape_Sequences_Must_Be_Correctly_Closed(string validEscapeSequence)
        {
            var expected              = $"\u001b[38;2;0;128;0mSTART____\u001b[0m\u001b[38;2;255;0;0m[TEST1]\u001b[0m\u001b[38;2;0;128;0m______\u001b[0m\u001b[38;2;0;0;255m[TEST2]\u001b[0m\u001b[38;2;0;128;0m____{validEscapeSequence}____END\u001b[0m";
            var outputAnsiColorString = $"{$"START____{"[TEST1]".Pastel(Color.Red)}______{"[TEST2]".Pastel(Color.Blue)}____{validEscapeSequence}____END".Pastel(Color.Green)}";

            outputAnsiColorString.Should().Be(expected);
        }

        [InlineData("\x1b")]
        [InlineData("\x1b[")]

        [InlineData("x[38")]
        [InlineData("\x1bx38")]
        [InlineData("\x1b[x8")]
        [InlineData("\x1b[3x")]

        [InlineData("x[48")]
        [InlineData("\x1bx48")]
        [InlineData("\x1b[4x")]

        [InlineData("x[0m")]
        [InlineData("\x1bx0m")]
        [InlineData("\x1b[xm")]
        [InlineData("\x1b[0x")]
        [Theory]
        public void A_Foreground_And_Background_Nested_Color_String_Containing_Invalid_Escape_Sequences_Must_Be_Correctly_Closed(string invalidEscapeSequence)
        {
            var expected = $"\u001b[38;2;0;128;0mSTART__\u001b[0m\u001b[38;2;255;0;0m[TEST1]\u001b[0m\u001b[38;2;0;128;0m____\u001b[0m\u001b[38;2;0;128;0m\u001b[48;2;0;0;255m[TEST2]\u001b[0m\u001b[38;2;0;128;0m____{invalidEscapeSequence}____END\u001b[0m";

            var outputAnsiColorString = $"{$"START__{"[TEST1]".Pastel(Color.Red)}____{"[TEST2]".PastelBg(Color.Blue)}____{invalidEscapeSequence}____END".Pastel(Color.Green)}";

            outputAnsiColorString.Should().Be(expected);
        }
    }
}
