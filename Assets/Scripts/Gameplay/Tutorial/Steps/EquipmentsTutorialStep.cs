using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Equipments;
using Honeylab.Gameplay.Ui;
using Honeylab.Utils.Arrows;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.Tutorial
{
    public class EquipmentsTutorialStep : TutorialStepBase
    {
        [SerializeField] private EquipmentId _equipmentId;
        [SerializeField] private int _requiredUpgradeLevel;
        [SerializeField] private TutorialInfo _info;
        private UiArrowsPool _uiArrowsPool;
        private EquipmentsService _equipmentsService;
        private GameplayScreen _gameplayScreen;
        private GameplayScreenPresenter _gameplayScreenPresenter;


        [Inject]
        public void Construct(UiArrowsPool uiArrowsPool,
            EquipmentsService equipmentsService,
            GameplayScreen gameplayScreen,
            GameplayScreenPresenter gameplayScreenPresenter)
        {
            _uiArrowsPool = uiArrowsPool;
            _equipmentsService = equipmentsService;
            _gameplayScreen = gameplayScreen;
            _gameplayScreenPresenter = gameplayScreenPresenter;
        }


        protected override List<Func<UniTask>> CreateSubSteps(CancellationToken ct) => new()
        {
            async () => await WorkAsync(ct)
        };


        private async UniTask WorkAsync(CancellationToken ct)
        {
            SendStepStartAnalytics(_info);

            if (CheckUpgrade())
            {
                return;
            }

            ShowScreen(_info, activeSortingLayer: true);
            using (BlockPlayerInput())
            {
                var presenter = _gameplayScreenPresenter.PlayerInventoryPresenter;
                var hiderScreen = _gameplayScreen.HiderScreen;
                var inventoryButtonPanel = _gameplayScreen.InventoryButton;
                hiderScreen.SetActive(true);

                Transform targetUi = inventoryButtonPanel.transform;
                float uiArrowPosY = (float)_info.ArrowPositionY / 20;
                IArrowHandle arrowUi = ShowTargetUiArrow(targetUi, uiArrowPosY, activeSortingLayer: true);
                if (inventoryButtonPanel.TryGetComponent(out Canvas inv))
                {
                    inv.overrideSorting = true;
                }

                await UniTask.WaitUntil(() => presenter.IsRunning, cancellationToken: ct);

                _uiArrowsPool.HideImmediately(arrowUi);

                HideScreen();

                if (inv != null)
                {
                    inv.overrideSorting = false;
                }

                var inventoryPanel = presenter.Screen.PlayerInventoryPanel;

                UIEquipmentsSlotInfo equipmentSlot = inventoryPanel.GetSlotItem(_equipmentId);

                targetUi = equipmentSlot.transform;
                arrowUi = ShowTargetUiArrow(targetUi, uiArrowPosY, activeSortingLayer: true);


                if (equipmentSlot.TryGetComponent(out Canvas slot))
                {
                    slot.overrideSorting = true;
                }

                EquipmentUpgradePanel equipmentUpgradePanel = inventoryPanel.EquipmentUpgradePanel;

                await UniTask.WaitUntil(() => equipmentUpgradePanel.IsRunning, cancellationToken: ct);

                hiderScreen.SetActive(false);

                _uiArrowsPool.HideImmediately(arrowUi);

                if (slot != null)
                {
                    slot.overrideSorting = false;
                }

                equipmentUpgradePanel.CloseButton.interactable = false;
                equipmentUpgradePanel.BackgroundButton.interactable = false;

                targetUi = equipmentUpgradePanel.UpgradeButton.transform;
                arrowUi = ShowTargetUiArrow(targetUi, uiArrowPosY, activeSortingLayer: true);

                await UniTask.WaitUntil(CheckUpgrade, cancellationToken: ct);

                _uiArrowsPool.HideImmediately(arrowUi);

                equipmentUpgradePanel.CloseButton.interactable = true;
                equipmentUpgradePanel.BackgroundButton.interactable = true;
            }
        }


        private bool CheckUpgrade() =>
            _equipmentsService.GetEquipmentLevel(_equipmentId).Value >= _requiredUpgradeLevel;


        public override TutorialInfo GetTutorialInfo() => _info;


        protected override void OnSubStepComplete()
        {
            SendStepEndAnalytics();
        }
    }
}
