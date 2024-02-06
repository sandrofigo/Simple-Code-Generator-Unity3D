using FluentAssertions;
using SimpleCodeGenerator.Core;
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

    [Fact]
    public Task GenerateFromTemplate_RenderFromValidTemplatePath_TemplateIsRenderedCorrectly()
    {
        var data = new
        {
            TemplateNumber = 1,
            SomeNumbers = new[] { 1, 3, 4, 2, 5 }
        };

        CodeGenerator.GenerateFromTemplate("Templates/Test_Template_1.txt", "result.txt", data);

        string result = File.ReadAllText("result.txt");

        var settings = new VerifySettings();
        settings.UseDirectory("Verify");
        return Verify(result, settings);
    }

    [Fact]
    public Task GenerateFromTemplate_RenderFromValidTemplateInstance_TemplateIsRenderedCorrectly()
    {
        var data = new
        {
            TemplateNumber = 1,
            SomeNumbers = new[] { 1, 3, 4, 2, 5 }
        };

        Template.TryFindTemplateInAssets("Templates/Test_Template_1.txt", out Template t);

        CodeGenerator.GenerateFromTemplate(t, "result.txt", data);

        string result = File.ReadAllText("result.txt");

        var settings = new VerifySettings();
        settings.UseDirectory("Verify");
        return Verify(result, settings);
    }
}