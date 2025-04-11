using Honeylab.Consumables;
using Honeylab.Gameplay;
using Honeylab.Gameplay.Equipments;
using Honeylab.Gameplay.Ui;
using Honeylab.Gameplay.Weapons;
using Honeylab.Gameplay.World;
using Honeylab.Sounds.Data;
using Honeylab.Utils.Data;
using UnityEngine;


namespace Honeylab.Project.Levels
{
    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(LevelData),
        menuName = DataUtil.MenuNamePrefix + "Level Data")]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private WorldObjectId _id;
        [SerializeField] private string _sceneName;
        [SerializeField] private string _sceneTitle;
        [SerializeField] private ConsumablesData _consumablesData;
        [SerializeField] private WorldObjectsData _worldObjectsData;
        [SerializeField] private EquipmentsData _equipmentsData;
        [SerializeField] private WeaponsData _weaponsData;
        [SerializeField] private SoundsData _soundsData;
        [SerializeField] private string _speedUpConfigId;
        [SerializeField] private AdOfferData _adOfferData;

        public WorldObjectId Id => _id;
        public string SceneName => _sceneName;
        public string SceneTitle => _sceneTitle;
        public ConsumablesData ConsumablesData => _consumablesData;
        public WorldObjectsData WorldObjectsData => _worldObjectsData;
        public EquipmentsData EquipmentsData => _equipmentsData;
        public WeaponsData WeaponsData => _weaponsData;
        public SoundsData SoundsData => _soundsData;
        public string SpeedUpConfigId => _speedUpConfigId;
        public AdOfferData AdOfferData => _adOfferData;
    }
}
