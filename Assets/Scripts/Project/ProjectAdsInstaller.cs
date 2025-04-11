using Honeylab.Utils.Ads;
using Zenject;


namespace Honeylab.Project
{
    public class ProjectAdsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<StubInterstitialService>()
                .AsSingle()
                .WithArguments(InterstitialStartResult.InterstitialsDisabled);

            Container.BindInterfacesTo<StubRewardedVideoService>()
                .AsSingle()
                .WithArguments(RewardedVideoStartResult.Success);
        }
    }
}
