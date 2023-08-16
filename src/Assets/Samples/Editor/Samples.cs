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
            CodeGenerator.GenerateStringDictionary(values, "MyNamespace", "MyClass", "Samples/Generated");
        }
    }
}