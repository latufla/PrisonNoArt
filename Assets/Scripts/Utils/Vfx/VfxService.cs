using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Pools;
using Honeylab.Utils.Pool;
using System;
using UnityEngine;


namespace Honeylab.Utils.Vfx
{
    public class VfxService
    {
        private readonly VfxPool _pool;


        protected VfxService(GameplayPoolsService pools)
        {
            _pool = pools.Get<VfxPool>();
        }


        public async UniTask PlayOnceAsync(VfxId id, Vector3 position, Quaternion rotation)
        {
            GameObject vfxGo = Play(id, position, rotation);

            Vfx vfx = vfxGo.GetComponent<Vfx>();
            await UniTask.Delay(TimeSpan.FromSeconds(vfx.Duration));

            Stop(id, vfxGo);
        }


        public async UniTask PlayOnceAsync(VfxId id, Transform anchor, Transform parent, float delay = 0.0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            GameObject vfxGo = Play(id, anchor.position, anchor.rotation);
            vfxGo.transform.parent = parent;

            Vfx vfx = vfxGo.GetComponent<Vfx>();
            await UniTask.Delay(TimeSpan.FromSeconds(vfx.Duration));

            Stop(id, vfxGo);
        }


        public async UniTask PlayOnceAsync(VfxId id, Transform anchor, float delay = 0.0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            GameObject vfxGo = Play(id, anchor.position, anchor.rotation);

            Vfx vfx = vfxGo.GetComponent<Vfx>();
            await UniTask.Delay(TimeSpan.FromSeconds(vfx.Duration));

            Stop(id, vfxGo);
        }


        public GameObject Play(VfxId id, Vector3 position, Quaternion rotation)
        {
            Vfx vfx = PlayVfx(id, position, rotation);
            return vfx.gameObject;
        }


        public void Stop(VfxId id, GameObject vfxGo)
        {
            if (vfxGo == null)
            {
                return;
            }

            Vfx vfx = vfxGo.GetComponent<Vfx>();
            vfx.Stop();
            _pool.Push(id, vfxGo);
        }


        private Vfx PlayVfx(VfxId id, Vector3 position, Quaternion rotation)
        {
            GameObject vfxGo = _pool.Pop(id);
            if (vfxGo == null)
            {
                throw new Exception($"No vfx with id {id}");
            }

            Transform vfxTransform = vfxGo.transform;
            vfxTransform.position = position;
            vfxTransform.rotation = rotation;

            Vfx vfx = vfxGo.GetComponent<Vfx>();

            if (vfx == null)
            {
                throw new Exception($"No vfx component on {vfxGo.name} with id {id}");
            }

            vfx.Play();
            return vfx;
        }
    }
}
