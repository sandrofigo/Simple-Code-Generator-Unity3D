using System.Collections.Generic;
using SimpleCodeGenerator.Editor;
using UnityEditor;

namespace Samples.Editor
{
    public static class Samples
    {
        [MenuItem("Code Generation/Generate Sample String Dictionary")]
        [CodeGenerationMethod]
        public static void GenerateSampleStringDictionary()
        {
            var values = new List<StringDictionaryItem>
            {
                new("Hello", "World", "Hello World!"),
                new("Text", "Hello World!", "A short text."),
                new("Some Key", "Some Value", "A summary.")
            };
            CodeGenerator.GenerateStringDictionary(values, "Generated", "StringDictionary", "Samples/Generated/StringDictionary.generated.cs");
        }
        
        [MenuItem("Code Generation/Generate Sample Enum")]
        [CodeGenerationMethod]
        public static void GenerateSampleEnum()
        {
            var values = new List<string>
            {
                "Hello",
                "World",
                "OtherValue"
            };
            CodeGenerator.GenerateEnum(values, "Generated", "Enum", "Samples/Generated/Enum.generated.cs");
        }

        [MenuItem("Code Generation/Generate Template_1")]
        [CodeGenerationMethod]
        public static void GenerateTemplate1()
        {
            var data = new
            {
                Fox = new
                {
                    Color = "brown"
                },
                Activity = "jumps",
                Values = new[] { "gray", "brown", "black", "white", "purple" }
            };

            CodeGenerator.GenerateFromTemplate("Samples/Editor/Templates/Template_1.txt", "Samples/Generated/Template_1.generated.cs", data);
        }
    }
}