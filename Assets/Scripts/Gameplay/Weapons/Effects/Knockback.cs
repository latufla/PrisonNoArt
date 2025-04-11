using Cysharp.Threading.Tasks;
using DG.Tweening;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.Weapons.Upgrades;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Extensions;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.Creatures
{
    public class Knockback : WorldObjectComponentBase
    {
        private PlayerMotion _playerMotion;
        private CancellationTokenSource _knockbackCts;
        public bool IsKnockback => _knockbackCts != null;


        protected override void OnInit()
        {
            GetFlow().TryGet(out _playerMotion);
        }


        public void PlayKnockback(WeaponFlow weapon)
        {
            WeaponUpgradeLevelConfig config = weapon.UpgradeConfigProp.Value;
            Vector3 fromPosition = weapon.GetAgent().Transform.position;

            PlayKnockbackWork(fromPosition, config.KnockbackPower, config.KnockbackDuration)
                .Forget();
        }


        private async UniTask PlayKnockbackWork(Vector3 fromPosition, float power, float duration)
        {
            if (power < 0.001f || duration < 0.001f)
            {
                return;
            }

            if (IsKnockback)
            {
                return;
            }

            WeaponFlow weapon = null;
            if (GetFlow().TryGet(out WeaponAgentBase weaponAgentBase))
            {
                weapon = weaponAgentBase.Weapon;
            }

            if (weapon != null)
            {
                WeaponAttackBase attack = weapon.Get<WeaponAttackBase>();
                attack.PauseAttack();
            }

            _knockbackCts?.CancelThenDispose();
            _knockbackCts = new CancellationTokenSource();

            CancellationToken ct = _knockbackCts.Token;

            Vector3 direction = transform.position - fromPosition;
            direction.y = 0.0f;
            direction.Normalize();
            Vector3 movement = transform.position + direction * power;


            if (_playerMotion == null)
            {
                await GetFlow()
                    .transform.DOMove(movement, duration)
                    .SetUpdate(UpdateType.Fixed)
                    .ToUniTask(cancellationToken: ct);
            }
            else
            {
                float powerRed = power / (duration / Time.fixedDeltaTime);
                while (duration > 0)
                {
                    _playerMotion.UpdateMove(power, direction, Time.fixedDeltaTime);
                    power -= powerRed;
                    duration -= Time.fixedDeltaTime;
                    await UniTask.Yield(ct);
                }
            }

            _knockbackCts?.CancelThenDispose();
            _knockbackCts = null;

            if (weapon != null)
            {
                WeaponAttackBase attack = weapon.Get<WeaponAttackBase>();
                if (attack != null)
                {
                    attack.ResumeAttack();
                }
            }
        }


        protected override void OnClear()
        {
            _knockbackCts?.CancelThenDispose();
            _knockbackCts = null;
        }
    }
}
