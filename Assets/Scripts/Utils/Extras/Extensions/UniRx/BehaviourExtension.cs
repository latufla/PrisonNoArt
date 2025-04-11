using UniRx;
using UniRx.Triggers;
using UnityEngine;


namespace Honeylab.Utils.Extensions
{
    public static class BehaviourExtension
    {
        public static ReadOnlyReactiveProperty<bool> CreateEnabledStateProp(this Behaviour behaviour)
        {
            var enables = behaviour
                .OnEnableAsObservable()
                .Select(_ => true);
            var disables = behaviour
                .OnDisableAsObservable()
                .Select(_ => false);
            return enables
                .Merge(disables)
                .ToReadOnlyReactiveProperty(behaviour.isActiveAndEnabled);
        }
    }
}
