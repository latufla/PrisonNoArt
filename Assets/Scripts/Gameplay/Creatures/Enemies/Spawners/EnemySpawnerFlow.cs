using Honeylab.Gameplay.World.Spawners;
using Honeylab.Persistence;
using Honeylab.Utils.Configs;
using Honeylab.Utils.Persistence;
using UnityEngine;


namespace Honeylab.Gameplay.Creatures.Spawners
{
    public class EnemySpawnerFlow : WorldObjectsSpawnerFlow
    {
        [SerializeField] private ConfigIdProvider _configId;
        [SerializeField] private ConfigIdProvider _enemyConfigId;

        public EnemySpawnerConfig Config { get; private set; }
        public EnemySpawnerPersistentComponent DiedEnemiesPersistence { get; set; }

        public float NextSpawnTime { get; set; }
        public bool IsSpawnPaused { get; set; }

        public string EnemyConfig => _enemyConfigId.Id;

        protected override void OnInit()
        {
            IConfigsService configs = Resolve<IConfigsService>();
            Config = configs.Get<EnemySpawnerConfig>(_configId.Id);

            LevelPersistenceService persistence = Resolve<LevelPersistenceService>();
            PersistentObject po = persistence.GetOrCreate(Id);
            DiedEnemiesPersistence = po.GetOrAdd<EnemySpawnerPersistentComponent>();
        }
    }
}
