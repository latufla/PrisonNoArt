using Honeylab.Gameplay.Ui;
using UnityEngine;


namespace Honeylab.Gameplay.Buildings
{
    public class ExpeditionsBuildingPopup : PopupBase
    {
        [SerializeField] private GameObject _outOfFuelPanel;


        public void ShowPopup(UnlockBuildingFlow flow, bool forceOutOfFuel = false)
        {
            UnlockBuildingInteractable interactable = flow.Get<UnlockBuildingInteractable>();
            bool isInteract = interactable != null && interactable.HasAgent();

            _outOfFuelPanel.SetActive(!isInteract || forceOutOfFuel);
        }


        public void Hide()
        {
            _outOfFuelPanel.SetActive(false);
        }
    }
}
