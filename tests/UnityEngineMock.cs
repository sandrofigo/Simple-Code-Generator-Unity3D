namespace UnityEditor
{
    public class AssetPostprocessor
    {
    }
}

namespace UnityEngine
{
    public class MenuItem : Attribute
    {
        public MenuItem(string s, bool b, int i)
        {
        }
    }

    public static class Debug
    {
        public static void LogWarning(string s)
        {
        }

        public static void LogError(string s)
        {
        }
    }

    public class Application
    {
        public static string dataPath = ".";
    }

    public static class AssetDatabase
    {
        public static void Refresh()
        {
        }

        public static Object LoadAssetAtPath(string s, Type t)
        {
            if (s.StartsWith("Assets/"))
                s = s.Remove(0, "Assets/".Length);

            var mockAsset = Activator.CreateInstance(t) as IMockAsset;

            mockAsset.Path = s;

            return (Object)mockAsset;
        }

        public static string GetAssetPath(Object o)
        {
            var mockAsset = o as IMockAsset;

            return mockAsset.Path;
        }
    }

    public class TextAsset : Object, IMockAsset
    {
        public string Path { get; set; }
    }

    public class Object
    {
    }

    public interface IMockAsset
    {
        public string Path { get; set; }
    }
}