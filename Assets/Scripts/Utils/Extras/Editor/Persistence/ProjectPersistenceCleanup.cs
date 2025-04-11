using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Honeylab.Utils.Persistence.Editor
{
    internal static class ProjectPersistenceCleanup
    {
        [MenuItem("Edit/Clear Persistent Data")]
        private static void ClearPersistentData()
        {
            foreach (PersistenceData data in EnumerateLevelPersistentFilePathsData())
            {
                string path = string.Concat(data.GetFilePath(), "_shared", ".", data.GetExtension());
                File.Delete(path);

                path = string.Concat(data.GetFilePath(), "_levels_0", ".", data.GetExtension());
                File.Delete(path);

                path = string.Concat(data.GetFilePath(), "_levels_1", ".", data.GetExtension());
                File.Delete(path);
            }

            PlayerPrefs.DeleteAll();
        }


        private static IEnumerable<PersistenceData> EnumerateLevelPersistentFilePathsData() => AssetDatabase
            .FindAssets($"t:{nameof(PersistenceData)}")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<PersistenceData>);
    }
}
