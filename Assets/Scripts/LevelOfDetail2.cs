using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelOfDetail2 : MonoBehaviour
{
	public float Interval = 1f;
	public List<DistanceConfig> Distances;

	private void Awake()
	{
		StartCoroutine(UpdateLod(Random.Range(0f, Interval)));
	}

	private IEnumerator UpdateLod(float delay)
	{
		yield return new WaitForSeconds(delay);
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
		StartCoroutine(UpdateLod(Interval));
	}
}

//[Serializable]
//public class DistanceConfig
//{
//	public float Distance;

//	public List<GameObject> Enabled;

//	public List<GameObject> Disabled;
//}