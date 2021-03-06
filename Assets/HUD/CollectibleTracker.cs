﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleTracker : Tracker
{
	public Sprite ArrowSprite;
    public GameObject TrackerPlanePrefab;
    public float TrackerPlaneScale = 100f;

    public Color TrackerColor = Color.white;
    public float MaxDistance = 2000f;

	private Vector2 _screenCentre;
	private Rect _screenBounds;
	private Image _imageInstance;

	private Sprite _arrowSprite;
    private GameObject _trackerPlaneInstance;
    private MeshRenderer _trackerPlaneRenderer;

	private bool _isVisible;
	private bool _isFading;

	private float _width;

	private float _fadeStep = 0.02f;

    private float _maxDistanceSquared;
    private float _lastDistanceSquared;

	private void Awake()
	{
		_screenCentre = new Vector3(0.5f * Screen.width, 0.5f * Screen.height);
		var boundaryPadding = 20f;
		_screenBounds = new Rect(boundaryPadding, boundaryPadding, Screen.width - 2f * boundaryPadding, Screen.height - 2f * boundaryPadding);
        _maxDistanceSquared = MaxDistance * MaxDistance;
	}

    public override Image CreateInstance()
    {
        var trackerObj = new GameObject(string.Format("{0}_Tracker", transform.name));
        var trackerImg = trackerObj.AddComponent<Image>();

        trackerImg.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        trackerImg.color = new Color(1f, 1f, 1f, 1f);

        _arrowSprite = ArrowSprite;

        trackerImg.SetNativeSize();

        _imageInstance = trackerImg;
        _imageInstance.color = TrackerColor;
        _width = _imageInstance.rectTransform.rect.width;

        _trackerPlaneInstance = Instantiate(TrackerPlanePrefab);
        _trackerPlaneRenderer = _trackerPlaneInstance.GetComponent<MeshRenderer>();

        _isVisible = false;
        SetAlpha(0f);
        _lastDistanceSquared = Mathf.Infinity;

        return trackerImg;
    }

    public override void UpdateInstance()
    {
        var distanceSquared = (transform.position - Universe.Current.ViewPort.transform.position).sqrMagnitude;
        if (_lastDistanceSquared < _maxDistanceSquared)
        {
            if (distanceSquared > _maxDistanceSquared)
            {
                TriggerFadeOut();
            }
        }

        if (_lastDistanceSquared > _maxDistanceSquared)
        {
            if (distanceSquared < _maxDistanceSquared)
            {
                TriggerFadeIn();
            }
        }

        var screenPosition = Universe.Current.ViewPort.AttachedCamera.WorldToScreenPoint(transform.position);
        if (screenPosition.z < 0f)
        {
            screenPosition *= -1f;
            screenPosition = (screenPosition - new Vector3(_screenCentre.x, _screenCentre.y, 0f)) * Utility.ProjectOffscreenLength + new Vector3(_screenCentre.x, _screenCentre.y, 0f);
        }
        screenPosition.z = 0f;

        // Update TrackerPlane
        var cameraPlane = new Plane(Universe.Current.ViewPort.transform.forward, Universe.Current.ViewPort.transform.position + 5f * Universe.Current.ViewPort.transform.forward);
        float planeDist;
        var toCamRay = new Ray(transform.position, Universe.Current.ViewPort.transform.position - transform.position);
        var dotCam = Vector3.Dot(transform.position - Universe.Current.ViewPort.transform.position, Universe.Current.ViewPort.transform.forward);
        if (dotCam > 0f)
        {
            cameraPlane.Raycast(toCamRay, out planeDist);
            _trackerPlaneInstance.transform.position = toCamRay.GetPoint(planeDist);

            _trackerPlaneInstance.transform.localRotation = Quaternion.LookRotation(-Universe.Current.ViewPort.transform.forward, Universe.Current.ViewPort.transform.up) * Quaternion.Euler(0, 0, 45f);

            _trackerPlaneRenderer.enabled = true;

            var dist = (Universe.Current.ViewPort.transform.position - transform.position).magnitude;
            var frac = TrackerPlaneScale / dist;
            _trackerPlaneRenderer.material.SetFloat("_Expand", Mathf.Clamp(frac, 0.18f, 1f));
        }
        else
        {
            _trackerPlaneRenderer.enabled = false;
        }

        if (_screenBounds.Contains(screenPosition))
        {
            _imageInstance.rectTransform.localPosition = screenPosition - new Vector3(_screenCentre.x, _screenCentre.y, 0f);
            _imageInstance.rectTransform.localRotation = Quaternion.identity;
            _imageInstance.enabled = false;
        }
        else
        {
            _imageInstance.sprite = _arrowSprite;
            _imageInstance.rectTransform.localPosition = Utility.GetBoundsIntersection(screenPosition, _screenBounds);
            _imageInstance.rectTransform.localRotation = Quaternion.Euler(0f, 0f, GetScreenAngle(screenPosition));
            _imageInstance.rectTransform.sizeDelta = 64f * Vector2.one;
            _imageInstance.enabled = true;
        }
        _lastDistanceSquared = distanceSquared;

    }

    public void TriggerFadeIn(float time = 0.5f)
	{
		if (!_isFading && !_isVisible)
		{
			StartCoroutine(FadeIn(time));
		}
	}

	public void TriggerFadeOut(float time = 0.5f)
	{
		if (!_isFading && _isVisible)
		{
			StartCoroutine(FadeOut(time));
		}
	}

    public void SetColor(Color color)
    {
        TrackerColor = color;
        if (_imageInstance != null)
            _imageInstance.color = color;
        if (_trackerPlaneRenderer != null)
            _trackerPlaneRenderer.material.color = color;
    }

	public void SetAlpha(float alpha)
	{
		SetColor(Utility.SetColorAlpha(TrackerColor, alpha));
	}

	public void SetScale(float scale)
	{
		if (_imageInstance != null)
		{
			_imageInstance.rectTransform.sizeDelta = Vector2.one * _width * scale;
		}
	}

	private IEnumerator FadeOut(float duration)
	{
		var start = DateTime.Now;
		var ii = 0;

		var stepFraction = _fadeStep / duration;
		for (var fraction = 1f; fraction >= 0f; fraction -= stepFraction)
		{
			//_imageInstance.color = new Color(TrackerColor.r, TrackerColor.g, TrackerColor.b, fraction);
            SetAlpha(fraction);

			ii++;
			//Debug.Log("Time running: " + DateTime.Now.Subtract(start).TotalSeconds + " (" + ii + ")");

			yield return new WaitForSeconds(_fadeStep);
		}
        //_imageInstance.color = new Color(TrackerColor.r, TrackerColor.g, TrackerColor.b, 0f);
        SetAlpha(0f);
		_isVisible = false;
		_isFading = false;
	}

	private IEnumerator FadeIn(float duration)
	{
		var stepFraction = _fadeStep / duration;
		for (var fraction = 1f; fraction >= 0f; fraction -= stepFraction)
		{
            //_imageInstance.color = new Color(TrackerColor.r, TrackerColor.g, TrackerColor.b, 1f - fraction);
            SetAlpha(1f - fraction);
			yield return new WaitForSeconds(_fadeStep);
		}
        //_imageInstance.color = new Color(TrackerColor.r, TrackerColor.g, TrackerColor.b, 1f);
        SetAlpha(1f);
		_isVisible = true;
		_isFading = false;
	}

	public override void DestroyInstance()
	{
        if (_trackerPlaneInstance != null)
        {
            Destroy(_trackerPlaneInstance);
            Destroy(_trackerPlaneRenderer);
        }
        if (_imageInstance != null)
			Destroy(_imageInstance);
	}

	public override void SetVisible(bool value)
	{
		_imageInstance.enabled = value;
	}

	private float GetScreenAngle(Vector2 point)
	{
		var delta = point - _screenCentre;
		var angle = Mathf.Rad2Deg * Mathf.Atan2(delta.x, -delta.y) + 180f;
		return angle;
	}
}
