using UnityEngine;


namespace Honeylab.Utils.Animation
{
    public class RandomStateEntryPosition : StateMachineBehaviour
    {
        [SerializeField] private string _stateProgressParameter;

        private float _position;
        private int _parameterHash;


        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            _position = Random.Range(0.0f, 0.75f);
            _parameterHash = Animator.StringToHash(_stateProgressParameter);
        }


        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);

            _position = (_position + Time.deltaTime / stateInfo.length) % 1.0f;
            animator.SetFloat(_parameterHash, _position);
        }
    }
}
