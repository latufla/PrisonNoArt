using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Cameras;
using Honeylab.Gameplay.Player.Upgrades;
using Honeylab.Gameplay.Upgrades;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Configs;
using System;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Player
{
    public class PlayerFlow : WorldObjectFlow
    {
        [SerializeField] private PlayerMotion _motion;
        [SerializeField] private PlayerView _view;
        [SerializeField] private ConfigIdProvider _speedUpgradeConfigId;
        [SerializeField] private WorldObjectIdProvider _speedUpgradeId;

        private IConfigsService _configs;
        private WorldObjectsService _world;

        private CompositeDisposable _run;


        public PlayerView View => _view;
        public PlayerMotion Motion => _motion;


        protected override void OnInit()
        {
            CameraProvider cameraProvider = Resolve<CameraProvider>();
            cameraProvider.PlayerCamera.Follow = transform;

            _configs = Resolve<IConfigsService>();
            _world = Resolve<WorldObjectsService>();
        }


        protected override async UniTask OnRunAsync(CancellationToken ct)
        {
            _run = new CompositeDisposable();
            IDisposable onSpeedUpgrade = RunOnSpeedUpgrade();
            _run.Add(onSpeedUpgrade);

            IDisposable walk = _motion.IsMoving
                .Subscribe(it =>
                {
                    if (it)
                    {
                        _view.Animations.PlayWalk();
                    }
                    else
                    {
                        _view.Animations.PlayIdle();
                    }
                });

            _run.Add(walk);

            await UniTask.WaitUntilCanceled(ct);
        }


        protected override void OnStop()
        {
            _run?.Clear();
        }


        protected override void OnClear()
        {
            _run?.Dispose();
            _run = null;
        }


        private IDisposable RunOnSpeedUpgrade()
        {
            PlayerSpeedUpgradeConfig speedConfig = _configs.Get<PlayerSpeedUpgradeConfig>(_speedUpgradeConfigId.Id);

            UpgradeFlow upgrade = _world.GetObject<UpgradeFlow>(_speedUpgradeId.Id);
            return upgrade.UpgradeLevelPersistence.ValueProperty.Subscribe(upgradeLevel =>
            {
                PlayerSpeedUpgradeLevelConfig config = upgrade.GetLevelUpgradeConfig<PlayerSpeedUpgradeLevelConfig>(
                    speedConfig.Upgrade,
                    upgradeLevel);
                _motion.SetSpeed(config.Speed);
            });
        }
    }
}
