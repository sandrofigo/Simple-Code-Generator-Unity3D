using System.Runtime.CompilerServices;

namespace SimpleCodeGenerator.Editor
{
    public static class CurrentFile
    {
        public static AbsolutePath Path([CallerFilePath] string callerFilePath = null) => callerFilePath;
        public static AbsolutePath Directory([CallerFilePath] string callerFilePath = null) => System.IO.Path.GetDirectoryName(callerFilePath);
    }
}