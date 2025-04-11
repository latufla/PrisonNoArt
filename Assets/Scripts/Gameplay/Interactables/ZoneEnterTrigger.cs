using System.Collections.Generic;
using UnityEngine;
using Honeylab.Gameplay.Interactables.World;


namespace Honeylab.Gameplay.Interactables
{
    public class ZoneEnterTrigger : InteractableBase
    {
        [SerializeField]
        private List<GameObject> _zones = new List<GameObject>();

        [SerializeField]
        private bool _inverseActive;

        protected override void OnEnterInteract(IInteractAgent agent)
        {
            _zones.ForEach(zone => zone.SetActive(!_inverseActive));
        }

        protected override void OnExitInteract(IInteractAgent agent)
        {
            _zones.ForEach(zone => zone.SetActive(_inverseActive));
        }
    }
}
