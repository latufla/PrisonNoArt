using Honeylab.Utils.App;


namespace Honeylab.Utils.Persistence
{
    public class AppPauseAutoSaveService : IAutoSaveService
    {
        private readonly SharedPersistenceService _sharedPersistenceService;
        private readonly LevelPersistenceService _levelPersistenceService;

        private readonly IAppPauseStateProvider _appPauseStateProvider;


        public AppPauseAutoSaveService(SharedPersistenceService sharedPersistenceService,
            LevelPersistenceService levelPersistenceService,
            IAppPauseStateProvider appPauseStateProvider)
        {
            _sharedPersistenceService = sharedPersistenceService;
            _levelPersistenceService = levelPersistenceService;
            _appPauseStateProvider = appPauseStateProvider;
        }


        public void Run() => _appPauseStateProvider.AppPause += AppPauseStateProvider_OnAppPause;
        public void Dispose() => _appPauseStateProvider.AppPause -= AppPauseStateProvider_OnAppPause;


        private void AppPauseStateProvider_OnAppPause(bool isPaused)
        {
            _sharedPersistenceService.Save();
            _levelPersistenceService.Save();
        }
    }
}
