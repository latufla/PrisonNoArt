using Cysharp.Threading.Tasks;
using Honeylab.Cutscene;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.World;
using Honeylab.Project.Levels;
using Honeylab.Utils.Data;
using Honeylab.Utils.Extensions;
using Honeylab.Utils.OffscreenTargetIndicators;
using Honeylab.Utils.Persistence;
using System;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;


namespace Honeylab.Gameplay.Buildings
{
    public class LevelLeaveBuildingView : WorldObjectComponentBase
    {
        [SerializeField] private LevelLeaveBuildingPopupView _levelLeaveBuildingPopupView;
        [SerializeField] private ScriptableId _offscreenIndicatorId;
        [SerializeField] private PersistenceId _cutsceneId;

        private LevelLeaveBuildingFlow _flow;
        private LevelsLoadService _levelsLoadService;
        private CutsceneService _cutsceneService;
        private WorldObjectsService _world;

        private LevelLeaveBuildingPopup _levelLeaveBuildingPopup;
        private CompositeDisposable _showPopupDisposable;
        private CancellationTokenSource _cts;

        private OffscreenIndicator _offscreenIndicator;
        private OffscreenIndicatorsService _offscreenIndicators;


        protected override void OnInit()
        {
            _flow = GetFlow<LevelLeaveBuildingFlow>();
            _levelsLoadService = _flow.Resolve<LevelsLoadService>();
            _cutsceneService = _flow.Resolve<CutsceneService>();
            _world = _flow.Resolve<WorldObjectsService>();

            GameplayPoolsService pools = _flow.Resolve<GameplayPoolsService>();
            BillboardPresenterFactory billboards = _flow.Resolve<BillboardPresenterFactory>();

            _levelLeaveBuildingPopupView.Init(pools, billboards);

            _offscreenIndicators = _flow.Resolve<OffscreenIndicatorsService>();
            if (_offscreenIndicatorId != null)
            {
                _offscreenIndicator = _offscreenIndicators.Add(_offscreenIndicatorId, transform);
            }
        }


        protected override void OnClear()
        {
            if (_offscreenIndicator != null)
            {
                _offscreenIndicators.Remove(_offscreenIndicator);
                _offscreenIndicator = null;
            }

            _showPopupDisposable?.Dispose();
            _showPopupDisposable = null;

            _cts?.CancelThenDispose();
            _cts = null;
        }


        public void ShowLevelLeavePopup()
        {
            if (_levelLeaveBuildingPopup != null)
            {
                return;
            }

            _levelLeaveBuildingPopup = _levelLeaveBuildingPopupView.Show();

            _showPopupDisposable = new CompositeDisposable();
            _cts = new CancellationTokenSource();
            IDisposable craft = _levelLeaveBuildingPopup.OnLevelLeaveButtonClickAsObservable()
                .Subscribe(_ =>
                {
                    LeaveLevelAsync(_cts.Token).Forget();
                });
            _showPopupDisposable.Add(craft);
        }

        private async UniTask LeaveLevelAsync(CancellationToken ct)
        {
            PlayerFlow player = null;
            await UniTask.WaitUntil(() =>
            {
                player = _world.GetObjects<PlayerFlow>().FirstOrDefault();
                return player != null;
            }, cancellationToken: ct);

            var interaction = player.Get<PlayerInteraction>();
            interaction.ExitInteract();
            player.gameObject.SetActive(false);

            await _cutsceneService.PlayCutsceneAsync(_cutsceneId, ct);

            _levelsLoadService.LeaveLevel();
            _levelsLoadService.LoadLevelInOrderAsync(_flow.Config.NextLevel).Forget();
        }


        public void HideLevelLeavePopup()
        {
            _showPopupDisposable?.Dispose();
            _showPopupDisposable = null;

            if (_levelLeaveBuildingPopup != null)
            {
                _levelLeaveBuildingPopupView.HideAsync(true).Forget();
                _levelLeaveBuildingPopup = null;
            }
        }


        public bool IsLevelLeavePopupShown() => _levelLeaveBuildingPopup != null;
    }
}
