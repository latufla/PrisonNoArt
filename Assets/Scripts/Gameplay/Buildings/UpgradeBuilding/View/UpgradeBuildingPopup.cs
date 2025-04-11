using Honeylab.Gameplay.Ui;
using UnityEngine;


namespace Honeylab.Gameplay.Buildings
{
    public class UpgradeBuildingPopup : PopupBase
    {
        [SerializeField] private UpgradePopupPanel _upgradePanel;
        [SerializeField] private UpgradePopupWorkPanel _upgradeWorkPanel;

        public UpgradePopupPanel UpgradePanel => _upgradePanel;
        public UpgradePopupWorkPanel UpgradeWorkPanel => _upgradeWorkPanel;
    }
}
