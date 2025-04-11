using Honeylab.Gameplay.World;
using Honeylab.Utils.Configs;
using UnityEngine;


namespace Honeylab.Gameplay.Buildings
{
    public class LevelLeaveBuildingFlow : WorldObjectFlow
    {
        [SerializeField] private ConfigIdProvider _configId;


        public LevelLeaveBuildingConfig Config { get; private set; }


        protected override void OnInit()
        {
            IConfigsService configs = Resolve<IConfigsService>();
            Config = configs.Get<LevelLeaveBuildingConfig>(_configId.Id);
        }
    }
}
