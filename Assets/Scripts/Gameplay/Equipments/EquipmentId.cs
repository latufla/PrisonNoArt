using Honeylab.Gameplay.World;
using Honeylab.Utils.Persistence;
using UnityEngine;


namespace Honeylab.Gameplay.Equipments
{
    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(EquipmentId),
        menuName = DataUtil.MenuNamePrefix + "Equipment ID")]
    public class EquipmentId : WorldObjectId { }
}
