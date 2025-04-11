using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Player;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Extensions;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using Zenject;


namespace Honeylab.Project.Cheats
{
    public class PlayerCheat : IInitializable, IDisposable
    {
        private readonly WorldObjectsService _world;
        private readonly CancellationTokenSource _cts = new();


        public PlayerCheat(WorldObjectsService world)
        {
            _world = world;
        }


        public void Initialize()
        {
            RunAsync(_cts.Token).Forget();
        }


        public void Dispose()
        {
            _cts.CancelThenDispose();
        }


        private async UniTask RunAsync(CancellationToken ct)
        {
            PlayerFlow player = null;
            await UniTask.WaitUntil(() =>
                {
                    player = _world.GetObjects<PlayerFlow>().FirstOrDefault();
                    return player != null;
                },
                cancellationToken: ct);

            float initialSpeed = player.Motion.GetSpeed();
            while (true)
            {
                await UniTask.Yield(ct);

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    player.Motion.SetSpeed(20.0f);
                }
                else
                {
                    player.Motion.SetSpeed(initialSpeed);
                }
            }
        }
    }
}
