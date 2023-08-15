using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

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
                .SelectMany(t => t.GetMethods())
                .Where(m => Attribute.GetCustomAttribute(m, typeof(CodeGenerationMethod)) != null)
                .ToArray();

            foreach (MethodInfo methodInfo in methods)
            {
                methodInfo.Invoke(null, null);
            }

#if SF_SIMPLE_CODE_GENERATOR_DEBUG
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Code generation took {stopwatch.Elapsed.TotalSeconds:0.00}s");
#endif
        }
    }
}