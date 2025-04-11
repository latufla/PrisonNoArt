using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Arrows;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.Tutorial
{
    public class DamageTutorialStep : TutorialStepBase
    {
        [SerializeField] private WorldObjectId _worldObjectId;
        [SerializeField] private TutorialInfo _info;
        [SerializeField] private int _healtLeft;


        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await DamageWorldObjectAsync(ct)
        };


        private async UniTask DamageWorldObjectAsync(CancellationToken ct)
        {
            WorldObjectFlow wo = await GetObjectAsync(_worldObjectId, ct);

            Transform target = wo.transform;
            ShowScreen(_info, target);
            SendStepStartAnalytics(_info);

            IArrowHandle arrow = ShowTargetArrow(target, _info.ArrowPositionY);
            ShowOffscreenIndicator(target, _info.Icon);

            if (_info.FocusOnStart)
            {
                FocusTargetAsync(target);
            }

            Health health = wo.Get<Health>();
            await UniTask.WaitUntil(() => health.HealthProp.Value <= _healtLeft, cancellationToken: ct);

            HideTargetArrow(arrow);
            HideOffscreenIndicator();
        }


        public override TutorialInfo GetTutorialInfo() => _info;


        protected override void OnSubStepComplete()
        {
            SendStepEndAnalytics();
        }
    }
}
