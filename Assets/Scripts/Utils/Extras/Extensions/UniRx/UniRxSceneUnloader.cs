using System;
using System.Linq;
using UniRx;
using UnityEngine.SceneManagement;


namespace Honeylab.Utils.Extensions
{
    public static class UniRxSceneUnloader
    {
        public static IObservable<Unit> UnloadAllScenesExcept(Scene exceptScene)
        {
            var unloads = Enumerable.Range(0, SceneManager.sceneCount)
                .Select(SceneManager.GetSceneAt)
                .Where(s => s != exceptScene)
                .Select(SceneManager.UnloadSceneAsync)
                .Select(op => op.AsObservable())
                .ToArray();

            return Observable
                .WhenAll(unloads)
                .Select(_ => Unit.Default);
        }


        public static IObservable<Unit> UnloadAllScenesExcept(string sceneName) => Observable
            .Defer(() =>
            {
                Scene exceptScene = Enumerable.Range(0, SceneManager.sceneCount)
                    .Select(SceneManager.GetSceneAt)
                    .Single(s => s.name.Equals(sceneName));
                return UnloadAllScenesExcept(exceptScene);
            });
    }
}
