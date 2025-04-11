using Honeylab.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Honeylab.Gameplay.Creatures
{
    [Serializable]
    public class CreatureData
    {
        [SerializeField] private CreatureId _id;
        [SerializeField] private string _configId;

        public CreatureId Id => _id;
        public string ConfigId => _configId;
    }

    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(CreaturesData),
        menuName = DataUtil.MenuNamePrefix + "Creatures Data")]
    public class CreaturesData : ScriptableObject
    {
        [SerializeField] private List<CreatureData> _creatures;

        public IReadOnlyList<CreatureData> GetData() => _creatures.AsReadOnly();
        public CreatureData GetData(CreatureId id) => _creatures.First(it => id.Equals(it.Id));

    }
}
