using Honeylab.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Honeylab.Consumables
{
    public enum ConsumableType
    {
        Regular,
        Hard,
        Soft,
        Hidden,
        Card,
        Finds
    }


    [Serializable]
    public class ConsumableData
    {
        [SerializeField] private string _name;
        [SerializeField] private ConsumablePersistenceId _id;
        [SerializeField] [Min(0)] private int _initialAmount;
        [SerializeField] private string _title;

        [SerializeField] [TextArea] private string _description;
        [SerializeField] private Sprite _sprite;
        [SerializeField] private ConsumableType _consumableType = ConsumableType.Regular;


        public string Name => _name;
        public string Title => _title;
        public string Description => _description;
        public ConsumablePersistenceId Id => _id;
        public int InitialAmount => _initialAmount;
        public Sprite Sprite => _sprite;
        public ConsumableType ConsumableType => _consumableType;
    }


    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(ConsumablesData),
        menuName = DataUtil.MenuNamePrefix + "Consumables Data")]
    public class ConsumablesData : ScriptableObject
    {
        [SerializeField] private int _numberVisibleConsumables;
        [SerializeField] private List<ConsumableData> _data;


        public IReadOnlyList<ConsumableData> GetData() => _data.AsReadOnly();
        public ConsumableData GetData(ConsumablePersistenceId id) => _data.First(e => e.Id.Equals(id));
        public ConsumableData GetData(string consumableName) => _data.First(it => it.Name.Equals(consumableName));


        public IEnumerable<ConsumableData> GetData(ConsumableType consumableType) =>
            _data.Where(it => it.ConsumableType.Equals(consumableType));


        public bool Contains(string consumableName) =>
            _data.FirstOrDefault(it => it.Name.Equals(consumableName)) != null;

        public bool Contains(ConsumablePersistenceId consumableId) =>
            _data.FirstOrDefault(it => it.Id.Equals(consumableId)) != null;


        public int NumberVisibleConsumables => _numberVisibleConsumables;
    }
}
