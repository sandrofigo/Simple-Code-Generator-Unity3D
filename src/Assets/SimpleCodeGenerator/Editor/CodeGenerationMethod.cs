using System;

namespace SimpleCodeGenerator.Editor
{
    /// <summary>
    /// This attribute is used to mark methods for the <see cref="CodeGenerator"/> in order for them to be picked called during code generation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CodeGenerationMethod : Attribute
    {
    }
}