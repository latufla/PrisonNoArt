using Honeylab.Gameplay.World;
using UnityEngine;


namespace Honeylab.Gameplay.Creatures.Spawners
{
    public class EnemySpawnerWithUnlock : EnemySpawner
    {
        [SerializeField] private WorldObjectFlow _unlockWorldObject;
        private WorldObjectsService _world;
        private bool _isUnlocked;

        protected override void OnInit()
        {
            base.OnInit();
            EnemySpawnerFlow flow = GetFlow<EnemySpawnerFlow>();
            _world = flow.Resolve<WorldObjectsService>();
            _unlockWorldObject.gameObject.SetActive(false);

            int maxInLifetimeCount = flow.Config.MaxInLifetimeCount;
            _isUnlocked = maxInLifetimeCount > 0 && flow.DiedEnemiesPersistence.Value >= maxInLifetimeCount;

            _unlockWorldObject.gameObject.SetActive(_isUnlocked);
            if (_isUnlocked)
            {
                _world.AddObject(_unlockWorldObject);
            }
        }


        protected override void OnDespawn()
        {
            base.OnDespawn();

            if (!_isUnlocked)
            {
                _unlockWorldObject.gameObject.SetActive(true);
                _world.AddObject(_unlockWorldObject);
                _world.RunObject(_unlockWorldObject);
                _isUnlocked = true;
            }
        }
    }
}
