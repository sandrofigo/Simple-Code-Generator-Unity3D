using System.Collections.Generic;
using SimpleCodeGenerator.Editor;
using UnityEditor;

namespace Samples.Editor
{
    public static class Samples
    {
        [MenuItem("Code Generation/Generate Some Code")]
        [CodeGenerationMethod]
        public static void GenerateSomeCode()
        {
            var values = new List<StringDictionaryItem>
            {
                new("Hello", "World", "Hello World!"),
                new("Hello2", "World2", "Hello World!2"),
                new("Hello3", "World3", "Hello World!3"),
            };
            CodeGenerator.GenerateStringDictionary(values, "MyNamespace", "MyClass", "Samples/Generated/MyClass.generated.cs");
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