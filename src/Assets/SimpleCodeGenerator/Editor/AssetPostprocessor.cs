using System.Linq;

namespace SimpleCodeGenerator.Editor
{
    public class AssetPostprocessor : UnityEditor.AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var allChangedAssets = importedAssets.Concat(deletedAssets).Concat(movedAssets).Concat(movedAssets);

            if (allChangedAssets.Any(s =>
                    s.Contains(".cs") ||
                    s.Contains(".json")))
                CodeGenerator.GenerateAll();
        }
    }
}