using Cysharp.Threading.Tasks;
using DG.Tweening;
using Honeylab.Gameplay.Ui;
using System;
using System.Threading;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Tutorial
{
    public class TutorialScreen : MonoBehaviour
    {
        private static readonly int End = Animator.StringToHash("End");
        private static readonly int Start = Animator.StringToHash("Start");

        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject _rootPanel;
        [SerializeField] private TaskInfo _taskInfo;
        [SerializeField] private CompleteInfo _completeInfo;
        [SerializeField] private Transform _clickHerePanel;
        [SerializeField] private Animator _screenAnimator;
        [SerializeField] private Animator _clickHereAnimator;
        [SerializeField] private TutorialScreenRequiredPresenter _requiredItems;

        private TutorialInfo _tutorialInfo;
        private int _maxAmountToCollect;
        private float _completeLifetime;

        private int _collectedAmount;
        private bool _isShowing;
        private readonly Subject<TutorialInfo> _onShowScreenSubjects = new Subject<TutorialInfo>();

        public Transform ClickHerePanel => _clickHerePanel;
        public Animator ClickHereAnimator => _clickHereAnimator;
        public TutorialScreenRequiredPresenter RequiredItems => _requiredItems;

        public bool IsShowing() => _isShowing;
        public IObservable<TutorialInfo> OnShowScreenAsObservable() => _onShowScreenSubjects.AsObservable();
        public IObservable<Unit> OnFocusButtonClickAsObservable() => _taskInfo.FocusButton.OnButtonClickAsObservable();
        public TutorialInfo TutorialInfo => _tutorialInfo;

        public TutorialScreenFocusButton FocusButton => _taskInfo.FocusButton;


        public void Init()
        {
            Hide();
        }


        public void Show(TutorialInfo tutorialInfo, bool activeSortingLayer)
        {
            _tutorialInfo = tutorialInfo;
            _canvas.overrideSorting = activeSortingLayer;

            _screenAnimator.ResetTrigger(End);
            _screenAnimator.SetTrigger(Start);

            _maxAmountToCollect = _tutorialInfo.MaxAmountToCollect;
            _completeLifetime = _tutorialInfo.CompleteDuration;
            string taskText = _tutorialInfo.TaskText;

            _rootPanel.SetActive(true);
            _rootPanel.transform.localScale = Vector3.one;

            _completeInfo.TaskCompletePanel.gameObject.SetActive(false);
            _completeInfo.TaskCompletePanel.transform.localScale = Vector3.one;

            if (taskText != string.Empty)
            {
                _taskInfo.TaskActivePanel.SetActive(true);
                _taskInfo.TaskText.text = taskText;
            }

            _taskInfo.SliderForAmount.maxValue = _maxAmountToCollect;
            _taskInfo.SliderForAmount.value = 0;

            if (_taskInfo.CollectTaskPanel)
            {
                _taskInfo.CollectTaskPanel.SetActive(_maxAmountToCollect > 1);
            }

            SetCollectedAmount(0);
            _isShowing = true;

            _requiredItems.Run(_tutorialInfo);
            _onShowScreenSubjects.OnNext(_tutorialInfo);
        }


        public void SetIcon(Sprite icon)
        {
            _taskInfo.TaskImage.gameObject.SetActive(icon != null);
            _taskInfo.TaskImage.sprite = icon;
        }


        public void SetCollectedAmount(int amount)
        {
            if (_maxAmountToCollect <= 1)
            {
                return;
            }

            _collectedAmount = amount <= _maxAmountToCollect ? amount : _maxAmountToCollect;
            UpdateAmount();
        }


        public async UniTask ShowCompleteAsync(CancellationToken ct)
        {
            if (!_isShowing)
            {
                return;
            }

            _completeInfo.TaskCompleteImage.sprite = _taskInfo.TaskImage.sprite;
            _completeInfo.TaskCompletePanel.gameObject.SetActive(true);

            await RunCompleteAsync(ct);

            _screenAnimator.ResetTrigger(Start);
            _screenAnimator.SetTrigger(End);
            await UniTask.Delay(TimeSpan.FromSeconds(1.8f), cancellationToken: ct);

            _taskInfo.TaskActivePanel.SetActive(false);
        }


        private async UniTask RunCompleteAsync(CancellationToken ct)
        {
            const float bounceTime = 2.0f;
            float currentBounceTime = 0.0f;
            float timeLeft = _completeLifetime;

            while (timeLeft > 0)
            {
                await UniTask.Yield(ct);

                if (currentBounceTime <= 0)
                {
                    await _rootPanel.transform
                        .DOPunchScale(new Vector3(0.4f, 0.4f, 0.4f), 0.5f, vibrato: 0)
                        .ToUniTask(cancellationToken: ct);

                    currentBounceTime = bounceTime;
                }
                else
                {
                    currentBounceTime -= Time.deltaTime;
                }

                timeLeft -= Time.deltaTime;
            }
        }


        public void FocusButtonActive(bool active)
        {
            _taskInfo.FocusButton.Button.interactable = active;
        }


        public void FocusImageActive(bool active)
        {
            _taskInfo.FocusImage.SetActive(active);
        }


        public void Hide()
        {
            _rootPanel.SetActive(false);

            TaskHide();

            _requiredItems.Hide();
            _isShowing = false;

            _canvas.overrideSorting = false;
        }


        public void TaskHide()
        {
            _taskInfo.TaskActivePanel.SetActive(false);
            _completeInfo.TaskCompletePanel.gameObject.SetActive(false);
        }


        private void UpdateAmount()
        {
            _taskInfo.TaskAmountText.text = $"{_collectedAmount} / {_maxAmountToCollect}";
            _taskInfo.SliderForAmount.value = _collectedAmount;
        }


        [Serializable]
        private class TaskInfo
        {
            public GameObject TaskActivePanel;
            public Image TaskImage;
            public TextMeshProUGUI TaskText;
            public TextMeshProUGUI TaskAmountText;
            public GameObject CollectTaskPanel;
            public Slider SliderForAmount;
            public TutorialScreenFocusButton FocusButton;
            public GameObject FocusImage;
        }


        [Serializable]
        private class CompleteInfo
        {
            public GameObject TaskCompletePanel;
            public Image TaskCompleteImage;
        }
    }
}
