using FluentAssertions;
using SimpleCodeGenerator.Editor;

namespace tests;

[UsesVerify]
public class TemplateTests
{
    private const string TemplateFile1 = "Templates/Test_Template_1.txt";
    private const string StringDictionaryTemplateFile = "Templates/StringDictionary.txt";
    private const string EnumTemplateFile = "Templates/Enum.txt";

    [Fact]
    public void ParseFromFile_ParseValidTemplate_TemplateIsParsedWithoutExceptions()
    {
        Action act = () => Template.ParseFromFile(TemplateFile1);

        act.Should().NotThrow();
    }

    [Fact]
    public void ParseFromFile_ParseNonExistingFile_ThrowsException()
    {
        Action act = () => Template.ParseFromFile("Test_Template_That_Does_Not_Exist.txt");

        act.Should().Throw<FileNotFoundException>();
    }

    [Fact]
    public void Render_RenderValidTemplate_TemplateIsRenderedWithoutExceptions()
    {
        Template template = Template.ParseFromFile(TemplateFile1);

        var data = new
        {
        };

        Action act = () => template.Render(data);

        act.Should().NotThrow();
    }

    [Fact]
    public Task Render_RenderValidTemplate1_TemplateIsRenderedCorrectly()
    {
        Template template = Template.ParseFromFile(TemplateFile1);

        var data = new
        {
            TemplateNumber = 1,
            SomeNumbers = new[] { 1, 3, 4, 2, 5 }
        };

        string result = template.Render(data);

        var settings = new VerifySettings();
        settings.UseDirectory("Verify");
        return Verify(result, settings);
    }

    [Fact]
    public Task Render_RenderValidTemplate2_TemplateIsRenderedCorrectly()
    {
        Template template = Template.ParseFromFile(StringDictionaryTemplateFile);

        var data = new
        {
            TemplateFile = StringDictionaryTemplateFile,
            Namespace = "TestNamespace",
            Class = "TestClass",
            Values = new StringDictionaryItem[]
            {
                new("Apple", "Fruit", "An apple is a fruit."),
                new("Banana", "Fruit", "A banana is a fruit."),
                new("Potato", "Vegetable", "A potato is a vegetable.")
            },
            Count = 3
        };

        string result = template.Render(data);

        var settings = new VerifySettings();
        settings.UseDirectory("Verify");
        return Verify(result, settings);
    }

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