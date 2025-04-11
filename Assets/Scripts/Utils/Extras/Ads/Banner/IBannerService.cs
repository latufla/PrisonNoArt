using System;


namespace Honeylab.Utils.Ads
{
    public interface IBannerService
    {
        event Action<BannerClickedEventArgs> BannerClicked;
        void ShowBanner();
        void HideBanner();
    }
}
