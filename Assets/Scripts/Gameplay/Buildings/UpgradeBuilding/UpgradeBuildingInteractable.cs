using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Interactables;
using Honeylab.Gameplay.Interactables.World;
using System.Threading;


namespace Honeylab.Gameplay.Buildings
{
    public class UpgradeBuildingInteractable : InteractableBase
    {
        private UpgradeBuildingView _view;
        private UpgradeBuildingFlow _flow;


        protected override void OnInit()
        {
            _flow = GetFlow<UpgradeBuildingFlow>();
            _view = _flow.Get<UpgradeBuildingView>();
        }


        protected override void OnClear()
        {
            _view.HideUpgradePopup();
        }


        protected override async UniTask OnInteractAsync(IInteractAgent agent, CancellationToken ct)
        {
            ShowUpgradePopup();
            await UniTask.WaitUntilCanceled(ct);
        }


        protected override void OnExitInteract(IInteractAgent agent)
        {
            HideUpgradePopup();
        }


        public void ShowUpgradePopup()
        {
            if (!_view.IsUpgradePopupShown())
            {
                _view.ShowUpgradePopup();
            }
            else
            {
                _view.UpdateUpgradePopup(_flow.State.Value);
            }
        }


        public void HideUpgradePopup()
        {
            if (_flow.State.Value == UpgradeBuildingStates.Done)
            {
                _view.UpdateUpgradePopup(_flow.State.Value);
            }
            else
            {
                _view.HideUpgradePopup();
            }
        }
    }
}
