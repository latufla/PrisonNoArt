using Honeylab.Gameplay.World;
using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Gameplay.Creatures
{
    public class EnemySkinView : MonoBehaviour
    {
        [SerializeField] private AnimatorProvider _animatorProvider;
        [SerializeField] private List<Transform> _weaponSlots;
        [SerializeField] private Collider _collider;

        public AnimatorProvider AnimatorProvider => _animatorProvider;
        public List<Transform> WeaponSlots => _weaponSlots;
        public Collider Collider => _collider;
    }
}
