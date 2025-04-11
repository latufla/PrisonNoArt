using Honeylab.Utils.Data;
using Honeylab.Utils.Logging;
using System.Linq;
using UnityEditor;


namespace Honeylab.Utils.Editor
{
    public class ScriptableIdAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string importedAsset in importedAssets)
            {
                GenerateNewGuidForPathIfNeeded(importedAsset);
            }

            foreach (string movedAsset in movedAssets)
            {
                GenerateNewGuidForPathIfNeeded(movedAsset);
            }
        }


        private static void GenerateNewGuidForPathIfNeeded(string assetName)
        {
            ScriptableId idForGeneration = AssetDatabase.LoadAssetAtPath<ScriptableId>(assetName);
            if (idForGeneration == null)
            {
                return;
            }

            var otherIds = AssetDatabase.FindAssets($"t:{nameof(ScriptableId)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ScriptableId>)
                .Where(id => !ReferenceEquals(id, idForGeneration));
            if (otherIds.Any(otherId => otherId.Equals(idForGeneration)))
            {
                TypeTaggedLogger.SelfLogWarning<ScriptableIdAssetPostprocessor>(
                    $"Found ID duplicate for {assetName}. Generating new GUID.");
                idForGeneration.GenerateNewGuid();
                EditorUtility.SetDirty(idForGeneration);
            }
        }
    }
}
