using Honeylab.Gameplay.World;
using Honeylab.Utils.Data;
using UnityEngine;


namespace Honeylab.Gameplay.Ui.Minimap
{
    public class MinimapIndicatorTarget : WorldObjectComponentBase
    {
        [SerializeField] private ScriptableId _minimapIndicatorId;
        [SerializeField] private Sprite _icon;

        public ScriptableId MinimapIndicatorId => _minimapIndicatorId;


        public void UpdateIndicator(MinimapIndicator indicator)
        {
            if (_icon != null)
            {
                indicator.SetIcon(_icon);
            }

            OnUpdateIndicator(indicator);
        }


        protected virtual void OnUpdateIndicator(MinimapIndicator indicator) { }
    }
}
