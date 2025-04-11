using UnityEngine;


namespace Honeylab.Utils.Configs
{
    public class ConfigIdProvider : MonoBehaviour
    {
        [SerializeField] private string _id;


        public string Id => _id;


        public void SetId(string newId)
        {
            _id = newId;
        }
    }
}
