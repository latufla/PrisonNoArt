using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Interactables;
using Honeylab.Gameplay.Interactables.World;
using System.Threading;


namespace Honeylab.Gameplay.Buildings
{
    public class CraftBuildingInteractable : InteractableBase
    {
        private CraftBuildingViewBase _view;
        private CraftBuildingFlow _flow;


        protected override void OnInit()
        {
            _flow = GetFlow<CraftBuildingFlow>();
            _view = _flow.Get<CraftBuildingViewBase>();
        }


        protected override void OnClear()
        {
            _view.HideCraftPopup();
        }


        protected override async UniTask OnInteractAsync(IInteractAgent agent, CancellationToken ct)
        {
            if (!_view.IsCraftPopupShown())
            {
                _view.ShowCraftPopup();
            }
            else
            {
                _view.UpdateCraftPopup(_flow.State.Value);
            }

            await UniTask.WaitUntilCanceled(ct);
        }


        protected override void OnExitInteract(IInteractAgent agent)
        {
            if (_flow.State.Value == CraftBuildingStates.Done)
            {
                _view.UpdateCraftPopup(_flow.State.Value);
            }
            else
            {
                _view.HideCraftPopup();
            }
        }
    }
}
