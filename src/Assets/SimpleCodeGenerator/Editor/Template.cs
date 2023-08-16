using System.IO;

namespace SimpleCodeGenerator.Editor
{
    public class Template
    {
        private readonly string[] content;

        private Template(string[] content)
        {
            this.content = content;
        }

        public static Template ParseFromFile(AbsolutePath path)
        {
            return new Template(File.ReadAllLines(path));
        }

        public string Render(object data)
        {
            return new IntermediateTemplate(content, data)
                .RenderStandaloneValues()
                .RenderForLoops()
                .Build();
        }
    }
}