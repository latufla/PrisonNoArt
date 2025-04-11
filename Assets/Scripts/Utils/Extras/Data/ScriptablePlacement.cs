using UnityEngine;


namespace Honeylab.Utils.Data
{
    [CreateAssetMenu(menuName = DataUtil.MenuNamePrefix + "Scriptable Placement",
        fileName = DataUtil.DefaultFileNamePrefix + nameof(ScriptablePlacement))]
    public class ScriptablePlacement : ScriptableObject
    {
        [SerializeField] private string _placement;


        public string Placement => _placement;
    }
}
