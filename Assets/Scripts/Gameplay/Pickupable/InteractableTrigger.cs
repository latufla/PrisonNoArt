using Honeylab.Gameplay.Interactables;
using Honeylab.Gameplay.Interactables.World;


namespace Honeylab.Gameplay.Pickup
{
    public class InteractableTrigger : InteractableBase
    {
        public bool IsOnTrigger { get; private set; }


        protected override void OnEnterInteract(IInteractAgent agent)
        {
            IsOnTrigger = true;
        }


        protected override void OnExitInteract(IInteractAgent agent)
        {
            IsOnTrigger = false;
        }


        protected override void OnCancelInteract(IInteractAgent agent)
        {
            IsOnTrigger = false;
        }
    }
}
