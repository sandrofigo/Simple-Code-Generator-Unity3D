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
                new("Hello", "World", "Hello World!")
            };
            CodeGenerator.GenerateStringDictionary(values, "MyNamespace", "MyClass", "Samples/Generated");
        }
    }
}