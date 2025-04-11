using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.Pool;
using Honeylab.Utils.Tutorial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.AI;


namespace Honeylab.Gameplay.Tutorial
{
    public class TutorialView : WorldObjectComponentBase
    {
        [SerializeField] private bool _enabled;
        [SerializeField] private GameObjectPoolBehaviour _pool;
        [SerializeField] private float _directedDelay;
        [SerializeField] private float _directedTime;
        [SerializeField] private float _directedDistance;
        [SerializeField] private float _directedSpeed;
        private WorldObjectsService _world;
        private TutorialScreenPresenter _tutorialScreenPresenter;
        private CompositeDisposable _disposable;
        private CancellationTokenSource _cts;
        private TutorialFlow _flow;
        private List<TrailRenderer> _items = new();
        private NavMeshPath _path;
        protected override void OnInit()
        {
            _flow = GetFlow<TutorialFlow>();
            _world = _flow.Resolve<WorldObjectsService>();
            _tutorialScreenPresenter = _flow.Resolve<TutorialScreenPresenter>();
        }


        protected override void OnRun()
        {
            if (!_enabled)
            {
                return;
            }

            _disposable = new CompositeDisposable();
            _cts?.CancelThenDispose();
            _cts = new CancellationTokenSource();

            WorkAsync(_cts.Token).Forget();
        }


        private async UniTask WorkAsync(CancellationToken ct)
        {
            TutorialScreen screen = _tutorialScreenPresenter.Screen;
            await UniTask.WaitUntil(() =>
                {
                    screen = _tutorialScreenPresenter.Screen;
                    return screen != null;
                },
                cancellationToken: ct);

            TutorialScreenFocus screenFocus = _tutorialScreenPresenter.ScreenFocus;
            await UniTask.WaitUntil(() =>
                {
                    screenFocus = _tutorialScreenPresenter.ScreenFocus;
                    return screenFocus != null;
                },
                cancellationToken: ct);

            /*IDisposable onShow = screen
                .OnShowScreenAsObservable()
                .Subscribe(info =>
                {
                    WorldObjectFlow target = info.WorldObject;
                    if (target != null)
                    {
                        DirectedWorkAsync(target.transform, ct).Forget();
                    }
                });
            _disposable.Add(onShow);*/

            IDisposable onFocus = screenFocus
                .OnEndFocusSubjectsAsObservable()
                .Subscribe(target =>
                {
                    DirectedWorkAsync(target, ct).Forget();
                });
            _disposable.Add(onFocus);
        }


        private async UniTask DirectedWorkAsync(Transform target, CancellationToken ct)
        {
            if (target == null)
            {
                return;
            }

            PlayerFlow playerFlow = null;
            await UniTask.WaitUntil(() =>
                {
                    playerFlow = _world.GetObjects<PlayerFlow>().First();
                    return playerFlow != null;
                },
                cancellationToken: ct);

            TrailRenderer directedRenderer = _pool.PopWithComponent<TrailRenderer>(false);
            _items.Add(directedRenderer);

            Transform directedRendererTransform = directedRenderer.transform;
            directedRendererTransform.SetParent(transform);
            directedRendererTransform.localPosition = Vector3.zero;

            await UniTask.Delay(TimeSpan.FromSeconds(_directedDelay), cancellationToken: ct);
            _path ??= new NavMeshPath();
            _path.ClearCorners();

            Vector3 playerPosition = playerFlow.transform.position;
            Vector3 targetPosition = target.position;

            Vector3 sourcePos = new(playerPosition.x, 1, playerPosition.z);
            Vector3 targetPos = new(targetPosition.x, 1, targetPosition.z);

            directedRendererTransform.position = sourcePos;
            directedRenderer.gameObject.SetActive(true);

            bool pathCompleted = NavMesh.CalculatePath(sourcePos, targetPos, NavMesh.AllAreas, _path);
            if (!pathCompleted)
            {
                return;
            }
            float time = _directedTime;
            Vector3[] corners = _path.corners;
            int lastIndex = corners.Length - 1;
            int i = 0;
            directedRenderer.time = _directedDistance;
            directedRenderer.enabled = true;
            await UniTask.Yield(ct);
            while (time > 0)
            {
                if (i <= lastIndex && corners.Length >= i)
                {
                    directedRenderer.transform.position =
                        Vector3.MoveTowards(directedRenderer.transform.position,
                            corners[i],
                            _directedSpeed * Time.deltaTime);
                    if (Vector3.Distance(directedRenderer.transform.position, corners[i]) < 0.1f)
                    {
                        i++;
                    }
                }

                time -= Time.deltaTime;
                await UniTask.Yield(ct);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(directedRenderer.time), cancellationToken: ct);
            directedRenderer.enabled = false;
            _pool.Push(directedRenderer.gameObject);
            _items.Remove(directedRenderer);
        }


        protected override void OnClear()
        {
            _disposable?.Clear();
            _disposable?.Dispose();
            _disposable = null;

            _cts?.CancelThenDispose();
            _cts = null;

            foreach (TrailRenderer item in _items)
            {
                _pool.Push(item.gameObject);
            }
            _items.Clear();
        }
    }
}
