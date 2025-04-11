using Honeylab.Gameplay.Weapons.Upgrades;
using Honeylab.Utils;
using Honeylab.Utils.HitTesting;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Honeylab.Gameplay.Weapons
{
    public class MeleeWeaponBossAttack : MeleeWeaponAttack
    {
        [SerializeField] private LineAreaView _lineAreaView;
        private LineCastProvider _lineSearch;
        private CompositeDisposable _disposable;

        protected override void OnInit()
        {
            base.OnInit();
            _lineSearch = new LineCastProvider();
        }


        protected override void OnRun()
        {
            base.OnRun();
            _disposable = new CompositeDisposable();
            IWeaponAgent agent = Flow.GetAgent();

            IDisposable onStartAttack = OnStartAttackAsObservable()
                .Subscribe(_ =>
                {
                    WeaponUpgradeLevelConfig weaponUpgradeConfig = Flow.UpgradeConfigProp.Value;
                    Vector3 direction = agent.AttackAnchor.forward;
                    float radius = weaponUpgradeConfig.AttackRadius;
                    float width = weaponUpgradeConfig.AttackWidth;

                    if (width <= 0)
                    {
                        return;
                    }

                    _lineAreaView.UpdateView(agent.AttackAnchor, direction, radius, width);
                    _lineAreaView.SetActive(true);
                });
            _disposable.Add(onStartAttack);

            IDisposable onEndAttack = OnEndAttackAsObservable()
                .Subscribe(_ =>
                {
                    _lineAreaView.SetActive(false);
                });
            _disposable.Add(onEndAttack);
        }


        protected override void OnClear()
        {
            _disposable?.Dispose();
            _disposable = null;
            base.OnClear();
        }


        protected override IEnumerable<WeaponAttackTarget> SearchTargets(IWeaponAgent agent,
            WeaponUpgradeLevelConfig weaponUpgradeConfig)
        {
            float radius = weaponUpgradeConfig.AttackRadius;
            float width = weaponUpgradeConfig.AttackWidth;
            if (width <= 0)
            {
                return base.SearchTargets(agent, weaponUpgradeConfig);
            }

            var res = _lineSearch.CalcHits<WeaponAttackTarget>(agent.AttackAnchor, radius, width);
            return res;

        }
    }
}
