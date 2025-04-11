using Cysharp.Threading.Tasks;
using DG.Tweening;
using Honeylab.Gameplay.Creatures;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Upgrades;
using Honeylab.Gameplay.Weapons.Upgrades;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.HitTesting;
using Honeylab.Utils.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Weapons
{
    public class WeaponAgent : WeaponAgentBase, IWeaponAgent
    {
        [SerializeField] private WorldObjectIdProvider _defaultWeaponId;
        [SerializeField] private ConfigIdProvider _searchConfigId;
        [SerializeField] private RadialAreaView _searchAreaView;
        [SerializeField] private WeaponAttackTargetView _weaponAttackTargetView;
        [SerializeField] private Transform _attackAnchor;
        [SerializeField] private Transform _rangeAttackAnchor;
        [SerializeField] private List<Transform> _weaponSlots;

        private SphereCastProvider _search;

        private WorldObjectsService _world;
        private IConfigsService _configs;
        private WeaponsFactory _weaponsFactory;
        private CreatureAnimations _animations;
        private PlayerMotion _playerMotion;

        private List<WorldObjectId> _allWeaponIds = new();

        private LevelPersistenceService _levelPersistenceService;
        private WeaponsPersistentComponent _allWeaponsPersistence;
        private WeaponsData _weaponsData;
        private WeaponFlow _weapon;
        private WeaponAttackTarget _selfAttackTarget;
        private CancellationTokenSource _run;
        private WorldObjectFlow _flow;

        public Transform Transform => transform;
        public Transform AttackAnchor => _attackAnchor;
        public Transform RangeAttackAnchor => _rangeAttackAnchor;
        public override WeaponFlow Weapon => _weapon;


        public WeaponAttackTarget SelfAttackTarget => _selfAttackTarget;
        public bool IsMoving => _flow.Get<PlayerMotion>().IsMoving.Value;

        public WorldObjectId GetId => _flow.Id;

        public Transform GetSlot(int index) => _weaponSlots[index];

        private Vector3 _lastRotation;

        private IDisposable _rotateDisposable;

        public void PlayWeaponHit(IEnumerable<WeaponAttackTarget> targets, float rotationSpeed, bool rotateToTarget = false)
        {
            if (rotateToTarget)
            {
                _rotateDisposable?.Dispose();
                _rotateDisposable = LookAt(targets.First().transform, rotationSpeed);
                _playerMotion.SetFight(true);
            }

            if (_weapon != null)
            {
                WeaponAttackAnimations attackAnimations = _weapon.Get<WeaponAttackAnimations>();
                _animations.PlayWeaponHit(_weapon.Id, attackAnimations.AttackAnimation);
            }
        }

        public void TargetsEmpty()
        {
            _playerMotion.SetFight(false);
        }


        public void ClearWeaponHit()
        {
            _animations.ClearWeaponHit();
            _playerMotion.SetFight(false);
            _rotateDisposable?.Dispose();
        }


        public IObservable<Unit> OnWeaponHitAsObservable() => _animations.OnWeaponHitAsObservable();
        public IObservable<Unit> OnWeaponHitEndAsObservable() => _animations.OnWeaponHitEndAsObservable();


        protected override void OnInit()
        {
            base.OnInit();

            _flow = GetFlow();
            _world = _flow.Resolve<WorldObjectsService>();
            _configs = _flow.Resolve<IConfigsService>();
            _weaponsFactory = _flow.Resolve<WeaponsFactory>();
            _animations = _flow.Get<CreatureAnimations>();
            _playerMotion = _flow.Get<PlayerMotion>();

            _levelPersistenceService = _flow.Resolve<LevelPersistenceService>();
            _allWeaponsPersistence = _levelPersistenceService.GetOrAddComponent<WeaponsPersistentComponent>(_flow.Id);

            _weaponsData = _flow.Resolve<WeaponsData>();
            if (_allWeaponsPersistence.Weapons.Count != 0)
            {
                _allWeaponIds = _allWeaponsPersistence.Weapons.Select(it => _weaponsData.GetData(it).Id).ToList();
            }
            else if (_defaultWeaponId != null && _defaultWeaponId.Id != null)
            {
                _allWeaponIds.Add(_defaultWeaponId.Id);

                string weaponName = _weaponsData.GetData(_defaultWeaponId.Id).Name;
                _allWeaponsPersistence.Weapons.Add(weaponName);
            }

            _selfAttackTarget = _flow.Get<WeaponAttackTarget>();

            if (_searchAreaView != null)
            {
                _searchAreaView.Init(_flow);
            }
        }


        protected override void OnRun()
        {
            _run = new CancellationTokenSource();
            RunSearchAsync(_run.Token).Forget();
        }


        protected override void OnClear()
        {
            if (_weapon != null)
            {
                _animations.ClearWeaponHit(false);

                _weaponsFactory.Destroy(_weapon);
                _weapon = null;
            }
        }


        protected override void OnStop()
        {
            _run?.CancelThenDispose();
            _run = null;
            _rotateDisposable?.Dispose();
            _playerMotion.SetFight(false);
        }


        private async UniTask RunSearchAsync(CancellationToken ct)
        {
            _search = new();

            while (true)
            {
                await UniTask.Yield(ct);

                WeaponTargetsSearchConfig searchConfig = _configs.Get<WeaponTargetsSearchConfig>(_searchConfigId.Id);
                Vector3 position = transform.position;
                float radius = searchConfig.SearchRadius;
                if (_searchAreaView != null)
                {
                    _searchAreaView.UpdateView(position, radius);
                }

                var targetsFromClosest = _search.CalcHits<WeaponAttackTarget>(position, radius, true)
                    .Where(t => t != _selfAttackTarget && t.CanHit())
                    .ToList();

                WorldObjectId weaponIdToEquip = null;
                var weaponIdsByPriority =
                    _allWeaponIds.OrderByDescending(it => _weaponsData.GetData(it).Priority).ToList();
                var weaponsDataByPriority = weaponIdsByPriority.Select(it => _weaponsData.GetData(it)).ToList();
                int n = weaponIdsByPriority.Count;
                for (int i = 0; i < n; i++)
                {
                    WorldObjectId weaponId = weaponIdsByPriority[i];
                    WeaponData weaponData = weaponsDataByPriority[i];
                    WeaponAttackTarget weaponTarget =
                        targetsFromClosest.FirstOrDefault(t => t.WeaponTypeIds.Contains(weaponData.TypeId));
                    if (CanEquipWeapon(weaponTarget, weaponId))
                    {
                        weaponIdToEquip = weaponId;
                        break;
                    }
                }

                if (weaponIdToEquip != null)
                {
                    if (_weapon == null || !_weapon.Id.Equals(weaponIdToEquip))
                    {
                        TryUnequipWeapon();
                        EquipWeapon(weaponIdToEquip);
                    }
                }
                else
                {
                    TryUnequipWeapon();
                }
            }
        }


        private IDisposable LookAt(Transform target, float rotationSpeed)
        {
            return Observable.EveryUpdate()
                .TakeWhile(_ =>
                {
                    Vector3 direction = target.transform.position - _flow.transform.position;
                    direction.y = 0;

                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    //_flow.transform.rotation = targetRotation;
                     _flow.transform.rotation =
                         Quaternion.Lerp(_flow.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                    _playerMotion.SetTargetDirrection(direction.normalized);

                    return true;
                })
                .Subscribe();
        }


        public WorldObjectId GetWeaponByTypeFirstOrDefault(WorldObjectId weaponType)
        {
            return _allWeaponIds.FirstOrDefault(id => _weaponsData.GetData(id).TypeId.Equals(weaponType));
        }


        public bool TryAddWeapon(string weaponName)
        {
            WeaponData data = _weaponsData.GetData(weaponName);
            if (data == null)
            {
                return false;
            }

            AddWeapon(data);
            return true;
        }


        public bool TryAddWeapon(WorldObjectId weaponId)
        {
            WeaponData data = _weaponsData.GetData(weaponId);
            if (data == null)
            {
                return false;
            }

            AddWeapon(data);
            return true;
        }


        public void RemoveWeapon(WorldObjectId weaponId)
        {
            WeaponData data = _weaponsData.GetData(weaponId);
            if (data == null)
            {
                return;
            }

            if (_allWeaponIds.Contains(weaponId))
            {
                _allWeaponIds.Remove(weaponId);
            }

            string weaponName = data.Name;
            if (_allWeaponsPersistence.Weapons.Contains(weaponName))
            {
                _allWeaponsPersistence.Weapons.Remove(weaponName);
                _levelPersistenceService.Save();
            }

            if (_weapon != null)
            {
                _weapon.Stop();
                _weaponsFactory.Destroy(_weapon);
                _weapon = null;
                ClearWeaponHit();
            }
        }


        public WeaponAttackTargetView WeaponAttackTargetView => _weaponAttackTargetView;


        private void AddWeapon(WeaponData data)
        {
            WorldObjectId weaponId = data.Id;
            if (!_allWeaponIds.Contains(weaponId))
            {
                _allWeaponIds.Add(weaponId);
            }

            string weaponName = data.Name;
            if (!_allWeaponsPersistence.Weapons.Contains(weaponName))
            {
                _allWeaponsPersistence.Weapons.Add(weaponName);
            }
        }

        private bool CanEquipWeapon(WeaponAttackTarget target, WorldObjectId weaponId)
        {
            if (target == null || weaponId == null)
            {
                return false;
            }

            UpgradeFlow weaponUpgrade = _world.GetObject<UpgradeFlow>(weaponId);
            WeaponUpgradeConfig weaponUpgradeConfig =
                _configs.Get<WeaponUpgradeConfig>(weaponUpgrade.ConfigId);

            WeaponUpgradeLevelConfig weaponUpgradeLevelConfig =
                weaponUpgrade.GetLevelUpgradeConfig<WeaponUpgradeLevelConfig>(weaponUpgradeConfig.Upgrade,
                    weaponUpgrade.GetLevel());

            Vector3 attackAnchor = transform.position; // TODO: use different anchors
            BoxCollider targetCollider = target.GetComponent<BoxCollider>();
            Vector3 closestPointOnTargetCollider = targetCollider.ClosestPointOnBounds(attackAnchor);
            float distance = Vector3.Distance(attackAnchor, closestPointOnTargetCollider);
            if (distance > weaponUpgradeLevelConfig.SearchRadius)
            {
                return false;
            }

            return true;
        }


        private void EquipWeapon(WorldObjectId weaponId)
        {
            if (_weapon != null && _weapon.Id.Equals(weaponId))
            {
                return;
            }

            if (_animations.IsWeaponHit.Value)
            {
                return;
            }

            _weapon = _weaponsFactory.Create(weaponId, this);
        }


        private void TryUnequipWeapon()
        {
            if (_weapon == null)
            {
                return;
            }

            if (!_animations.IsWeaponHit.Value)
            {
                _weaponsFactory.Destroy(_weapon);
                _weapon = null;
            }
        }
    }
}
