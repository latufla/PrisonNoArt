using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.World;
using System.Linq;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.Buildings
{
    public class WorldObjectsUnlockBuildingAction : UnlockBuildingActionBase
    {
        [SerializeField] private Transform _root;

        private WorldObjectsService _world;


        protected override void OnInit()
        {
            base.OnInit();

            _world = Flow.Resolve<WorldObjectsService>();

            if (_root == null)
            {
                return;
            }

            if (IsUnlocked())
            {
                var objects = _root.GetComponentsInChildren<WorldObjectFlow>().OrderBy(it => it.Order).ToList();
                objects.ForEach(_world.AddObject);
            }
        }


        protected override UniTask OnUnlockAsync(CancellationToken ct)
        {
            if (_root == null)
            {
                return UniTask.CompletedTask;
            }

            var objects = _root.GetComponentsInChildren<WorldObjectFlow>().OrderBy(it => it.Order).ToList();
            objects.ForEach(_world.AddObject);
            objects.ForEach(_world.RunObject);

            return UniTask.CompletedTask;
        }
    }
}
