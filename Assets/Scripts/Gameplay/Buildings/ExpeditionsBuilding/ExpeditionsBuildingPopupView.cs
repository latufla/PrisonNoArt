using Honeylab.Gameplay.Ui;
using Honeylab.Pools;
using Honeylab.Utils.Pool;


namespace Honeylab.Gameplay.Buildings
{
    public class ExpeditionsBuildingPopupView : PopupViewBase<ExpeditionsBuildingPopup>
    {
        protected override IGameObjectPool GetPool() => Pools.Get<ExpeditionsBuildingPopupsPool>();
    }
}
