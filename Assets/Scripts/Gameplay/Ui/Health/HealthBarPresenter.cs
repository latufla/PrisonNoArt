using System;
using System.Threading;
using UniRx;


namespace Honeylab.Gameplay.Ui.Health
{
    public class HealthBarPresenter : ScreenPresenterBase<HealthBarScreen>
    {
        private readonly Gameplay.Health _health;
        private CompositeDisposable _disposable;

        public HealthBarPresenter(ScreenFactory factory,
            Gameplay.Health health) : base(factory)
        {
            _health = health;
        }


        protected override void OnRun(CancellationToken ct)
        {
            Screen.View.SetHealthInitial(_health.HealthProp.Value,
                _health.MaxHealth,
                true);

            _disposable = new CompositeDisposable();
            IDisposable healthChanged = _health.HealthProp.Subscribe(it => { Screen.View.SetHealth(it, _health.MaxHealth, true, true); });
            _disposable.Add(healthChanged);
        }


        protected override void OnStop()
        {
            _disposable?.Dispose();
            _disposable = null;
        }
    }
}
