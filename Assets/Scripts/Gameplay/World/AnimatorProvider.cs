using UnityEngine;


namespace Honeylab.Gameplay.World
{
    public class AnimatorProvider : MonoBehaviour
    {
        [SerializeField] private Animator _animator;


        public Animator Animator => _animator;
    }
}
