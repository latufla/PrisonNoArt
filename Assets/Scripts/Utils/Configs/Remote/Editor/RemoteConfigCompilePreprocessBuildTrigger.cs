using UnityEditor.Build;
using UnityEditor.Build.Reporting;


namespace Honeylab.Utils.Configs.Editor
{
    public class RemoteConfigCompilePreprocessBuildTrigger : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }


        public void OnPreprocessBuild(BuildReport report) => RemoteConfigCompiler.Compile();
    }
}
