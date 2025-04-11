using Honeylab.Gameplay.Weapons.Upgrades;
using Honeylab.Utils.HitTesting;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Honeylab.Gameplay.Weapons
{
    [Serializable]
    public class VacuumCleanerEffects
    {
        [field: SerializeField]
        public MeshRenderer WaterfallMesh { get; private set; }
        [field: SerializeField]
        public ParticleSystem GunEffect { get; private set; }

        public void SetActive(bool value)
        {
            WaterfallMesh.gameObject.SetActive(value);
            GunEffect.gameObject.SetActive(value);
        }
    }

    public class VacuumCleanerSuction : WeaponAttackBase
    {
        [SerializeField]
        private List<VacuumCleanerEffects> _weaponEffects;

        private RadialCastProvider _radialSearch;


        protected override void OnInit()
        {
            base.OnInit();
            _radialSearch = new();
            _weaponEffects.ForEach(x => x.SetActive(false));
        }


        protected override int GetDamageMultiplier() => 1;


        protected override Vector3 GetAttackAnchorPosition(IWeaponAgent agent) => agent.RangeAttackAnchor.position;


        protected override IEnumerable<WeaponAttackTarget> SearchTargets(IWeaponAgent agent, WeaponUpgradeLevelConfig weaponUpgradeConfig)
        {
            Vector3 position = agent.AttackAnchor.position;
            Vector3 direction = agent.AttackAnchor.forward;
            float radius = weaponUpgradeConfig.AttackRadius;
            float angle = weaponUpgradeConfig.AttackAngle;
            var result = _radialSearch.CalcHits<WeaponAttackTarget>(position, direction, radius, angle);
            return result;
        }


        protected override void OnHit(WeaponAttackTargetConfig config)
        {
            foreach (var waterfall in _weaponEffects)
            {
                int index = _weaponEffects.IndexOf(waterfall) + 1;

                if (config.WeaponLevel == index)
                {
                    waterfall.SetActive(true);
                }
                else
                {
                    waterfall.SetActive(false);
                }
            }
        }


        protected override void OnClearHit()
        {
            _weaponEffects.ForEach(x => x.SetActive(false));
        }
    }
}
