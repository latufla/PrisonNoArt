using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.World;
using Honeylab.Pools;
using Honeylab.Utils.Data;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.OffscreenTargetIndicators;
using Honeylab.Utils.Pool;
using System;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Pickup
{
    public class PickupView : WorldObjectComponentBase
    {
        [SerializeField] private Transform _skinRoot;
        [SerializeField] private float _rotateSpeed;
        [SerializeField] private ScriptableId _offscreenIndicatorId;

        private PickupFlow _flow;
        private PickupablesPool _pool;
        private OffscreenIndicatorsService _offscreenIndicatorsService;
        private OffscreenIndicator _offscreenIndicator;
        private ConsumablesData _consumablesData;
        private CompositeDisposable _disposable;
        private CancellationTokenSource _cts;

        private GameObject _skin;


        protected override void OnInit()
        {
            _flow = GetFlow<PickupFlow>();

            GameplayPoolsService pools = _flow.Resolve<GameplayPoolsService>();
            _pool = pools.Get<PickupablesPool>();

            _offscreenIndicatorsService = _flow.Resolve<OffscreenIndicatorsService>();
            _consumablesData = _flow.Resolve<ConsumablesData>();

            _disposable = new CompositeDisposable();

            if (_offscreenIndicatorId != null)
            {
                IDisposable collect = _flow.IsDeactivatePersistence.ValueProperty
                    .Subscribe(isCollected => { UpdateOffscreenIndicator(!isCollected); });
                _disposable.Add(collect);
            }
        }


        protected override void OnRun()
        {
            _cts = new CancellationTokenSource();
            OnRunAsync(_cts.Token).Forget();
        }


        private async UniTask OnRunAsync(CancellationToken ct)
        {
            _skin = _pool.Pop(_flow.Id);
            _skin.transform.SetParent(_skinRoot);
            _skin.transform.localPosition = Vector3.zero;
            _skin.transform.localRotation = Quaternion.identity;
            while (true)
            {
                _skinRoot.Rotate(Vector3.up * _rotateSpeed);
                await UniTask.Yield(ct);
            }
        }


        protected override void OnClear()
        {
            if (_skin != null)
            {
                _pool.Push(_flow.Id, _skin);
                _skin.transform.localScale = Vector3.one;
                _skin = null;
            }

            _cts?.CancelThenDispose();
            _cts = null;

            _disposable?.Dispose();
            _disposable = null;
        }


        private void UpdateOffscreenIndicator(bool isEnabled)
        {
            if (_offscreenIndicatorId == null)
            {
                return;
            }

            if (isEnabled)
            {
                _offscreenIndicator ??= _offscreenIndicatorsService.Add(_offscreenIndicatorId, transform);

                RewardAmountConfig reward = _flow.PickupConfig.Rewards[0];
                ConsumableData consumableData = _consumablesData.GetData(reward.Name);
                _offscreenIndicator.SetIcon(consumableData.Sprite);
            }
            else
            {
                if (_offscreenIndicator != null)
                {
                    _offscreenIndicatorsService.Remove(_offscreenIndicator);
                    _offscreenIndicator = null;
                }
            }
        }


        public void SetVisibleOffscreenIndicator(bool isVisible)
        {
            if (_offscreenIndicatorId == null || _offscreenIndicator == null)
            {
                return;
            }
            _offscreenIndicator.SetVisible(isVisible);
        }
    }
}
