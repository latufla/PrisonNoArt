using UnityEngine;


namespace Honeylab.Gameplay.World
{
    public class WorldObjectsArgs : MonoBehaviour
    {
        [SerializeField] private Transform _root;


        public Transform Root => _root;
    }
}
