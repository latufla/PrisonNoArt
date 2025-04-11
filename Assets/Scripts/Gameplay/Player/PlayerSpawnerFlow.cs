using Honeylab.Gameplay.World.Spawners;
using Honeylab.Utils.Configs;
using UnityEngine;


namespace Honeylab.Gameplay.Player
{
    public class PlayerSpawnerFlow : WorldObjectsSpawnerFlow
    {
        [SerializeField] private ConfigIdProvider _configId;

        public PlayerSpawnerConfig Config { get; private set; }

        public float NextSpawnTime { get; set; }
        public bool IsSpawnPaused { get; set; }

        protected override void OnInit()
        {
            IConfigsService configs = Resolve<IConfigsService>();
            Config = configs.Get<PlayerSpawnerConfig>(_configId.Id);
        }
    }
}
