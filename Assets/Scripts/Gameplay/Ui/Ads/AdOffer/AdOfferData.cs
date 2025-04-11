using Honeylab.Consumables;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Honeylab.Gameplay
{
    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(AdOfferData),
        menuName = DataUtil.MenuNamePrefix + "Ad Offer Data")]
    public class AdOfferData : ScriptableObject
    {
        [SerializeField] private WorldObjectId _id;
        [SerializeField] private string _configId;
        [SerializeField] private WorldObjectId _weaponTypeId;
        [SerializeField] private List<AdOfferIconData> _iconInfos;

        public WorldObjectId AdOfferId => _id;
        public WorldObjectId WeaponTypeId => _weaponTypeId;
        public string ConfigId => _configId;
        public Sprite GetIcon(ConsumablePersistenceId id) => _iconInfos.FirstOrDefault(it => it.Id.Equals(id))?.Sprite;
        public Sprite GetIconFirst() => _iconInfos.FirstOrDefault()?.Sprite;

    }


    [Serializable]
    public class AdOfferIconData
    {
        [SerializeField] private ConsumablePersistenceId _id;
        [SerializeField] private Sprite _sprite;

        public ConsumablePersistenceId Id => _id;
        public Sprite Sprite => _sprite;
    }
}
