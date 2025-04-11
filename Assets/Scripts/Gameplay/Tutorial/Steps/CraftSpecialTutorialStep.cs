using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
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
    public class CraftSpecialTutorialStep : TutorialStepBase
    {
        [SerializeField] private ConsumablePersistenceId _consumableId;
        [SerializeField] private TutorialInfo _craftInfo;
        [SerializeField] private TutorialInfo _collectInfo;
        [SerializeField] private WorldObjectId _unlockWorldId;
        [SerializeField] private bool _useExistingAmount;

        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await CraftAsync(ct)
        };


        private async UniTask CraftAsync(CancellationToken ct)
        {
            SendStepStartAnalytics(_craftInfo);
            if (CheckUnlock())
            {
                return;
            }

            var consumableAmountProp = GetConsumableAmountProp(_consumableId);
            int amountToCollect = _craftInfo.MaxAmountToCollect;

            if (_useExistingAmount)
            {
                if (consumableAmountProp.Value >= amountToCollect)
                {
                    return;
                }
            }

            CraftBuildingFlow building = null;
            await UniTask.WaitUntil(() =>
            {
                ConsumableData data = GetConsumableData(_consumableId);
                building = GetObjects<CraftBuildingFlow>()
                    .FirstOrDefault(it => data.Name.Equals(it.Config.CraftResult.Name));
                return building != null;
            },
                cancellationToken: ct);

            Transform target = building.transform;

            _craftInfo.WorldObject = building;
            _collectInfo.WorldObject = building;

            ShowScreen(_craftInfo, target);
            ScreenSetCollectedAmount(0);

            IArrowHandle arrow = ShowTargetArrow(target, _craftInfo.ArrowPositionY);
            ShowOffscreenIndicator(target, _craftInfo.Icon);

            if (_craftInfo.FocusOnStart)
            {
                FocusTargetAsync(target);
            }

            if (_useExistingAmount)
            {
                if (consumableAmountProp.Value >= amountToCollect)
                {
                    HideOffscreenIndicator();
                    HideTargetArrow(arrow);
                    return;
                }
            }

            bool isShowing = true;

            while (consumableAmountProp.Value < amountToCollect)
            {
                await UniTask.Yield(ct);
                ScreenSetCollectedAmount(consumableAmountProp.Value);

                if (!isShowing)
                {
                    ShowScreen(_craftInfo, target);
                    isShowing = true;
                }

                if (building.GetCurrentAmount() <= 0)
                {
                    continue;
                }

                int oldValue = consumableAmountProp.Value;

                ShowScreen(_collectInfo, target);

                await UniTask.WaitUntil(() => consumableAmountProp.Value > oldValue, cancellationToken: ct);

                isShowing = false;
            }

            HideTargetArrow(arrow);
            HideOffscreenIndicator();
            _craftInfo.WorldObject = null;
            _collectInfo.WorldObject = null;
        }

        private bool CheckUnlock()
        {
            bool isUnlocked = false;
            if (_unlockWorldId != null)
            {
                var building = GetObject<UnlockBuildingFlow>(_unlockWorldId);
                if (building != null && building.State.Value == UnlockBuildingStates.Unlocked)
                {
                    isUnlocked = true;
                }
            }
            return isUnlocked;
        }


        public override TutorialInfo GetTutorialInfo() => _craftInfo;


        protected override void OnSubStepComplete()
        {
            SendStepEndAnalytics();
        }
    }
}
