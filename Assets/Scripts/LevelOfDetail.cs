using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelOfDetail : MonoBehaviour
{
	public List<DistanceConfig> Distances;

	private void Awake()
    {
	}

    private void Update()
    {
        var toCamera = transform.position - Camera.main.transform.position;
        foreach (var distConfig in Distances)
        {
            if (toCamera.sqrMagnitude > distConfig.Distance*distConfig.Distance)
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
    }
}

[Serializable]
public class DistanceConfig
{
	public float Distance;

	public List<GameObject> Enabled;

	public List<GameObject> Disabled;
}