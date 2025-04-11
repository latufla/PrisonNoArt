using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui
{
    public class ConsumableCounterHardView : ConsumableCounterView
    {
        [SerializeField] private Button _clickButton;

        public IObservable<Unit> OnButtonClickAsObservable() => _clickButton.OnClickAsObservable();
    }
}
