using Honeylab.Utils.App;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Honeylab.Utils.Analytics.CommonAnalytics.Messages;


namespace Honeylab.Utils.Analytics
{
    public class MainCommonPayloadProvider : ICommonPayloadProvider
    {
        private readonly AppInstallDateTimeService _appInstallDateTimeService;
        private readonly AppSessionService _appSessionService;
        private readonly AppGlobalTimerService _appGlobalTimerService;
        private readonly InterstitialsWatchCountReadService _interstitialsWatchCountReadService;
        private readonly RewardedVideosWatchCountService _rewardedVideosWatchCountService;


        public MainCommonPayloadProvider(AppInstallDateTimeService appInstallDateTimeService,
            AppSessionService appSessionService,
            AppGlobalTimerService appGlobalTimerService,
            InterstitialsWatchCountReadService interstitialsWatchCountReadService,
            RewardedVideosWatchCountService rewardedVideosWatchCountService)
        {
            _appInstallDateTimeService = appInstallDateTimeService;
            _appSessionService = appSessionService;
            _appGlobalTimerService = appGlobalTimerService;
            _interstitialsWatchCountReadService = interstitialsWatchCountReadService;
            _rewardedVideosWatchCountService = rewardedVideosWatchCountService;
        }


        public void AddPayload(IDictionary<string, object> commonPayload)
        {
            DateTime utcNow = DateTime.UtcNow;

            commonPayload.Add(SessionCount, _appSessionService.GetSessionNumber());
            commonPayload.Add(SessionSeconds, _appSessionService.GetSecondsPassedSession());

            DateTime appInstallDateTime = _appInstallDateTimeService.GetAppInstallUtcDateTime();

            string appInstallDateStr = appInstallDateTime.ToString("dd.MM.yyyy");
            commonPayload.Add(AppInstallDate, appInstallDateStr);

            long appInstallTimestamp = ((DateTimeOffset)appInstallDateTime).ToUnixTimeSeconds();
            commonPayload.Add(AppInstallTimestamp, appInstallTimestamp);

            int daysAfterAppInstall = (int)Math.Ceiling((utcNow - appInstallDateTime).TotalDays);
            commonPayload.Add(DaysAfterAppInstall, daysAfterAppInstall);

            commonPayload.Add(TotalGameplaySeconds, _appGlobalTimerService.GetSecondsPassedGlobal());

            commonPayload.Add(InterstitialWatchCount, _interstitialsWatchCountReadService.GetWatchCount());
            commonPayload.Add(RewardedVideoWatchCount, _rewardedVideosWatchCountService.GetWatchCount());

            NetworkReachability internetReachability = Application.internetReachability;
            commonPayload.Add(HasInternet, internetReachability != NetworkReachability.NotReachable);
        }
    }
}
