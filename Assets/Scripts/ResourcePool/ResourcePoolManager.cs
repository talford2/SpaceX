using UnityEngine;
using System.Collections.Generic;

public class ResourcePoolManager
{
    private static Dictionary<GameObject, ResourcePool> _resourcePools;

    public static void AddResourcePool(ResourcePool pool)
    {
        if (_resourcePools == null)
            _resourcePools = new Dictionary<GameObject, ResourcePool>();
        _resourcePools.Add(pool.Prefab, pool);
    }

    public static GameObject GetAvailable(GameObject obj, Vector3 position, Quaternion rotation)
    {
        if (!_resourcePools.ContainsKey(obj))
            Debug.LogWarningFormat("Resource pool for '{0}' does not exist.", obj);
        return _resourcePools[obj].GetAvailable(position, rotation);
    }

    public static GameObject GetAvailable(GameObject obj, Transform parentTransform)
    {
        return _resourcePools[obj].GetAvailable(parentTransform);
    }

    public static T GetAvailable<T>(GameObject obj, Vector3 position, Quaternion rotation) where T : MonoBehaviour
    {
        return GetAvailable(obj, position, rotation).GetComponent<T>();
    }

    public static T GetAvailable<T>(GameObject obj, Transform parentTransform) where T : MonoBehaviour
    {
        return GetAvailable(obj, parentTransform).GetComponent<T>();
    }

    public static void RemoveResourcePool(ResourcePool pool)
    {
        _resourcePools.Remove(pool.Prefab);
    }
}
