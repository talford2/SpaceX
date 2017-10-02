using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePoolCollection : MonoBehaviour
{
    public List<ResourcePoolDefinition> Resources;

    private void Awake()
    {
        foreach (var resource in Resources)
        {
            var container = new GameObject(string.Format("{0} Pool", resource.Prefab.name));
            container.transform.SetParent(transform);
            container.AddComponent<ResourcePool>().Initialize(resource.Prefab, resource.PoolSize);
        }
    }

    [Serializable]
    public class ResourcePoolDefinition
    {
        public GameObject Prefab;
        public int PoolSize;
    }
}
