using Honeylab.Gameplay.Interactables;
using Honeylab.Gameplay.Interactables.World;
using Honeylab.Gameplay.World;
using UnityEngine;

namespace Honeylab.Gameplay.Player
{
    public class PlayerSpawnerInteractable : InteractableBase
    {
        [SerializeField] private Transform _spawnPoint;
        private WorldObjectFlow _flow;
        private PlayerSpawner _playerSpawner;

        public Transform SpawnPoint => _spawnPoint;

        protected override void OnInit()
        {
            _flow = GetFlow();
            _playerSpawner = _flow.Get<PlayerSpawner>();
        }

        protected override void OnEnterInteract(IInteractAgent agent)
        {
            _playerSpawner.SetSpawnPoint(_spawnPoint);
        }
    }
}
