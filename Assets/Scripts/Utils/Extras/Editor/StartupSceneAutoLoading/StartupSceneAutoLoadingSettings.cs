using Honeylab.Utils.Data;
using UnityEngine;


namespace Honeylab.Airport.Editor
{
    [CreateAssetMenu(menuName = DataUtil.MenuNamePrefix + "Startup Scene Auto Loading Settings",
        fileName = DataUtil.DefaultFileNamePrefix + nameof(StartupSceneAutoLoadingSettings))]
    public class StartupSceneAutoLoadingSettings : ScriptableObject
    {
        [SerializeField] private bool _isEnabled;
        [SerializeField] private string _sceneName;


        public bool IsEnabled => _isEnabled;
        public string SceneName => _sceneName;
    }
}
