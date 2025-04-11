using Honeylab.Consumables;
using Honeylab.Consumables.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Buildings.View
{
    public class UnlockConsumablesPopupPanel : MonoBehaviour
    {
        [SerializeField] private Transform _consumablePanelsRoot;
        [SerializeField] private Button _completeUseButton;

        private List<UnlockConsumablesAmountPanel> _consumablePanels;
        private List<UnlockConsumablesAmountPanel> _consumableCompletePanels;

        private readonly ISubject<int> _useButtonClickSubject = new Subject<int>();

        private CompositeDisposable _idleDisposable;


        public IObservable<int> OnUseButtonClickAsObservable() => _useButtonClickSubject.AsObservable();


        public IObservable<Unit> OnCompleteUseButtonClickAsObservable() => _completeUseButton.OnClickAsObservable();


        public void Show(UnlockBuildingFlow flow)
        {
            _idleDisposable?.Dispose();
            _idleDisposable = new CompositeDisposable();

            gameObject.SetActive(true);

            ConsumablesData consumablesData = flow.Resolve<ConsumablesData>();
            ConsumablesService consumablesService = flow.Resolve<ConsumablesService>();
            var consumablePanels = GetConsumablePanels();
            int n = consumablePanels.Count;
            int m = flow.Config.UnlockPrice.Count;
            bool isAllCompleted = true;
            for (int i = 0; i < n; ++i)
            {
                UnlockConsumablesAmountPanel panel = consumablePanels[i];
                bool hasAmount = i < m;
                panel.gameObject.SetActive(hasAmount);

                if (!hasAmount)
                {
                    continue;
                }

                RewardAmountConfig amountConfig = flow.Config.UnlockPrice[i];
                ConsumableData data = consumablesData.GetData(amountConfig.Name);

                panel.SetName(data.Title);

                panel.SetIcon(data.Sprite);

                var amountProp = consumablesService.GetAmountProp(data.Id);
                int consumableAmount = flow.Consumables.Amounts[i];
                panel.SetAmount(consumableAmount, amountConfig.Amount);

                bool isCompleted = consumableAmount >= amountConfig.Amount;
                panel.SetComplete(isCompleted);

                if (!isCompleted)
                {
                    panel.SetUseButtonEnabled(amountProp.Value > 0);

                    int index = i;
                    IDisposable useButtonClick = panel.OnUseButtonClickAsObserver()
                        .Subscribe(_ => { _useButtonClickSubject.OnNext(index); });
                    _idleDisposable.Add(useButtonClick);
                }

                if (isAllCompleted && consumableAmount < amountConfig.Amount)
                {
                    isAllCompleted = false;
                }
            }

            _completeUseButton.gameObject.SetActive(isAllCompleted);

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_consumablePanelsRoot);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
        }


        public void Hide()
        {
            gameObject.SetActive(false);
            _completeUseButton.gameObject.SetActive(false);

            _idleDisposable?.Dispose();
            _idleDisposable = null;
        }


        private List<UnlockConsumablesAmountPanel> GetConsumablePanels()
        {
            _consumablePanels ??=
                _consumablePanelsRoot.GetComponentsInChildren<UnlockConsumablesAmountPanel>().ToList();
            return _consumablePanels;
        }
    }
}
