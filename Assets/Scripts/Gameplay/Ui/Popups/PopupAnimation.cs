using Cysharp.Threading.Tasks;
using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public class PopupAnimation : MonoBehaviour
    {
        private static readonly int IsShowing = Animator.StringToHash(nameof(IsShowing));

        [SerializeField] private Animator _animator;

        private UniTaskCompletionSource _hiddenTcs;


        private void OnDestroy()
        {
            _hiddenTcs?.TrySetCanceled();
        }


        public void SetShowing(bool isShowing) => _animator.SetBool(IsShowing, isShowing);


        public UniTask WaitForHideAsync()
        {
            _hiddenTcs ??= new UniTaskCompletionSource();
            return _hiddenTcs.Task;
        }


        public void Animator_Internal_OnHidden()
        {
            _hiddenTcs?.TrySetResult();
            _hiddenTcs = null;
        }
    }
}
