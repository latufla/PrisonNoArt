using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using Honeylab.Utils;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Buildings
{
    public class GasPopupWorkPanel : MonoBehaviour
    {
        [SerializeField] private TimeProgressPanel _progressPanel;
        [SerializeField] private ConsumablesAmountPanel _resultPanel;
        [SerializeField] private GameObject _completePanel;
        [SerializeField] private Button _collectButtonEnabled;
        [SerializeField] private Button _collectButtonDisabled;
        [SerializeField] private Button _getConsumablesButton;

        private CompositeDisposable _showWorkDisposable;

        public Button GetConsumablesButton => _getConsumablesButton;
        public IObservable<Unit> OnCollectButtonClickAsObserver() => _collectButtonEnabled.OnClickAsObservable();
        public IObservable<Unit> OnGetConsumablesButtonClickAsObserver() => _getConsumablesButton.OnClickAsObservable();

        public void ShowWork(CraftBuildingFlow flow)
        {
            _showWorkDisposable?.Dispose();
            _showWorkDisposable = null;

            ConsumablesData consumablesData = flow.Resolve<ConsumablesData>();
            RewardAmountConfig result = flow.Config.CraftResult;
            ConsumableData resultData = consumablesData.GetData(result.Name);
            _resultPanel.SetIcon(resultData.Sprite);
            _showWorkDisposable = new();
            IDisposable updateProgress = flow.TimeLeft.Subscribe(timeLeft =>
            {
                int amount = flow.GetCurrentAmount();
                int amountLeft = flow.CraftAmount.Value - amount;
                if (timeLeft > 0)
                {
                    _progressPanel.SetTime(flow.Config.CraftDuration - (flow.Config.CraftDuration * amountLeft - (float)timeLeft),
                        flow.Config.CraftDuration);
                }

                _resultPanel.SetAmount(amount,flow.CraftAmount.Value);

                bool isFull = amount >= flow.CraftAmount.Value;
                _progressPanel.gameObject.SetActive(!isFull);
                _completePanel.SetActive(isFull);

                CollectButton(flow, amount);
            });
            _showWorkDisposable.Add(updateProgress);


            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }


        private void CollectButton(CraftBuildingFlow flow, int amount)
        {
            CraftBuildingInteractable interactable = flow.Get<CraftBuildingInteractable>();
            bool isCollect = interactable != null && interactable.HasAgent() && amount > 0;
            _collectButtonEnabled.gameObject.SetActive(isCollect);
            _collectButtonDisabled.gameObject.SetActive(!isCollect);
        }


        public void Clear()
        {
            _progressPanel.gameObject.SetActive(false);
            _completePanel.SetActive(false);
            _collectButtonEnabled.gameObject.SetActive(false);
            _collectButtonDisabled.gameObject.SetActive(false);

            _showWorkDisposable?.Dispose();
            _showWorkDisposable = null;
        }
    }
}
