using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.World;
using System;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using UnityEngine;


namespace Honeylab.Gameplay.Interactables.World
{
    public abstract class InteractableBase : WorldObjectComponentBase, IInteractable
    {
        [SerializeField] private bool _isAgentMoveAllowed;

        private IInteractAgent _agent;

        private readonly Subject<IInteractAgent> _enterInteractSubject = new Subject<IInteractAgent>();
        private readonly Subject<IInteractAgent> _exitInteractSubject = new Subject<IInteractAgent>();
        private readonly Subject<IInteractAgent> _interactSubject = new Subject<IInteractAgent>();

        protected bool IsInteractionActive = true;


        public void EnterInteract(IInteractAgent agent)
        {
            OnEnterInteract(agent);

            _enterInteractSubject.OnNext(agent);
        }


        public async UniTask InteractAsync(IInteractAgent agent, CancellationToken ct)
        {
            if (!IsInited())
            {
                return;
            }

            if (!IsInteractionActive)
            {
                return;
            }

            
            _agent = agent;

            _interactSubject.OnNext(_agent);

            try
            {
                await OnInteractAsync(agent, ct);
            }
            catch (OperationCanceledException)
            {
                _agent = null;
                return;
            }

            _agent = null;
        }


        public void CancelInteract(IInteractAgent agent)
        {
            OnCancelInteract(agent);

            _exitInteractSubject.OnNext(agent);

            _agent = null;
        }


        public void ExitInteract(IInteractAgent agent)
        {
            OnExitInteract(agent);

            _exitInteractSubject.OnNext(agent);

            _agent = null;
        }


        protected virtual void OnEnterInteract(IInteractAgent agent) { }
        protected virtual UniTask OnInteractAsync(IInteractAgent agent, CancellationToken ct) => UniTask.CompletedTask;
        protected virtual void OnCancelInteract(IInteractAgent agent) { }
        protected virtual void OnExitInteract(IInteractAgent agent) { }


        public virtual bool CanInteract(IInteractAgent agent) =>
            _agent == null && (_isAgentMoveAllowed || !agent.IsMoving);


        public bool IsAgentMoveAllowed() => _isAgentMoveAllowed;
        public IObservable<Unit> OnDisabledAsObservable() => this.OnDisableAsObservable();

        public IObservable<IInteractAgent> OnEnterInteractAsObservable() => _enterInteractSubject.AsObservable();
        public IObservable<IInteractAgent> OnExitInteractAsObservable() => _exitInteractSubject.AsObservable();
        public IObservable<IInteractAgent> OnInteractAsObservable() => _interactSubject.AsObservable();


        public void SetInteractionActive(bool isActive) => IsInteractionActive = isActive;
        public bool HasAgent() => _agent != null;
    }
}
