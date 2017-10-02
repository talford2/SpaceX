using UnityEngine;
using System.Collections.Generic;

public class ResourcePool : MonoBehaviour
{
	public GameObject Prefab;
	public int PoolSize;

    private bool _isInitialized;
	private List<GameObject> _pool;
	private List<ResourcePoolItem> _poolItems;

    private void Awake()
    {
        if (Prefab != null && PoolSize > 0)
            Initialize(Prefab, PoolSize);
    }

    public void Initialize(GameObject prefab, int poolSize)
    {
        if (!_isInitialized)
        {
            Prefab = prefab;
            PoolSize = poolSize;
            CreatePool(prefab, poolSize);
            ResourcePoolManager.AddResourcePool(this);
            _isInitialized = true;
        }
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

				poolItem.IsAvailable = false;
				return instance;
			}
		}
		PoolSize++;
		return AddItem(Prefab);
	}

    public GameObject GetAvailable(Transform parentTransform)
    {
        var instance = GetAvailable(parentTransform.position, parentTransform.rotation);
        instance.transform.SetParent(parentTransform);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;
        return instance;
    }

	private GameObject AddItem(GameObject prefab)
	{
		var instance = Object.Instantiate(prefab);
		var poolItem = instance.AddComponent<ResourcePoolItem>();

		// if the pool item has a resetter we need to set the resetters pool item
		var resourceResetter = instance.GetComponent<ResourceReseter>();
		if (resourceResetter != null)
		{
			resourceResetter.ResourcePoolItem = poolItem;
		}

		poolItem.IsAvailable = true;
		instance.transform.SetParent(transform);
		_pool.Add(instance);
		_poolItems.Add(poolItem);
		return instance;
	}

    private void OnDestroy()
    {
        ResourcePoolManager.RemoveResourcePool(this);
        foreach(var instance in _pool)
        {
            Destroy(instance);
        }
        _pool.Clear();
        _poolItems.Clear();
    }
}