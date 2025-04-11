using Honeylab.Gameplay.Cameras;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.World;
using Honeylab.Utils;
using Honeylab.Utils.OffscreenTargetIndicators;
using System;
using System.Threading;


namespace Honeylab.Gameplay.Ui.Minimap
{
    public class MinimapScreenPresenter : ScreenPresenterBase<MinimapScreen>
    {
        private readonly PlayerInputService _playerInputService;
        private readonly CameraProvider _cameraProvider;
        private readonly WorldObjectsService _world;
        private readonly TimeService _timeService;
        private readonly OffscreenIndicatorsService _offscreenIndicatorsService;
        private readonly MinimapIndicatorsService _minimapIndicatorsService;

        private IDisposable _blockInput;


        public MinimapScreenPresenter(ScreenFactory factory,
            PlayerInputService playerInputService,
            CameraProvider cameraProvider,
            WorldObjectsService world,
            TimeService timeService,
            OffscreenIndicatorsService offscreenIndicatorsService,
            MinimapIndicatorsService minimapIndicatorsService) : base(factory)
        {
            _playerInputService = playerInputService;
            _cameraProvider = cameraProvider;
            _world = world;
            _timeService = timeService;
            _offscreenIndicatorsService = offscreenIndicatorsService;
            _minimapIndicatorsService = minimapIndicatorsService;
        }


        protected override void OnRun(CancellationToken ct)
        {
            _blockInput = _playerInputService.BlockInput();
            //_timeService.Pause();

            _offscreenIndicatorsService.SetIndicatorsVisible(false);

            Screen.Run(_cameraProvider, _world, _minimapIndicatorsService);
        }


        protected override void OnStop()
        {
            Screen.Stop();

            _offscreenIndicatorsService.SetIndicatorsVisible(true);

            //_timeService.Resume();

            _blockInput?.Dispose();
            _blockInput = null;
        }
    }
}
