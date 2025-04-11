using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Arrows;
using UnityEngine;

namespace Honeylab.Gameplay.Tutorial
{
    public class UnlockBuildingTutorialStep : TutorialStepBase
    {
        [SerializeField] private WorldObjectId _buildingId;
        [SerializeField] private TutorialInfo _info;

        public (bool exist, Vector3 position) Position { get; private set; }

        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await UnlockBuildingAsync(ct)
        };


        private async UniTask UnlockBuildingAsync(CancellationToken ct)
        {
            UnlockBuildingFlow building = await GetObjectAsync<UnlockBuildingFlow>(_buildingId, ct);

            Transform target = building.transform;

            Position = (true, target.position);

            _info.WorldObject = building;
            ShowScreen(_info, target);
            SendStepStartAnalytics(_info);

            IArrowHandle arrow = ShowTargetArrow(target, _info.ArrowPositionY);
            ShowOffscreenIndicator(target, _info.Icon);

            if (_info.FocusOnStart)
            {
                FocusTargetAsync(target);
            }

            await UniTask.WaitUntil(() =>
                    building.State.Value == UnlockBuildingStates.Claim ||
                    building.State.Value == UnlockBuildingStates.Unlocked,
                cancellationToken: ct);

            HideOffscreenIndicator();

            await UniTask.WaitUntil(() => building.State.Value == UnlockBuildingStates.Unlocked, cancellationToken: ct);

            HideTargetArrow(arrow);
        }


        public override TutorialInfo GetTutorialInfo() => _info;


        protected override void OnSubStepComplete()
        {
            SendStepEndAnalytics();
        }
    }
}