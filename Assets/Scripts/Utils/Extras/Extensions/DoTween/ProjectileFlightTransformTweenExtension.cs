using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;


namespace Honeylab.Utils.Extensions
{
    public static class ProjectileFlightTransformTweenExtension
    {
        public static TweenerCore<Vector3, Vector3, ProjectileFlightOptions> DOProjectileMove(this Transform target,
            Vector3 endValue,
            float duration)
        {
            ProjectileFlightPlugin plugin = ProjectileFlightPlugin.Instance;
            var tweener = DOTween.To(plugin, () => target.position, x => target.position = x, endValue, duration)
                .SetTarget(target);
            return tweener;
        }


        public static T SetVertexRaise<T>(this T tweener, float vertexRaise)
            where T : TweenerCore<Vector3, Vector3, ProjectileFlightOptions>
        {
            tweener.plugOptions.VertexRaise = vertexRaise;
            return tweener;
        }
    }
}
