using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Pickup;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Arrows;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.Tutorial
{
    public class PickupWeaponTutorialStep : TutorialStepBase
    {
        [SerializeField] private WorldObjectId _worldObjectId;
        [SerializeField] private TutorialInfo _info;


        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await CollectWeaponAsync(ct)
        };


        private async UniTask CollectWeaponAsync(CancellationToken ct)
        {
            SendStepStartAnalytics(_info);

            PickupFlow flow = null;
            await UniTask.WaitUntil(() =>
                {
                    flow = GetObject<PickupFlow>(_worldObjectId);
                    return flow != null;
                },
                cancellationToken: ct);

            Transform target = flow.transform;

            if (flow.IsDeactivatePersistence.Value)
            {
                //ShowScreen(_info);
                return;
            }

            ShowScreen(_info, target);

            IArrowHandle arrow = ShowTargetArrow(target, _info.ArrowPositionY);
            ShowOffscreenIndicator(target, _info.Icon);

            await UniTask.WaitUntil(() => flow.IsDeactivatePersistence.Value, cancellationToken: ct);

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
