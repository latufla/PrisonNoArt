using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Creatures.Spawners;
using Honeylab.Gameplay.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.Tutorial
{
    public class EnemyKillTutorialStep : TutorialStepBase
    {
        [SerializeField] private WorldObjectId _spawnerId;
        [SerializeField] private TutorialInfo _info;
        [SerializeField] private bool _isNeedSpawn = true;


        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await EnemyKillAsync(ct)
        };


        private async UniTask EnemyKillAsync(CancellationToken ct)
        {
            EnemySpawnerFlow spawnerFlow = null;
            await UniTask.WaitUntil(() =>
                {
                    spawnerFlow = GetObject<EnemySpawnerFlow>(_spawnerId);
                    return spawnerFlow != null;
                },
                cancellationToken: ct);

            if (spawnerFlow.DiedEnemiesPersistence.Value > 0)
            {
                return;
            }

            EnemySpawner spawner = spawnerFlow.Get<EnemySpawner>();
            if (_isNeedSpawn)
            {
                spawner.SetMaxCount(1);
            }

            await UniTask.WaitUntil(() => spawnerFlow.Objects.Count > 0, cancellationToken: ct);
            WorldObjectFlow enemy = spawnerFlow.Objects.First();

            Transform target = enemy.transform;
            ShowScreen(_info, target);
            SendStepStartAnalytics(_info);

            ShowOffscreenIndicator(target, _info.Icon);

            if (_info.FocusOnStart)
            {
                FocusTargetAsync(target);
            }

            await UniTask.WaitUntil(() => spawnerFlow.Objects.Count == 0, cancellationToken: ct);

            if (_isNeedSpawn)
            {
                spawner.ClearMaxCount();
            }

            HideOffscreenIndicator();
        }


        public override TutorialInfo GetTutorialInfo() => _info;


        protected override void OnSubStepComplete()
        {
            SendStepEndAnalytics();
        }
    }
}
