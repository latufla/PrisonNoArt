using Honeylab.Gameplay.Ui;
using Honeylab.Pools;
using Honeylab.Utils.Pool;


namespace Honeylab.Gameplay.Buildings
{
    public class GasBuildingPopupView : PopupViewBase<GasBuildingPopup>
    {
        protected override IGameObjectPool GetPool() => Pools.Get<GasBuildingPopupsPool>();
    }
}
