using UnityEngine;
using System.Collections.Generic;

public class ResourcePool : MonoBehaviour
{
	public GameObject Prefab;
	public int PoolSize;

	private List<GameObject> _pool;
    private List<ResourcePoolItem> _poolItems;

    private void Awake()
	{
		CreatePool(Prefab, PoolSize);
        ResourcePoolManager.AddResourcePool(this);
	}

	public void CreatePool(GameObject prefab, int count)
	{
		_pool = new List<GameObject>();
        _poolItems = new List<ResourcePoolItem>();
		for (var i = 0; i < count; i++)
		{
            AddItem(prefab);
        }
    }

    public GameObject GetAvailable(Vector3 position, Quaternion rotation)
    {
        for (var i = 0; i < _poolItems.Count; i++)
        {
            var poolItem = _poolItems[i];
            if (poolItem.IsAvailable)
            {
                var instance = _pool[i];
                instance.transform.position = position;
                instance.transform.rotation = rotation;
                if (poolItem.OnGetAvaiable != null)
                    poolItem.OnGetAvaiable();
                return instance;
            }
        }
        PoolSize++;
        return AddItem(Prefab);
    }

    private GameObject AddItem(GameObject prefab)
    {
        var instance = Object.Instantiate(prefab);
        var poolItem = instance.AddComponent<ResourcePoolItem>();
        poolItem.IsAvailable = true;
        instance.transform.SetParent(transform);
        _pool.Add(instance);
        _poolItems.Add(poolItem);
        return instance;
    }
}