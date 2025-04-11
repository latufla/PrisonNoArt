using System.Threading;
using Honeylab.Gameplay.Player;
using Honeylab.Sounds;
using Honeylab.Utils;
using Honeylab.Utils.OffscreenTargetIndicators;
using UniRx;

namespace Honeylab.Gameplay.Ui.Pause
{
    public class PauseScreenPresenter : ScreenPresenterBase<PauseScreen>
    {
        private readonly PlayerInputService _playerInputService;
        private readonly SoundService _soundService;
        private readonly VibrationService _vibrationService;
        private readonly OffscreenIndicatorsService _offscreenIndicatorsService;
        private readonly TimeService _time;

        private CompositeDisposable _disposable;

        public PauseScreenPresenter(ScreenFactory factory,
            PlayerInputService playerInputService,
            SoundService soundService,
            VibrationService vibrationService,
            OffscreenIndicatorsService offscreenIndicatorsService,
            TimeService time) : base(factory)
        {
            _playerInputService = playerInputService;
            _soundService = soundService;
            _vibrationService = vibrationService;
            _offscreenIndicatorsService = offscreenIndicatorsService;
            _time = time;
        }


        protected override void OnRun(CancellationToken ct)
        {
            _disposable = new CompositeDisposable();

            Screen.Init(_soundService, _playerInputService, _vibrationService, _time, _offscreenIndicatorsService,
                false);
            Screen.Run();
        }


        protected override void OnStop()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}