using JetBrains.Annotations;
using SimpleCodeGenerator.Editor;
using UnityEngine;

namespace Samples.Editor
{
    public static class Samples
    {
        [CodeGenerationMethod, UsedImplicitly]
        public static void GenerateSomeCode()
        {
            Debug.Log("Hello World!");
        }
    }
}