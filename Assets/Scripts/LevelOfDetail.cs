using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelOfDetail : MonoBehaviour
{
	public List<DistanceConfig> Distances;

	//public float Distance;
	//public List<MonoBehaviour> SwitchOn;
	//public List<MonoBehaviour> SwitchOff;

	//private float distSquared;

	//private void Awake()
	//{
	//	distSquared = Distance * Distance;
	//}

	//private void Update()
	//{
	//	var toCamera = transform.position - Camera.main.transform.position;
	//	if (toCamera.sqrMagnitude > distSquared)
	//	{
	//		Switch();
	//	}
	//}

	//private void Switch()
	//{
	//	foreach (var onComponent in SwitchOn)
	//	{
	//		onComponent.enabled = true;
	//	}
	//	foreach (var offComponent in SwitchOff)
	//	{
	//		offComponent.enabled = false;
	//	}
	//}
}

[Serializable]
public class DistanceConfig
{
	public float Distance;

	public List<GameObject> Enabled;

	public List<GameObject> Disabled;
}