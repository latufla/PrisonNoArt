using Honeylab.Gameplay.World;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


namespace Honeylab.Utils.Editor
{
    public class EnsureWorldObjectsUniqueNames : ScriptableWizard
    {
        [MenuItem("Tools/Ensure World Objects Unique Names")]
        public static void ShowWizard()
        {
            DisplayWizard<EnsureWorldObjectsUniqueNames>("Ensure World Objects Unique Names");
        }


        private void OnEnable()
        {
            helpString = "Ensures that all World Objects in the currently active prefab have unique names";
        }


        private void OnWizardCreate()
        {
            List<GameObject> allGameObjects = new List<GameObject>();

            PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
            GameObject root = stage.prefabContentsRoot;
            foreach (WorldObjectFlow child in root.GetComponentsInChildren<WorldObjectFlow>(includeInactive: true))
            {
                allGameObjects.Add(child.gameObject);
            }

            for (int i = 0; i < allGameObjects.Count; i++)
            {
                GameObject go = allGameObjects[i];

                for (int j = i + 1; j < allGameObjects.Count; j++)
                {
                    GameObject otherGo = allGameObjects[j];
                    if (go.name == otherGo.name)
                    {
                        Undo.RecordObject(go, "Change Name");
                        go.name += i;
                        break;
                    }
                }
            }
        }
    }
}
