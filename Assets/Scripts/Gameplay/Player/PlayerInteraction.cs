using Honeylab.Gameplay.Interactables;
using Honeylab.Gameplay.Interactables.World;
using Honeylab.Gameplay.Weapons;
using UnityEngine;


namespace Honeylab.Gameplay.Player
{
    public class PlayerInteraction : InteractionBase
    {
        [SerializeField] private PlayerMotion _motion;

        public override bool IsMoving => _motion.IsMoving.Value;
        public override Transform ConsumablesOutAnchor => transform;
        public override IWeaponAgent WeaponAgent => GetFlow<PlayerFlow>().Get<WeaponAgent>();
    }
}
