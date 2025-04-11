using DG.Tweening;
using UnityEngine;


namespace Honeylab.Utils.Extensions
{
    public static class AnimatorDoTweenExtension
    {
        public static Tween TweenFloat(this Animator animator, int parameterHash, float endValue, float duration) =>
            DOTween.To(() => animator.GetFloat(parameterHash),
                value => animator.SetFloat(parameterHash, value),
                endValue,
                duration);
    }
}
