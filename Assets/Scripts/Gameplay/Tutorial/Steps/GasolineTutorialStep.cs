using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Gameplay.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Honeylab.Gameplay.Tutorial
{
    public class GasolineTutorialStep : TutorialStepBase
    {
        [SerializeField] private ConsumablePersistenceId _consumableId;
        [SerializeField] private TutorialInfo _info;
        [SerializeField] private bool _useExistingAmount;


        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await CollectAsync(ct)
        };


        private async UniTask CollectAsync(CancellationToken ct)
        {
            var amountProp = GetConsumableAmountProp(_consumableId);
            int amountToCollect = _info.MaxAmountToCollect;
            int collectedAmount = 0;
            SendStepStartAnalytics(_info);
            if (_useExistingAmount)
            {
                if (amountProp.Value >= amountToCollect)
                {
                    //ShowScreen(_info);
                    //ScreenSetCollectedAmount(amountToCollect);
                    return;
                }

                collectedAmount = amountProp.Value;
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

            ShowScreen(_info, target);
            ScreenSetCollectedAmount(collectedAmount);

            if (_info.FocusOnStart)
            {
                FocusTargetAsync(target);
            }

            while (collectedAmount < amountToCollect)
            {
                int amount = amountProp.Value;

                await UniTask.WaitUntil(() => amountProp.Value != amount,
                    cancellationToken: ct);

                int delta = amountProp.Value - amount;
                if (delta > 0)
                {
                    collectedAmount += delta;
                    collectedAmount = Mathf.Clamp(collectedAmount, 0, amountToCollect);

                    ScreenSetCollectedAmount(collectedAmount);
                }
            }
        }


        public override TutorialInfo GetTutorialInfo() => _info;


        protected override void OnSubStepComplete()
        {
            SendStepEndAnalytics();
        }
    }
}
