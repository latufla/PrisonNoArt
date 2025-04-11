using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Booster;
using Honeylab.Gameplay.Weapons.Upgrades;
using Honeylab.Gameplay.World;
using Honeylab.Sounds.Data;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.HitTesting;
using Honeylab.Utils.Vfx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Weapons
{
    public abstract class WeaponAttackBase : WorldObjectComponentBase
    {
        [SerializeField] private WorldObjectIdProvider _weaponTypeId;
        [SerializeField] private VfxId _hitVfxId;
        [SerializeField] private SoundId _hitSoundId;
        [SerializeField] private bool _rotateToTarget;
        [SerializeField] private bool _attackWhileMoving = true;
        [SerializeField] private int _hitsCount = 1;

        private readonly List<WeaponAttackTarget> _targets = new(64);

        private readonly Subject<Unit> _onStartAttack = new Subject<Unit>();
        private readonly Subject<Unit> _onEndAttack = new Subject<Unit>();

        public IObservable<Unit> OnStartAttackAsObservable() => _onStartAttack.AsObservable();
        public IObservable<Unit> OnEndAttackAsObservable() => _onEndAttack.AsObservable();

        protected WeaponFlow Flow;

        private CancellationTokenSource _cts;
        private RayсastProvider _walls;
        private bool _isAttackPaused;


        public VfxId HitVfxId => _hitVfxId;
        public SoundId HitSoundId => _hitSoundId;


        protected override void OnInit()
        {
            Flow = GetFlow<WeaponFlow>();

            _walls = new(100.0f, LayerMask.GetMask("Walls"));
        }


        protected override void OnRun()
        {
            _cts = new CancellationTokenSource();
            CancellationToken ct = _cts.Token;

            RunSearchAsync(Flow.UpgradeConfigProp, ct).Forget();
            RunAttackAsync(ct).Forget();
        }


        protected override void OnStop()
        {
            _cts?.CancelThenDispose();
            _cts = null;
            _targets.Clear();
        }


        private async UniTask RunSearchAsync(ReactiveProperty<WeaponUpgradeLevelConfig> upgradeConfigProp,
            CancellationToken ct)
        {
            while (ct.IsCancellationRequested is false)
            {
                await UniTask.Yield(ct);

                IWeaponAgent agent = Flow.GetAgent();
                UpdateView(upgradeConfigProp.Value, agent);

                _targets.Clear();

                if (!_attackWhileMoving && agent.IsMoving)
                {
                    continue;
                }

                Vector3 attackAnchorPosition = GetAttackAnchorPosition(agent);
                var hits = SearchTargets(agent, upgradeConfigProp.Value)
                    .Where(target =>
                    {
                        if (target == agent.SelfAttackTarget || !target.WeaponTypeIds.Contains(_weaponTypeId.Id))
                        {
                            return false;
                        }

                        if (!target.CanHit())
                        {
                            return false;
                        }

                        Vector3 targetDirection = (target.transform.position - attackAnchorPosition).normalized;
                        var wallHits = _walls.CalcHits(attackAnchorPosition, targetDirection);
                        if (!wallHits.Any())
                        {
                            return true;
                        }

                        RaycastHit wallHit = wallHits.First();
                        return Vector3.Distance(attackAnchorPosition, wallHit.point) >
                            Vector3.Distance(attackAnchorPosition, target.transform.position);
                    })
                    .ToList();

                var targets = FilterSearchTargets(hits);
                _targets.AddRange(targets);
            }
        }


        public void PauseAttack()
        {
            _isAttackPaused = true;
        }


        public void ResumeAttack()
        {
            _isAttackPaused = false;
        }


        private async UniTask RunAttackAsync(CancellationToken ct)
        {
            while (ct.IsCancellationRequested is false)
            {
                IWeaponAgent agent = Flow.GetAgent();
                if (agent == null)
                {
                    await UniTask.Yield(ct);
                    continue;
                }

                var targets = _targets.Where(it => it.CanHit());
                if (!targets.Any())
                {
                    agent.ClearWeaponHit();
                    OnClearHit();
                    await UniTask.Yield(ct);
                    ResumeAttack();
                    continue;
                }

                if (!_attackWhileMoving && agent.IsMoving)
                {
                    await UniTask.Yield(ct);
                    continue;
                }

                if (_isAttackPaused)
                {
                    await UniTask.Yield(ct);
                    continue;
                }


                agent.PlayWeaponHit(_targets, Flow.Config.RotationSpeed, _rotateToTarget);
                _onStartAttack.OnNext();

                for (int i = 0; i < _hitsCount; i++)
                {
                    await agent.OnWeaponHitAsObservable().ToUniTask(true, ct);
                    if (_isAttackPaused)
                    {
                        continue;
                    }

                    OnHit();

                    targets = _targets.Where(it => it.CanHit());
                    foreach (WeaponAttackTarget t in targets)
                    {
                        t.Hit(Flow, damageMultiplier: GetDamageMultiplier());
                        OnHit(t.Config);
                    }
                }

                if (_isAttackPaused)
                {
                    continue;
                }

                await agent.OnWeaponHitEndAsObservable().ToUniTask(true, ct);
                _onEndAttack.OnNext();
            }
        }


        protected abstract int GetDamageMultiplier();
        protected abstract Vector3 GetAttackAnchorPosition(IWeaponAgent agent);

        protected virtual void UpdateView(WeaponUpgradeLevelConfig weaponUpgradeConfig, IWeaponAgent agent) { }


        protected abstract IEnumerable<WeaponAttackTarget> SearchTargets(IWeaponAgent agent,
            WeaponUpgradeLevelConfig weaponUpgradeConfig);


        protected virtual List<WeaponAttackTarget> FilterSearchTargets(List<WeaponAttackTarget> targets) => targets;


        protected virtual void OnHit() { }


        protected virtual void OnHit(WeaponAttackTargetConfig config) { }


        protected virtual void OnClearHit() { }


        private void OnDrawGizmos()
        {
            _walls?.DrawGizmos();
        }
    }
}
