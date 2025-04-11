using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Player;
using Honeylab.Utils;
using Honeylab.Utils.CameraTargeting;
using Honeylab.Utils.Extensions;
using System;
using System.Threading;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    public abstract class UIStatusPanelBase : MonoBehaviour
    {
        [SerializeField] protected GameObject Root;
        [SerializeField] protected Button Button;
        [SerializeField] protected TimeProgressPanel ProgressPanel;
        [SerializeField] protected Image Icon;
        [SerializeField] protected TextMeshProUGUI LevelText;
        [SerializeField] protected Animator Animator;

        private string _levelLabelFormat;
        private ICameraTargetingHandle _handler;
        protected bool IsFocused;
        protected CompositeDisposable Disposable;
        protected CancellationTokenSource Cts;
        protected PlayerInputService PlayerInputService;
        protected ICameraTargetingService CameraTargetingService;

        private readonly Subject<bool> _onFocusedTargetSubject = new Subject<bool>();
        protected static readonly int ProcessState = Animator.StringToHash("ProcessState");

        public IObservable<Unit> OnButtonClickAsObservable() => Button.OnClickAsObservable();
        public IObservable<bool> OnFocusedTarget() => _onFocusedTargetSubject.AsObservable();
        public bool InProcess { get; protected set; }


        public void Init(PlayerInputService playerInputService, ICameraTargetingService cameraTargetingService)
        {
            PlayerInputService = playerInputService;
            CameraTargetingService = cameraTargetingService;
        }


        protected virtual void IdleWork()
        {
            Show();

            if (InProcess)
            {
                Animator.SetInteger(ProcessState, 0);
            }

            InProcess = false;

            Disposable?.Dispose();
            Disposable = null;
        }


        protected void SetIcon(Sprite icon)
        {
            Icon.sprite = icon;
        }


        protected void SetText(int level)
        {
            if (string.IsNullOrEmpty(_levelLabelFormat))
            {
                _levelLabelFormat = LevelText.text;
            }

            string amountText = level.ToString();
            LevelText.text = string.Format(_levelLabelFormat, amountText);
        }


        protected async UniTask FocusTarget(Transform target, CancellationToken ct)
        {
            if (IsFocused || target == null)
            {
                return;
            }

            IsFocused = true;
            using (PlayerInputService.BlockInput())
            {
                _handler =
                    CameraTargetingService.Enqueue(target);
                await _handler.WaitForFocusAsync(ct);
                _onFocusedTargetSubject.OnNext(true);
                await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: ct);
                _handler.Finish();
                _onFocusedTargetSubject.OnNext(false);
            }

            IsFocused = false;
        }


        public void Show()
        {
            Root.SetActive(true);
        }


        public void Hide()
        {
            Root.SetActive(false);
        }


        public void Clear()
        {
            OnClear();
            Hide();

            Cts?.CancelThenDispose();
            Cts = null;

            Disposable?.Dispose();
            Disposable = null;

            if (_handler == null)
            {
                return;
            }

            _handler.Finish();
            IsFocused = false;
        }


        protected virtual void OnClear() { }
    }
}
