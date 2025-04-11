using Honeylab.Pools;
using Honeylab.Utils.Pool;


namespace Honeylab.Gameplay.Ui
{
    public class HealthPopupView : PopupViewBase<HealthPopup>
    {
        protected override IGameObjectPool GetPool() => Pools.Get<HealthPopupsPool>();
    }
}
