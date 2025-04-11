using System;
using System.ComponentModel;


namespace Honeylab.Utils.Persistence
{
    public class PersistenceService : IPersistenceService
    {
        protected readonly IPersistenceStorageService Storage;


        protected PersistenceService(IPersistenceStorageService storage)
        {
            Storage = storage;
        }


        public void Init()
        {
            Storage.Init();
        }


        public void Clear()
        {
            Storage.Clear();
        }


        public bool TryGet(PersistenceId id, out PersistentObject persistentObject)
        {
            var allObjects = Storage.GetAll();
            for (int i = 0; i < allObjects.Count; i++)
            {
                PersistentObject candidateObject = allObjects[i];
                if (candidateObject._id.Equals(id))
                {
                    persistentObject = candidateObject;
                    return true;
                }
            }

            persistentObject = default;
            return false;
        }


        public PersistentObject Create(PersistenceId id)
        {
            if (TryGet(id, out _))
            {
                throw new InvalidOperationException();
            }

            PersistentObject obj = new(id);
            Storage.Add(obj);
            return obj;
        }


        public void Remove(PersistenceId id)
        {
            if (!TryGet(id, out _))
            {
                throw new InvalidOperationException();
            }

            Storage.Remove(id);
        }


        public void Save()
        {
            Storage.Save();
        }


        #region Cheats

        [Description("Cheats only")]
        public void SaveJson(string file)
        {
            Storage.SaveJson(file);
        }


        [Description("Cheats only")]
        public string GetSave() => Storage.GetSave();


        [Description("Cheast only")]
        public void SetPause(bool isPause)
        {
            Storage.SetPause(isPause);
        }

        #endregion
    }
}
