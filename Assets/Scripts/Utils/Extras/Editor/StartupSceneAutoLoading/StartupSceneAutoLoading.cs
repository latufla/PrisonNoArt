using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


namespace Honeylab.Airport.Editor
{
    [InitializeOnLoad]
    public static class StartupSceneAutoLoading
    {
        static StartupSceneAutoLoading()
        {
            EditorApplication.playModeStateChanged += EditorApplication_OnPlayModeStateChanged;
        }


        private static void RestoreSceneSetupIfPossible()
        {
            if (!TryLoadSceneSetupAsset(out SceneSetupAsset asset) ||
                !asset.TryLoad(out var setup))
            {
                return;
            }

            EditorSceneManager.RestoreSceneManagerSetup(setup);
        }


        private static SceneSetupAsset GetOrCreateSceneSetupAsset()
        {
            if (TryLoadSceneSetupAsset(out SceneSetupAsset asset))
            {
                return asset;
            }

            asset = ScriptableObject.CreateInstance<SceneSetupAsset>();
            asset.hideFlags = HideFlags.NotEditable;
            CreateAssetWithHierarchy(asset, "Assets/_EditorOnly/SceneSetupAsset.asset");
            return asset;
        }


        private static bool TryLoadSceneSetupAsset(out SceneSetupAsset asset)
        {
            string assetGuid = AssetDatabase.FindAssets($"t:{nameof(SceneSetupAsset)}")
                .FirstOrDefault();
            if (string.IsNullOrEmpty(assetGuid))
            {
                asset = default;
                return false;
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            asset = AssetDatabase.LoadAssetAtPath<SceneSetupAsset>(assetPath);
            return true;
        }


        private static void CreateAssetWithHierarchy(Object asset, string assetPath)
        {
            try
            {
                AssetDatabase.StartAssetEditing();

                string assetsLocalAssetPath = assetPath.Replace("Assets/", string.Empty);
                string fileSystemParentDirectoryPath = $"{Application.dataPath}/{assetsLocalAssetPath}";
                Directory.CreateDirectory(fileSystemParentDirectoryPath);

                AssetDatabase.CreateAsset(asset, assetPath);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }
        }


        private static void EditorApplication_OnPlayModeStateChanged(PlayModeStateChange change)
        {
            StartupSceneAutoLoadingSettings settings = AssetDatabase
                .FindAssets($"t:{nameof(StartupSceneAutoLoadingSettings)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<StartupSceneAutoLoadingSettings>)
                .SingleOrDefault();
            if (settings == null || !settings.IsEnabled)
            {
                return;
            }

            switch (change)
            {
                case PlayModeStateChange.ExitingEditMode:
                    GetOrCreateSceneSetupAsset().SetSetup(EditorSceneManager.GetSceneManagerSetup());
                    EditorSceneManager.OpenScene($"Assets/Scenes/{settings.SceneName}.unity");
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    RestoreSceneSetupIfPossible();
                    break;
                default:
                    break;
            }
        }
    }
}
