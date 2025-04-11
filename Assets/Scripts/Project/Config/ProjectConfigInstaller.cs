using UnityEngine;
using Zenject;


namespace Honeylab.Project
{
    public class ProjectConfigInstaller : MonoInstaller
    {
        [SerializeField] private ProjectConfig _editorConfig;
        [SerializeField] private ProjectConfig _debugConfig;
        [SerializeField] private ProjectConfig _releaseConfig;


        public override void InstallBindings()
        {
            const bool isEditor =
                    #if UNITY_EDITOR
                    true
                #else
                    false
                #endif
                ;

            ProjectConfig config = isEditor switch
            {
                true => _editorConfig,
                false => Debug.isDebugBuild switch
                {
                    true => _debugConfig,
                    false => _releaseConfig
                }
            };

            Container.BindInstance(config);
        }
    }
}
