using Honeylab.Analytics;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils.Analytics;
using Honeylab.Utils.Persistence;
using Honeylab.Utils.Tutorial;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;


namespace Honeylab.Gameplay.Analytics
{
    [Serializable]
    public class TutorialAnalyticsArgs
    {
        public PersistenceId TutorialAnalyticsId;
    }


    public class TutorialAnalyticsTracker : IAnalyticsTracker
    {
        private readonly TutorialAnalyticsArgs _args;
        private readonly WorldObjectsService _world;
        private readonly LevelPersistenceService _levelPersistenceService;
        private readonly IAnalyticsService _analyticsService;

        private CompositeDisposable _disposable;
        private TutorialAnalyticsPersistentComponent _persistence;



        public TutorialAnalyticsTracker(TutorialAnalyticsArgs args,
            WorldObjectsService world,
            LevelPersistenceService levelPersistenceService,
            IAnalyticsService analyticsService)
        {
            _args = args;
            _world = world;
            _levelPersistenceService = levelPersistenceService;
            _analyticsService = analyticsService;
        }


        public void Run()
        {
            _persistence =
                _levelPersistenceService.GetOrAddComponent<TutorialAnalyticsPersistentComponent>(_args.TutorialAnalyticsId);

            _disposable = new CompositeDisposable();

            TutorialFlow tutorialFlow = _world.GetObjects<TutorialFlow>().First();
            IDisposable stepStarted = tutorialFlow.OnTutorialStepStartAsObservable()
                .Where(info =>
                    !string.IsNullOrEmpty(info.MissionId) && info.MissionId != _persistence.MissionId)
                .Subscribe(info =>
                {
                    if (!string.IsNullOrEmpty(_persistence.MissionId))
                    {
                        string endMissionId = _persistence.MissionId;
                        SendMissionCompleted(endMissionId);
                    }

                    string startMissionId = info.MissionId;
                    SendMissionStarted(startMissionId);

                    _persistence.MissionId = info.MissionId;

                });

            _disposable.Add(stepStarted);
        }


        private void SendMissionStarted(string missionId)
        {
            var msg = new Dictionary<string, object>
            {
                [AnalyticsParameters.Name] = missionId,
                [AnalyticsParameters.Amount] = "Started"
            };
            _analyticsService.ReportEvent(AnalyticsKeys.Mission, msg);

        }
        private void SendMissionCompleted(string missionId)
        {
            var msg = new Dictionary<string, object>
            {
                [AnalyticsParameters.Name] = missionId,
                [AnalyticsParameters.Amount] = "Completed"
            };
            _analyticsService.ReportEvent(AnalyticsKeys.Mission, msg);
        }

        public void Dispose() => _disposable?.Dispose();
    }
}
