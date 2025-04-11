using Honeylab.Gameplay.Ui;
using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Gameplay.Buildings
{
    public class CraftBuildingPopup : PopupBase
    {
        [SerializeField] private CraftPopupPanel _craftPanel;
        [SerializeField] private CraftPopupWorkPanel _craftWorkPanel;

        public CraftPopupPanel CraftPanel => _craftPanel;
        public CraftPopupWorkPanel CraftWorkPanel => _craftWorkPanel;
    }
}
