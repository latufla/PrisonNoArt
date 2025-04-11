using Honeylab.Gameplay.Pools;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Pool;


namespace Honeylab.Gameplay.Weapons
{
    public class WeaponsFactory
    {
        private readonly WorldObjectsService _world;
        private readonly WeaponsPool _pool;


        public WeaponsFactory(WorldObjectsService world, GameplayPoolsService pools)
        {
            _world = world;
            _pool = pools.Get<WeaponsPool>();
        }


        public WeaponFlow Create(WorldObjectId id,
            IWeaponAgent agent,
            int fixedUpgradeLevel = -1)
        {
            IGameObjectPool pool = _pool.Get(id);
            WeaponFlow weapon = pool.PopWithComponent<WeaponFlow>(id);
            _world.AddObject(weapon);

            weapon.SetAgent(agent);

            _world.RunObject(weapon);

            if (fixedUpgradeLevel >= 0)
            {
                weapon.SetFixedUpgradeLevel(fixedUpgradeLevel);
            }

            return weapon;
        }


        public void Destroy(WeaponFlow weapon)
        {
            _world.RemoveObject(weapon);

            weapon.SetAgent(null);

            IGameObjectPool pool = _pool.Get(weapon.Id);
            pool.Push(weapon.gameObject);
        }
    }
}
