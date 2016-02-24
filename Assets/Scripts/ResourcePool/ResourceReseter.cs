using UnityEngine;

public class ResourceReseter : MonoBehaviour
{
	public float Cooldown;

	private float _cooldown;

	public ResourcePoolItem ResourcePoolItem
	{
		get; set;
	}

	private void Awake()
	{
		ResourcePoolItem = GetComponent<ResourcePoolItem>();
	}

	private void Update()
	{
		if (!ResourcePoolItem.IsAvailable)
		{
			if (_cooldown >= 0)
			{
				_cooldown -= Time.deltaTime;
			}

			if (_cooldown < 0)
			{
				ResourcePoolItem.IsAvailable = true;
				_cooldown = Cooldown;
			}
		}
	}
}
