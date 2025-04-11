namespace Honeylab.Utils.Persistence
{
    public class SharedPersistenceService : PersistenceService
    {
        public SharedPersistenceService(IPersistenceStorageService storage) : base(storage) { }
    }
}
