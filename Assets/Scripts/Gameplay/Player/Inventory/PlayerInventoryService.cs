using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Upgrades;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Honeylab.Gameplay.Player
{
    public class InventoryItem
    {
        public WorldObjectData Data { get; set; }
        public WeaponAttackTarget Target { get; set; }
    }


    public class PlayerInventoryService : IDisposable
    {
        private readonly WorldObjectsService _world;
        private readonly IConfigsService _configs;
        private readonly WorldObjectsData _data;

        private List<WorldObjectFlow> _flows;
        private List<WeaponAttackTarget> _targets;

        private int _weaponLevel = -1;
        private readonly List<InventoryItem> _items = new();


        public PlayerInventoryService(WorldObjectsService world, IConfigsService configs, WorldObjectsData data)
        {
            _world = world;
            _configs = configs;
            _data = data;
        }


        public void Init()
        {
            _flows = _world.GetRegisteredObjects()
                .Where(it => it.GetComponentInChildren<WeaponAttackTarget>(true) != null)
                .ToList();

            _targets = _flows.Select(it => it.GetComponentInChildren<WeaponAttackTarget>(true))
                .ToList();
        }


        public void Run() { }


        public IReadOnlyList<InventoryItem> GetItemsByWeaponLevel(int weaponLevel, bool includePreviousLevels = false)
        {
            if (weaponLevel == _weaponLevel)
            {
                return _items.AsReadOnly();
            }

            _items.Clear();

            int n = _flows.Count;
            for (int i = 0; i < n; i++)
            {
                WorldObjectFlow flow = _flows[i];
                WeaponAttackTarget target = _targets[i];
                bool hasTarget = _items.FirstOrDefault(it => it.Target.ConfigId.Equals(target.ConfigId)) != null;
                WorldObjectData data = _data.GetData(flow.Id);
                if (hasTarget || data == null)
                {
                    continue;
                }

                WeaponAttackTargetConfig config = _configs.Get<WeaponAttackTargetConfig>(target.ConfigId);
                int requiredWeaponLevel = config.WeaponLevel;
                if (includePreviousLevels && weaponLevel < requiredWeaponLevel)
                {
                    continue;
                }

                if (!includePreviousLevels && weaponLevel != requiredWeaponLevel)
                {
                    continue;
                }

                _items.Add(new InventoryItem
                {
                    Data = data,
                    Target = target
                });
            }

            return _items.AsReadOnly();
        }


        public WeaponInfo GetWeaponUpgradeInfo(WorldObjectId id)
        {
            var player = _world.GetObjects<PlayerFlow>().FirstOrDefault();
            if (player == null)
            {
                return null;
            }
            var weaponAgent = player.Get<WeaponAgent>();
            if (weaponAgent == null)
            {
                return null;
            }
            var weaponId = weaponAgent.GetWeaponByTypeFirstOrDefault(id);
            if (weaponId == null)
            {
                return null;
            }
            var upgrade = _world.GetObject<UpgradeFlow>(weaponId);

            WeaponInfo weaponInfo = new WeaponInfo()
            {
                Upgrade = upgrade,
                WeaponAgent = weaponAgent,
                WeaponId = weaponId
            };

            return weaponInfo;
        }

        public void Dispose() { }
    }
}
