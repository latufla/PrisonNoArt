using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Player;
using Honeylab.Utils.CameraTargeting;
using Honeylab.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Honeylab.Gameplay.Tutorial
{
    public class SpeechTutorialStep : TutorialStepBase
    {
        [SerializeField] private SpeechInfo[] _speechInfos;
        //[SerializeField] private string _taskText;

        [Serializable]
        private class SpeechInfo
        {
            public string Text;
            public float Time;
        }
        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await SpeechAsync(ct)
        };
        private async UniTask SpeechAsync(CancellationToken ct)
        {
            //SendStepStartAnalytics(_taskText);
            PlayerFlow player = GetObjects<PlayerFlow>().First();
            Vector3 position = player.transform.position;
            position.y += 10;

            using (BlockPlayerInput())
            {
                ICameraTargetingHandle handler = CameraTargetingService.Enqueue(player.transform, new CameraTargetingOverrides().WithCameraDistance(40));
                await handler.WaitForFocusAsync(ct);
                foreach (var info in _speechInfos)
                {
                    ShowToast(position, info.Text, info.Time);
                    await UniTask.Delay(TimeSpan.FromSeconds(info.Time + 0.5f), cancellationToken: ct);
                }
                handler.Finish();
            }
        }


        public override TutorialInfo GetTutorialInfo() => null;


        protected override void OnSubStepComplete()
        {
        }
    }
}
