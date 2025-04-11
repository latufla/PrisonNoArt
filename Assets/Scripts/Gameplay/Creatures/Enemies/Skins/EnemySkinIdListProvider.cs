using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Gameplay.Creatures
{
    public class EnemySkinIdListProvider : MonoBehaviour
    {
        [SerializeField] private List<EnemySkinId> _objects;


        public List<EnemySkinId> Objects => _objects;
    }
}
