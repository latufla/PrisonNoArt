using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;


namespace Honeylab.Utils
{
    public class PointingArrowView : MonoBehaviour
    {
        public Transform Target { get; private set; }
        [SerializeField] private float _distanceToArrowDissapear;
        [SerializeField] private Transform _body;
        private Transform _parentTransform;


        public bool IsActive
        {
            get => _body.gameObject.activeInHierarchy;
        }


        public void Init(Transform playerTransform)
        {
            _parentTransform = playerTransform;
        }


        internal async UniTask RunAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (Target != null)
                {
                    transform.position = _parentTransform.position;
                    transform.LookAt(new Vector3(Target.transform.position.x,
                        transform.position.y,
                        Target.transform.position.z));

                    _body.gameObject.SetActive((Vector3.Distance(transform.position, Target.transform.position) >
                        _distanceToArrowDissapear));
                }

                await UniTask.Yield(ct);
            }
        }


        public void Show(Transform target)
        {
            Target = target;
            _body.gameObject.SetActive(true);
        }


        public void Hide()
        {
            Target = null;
            _body.gameObject.SetActive(false);
        }
    }
}
