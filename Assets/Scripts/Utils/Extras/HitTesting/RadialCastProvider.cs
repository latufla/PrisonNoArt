using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Honeylab.Utils.HitTesting
{
    public class RadialCastProvider
    {
        private readonly Collider[] _colliders = new Collider[64];
        private readonly List<MonoBehaviour> _results = new(64);

        private Vector3 _origin;


        public IEnumerable<T> CalcHits<T>(Vector3 origin,
            Vector3 direction,
            float radius,
            float angle,
            bool sortByDistance = false)
            where T : MonoBehaviour
        {
            _origin = origin;

            _results.Clear();

            int n = Physics.OverlapSphereNonAlloc(origin, radius, _colliders);
            for (int i = 0; i < n; i++)
            {
                if (!_colliders[i].TryGetComponent(out T result))
                {
                    continue;
                }

                Vector3 dir = result.transform.position - origin;
                float yAngle = Vector3.Angle(direction, dir);
                if (angle / 2.0f < yAngle)
                {
                    continue;
                }

                _results.Add(result);
            }

            if (sortByDistance)
            {
                _results.Sort(SortByDistance);
            }

            return _results.Cast<T>();
        }


        private int SortByDistance(MonoBehaviour it1, MonoBehaviour it2) => Vector3
            .Distance(it1.transform.position, _origin)
            .CompareTo(Vector3.Distance(it2.transform.position, _origin));
    }
}
