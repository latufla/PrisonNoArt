using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.World;
using Honeylab.Utils;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.HitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


namespace Honeylab.Gameplay.Creatures
{
    public class EnemyWork : EnemyWorkBase, IWeaponAgent
    {
        [SerializeField] protected WorldObjectIdProvider _weaponId;
        [SerializeField] private RadialAreaView _searchAreaView;
        [SerializeField] private WeaponAttackTargetView _weaponAttackTargetView;
        [SerializeField] private Transform _attackAnchor;
        [SerializeField] private Transform _rangeAttackAnchor;
        [SerializeField] private bool _useSpawnPointRotation = true;
        public bool IsReady { get; private set; }

        private CompositeDisposable _disposable;
        private CancellationTokenSource _cts;

        private SphereCastProvider _search;
        private readonly ReactiveProperty<PlayerFlow> _player = new();

        private WorldObjectsService _world;
        private WeaponsFactory _weaponsFactory;
        private WeaponFlow _weapon;
        private WeaponAttackTarget _selfAttackTarget;
        private WeaponAttackTargetConfig _weaponAttackTargetConfig;
        private bool _lockedPlayerTarget;

        public Transform Transform => transform;
        public Transform AttackAnchor => _attackAnchor;
        public Transform RangeAttackAnchor => _rangeAttackAnchor;
        public override WeaponFlow Weapon => _weapon;


        protected override void OnInit()
        {
            base.OnInit();

            _world = Flow.Resolve<WorldObjectsService>();
            _weaponsFactory = Flow.Resolve<WeaponsFactory>();
            _selfAttackTarget = Flow.Get<WeaponAttackTarget>();
            IConfigsService configs = Flow.Resolve<IConfigsService>();
            _weaponAttackTargetConfig = configs.Get<WeaponAttackTargetConfig>(Flow.ConfigId);

            if (_searchAreaView != null)
            {
                _searchAreaView.Init(Flow);
            }
        }


        protected override void OnClear()
        {
            ClearWork();
        }


        public Transform GetSlot(int index) => View.SkinView.WeaponSlots[index];


        public virtual void PlayWeaponHit(IEnumerable<WeaponAttackTarget> targets, float rotationSpeed, bool rotateToTarget = false)
        {
            PlayerFlow player = _world.GetObjects<PlayerFlow>().First();
            WeaponAttackTarget target = targets.FirstOrDefault(it => it.GetFlow().Equals(player));
            target ??= targets.First();

            if (rotateToTarget)
            {
                Vector3 lookAtPosition = target.transform.position;
                View.LookAt(lookAtPosition.x, lookAtPosition.z);
            }

            View.Animations.PlayWeaponHit(_weaponId.Id);
        }


        public void ClearWeaponHit()
        {
            View.Animations.ClearWeaponHit(false);
        }


        public IObservable<Unit> OnWeaponHitAsObservable() => View.Animations.OnWeaponHitAsObservable();
        public IObservable<Unit> OnWeaponHitEndAsObservable() => View.Animations.OnWeaponHitEndAsObservable();
        public WeaponAttackTarget SelfAttackTarget => _selfAttackTarget;
        public bool IsMoving => Flow.Get<CreatureMotion>().IsMoving();

        public WorldObjectId GetId => Flow.GetSpawnerId;

        public bool TryAddWeapon(WorldObjectId weaponId) => false;
        public WeaponAttackTargetView WeaponAttackTargetView => _weaponAttackTargetView;
        public bool TryAddWeapon(string weaponName) => false;


        public override bool CanExecute() => !Die.IsDying;

        private const float ENEMY_AVOID_RANGE = 2f;
        private const float ENEMY_SAFE_DISTANCE = 4f;


        protected override async UniTask Execute(CancellationToken ct)
        {
            IsReady = true;
            _disposable = new();

            var enemyFlows = _world.GetObjects<EnemyFlow>().ToList();
            enemyFlows.Remove(Flow);

            IDisposable searchResult = _player.Subscribe(p =>
            {
                _cts?.CancelThenDispose();
                _cts = new();

                if (p != null)
                {
                    if (_weapon == null)
                    {
                        _weapon = _weaponsFactory.Create(_weaponId.Id, this, _weaponAttackTargetConfig.WeaponLevel);
                        FollowTargetAsync(_cts.Token, enemyFlows).Forget();
                    }
                }
            });
            _disposable.Add(searchResult);

            RunSearchAsync(ct).Forget();

            await UniTask.WaitUntil(() => !CanExecute(), cancellationToken: ct);

            StopMoveToTarget(false);
            ClearWork();
        }


