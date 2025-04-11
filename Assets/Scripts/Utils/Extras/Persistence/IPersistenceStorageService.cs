using System.Collections.Generic;


namespace Honeylab.Utils.Persistence
{
    public interface IPersistenceStorageService
    {
        void Init();
        void Clear();
        void Save();
        void SaveJson(string path);
        string GetSave();
        void Reset();
        void SetPause(bool isPause);
        void Add(PersistentObject obj);
        void Remove(PersistenceId obj);
        IReadOnlyList<PersistentObject> GetAll();
        void SetFileNamePostfix(string postfix);
        void SetFileNameEnding(string ending);
    }
}
