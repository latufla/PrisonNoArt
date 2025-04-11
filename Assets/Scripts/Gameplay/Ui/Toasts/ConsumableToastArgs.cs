using Honeylab.Consumables;
using UnityEngine;


namespace Honeylab.Gameplay.Ui
{
    public readonly struct ConsumableToastArgs
    {
        public readonly ConsumablePersistenceId PersistenceId;
        public readonly int Amount;
        public readonly Vector3 WorldPoint;

        public ConsumableToastArgs(ConsumablePersistenceId persistenceId, int amount, Vector3 worldPoint)
        {
            PersistenceId = persistenceId;
            Amount = amount;
            WorldPoint = worldPoint;
        }
    }
}
