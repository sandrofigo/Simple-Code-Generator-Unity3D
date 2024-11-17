using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using SimpleCodeGenerator.Core;
using UnityEditor;
using UnityEngine;

namespace SimpleCodeGenerator.Editor
{
    public class Template
    {
        private readonly string[] content;
        private readonly Dictionary<string, Template> importedTemplates = new();

        private Template(string[] content)
        {
            this.content = content;
        }

        [PublicAPI]
        public static Template ParseFromFile(string path)
        {
            return new Template(File.ReadAllLines(path));
        }

        [PublicAPI]
        public static Template ParseFromLines(string[] lines)
        {
            return new Template(lines);
        }

        [PublicAPI]
        public static Template Parse(string contentWithLineBreaks)
        {
            return ParseFromLines(contentWithLineBreaks.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));
        }

        [PublicAPI]
        public string Render(object data)
        {
            return new IntermediateTemplate(content, data)
                .RenderImports()
                .RenderStandaloneValues()
                .RenderForLoops()
                .Build();
        }

        internal static Template FindBuiltInTemplate(string templateName)
        {
            return ParseFromFile(GetAbsolutePathToBuiltInTemplate(templateName));
        }

        [PublicAPI]
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

        [PublicAPI]
        public Template Import(string key, Template template)
        {
            importedTemplates.Add(key, template);
            return this;
        }
    }
}