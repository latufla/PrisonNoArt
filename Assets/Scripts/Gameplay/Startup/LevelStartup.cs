using Cysharp.Threading.Tasks;
using DG.Tweening;
using Honeylab.Consumables;
using Honeylab.Cutscene;
using Honeylab.Gameplay.Analytics;
using Honeylab.Gameplay.Booster;
using Honeylab.Gameplay.Creatures;
using Honeylab.Gameplay.Equipments;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.World;
using Honeylab.Sounds;
using Honeylab.Sounds.Data;
using Honeylab.Utils;
using Honeylab.Utils.OffscreenTargetIndicators;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace Honeylab.Gameplay.Startup
{
    public class LevelStartup : ILevelStartup
    {
        private readonly GameplayPoolsService _poolsService;
        private readonly ConsumablesService _consumablesService;
        private readonly GameplayScreenPresenter _gameplayScreenPresenter;
        private readonly PlayerInputService _playerInputService;
        private readonly PlayerInputPresenter _playerInputPresenter;
        private readonly PlayerInventoryService _playerInventoryService;
        private readonly NavigationService _navigationService;
        private readonly ToastsService _toastsService;
        private readonly IAnalyticsTracker[] _trackers;
        private readonly SoundService _soundService;
        private readonly WorldObjectsService _worldObjectsService;
        private readonly CutsceneService _cutsceneService;
        private readonly OffscreenIndicatorsService _offscreenIndicatorsService;
        private readonly VibrationService _vibrationService;
        private readonly EquipmentsService _equipmentsService;
        private readonly SoundsData _soundsData;
        private readonly BoosterService _boosterService;


        public LevelStartup(GameplayPoolsService poolsService,
            ConsumablesService consumablesService,
            GameplayScreenPresenter gameplayScreenPresenter,
            PlayerInputService playerInputService,
            PlayerInputPresenter playerInputPresenter,
            PlayerInventoryService playerInventoryService,
            NavigationService navigationService,
            ToastsService toastsService,
            IEnumerable<IAnalyticsTracker> trackers,
            SoundService soundService,
            WorldObjectsService worldObjectsService,
            CutsceneService cutsceneService,
            OffscreenIndicatorsService offscreenIndicatorsService,
            VibrationService vibrationService,
            EquipmentsService equipmentsService,
            SoundsData soundsData,
            BoosterService boosterService)
        {
            _poolsService = poolsService;
            _consumablesService = consumablesService;
            _gameplayScreenPresenter = gameplayScreenPresenter;
            _playerInputService = playerInputService;
            _playerInputPresenter = playerInputPresenter;
            _playerInventoryService = playerInventoryService;
            _navigationService = navigationService;
            _toastsService = toastsService;
            _trackers = trackers.ToArray();
            _soundService = soundService;
            _worldObjectsService = worldObjectsService;
            _cutsceneService = cutsceneService;
            _offscreenIndicatorsService = offscreenIndicatorsService;
            _vibrationService = vibrationService;
            _equipmentsService = equipmentsService;
            _soundsData = soundsData;
            _boosterService = boosterService;
        }


        public async UniTask InitAsync(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            DOTween.SetTweensCapacity(1400, 1400);

            await _poolsService.InitAsync(ct);

            _consumablesService.Init();
            _equipmentsService.Init();
            await UniTask.Yield(ct);

            _soundService.Init();

            _worldObjectsService.Init();

            _playerInputPresenter.Init();
            _playerInventoryService.Init();
            _navigationService.Init();
            _vibrationService.Init();

            _boosterService.Init();
        }


        public async UniTask RunAsync(CancellationToken ct)
        {
            await _cutsceneService.RunAsync(ct);

            _worldObjectsService.Run();

            _gameplayScreenPresenter.Run();
            _playerInventoryService.Run();

            _playerInputService.RunAsync().Forget();
            _playerInputPresenter.RunAsync().Forget();

            _toastsService.Run();
            _offscreenIndicatorsService.Run();
            _equipmentsService.Run();

            _soundService.Run();
            _soundService.RequestSoundPlay(_soundsData.MainMusicSoundId);

            _vibrationService.Run();
            _boosterService.RunAsync().Forget();

            foreach (IAnalyticsTracker tracker in _trackers)
            {
                tracker.Run();
            }
        }
    }
}
