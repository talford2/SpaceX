using System.Collections.Generic;
using UnityEngine;

public class DistantScaling : MonoBehaviour
{
	private float _thresholdDistance;
	private float _thresholdDistanceSquared;
	private float _distantScale;

	// Crappy solution
    public Shiftable FocusShiftable;
	public float LineRendererWidth = 35f;
	public List<LineRenderer> LineRenderers;

    private bool _isDistant;
    private bool _lastIsDistant;

    public GameObject DistantObject;
    public GameObject NearObject;

	private void Awake()
	{
		_thresholdDistance = 1000f;
		_distantScale = 1000f;

		_thresholdDistanceSquared = _thresholdDistance * _thresholdDistance;
	}

	private void Start()
	{
		Universe.Current.ViewPort.OnMove += UpdatePositionAndScale;
	}

	private void UpdatePositionAndScale()
	{
	    if (FocusShiftable == null)
	        return;
		// Scaling
		var worldDestination = Universe.Current.GetWorldPosition(FocusShiftable.UniversePosition);
        var toCamera = worldDestination - Universe.Current.ViewPort.AttachedCamera.transform.position;

		if (toCamera.sqrMagnitude > _thresholdDistanceSquared)
		{
		    if (!_isDistant)
		    {
                DistantObject.SetActive(true);
                NearObject.SetActive(false);
		        _isDistant = true;
		    }
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
		}
		else
		{
		    if (_isDistant)
		    {
                DistantObject.SetActive(false);
                NearObject.SetActive(true);
                _isDistant = false;
		    }
		    transform.localScale = new Vector3(1f, 1f, 1f);

			// Crappy solution
			foreach (var lineRenderer in LineRenderers)
			{
				lineRenderer.SetWidth(LineRendererWidth, LineRendererWidth);
			}

			transform.position = worldDestination;
		}
	    _lastIsDistant = _isDistant;
	}
}