using Honeylab.Gameplay.Ui;
using Honeylab.Pools;
using Honeylab.Utils.Pool;


namespace Honeylab.Gameplay.Buildings.View
{
    public class UnlockBuildingPopupView : PopupViewBase<UnlockBuildingPopup>
    {
        protected override IGameObjectPool GetPool() => Pools.Get<UnlockBuildingPopupsPool>();
    }
}
