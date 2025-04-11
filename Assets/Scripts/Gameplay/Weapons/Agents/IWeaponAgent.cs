using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.World;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Weapons
{
    public interface IWeaponAgent
    {
        WorldObjectId GetId { get; }
        Transform Transform { get; }
        Transform AttackAnchor { get; }
        Transform RangeAttackAnchor { get; }

        Transform GetSlot(int index);

        void PlayWeaponHit(IEnumerable<WeaponAttackTarget> targets, float rotationSpeed, bool rotateToTarget = false);
        void ClearWeaponHit();

        IObservable<Unit> OnWeaponHitAsObservable();
        IObservable<Unit> OnWeaponHitEndAsObservable();

        WeaponAttackTarget SelfAttackTarget { get; }
        bool IsMoving { get; }

        bool TryAddWeapon(string weaponName);
        bool TryAddWeapon(WorldObjectId weaponId);

        WeaponAttackTargetView WeaponAttackTargetView { get; }
    }
}
