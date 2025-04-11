using Honeylab.Utils.App;


namespace Honeylab.Utils.Persistence
{
    public class AppQuitAutoSaveService : IAutoSaveService
    {
        private readonly SharedPersistenceService _sharedPersistenceService;
        private readonly LevelPersistenceService _levelPersistenceService;
        private readonly IAppQuitProvider _appQuitProvider;


        public AppQuitAutoSaveService(SharedPersistenceService sharedPersistenceService,
            LevelPersistenceService levelPersistenceService,
            IAppQuitProvider appQuitProvider)
        {
            _sharedPersistenceService = sharedPersistenceService;
            _levelPersistenceService = levelPersistenceService;
            _appQuitProvider = appQuitProvider;
        }


        public void Run() => _appQuitProvider.AppQuit += AppQuitProvider_OnAppQuit;
        public void Dispose() => _appQuitProvider.AppQuit -= AppQuitProvider_OnAppQuit;


        private void AppQuitProvider_OnAppQuit()
        {
            _sharedPersistenceService.Save();
            _levelPersistenceService.Save();
        }
    }
}
