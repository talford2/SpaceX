using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOfDetail : MonoBehaviour
{
	public bool Running = true;

	public float Interval = 1f;
	public List<DistanceConfig> Distances;


	private void Awake()
	{
		if (Running)
		{
			StartCoroutine(UpdateLod(UnityEngine.Random.Range(0f, Interval)));
		}
	}

	private IEnumerator UpdateLod(float delay)
	{
		yield return new WaitForSeconds(delay);

		var toCamera = transform.position - FollowCamera.Current.transform.position;
		var dotProd = Vector3.Dot(toCamera, FollowCamera.Current.transform.forward);
		if (dotProd > 0f)
		{
			//var toCamera = transform.position - Camera.main.transform.position;

			var curDisConfig = Distances[0];

			foreach (var distConfig in Distances)
			{
				if (toCamera.sqrMagnitude > distConfig.Distance * distConfig.Distance)
				{
					curDisConfig = distConfig;
				}
			}

			foreach (var enableObj in curDisConfig.Enabled)
			{
				enableObj.SetActive(true);
			}
			foreach (var disableObj in curDisConfig.Disabled)
			{
				disableObj.SetActive(false);
			}
		}

		StartCoroutine(UpdateLod(Interval));
	}
}

[Serializable]
public class DistanceConfig
{
	public float Distance;

	public List<GameObject> Enabled;

	public List<GameObject> Disabled;
}