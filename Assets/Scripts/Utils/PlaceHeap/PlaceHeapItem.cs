using System;
using UnityEngine;

namespace Honeylab.Utils
{
    public class PlaceHeapItem<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private Transform _anchor;
        public Transform Place => transform;
        public Transform Anchor => _anchor;
        public T Occupier { get; set; }
        public float OccupationTime { get; set; }

        private void OnDrawGizmos()
        {
            if (Place != null)
            {
                Gizmos.color = Occupier == null ? Color.blue : Color.red;
                Gizmos.DrawSphere(Place.position, 1.0f);
            }

            if (Anchor != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(Anchor.position, 1.0f);
            }
        }
    }
}
