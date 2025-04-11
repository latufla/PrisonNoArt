using System.Linq;
using UnityEngine;


namespace Honeylab.Utils.Animation
{
    public static class AnimatorUtil
    {
        public static AnimationClip GetAnimationClip(Animator animator, string animation)
        {
            var clips = animator.runtimeAnimatorController.animationClips;
            return clips.FirstOrDefault(clip => clip.name == animation);
        }
    }
}
