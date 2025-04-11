using Honeylab.Gameplay.Weapons;
using UnityEngine;


namespace Honeylab.Gameplay.Interactables
{
    public interface IInteractAgent
    {
        bool IsMoving { get; }

        Transform ConsumablesOutAnchor { get; }
        IWeaponAgent WeaponAgent { get;}
    }
}
