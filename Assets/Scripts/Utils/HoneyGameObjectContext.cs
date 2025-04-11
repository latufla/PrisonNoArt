using UnityEngine;
using Zenject;


namespace Honeylab.Utils
{
    public class HoneyGameObjectContext : GameObjectContext
    {
        [SerializeField] private Transform _installersRoot;


        protected override void RunInternal()
        {
            Installers = _installersRoot.GetComponentsInChildren<MonoInstaller>();

            base.RunInternal();
        }
    }
}
