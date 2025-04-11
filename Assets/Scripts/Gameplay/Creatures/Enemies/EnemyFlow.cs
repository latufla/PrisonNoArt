using Cysharp.Threading.Tasks;
using Honeylab.Gameplay.Creatures.Configs;
using Honeylab.Gameplay.World;
using Honeylab.Utils.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


namespace Honeylab.Gameplay.Creatures
{
    public struct EnemyArgs
    {
        public WorldObjectId SpawnerId;
        public EnemySkinId SkinId;
        public Transform SpawnPoint;
        public string ConfigId;
    }


    public class EnemyFlow : WorldObjectFlow
    {
        [SerializeField] private ConfigIdProvider _configId;
        [SerializeField] private CreatureMotion _motion;
        [SerializeField] private List<EnemyWorkBase> _works;
        [SerializeField] private float _checkWorksInterval = 0.1f;

        private EnemyArgs _args;


        public EnemySkinId SkinId => _args.SkinId;
        public Transform SpawnPoint => _args.SpawnPoint;
        public EnemyConfig Config { get; private set; }
        public string ConfigId => _configId.Id;
        private Die _die;

        public WorldObjectId GetSpawnerId => _args.SpawnerId;


        protected override void OnInit()
        {
            IConfigsService configs = Resolve<IConfigsService>();
            _configId.SetId(_args.ConfigId);

            Config = configs.Get<EnemyConfig>(_configId.Id);
            _motion.SetSpeed(Config.Speed);
        }


        public void SetArgs(EnemyArgs args)
        {
            _args = args;
        }


        protected override async UniTask OnRunAsync(CancellationToken ct)
        {
            _die = Get<Die>();
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_checkWorksInterval), cancellationToken: ct);

                var works = GetAvailableWorks();
                if (works.Count == 0 && _die != null)
                {
                    await UniTask.WaitUntil(() => _die.IsDead, cancellationToken: ct);
                    break;
                }

                _works.ForEach(it => it.gameObject.SetActive(false));

                EnemyWorkBase work = works.First();
                work.gameObject.SetActive(true);
                await work.TryExecute(ct);
            }
        }


        private List<EnemyWorkBase> GetAvailableWorks() => _works.Where(it => it.CanExecute()).ToList();
    }
}
