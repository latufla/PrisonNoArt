using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.World;
using Honeylab.Utils.CameraTargeting;
using Honeylab.Utils.Pool;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Honeylab.Gameplay.Ui.Craft
{
    public class CraftStatusScreen : ScreenBase
    {
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private GameObjectPoolBehaviour _craftStatusPanelPool;
        [SerializeField] private Transform _craftStatusPanelsRoot;

        private List<CraftStatusPanel> _items = new();
        private IDisposable _blockInput;
        private PlayerInputService _playerInputService;
        private WorldObjectsService _world;
        private ICameraTargetingService _cameraTargetingService;
        private CompositeDisposable _disposables;

        public override string Name => ScreenName.AdOffer;
        public override IObservable<Unit> OnCloseButtonClickAsObservable() => base.OnCloseButtonClickAsObservable()
            .Merge(_backgroundButton.OnClickAsObservable());


        [Inject]
        public void Construct(PlayerInputService playerInputService,
            WorldObjectsService world,
            ICameraTargetingService cameraTargetingService)
        {
            _playerInputService = playerInputService;
            _world = world;
            _cameraTargetingService = cameraTargetingService;
        }

        public void Run(CancellationToken ct)
        {
            _blockInput = _playerInputService.BlockInput();

            var buildings = _world.GetObjects<CraftBuildingFlow>();

            _disposables = new CompositeDisposable();
            foreach (var building in buildings)
            {
                var buildingView = building.Get<CraftBuildingView>();
                if (buildingView == null)
                {
                    continue;
                }
                if (building.State.Value != CraftBuildingStates.Idle)
                {
                    SetItems(building);
                }
            }
        }


        private void SetItems(CraftBuildingFlow flow)
        {
            GameObject itemGo = _craftStatusPanelPool.Pop(true);
            if (itemGo.TryGetComponent(out CraftStatusPanel itemInfo))
            {
                _items.Add(itemInfo);
                itemInfo.transform.SetParent(_craftStatusPanelsRoot, false);
                itemInfo.Init(_playerInputService, _cameraTargetingService);
                IDisposable craft = flow.State.ValueProperty
                    .Subscribe(state =>
                    {
                        itemInfo.StateChanged(flow);
                    });
                _disposables.Add(craft);
            }
            else
            {
                _craftStatusPanelPool.Push(itemGo);
            }
        }
        public void Stop()
        {
            foreach (var item in _items)
            {
                item.Clear();
                _craftStatusPanelPool.Push(item.gameObject);
            }

            _items.Clear();

            _blockInput?.Dispose();
            _blockInput = null;

            _disposables?.Dispose();
            _disposables = null;
        }
    }
}
