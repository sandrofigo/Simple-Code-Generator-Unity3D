using FluentAssertions;
using SimpleCodeGenerator.Core;

namespace tests;

public class AbsolutePathTests
{
    [Fact]
    public void Constructor_AbsolutePathFromRelativePath_AbsolutePathIsCorrect()
    {
        var result = (AbsolutePath)".";

        result.ToString().Should().Be(Path.GetFullPath(".").Replace('\\', '/'));
    }

    [Theory]
    [InlineData("test", "/test")]
    [InlineData("/test", "/test")]
    [InlineData("//test", "/test")]
    [InlineData("///test", "/test")]
    [InlineData("/test/123", "/test/123")]
    [InlineData("/test/123/", "/test/123")]
    public void DivisionOperator_AbsolutePathIsJoinedWithString_CreatesCorrectAbsolutePath(string input, string expected)
    {
        AbsolutePath result = (AbsolutePath)"." / input;

        result.ToString().Should().Be(Path.GetFullPath(".").Replace('\\', '/') + expected);
    }
}