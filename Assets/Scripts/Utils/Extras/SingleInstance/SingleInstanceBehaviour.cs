using UniRx;
using UnityEngine;
using Zenject;


namespace Honeylab.Utils.SingleInstance
{
    public class SingleInstanceBehaviour<T> : MonoBehaviour where T : Behaviour
    {
        [SerializeField] private T _component;

        private SingleInstanceService<T> _singleInstanceService;


        [Inject]
        public void Construct(SingleInstanceService<T> singleInstanceService)
        {
            _singleInstanceService = singleInstanceService;
        }


        private void Start()
        {
            _singleInstanceService.AddInstance(_component);
            Disposable.CreateWithState(_singleInstanceService, s => s.RemoveInstance(_component))
                .AddTo(this);

            _singleInstanceService.ActiveInstanceAsObservable()
                .Select(l => _component == l)
                .Subscribe(isLightActive => _component.enabled = isLightActive)
                .AddTo(this);
        }
    }
}
