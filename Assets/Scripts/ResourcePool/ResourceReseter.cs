using UnityEngine;

public class ResourceReseter : MonoBehaviour
{
	public float Cooldown;

	private float _cool;

	public bool StartOn;

	public ResourcePoolItem ResourcePoolItem
	{
		get; set;
	}

	public void StartCooldown()
	{
		StartOn = true;
	}

	private void Awake()
	{
		ResourcePoolItem = GetComponent<ResourcePoolItem>();
	}

	private void Update()
	{
		if (StartOn)
		{
			if (_cool >= 0)
			{
				_cool -= Time.deltaTime;
			}

			if (_cool < 0)
			{
				ResourcePoolItem.IsAvailable = true;
				_cool = Cooldown;
			}
		}
	}
}
