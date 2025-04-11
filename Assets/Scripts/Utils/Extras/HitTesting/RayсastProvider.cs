using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Utils.HitTesting
{
    public class RayсastProvider
    {
        private readonly float _maxDistance;
        private readonly int _layerMask;

        private Vector3 _position;
        private Vector3 _direction;

        private readonly RaycastHit[] _hits = new RaycastHit[64];
        private readonly List<RaycastHit> _results = new(64);


        public RayсastProvider(float maxDistance, int layerMask)
        {
            _maxDistance = maxDistance;
            _layerMask = layerMask;
        }


        public List<RaycastHit> CalcHits(Ray ray) => CalcHits(ray.origin, ray.direction);


        public List<RaycastHit> CalcHits(Vector3 position, Vector3 direction)
        {
            _position = position;
            _direction = direction;

            _results.Clear();

            int size = Physics.RaycastNonAlloc(position, direction, _hits, _maxDistance, _layerMask);
            for (int i = 0; i < size; ++i)
            {
                _results.Add(_hits[i]);
            }

            _results.Sort((a1, a2) =>
                Vector3.Distance(position, a1.point).CompareTo(Vector3.Distance(position, a2.point)));

            return _results;
        }


        public void DrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_position, _direction * _maxDistance);
        }
    }
}
