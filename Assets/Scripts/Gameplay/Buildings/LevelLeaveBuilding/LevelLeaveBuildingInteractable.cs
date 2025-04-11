using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Interactables;
using Honeylab.Gameplay.Interactables.World;
using Honeylab.Gameplay.World;
using System.Threading;


namespace Honeylab.Gameplay.Buildings
{
    public class LevelLeaveBuildingInteractable : InteractableBase
    {
        private LevelLeaveBuildingView _view;


        protected override void OnInit()
        {
            WorldObjectFlow flow = GetFlow<WorldObjectFlow>();
            _view = flow.Get<LevelLeaveBuildingView>();
        }


        protected override void OnClear()
        {
            _view.HideLevelLeavePopup();
        }


        protected override async UniTask OnInteractAsync(IInteractAgent agent, CancellationToken ct)
        {
            if (!_view.IsLevelLeavePopupShown())
            {
                _view.ShowLevelLeavePopup();
            }

            await UniTask.WaitUntilCanceled(ct);
        }


        protected override void OnExitInteract(IInteractAgent agent)
        {
            _view.HideLevelLeavePopup();
        }
    }
}
