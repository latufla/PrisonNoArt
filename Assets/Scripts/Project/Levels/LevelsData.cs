using Honeylab.Utils.Data;
using System.Collections.Generic;
using UnityEngine;


namespace Honeylab.Project.Levels
{
    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(LevelsData),
        menuName = DataUtil.MenuNamePrefix + "Levels Data")]
    public class LevelsData : ScriptableObject
    {
        [SerializeField] private List<LevelData> _levels;


        public IReadOnlyList<LevelData> GetLevels() => _levels.AsReadOnly();
    }
}
