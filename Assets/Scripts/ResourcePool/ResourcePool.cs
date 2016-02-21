using UnityEngine;
using System.Collections.Generic;

public class ResourcePool : MonoBehaviour
{
	public GameObject Prefab;
	public int PoolSize;

	private static ResourcePool _current;

	public static ResourcePool Current { get { return _current; } }

	private static List<GameObject> _pool;

	private void Awake()
	{
		_current = this;
		CreatePool(Prefab, PoolSize);
	}

	public void CreatePool(GameObject item, int count)
	{
		_pool = new List<GameObject>();
		for (var i = 0; i < count; i++)
		{
			var instance = Object.Instantiate(item);
			var poolItem = instance.AddComponent<ResourcePoolItem>();
			poolItem.IsAvailable = true;
			instance.transform.SetParent(transform);
			_pool.Add(instance);
		}
	}

	public GameObject GetAvailable()
	{
		foreach (var item in _pool)
		{
			var poolItem = item.GetComponent<ResourcePoolItem>();
			if (poolItem != null)
			{
				if (poolItem.IsAvailable)
				{
					// Reset instance here.
					var expl = item.GetComponent<Explosion>();
					expl.Reset();
					return item;
				}
			}
		}
		Debug.Log("No Available Items!");
		return null;
	}
}