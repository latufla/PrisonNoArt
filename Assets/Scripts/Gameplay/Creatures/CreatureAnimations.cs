using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.World;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Creatures
{
    public class CreatureAnimations : WorldObjectAnimationsBase
    {
        private static readonly int Idle = Animator.StringToHash(nameof(Idle));
        private static readonly int Walk = Animator.StringToHash(nameof(Walk));
        private static readonly int WeaponHit = Animator.StringToHash(nameof(WeaponHit));
        private static readonly int WeaponHitCancel = Animator.StringToHash(nameof(WeaponHitCancel));
        private static readonly int Die = Animator.StringToHash(nameof(Die));
        private static readonly int GunWalk = Animator.StringToHash(nameof(GunWalk));
        private static readonly int MoveX = Animator.StringToHash(nameof(MoveX));
        private static readonly int MoveY = Animator.StringToHash(nameof(MoveY));

        [SerializeField] private List<WeaponAttackAnimation> _weaponHitAnimations;

        private AnimatorOverrideController _controller;
        private int _weaponHitClipIndex;


        public ReactiveProperty<bool> IsWeaponHit { get; } = new();


        public IObservable<Vector3> OnStepAsObservable() =>
            GetAnimatorProvider<CreatureAnimatorProvider>().OnStepAsObservable();


        public IObservable<Unit> OnWeaponHitAsObservable() =>
            GetAnimatorProvider<CreatureAnimatorProvider>().OnWeaponHitAsObservable();


        public IObservable<Unit> OnWeaponHitEndAsObservable() =>
            GetAnimatorProvider<CreatureAnimatorProvider>().OnWeaponHitEndAsObservable();


        public void PlayIdle()
        {
            CreatureAnimatorProvider animator = GetAnimatorProvider<CreatureAnimatorProvider>();
            ChangeState(Idle);
        }


        public void PlayWalk()
        {
            ChangeState(Walk);
            CreatureAnimatorProvider animator = GetAnimatorProvider<CreatureAnimatorProvider>();
        }

        public void PlayGunWalk(float x, float y)
        {
            ChangeState(GunWalk);
            CreatureAnimatorProvider animator = GetAnimatorProvider<CreatureAnimatorProvider>();
            animator.Animator.SetFloat(MoveX, x);
            animator.Animator.SetFloat(MoveY, y);
        }


        public void PlayWeaponHit(WorldObjectId weaponId, WeaponAttackAnimation weaponAnimationInfo = null)
        {
            CreatureAnimatorProvider animator = GetAnimatorProvider<CreatureAnimatorProvider>();
            if (_weaponHitAnimations == null && weaponAnimationInfo == null)
            {
                animator.Animator.SetTrigger(WeaponHit);
                IsWeaponHit.Value = true;

                return;
            }

            _controller = SetAnimatorOverrideController();

            WeaponAttackAnimation animInfo = weaponAnimationInfo ??
                _weaponHitAnimations.FirstOrDefault(it => it.WeaponId.Equals(weaponId));
            if (animInfo == null)
            {
                animator.Animator.SetTrigger(WeaponHit);
                IsWeaponHit.Value = true;

                return;
            }

            if (_weaponHitClipIndex >= animInfo.AnimationOverrides.Count)
            {
                _weaponHitClipIndex = 0;
            }

            AnimationClip clip = animInfo.AnimationOverrides[_weaponHitClipIndex];
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>
            {
                new(animInfo.Animation, clip)
            };
            _controller.ApplyOverrides(anims);

            if (!IsWeaponHit.Value)
            {
                animator.Animator.SetTrigger(WeaponHit);
                animator.Animator.Update(0.0f);

                IsWeaponHit.Value = true;
            }

            _weaponHitClipIndex++;
        }


        public void ClearWeaponHit(bool cancelAnimation = true)
        {
            if (!IsWeaponHit.Value)
            {
                return;
            }

            if (cancelAnimation)
            {
                CreatureAnimatorProvider animator = GetAnimatorProvider<CreatureAnimatorProvider>();
                animator.Animator.SetTrigger(WeaponHitCancel);
                animator.Animator.Update(0.0f);
            }

            _weaponHitClipIndex = 0;
            IsWeaponHit.Value = false;
        }


        public Transform PlayDie()
        {
            CreatureAnimatorProvider animator = GetAnimatorProvider<CreatureAnimatorProvider>();
            var layerCount = animator.Animator.layerCount;

            for (int i = 1; i < layerCount; i++)
            {
                animator.Animator.SetLayerWeight(i, 0);
            }

            ChangeState(Die);

            return animator.DeathAnchor;
        }


        private AnimatorOverrideController SetAnimatorOverrideController()
        {
            if (_controller != null)
            {
                return _controller;
            }

            AnimatorProvider animatorProvider = GetAnimatorProvider<AnimatorProvider>();
            AnimatorOverrideController controller = new(animatorProvider.Animator.runtimeAnimatorController);
            animatorProvider.Animator.runtimeAnimatorController = controller;
            return controller;
        }
    }
}
