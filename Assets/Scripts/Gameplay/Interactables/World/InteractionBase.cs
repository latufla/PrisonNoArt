using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Data;
using Honeylab.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Interactables.World
{
    public abstract class InteractionBase : WorldObjectComponentBase, IInteractAgent
    {
        [SerializeField] private InteractableTrigger _trigger;

        private CompositeDisposable _run;

        private readonly Dictionary<IInteractable, CancellationTokenSource> _interactables =
            new Dictionary<IInteractable, CancellationTokenSource>();


        protected override void OnRun()
        {
            _run = new CompositeDisposable();

            IDisposable enter = _trigger.OnEnterAsObservable()
                .Subscribe(interactable => { interactable.EnterInteract(this); });
            _run.Add(enter);

            IDisposable stay = _trigger.OnStayAsObservable()
                .Select(it => it.DetectedObject)
                .Subscribe(interactable =>
                {
                    if (_interactables.ContainsKey(interactable) && !interactable.IsAgentMoveAllowed() && IsMoving)
                    {
                        interactable.CancelInteract(this);

                        _interactables[interactable].CancelThenDispose();
                        _interactables.Remove(interactable);

                        return;
                    }

                    if (interactable.CanInteract(this))
                    {
                        var cts = new CancellationTokenSource();

                        if (!_interactables.TryAdd(interactable, cts))
                        {
                            _interactables[interactable] = cts;
                        }

                        interactable.InteractAsync(this, cts.Token).Forget();
                    }
                });
            _run.Add(stay);

            IDisposable exit = _trigger.OnExitAsObservable()
                .Subscribe(interactable =>
                {
                    if (_interactables.TryGetValue(interactable, out CancellationTokenSource cts))
                    {
                        cts.CancelThenDispose();
                        _interactables.Remove(interactable);
                    }

                    interactable.ExitInteract(this);
                });
            _run.Add(exit);
        }


        protected override void OnStop()
        {
            _run?.Dispose();
            _run = null;

            foreach ((IInteractable interactable, CancellationTokenSource cts) in _interactables)
            {
                cts.CancelThenDispose();
                interactable.ExitInteract(this);
            }

            _interactables.Clear();
        }


        public void ExitInteract()
        {
            foreach ((IInteractable interactable, CancellationTokenSource cts) in _interactables)
            {
                cts.CancelThenDispose();
                interactable.ExitInteract(this);
            }
            _interactables.Clear();

            var enteredObjects = new List<IInteractable>(_trigger.EnumerateEnteredObjects());
            foreach (var obj in enteredObjects)
            {
                _trigger.TryRemove(obj);
            }
        }

        public abstract bool IsMoving { get; }
        public abstract Transform ConsumablesOutAnchor { get; }
        public ScriptableId Id => GetFlow().Id;
        public abstract IWeaponAgent WeaponAgent { get; }
    }
}
