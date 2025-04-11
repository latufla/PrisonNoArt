using Honeylab.Gameplay.World;
using Honeylab.Utils.Extensions;
using System;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Creatures
{
    public class CreatureAnimatorProvider : AnimatorProvider
    {
        [SerializeField] private Transform _leftStepAnchor;
        [SerializeField] private Transform _rightStepAnchor;
        [SerializeField] private Transform _deathAnchor;

        private readonly Subject<Vector3> _stepsSubject = new();
        public IObservable<Vector3> OnStepAsObservable() => _stepsSubject.AsObservable();

        private readonly Subject<Unit> _weaponHitSubject = new();
        public IObservable<Unit> OnWeaponHitAsObservable() => _weaponHitSubject.AsObservable();

        private readonly Subject<Unit> _weaponHitEndSubject = new();
        public IObservable<Unit> OnWeaponHitEndAsObservable() => _weaponHitEndSubject.AsObservable();

        public Transform DeathAnchor => _deathAnchor;


        public void Animator_Internal_OnStepLeft() => _stepsSubject.OnNext(_leftStepAnchor.position);
        public void Animator_Internal_OnStepRight() => _stepsSubject.OnNext(_rightStepAnchor.position);

        public void Animator_Internal_Weapon_Hit() => _weaponHitSubject.OnNext();
        public void Animator_Internal_Weapon_HitEnd() => _weaponHitEndSubject.OnNext();
    }
}
