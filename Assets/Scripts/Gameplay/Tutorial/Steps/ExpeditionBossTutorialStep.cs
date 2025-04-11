using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Creatures.Spawners;
using Honeylab.Gameplay.World;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.Tutorial
{
    public class ExpeditionBossTutorialStep : TutorialStepBase
    {
        [SerializeField] private WorldObjectId _spawnerId;

        public override TutorialInfo GetTutorialInfo() => null;


        protected override void OnSubStepComplete() { }


        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await ExpeditionCompleteAsync(ct)
        };


        private async UniTask ExpeditionCompleteAsync(CancellationToken ct)
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

            await UniTask.WaitUntil(() => spawnerFlow.Objects.Count > 0, cancellationToken: ct);

            await UniTask.WaitUntil(() => spawnerFlow.Objects.Count == 0, cancellationToken: ct);
        }
    }
}
