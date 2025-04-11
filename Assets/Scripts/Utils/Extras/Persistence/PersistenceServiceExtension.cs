using System;


namespace Honeylab.Utils.Persistence
{
    public static class PersistenceServiceExtension
    {
        public static PersistentObject GetOrCreate(this IPersistenceService service, PersistenceId id) =>
            service.TryGet(id, out PersistentObject persistentObject) ? persistentObject : service.Create(id);


        public static bool TryGetComponent<T>(this IPersistenceService service, PersistenceId id, out T component)
            where T : PersistentComponent, new()
        {
            if (service.TryGet(id, out PersistentObject po))
            {
                return po.TryGetFirst(out component);
            }

            component = default;
            return false;
        }


        public static T GetComponent<T>(this IPersistenceService service, PersistenceId id)
            where T : PersistentComponent, new()
        {
            if (!TryGetComponent(service, id, out T component))
            {
                throw new InvalidOperationException($"No {typeof(T).Name} component found with id {id}");
            }

            return component;
        }


        public static bool HasComponent<T>(this IPersistenceService service, PersistenceId id)
            where T : PersistentComponent, new() => TryGetComponent(service, id, out T _);


        public static T GetOrAddComponent<T>(this IPersistenceService service, PersistenceId id)
            where T : PersistentComponent, new()
        {
            PersistentObject persistentObject = GetOrCreate(service, id);
            return persistentObject.GetOrAdd<T>();
        }


        public static T AddUniqueComponent<T>(this IPersistenceService service, PersistenceId id)
            where T : PersistentComponent, new()
        {
            PersistentObject persistentObject = GetOrCreate(service, id);
            if (persistentObject.Has<T>())
            {
                throw new InvalidOperationException();
            }

            return persistentObject.Add<T>();
        }


        public static T FirstComponent<T>(this IPersistenceService service, PersistenceId id)
            where T : PersistentComponent, new()
        {
            PersistentObject po = Get(service, id);
            return po.First<T>();
        }


        public static PersistentObject Get(this IPersistenceService service, PersistenceId id)
        {
            if (!service.TryGet(id, out PersistentObject po))
            {
                throw new ArgumentOutOfRangeException(nameof(id), id, "Not found.");
            }

            return po;
        }


        public static bool Has(this IPersistenceService service, PersistenceId id) => service.TryGet(id, out _);
    }
}
