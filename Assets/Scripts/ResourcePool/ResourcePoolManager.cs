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
        return _resourcePools[obj].GetAvailable(position, rotation);
    }

    public static GameObject GetAvailable(GameObject obj, Transform parentTransform)
    {
        return _resourcePools[obj].GetAvailable(parentTransform);
    }

    public static void RemoveResourcePool(ResourcePool pool)
    {
        _resourcePools.Remove(pool.Prefab);
    }
}
