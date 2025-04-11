using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Gameplay.World.Spawners
{
    public class WorldObjectsSpawnerFlow : WorldObjectFlow
    {
        [SerializeField] private Transform _objectsRoot;
        [SerializeField] private WorldObjectIdListProvider _objectIds;

        public Transform ObjectsRoot => _objectsRoot;
        public List<WorldObjectId> ObjectIds => _objectIds.Objects;

        public List<WorldObjectFlow> Objects { get; } = new();
    }
}
