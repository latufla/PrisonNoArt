using Cysharp.Threading.Tasks;
using Honeylab.Analytics;
using Honeylab.Gameplay.Startup;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Startup;
using Honeylab.Utils.Analytics;
using Honeylab.Utils.App;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Honeylab.Project.Levels
{
    [Serializable]
    public class LevelsServiceArgs
    {
        public PersistenceId PersistenceId;
        public List<WorldObjectId> LevelsInOrder;
        public LevelsData AllLevelsData;
    }


    public class LevelsLoadService : IDisposable
    {
        private readonly LevelsServiceArgs _args;
        private readonly SharedPersistenceService _sharedPersistenceService;
        private readonly LevelPersistenceService _levelPersistenceService;

        private List<LevelData> _levelsInOrder;

        private LevelIndexPersistentComponent _levelInOrderPersistence;
        private LevelNamePersistentComponent _levelAllPersistence;

        private readonly IAnalyticsService _analyticsService;
        private CancellationTokenSource _cts = new();
        private LoadingScreen _screen;

        private bool _isLevelLoadBlocked;


        public LevelsLoadService(LevelsServiceArgs args,
            SharedPersistenceService sharedPersistenceService,
            LevelPersistenceService levelPersistenceService,
            IAnalyticsService analyticsService)
        {
            _args = args;
            _sharedPersistenceService = sharedPersistenceService;
            _levelPersistenceService = levelPersistenceService;
            _analyticsService = analyticsService;
        }


        public void Init(LoadingScreen screen)
        {
            _screen = screen;

            PersistentObject po = _sharedPersistenceService.GetOrCreate(_args.PersistenceId);
            _levelInOrderPersistence = po.GetOrAdd<LevelIndexPersistentComponent>();
            _levelAllPersistence = po.GetOrAdd<LevelNamePersistentComponent>();

            _levelsInOrder = _args.LevelsInOrder.Select(GetLevelData).ToList();
        }


        public void Run()
        {
        }


        public void Dispose()
        {
            _cts?.CancelThenDispose();
            _cts = null;
        }


        public IReadOnlyList<LevelData> GetLevelsInOrder() => _levelsInOrder;


        public LevelData GetLevelData(WorldObjectId id)
        {
            var allLevels = _args.AllLevelsData.GetLevels();
            return allLevels.FirstOrDefault(it => it.Id.Equals(id));
        }


        public bool IsLevelActive(WorldObjectId id) => GetLevelIndex(id) == GetActiveLevelIndex();


        private int GetLevelIndex(WorldObjectId id)
        {
            LevelData level = GetLevelData(id);
            return _levelsInOrder.IndexOf(level);
        }


        public int GetActiveLevelIndex() => _levelInOrderPersistence.Value;
        public LevelData GetActiveLevelData() => _levelsInOrder[GetActiveLevelIndex()];


        public bool SetLevelLoadBlocked(bool isBlocked) => _isLevelLoadBlocked = isBlocked;
        public bool IsLevelLoadBlocked() => _isLevelLoadBlocked;


        public async UniTask LoadLevelAsync()
        {
            string name = _levelAllPersistence.Value;
            if (!string.IsNullOrEmpty(name))
            {
                LevelData data = GetLevelData(name);
                await LoadLevelAsync(data);

                return;
            }

            await LoadLevelInOrderAsync(_levelInOrderPersistence.Value);
        }


        public async UniTask LoadLevelInOrderAsync(int index)
        {
            LevelData data = _levelsInOrder[index];
            _levelInOrderPersistence.Value = index;
            _levelAllPersistence.Value = string.Empty;

            await LoadLevelAsync(data);
        }

        public void SetLevelAllPersistence(int index)
        {
            LevelData data = _levelsInOrder[index];
            _levelInOrderPersistence.Value = index;
            _levelAllPersistence.Value = data.SceneName;
        }


        public async UniTask LoadLevelAllAsync(string name)
        {
            LevelData data = GetLevelData(name);
            _levelAllPersistence.Value = data.SceneName;

            await LoadLevelAsync(data);
        }


        private async UniTask LoadLevelAsync(LevelData data)
        {
            if (IsLevelLoadBlocked())
            {
                throw new Exception("Level load is blocked");
            }

            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            _screen.gameObject.SetActive(true);

            string sceneName = data.SceneName;
            await SceneManager.LoadSceneAsync(sceneName).ToUniTask();

            Scene gameplayScene = SceneManager.GetSceneByName(sceneName);
            GameplayStartup gameplayStartup = gameplayScene.Resolve<GameplayStartup>();

            LoadLevelPersistence();

            await gameplayStartup.InitAsync(_cts.Token);

            _screen.gameObject.SetActive(false);

            await gameplayStartup.RunAsync(_cts.Token);

            var msg = new Dictionary<string, object>
            {
                [AnalyticsParameters.Name] = _levelInOrderPersistence.Value,
            };
            _analyticsService.ReportEvent(AnalyticsKeys.LoadLevel, msg);
        }


        public LevelData GetLevelData(string sceneName)
        {
            var allLevels = _args.AllLevelsData.GetLevels();
            return allLevels.FirstOrDefault(it => it.SceneName.Equals(sceneName));
        }


        public void LeaveLevel()
        {
            _sharedPersistenceService.Save();
            _levelPersistenceService.Save();
        }


        private void LoadLevelPersistence()
        {
            _levelPersistenceService.Clear();

            _levelPersistenceService.SetLevelIndex(_levelInOrderPersistence.Value);
            _levelPersistenceService.Init();
        }
    }
}
