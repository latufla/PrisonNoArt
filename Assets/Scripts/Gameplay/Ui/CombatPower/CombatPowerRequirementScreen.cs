using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace Honeylab.Gameplay.Ui.CombatPower
{
    public class CombatPowerRequirementScreen : ScreenBase
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private Button _backgroundButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _equipmentButton;

        public override string Name => ScreenName.CombatPower;
        public IObservable<Unit> OnShopButtonClickAsObservable() => _shopButton.OnClickAsObservable();
        public IObservable<Unit> OnEquipmentButtonClickAsObservable() => _equipmentButton.OnClickAsObservable();


        public override IObservable<Unit> OnCloseButtonClickAsObservable() => base.OnCloseButtonClickAsObservable()
            .Merge(_backgroundButton.OnClickAsObservable());


        public void Run() { }


        public async UniTask RunAsync(CancellationToken ct)
        {
            await UniTask.Yield(ct);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_root);
            await UniTask.Yield(ct);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_root);
        }


        public void Clear() { }
    }
}
