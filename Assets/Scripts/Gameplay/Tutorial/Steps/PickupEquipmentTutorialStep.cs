using Cysharp.Threading.Tasks;
using Honeylab.Consumables;
using Honeylab.Gameplay.Buildings;
using Honeylab.Gameplay.Equipments;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils.Arrows;
using Honeylab.Utils.Persistence;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.Tutorial
{
    public class PickupEquipmentTutorialStep : TutorialStepBase
    {
        [SerializeField] private WorldObjectId _worldObjectId;
        [SerializeField] private EquipmentId _pickupableId;
        [SerializeField] private ConsumablePersistenceId _craftConsumableId;
        [SerializeField] private WorldObjectId _unlockWorldId;
        [SerializeField] private TutorialInfo _info;
        [SerializeField] private bool _useExistingAmount;

        private EquipmentsService _equipmentsService;
        private ConsumablesService _consumablesService;

        [Inject]
        public void Construct(EquipmentsService equipmentsService,
            ConsumablesService consumablesService)
        {
            _equipmentsService = equipmentsService;
            _consumablesService = consumablesService;
        }

        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await PickupWorldObjectAsync(ct)
        };

        private async UniTask PickupWorldObjectAsync(CancellationToken ct)
        {
            SendStepStartAnalytics(_info);
            if (_useExistingAmount && CheckConsumable())
            {
                //ShowScreen(_info);
                return;
            }

            Transform target = null;

            if (_worldObjectId != null)
            {
                WorldObjectFlow wo = await GetObjectAsync(_worldObjectId, ct);
                target = wo.transform;
            }

            ShowScreen(_info, target);

            if (_worldObjectId != null)
            {
                if (Resolve<LevelPersistenceService>().TryGet(_worldObjectId, out PersistentObject po))
                {
                    if (po.TryGetFirst<ReactiveValuePersistentComponent<bool>>(out var isDeactive))
                    {
                        if (isDeactive is { Value: true })
                        {
                            return;
                        }
                    }
                }
            }

            IArrowHandle arrow = null;
            if (target != null)
            {
                arrow = ShowTargetArrow(target, _info.ArrowPositionY);
                ShowOffscreenIndicator(target, _info.Icon);

                if (_info.FocusOnStart)
                {
                    FocusTargetAsync(target);
                }
            }

            if (_useExistingAmount && CheckConsumable())
            {
                return;
            }

            await UniTask.WaitUntil(CheckConsumable, cancellationToken: ct);

            if (arrow != null)
            {
                HideTargetArrow(arrow);
            }
            HideOffscreenIndicator();
        }


        private bool CheckConsumable()
        {
            var curLevel = _equipmentsService.GetEquipmentLevel(_pickupableId);
            var curSecondAmountValue = _craftConsumableId != null ? _consumablesService.GetAmountProp(_craftConsumableId).Value : 0;
            bool isUnlocked = false;
            if (_unlockWorldId != null)
            {
                var building = GetObject<UnlockBuildingFlow>(_unlockWorldId);
                if (building != null && building.State.Value == UnlockBuildingStates.Unlocked)
                {
                    isUnlocked = true;
                }
            }
            return curLevel.Value >= 0 || curSecondAmountValue > 0 || isUnlocked;
        }


        public override TutorialInfo GetTutorialInfo() => _info;


        protected override void OnSubStepComplete()
        {
            SendStepEndAnalytics();
        }
    }
}
