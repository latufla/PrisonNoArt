using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Player;
using Honeylab.Utils.CameraTargeting;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Tutorial;
using System;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.Tutorial
{
    public class TutorialScreenPresenter : IDisposable
    {
        private readonly TutorialScreen _screen;
        private readonly PlayerInputService _playerInputService;
        private readonly ICameraTargetingService _cameraTargetingService;

        private TutorialFlow _flow;

        private TutorialScreenFocus _screenFocus;
        private CancellationTokenSource _runCompeteCts;

        public TutorialScreen Screen => _screen;
        public TutorialScreenFocus ScreenFocus => _screenFocus;

        public TutorialScreenPresenter(TutorialScreen screen,
            PlayerInputService playerInputService,
            ICameraTargetingService cameraTargetingService)
        {
            _screen = screen;
            _playerInputService = playerInputService;
            _cameraTargetingService = cameraTargetingService;
        }


        public void Run(TutorialFlow flow)
        {
            _flow = flow;

            _screen.Init();

            _screenFocus = new TutorialScreenFocus(_screen,
                _playerInputService,
                _cameraTargetingService);

            _screen.RequiredItems.Init(_flow, _screenFocus);

            _screenFocus.Run();
        }


        public void Dispose()
        {
            _screen.Hide();

            _runCompeteCts?.CancelThenDispose();
            _runCompeteCts = null;

            _screenFocus?.Stop();
            _screenFocus = null;
        }


        public void Show(TutorialInfo tutorialInfo, Transform target, bool activeSortingLayer)
        {
            _screenFocus.SetTarget(tutorialInfo.FocusOnClick ? target : null);

            _screen.Show(tutorialInfo, activeSortingLayer);

            _screenFocus.SetClickHerePanelActive(tutorialInfo.IsClickHerePanelActive);

            _screen.SetIcon(tutorialInfo.Icon);
        }


        public void SetCollectedAmount(int amount)
        {
            _screen.SetCollectedAmount(amount);
        }


        public async UniTask RunCompleteAsync()
        {
            _runCompeteCts?.CancelThenDispose();
            _runCompeteCts = new CancellationTokenSource();

            await _screen.ShowCompleteAsync(_runCompeteCts.Token);

            _screen.TaskHide();
            _screenFocus.RemoveTarget();
            _screenFocus.SetClickHerePanelActive(false);
        }


        public void Hide()
        {
            _screen.Hide();
        }


        public void SetFocusTarget(Transform target)
        {
            _screenFocus.SetTarget(target);
        }


        public void FocusTarget(Transform target)
        {
            _screenFocus.FocusTarget(target);
        }


        public UniTask FocusTargetAsync(Transform target, float duration, CancellationToken ct) =>
            _screenFocus.FocusTargetAsync(target, ct, duration);
    }
}
