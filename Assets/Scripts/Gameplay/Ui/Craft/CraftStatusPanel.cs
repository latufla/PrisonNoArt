using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Buildings;
using System.Threading;
using System;
using UniRx;
using Honeylab.Consumables.Configs;
using Honeylab.Consumables;

namespace Honeylab.Gameplay.Ui.Craft
{
    public class CraftStatusPanel : UIStatusPanelBase
    {
        public void StateChanged(CraftBuildingFlow flow)
        {
            Cts ??= new CancellationTokenSource();
            var state = flow.State.Value;
            switch (state)
            {
                case CraftBuildingStates.Idle:
                    IdleWork();
                    break;

                case CraftBuildingStates.Work:
                    RunAsync(flow, Cts.Token).Forget();
                    break;

                case CraftBuildingStates.Done:
                    CompleteWork(flow, Cts.Token).Forget();
                    break;
            }
        }
        private async UniTask RunAsync(CraftBuildingFlow flow, CancellationToken ct)
        {
            InProcess = true;

            await UniTask.WaitUntil(() => PlayerInputService != null && CameraTargetingService != null,
                cancellationToken: ct);

            if (Disposable != null)
            {
                return;
            }

            Show();

            await UniTask.WaitUntil(() =>
            {
                Animator.SetInteger("ProcessState", 1);
                return Animator.GetInteger("ProcessState") == 1;
            }, cancellationToken: ct);

            UpdateInfo(flow);

            ProgressPanel.gameObject.SetActive(true);

            Disposable = new CompositeDisposable();
            IDisposable updateProgress = flow.TimeLeft.Subscribe(timeLeft =>
            {
                ProgressPanel.SetTime((float)timeLeft, flow.Config.CraftDuration * flow.CraftAmount.Value);
            });
            Disposable.Add(updateProgress);
        }


        private async UniTask CompleteWork(CraftBuildingFlow flow, CancellationToken ct)
        {
            InProcess = true;

            Disposable?.Dispose();
            Disposable = null;

            Show();

            await UniTask.WaitUntil(() =>
            {
                Animator.SetInteger("ProcessState", 2);
                return Animator.GetInteger("ProcessState") == 2;
            }, cancellationToken: ct);


            UpdateInfo(flow);

            Disposable = new CompositeDisposable();

            IDisposable onButtonClick = OnButtonClickAsObservable()
                .Subscribe(_ => { FocusTarget(flow.transform, ct).Forget(); });
            Disposable.Add(onButtonClick);
        }


        private void UpdateInfo(CraftBuildingFlow flow)
        {
            ConsumablesData consumablesData = flow.Resolve<ConsumablesData>();

            RewardAmountConfig result = flow.Config.CraftResult;
            ConsumableData resultData = consumablesData.GetData(result.Name);
            SetIcon(resultData.Sprite);
            SetText(result.Amount * flow.CraftAmount.Value);
        }
    }
}
