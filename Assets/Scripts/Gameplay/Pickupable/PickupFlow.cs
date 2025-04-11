using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.World;
using Honeylab.Persistence;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Persistence;
using System;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Pickup
{
    public class PickupFlow : WorldObjectFlow
    {
        [SerializeField] private ConfigIdProvider _configId;

        public IConfigsService Configs { get; private set; }
        public PickupConfig PickupConfig { get; private set; }
        public GameplayPoolsService Pools { get; private set; }
        public string ConfigId => _configId.Id;

        private CompositeDisposable _disposable;


        public PickupPersistentComponent IsDeactivatePersistence { get; private set; }


        protected override void OnInit()
        {
            Pools = Resolve<GameplayPoolsService>();
            Configs = Resolve<IConfigsService>();
            PickupConfig = Configs.Get<PickupConfig>(ConfigId);

            PersistentObject persistence = Resolve<LevelPersistenceService>().GetOrCreate(Id);
            IsDeactivatePersistence = persistence.GetOrAdd<PickupPersistentComponent>();
        }


        protected override UniTask OnRunAsync(CancellationToken ct)
        {
            _disposable = new CompositeDisposable();

            IDisposable onDeactivate = RunOnDeactivate();
            _disposable?.Add(onDeactivate);
            return UniTask.CompletedTask;
        }


        private IDisposable RunOnDeactivate()
        {
            var persistence = Resolve<LevelPersistenceService>().GetOrCreate(Id);
            var isActive = persistence.GetOrAdd<ReactiveValuePersistentComponent<bool>>();
            return isActive.ValueProperty.Where(it => it).Subscribe(_ => Clear());
        }


        protected override void OnClear()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}
