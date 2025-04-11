using UnityEngine;


namespace Honeylab.Utils.Triggers
{
    public abstract class TriggerFilterBehaviour<T> : MonoBehaviour, ITriggerFilter<T>
    {
        public abstract bool ShouldPassObject(T obj);
    }
}
