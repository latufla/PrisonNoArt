using Honeylab.Gameplay.Player;
using Honeylab.Sounds;
using Honeylab.Utils;
using Honeylab.Utils.OffscreenTargetIndicators;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.Pause
{
    public class PauseScreen : ScreenBase
    {
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _sfxToggle;
        [SerializeField] private Toggle _vibrateToggle;
        [SerializeField] private Button _leaveButton;
        [SerializeField] private TextMeshProUGUI _versionText;
        private SoundService _soundService;
        private VibrationService _vibrationService;
        private PlayerInputService _playerInputService;
        private TimeService _time;
        private OffscreenIndicatorsService _offscreenIndicatorsService;
        private IDisposable _blockInput;

        public override string Name => ScreenName.Pause;
        public override IObservable<Unit> OnCloseButtonClickAsObservable() => base.OnCloseButtonClickAsObservable()
            .Merge(_backgroundButton.OnClickAsObservable());
        public IObservable<Unit> OnLeaveButtonClickAsObservable() => _leaveButton.OnClickAsObservable();

        public void Init(SoundService soundService, PlayerInputService playerInputService, VibrationService vibrationService, TimeService time, OffscreenIndicatorsService offscreenIndicatorsService, bool isExpedition)
        {
            _soundService = soundService;
            _playerInputService = playerInputService;
            _vibrationService = vibrationService;
            _time = time;
            _offscreenIndicatorsService = offscreenIndicatorsService;

            LeaveButtonActive(isExpedition);

            string versionTextLabel = _versionText.text;
            _versionText.text = string.Format(versionTextLabel, Application.version);
        }


        public void Run()
        {
            _blockInput = _playerInputService.BlockInput();
            _time.Pause();

            _offscreenIndicatorsService.SetIndicatorsVisible(false);

            _musicToggle.isOn = _soundService.MusicEnabledProp.Value;
            _sfxToggle.isOn = _soundService.SfxEnabledProp.Value;
            _vibrateToggle.isOn = _vibrationService.IsEnabledProp.Value;

            _musicToggle.onValueChanged.AddListener(OnChangeMusicVolume);
            _sfxToggle.onValueChanged.AddListener(OnChangeSfxVolume);
            _vibrateToggle.onValueChanged.AddListener(OnChangeVibrate);
        }


        private void OnChangeMusicVolume(bool value)
        {
            _soundService.MusicEnabledProp.Value = value;
            ScreenAction(value ? ScreenParameters.MusicEnabled : ScreenParameters.MusicDisabled);
        }


        private void OnChangeSfxVolume(bool value)
        {
            _soundService.SfxEnabledProp.Value = value;
            ScreenAction(value ? ScreenParameters.SfxEnabled : ScreenParameters.SfxDisabled);
        }


        private void OnChangeVibrate(bool value)
        {
            _vibrationService.IsEnabledProp.Value = value;
            ScreenAction(value ? ScreenParameters.VibrationEnabled : ScreenParameters.VibrationDisabled);
        }


        public void LeaveButtonActive(bool active)
        {
            _leaveButton.gameObject.SetActive(active);
        }


        public override void Hide()
        {
            base.Hide();
            _time.Resume();

            _blockInput?.Dispose();
            _blockInput = null;

            _musicToggle.onValueChanged.RemoveAllListeners();
            _sfxToggle.onValueChanged.RemoveAllListeners();
            _vibrateToggle.onValueChanged.RemoveAllListeners();

            _offscreenIndicatorsService.SetIndicatorsVisible(true);
        }
    }
}
