using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Arrows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.Tutorial
{
    public class UpgradeTutorialStep : TutorialStepBase
    {
        [SerializeField] private WorldObjectId _weaponId;
        [SerializeField] private TutorialInfo _upgradeInfo;
        [SerializeField] private TutorialInfo _collectInfo;


        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await UpgradeAsync(ct)
        };

        private async UniTask UpgradeAsync(CancellationToken ct)
        {
            UpgradeBuildingFlow building = null;
            await UniTask.WaitUntil(() =>
                {
                    building = GetObjects<UpgradeBuildingFlow>().FirstOrDefault(it => _weaponId.Equals(it.WeaponId));
                    return building != null;
                },
                cancellationToken: ct);

            int upgradeLevel = building.WeaponUpgrade.UpgradeLevelPersistence.Value + 1;
            SendStepStartAnalytics(_upgradeInfo);
            if (upgradeLevel >= _upgradeInfo.RequiredWeaponLevel)
            {
                //ShowScreen(_upgradeInfo);
                return;
            }

            _upgradeInfo.WorldObject = building;

            Transform target = building.transform;
            ShowScreen(_upgradeInfo, target);

            IArrowHandle arrow = ShowTargetArrow(target, _upgradeInfo.ArrowPositionY);
            ShowOffscreenIndicator(target, _upgradeInfo.Icon);

            if (_upgradeInfo.FocusOnStart)
            {
                FocusTargetAsync(target);
            }

            int n = _upgradeInfo.RequiredWeaponLevel - upgradeLevel;
            for (int i = 0; i < n; i++)
            {
                if (building.State.Value == UpgradeBuildingStates.Idle)
                {
                    await UniTask.WaitUntil(() => building.State.Value == UpgradeBuildingStates.Work,
                        cancellationToken: ct);
                }

                await UniTask.WaitUntil(() => building.State.Value == UpgradeBuildingStates.Done, cancellationToken: ct);

                HideOffscreenIndicator();
                ShowScreen(_collectInfo, building.transform);

                await UniTask.WaitUntil(() => building.State.Value == UpgradeBuildingStates.Idle, cancellationToken: ct);

                if (i < _upgradeInfo.RequiredWeaponLevel - 1)
                {
                    ShowScreen(_upgradeInfo, building.transform);
                }
            }

            HideTargetArrow(arrow);
            HideOffscreenIndicator();
        }


        public override TutorialInfo GetTutorialInfo() => _upgradeInfo;


        protected override void OnSubStepComplete()
        {
            SendStepEndAnalytics();
        }
    }
}
