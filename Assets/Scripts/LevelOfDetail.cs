﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelOfDetail : MonoBehaviour
{
    public bool Running = true;

    public float Interval = 1f;
    public List<DistanceConfig> Distances;

    public delegate void OnExitLargestDistanceEvent(GameObject instance);

    public OnExitLargestDistanceEvent OnExitLargestDistance;

    public delegate void OnEnterLargestDistanceEvent(GameObject instance);

    public OnEnterLargestDistanceEvent OnEnterLargestDistance;

    private float _largestDistanceSquared;

    private void Awake()
    {
        var largestDistance = Distances.OrderByDescending(d => d.Distance).First().Distance;
        _largestDistanceSquared = largestDistance*largestDistance;

        if (Running)
        {
            StartCoroutine(UpdateLod(UnityEngine.Random.Range(0f, Interval)));
        }
    }

    private IEnumerator UpdateLod(float delay)
    {
        yield return new WaitForSeconds(delay);

        var toCamera = transform.position - Universe.Current.ViewPort.transform.position;

        var curDisConfig = Distances[0];

        foreach (var distConfig in Distances)
        {
            if (toCamera.sqrMagnitude > distConfig.Distance*distConfig.Distance)
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


        if (toCamera.sqrMagnitude > _largestDistanceSquared)
        {
            if (OnExitLargestDistance != null)
                OnExitLargestDistance(gameObject);
        }
        else
        {
            if (OnEnterLargestDistance != null)
                OnEnterLargestDistance(gameObject);
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