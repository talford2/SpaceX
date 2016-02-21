using UnityEngine;
using System.Collections.Generic;

public class ResourcePool : MonoBehaviour
{
	public GameObject Prefab;
	public int PoolSize;

	private static ResourcePool _current;

	public static ResourcePool Current { get { return _current; } }

	private List<GameObject> _pool;
    private List<ResourcePoolItem> _poolItems;

    private void Awake()
	{
		_current = this;
		CreatePool(Prefab, PoolSize);
	}

	public void CreatePool(GameObject item, int count)
	{
		_pool = new List<GameObject>();
        _poolItems = new List<ResourcePoolItem>();
		for (var i = 0; i < count; i++)
		{
			var instance = Object.Instantiate(item);
			var poolItem = instance.AddComponent<ResourcePoolItem>();
			poolItem.IsAvailable = true;
			instance.transform.SetParent(transform);
			_pool.Add(instance);
            _poolItems.Add(poolItem);
        }
    }

	public GameObject GetAvailable()
	{
        for(var i =0; i< _poolItems.Count; i++)
        {
            var poolItem = _poolItems[i];
            if (poolItem.IsAvailable)
            {
                var instance = _pool[i];
                var expl = instance.GetComponent<Explosion>();
                expl.Reset();
                return instance;
            }
        }
		Debug.Log("No Available Items!");
		return null;
	}
}