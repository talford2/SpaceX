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

    public static GameObject GetAvailable(GameObject obj)
    {
        var instance = _resourcePools[obj].GetAvailable();
        if (instance == null)
            Debug.LogWarning("Insufficient " + obj.name + "s in resource pool.");
        return instance;
    }
}
