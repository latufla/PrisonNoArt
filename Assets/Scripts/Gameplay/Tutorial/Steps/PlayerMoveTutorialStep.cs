using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Tutorial
{
    public class PlayerMoveTutorialStep : TutorialStepBase
    {
        [SerializeField] private GameObject _moveScreen;
        [SerializeField] private float _finishDelay = 1.0f;


        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await WaitPlayerMoveAsync(ct)
        };


        public override TutorialInfo GetTutorialInfo() => null;


        protected override void OnSubStepComplete()
        {
            //SendStepEndAnalytics();
        }

        private async UniTask WaitPlayerMoveAsync(CancellationToken ct)
        {
            //SendStepStartAnalytics("PlayerMove");
            _moveScreen.SetActive(true);

            PlayerFlow player = null;
            await UniTask.WaitUntil(() =>
                {
                    player = GetObjects<PlayerFlow>().FirstOrDefault();
                    return player != null;
                },
                cancellationToken: ct);

            await player.Motion.IsMoving.Where(isMoving => isMoving).ToUniTask(true, ct);

            _moveScreen.SetActive(false);

            await UniTask.Delay(TimeSpan.FromSeconds(_finishDelay), cancellationToken: ct);
        }
    }
}
