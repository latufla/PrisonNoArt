namespace Honeylab.Utils.Persistence
{
    public class LevelPersistenceService : PersistenceService
    {
        private int _levelIndex;

        public LevelPersistenceService(IPersistenceStorageService storage) : base(storage) { }


        public void SetLevelIndex(int levelIndex)
        {
            _levelIndex = levelIndex;

            string ending = string.Concat("_", _levelIndex);
            Storage.SetFileNameEnding(ending);
        }
    }
}
