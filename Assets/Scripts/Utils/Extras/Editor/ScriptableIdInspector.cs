using Honeylab.Utils.Data;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Honeylab.Utils.Editor
{
    [CustomEditor(typeof(ScriptableId), true)]
    [CanEditMultipleObjects]
    public class ScriptableIdInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;

            if (GUILayout.Button($"Generate new {nameof(Guid)}"))
            {
                foreach (ScriptableId id in targets.Cast<ScriptableId>())
                {
                    id.GenerateNewGuid();
                    EditorUtility.SetDirty(id);
                }
            }
        }
    }
}
