using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Weapons.Upgrades;
using Honeylab.Utils;
using Honeylab.Utils.HitTesting;
using Honeylab.Utils.Vfx;
using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Gameplay.Weapons
{
    public class RangeWeaponAttack : WeaponAttackBase
    {
        [SerializeField] private CircleStrokeAreaView _attackAreaView;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private VfxId _hitMuzzleVfxId;
        [SerializeField] private int _maxTargetsCount = 1;

        private VfxService _vfxService;
        private SphereCastProvider _sphereSearch;


        protected override void OnInit()
        {
            base.OnInit();

            _vfxService = Flow.Resolve<VfxService>();
            _sphereSearch = new();
        }


        protected override void OnRun()
        {
            base.OnRun();

            IWeaponAgent agent = Flow.GetAgent();
            agent.WeaponAttackTargetView.ShowHealthPopup();
        }


        protected override void OnStop()
        {
            IWeaponAgent agent = Flow.GetAgent();
            agent.WeaponAttackTargetView.HideHealthPopup();

            base.OnStop();
        }


        protected override void OnHit()
        {
            _vfxService.PlayOnceAsync(_hitMuzzleVfxId, _muzzle).Forget();
        }


        protected override IEnumerable<WeaponAttackTarget> SearchTargets(IWeaponAgent agent,
            WeaponUpgradeLevelConfig weaponUpgradeConfig)
        {
            Vector3 position = GetAttackAnchorPosition(agent);
            float radius = weaponUpgradeConfig.AttackRadius;
            var result = _sphereSearch.CalcHits<WeaponAttackTarget>(position, radius, true);
            return result;
        }


        protected override List<WeaponAttackTarget> FilterSearchTargets(List<WeaponAttackTarget> targets)
        {
            int targetsCount = Mathf.Min(_maxTargetsCount, targets.Count);
            var result = targets.GetRange(0, targetsCount);
            return result;
        }


        protected override int GetDamageMultiplier() => 1;


        protected override Vector3 GetAttackAnchorPosition(IWeaponAgent agent) => agent.RangeAttackAnchor.position;


        protected override void UpdateView(WeaponUpgradeLevelConfig weaponUpgradeConfig, IWeaponAgent agent)
        {
            Vector3 position = GetAttackAnchorPosition(agent);
            float radius = weaponUpgradeConfig.AttackRadius;

            if (_attackAreaView != null)
            {
                _attackAreaView.UpdateView(position, radius, 30);
            }
        }
    }
}
