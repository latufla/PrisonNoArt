using Honeylab.Gameplay.World;
using Honeylab.Utils.Data;
using UnityEngine;


namespace Honeylab.Gameplay.Creatures
{
    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(CreatureId),
        menuName = DataUtil.MenuNamePrefix + "Creature ID")]
    public class CreatureId : WorldObjectId { }
}