        public void SetPlayerTarget(PlayerFlow player)
        {
            if (player == null)
            {
                _lockedPlayerTarget = false;
                return;
            }
            _player.Value = player;
            _lockedPlayerTarget = true;
        }


        private async UniTask RunSearchAsync(CancellationToken ct)
        {
            _search = new();

            while (true)
            {
                await UniTask.Yield(ct);

                if (_lockedPlayerTarget)
                {
                    return;
                }

                Vector3 position = Flow.transform.position;
                if (!Flow.Config.SearchRadiusMove)
                {
                    position.x = Flow.SpawnPoint.position.x;
                    position.z = Flow.SpawnPoint.position.z;
                }

                float radius = Flow.Config.SearchRadius;
                if (_searchAreaView != null)
                {
                    _searchAreaView.UpdateView(position, radius);
                }

                PlayerFlow target = _search.CalcHits<PlayerFlow>(position, radius).FirstOrDefault();
                if (_player.Value != target && target != null)
                {
                    _player.Value = target;
                }

                if (target == null && Motion.GetRemainDistance() > Flow.Config.FollowRadius && !_lockedPlayerTarget)
                {
                    _player.Value = null;
                    if (_weapon != null)
                    {
                        ClearWeaponHit();

                        _weaponsFactory.Destroy(_weapon);
                        _weapon = null;

                        MoveToSpawnPoint();
                    }
                }
            }
        }


        private async UniTask FollowTargetAsync(CancellationToken ct, List<EnemyFlow> enemies)
        {
            Vector3 destination = Vector3.negativeInfinity;
            bool isReached = false;
            while (_player.Value != null && CanExecute())
            {
                Knockback knockback = Flow.Get<Knockback>();
                bool isKnockback = knockback != null && knockback.IsKnockback;
                if (View.Animations.IsWeaponHit.Value || isKnockback)
                {
                    await UniTask.Yield(ct);
                    continue;
                }

                var nearEnemeis = enemies.Where(x => Vector3.Distance(x.transform.position, transform.position) < ENEMY_SAFE_DISTANCE).ToList();

                Vector3 position = _player.Value.transform.position;
                bool moveUnitlAvoid = false;

                foreach (var enemy in nearEnemeis)
                {
                    Vector3 dirrectionAway = transform.position - enemy.transform.position;
                    Vector3 randomOffset = Random.insideUnitSphere * ENEMY_AVOID_RANGE;
                    Vector3 newDistance = transform.position + dirrectionAway.normalized * ENEMY_SAFE_DISTANCE + randomOffset;

                    if (NavMesh.SamplePosition(newDistance, out NavMeshHit navMeshHit, ENEMY_SAFE_DISTANCE, NavMesh.AllAreas))
                    {
                        position = navMeshHit.position;
                        moveUnitlAvoid = true;
                        break;
                    }
                }

                Vector3 direction = position - transform.position;
                if (!destination.Equals(position))
                {
                    if (moveUnitlAvoid is false)
                    {
                        MoveToTargetAsync(position, direction, enemies, ENEMY_SAFE_DISTANCE).Forget();
                    }
                    else
                    {
                        await MoveToTargetAsync(position, direction, enemies, ENEMY_SAFE_DISTANCE);
                    }
                    destination = position;
                    isReached = false;
                }
                float remainDistance = Motion.GetRemainDistance();

                if (!isReached && remainDistance > 0.0f && remainDistance <= Flow.Config.FollowStoppageDistance)
                {
                    StopMoveToTarget(direction);
                    isReached = true;
                }

                await UniTask.Yield(ct);
            }
        }


        private void MoveToSpawnPoint()
        {
            Vector3 destination = Flow.SpawnPoint.position;
            if (_useSpawnPointRotation)
            {
                Vector3 endDirection = Flow.SpawnPoint.forward;
                MoveToTargetAsync(destination, endDirection, null).Forget();
            }
            else
            {
                MoveToTargetAsync(destination, null).Forget();
            }
        }


        private void ClearWork()
        {
            if (_weapon != null)
            {
                ClearWeaponHit();

                _weaponsFactory.Destroy(_weapon);
                _weapon = null;
            }

            _cts?.CancelThenDispose();
            _cts = null;

            _disposable?.Dispose();
            _disposable = null;

            _player.Value = null;
        }
    }
}
