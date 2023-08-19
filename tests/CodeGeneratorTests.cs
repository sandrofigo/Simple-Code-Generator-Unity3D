using FluentAssertions;
using SimpleCodeGenerator.Editor;

namespace tests;

[UsesVerify]
public class CodeGeneratorTests
{
    [Fact]
    public Task Render_RenderBuiltInStringDictionaryTemplate_TemplateIsRenderedCorrectly()
    {
        var values = new StringDictionaryItem[]
        {
            new("Apple", "Fruit", "An apple is a fruit."),
            new("Banana", "Fruit", "A banana is a fruit."),
            new("Potato", "Vegetable", "A potato is a vegetable.")
        };

        CodeGenerator.GenerateStringDictionary(values, "TestNamespace", "TestClass", "result.txt");

        string result = File.ReadAllText("result.txt");

        var settings = new VerifySettings();
        settings.UseDirectory("Verify");
        return Verify(result, settings);
    }

    [Fact]
    public Task Render_RenderBuiltInEnumTemplate_TemplateIsRenderedCorrectly()
    {
        string[] values =
        {
            "Hello",
            "World",
            "1Apple",
            "2 Bananas"
        };

        CodeGenerator.GenerateEnum(values, "TestNamespace", "TestEnum", "result.txt");

        string result = File.ReadAllText("result.txt");

        var settings = new VerifySettings();
        settings.UseDirectory("Verify");
        return Verify(result, settings);
    }

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
        string result = CodeGenerator.SanitizeStringForVariableName(input);

        result.Should().Be(expected);
    }
}