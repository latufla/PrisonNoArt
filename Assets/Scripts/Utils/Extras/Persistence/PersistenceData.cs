using System.IO;
using UnityEngine;


namespace Honeylab.Utils.Persistence
{
    [CreateAssetMenu(fileName = DataUtil.DefaultFileNamePrefix + nameof(PersistenceData),
        menuName = DataUtil.MenuNamePrefix + "Persistence Data")]
    public class PersistenceData : ScriptableObject
    {
        [SerializeField] private string _file;
        [SerializeField] private string _extension;
        [SerializeField] private string _namespace;


        public string GetFilePath() => Path.Combine(Application.persistentDataPath, _file);
        public string GetExtension() => _extension;
        public string GetNamespace() => _namespace;
    }
}
