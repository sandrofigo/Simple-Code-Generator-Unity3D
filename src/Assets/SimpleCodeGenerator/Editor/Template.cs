using System;
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
            // TODO-SFIGO: WIP

            for (int i = 0; i < content.Length; i++)
            {
                content[i] = "// " + content[i];
            }

            return string.Join(Environment.NewLine, content);
        }
    }
}