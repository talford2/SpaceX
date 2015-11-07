using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelOfDetail : MonoBehaviour
{
	public List<DistanceConfig> Distances;

	public float Interval = 1f;
	private float _coolDown = 0f;

	private void Awake()
	{
		_coolDown = UnityEngine.Random.Range(0, Interval);
	}

	private void Update()
	{
		_coolDown += Time.deltaTime;

		if (_coolDown < 0)
		{
			var toCamera = transform.position - Camera.main.transform.position;
			foreach (var distConfig in Distances)
			{
				if (toCamera.sqrMagnitude > distConfig.Distance * distConfig.Distance)
				{
					foreach (var enableObj in distConfig.Enabled)
					{
						enableObj.SetActive(true);
					}
					foreach (var disableObj in distConfig.Disabled)
					{
						disableObj.SetActive(false);
					}
				}
			}
			_coolDown = Interval;
		}
	}
}

[Serializable]
public class DistanceConfig
{
	public float Distance;

	public List<GameObject> Enabled;

	public List<GameObject> Disabled;
}