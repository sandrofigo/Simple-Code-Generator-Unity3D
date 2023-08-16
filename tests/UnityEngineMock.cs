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
        public static string dataPath = "";
    }

    public static class AssetDatabase
    {
        public static void Refresh()
        {
        }

        public static Object LoadAssetAtPath(string s, Type t)
        {
            return new Object();
        }

        public static string GetAssetPath(Object o)
        {
            return "";
        }
    }

    public class TextAsset : Object
    {
    }

    public class Object
    {
    }
}