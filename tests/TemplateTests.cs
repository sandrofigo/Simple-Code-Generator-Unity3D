using FluentAssertions;
using SimpleCodeGenerator.Editor;

namespace tests;

[UsesVerify]
public class TemplateTests
{
    private const string TemplateFile1 = "Templates/Test_Template_1.txt";
    private const string TemplateFileImport = "Templates/Test_Template_Import.txt";
    private const string TemplateFileImportPartial = "Templates/Test_Template_Import_Partial.txt";
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
    public Task Render_TemplateWithImportedTemplate_TemplateIsRenderedCorrectly()
    {
        Template template = Template.ParseFromFile(TemplateFileImport);
        template.Import("Template_To_Import", Template.ParseFromFile(TemplateFileImportPartial));
        template.Import("This_Can_Be_Any_Key", Template.Parse("1234567890"));

        var data = new
        {
            Template = "Import",
            SomeNumbers = new[] { 1, 2, 3, 4, 5 }
        };

        string result = template.Render(data);

        var settings = new VerifySettings();
        settings.UseDirectory("Verify");
        return Verify(result, settings);
    }
}