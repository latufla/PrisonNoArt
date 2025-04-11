using Honeylab.Gameplay.World;
using Honeylab.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Honeylab.Gameplay.Weapons
{
    [Serializable]
    public class WeaponLevelData
    {
        [SerializeField] private Sprite _sprite;


        public Sprite Sprite => _sprite;
    }


    [Serializable]
    public class WeaponData
    {
        [SerializeField] private string _name;
        [SerializeField] private WorldObjectId _id;
        [SerializeField] private WorldObjectId _typeId;
        [SerializeField] private List<WeaponLevelData> _levels;
        [SerializeField] private int _priority;


        public string Name => _name;
        public WorldObjectId Id => _id;
        public WorldObjectId TypeId => _typeId;
        public List<WeaponLevelData> Levels => _levels;
        public int Priority => _priority;
    }


    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(WeaponsData),
        menuName = DataUtil.MenuNamePrefix + "Weapons Data")]
    public class WeaponsData : ScriptableObject
    {
        [SerializeField] private List<WeaponData> _weapons;
        [SerializeField] private WorldObjectId _inventoryWeaponTypeId;
        public WeaponData GetData(string weaponName) => _weapons.FirstOrDefault(it => weaponName.Equals(it.Name));
        public WeaponData GetData(WorldObjectId id) => _weapons.FirstOrDefault(it => id.Equals(it.Id));
        public WorldObjectId GetInventoryWeaponTypeId() => _inventoryWeaponTypeId;
    }
}
