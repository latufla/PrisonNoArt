using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Pools;
using Honeylab.Pools;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Pool;
using System;
using System.Threading;
using UnityEngine;


namespace Honeylab.Utils
{
    public class PointingArrow : IDisposable
    {
        private readonly CancellationTokenSource _disposeCts = new CancellationTokenSource();
        private readonly PointingArrowsPool _pool;

        public PointingArrowView TutorialPlayerArrowView { get; private set; }


        public PointingArrow(GameplayPoolsService pools)
        {
            _pool = pools.Get<PointingArrowsPool>();
        }


        public void Init(Transform origin)
        {
            TutorialPlayerArrowView = _pool.PopWithComponent<PointingArrowView>(true);
            TutorialPlayerArrowView.Init(origin);
            TutorialPlayerArrowView.RunAsync(_disposeCts.Token).Forget();
        }


        public void Show(Transform origin, Transform target)
        {
            if (!TutorialPlayerArrowView)
            {
                Init(origin);
            }

            TutorialPlayerArrowView.Show(target);
        }


        public void Hide()
        {
            if (!TutorialPlayerArrowView)
            {
                return;
            }

            TutorialPlayerArrowView.Hide();
        }


        public void Dispose()
        {
            _disposeCts.CancelThenDispose();
        }
    }
}
