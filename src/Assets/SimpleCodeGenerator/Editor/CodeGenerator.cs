using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SimpleCodeGenerator.Editor
{
    public static class CodeGenerator
    {
        [MenuItem("Code Generation/Generate All", false, 0)]
        public static void GenerateAll()
        {
#if SF_SIMPLE_CODE_GENERATOR_DEBUG
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
#endif

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var methods = assemblies
                .SelectMany(a => a.GetTypes())
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                .Where(m => Attribute.GetCustomAttribute(m, typeof(CodeGenerationMethod)) != null)
                .ToArray();

            foreach (MethodInfo methodInfo in methods)
            {
                methodInfo.Invoke(null, null);
            }

#if SF_SIMPLE_CODE_GENERATOR_DEBUG
            stopwatch.Stop();
            Debug.Log($"Code generation took {stopwatch.Elapsed.TotalSeconds:0.00}s");
#endif
        }

        public static void GenerateStringDictionary(IEnumerable<StringDictionaryItem> values, string namespaceName, string className, string outputAssetPath)
        {
            var valueArray = values as StringDictionaryItem[] ?? values.ToArray();

            if (!valueArray.Any())
            {
                Debug.LogWarning($"Skipped code generation for '{namespaceName}.{className}', because no values for the dictionary were provided");
                return;
            }

            for (int i = 0; i < valueArray.Length; i++)
            {
                valueArray[i].Key = SanitizeStringForVariableName(valueArray[i].Key);
                valueArray[i].Summary = EscapeSummaryText(valueArray[i].Summary);
            }

            Template template = FindBuiltInTemplate("StringDictionary");

            var data = new
            {
                TemplateFile = Path.GetFileNameWithoutExtension(GetRelativePathToBuiltInTemplate("StringDictionary")),
                Namespace = namespaceName,
                Class = className,
                Values = valueArray,
                Count = valueArray.Length
            };

            RenderTemplateToFile(template, data, outputAssetPath);
        }

        public static void GenerateFromTemplate(string templateAssetPath, string outputAssetPath, object data)
        {
            if (TryFindTemplateInAssets(templateAssetPath, out Template template))
            {
                RenderTemplateToFile(template, data, outputAssetPath);
            }
            else
            {
                Debug.LogWarning($"Could not find template '{templateAssetPath}' in assets.");
            }
        }

        private static void RenderTemplateToFile(Template template, object data, string toAssetPath)
        {
            string result = template.Render(data);

            AbsolutePath outputPath = (AbsolutePath)Application.dataPath / toAssetPath;

            if (!FileHasSameContent(outputPath, result))
                WriteTextToFile(result, outputPath);
        }

        private static void WriteTextToFile(string contents, string filePath)
        {
            try
            {
                string path = Path.Combine(Application.dataPath, filePath);
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new ArgumentException("The provided file path is invalid!", nameof(filePath)));
                File.WriteAllText(path, contents);

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError($"An error occurred while writing a file: {e}");
            }
        }

        private static bool FileHasSameContent(string filePath, string contentToCompare)
        {
            return File.Exists(filePath) && File.ReadAllText(filePath) == contentToCompare;
        }

        private static AbsolutePath GetPathToBuiltInTemplates()
        {
            return CurrentFile.Directory() / "Templates";
        }

        private static AbsolutePath GetAbsolutePathToBuiltInTemplate(string templateName)
        {
            return GetPathToBuiltInTemplates() / $"{templateName}.txt";
        }

        private static string GetRelativePathToBuiltInTemplate(string templateName)
        {
            return GetAbsolutePathToBuiltInTemplate(templateName) - Application.dataPath;
        }

        private static Template FindBuiltInTemplate(string templateName)
        {
            return Template.ParseFromFile(GetAbsolutePathToBuiltInTemplate(templateName));
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

            template = Template.ParseFromFile(AssetDatabase.GetAssetPath(textAsset));

            return true;
        }

        public static string SanitizeStringForVariableName(string input)
        {
            string variableName = Regex.Replace(input, "[^0-9A-Za-z_]", string.Empty);

            if (int.TryParse(variableName[0].ToString(), out int _))
            {
                variableName = $"_{variableName}";
            }

            return variableName;
        }

        public static string EscapeSummaryText(string text)
        {
            return $"<![CDATA[{text}]]>";
        }
    }
}