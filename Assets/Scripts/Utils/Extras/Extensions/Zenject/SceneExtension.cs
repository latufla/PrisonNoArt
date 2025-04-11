using System.Linq;
using UnityEngine.SceneManagement;
using Zenject;


namespace Honeylab.Utils.Extensions
{
    public static class SceneExtension
    {
        public static T Resolve<T>(this Scene scene)
        {
            DiContainer diContainer = scene.GetRootGameObjects()
                .SelectMany(go => go.GetComponentsInChildren<SceneContext>())
                .Select(ctx => ctx.Container)
                .First();

            return diContainer.Resolve<T>();
        }
    }
}
