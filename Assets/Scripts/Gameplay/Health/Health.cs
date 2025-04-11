using Honeylab.Gameplay.Config;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Persistence;
using System;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay
{
    public class Health : WorldObjectComponentBase
    {
        [SerializeField] private ConfigIdProvider _configId;
        [SerializeField] private bool _withPersistence;

        private HealthPersistentComponent _healthPersistence;
        private CompositeDisposable _disposable;

        public HealthConfig Config { get; private set; }
        public readonly IReactiveProperty<float> HealthProp = new ReactiveProperty<float>();

        private Subject<WorldObjectId> _onChangedHealth = new Subject<WorldObjectId>();
        public IObservable<WorldObjectId> OnChangedHealthAsObservable() => _onChangedHealth.AsObservable();

        public float MaxHealth { get; private set; }
        
        public bool WithPersistence
        {
            set => _withPersistence = value;
        }


        protected override void OnInit()
        {
            WorldObjectFlow flow = GetFlow();

            IConfigsService configs = flow.Resolve<IConfigsService>();
            Config = configs.Get<HealthConfig>(_configId.Id);

            MaxHealth = Config.Health;

            if (!_withPersistence)
            {
                HealthProp.Value = MaxHealth;
            }
            else
            {
                LevelPersistenceService persistence = flow.Resolve<LevelPersistenceService>();
                Guid guid = flow.CalcGuidFromName();
                PersistenceId id = PersistenceId.CreateWithGuid(guid);
                if (!persistence.Has(id))
                {
                    PersistentObject po = persistence.Create(id);
                    _healthPersistence = po.Add<HealthPersistentComponent>();
                    _healthPersistence.Value = MaxHealth;
                }
                else
                {
                    PersistentObject po = persistence.Get(id);
                    _healthPersistence = po.GetOrAdd<HealthPersistentComponent>();
                }

                HealthProp.Value = _healthPersistence.Value;

                _disposable = new CompositeDisposable();
                IDisposable healthChange = HealthProp.Subscribe(health => _healthPersistence.Value = health);
                _disposable.Add(healthChange);
            }
        }


        public void SetMaxHealth(float health) => MaxHealth = health;

        public void ChangeHealth(WorldObjectId id, float newHealth)
        {
            HealthProp.Value = newHealth;
            _onChangedHealth.OnNext(id);
        }

        protected override void OnClear()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}
