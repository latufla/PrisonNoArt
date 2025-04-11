using Honeylab.Gameplay.Ui.Health;
using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public class HealthPopup : PopupBase
    {
        [SerializeField] private HealthBarView _healthBarView;


        public HealthBarView HealthBarView => _healthBarView;


        public void Clear()
        {
            HealthBarView.Clear();
        }
    }
}
