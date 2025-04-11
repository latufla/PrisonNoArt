using Cysharp.Threading.Tasks;
using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public abstract class PopupBase : MonoBehaviour
    {
        [SerializeField] private PopupAnimation _animation;


        public virtual void SetShowing(bool isShowing) => _animation.SetShowing(isShowing);
        public UniTask WaitForHideAsync() => _animation.WaitForHideAsync();
    }
}
