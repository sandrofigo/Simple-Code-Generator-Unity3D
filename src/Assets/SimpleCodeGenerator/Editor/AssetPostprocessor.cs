using System.Linq;

namespace SimpleCodeGenerator.Editor
{
    public class AssetPostprocessor : UnityEditor.AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            string[] allChangedAssets = importedAssets.Concat(deletedAssets).Concat(movedAssets).Concat(movedAssets).ToArray();

            bool relevantFilesHaveChanged = allChangedAssets.Any(s =>
                s.EndsWith(".cs") ||
                s.EndsWith(".txt") ||
                s.EndsWith(".json"));

            bool onlyGeneratedFilesHaveChanged = allChangedAssets.All(s =>
                s.Contains(".g.") ||
                s.Contains(".generated."));

            if (relevantFilesHaveChanged && !onlyGeneratedFilesHaveChanged)
                CodeGenerator.GenerateAll();
        }
    }
}