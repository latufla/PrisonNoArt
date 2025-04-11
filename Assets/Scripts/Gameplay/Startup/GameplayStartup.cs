using Cysharp.Threading.Tasks;
using Honeylab.Utils.Analytics;
using System.Threading;
using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.Startup
{
    public class GameplayStartup
    {
        private readonly DiContainer _diContainer;
        private readonly GameObjectContext _levelPrefab;
        private readonly GameStartTracker _gameStartTracker;

        private ILevelStartup _levelStartup;


        public GameplayStartup(DiContainer diContainer,
            GameObjectContext levelPrefab,
            GameStartTracker gameStartTracker)
        {
            _diContainer = diContainer;
            _levelPrefab = levelPrefab;
            _gameStartTracker = gameStartTracker;
        }


        public async UniTask InitAsync(CancellationToken ct)
        {
            GameObject levelGo = _diContainer.InstantiatePrefab(_levelPrefab);
            DiContainer levelDiContainer = levelGo.GetComponent<GameObjectContext>().Container;
            _levelStartup = levelDiContainer.Resolve<ILevelStartup>();

            await _levelStartup.InitAsync(ct);
        }


        public async UniTask RunAsync(CancellationToken ct)
        {
            _gameStartTracker.Run();
            await _levelStartup.RunAsync(ct);
        }
    }
}
