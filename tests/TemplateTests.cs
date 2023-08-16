using FluentAssertions;
using SimpleCodeGenerator.Editor;

namespace tests;

public class TemplateTests
{
    [Fact]
    public void ParseFromFile_ParseValidTemplate_TemplateIsParsedWithoutErrors()
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
}