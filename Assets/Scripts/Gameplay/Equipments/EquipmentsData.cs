using Honeylab.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Honeylab.Gameplay.Equipments
{
    [Serializable]
    public class EquipmentLevelData
    {
        [SerializeField] private Sprite _sprite;

        public Sprite Sprite => _sprite;
    }


    [Serializable]
    public class EquipmentData
    {
        [SerializeField] private string _name;
        [SerializeField] private EquipmentId _id;
        [SerializeField] private string _configId;
        [SerializeField] private string _title;
        [SerializeField] private string _description;
        [SerializeField] private List<EquipmentLevelData> _levels;


        public string Name => _name;
        public EquipmentId Id => _id;
        public string ConfigId => _configId;
        public string Title => _title;
        public string Description => _description;
        public List<EquipmentLevelData> Levels => _levels;
    }


    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(EquipmentsData),
        menuName = DataUtil.MenuNamePrefix + "Equipments Data")]
    public class EquipmentsData : ScriptableObject
    {
        [SerializeField] private List<EquipmentData> _equipments;
        [SerializeField] private Sprite _combatPowerSprite;
        [SerializeField] private Sprite _attackPowerSprite;
        [SerializeField] private Sprite _healthPointSprite;
        [SerializeField] private EquipmentId _inventoryEquipmentId;

        public IReadOnlyList<EquipmentData> GetData() => _equipments.AsReadOnly();
        public EquipmentData GetData(string equipmentName) => _equipments.FirstOrDefault(it => equipmentName.Equals(it.Name));
        public EquipmentData GetData(EquipmentId id) => _equipments.FirstOrDefault(it => id.Equals(it.Id));

        public Sprite CombatPowerSprite => _combatPowerSprite;
        public Sprite AttackPowerSprite => _attackPowerSprite;
        public Sprite HealthPointSprite => _healthPointSprite;
        public EquipmentId GetInventoryEquipmentId() => _inventoryEquipmentId;
    }
}
