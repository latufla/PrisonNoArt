using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;


namespace Honeylab.Utils.Arrows
{
    public class ArrowView : MonoBehaviour
    {
        private static readonly int Hide = Animator.StringToHash(nameof(Hide));

        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _showRoot;

        private AutoResetUniTaskCompletionSource _hiddenTcs;


        public void Show() => _showRoot.SetActive(true);
        public virtual void HideImmediately() => _showRoot.SetActive(false);


        public UniTask HideAsync(CancellationToken ct)
        {
            _hiddenTcs ??= AutoResetUniTaskCompletionSource.Create();
            _animator.SetTrigger(Hide);
            return _hiddenTcs.Task.AttachExternalCancellation(ct);
        }


        public void Animator_Internal_OnHidden()
        {
            _hiddenTcs?.TrySetResult();
            _hiddenTcs = null;

            HideImmediately();
        }
    }
}
