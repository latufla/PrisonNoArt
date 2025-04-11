using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.World;
using Honeylab.Utils;
using Honeylab.Utils.Arrows;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Honeylab.Gameplay.Tutorial
{
    public class BringConsumablesUnlockTutorialStep : TutorialStepBase
    {
        [SerializeField] private ConsumablePersistenceId _consumableId;
        [SerializeField] private WorldObjectId _unlockBuildingId;
        [SerializeField] private TutorialInfo _info;
        [SerializeField] private bool _useExistingAmount;
        private VibrationService _vibrationService;

        [Inject]
        public void Construct(VibrationService vibrationService)
        {
            _vibrationService = vibrationService;
        }

        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await BringConsumablesAsync(ct)
        };


        private async UniTask BringConsumablesAsync(CancellationToken ct)
        {
            UnlockBuildingFlow building = null;
            await UniTask.WaitUntil(() =>
                {
                    building = GetObject<UnlockBuildingFlow>(_unlockBuildingId);
                    return building != null;
                },
                cancellationToken: ct);

            ConsumableData data = GetConsumableData(_consumableId);
            int index = building.Config.UnlockPrice.FindIndex(it => it.Name.Equals(data.Name));
            int currentAmount = building.Consumables.Amounts[index];
            RewardAmountConfig config = building.Config.UnlockPrice[index];
            Transform target = building.transform;

            SendStepStartAnalytics(_info);

            if (_useExistingAmount && (currentAmount >= _info.MaxAmountToCollect || currentAmount >= config.Amount))
            {
                //ShowScreen(_info);
                //ScreenSetCollectedAmount(_info.MaxAmountToCollect);
                return;
            }

            ShowScreen(_info, target);
            ScreenSetCollectedAmount(0);

            IArrowHandle arrow = ShowTargetArrow(target, _info.ArrowPositionY);
            ShowOffscreenIndicator(target, _info.Icon);

            if (_info.FocusOnStart)
            {
                FocusTargetAsync(target);
            }

            currentAmount = building.Consumables.Amounts[index];
            if (_useExistingAmount && (currentAmount >= _info.MaxAmountToCollect || currentAmount >= config.Amount))
            {
                return;
            }

            int collectedAmount = 0;
            int reqAmount = currentAmount + _info.MaxAmountToCollect;
            while (collectedAmount < _info.MaxAmountToCollect)
            {
                await UniTask.WaitUntil(() => currentAmount != building.Consumables.Amounts[index],
                    cancellationToken: ct); // TODO: await property change

                int delta = building.Consumables.Amounts[index] - currentAmount;
                if (delta > 0)
                {
                    collectedAmount += delta;
                    collectedAmount = Mathf.Clamp(collectedAmount, 0, reqAmount);

                    ScreenSetCollectedAmount(collectedAmount);

                    _vibrationService.Vibrate();
                }

                currentAmount = building.Consumables.Amounts[index];
            }

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
