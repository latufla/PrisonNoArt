using Honeylab.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Honeylab.Gameplay.World
{
    [Serializable]
    public class WorldObjectData
    {
        [Tooltip("Editor only")]
        [SerializeField] private string _name;

        [SerializeField] private WorldObjectId _id;
        [SerializeField] private Sprite _sprite;

        public WorldObjectId Id => _id;
        public Sprite Sprite => _sprite;


        public string Name
        {
            get => _name;
            set => _name = value;
        }
    }


    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(WorldObjectsData),
        menuName = DataUtil.MenuNamePrefix + "World Objects Data")]
    public class WorldObjectsData : ScriptableObject
    {
        [SerializeField] private List<WorldObjectData> _data;


        private void OnValidate()
        {
            _data.ForEach(it =>
            {
                if (it.Id != null)
                {
                    it.Name = it.Id.name.Replace("Data_WorldObjects_Id_", string.Empty);
                }
            });
        }


        public WorldObjectData GetData(WorldObjectId id) => _data.FirstOrDefault(e => e.Id.Equals(id));
    }
}
