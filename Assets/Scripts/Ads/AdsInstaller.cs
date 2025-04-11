using Zenject;


namespace Honeylab.Ads
{
    public class AdsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<RewardedAdsService>()
                .AsSingle();
        }
    }
}
