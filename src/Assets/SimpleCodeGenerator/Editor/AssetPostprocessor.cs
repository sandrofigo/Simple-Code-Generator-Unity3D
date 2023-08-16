using System.Linq;

namespace SimpleCodeGenerator.Editor
{
    public class AssetPostprocessor : UnityEditor.AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var allChangedAssets = importedAssets.Concat(deletedAssets).Concat(movedAssets).Concat(movedAssets);

            bool relevantFilesChanged = allChangedAssets.Any(s =>
                s.Contains(".cs") ||
                s.Contains(".txt") ||
                s.Contains(".json"));

            if (relevantFilesChanged)
                CodeGenerator.GenerateAll();
        }
    }
}