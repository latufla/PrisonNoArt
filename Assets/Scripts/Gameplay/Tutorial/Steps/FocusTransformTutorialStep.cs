using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Honeylab.Gameplay.Tutorial
{
    public class FocusTransformTutorialStep : TutorialStepBase
    {
        [SerializeField] private Transform _focusObject;
        [SerializeField] private float _duration = 1.0f;


        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct)
        {
            var result = new List<Func<UniTask>>
            {
                async () =>
                {
                    await FocusTargetAsync(_focusObject, _duration, ct);
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
