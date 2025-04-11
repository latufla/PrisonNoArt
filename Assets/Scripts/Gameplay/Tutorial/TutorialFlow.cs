using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Honeylab.Cutscene;
using Honeylab.Gameplay.Tutorial;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils.Data;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Persistence;
using UniRx;
using UnityEngine;

namespace Honeylab.Utils.Tutorial
{
    public class TutorialFlow : WorldObjectFlow
    {
        [SerializeField] private TutorialStepContext[] _steps;
        [SerializeField] private float _startDelay;
        [SerializeField] private ScriptableId _offscreenIndicatorId;
        [SerializeField] private int _startStep;

        public ScriptableId OffscreenIndicatorId => _offscreenIndicatorId;

        private CutsceneService _cutSceneService;
        private LevelPersistenceService _levelPersistenceService;
        private TutorialScreenPresenter _screenPresenter;

        private TutorialPersistentComponent _state;

        private readonly ISubject<TutorialStepInfo> _stepStartSubject = new Subject<TutorialStepInfo>();
        private readonly ISubject<Unit> _stepEndSubject = new Subject<Unit>();

        public TutorialStepContext CurrentTutorialStepContext { get; private set; }
        public IObservable<TutorialStepInfo> OnTutorialStepStartAsObservable() => _stepStartSubject.AsObservable();
        public IObservable<Unit> OnTutorialStepEndAsObservable() => _stepEndSubject.AsObservable();


        protected override void OnInit()
        {
            _cutSceneService = Resolve<CutsceneService>();

            _levelPersistenceService = Resolve<LevelPersistenceService>();
            _screenPresenter = Resolve<TutorialScreenPresenter>();

            _state = _levelPersistenceService.GetOrAddComponent<TutorialPersistentComponent>(Id);

            int n = _steps.Length;
            for (int i = 0; i < n; ++i)
            {
                _steps[i].Init();
            }
        }


        protected override async UniTask OnRunAsync(CancellationToken ct)
        {
            if (_cutSceneService.HasCutScene())
            {
                await UniTask.WaitUntil(() => _cutSceneService.IsCutSceneFinished(), cancellationToken: ct);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(_startDelay), cancellationToken: ct);

            _screenPresenter.Run(this);

            int n = _steps.Length;
            int completedStepsCount = CalcCompletedStepsCount();
            for (int i = completedStepsCount; i < n; i++)
            {
                CurrentTutorialStepContext = _steps[i];
                await CurrentTutorialStepContext.RunAsync(ct);

                if (CurrentTutorialStepContext.Step.Id != null)
                {
                    _state.Step++;
                    _state.Progress = 0;
                }
            }

            _screenPresenter.Hide();
        }


        private int CalcCompletedStepsCount()
        {
            int count = 0;
            int i = 0;
            _state.Step = _startStep > 0 ? _startStep : _state.Step;
            while (i < _state.Step && count < _steps.Length)
            {
                TutorialStepContext step = _steps[count];
                count++;

                if (step.Step.Id != null)
                {
                    i++;
                }
            }

            return count;
        }


        public void OnStepStart(TutorialStepInfo info)
        {
            _stepStartSubject.OnNext(info);
        }


        public void OnStepEnd()
        {
            _stepEndSubject.OnNext();
        }


        protected override void OnClear()
        {
            if (CurrentTutorialStepContext != null)
            {
                CurrentTutorialStepContext.Clear();
            }
        }
    }
}