using System.Collections.Generic;
using System.Linq;


namespace Honeylab.Utils.Persistence
{
    public class PersistenceStartup
    {
        private readonly SharedPersistenceService _sharedPersistenceService;
        private readonly IAutoSaveService[] _autoSaveServices;


        public PersistenceStartup(SharedPersistenceService sharedPersistenceService,
            IEnumerable<IAutoSaveService> autoSaveServices)
        {
            _sharedPersistenceService = sharedPersistenceService;
            _autoSaveServices = autoSaveServices.ToArray();
        }


        public void Run()
        {
            _sharedPersistenceService.Init();

            foreach (IAutoSaveService autoSaveService in _autoSaveServices)
            {
                autoSaveService.Run();
            }
        }
    }
}
