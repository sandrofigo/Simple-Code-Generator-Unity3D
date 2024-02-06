using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using SimpleCodeGenerator.Core;
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
                valueArray[i].Key = TextProcessing.SanitizeStringForVariableName(valueArray[i].Key);
                valueArray[i].Summary = TextProcessing.EscapeSummaryText(valueArray[i].Summary);
            }

            var template = Template.FindBuiltInTemplate("StringDictionary");

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

        public static void GenerateEnum(IEnumerable<string> values, string namespaceName, string enumName, string outputAssetPath)
        {
            string[] valueArray = values as string[] ?? values.ToArray();

            if (!valueArray.Any())
            {
                Debug.LogWarning($"Skipped code generation for '{namespaceName}.{enumName}', because no values for the enum were provided");
                return;
            }

            for (int i = 0; i < valueArray.Length; i++)
            {
                valueArray[i] = TextProcessing.SanitizeStringForVariableName(valueArray[i]);
            }

            var template = Template.FindBuiltInTemplate("Enum");

            var data = new
            {
                TemplateFile = Path.GetFileNameWithoutExtension(GetRelativePathToBuiltInTemplate("Enum")),
                Namespace = namespaceName,
                Enum = enumName,
                Values = valueArray
            };

            RenderTemplateToFile(template, data, outputAssetPath);
        }

        public static void GenerateFromTemplate(Template template, string outputAssetPath, object data)
        {
            RenderTemplateToFile(template, data, outputAssetPath);
        }

        public static void GenerateFromTemplate(string templateAssetPath, string outputAssetPath, object data)
        {
            if (Template.TryFindTemplateInAssets(templateAssetPath, out Template template))
            {
                GenerateFromTemplate(template, outputAssetPath, data);
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

        private static string GetRelativePathToBuiltInTemplate(string templateName)
        {
            return Template.GetAbsolutePathToBuiltInTemplate(templateName) - Application.dataPath;
        }
    }
}