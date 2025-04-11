using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;


namespace Honeylab.Gameplay.Interactables
{
    public interface IInteractable
    {
        void EnterInteract(IInteractAgent agent);
        UniTask InteractAsync(IInteractAgent agent, CancellationToken ct);
        void CancelInteract(IInteractAgent agent);
        void ExitInteract(IInteractAgent agent);

        bool CanInteract(IInteractAgent agent);
        bool IsAgentMoveAllowed();

        IObservable<Unit> OnDisabledAsObservable();
    }
}
