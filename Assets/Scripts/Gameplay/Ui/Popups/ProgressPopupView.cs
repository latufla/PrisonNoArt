using Cysharp.Threading.Tasks;
using Honeylab.Pools;
using Honeylab.Utils.Pool;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public class ProgressPopupView : PopupViewBase<ProgressPopup>
    {
        protected override IGameObjectPool GetPool() => Pools.Get<ProgressPopupsPool>();


        public async UniTask ShowProgressPopupAsync(float duration, CancellationToken ct)
        {
            ProgressPopup popup = Show();
            popup.SetProgress(0.0f);

            float time = 0.0f;
            while (time < duration)
            {
                await UniTask.Yield(ct);
                time += Time.deltaTime;

                if (popup == null)
                {
                    return;
                }

                popup.SetProgress(time / duration);
            }

            HideProgressPopupAsync();
        }


        public UniTask HideProgressPopupAsync() => HideAsync(true);
    }
}
