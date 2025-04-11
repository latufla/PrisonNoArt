using Honeylab.Project.Levels;
using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.Startup
{
    public class LevelStartupInstaller : MonoInstaller
    {
        [SerializeField] private LevelData _data;


        public override void InstallBindings()
        {
            Container.BindInstance(_data.ConsumablesData);
            Container.BindInstance(_data.WorldObjectsData);
            Container.BindInstance(_data.EquipmentsData);
            Container.BindInstance(_data.WeaponsData);
            Container.BindInstance(_data.SoundsData);

            Container.BindInterfacesAndSelfTo<LevelStartup>()
                .AsSingle();
        }
    }
}
