using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        public static void GenerateStringDictionary(IEnumerable<StringDictionaryItem> values, string namespaceName, string className, string outputDirectory)
        {
            var valueArray = values as StringDictionaryItem[] ?? values.ToArray();

            if (!valueArray.Any())
            {
                Debug.LogWarning($"Skipped code generation for '{namespaceName}.{className}', because no values for the dictionary were provided");
                return;
            }

            Template template = FindTemplate("StringDictionary");

            string result = template.Render(new
            {
                templateFile = GetRelativePathToTemplate("StringDictionary"),
                @namespace = namespaceName,
                @class = className,
                values = valueArray,
                count = valueArray.Length,
                nestedTest = new
                {
                    innerString = "test string",
                    innerTest = new
                    {
                        mostInnerString = "hello!"
                    }
                }
            });

            AbsolutePath outputPath = (AbsolutePath)Application.dataPath / outputDirectory / $"{className}.generated.cs";

            if (!FileHasSameContent(outputPath, result))
                WriteTextToAsset(result, outputPath);
        }

        private static void WriteTextToAsset(string contents, string filePath)
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

        private static AbsolutePath GetPathToBuiltInTemplates([CallerFilePath] string callerFilePath = null)
        {
            var pathToCurrentDirectory = (AbsolutePath)Path.GetDirectoryName(callerFilePath);

            return pathToCurrentDirectory / "Templates";
        }

        private static AbsolutePath GetAbsolutePathToTemplate(string templateName)
        {
            return GetPathToBuiltInTemplates() / $"{templateName}.txt";
        }
        
        private static string GetRelativePathToTemplate(string templateName)
        {
            return GetAbsolutePathToTemplate(templateName) - Application.dataPath;
        }

        private static Template FindTemplate(string templateName)
        {
            return Template.ParseFromFile(GetAbsolutePathToTemplate(templateName));
        }

        private static string SanitizeStringForVariableName(string input)
        {
            string variableName = Regex.Replace(input, "[^0-9A-Za-z_]", string.Empty);

            if (int.TryParse(variableName[0].ToString(), out int _))
            {
                variableName = $"_{variableName}";
            }

            return variableName;
        }

        private static string EscapeSummaryText(string text)
        {
            return $"<![CDATA[{text}]]>";
        }
    }
}