using UnityEngine;


namespace Honeylab.Gameplay.Ui.Minimap
{
    public class PlayerMinimapIndicatorTarget : MinimapIndicatorTarget
    {
        protected override void OnUpdateIndicator(MinimapIndicator indicator)
        {
            indicator.SetAxis(BillboardAxis.X | BillboardAxis.Z);

            Vector3 direction = transform.forward;
            indicator.transform.forward = direction;
        }
    }
}
