using Honeylab.Gameplay.Equipments;
using Honeylab.Gameplay.Upgrades;
using Honeylab.Gameplay.Weapons.Upgrades;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Configs;
using System;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Weapons
{
    public class WeaponFlow : WorldObjectFlow
    {
        [SerializeField] private ConfigIdProvider _configId;
        [SerializeField] private WeaponView _view;

        private WorldObjectsService _world;
        private EquipmentsService _equipmentService;
        private CompositeDisposable _disposable;
        private IWeaponAgent _agent;
        private UpgradeFlow _upgrade;
        private bool _useFixedUpgradeLevel;

        public WeaponUpgradeConfig Config { get; private set; }

        public ReactiveProperty<int> UpgradeLevel { get; } = new();
        public ReactiveProperty<WeaponUpgradeLevelConfig> UpgradeConfigProp { get; } = new();


        protected override void OnInit()
        {
            _world = Resolve<WorldObjectsService>();
            _equipmentService = Resolve<EquipmentsService>();

            IConfigsService configs = Resolve<IConfigsService>();
            Config = configs.Get<WeaponUpgradeConfig>(_configId.Id);
            _upgrade = _world.GetObject<UpgradeFlow>(Id);

            _disposable = new CompositeDisposable();

            EquipmentId equipmentId = Id as EquipmentId;
            if (_equipmentService.HasEquipment(equipmentId))
            {
                IDisposable equipmentLevelChange = _equipmentService.GetEquipmentLevel(equipmentId)
                    .Where(_ => !_useFixedUpgradeLevel)
                    .Subscribe(toLevel => { _upgrade.Upgrade(toLevel); });
                _disposable.Add(equipmentLevelChange);
            }

            IDisposable upgrade = _upgrade.UpgradeLevelPersistence.ValueProperty.Where(_ => !_useFixedUpgradeLevel)
                .Subscribe(SetUpgradeLevel);
            _disposable.Add(upgrade);
        }


        protected override void OnClear()
        {
            _useFixedUpgradeLevel = false;

            _disposable?.Dispose();
            _disposable = null;
        }


        public void SetAgent(IWeaponAgent agent)
        {
            _agent = agent;

            if (_agent == null)
            {
                if (_view != null)
                {
                    _view.SetSlot(null);
                }

                return;
            }

            Transform slot = _agent.GetSlot(0);
            _view.SetSlot(slot);
        }


        public IWeaponAgent GetAgent() => _agent;


        public void SetFixedUpgradeLevel(int level)
        {
            _useFixedUpgradeLevel = true;
            SetUpgradeLevel(level);
        }


        private void SetUpgradeLevel(int level)
        {
            UpgradeLevel.Value = level;
            UpgradeConfigProp.Value =
                _upgrade.GetLevelUpgradeConfig<WeaponUpgradeLevelConfig>(Config.Upgrade, level);
        }
    }
}
