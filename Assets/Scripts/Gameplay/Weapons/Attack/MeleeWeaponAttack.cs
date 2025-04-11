using Honeylab.Gameplay.Booster;
using Honeylab.Gameplay.Weapons.Upgrades;
using Honeylab.Utils;
using Honeylab.Utils.HitTesting;
using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Gameplay.Weapons
{
    public class MeleeWeaponAttack : WeaponAttackBase
    {
        [SerializeField] private RadialAreaView _attackAreaView;

        private RadialCastProvider _radialSearch;

        private BoosterService _boosterService;


        protected override void OnInit()
        {
            base.OnInit();

            if (_attackAreaView != null)
            {
                _attackAreaView.Init(Flow);
            }

            _boosterService = Flow.Resolve<BoosterService>();
            _radialSearch = new RadialCastProvider();
        }


        protected override void UpdateView(WeaponUpgradeLevelConfig weaponUpgradeConfig, IWeaponAgent agent)
        {
            Vector3 position = agent.AttackAnchor.position;
            Vector3 direction = agent.AttackAnchor.forward;
            float radius = weaponUpgradeConfig.AttackRadius;
            float angle = weaponUpgradeConfig.AttackAngle;

            if (_attackAreaView != null)
            {
                _attackAreaView.UpdateView(position, direction, radius, angle);
            }
        }


        protected override IEnumerable<WeaponAttackTarget> SearchTargets(IWeaponAgent agent,
            WeaponUpgradeLevelConfig weaponUpgradeConfig)
        {
            Vector3 position = agent.AttackAnchor.position;
            Vector3 direction = agent.AttackAnchor.forward;
            float radius = weaponUpgradeConfig.AttackRadius;
            float angle = weaponUpgradeConfig.AttackAngle;
            var result = _radialSearch.CalcHits<WeaponAttackTarget>(position, direction, radius, angle);
            return result;
        }


        protected override int GetDamageMultiplier() => _boosterService.GetDamageBoost();


        protected override Vector3 GetAttackAnchorPosition(IWeaponAgent agent) => agent.AttackAnchor.position;
    }
}
