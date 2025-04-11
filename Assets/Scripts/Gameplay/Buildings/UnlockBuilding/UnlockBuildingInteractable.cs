using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Buildings.View;
using Honeylab.Gameplay.Interactables;
using Honeylab.Gameplay.Interactables.World;
using System.Threading;


namespace Honeylab.Gameplay.Buildings
{
    public class UnlockBuildingInteractable : InteractableBase
    {
        private UnlockBuildingFlow _flow;
        private UnlockBuildingViewBase _view;

        private bool _interacted;


        protected override void OnInit()
        {
            _flow = GetFlow<UnlockBuildingFlow>();
            _view = _flow.Get<UnlockBuildingViewBase>();
        }


        protected override void OnClear()
        {
            _view.HideUnlockPopup();
        }


        public override bool CanInteract(IInteractAgent agent) => base.CanInteract(agent) &&
            _flow.State.Value != UnlockBuildingStates.Unlocked;


        protected override async UniTask OnInteractAsync(IInteractAgent agent, CancellationToken ct)
        {
            if (!_view.IsUnlockPopupShown())
            {
                _view.ShowUnlockPopup();
            }
            else
            {
                _view.UpdateUnlockPopup(_flow.State.Value);
            }

            await UniTask.WaitUntilCanceled(ct);
        }


        protected override void OnExitInteract(IInteractAgent agent)
        {
            if (_flow.State.Value == UnlockBuildingStates.Claim)
            {
                _view.UpdateUnlockPopup(_flow.State.Value);
            }
            else
            {
                _view.HideUnlockPopup();
            }
        }
    }
}
