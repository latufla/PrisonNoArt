using Honeylab.Consumables;
using Honeylab.Gameplay.Cameras;
using Honeylab.Utils.Data;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Pool;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.Ui.Minimap
{
    [Serializable]
    public class MinimapIndicatorsServiceArgs
    {
        [SerializeField] private MinimapIndicatorsPool _pool;
        [SerializeField] private ConsumablePersistenceId _consumableId;

        public MinimapIndicatorsPool Pool => _pool;
        public ConsumablePersistenceId ConsumableId => _consumableId;
    }


    public class MinimapIndicatorsService : IDisposable
    {
        private readonly MinimapIndicatorsServiceArgs _args;
        private readonly CameraProvider _cameraProvider;

        private readonly List<MinimapIndicator> _indicators = new();

        private CancellationTokenSource _run;
        private bool _isVisible;


        public MinimapIndicatorsService(MinimapIndicatorsServiceArgs args, CameraProvider cameraProvider)
        {
            _args = args;
            _cameraProvider = cameraProvider;
        }


        public void Dispose()
        {
            _run?.CancelThenDispose();
            _run = null;

            foreach (MinimapIndicator indicator in _indicators)
            {
                _args.Pool.Push(indicator.Id, indicator.gameObject);
            }

            _indicators.Clear();
        }


        public MinimapIndicator Add(ScriptableId id, Transform target)
        {
            GameObject go = _args.Pool.Pop(id, false);
            MinimapIndicator indicator = go.GetComponent<MinimapIndicator>();
            indicator.Init(id, target, _cameraProvider.MinimapCamera);

            _indicators.Add(indicator);
            return indicator;
        }


        public void Remove(MinimapIndicator indicator)
        {
            ScriptableId id = indicator.Id;

            indicator.Clear();

            _indicators.Remove(indicator);
            _args.Pool.Push(id, indicator.gameObject);
        }


        public void RemoveAll()
        {
            int n = _indicators.Count;
            for (int i = n - 1; i >= 0; --i)
            {
                Remove(_indicators[i]);
            }
        }


        public ConsumablePersistenceId GetConsumableId() => _args.ConsumableId;
    }
}
