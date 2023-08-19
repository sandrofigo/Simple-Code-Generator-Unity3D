using System.IO;
using UnityEditor;
using UnityEngine;

namespace SimpleCodeGenerator.Editor
{
    public class Template
    {
        private readonly string[] content;

        private Template(string[] content)
        {
            this.content = content;
        }

        public static Template ParseFromFile(string path)
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

        internal static Template FindBuiltInTemplate(string templateName)
        {
            return ParseFromFile(GetAbsolutePathToBuiltInTemplate(templateName));
        }

        public static bool TryFindTemplateInAssets(string templateAssetPath, out Template template)
        {
            if (!templateAssetPath.StartsWith("Assets/"))
                templateAssetPath = $"Assets/{templateAssetPath}";

            var textAsset = (TextAsset)AssetDatabase.LoadAssetAtPath(templateAssetPath, typeof(TextAsset));

            if (textAsset == null)
            {
                template = null;
                return false;
            }

            template = ParseFromFile(AssetDatabase.GetAssetPath(textAsset));

            return true;
        }

        private static AbsolutePath GetPathToBuiltInTemplates()
        {
            return CurrentFile.Directory() / "Templates";
        }

        internal static AbsolutePath GetAbsolutePathToBuiltInTemplate(string templateName)
        {
            return GetPathToBuiltInTemplates() / $"{templateName}.txt";
        }
    }
}