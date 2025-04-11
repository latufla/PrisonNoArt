using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Gameplay.Ui;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Buildings
{
    public class GasBuildingPopup : PopupBase
    {
        [SerializeField] private GasPopupWorkPanel _craftWorkPanel;
        [SerializeField] private GameObject _craftResultPanel;
        [SerializeField] private GameObject _craftResultFullPanel;
        [SerializeField] private GameObject _craftIconPanel;
        [SerializeField] private Image _craftIcon;
        [SerializeField] private Image _craftResultIcon;
        [SerializeField] private Image _craftGetIcon;

        public GasPopupWorkPanel CraftWorkPanel => _craftWorkPanel;


        public void ShowWork(CraftBuildingFlow flow)
        {
            CraftBuildingInteractable interactable = flow.Get<CraftBuildingInteractable>();
            bool isCollect = interactable != null && interactable.HasAgent();
            bool isFull = flow.GetCurrentAmount() >= flow.CraftAmount.Value;

            _craftResultPanel.SetActive(!isCollect);
            _craftResultFullPanel.SetActive(!isCollect && isFull);

            CraftWorkPanel.gameObject.SetActive(isCollect);
            _craftIconPanel.gameObject.SetActive(isCollect);
            CraftWorkPanel.GetConsumablesButton.gameObject.SetActive(isCollect);
            if (isCollect)
            {
                CraftWorkPanel.ShowWork(flow);
            }
            SetImage(flow);
        }


        private void SetImage(CraftBuildingFlow flow)
        {
            var consumablesData = flow.Resolve<ConsumablesData>();
            RewardAmountConfig result = flow.Config.CraftResult;
            ConsumableData resultData = consumablesData.GetData(result.Name);
            _craftIcon.sprite = resultData.Sprite;
            _craftResultIcon.sprite = resultData.Sprite;
            _craftGetIcon.sprite = resultData.Sprite;
        }
    }
}
