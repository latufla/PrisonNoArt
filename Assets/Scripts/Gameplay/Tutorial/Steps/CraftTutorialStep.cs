using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Arrows;
using UnityEngine;

namespace Honeylab.Gameplay.Tutorial
{
    public class CraftTutorialStep : TutorialStepBase
    {
        [SerializeField] private ConsumablePersistenceId _consumableId;
        [SerializeField] private TutorialInfo _craftInfo;
        [SerializeField] private TutorialInfo _collectInfo;
        [SerializeField] private WorldObjectId _unlockWorldId;
        [SerializeField] private bool _useExistingAmount;

        [field: SerializeField] public bool OneTimeResourceActive { get; private set; }

        public (bool exist, Vector3 position) Position { get; private set; }


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

                _craftInfo.CurrentAmountToCollect = consumableAmountProp.Value;
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

            Position = (true, target.position);

            _craftInfo.WorldObject = building;
            _collectInfo.WorldObject = building;

            ShowScreen(_craftInfo, target);
            ScreenSetCollectedAmount(_craftInfo.CurrentAmountToCollect);

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

                _craftInfo.CurrentAmountToCollect = consumableAmountProp.Value;
            }

            while (_craftInfo.CurrentAmountToCollect < _craftInfo.MaxAmountToCollect)
            {
                if (building.State.Value == CraftBuildingStates.Idle)
                {
                    await UniTask.WaitUntil(() => building.State.Value == CraftBuildingStates.Work,
                        cancellationToken: ct);
                }

                await UniTask.WaitUntil(() => building.State.Value == CraftBuildingStates.Done, cancellationToken: ct);

                HideOffscreenIndicator();

                int prevAmount = consumableAmountProp.Value;
                ShowScreen(_collectInfo, building.transform);

                await UniTask.WaitUntil(() => building.State.Value != CraftBuildingStates.Done, cancellationToken: ct);

                int delta = consumableAmountProp.Value - prevAmount;
                _craftInfo.CurrentAmountToCollect += delta;

                if (_craftInfo.CurrentAmountToCollect < _craftInfo.MaxAmountToCollect)
                {
                    ShowScreen(_craftInfo, building.transform);
                }

                ScreenSetCollectedAmount(_craftInfo.CurrentAmountToCollect);
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