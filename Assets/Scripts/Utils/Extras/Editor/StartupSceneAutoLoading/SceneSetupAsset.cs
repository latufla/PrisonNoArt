using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


namespace Honeylab.Airport.Editor
{
    public class SceneSetupAsset : ScriptableObject
    {
        [SerializeField] private SceneSetup[] _setup;


        public bool TryLoad(out SceneSetup[] setup)
        {
            setup = _setup;
            return setup != null && setup.Length > 0;
        }


        public void SetSetup(SceneSetup[] newSetup)
        {
            _setup = newSetup;
            EditorUtility.SetDirty(this);
        }
    }
}
