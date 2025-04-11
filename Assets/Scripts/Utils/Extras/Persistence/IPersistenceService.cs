namespace Honeylab.Utils.Persistence
{
    public interface IPersistenceService
    {
        void Init();
        void Clear();
        bool TryGet(PersistenceId id, out PersistentObject persistentObject);
        PersistentObject Create(PersistenceId id);
        void Remove(PersistenceId id);
        void Save();
        void SaveJson(string saveCopyJson);
        string GetSave();
        void SetPause(bool isPause);
    }
}
