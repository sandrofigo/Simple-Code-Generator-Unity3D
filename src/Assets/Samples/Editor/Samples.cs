using SimpleCodeGenerator.Editor;
using UnityEditor;
using UnityEngine;

namespace Samples.Editor
{
    public static class Samples
    {
        [MenuItem("Code Generation/Generate Some Code")]
        [CodeGenerationMethod]
        public static void GenerateSomeCode()
        {
            Debug.Log("Hello World!");
        }
    }
}