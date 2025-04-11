using UnityEngine;


namespace Honeylab.Gameplay.World
{
    public class WorldObjectAnimationsBase : WorldObjectComponentBase
    {
        [SerializeField] private AnimatorProvider _animatorProvider;

        private int _state = -1;


        protected T GetAnimatorProvider<T>() where T : AnimatorProvider => _animatorProvider as T;


        protected override void OnInit() { }


        protected override void OnClear()
        {
            _state = -1;
        }


        public void SetAnimatorProvider(AnimatorProvider animatorProvider)
        {
            _animatorProvider = animatorProvider;
        }


        protected void ChangeState(int state, bool force = false)
        {
            if (_state == state && !force || _animatorProvider == null)
            {
                return;
            }

            if (_state != -1)
            {
                _animatorProvider.Animator.ResetTrigger(_state);
            }

            _state = state;
            _animatorProvider.Animator.SetTrigger(_state);
        }
    }
}
