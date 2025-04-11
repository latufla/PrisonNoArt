using Honeylab.Gameplay.Ui;
using Honeylab.Pools;
using Honeylab.Utils.Pool;


namespace Honeylab.Gameplay.Buildings
{
    public class CraftBuildingPopupView : PopupViewBase<CraftBuildingPopup>
    {
        protected override IGameObjectPool GetPool() => Pools.Get<CraftBuildingPopupsPool>();
    }
}
