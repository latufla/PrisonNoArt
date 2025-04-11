using System;
using UnityEditor;


#if UNITY_EDITOR_WIN
[InitializeOnLoad]
public class ResolverFix
{
    // This saves from if a new java is installed on the system
    // https://forum.unity.com/threads/failed-to-update-android-sdk-package-list-error-when-using-sdk-installed-with-unity.722777/
    // https://www.notion.so/honeylab/Facebook-SDK-fc43742a2aff4a6caea9d5fe58667923
    static ResolverFix()
    {
        string newJdkPath =
            EditorApplication.applicationPath.Replace("Unity.exe", "Data/PlaybackEngines/AndroidPlayer/OpenJDK");
        Environment.SetEnvironmentVariable("JAVA_HOME", newJdkPath);
    }
}
#endif
