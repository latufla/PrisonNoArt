using UnityEngine;

namespace Honeylab.Gameplay.Ui.Health
{
    public class HealthBarScreen : ScreenBase
    {
        [SerializeField] private HealthBarView _view;
        public override string Name => ScreenName.Health;

        public HealthBarView View => _view;
    }
}
