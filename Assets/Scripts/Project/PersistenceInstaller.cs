using Honeylab.Utils.Persistence;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Zenject;


namespace Honeylab.Project
{
    public class PersistenceInstaller : MonoInstaller
    {
        [SerializeField] private PersistenceData _persistenceData;


        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PersistenceStartup>()
                .AsSingle();

            LocalPersistenceStorageService sharedStorage = new(_persistenceData.GetFilePath(),
                _persistenceData.GetExtension(),
                _persistenceData.GetNamespace(),
                new[] { Assembly.GetExecutingAssembly() });

            sharedStorage.SetFileNamePostfix("_shared");

            Container.Bind<SharedPersistenceService>()
                .AsSingle()
                .WithArguments(sharedStorage);

            LocalPersistenceStorageService levelStorage = new(_persistenceData.GetFilePath(),
                _persistenceData.GetExtension(),
                _persistenceData.GetNamespace(),
                new[] { Assembly.GetExecutingAssembly() });

            levelStorage.SetFileNamePostfix("_levels");

            Container.Bind<LevelPersistenceService>()
                .AsSingle()
                .WithArguments(levelStorage);

            Container.BindInterfacesAndSelfTo<AppPauseAutoSaveService>()
                .AsSingle();

            #if UNITY_EDITOR
            Container.BindInterfacesAndSelfTo<AppQuitAutoSaveService>()
                .AsSingle();
            #endif

            Container.Bind<IEnumerable<IAutoSaveService>>()
                .FromMethod(ctx => ctx.Container.ResolveAll<IAutoSaveService>())
                .AsSingle();
        }
    }
}
