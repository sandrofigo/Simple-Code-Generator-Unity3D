using System.IO;

namespace SimpleCodeGenerator.Editor
{
    public readonly struct AbsolutePath
    {
        private readonly string path;

        private AbsolutePath(string path)
        {
            this.path = Sanitize(path);
        }

        public static implicit operator string(AbsolutePath p) => p.path;
        public static implicit operator AbsolutePath(string s) => new(s);

        public static AbsolutePath operator /(AbsolutePath a, string b)
        {
            return new AbsolutePath(a.path + "/" + b);
        }

        public static AbsolutePath operator /(string a, AbsolutePath b)
        {
            return new AbsolutePath(a + "/" + b.path);
        }

        public static string operator -(AbsolutePath a, AbsolutePath b)
        {
            return a.path.StartsWith(b.path) ? a.path[b.path.Length..].TrimStart('/') : a;
        }

        private static string Sanitize(string path)
        {
            string fullPath = Path.GetFullPath(path);
            string normalizedPath = fullPath.Replace(@"\", "/").TrimEnd('/');
            return normalizedPath;
        }

        public override string ToString()
        {
            return path;
        }
    }
}