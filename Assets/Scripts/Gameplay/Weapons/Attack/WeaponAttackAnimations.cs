using Honeylab.Gameplay.World;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Gameplay.Weapons
{
    [Serializable]
    public class WeaponAttackAnimation
    {
        public WorldObjectId WeaponId;
        public AnimationClip Animation;
        public List<AnimationClip> AnimationOverrides;
    }


    public class WeaponAttackAnimations : WorldObjectComponentBase
    {
        [SerializeField] private WeaponAttackAnimation _attackAnimation;

        public WeaponAttackAnimation AttackAnimation => _attackAnimation;
    }
}
