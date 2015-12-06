﻿using System.Collections.Generic;
using UnityEngine;

public class DistantScaling : MonoBehaviour
{
	private Shiftable _shiftable;
	private float _thresholdDistance;
	private float _thresholdDistanceSquared;
	private float _distantScale;

	private int _distantLayer;
	private int _defaultLayer;

	// Crappy solution
	public float LineRendererWidth = 35f;
	public List<LineRenderer> LineRenderers;

    private bool isDistant;
    private bool lastIsDistant;

    public List<Targetable> TargetableObjects;

	private void Awake()
	{
		_shiftable = GetComponent<Shiftable>();
		_thresholdDistance = 1000f;
		_distantScale = 1000f;

		_thresholdDistanceSquared = _thresholdDistance * _thresholdDistance;
		_distantLayer = LayerMask.NameToLayer("Distant");
		_defaultLayer = LayerMask.NameToLayer("Default");
	}

	private void Start()
	{
		Universe.Current.ViewPort.OnMove += UpdatePositionAndScale;
	}

	private void UpdatePositionAndScale()
	{
		// Scaling
		var worldDestination = Universe.Current.GetWorldPosition(_shiftable.UniversePosition);
        var toCamera = worldDestination - Universe.Current.ViewPort.AttachedCamera.transform.position;

		if (toCamera.sqrMagnitude > _thresholdDistanceSquared)
		{
		    isDistant = true;
			var distance = toCamera.magnitude;

			// This will only be able to scale objects of a radius up to the _thresholdDistance.
			// Keep the scaled object 1 unit in front of the camera!
			var scaledDistance = Mathf.Abs(distance - _thresholdDistance - 1f) / _distantScale + 1f; // + _thresholdDistance;

			var scale = Mathf.Clamp(scaledDistance / distance, 0f, 1f);
			transform.localScale = new Vector3(scale, scale, scale);

			// Crappy solution
			var lineRenderWidth = scale * LineRendererWidth;
			foreach (var lineRenderer in LineRenderers)
			{
				lineRenderer.SetWidth(lineRenderWidth, lineRenderWidth);
			}

			// Positioning
            transform.position = Universe.Current.ViewPort.AttachedCamera.transform.position + toCamera.normalized * scaledDistance;
			if (gameObject.layer != _distantLayer)
				Utility.SetLayerRecursively(gameObject, _distantLayer);
		}
		else
		{
		    isDistant = false;
			transform.localScale = new Vector3(1f, 1f, 1f);

			// Crappy solution
			foreach (var lineRenderer in LineRenderers)
			{
				lineRenderer.SetWidth(LineRendererWidth, LineRendererWidth);
			}

			transform.position = worldDestination;
			if (gameObject.layer != _defaultLayer)
				Utility.SetLayerRecursively(gameObject, _defaultLayer);
		}

	    if (isDistant != lastIsDistant)
	    {
	        if (isDistant)
	        {
	            ManageTargtables();
	        }
	        else
	        {
	            ManageTargtables();
	        }
	    }
	    lastIsDistant = isDistant;

        ManageTargtables();
	}

    private void ManageTargtables()
    {
        if (isDistant)
        {
            foreach (var targetable in TargetableObjects)
            {
                if (targetable != null)
                {
                    targetable.SetEnabled(false);
                    targetable.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            foreach (var targetable in TargetableObjects)
            {
                if (targetable != null)
                {
                    targetable.gameObject.SetActive(true);
                    targetable.SetEnabled(true);
                }
            }
        }
    }
}