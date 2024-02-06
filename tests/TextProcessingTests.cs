using FluentAssertions;
using SimpleCodeGenerator.Core;
using SimpleCodeGenerator.Editor;

namespace tests;

public class TextProcessingTests
{
    [Theory]
    [InlineData("test", "test")]
    [InlineData("test123", "test123")]
    [InlineData("1test", "_1test")]
    [InlineData("1 test", "_1test")]
    [InlineData("hello world", "helloworld")]
    [InlineData(" hello world", "helloworld")]
    [InlineData(" hello   world", "helloworld")]
    [InlineData(" hello 1 world", "hello1world")]
    [InlineData(" ", "")]
    [InlineData("   ", "")]
    [InlineData("/test", "test")]
    [InlineData("test%$&§!?`+-#", "test")]
    public void SanitizeStringForVariableName_SanitizeStrings_StringAreSanitizedCorrectly(string input, string expected)
    {
        string result = TextProcessing.SanitizeStringForVariableName(input);

        result.Should().Be(expected);
    }
}