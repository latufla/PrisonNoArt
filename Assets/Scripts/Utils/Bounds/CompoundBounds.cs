using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Honeylab.Utils.Bounds
{
    public class CompoundBounds : MonoBehaviour
    {
        private List<Collider> _bounds;
        private Vector3 _point;


        public bool CheckBounds(float x, float z)
        {
            _bounds ??= GetComponentsInChildren<Collider>().ToList();

            _point.x = x;
            _point.z = z;

            int n = _bounds.Count;
            for (int i = 0; i < n; i++)
            {
                Collider b = _bounds[i];
                _point.y = b.transform.position.y;

                if (b.bounds.Contains(_point))
                {
                    return true;
                }
            }

            return false;
        }


        public Vector2 ClosestPointOnBounds(float x, float z)
        {
            _bounds ??= GetComponentsInChildren<Collider>().ToList();

            _point.x = x;
            _point.z = z;

            Vector3 closestPoint = new(float.MaxValue, float.MaxValue, float.MaxValue);

            int n = _bounds.Count;
            for (int i = 0; i < n; i++)
            {
                Collider b = _bounds[i];
                _point.y = b.transform.position.y;

                Vector3 p = b.bounds.ClosestPoint(_point);
                if (Vector3.Distance(p, _point) < Vector3.Distance(closestPoint, _point))
                {
                    closestPoint = p;
                }
            }

            return new Vector2(closestPoint.x, closestPoint.z);
        }
    }
}
