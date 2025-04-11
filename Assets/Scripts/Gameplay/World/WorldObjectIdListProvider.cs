using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Gameplay.World
{
    public class WorldObjectIdListProvider : MonoBehaviour
    {
        [SerializeField] private List<WorldObjectId> _objects;


        public List<WorldObjectId> Objects => _objects;
    }
}
