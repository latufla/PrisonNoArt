using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Honeylab.Gameplay;
using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.World;
using UnityEngine;
using UniRx;

namespace Honeylab.Gameplay.World
{
    public class ResourceHighlightItem : WorldObjectComponentBase
    {
        [SerializeField]
        private ResourcePopupView _resourcePopupView;

        private WorldObjectFlow _flow;

        private CancellationTokenSource _cts;

        private const float SHOW_DURATION = 2;
        private bool _showed;


        protected override void OnInit()
        {
            _cts = new();
            _flow = GetFlow();

            BillboardPresenterFactory billboardPresenter = _flow.Resolve<BillboardPresenterFactory>();
            GameplayPoolsService gameplayPools = _flow.Resolve<GameplayPoolsService>();
            _resourcePopupView.Init(gameplayPools, billboardPresenter);
        }

        public async UniTask ShowItem(Sprite icon, CancellationToken ct)
        {
            if (_showed)
                return;

            _showed = true;
            var resourcePopup = _resourcePopupView.Show();
            bool hasIcon = resourcePopup != null;

            if (hasIcon)
            {
                resourcePopup.Image.sprite = icon;
            }
            else
            {
                Debug.LogError($"{nameof(ResourceHighlightItem)} Icon set is null");
                resourcePopup.Image.gameObject.SetActive(false);
                return;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(SHOW_DURATION), cancellationToken: ct);

            await _resourcePopupView.HideAsync();

            _showed = false;
        }


        protected override void OnClear()
        {
            _resourcePopupView.HideAsync().Forget();
        }
    }
}
