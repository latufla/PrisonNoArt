using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.World;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.Tutorial
{
    public class FocusTutorialStep : TutorialStepBase
    {
        [SerializeField] private WorldObjectId[] _focusObjectIds;
        [SerializeField] private float _duration = 1.0f;


        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct)
        {
            var result = new List<Func<UniTask>>
            {
                async () =>
                {
                    foreach (var id in _focusObjectIds)
                    {
                        WorldObjectFlow wo = await GetObjectAsync(id, ct);
                        Transform target = wo.transform;
                        await FocusTargetAsync(target, _duration, ct);
                    }
                }
            };
            return result;
        }


        public override TutorialInfo GetTutorialInfo() => null;


        protected override void OnSubStepComplete()
        {
        }
    }
}
