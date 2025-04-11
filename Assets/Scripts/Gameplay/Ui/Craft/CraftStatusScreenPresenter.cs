using System.Threading;

namespace Honeylab.Gameplay.Ui.Craft
{
    public class CraftStatusScreenPresenter : ScreenPresenterBase<CraftStatusScreen>
    {

        public CraftStatusScreenPresenter(ScreenFactory factory) : base(factory)
        {
        }


        protected override void OnRun(CancellationToken ct)
        {
            Screen.Run(ct);
        }


        protected override void OnStop()
        {
            Screen.Stop();
        }
    }
}
