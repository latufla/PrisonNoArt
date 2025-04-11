namespace Honeylab.Utils.App
{
    public class AppGlobalServicesStartup
    {
        private readonly AppInstallDateTimeService _installDateTimeService;
        private readonly AppGlobalTimerService _globalTimerService;
        private readonly AppSessionService _sessionService;


        public AppGlobalServicesStartup(AppInstallDateTimeService installDateTimeService,
            AppGlobalTimerService globalTimerService,
            AppSessionService sessionService)
        {
            _installDateTimeService = installDateTimeService;
            _globalTimerService = globalTimerService;
            _sessionService = sessionService;
        }


        public void Run()
        {
            _installDateTimeService.Run();
            _globalTimerService.Run();
            _sessionService.Run();
        }
    }
}
