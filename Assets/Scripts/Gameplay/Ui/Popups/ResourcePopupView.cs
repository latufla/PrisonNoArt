using Honeylab.Pools;
using Honeylab.Utils.Pool;

namespace Honeylab.Gameplay.Ui
{
    public class ResourcePopupView : PopupViewBase<ResourcePopup>
    {
        protected override IGameObjectPool GetPool() => Pools.Get<ResourcePopupsPool>();
    }
}
