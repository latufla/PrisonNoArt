using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Honeylab.Utils.HitTesting
{
    public class LineCastProvider
    {
        private readonly Collider[] _colliders = new Collider[64];
        private readonly List<MonoBehaviour> _results = new(64);

        private Vector3 _origin;


        public IEnumerable<T> CalcHits<T>(Transform origin,
            float radius,
            float width,
            bool sortByDistance = false)
            where T : MonoBehaviour
        {
            //_origin = origin.position;
            _results.Clear();

            Quaternion rotation = Quaternion.LookRotation(origin.forward);
            Vector3 rotationEuler = new(0.0f, rotation.eulerAngles.y, 0.0f);
            rotation = Quaternion.Euler(rotationEuler);
            _origin = origin.transform.TransformPoint(Vector3.forward * radius);

            int n = Physics.OverlapBoxNonAlloc(_origin, new Vector3(width, 5, radius), _colliders, rotation);

            for (int i = 0; i < n; i++)
            {
                if (!_colliders[i].TryGetComponent(out T result))
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
