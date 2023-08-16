using FluentAssertions;
using SimpleCodeGenerator.Editor;

namespace tests;

[UsesVerify]
public class TemplateTests
{
    [Fact]
    public void ParseFromFile_ParseValidTemplate_TemplateIsParsedWithoutExceptions()
    {
        Action act = () => Template.ParseFromFile("Test_Template_1.txt");

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
        Template template = Template.ParseFromFile("Test_Template_1.txt");

        var data = new
        {
        };

        Action act = () => template.Render(data);

        act.Should().NotThrow();
    }

    [Fact]
    public Task Render_RenderValidTemplate_TemplateIsRenderedCorrectly()
    {
        Template template = Template.ParseFromFile("Test_Template_1.txt");

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
}