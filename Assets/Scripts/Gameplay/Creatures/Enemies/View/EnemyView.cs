using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Data;
using Honeylab.Utils.OffscreenTargetIndicators;
using Honeylab.Utils.Pool;
using UnityEngine;
using UnityEngine.AI;


namespace Honeylab.Gameplay.Creatures
{
    public class EnemyView : WorldObjectComponentBase
    {
        [SerializeField] private CreatureAnimations _animations;
        [SerializeField] private Transform _skinRoot;
        [SerializeField] private ScriptableId _offscreenIndicatorId;
        [SerializeField] private Sprite _offscreenIndicatorIcon;

        private EnemyFlow _flow;
        private EnemySkinsPool _skinsPool;
        private GameObject _skin;
        private EnemySkinView _skinView;

        private OffscreenIndicatorsService _offscreenIndicatorsService;

        private OffscreenIndicator _offscreenIndicator;
        private NavMeshAgent _navMeshAgent;


        public virtual CreatureAnimations Animations => _animations;
        public EnemySkinView SkinView => _skinView;


        protected override void OnInit()
        {
            _flow = GetFlow<EnemyFlow>();
            _navMeshAgent = _flow.GetComponent<NavMeshAgent>();
            GameplayPoolsService pools = _flow.Resolve<GameplayPoolsService>();
            _skinsPool = pools.Get<EnemySkinsPool>();
            _offscreenIndicatorsService = _flow.Resolve<OffscreenIndicatorsService>();

            _skin = _skinsPool.Pop(_flow.SkinId);
            _skin.transform.parent = _skinRoot;
            _skin.transform.localPosition = Vector3.zero;
            _skin.transform.localRotation = Quaternion.identity;

            _skinView = _skin.GetComponentInChildren<EnemySkinView>();
            _animations.SetAnimatorProvider(_skinView.AnimatorProvider);

            UpdateOffscreenIndicator(true);
        }


        protected override void OnClear()
        {
            UpdateOffscreenIndicator(false);

            if (_skin != null)
            {
                _skinsPool.Push(_flow.SkinId, _skin);
                _skin.transform.localScale = Vector3.one;
                _skin = null;
                _skinView = null;
            }

            _animations.SetAnimatorProvider(null);
        }


        public void LookAt(float x, float z)
        {
            _navMeshAgent.updateRotation = false;
            _flow.transform.LookAt(new Vector3(x, _flow.transform.position.y, z));
            _navMeshAgent.updateRotation = true;
        }


        private void UpdateOffscreenIndicator(bool isEnabled)
        {
            if (_offscreenIndicatorId == null)
            {
                return;
            }

            if (isEnabled)
            {
                _offscreenIndicator ??= _offscreenIndicatorsService.AddEnemy(_offscreenIndicatorId, transform);
                _offscreenIndicator.SetIcon(_offscreenIndicatorIcon);
            }
            else
            {
                if (_offscreenIndicator != null)
                {
                    _offscreenIndicatorsService.Remove(_offscreenIndicator);
                    _offscreenIndicator = null;
                }
            }
        }
    }
}
