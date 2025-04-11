using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Data;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


namespace Honeylab.Utils.OffscreenTargetIndicators
{
    [Serializable]
    public class OffscreenIndicatorsServiceArgs
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private OffscreenIndicatorsPool _pool;

        public Canvas Canvas => _canvas;
        public OffscreenIndicatorsPool Pool => _pool;
    }


    public class OffscreenIndicatorsService : IDisposable
    {
        private readonly OffscreenIndicatorsServiceArgs _args;
        private readonly WorldObjectsService _worldObjectsService;
        private readonly List<OffscreenIndicator> _indicators = new();

        private PlayerFlow _playerFlow;

        private CancellationTokenSource _run;
        private bool _isVisible;


        public OffscreenIndicatorsService(OffscreenIndicatorsServiceArgs args,
            WorldObjectsService worldObjectsService)
        {
            _args = args;
            _worldObjectsService = worldObjectsService;
        }


        public void Run()
        {
            _run = new CancellationTokenSource();

            RunAsync(_run.Token).Forget();
        }


        public void Dispose()
        {
            _run?.CancelThenDispose();
            _run = null;

            foreach (OffscreenIndicator indicator in _indicators)
            {
                _args.Pool.Push(indicator.Id, indicator.gameObject);
            }

            _indicators.Clear();
        }


        public void SetIndicatorsVisible(bool isVisible)
        {
            _isVisible = isVisible;

            foreach (OffscreenIndicator indicator in _indicators)
            {
                indicator.SetVisible(_isVisible);
            }
        }


        public OffscreenIndicator Add(ScriptableId id, Transform target)
        {
            GameObject go = _args.Pool.Pop(id, false);
            OffscreenIndicator indicator = go.GetComponent<OffscreenIndicator>();

            indicator.transform.SetParent(_args.Canvas.transform);
            indicator.transform.localPosition = Vector3.zero;
            indicator.transform.localRotation = Quaternion.identity;
            indicator.transform.localScale = Vector3.one;

            indicator.Init(id, target);

            _indicators.Add(indicator);
            return indicator;
        }

        public OffscreenIndicator AddEnemy(ScriptableId id, Transform target)
        {
            GameObject go = _args.Pool.Pop(id, false);
            OffscreenIndicator indicator = go.GetComponent<OffscreenIndicator>();

            indicator.transform.SetParent(_args.Canvas.transform);
            indicator.transform.localPosition = Vector3.zero;
            indicator.transform.localRotation = Quaternion.identity;
            indicator.transform.localScale = Vector3.one;

            indicator.Init(id, target);

            _indicators.Add(indicator);
            return indicator;
        }

        public void Remove(OffscreenIndicator indicator)
        {
            ScriptableId id = indicator.Id;

            indicator.Clear();

            _indicators.Remove(indicator);
            _args.Pool.Push(id, indicator.gameObject);
        }


        private async UniTask RunAsync(CancellationToken ct)
        {
            while (_playerFlow == null)
            {
                _playerFlow = _worldObjectsService.GetObjects<PlayerFlow>().FirstOrDefault();
                await UniTask.Yield(ct);
            }

            while (true)
            {
                await UniTask.Yield(ct);

                _indicators.ForEach(it => it.UpdateIndicator(_playerFlow.transform, 1));
            }
        }
    }
}
