using UnityEngine;


namespace Honeylab.Utils
{
    public class AlignByTransform : MonoBehaviour
    {
        [SerializeField] private Transform _anchor;
        [SerializeField] private bool _x;
        [SerializeField] private bool _y;
        [SerializeField] private bool _z;
        [SerializeField] private bool _rotation;

        private Vector3 _position = Vector3.zero;

        private Vector3 _relativePosition;
        private Quaternion _relativeRotation;

        public void SetAnchor(Transform anchor)
        {
            _anchor = anchor;

            if (_anchor == null)
            {
                return;
            }

            if (_rotation)
            {
                _relativeRotation = Quaternion.Inverse(_anchor.rotation) * transform.rotation;
            }
            
        }


        public void Update()
        {
            if (_anchor == null)
            {
                return;
            }

            _position = transform.position;

            if (_x)
            {
                _position.x = _anchor.position.x;
            }
            if (_y)
            {
                _position.y = _anchor.position.y;
            }
            if (_z)
            {
                _position.y = _anchor.position.z;
            }

            transform.position = _position;


            if (_rotation)
            {
                transform.rotation = _anchor.rotation * _relativeRotation;
            }
            
        }
    }
}
