using UnityEditor;


namespace Honeylab.Utils.Configs.Editor
{
    [InitializeOnLoad]
    public class RemoteConfigCompilePlayModeEnterTrigger
    {
        static RemoteConfigCompilePlayModeEnterTrigger()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }


        private static void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange == PlayModeStateChange.ExitingEditMode)
            {
                RemoteConfigCompiler.Compile();
            }
        }
    }
}
