using Honeylab.Gameplay.Interactables;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace Honeylab.Utils.Triggers
{
    public class Trigger<T> : MonoBehaviour where T : IInteractable
    {
        [SerializeField] private TriggerFilterBehaviour<T>[] _filterBehaviours;

        private readonly List<ITriggerFilter<T>> _dynamicFilters = new List<ITriggerFilter<T>>();
        private readonly Dictionary<T, int> _enterCountByObj = new Dictionary<T, int>();

        private readonly ISubject<T> _onEnterSubject = new Subject<T>();
        private readonly ISubject<StayArgs> _onStaySubject = new Subject<StayArgs>();
        private readonly ISubject<T> _onExitSubject = new Subject<T>();

        private readonly Dictionary<T, IDisposable> _objToOnDisable = new();

        public IObservable<T> OnEnterAsObservable() => _onEnterSubject.AsObservable();
        public IObservable<StayArgs> OnStayAsObservable() => _onStaySubject.AsObservable();
        public IObservable<T> OnExitAsObservable() => _onExitSubject.AsObservable();


        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out T detectedObject) ||
                !IsPassingFilters(detectedObject))
            {
                return;
            }

            bool isEnteredNow = false;
            if (!_enterCountByObj.TryGetValue(detectedObject, out int oldEnterCount))
            {
                oldEnterCount = 0;
                isEnteredNow = true;
            }

            _enterCountByObj[detectedObject] = oldEnterCount + 1;
            if (isEnteredNow)
            {
                _onEnterSubject.OnNext(detectedObject);
            }

            IDisposable onDisable =
                detectedObject.OnDisabledAsObservable().Subscribe(_ => { TryRemove(detectedObject); });
            _objToOnDisable.Add(detectedObject, onDisable);
        }


        private void FixedUpdate()
        {
            foreach (var enterCountKvp in _enterCountByObj)
            {
                StayArgs args = new StayArgs(enterCountKvp.Key, Time.fixedDeltaTime * Time.timeScale);
                _onStaySubject.OnNext(args);
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out T detectedObject))
            {
                return;
            }

            TryRemove(detectedObject);
        }


        public void TryRemove(T obj)
        {
            if (!_enterCountByObj.TryGetValue(obj, out int oldEnterCount))
            {
                return;
            }

            int newEnterCount = oldEnterCount - 1;
            if (newEnterCount > 0)
            {
                _enterCountByObj[obj] = newEnterCount;
                return;
            }

            _enterCountByObj.Remove(obj);

            if (_objToOnDisable.ContainsKey(obj))
            {
                _objToOnDisable[obj].Dispose();
                _objToOnDisable.Remove(obj);
            }

            _onExitSubject.OnNext(obj);
        }


        public IEnumerable<T> EnumerateEnteredObjects() => _enterCountByObj.Keys;


        public void FillEnteredObjectsBuffer(ICollection<T> buffer)
        {
            buffer.Clear();
            foreach (var enterCountKvp in _enterCountByObj)
            {
                buffer.Add(enterCountKvp.Key);
            }
        }


        public bool HasEnteredObjects() => _enterCountByObj.Count > 0;
        public bool IsObjectEntered(T obj) => _enterCountByObj.ContainsKey(obj);

        public void AddFilter(ITriggerFilter<T> filter) => _dynamicFilters.Add(filter);
        public void RemoveFilter(ITriggerFilter<T> filter) => _dynamicFilters.Remove(filter);


        private bool IsPassingFilters(T obj)
        {
            foreach (var filterBehaviour in _filterBehaviours)
            {
                if (!filterBehaviour.ShouldPassObject(obj))
                {
                    return false;
                }
            }

            foreach (var dynamicFilter in _dynamicFilters)
            {
                if (!dynamicFilter.ShouldPassObject(obj))
                {
                    return false;
                }
            }

            return true;
        }


        public readonly struct StayArgs
        {
            public readonly T DetectedObject;
            public readonly float DeltaTime;


            public StayArgs(T detectedObject, float deltaTime)
            {
                DetectedObject = detectedObject;
                DeltaTime = deltaTime;
            }
        }
    }
}
