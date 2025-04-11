using System.Threading;


namespace Honeylab.Gameplay.Ui.Upgrades
{
    public class WeaponUpgradeScreenPresenter : ScreenPresenterBase<WeaponUpgradeScreenResult>
    {
        public WeaponUpgradeScreenPresenter(ScreenFactory factory) : base(factory) { }


        protected override void OnRun(CancellationToken ct)
        {
            Screen.Run();
        }


        protected override void OnStop()
        {
            Screen.Stop();
        }
    }
}
