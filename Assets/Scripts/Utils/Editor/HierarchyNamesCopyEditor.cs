using UnityEditor;
using UnityEngine;
using StringBuilder = System.Text.StringBuilder;


namespace Honeylab.ProjectName
{
    public static class HierarchyNamesCopyEditor
    {
        [MenuItem("GameObject/Honeylab/Copy Hierarchy")]
        public static void HierarchyNamesCopy()
        {
            if (Selection.activeObject != null && Selection.activeObject is GameObject)
            {
                Transform root = (Selection.activeObject as GameObject)?.transform;
                StringBuilder hierarchyNames = new();
                for (int i = 0; i < root.childCount; i++)
                {
                    hierarchyNames.AppendLine(root.GetChild(i).name);
                }
                GUIUtility.systemCopyBuffer = hierarchyNames.ToString();
            }
        }
    }
}
