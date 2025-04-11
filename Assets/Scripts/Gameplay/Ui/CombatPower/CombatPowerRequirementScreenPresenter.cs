using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Player;
using UniRx;

namespace Honeylab.Gameplay.Ui.CombatPower
{
    public class CombatPowerRequirementScreenPresenter : ScreenPresenterBase<CombatPowerRequirementScreen>
    {
        private readonly PlayerInputService _playerInputService;
        private IDisposable _blockInput;
        private CompositeDisposable _disposable;
        private Action<ScreenOpenType> _runInventoryScreenAction;

        public CombatPowerRequirementScreenPresenter(ScreenFactory factory,
            PlayerInputService playerInputService,
            Action<ScreenOpenType> runInventoryScreenAction) : base(factory)
        {
            _playerInputService = playerInputService;
            _runInventoryScreenAction = runInventoryScreenAction;
        }


        protected override void OnRun(CancellationToken ct)
        {
            _blockInput = _playerInputService.BlockInput();
            Screen.RunAsync(ct).Forget();

            _disposable = new CompositeDisposable();

            IDisposable onShopClick = Screen.OnShopButtonClickAsObservable()
                .Subscribe(_ =>
                {
                    Screen.ScreenAction(ScreenParameters.Shop);
                    Stop();
                });
            _disposable.Add(onShopClick);

            IDisposable onEquipmentClick = Screen.OnEquipmentButtonClickAsObservable()
                .Subscribe(_ =>
                {
                    Screen.ScreenAction(ScreenParameters.Inventory);
                    _runInventoryScreenAction?.Invoke(ScreenOpenType.Auto);
                    Stop();
                });
            _disposable.Add(onEquipmentClick);
        }


        protected override void OnStop()
        {
            Screen.Clear();

            _blockInput?.Dispose();
            _blockInput = null;

            _disposable?.Dispose();
            _disposable = null;
        }
    }
}
