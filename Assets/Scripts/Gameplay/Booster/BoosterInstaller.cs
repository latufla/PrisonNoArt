using Honeylab.Utils.Persistence;
using UnityEngine;
using Zenject;


namespace Honeylab.Gameplay.Booster
{
    public class BoosterInstaller : MonoInstaller
    {
        [SerializeField] private PersistenceId _id;
        [SerializeField] private string _configId;


        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<BoosterService>()
                .AsSingle()
                .WithArguments(_id, _configId);
        }
    }
}
