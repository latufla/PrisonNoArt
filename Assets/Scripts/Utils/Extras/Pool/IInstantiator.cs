using UnityEngine;


namespace Honeylab.Utils.Pool
{
    public interface IInstantiator
    {
        GameObject Instantiate(GameObject prefab, Transform parent);
    }
}
